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

namespace Fund.Components
{

    public interface IEmissionsState
    {
        IVariable2Dimensional<Timestep, Region, double> mitigationcost { get; }
        IVariable2Dimensional<Timestep, Region, double> ch4cost { get; }
        IVariable2Dimensional<Timestep, Region, double> ch4costindollar { get; }
        IVariable2Dimensional<Timestep, Region, double> n2ocost { get; }
        IVariable2Dimensional<Timestep, Region, double> ryg { get; }
        IVariable2Dimensional<Timestep, Region, double> energint { get; }
        IVariable2Dimensional<Timestep, Region, double> emissint { get; }
        IVariable2Dimensional<Timestep, Region, double> emission { get; }
        IVariable2Dimensional<Timestep, Region, double> emissionwithforestry { get; }
        IVariable2Dimensional<Timestep, Region, double> sf6 { get; }
        IVariable2Dimensional<Timestep, Region, double> reei { get; }
        IVariable2Dimensional<Timestep, Region, double> rcei { get; }
        IVariable2Dimensional<Timestep, Region, double> energuse { get; }
        IVariable2Dimensional<Timestep, Region, double> seei { get; }
        IVariable2Dimensional<Timestep, Region, double> scei { get; }
        IVariable2Dimensional<Timestep, Region, double> ch4 { get; }
        IVariable2Dimensional<Timestep, Region, double> n2o { get; }
        IVariable2Dimensional<Timestep, Region, double> n2ored { get; }
        IVariable2Dimensional<Timestep, Region, double> taxpar { get; }
        IVariable2Dimensional<Timestep, Region, double> co2red { get; }
        IVariable2Dimensional<Timestep, Region, double> know { get; }
        IVariable2Dimensional<Timestep, Region, double> perm { get; }
        IVariable2Dimensional<Timestep, Region, double> cumaeei { get; }
        IVariable2Dimensional<Timestep, Region, double> ch4red { get; }

        IVariable1Dimensional<Timestep, double> minint { get; }
        IVariable1Dimensional<Timestep, double> globknow { get; }

        IVariable1Dimensional<Timestep, double> mco2 { get; }
        IVariable1Dimensional<Timestep, double> globch4 { get; }
        IVariable1Dimensional<Timestep, double> globn2o { get; }
        IVariable1Dimensional<Timestep, double> globsf6 { get; }

        IVariable1Dimensional<Timestep, double> cumglobco2 { get; }
        IVariable1Dimensional<Timestep, double> cumglobch4 { get; }
        IVariable1Dimensional<Timestep, double> cumglobn2o { get; }
        IVariable1Dimensional<Timestep, double> cumglobsf6 { get; }

        IParameter1Dimensional<Region, double> taxmp { get; }
        IParameter1Dimensional<Region, double> sf60 { get; }
        IParameter1Dimensional<Region, double> GDP90 { get; }
        IParameter1Dimensional<Region, double> pop90 { get; }
        IParameter1Dimensional<Region, double> ch4par1 { get; }
        IParameter1Dimensional<Region, double> ch4par2 { get; }
        IParameter1Dimensional<Region, double> n2opar1 { get; }
        IParameter1Dimensional<Region, double> n2opar2 { get; }
        IParameter1Dimensional<Region, double> gdp0 { get; }
        IParameter1Dimensional<Region, double> emissint0 { get; }

        IParameter2Dimensional<Timestep, Region, double> forestemm { get; }
        IParameter2Dimensional<Timestep, Region, double> aeei { get; }
        IParameter2Dimensional<Timestep, Region, double> acei { get; }
        IParameter2Dimensional<Timestep, Region, double> ch4em { get; }
        IParameter2Dimensional<Timestep, Region, double> n2oem { get; }
        IParameter2Dimensional<Timestep, Region, double> currtax { get; }
        IParameter2Dimensional<Timestep, Region, double> currtaxch4 { get; }
        IParameter2Dimensional<Timestep, Region, double> currtaxn2o { get; }
        IParameter2Dimensional<Timestep, Region, double> pgrowth { get; }
        IParameter2Dimensional<Timestep, Region, double> ypcgrowth { get; }
        IParameter2Dimensional<Timestep, Region, double> income { get; }
        IParameter2Dimensional<Timestep, Region, double> population { get; }

        double sf6gdp { get; }
        double sf6ypc { get; }
        double knowpar { get; }
        double knowgpar { get; }
        double gwpch4 { get; }
        double gwpn2o { get; }

        double TaxConstant { get; }
        double TaxEmInt { get; }
        double TaxThreshold { get; }
        double TaxDepreciation { get; }
        double MaxCostFall { get; }

        double ch4add { get; }
        double n2oadd { get; }
        double sf6add { get; }
    }

    public class EmissionsComponent
    {
        public void Run(Clock clock, IEmissionsState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                // Initial values, should eventually also come from parameter file
                var t0 = Timestep.FromSimulationYear(0);

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.energint[t0, r] = 1;
                    s.energuse[t0, r] = s.gdp0[r];
                    s.emissint[t0, r] = s.emissint0[r];
                    s.emission[t0, r] = s.emissint[t0, r] / s.energuse[t0, r];
                    s.ch4cost[t0, r] = 0;
                    s.n2ocost[t0, r] = 0;
                    s.ryg[t0, r] = 0;
                    s.reei[t0, r] = 0;
                    s.rcei[t0, r] = 0;
                    s.seei[t0, r] = 0;
                    s.scei[t0, r] = 0;
                    s.co2red[t0, r] = 0;
                    s.know[t0, r] = 1;
                    s.ch4red[t0, r] = 0;
                    s.n2ored[t0, r] = 0;
                    s.mitigationcost[t0, r] = 0;
                }

                s.globknow[t0] = 1;
                s.cumglobco2[t0] = 0.0;
                s.cumglobch4[t0] = 0.0;
                s.cumglobn2o[t0] = 0.0;
                s.cumglobsf6[t0] = 0.0;

                //SocioEconomicState.minint[t0]=Double.PositiveInfinity;
                var minint = double.PositiveInfinity;
                foreach (var r in dimensions.GetValues<Region>())
                {
                    if (s.emission[t0, r] / s.income[t0, r] < minint)
                        minint = s.emission[t0, r] / s.income[t0, r];
                }
                //s.minint[t0] = minint;
                s.minint[t0] = 0;

            }
            else
            {

                // Calculate emission and carbon intensity
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.energint[t, r] = (1.0 - 0.01 * s.aeei[t, r] - s.reei[t - 1, r]) * s.energint[t - 1, r];
                    s.emissint[t, r] = (1.0 - 0.01 * s.acei[t, r] - s.rcei[t - 1, r]) * s.emissint[t - 1, r];
                }

                // Calculate sf6 emissions
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.sf6[t, r] = (s.sf60[r] + s.sf6gdp * (s.income[t, r] - s.GDP90[r]) + s.sf6ypc * (s.income[t - 1, r] / s.population[t - 1, r] - s.GDP90[r] / s.pop90[r])) * (t <= Timestep.FromSimulationYear(60) ? 1 + (t.Value - 40.0) / 40.0 : 1.0 + (60.0 - 40.0) / 40.0) * (t > Timestep.FromSimulationYear(60) ? Math.Pow(0.99, t.Value - 60.0) : 1.0);
                }
                // Check for unrealistic values
                foreach (var r in dimensions.GetValues<Region>())
                {
                    if (s.sf6[t, r] < 0.0)
                        s.sf6[t, r] = 0;

                }
                // Calculate energy use
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.energuse[t, r] = (1 - s.seei[t - 1, r]) * s.energint[t, r] * s.income[t, r];
                }

                // Calculate co2 emissions
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.emission[t, r] = (1 - s.scei[t - 1, r]) * s.emissint[t, r] * s.energuse[t, r];
                    s.emissionwithforestry[t, r] = s.emission[t, r] + s.forestemm[t, r];
                }

                // Calculate ch4 emissions
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.ch4[t, r] = s.ch4em[t, r] * (1 - s.ch4red[t - 1, r]);
                }

                // Calculate n2o emissions
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.n2o[t, r] = s.n2oem[t, r] * (1 - s.n2ored[t - 1, r]);
                }


                // TODO RT check
                foreach (var r in dimensions.GetValues<Region>())
                {
                    if (s.emission[t, r] / s.income[t, r] - s.minint[t - 1] <= 0)
                        s.taxpar[t, r] = s.TaxConstant;
                    else
                        s.taxpar[t, r] = s.TaxConstant - s.TaxEmInt * Math.Sqrt(s.emission[t, r] / s.income[t, r] - s.minint[t - 1]);
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.co2red[t, r] = s.currtax[t, r] * s.emission[t, r] * s.know[t - 1, r] * s.globknow[t - 1] / 2 / s.taxpar[t, r] / s.income[t, r] / 1000;

                    if (s.co2red[t, r] < 0)
                        s.co2red[t, r] = 0;
                    else if (s.co2red[t, r] > 0.99)
                        s.co2red[t, r] = 0.99;
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.ryg[t, r] = s.taxpar[t, r] * Math.Pow(s.co2red[t, r], 2) / s.know[t - 1, r] / s.globknow[t - 1];
                }

                // TODO RT check
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.perm[t, r] = 1.0 - 1.0 / s.TaxThreshold * s.currtax[t, r] / (1 + 1.0 / s.TaxThreshold * s.currtax[t, r]);
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.reei[t, r] = s.perm[t, r] * 0.5 * s.co2red[t, r];
                }

                // TODO RT check
                foreach (var r in dimensions.GetValues<Region>())
                {
                    if (s.currtax[t, r] < s.TaxThreshold)
                        s.rcei[t, r] = s.perm[t, r] * 0.5 * Math.Pow(s.co2red[t, r], 2);
                    else
                        s.rcei[t, r] = s.perm[t, r] * 0.5 * s.co2red[t, r];
                }

                // TODO RT check
                // TODO RT what is the 1.7?
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.seei[t, r] = (1.0 - s.TaxDepreciation) * s.seei[t - 1, r] + (1.0 - s.perm[t, r]) * 0.5 * s.co2red[t, r] * 1.7;
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    if (s.currtax[t, r] < 100)
                        s.scei[t, r] = 0.9 * s.scei[t - 1, r] + (1 - s.perm[t, r]) * 0.5 * Math.Pow(s.co2red[t, r], 2);
                    else
                        s.scei[t, r] = 0.9 * s.scei[t - 1, r] + (1 - s.perm[t, r]) * 0.5 * s.co2red[t, r] * 1.7;
                }

                // TODO RT check
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.know[t, r] = s.know[t - 1, r] * Math.Sqrt(1 + s.knowpar * s.co2red[t, r]);

                    if (s.know[t, r] > Math.Sqrt(s.MaxCostFall))
                        s.know[t, r] = Math.Sqrt(s.MaxCostFall);
                }

                s.globknow[t] = s.globknow[t - 1];
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.globknow[t] = s.globknow[t] * Math.Sqrt(1 + s.knowgpar * s.co2red[t, r]);
                }
                if (s.globknow[t] > 3.16)
                    s.globknow[t] = 3.16;

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.ch4red[t, r] = s.currtaxch4[t, r] * s.ch4em[t, r] / 2 / s.ch4par1[r] / s.ch4par2[r] / s.ch4par2[r] / s.income[t, r] / 1000;

                    if (s.ch4red[t, r] < 0)
                        s.ch4red[t, r] = 0;
                    else if (s.ch4red[t, r] > 0.99)
                        s.ch4red[t, r] = 0.99;
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.n2ored[t, r] = s.gwpn2o * s.currtaxn2o[t, r] * s.n2oem[t, r] / 2 / s.n2opar1[r] / s.n2opar2[r] / s.n2opar2[r] / s.income[t, r] / 1000;

                    if (s.n2ored[t, r] < 0)
                        s.n2ored[t, r] = 0;
                    else if (s.n2ored[t, r] > 0.99)
                        s.n2ored[t, r] = 0.99;
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.ch4cost[t, r] = s.ch4par1[r] * Math.Pow(s.ch4par2[r], 2) * Math.Pow(s.ch4red[t, r], 2);
                    s.ch4costindollar[t, r] = s.ch4cost[t, r] * s.income[t, r];
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.n2ocost[t, r] = s.n2opar1[r] * Math.Pow(s.n2opar2[r], 2) * Math.Pow(s.n2ored[t, r], 2);
                }

                var minint = Double.PositiveInfinity;
                foreach (var r in dimensions.GetValues<Region>())
                {
                    if (s.emission[t, r] / s.income[t, r] < minint)
                        minint = s.emission[t, r] / s.income[t, r];
                }
                s.minint[t] = minint;

                foreach (var r in dimensions.GetValues<Region>())
                {
                    if (t > Timestep.FromYear(2000))
                        s.cumaeei[t, r] = s.cumaeei[t - 1, r] * (1.0 - 0.01 * s.aeei[t, r] - s.reei[t, r] + s.seei[t - 1, r] - s.seei[t, r]);
                    else
                        s.cumaeei[t, r] = 1.0;
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.mitigationcost[t, r] = (s.taxmp[r] * s.ryg[t, r] /*+ s.ch4cost[t, r]*/ + s.n2ocost[t, r]) * s.income[t, r];
                }

                double globco2 = 0;
                double globch4 = 0;
                double globn2o = 0;
                double globsf6 = 0;

                foreach (var r in dimensions.GetValues<Region>())
                {
                    globco2 = globco2 + s.emissionwithforestry[t, r];
                    globch4 = globch4 + s.ch4[t, r];
                    globn2o = globn2o + s.n2o[t, r];
                    globsf6 = globsf6 + s.sf6[t, r];
                }

                s.mco2[t] = globco2;
                s.globch4[t] = Math.Max(0.0, globch4 + (t.Value > 50 ? s.ch4add * (t.Value - 50) : 0.0));
                s.globn2o[t] = Math.Max(0.0, globn2o + (t.Value > 50 ? s.n2oadd * (t.Value - 50) : 0));
                s.globsf6[t] = Math.Max(0.0, globsf6 + (t.Value > 50 ? s.sf6add * (t.Value - 50) : 0.0));

                s.cumglobco2[t] = s.cumglobco2[t - 1] + s.mco2[t];
                s.cumglobch4[t] = s.cumglobch4[t - 1] + s.globch4[t];
                s.cumglobn2o[t] = s.cumglobn2o[t - 1] + s.globn2o[t];
                s.cumglobsf6[t] = s.cumglobsf6[t - 1] + s.globsf6[t];
            }
        }
    }

}

