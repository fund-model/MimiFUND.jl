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

namespace Fund.Components.ImpactAgriculture
{

    public interface IImpactAgricultureState
    {
        IParameter1Dimensional<Region, double> gdp90 { get; }
        IParameter2Dimensional<Timestep, Region, double> income { get; }
        IParameter1Dimensional<Region, double> pop90 { get; }
        IParameter2Dimensional<Timestep, Region, double> population { get; }

        IVariable2Dimensional<Timestep, Region, double> agrish { get; }
        IParameter1Dimensional<Region, double> agrish0 { get; }
        Double agel { get; }

        IVariable2Dimensional<Timestep, Region, double> agrate { get; }
        IVariable2Dimensional<Timestep, Region, double> aglevel { get; }
        IVariable2Dimensional<Timestep, Region, double> agco2 { get; }
        IVariable2Dimensional<Timestep, Region, double> agcost { get; }

        IParameter1Dimensional<Region, double> agrbm { get; }
        IParameter1Dimensional<Region, double> agtime { get; }
        Double agnl { get; }

        IParameter1Dimensional<Region, double> aglparl { get; }
        IParameter1Dimensional<Region, double> aglparq { get; }

        IParameter1Dimensional<Region, double> agcbm { get; }
        Double co2pre { get; }

        IParameter2Dimensional<Timestep, Region, double> temp { get; }
        IParameter1Dimensional<Timestep, double> acco2 { get; }
    }

    public class ImpactAgricultureComponent
    {
        public const double DBsT = 0.04;     // base case yearly warming

        public void Run(Clock clock, IImpactAgricultureState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.agrate[t, r] = s.agrbm[r] * Math.Pow(0.005 / DBsT, s.agnl) * s.agtime[r];
                }
            }
            else
            {
                foreach (var r in dimensions.GetValues<Region>())
                {
                    Double ypc = s.income[t, r] / s.population[t, r] * 1000.0;
                    Double ypc90 = s.gdp90[r] / s.pop90[r] * 1000.0;

                    s.agrish[t, r] = s.agrish0[r] * Math.Pow(ypc / ypc90, -s.agel);
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    var dtemp = Math.Abs(s.temp[t, r] - s.temp[t - 1, r]);

                    if (double.IsNaN(Math.Pow(dtemp / 0.04, s.agnl)))
                        s.agrate[t, r] = 0.0;
                    else
                        s.agrate[t, r] = s.agrbm[r] * Math.Pow(dtemp / 0.04, s.agnl) + (1.0 - 1.0 / s.agtime[r]) * s.agrate[t - 1, r];
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.aglevel[t, r] = s.aglparl[r] * s.temp[t, r] + s.aglparq[r] * Math.Pow(s.temp[t, r], 2.0);
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.agco2[t, r] = s.agcbm[r] / Math.Log(2.0) * Math.Log(s.acco2[t - 1] / s.co2pre);
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.agcost[t, r] = Math.Min(1.0, s.agrate[t, r] + s.aglevel[t, r] + s.agco2[t, r]) * s.agrish[t, r] * s.income[t, r];
                }
            }
        }
    }
}
