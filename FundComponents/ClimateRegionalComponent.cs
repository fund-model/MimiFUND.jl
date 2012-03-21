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

namespace Fund.Components.ClimateRegional
{
    public interface IClimateRegionalState
    {
        IParameter1Dimensional<Timestep, Double> inputtemp { get; }
        IParameter1Dimensional<Region, Double> bregtmp { get; }
        IParameter1Dimensional<Region, Double> bregstmp { get; }
        IParameter2Dimensional<Timestep, Region, Double> scentemp { get; }

        IVariable2Dimensional<Timestep, Region, Double> temp { get; }
        IVariable2Dimensional<Timestep, Region, Double> regtmp { get; }
        IVariable2Dimensional<Timestep, Region, Double> regstmp { get; }
    }

    public class ClimateRegionalComponent
    {
        public void Run(Clock clock, IClimateRegionalState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            foreach (var r in dimensions.GetValues<Region>())
            {
                s.regtmp[t, r] = s.inputtemp[t] * s.bregtmp[r] + s.scentemp[t, r];
            }

            foreach (var r in dimensions.GetValues<Region>())
            {
                s.temp[t, r] = s.regtmp[t, r] / s.bregtmp[r];
            }

            foreach (var r in dimensions.GetValues<Region>())
            {
                s.regstmp[t, r] = s.inputtemp[t] * s.bregstmp[r] + s.scentemp[t, r];
            }
        }

    }
}
