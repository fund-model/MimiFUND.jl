// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmf;

namespace Fund.Components.ClimateN2OCycle
{
    public interface IClimateN2OCycleState
    {
        /// <summary>Global N2O emissions in Mt of N</summary>
        IParameter1Dimensional<Timestep, double> globn2o { get; }

        /// <summary>Atmospheric N2O concentration</summary>
        IVariable1Dimensional<Timestep, double> acn2o { get; }

        /// <summary>N2O decay</summary>
        double n2odecay { get; set; }

        /// <summary></summary>
        Double lifen2o { get; }

        /// <summary>N2o pre industrial</summary>
        double n2opre { get; }
    }

    public class ClimateN2OCycleComponent
    {

        public void Run(Clock clock, IClimateN2OCycleState state, IDimensions dimensions)
        {
            // create shortcuts for commonly accessed data
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                s.n2odecay = 1.0 / s.lifen2o;

                s.acn2o[t] = 296;
            }
            else
            {
                // Calculate N2O concentrations
                s.acn2o[t] = s.acn2o[t - 1] + 0.2079 * s.globn2o[t] - s.n2odecay * (s.acn2o[t - 1] - s.n2opre);

                if (s.acn2o[t] < 0)
                    throw new ApplicationException("n2o atmospheric concentration out of range");
            }
        }
    }
}
