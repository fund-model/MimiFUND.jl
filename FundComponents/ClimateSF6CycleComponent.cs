// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmf;

namespace Fund.Components.ClimateSF6Cycle
{
    public interface IClimateSF6CycleState
    {
        /// <summary> Global SF6 emissions in kt of SF6</summary>
        IParameter1Dimensional<Timestep, double> globsf6 { get; }

        /// <summary>Atmospheric SF6 concentrations</summary>
        IVariable1Dimensional<Timestep, double> acsf6 { get; }

        /// <summary>SF6 pre industrial</summary>
        double sf6pre { get; }

        /// <summary>SF6 decay</summary>
        double sf6decay { get; set; }

        /// <summary></summary>
        double lifesf6 { get; }
    }

    public class ClimateSF6CycleComponent
    {

        public void Run(Clock clock, IClimateSF6CycleState state, IDimensions dimensions)
        {
            // create shortcuts for commonly accessed data
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                s.sf6decay = 1.0 / s.lifesf6;

                s.acsf6[t] = s.sf6pre;
            }
            else
            {
                // Calculate SF6 concentrations   
                s.acsf6[t] = s.sf6pre + (s.acsf6[t - 1] - s.sf6pre) * (1 - s.sf6decay) + s.globsf6[t] / 25.1;

                if (s.acsf6[t] < 0)
                    throw new ApplicationException("sf6 atmospheric concentration out of range");
            }
        }
    }
}
