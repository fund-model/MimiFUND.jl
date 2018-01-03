// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmf;

namespace Fund.Components
{

    public interface IClimateCH4CycleState
    {


        /// <summary>Global CH4 emissions in Mt of CH4</summary>
        IParameter1Dimensional<Timestep, double> globch4 { get; }

        /// <summary>Atmospheric CH4 concentration</summary>
        IVariable1Dimensional<Timestep, double> acch4 { get; }

        /// <summary>CH4 decay</summary>
        double ch4decay { get; set; }

        /// <summary></summary>
        double lifech4 { get; }

        /// <summary> CH4 pre industrial</summary>
        double ch4pre { get; }

    }
    public class ClimateCH4CycleComponent
    {

        public void Run(Clock clock, IClimateCH4CycleState state, IDimensions dimensions)
        {
            // create shortcuts for commonly accessed data
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                s.ch4decay = 1.0 / s.lifech4;

                s.acch4[t] = 1222.0;
            }
            else
            {
                // Calculate CH4 concentrations
                s.acch4[t] = s.acch4[t - 1] + 0.3597 * s.globch4[t] - s.ch4decay * (s.acch4[t - 1] - s.ch4pre);

                if (s.acch4[t] < 0)
                    throw new ApplicationException("ch4 atmospheric concentration out of range");
            }
        }
    }
}
