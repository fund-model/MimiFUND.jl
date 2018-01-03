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

    public interface IImpactDiarrhoeaState
    {
        IVariable2Dimensional<Timestep, Region, Double> diadead { get; }
        IVariable2Dimensional<Timestep, Region, Double> diasick { get; }

        IParameter1Dimensional<Region, Double> diamort { get; }
        Double diamortel { get; }
        Double diamortnl { get; }

        IParameter1Dimensional<Region, Double> diayld { get; }
        Double diayldel { get; }
        Double diayldnl { get; }

        IParameter2Dimensional<Timestep, Region, Double> income { get; }
        IParameter2Dimensional<Timestep, Region, Double> population { get; }
        IParameter1Dimensional<Region, Double> gdp90 { get; }
        IParameter1Dimensional<Region, Double> pop90 { get; }

        IParameter1Dimensional<Region, Double> temp90 { get; }
        IParameter1Dimensional<Region, Double> bregtmp { get; }
        IParameter2Dimensional<Timestep, Region, Double> regtmp { get; }
    }

    public class ImpactDiarrhoeaComponent
    {
        public void Run(Clock clock, IImpactDiarrhoeaState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            foreach (var r in dimensions.GetValues<Region>())
            {
                var ypc = 1000.0 * s.income[t, r] / s.population[t, r];
                var ypc90 = 1000.0 * s.gdp90[r] / s.pop90[r];

                // 0.49 is the increase in global temperature from pre-industrial to 1990
                var absoluteRegionalTempPreIndustrial = s.temp90[r] - 0.49 * s.bregtmp[r];

                if (absoluteRegionalTempPreIndustrial > 0.0)
                {
                    s.diadead[t, r] = s.diamort[r] * s.population[t, r] * Math.Pow(ypc / ypc90, s.diamortel)
                        * (Math.Pow((absoluteRegionalTempPreIndustrial + s.regtmp[t, r]) / absoluteRegionalTempPreIndustrial, s.diamortnl) - 1.0);

                    s.diasick[t, r] = s.diayld[r] * s.population[t, r] * Math.Pow(ypc / ypc90, s.diayldel)
                        * (Math.Pow((absoluteRegionalTempPreIndustrial + s.regtmp[t, r]) / absoluteRegionalTempPreIndustrial, s.diayldnl) - 1.0);
                }
                else
                {
                    s.diadead[t, r] = 0.0;
                    s.diasick[t, r] = 0.0;
                }
            }
        }

    }
}
