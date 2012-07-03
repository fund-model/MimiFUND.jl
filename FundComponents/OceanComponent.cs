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

    /// <summary>State for the ocean component</summary>  
    public interface IOceanState
    {
        /// <summary> Sea-level rise in cm </summary>
        IVariable1Dimensional<Timestep, double> sea { get; }

        /// <summary></summary>
        double lifesea { get; }

        /// <summary> </summary>
        double seas { get; }

        double delaysea { get; set; }

        /// <summary> Temperature incrase in C° </summary>
        IParameter1Dimensional<Timestep, double> temp { get; }
    }


    public class OceanComponent
    {
        public void Run(Clock clock, IOceanState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                // Delay in sea-level rise
                s.delaysea = 1.0 / s.lifesea;
                s.sea[t] = 0.0;
            }
            else
            {
                // Calculate sea level rise  
                var ds = s.delaysea * s.seas * s.temp[t] - s.delaysea * s.sea[t - 1];

                s.sea[t] = s.sea[t - 1] + ds;
            }
        }
    }
}
