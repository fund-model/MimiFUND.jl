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
    public interface IClimateSO2CycleState
    {
        /// <summary>Global SO2 emissions</summary>
        IParameter1Dimensional<Timestep, double> globso2 { get; }

        /// <summary>Atmospheric SO2 concentration</summary>
        IVariable1Dimensional<Timestep, double> acso2 { get; }
    }
    public class ClimateSO2CycleComponent
    {

        public void Run(Clock clock, IClimateSO2CycleState state, IDimensions dimensions)
        {
            // create shortcuts for commonly accessed data
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {

            }
            else
            {
                // Calculate SO2 concentrations
                s.acso2[t] = s.globso2[t];
            }
        }
    }
}
