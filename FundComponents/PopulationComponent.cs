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

namespace Fund.Components.Population
{
    public interface IPopulationState
    {
        IVariable2Dimensional<Timestep, Region, double> population { get; }
        IVariable2Dimensional<Timestep, Region, double> populationin1 { get; }

        IVariable1Dimensional<Timestep, double> globalpopulation { get; }

        IParameter2Dimensional<Timestep, Region, double> pgrowth { get; }
        IParameter2Dimensional<Timestep, Region, double> enter { get; }
        IParameter2Dimensional<Timestep, Region, double> leave { get; }
        IParameter2Dimensional<Timestep, Region, double> dead { get; }

        IParameter1Dimensional<Region, double> pop0 { get; }

        [DefaultParameterValue(false)]
        bool runwithoutpopulationperturbation { get; }



    }

    public class PopulationComponent
    {
        public void Run(Clock clock, IPopulationState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                double globalpopulation = 0.0;

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.population[t, r] = s.pop0[r];
                    s.populationin1[t, r] = s.population[t, r] * 1000000.0;

                    globalpopulation = globalpopulation + s.populationin1[t, r];
                }

                s.globalpopulation[t] = globalpopulation;
            }
            else
            {
                var globalPopulation = 0.0;
                // Calculate population
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.population[t, r] = (1.0 + 0.01 * s.pgrowth[t - 1, r]) * (s.population[t - 1, r] +
                        (
                        (t >= Timestep.FromSimulationYear(40)) && !s.runwithoutpopulationperturbation ? (s.enter[t - 1, r] / 1000000.0) - (s.leave[t - 1, r] / 1000000.0) - (s.dead[t - 1, r] >= 0 ? s.dead[t - 1, r] / 1000000.0 : 0) : 0
                          )
                    );

                    if (s.population[t, r] < 0)
                        s.population[t, r] = 0.000001;
                    //raise new Exception;

                    s.populationin1[t, r] = s.population[t, r] * 1000000.0;
                    globalPopulation = globalPopulation + s.populationin1[t, r];
                }
                s.globalpopulation[t] = globalPopulation;



            }
        }
    }
}
