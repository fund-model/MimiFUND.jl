// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmf;

namespace Fund.Components.ClimateDynamics
{

    /// <summary>State for the climate component</summary>
    public interface IClimateDynamicsState
    {
        /// <summary>Total radiative forcing</summary>
        IParameter1Dimensional<Timestep, double> radforc { get; }

        /// <summary>Average global temperature</summary>
        IVariable1Dimensional<Timestep, double> temp { get; }

        /// <summary>LifeTempConst</summary>
        double LifeTempConst { get; }

        /// <summary>LifeTempLin</summary>
        double LifeTempLin { get; }

        /// <summary>LifeTempQd</summary>
        double LifeTempQd { get; }

        /// <summary>Climate sensitivity</summary>
        double ClimateSensitivity { get; }
    }

    public class ClimateDynamicsComponent
    {

        public void Run(Clock clock, IClimateDynamicsState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                s.temp[t] = 0.20;
            }
            else
            {
                var LifeTemp = Math.Max(s.LifeTempConst + s.LifeTempLin * s.ClimateSensitivity + s.LifeTempQd * Math.Pow(s.ClimateSensitivity, 2.0), 1.0);

                var delaytemp = 1.0 / LifeTemp;

                var temps = s.ClimateSensitivity / 5.35 / Math.Log(2.0);

                // Calculate temperature          
                var dtemp = delaytemp * temps * s.radforc[t] - delaytemp * s.temp[t - 1];

                s.temp[t] = s.temp[t - 1] + dtemp;
            }
        }
    }
}
