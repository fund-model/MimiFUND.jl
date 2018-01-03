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

    public interface IImpactTropicalStormsState
    {
        IVariable2Dimensional<Timestep, Region, double> hurrdam { get; }
        IVariable2Dimensional<Timestep, Region, double> hurrdead { get; }

        IParameter1Dimensional<Region, double> hurrbasedam { get; }
        Double hurrdamel { get; }
        Double hurrnl { get; }
        Double hurrpar { get; }
        IParameter1Dimensional<Region, double> hurrbasedead { get; }
        Double hurrdeadel { get; }

        IParameter1Dimensional<Region, double> gdp90 { get; }
        IParameter1Dimensional<Region, double> pop90 { get; }

        IParameter2Dimensional<Timestep, Region, double> population { get; }
        IParameter2Dimensional<Timestep, Region, double> income { get; }

        IParameter2Dimensional<Timestep, Region, double> regstmp { get; }
    }


    public class ImpactTropicalStormsComponent
    {
        public void Run(Clock clock, IImpactTropicalStormsState state, IDimensions dimensions)
        {
            var t = clock.Current;
            var s = state;

            foreach (var r in dimensions.GetValues<Region>())
            {
                double ypc = s.income[t, r] / s.population[t, r] * 1000.0;
                double ypc90 = s.gdp90[r] / s.pop90[r] * 1000.0;

                // This is hurrican damage
                s.hurrdam[t, r] = 0.001 * s.hurrbasedam[r] * s.income[t, r] * Math.Pow(ypc / ypc90, s.hurrdamel) * (Math.Pow(1.0 + s.hurrpar * s.regstmp[t, r], s.hurrnl) - 1.0);

                s.hurrdead[t, r] = 1000.0 * s.hurrbasedead[r] * s.population[t, r] * Math.Pow(ypc / ypc90, s.hurrdeadel) * (Math.Pow(1.0 + s.hurrpar * s.regstmp[t, r], s.hurrnl) - 1.0);
            }
        }

    }
}
