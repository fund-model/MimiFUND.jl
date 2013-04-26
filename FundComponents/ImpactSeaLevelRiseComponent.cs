// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmf;
using Fund.CommonDimensions;

namespace Fund.Components.ImpactSeaLevelRise
{

    public interface IImpactSeaLevelRiseState
    {
        IVariable2Dimensional<Timestep, Region, double> wetval { get; }
        IVariable2Dimensional<Timestep, Region, double> wetlandloss { get; }
        IVariable2Dimensional<Timestep, Region, double> cumwetlandloss { get; }
        IVariable2Dimensional<Timestep, Region, double> wetlandgrowth { get; }
        IVariable2Dimensional<Timestep, Region, double> wetcost { get; }


        IVariable2Dimensional<Timestep, Region, double> dryval { get; }
        IVariable2Dimensional<Timestep, Region, double> landloss { get; }
        IVariable2Dimensional<Timestep, Region, double> cumlandloss { get; }
        IVariable2Dimensional<Timestep, Region, double> drycost { get; }

        IVariable2Dimensional<Timestep, Region, double> npprotcost { get; }
        IVariable2Dimensional<Timestep, Region, double> npwetcost { get; }
        IVariable2Dimensional<Timestep, Region, double> npdrycost { get; }

        IVariable2Dimensional<Timestep, Region, double> protlev { get; }
        IVariable2Dimensional<Timestep, Region, double> protcost { get; }

        IVariable2Dimensional<Timestep, Region, double> enter { get; }
        IVariable2Dimensional<Timestep, Region, double> leave { get; }
        IVariable2Dimensional<Timestep, Region, double> entercost { get; }
        IVariable2Dimensional<Timestep, Region, double> leavecost { get; }

        IVariable2Dimensional<Region, Region, double> imigrate { get; }

        double incdens { get; }
        double emcst { get; }
        double immcst { get; }
        double dvydl { get; }
        double wvel { get; }
        double wvbm { get; }
        double slrwvpopdens0 { get; }
        double wvpdl { get; }
        double wvsl { get; }
        double dvbm { get; }
        double slrwvypc0 { get; }

        IParameter1Dimensional<Region, double> pc { get; }
        IParameter1Dimensional<Region, double> slrprtp { get; }
        IParameter1Dimensional<Region, double> wmbm { get; }
        IParameter1Dimensional<Region, double> dlbm { get; }
        IParameter1Dimensional<Region, double> drylandlossparam { get; }
        IParameter1Dimensional<Region, double> wlbm { get; }
        IParameter1Dimensional<Region, double> coastpd { get; }
        IParameter1Dimensional<Region, double> wetmax { get; }
        IParameter1Dimensional<Region, double> wetland90 { get; }
        IParameter1Dimensional<Region, double> maxlandloss { get; }

        IParameter1Dimensional<Timestep, double> sea { get; }

        IParameter2Dimensional<Region, Region, double> migrate { get; }

        IParameter2Dimensional<Timestep, Region, double> income { get; }
        IParameter2Dimensional<Timestep, Region, double> population { get; }
        IParameter2Dimensional<Timestep, Region, double> area { get; }
    }

    public class ImpactSeaLevelRiseComponent
    {
        public void Run(Clock clock, IImpactSeaLevelRiseState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                foreach (var r1 in dimensions.GetValues<Region>())
                {
                    foreach (var r2 in dimensions.GetValues<Region>())
                    {
                        double immsumm = 0;
                        foreach (var i in dimensions.GetValues<Region>())
                        {
                            immsumm += s.migrate[i, r1];
                        }
                        s.imigrate[r1, r2] = s.migrate[r2, r1] / immsumm;
                    }

                    var t0 = clock.StartTime;
                    s.landloss[t0, r1] = 0.0;
                    s.cumlandloss[t0, r1] = 0.0;
                    s.cumwetlandloss[t0, r1] = 0.0;
                    s.wetlandgrowth[t0, r1] = 0.0;
                }
            }
            else
            {
                // slr in m/year
                double ds = s.sea[t] - s.sea[t - 1];

                foreach (var r in dimensions.GetValues<Region>())
                {
                    double ypc = s.income[t, r] / s.population[t, r] * 1000.0;
                    double ypcprev = s.income[t - 1, r] / s.population[t - 1, r] * 1000.0;
                    double ypcgrowth = ypc / ypcprev - 1.0;

                    if (t == Timestep.FromYear(1951))
                        ypcgrowth = 0;

                    // Needs to be in $bn per km^2
                    // Income is in billion, area is in km^2
                    double incomedens = s.income[t, r] / s.area[t, r];

                    double incomedensprev = s.income[t - 1, r] / s.area[t - 1, r];

                    double incomedensgrowth = incomedens / incomedensprev - 1.0;

                    // In population/km^2
                    // population is in million, area is in km^2
                    double popdens = s.population[t, r] / s.area[t, r] * 1000000.0;
                    double popdensprev = s.population[t - 1, r] / s.area[t - 1, r] * 1000000.0;
                    double popdensgrowth = popdens / popdensprev - 1.0;

                    // Unit of dryval is $bn/km^2
                    s.dryval[t, r] = s.dvbm * Math.Pow(incomedens / s.incdens, s.dvydl);

                    // Unit of wetval is $bn/km^2
                    s.wetval[t, r] = s.wvbm *
                        Math.Pow(ypc / s.slrwvypc0, s.wvel) *
                        Math.Pow(popdens / s.slrwvpopdens0, s.wvpdl) *
                        Math.Pow((s.wetland90[r] - s.cumwetlandloss[t - 1, r]) / s.wetland90[r], s.wvsl);

                    double potCumLandloss = Math.Min(s.maxlandloss[r], s.dlbm[r] * Math.Pow(s.sea[t], s.drylandlossparam[r]));

                    double potLandloss = potCumLandloss - s.cumlandloss[t - 1, r];

                    // If sea levels fall, no protection is build
                    if (ds < 0)
                    {
                        s.npprotcost[t, r] = 0;
                        s.npwetcost[t, r] = 0;
                        s.npdrycost[t, r] = 0;
                        s.protlev[t, r] = 0;
                    }
                    // If the discount rate < -100% people will not build protection
                    else if ((1.0 + s.slrprtp[r] + ypcgrowth) < 0.0)
                    {
                        s.npprotcost[t, r] = 0;
                        s.npwetcost[t, r] = 0;
                        s.npdrycost[t, r] = 0;
                        s.protlev[t, r] = 0;
                    }
                    // Dryland value is worthless
                    else if (((1.0 + s.dvydl * incomedensgrowth) < 0.0))
                    {
                        s.npprotcost[t, r] = 0;
                        s.npwetcost[t, r] = 0;
                        s.npdrycost[t, r] = 0;
                        s.protlev[t, r] = 0;
                    }
                    // Is protecting the coast infinitly expensive?
                    else if ((1.0 / (1.0 + s.slrprtp[r] + ypcgrowth)) >= 1)
                    {
                        s.npprotcost[t, r] = 0;
                        s.npwetcost[t, r] = 0;
                        s.npdrycost[t, r] = 0;
                        s.protlev[t, r] = 0;
                    }
                    // Is dryland infinitly valuable?
                    else if (((1.0 + s.dvydl * incomedensgrowth) / (1.0 + s.slrprtp[r] + ypcgrowth)) >= 1.0)
                    {
                        s.npprotcost[t, r] = 0;
                        s.npwetcost[t, r] = 0;
                        s.npdrycost[t, r] = 0;
                        s.protlev[t, r] = 1;
                    }
                    // Is wetland infinitly valuable?
                    else if (((1.0 + s.wvel * ypcgrowth + s.wvpdl * popdensgrowth + s.wvsl * s.wetlandgrowth[t - 1, r]) / (1.0 + s.slrprtp[r] + ypcgrowth)) >= 1.0)
                    {
                        s.npprotcost[t, r] = 0;
                        s.npwetcost[t, r] = 0;
                        s.npdrycost[t, r] = 0;
                        s.protlev[t, r] = 0;
                    }
                    else
                    {
                        // NPV of protecting the whole coast
                        // pc is in $bn/m
                        s.npprotcost[t, r] = s.pc[r] * ds * (1.0 + s.slrprtp[r] + ypcgrowth) / (s.slrprtp[r] + ypcgrowth);

                        // NPV of wetland
                        if ((1.0 + s.wvel * ypcgrowth + s.wvpdl * popdensgrowth + s.wvsl * s.wetlandgrowth[t - 1, r]) < 0.0)
                            s.npwetcost[t, r] = 0;
                        else
                            s.npwetcost[t, r] = s.wmbm[r] * ds * s.wetval[t, r] * (1.0 + s.slrprtp[r] + ypcgrowth) / (s.slrprtp[r] + ypcgrowth - s.wvel * ypcgrowth - s.wvpdl * popdensgrowth - s.wvsl * s.wetlandgrowth[t - 1, r]);

                        // NPV of dryland
                        if ((1.0 + s.dvydl * incomedensgrowth) < 0.0)
                            s.npdrycost[t, r] = 0;
                        else
                            s.npdrycost[t, r] = potLandloss * s.dryval[t, r] * (1 + s.slrprtp[r] + ypcgrowth) / (s.slrprtp[r] + ypcgrowth - s.dvydl * incomedensgrowth);

                        // Calculate protection level
                        s.protlev[t, r] = Math.Max(0.0, 1.0 - 0.5 * (s.npprotcost[t, r] + s.npwetcost[t, r]) / s.npdrycost[t, r]);

                        if (s.protlev[t, r] > 1)
                            throw new Exception("protlevel >1 should not happen");
                    }

                    // Calculate actual wetland loss and cost
                    s.wetlandloss[t, r] = Math.Min(
                        s.wlbm[r] * ds + s.protlev[t, r] * s.wmbm[r] * ds,
                        s.wetmax[r] - s.cumwetlandloss[t - 1, r]);

                    s.cumwetlandloss[t, r] = s.cumwetlandloss[t - 1, r] + s.wetlandloss[t, r];

                    // Calculate wetland growth                    
                    s.wetlandgrowth[t, r] = (s.wetland90[r] - s.cumwetlandloss[t, r]) / (s.wetland90[r] - s.cumwetlandloss[t - 1, r]) - 1.0;

                    s.wetcost[t, r] = s.wetval[t, r] * s.wetlandloss[t, r];

                    s.landloss[t, r] = (1.0 - s.protlev[t, r]) * potLandloss;

                    s.cumlandloss[t, r] = s.cumlandloss[t - 1, r] + s.landloss[t, r];
                    s.drycost[t, r] = s.dryval[t, r] * s.landloss[t, r];

                    s.protcost[t, r] = s.protlev[t, r] * s.pc[r] * ds;

                    if (s.landloss[t, r] < 0)
                        s.leave[t, r] = 0;
                    else
                        s.leave[t, r] = s.coastpd[r] * popdens * s.landloss[t, r];

                    s.leavecost[t, r] = s.emcst * ypc * s.leave[t, r] / 1000000000;
                }

                foreach (var destination in dimensions.GetValues<Region>())
                {
                    double enter = 0.0;
                    foreach (var source in dimensions.GetValues<Region>())
                    {
                        enter += s.leave[t, source] * s.imigrate[source, destination];
                    }
                    s.enter[t, destination] = enter;
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    double ypc = s.income[t, r] / s.population[t, r] * 1000.0;
                    s.entercost[t, r] = s.immcst * ypc * s.enter[t, r] / 1000000000;
                }
            }
        }
    }

}
