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

    public interface IImpactBioDiversityState
    {
        /// <summary> Change in number of species in relation to the year 2000 </summary>
        IVariable1Dimensional<Timestep, double> biodiv { get; }

        IVariable2Dimensional<Timestep, Region, double> species { get; }

        /// <summary> Number of species in the year 2000 </summary>
        double nospecbase { get; }

        IParameter1Dimensional<Timestep, double> nospecies { get; }
        IParameter2Dimensional<Timestep, Region, double> temp { get; }
        IParameter2Dimensional<Timestep, Region, double> income { get; }
        IParameter2Dimensional<Timestep, Region, double> population { get; }
        IParameter1Dimensional<Region, double> valinc { get; }
        double bioshare { get; }
        double spbm { get; }
        double valbase { get; }
        double dbsta { get; }
    }
    public class ImpactBioDiversityComponent
    {
        public void Run(Clock clock, IImpactBioDiversityState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {

            }
            else
            {

                s.biodiv[t] = s.nospecbase / s.nospecies[t];

                foreach (var r in dimensions.GetValues<Region>())
                {
                    double ypc = 1000.0 * s.income[t, r] / s.population[t, r];

                    var dt = Math.Abs(s.temp[t, r] - s.temp[t - 1, r]);

                    var valadj = s.valbase / s.valinc[r] / (1 + s.valbase / s.valinc[r]);

                    s.species[t, r] = s.spbm /
                                    s.valbase * ypc / s.valinc[r] / (1.0 + ypc / s.valinc[r]) / valadj * ypc *
                                    s.population[t, r] / 1000.0 *
                                    dt / s.dbsta / (1.0 + dt / s.dbsta) *
                                    (1.0 - s.bioshare + s.bioshare * s.biodiv[t]);
                }
            }
        }
    }
}
