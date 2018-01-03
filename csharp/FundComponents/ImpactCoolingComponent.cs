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

    public interface IImpactCoolingState
    {
        IVariable2Dimensional<Timestep, Region, double> cooling { get; }

        IParameter1Dimensional<Region, double> cebm { get; }
        IParameter1Dimensional<Region, double> gdp90 { get; }

        IParameter2Dimensional<Timestep, Region, double> population { get; }
        IParameter1Dimensional<Region, double> pop90 { get; }

        IParameter2Dimensional<Timestep, Region, double> income { get; }
        Double ceel { get; }

        IParameter2Dimensional<Timestep, Region, double> temp { get; }
        Double cenl { get; }

        IParameter2Dimensional<Timestep, Region, double> cumaeei { get; }
    }

    public class ImpactCoolingComponent
    {
        public void Run(Clock clock, IImpactCoolingState state, IDimensions dimensions)
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
                    double ypc90 = s.gdp90[r] / s.pop90[r] * 1000.0;

                    s.cooling[t, r] = s.cebm[r] * s.cumaeei[t, r] * s.gdp90[r] * Math.Pow(s.temp[t, r] / 1.0, s.cenl) * Math.Pow(ypc / ypc90, s.ceel) * s.population[t, r] / s.pop90[r];
                }
            }
        }
    }
}
