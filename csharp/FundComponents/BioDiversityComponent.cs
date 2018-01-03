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

    /// <summary>State for the biodiversity component</summary>
    public interface IBioDiversityState
    {
        /// <summary> Number of species </summary>
        IVariable1Dimensional<Timestep, double> nospecies { get; }

        /// <summary> additive parameter </summary>
        double bioloss { get; }

        /// <summary> multiplicative parameter </summary>
        double biosens { get; }

        /// <summary> Temperature </summary>
        IParameter1Dimensional<Timestep, double> temp { get; }

        /// <summary> benchmark temperature change </summary>
        double dbsta { get; }

        /// <summary> Number of species in the year 2000 </summary>
        double nospecbase { get; }
    }

    public class BioDiversityComponent
    {
        public void Run(Clock clock, IBioDiversityState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (t > Timestep.FromYear(2000))
            {
                var dt = Math.Abs(s.temp[t] - s.temp[t - 1]);

                s.nospecies[t] = Math.Max(
                  s.nospecbase / 100,
                  s.nospecies[t - 1] * (1.0 - s.bioloss - s.biosens * dt * dt / s.dbsta / s.dbsta)
                  );
            }
            else
                s.nospecies[t] = s.nospecbase;
        }
    }
}
