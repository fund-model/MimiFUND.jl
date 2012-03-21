// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fund.CommonDimensions;
using Esmf;

namespace Fund.Components.ImpactForests
{
    public interface IImpactForestsState
    {
        IVariable2Dimensional<Timestep, Region, double> forests { get; }

        IParameter1Dimensional<Region, double> forbm { get; }
        IParameter1Dimensional<Region, double> gdp90 { get; }
        IParameter1Dimensional<Region, double> pop90 { get; }

        IParameter1Dimensional<Timestep, double> acco2 { get; }

        IParameter2Dimensional<Timestep, Region, double> income { get; }
        IParameter2Dimensional<Timestep, Region, double> population { get; }
        IParameter2Dimensional<Timestep, Region, double> temp { get; }

        double forel { get; }
        double fornl { get; }
        double co2pre { get; }
        double forco2 { get; }
    }

    public class ImpactForests
    {
        public void Run(Clock clock, IImpactForestsState state, IDimensions dimensions)
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
                    double ypc = 1000.0 * s.income[t, r] / s.population[t, r];
                    double ypc90 = s.gdp90[r] / s.pop90[r] * 1000.0;

                    // TODO -oDavid Anthoff: RT uses -lP.forel for ppp case
                    s.forests[t, r] = s.forbm[r] * s.income[t, r] * Math.Pow(ypc / ypc90, s.forel) * (0.5 * Math.Pow(s.temp[t, r], s.fornl) + 0.5 * Math.Log(s.acco2[t - 1] / s.co2pre) * s.forco2);

                    if (s.forests[t, r] > 0.1 * s.income[t, r])
                        s.forests[t, r] = 0.1 * s.income[t, r];
                }
            }
        }

    }
}
