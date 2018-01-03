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

    public interface IGeographyState
    {
        /// <summary>
        /// area in km^2
        /// </summary>
        IVariable2Dimensional<Timestep, Region, double> area { get; }
        IParameter2Dimensional<Timestep, Region, double> landloss { get; }
        IParameter1Dimensional<Region, double> area0 { get; }
    }

    public class GeographyComponent
    {
        public void Run(Clock clock, IGeographyState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.area[t, r] = s.area0[r];
                }
            }
            else
            {
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.area[t, r] = s.area[t - 1, r] - s.landloss[t - 1, r];
                }
            }
        }
    }

}
