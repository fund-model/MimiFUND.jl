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

namespace Fund.Components.ImpactWaterResources
{

    public interface IImpactWaterResourcesState
    {
        IVariable1Dimensional<Timestep, double> watech { get; }
        Double watechrate { get; }

        IVariable2Dimensional<Timestep, Region, double> water { get; }
        IParameter1Dimensional<Region, double> wrbm { get; }
        Double wrel { get; }
        Double wrnl { get; }
        Double wrpl { get; }

        IParameter1Dimensional<Region, double> gdp90 { get; }
        IParameter2Dimensional<Timestep, Region, double> income { get; }

        IParameter2Dimensional<Timestep, Region, double> population { get; }
        IParameter1Dimensional<Region, double> pop90 { get; }

        IParameter2Dimensional<Timestep, Region, double> temp { get; }
    }


    public class ImpactWaterResourcesComponent
    {
        public void Run(Clock clock, IImpactWaterResourcesState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (t > Timestep.FromYear(2000))
                s.watech[t] = Math.Pow(1.0 - s.watechrate, t.Value - Timestep.FromYear(2000).Value);
            else
                s.watech[t] = 1.0;

            foreach (var r in dimensions.GetValues<Region>())
            {
                double ypc = s.income[t, r] / s.population[t, r] * 1000.0;
                double ypc90 = s.gdp90[r] / s.pop90[r] * 1000.0;

                var water = s.wrbm[r] * s.gdp90[r] * s.watech[t] * Math.Pow(ypc / ypc90, s.wrel) * Math.Pow(s.population[t, r] / s.pop90[r], s.wrpl) * Math.Pow(s.temp[t, r], s.wrnl);

                if (water > 0.1 * s.income[t, r])
                    s.water[t, r] = 0.1 * s.income[t, r];
                else
                    s.water[t, r] = water;
            }
        }
    }
}
