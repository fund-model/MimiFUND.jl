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

namespace Fund.Components.ImpactDeathMorbidity
{
    public interface IImpactDeathMorbidityState
    {
        IVariable2Dimensional<Timestep, Region, double> dead { get; }
        IVariable2Dimensional<Timestep, Region, double> yll { get; }
        IVariable2Dimensional<Timestep, Region, double> yld { get; }
        IVariable2Dimensional<Timestep, Region, double> deadcost { get; }
        IVariable2Dimensional<Timestep, Region, double> morbcost { get; }
        IVariable2Dimensional<Timestep, Region, double> vsl { get; }
        IVariable2Dimensional<Timestep, Region, double> vmorb { get; }

        IParameter1Dimensional<Region, double> d2ld { get; }
        IParameter1Dimensional<Region, double> d2ls { get; }
        IParameter1Dimensional<Region, double> d2lm { get; }
        IParameter1Dimensional<Region, double> d2lc { get; }
        IParameter1Dimensional<Region, double> d2lr { get; }
        IParameter1Dimensional<Region, double> d2dd { get; }
        IParameter1Dimensional<Region, double> d2ds { get; }
        IParameter1Dimensional<Region, double> d2dm { get; }
        IParameter1Dimensional<Region, double> d2dc { get; }
        IParameter1Dimensional<Region, double> d2dr { get; }

        IParameter2Dimensional<Timestep, Region, double> dengue { get; }
        IParameter2Dimensional<Timestep, Region, double> schisto { get; }
        IParameter2Dimensional<Timestep, Region, double> malaria { get; }
        IParameter2Dimensional<Timestep, Region, double> cardheat { get; }
        IParameter2Dimensional<Timestep, Region, double> cardcold { get; }
        IParameter2Dimensional<Timestep, Region, double> resp { get; }
        IParameter2Dimensional<Timestep, Region, double> diadead { get; }
        IParameter2Dimensional<Timestep, Region, double> hurrdead { get; }
        IParameter2Dimensional<Timestep, Region, double> extratropicalstormsdead { get; }
        IParameter2Dimensional<Timestep, Region, double> population { get; }
        IParameter2Dimensional<Timestep, Region, double> diasick { get; }
        IParameter2Dimensional<Timestep, Region, double> income { get; }

        double vslbm { get; }
        double vslel { get; }
        double vmorbbm { get; }
        double vmorbel { get; }
        double vslypc0 { get; }
        double vmorbypc0 { get; }
    }


    public class ImpactDeathMorbidityComponent
    {
        public void Run(Clock clock, IImpactDeathMorbidityState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {

            }
            else
            {
                foreach (var r in dimensions.GetValues<Region>())
                {
                    double ypc = s.income[t, r] / s.population[t, r] * 1000.0;

                    s.dead[t, r] = s.dengue[t, r] + s.schisto[t, r] + s.malaria[t, r] + s.cardheat[t, r] + s.cardcold[t, r] + s.resp[t, r] + s.diadead[t, r] + s.hurrdead[t, r] + s.extratropicalstormsdead[t, r];
                    if (s.dead[t, r] > s.population[t, r] * 1000000.0)
                        s.dead[t, r] = s.population[t, r] / 1000000.0;

                    s.yll[t, r] = s.d2ld[r] * s.dengue[t, r] + s.d2ls[r] * s.schisto[t, r] + s.d2lm[r] * s.malaria[t, r] + s.d2lc[r] * s.cardheat[t, r] + s.d2lc[r] * s.cardcold[t, r] + s.d2lr[r] * s.resp[t, r];

                    s.yld[t, r] = s.d2dd[r] * s.dengue[t, r] + s.d2ds[r] * s.schisto[t, r] + s.d2dm[r] * s.malaria[t, r] + s.d2dc[r] * s.cardheat[t, r] + s.d2dc[r] * s.cardcold[t, r] + s.d2dr[r] * s.resp[t, r] + s.diasick[t, r];

                    s.vsl[t, r] = s.vslbm * Math.Pow(ypc / s.vslypc0, s.vslel);
                    s.deadcost[t, r] = s.vsl[t, r] * s.dead[t, r] / 1000000000.0;
                    // deadcost:= vyll*ypc*yll/1000000000;

                    s.vmorb[t, r] = s.vmorbbm * Math.Pow(ypc / s.vmorbypc0, s.vmorbel);
                    s.morbcost[t, r] = s.vmorb[t, r] * s.yld[t, r] / 1000000000.0;

                }
            }
        }
    }
}
