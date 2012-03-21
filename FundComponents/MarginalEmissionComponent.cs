// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmf;

namespace Fund.Components.MarginalEmission
{
    public interface IMarginalEmissionState
    {
        Timestep emissionperiod { get; }
        IParameter1Dimensional<Timestep, Double> emission { get; }

        IVariable1Dimensional<Timestep, Double> modemission { get; }
    }

    public class MarginalEmissionComponent
    {
        public void Run(Clock clock, IMarginalEmissionState state, IDimensions dimensions)
        {
            var t = clock.Current;
            var s = state;

            if (clock.IsFirstTimestep)
            {

            }
            else
            {
                if ((t.Value >= s.emissionperiod.Value) && (t.Value < (s.emissionperiod.Value + 10)))
                {
                    s.modemission[t] = s.emission[t] + 1;
                }
                else
                    s.modemission[t] = s.emission[t];
            }
        }

    }
}
