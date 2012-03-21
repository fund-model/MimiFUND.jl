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

namespace Fund.Components.ImpactExtratropicalStorms
{

    public interface IImpactExtratropicalStormsState
    {
        IVariable2Dimensional<Timestep, Region, double> extratropicalstormsdam { get; }
        IVariable2Dimensional<Timestep, Region, double> extratropicalstormsdead { get; }

        IParameter1Dimensional<Region, double> extratropicalstormsbasedam { get; }
        Double extratropicalstormsdamel { get; }
        IParameter1Dimensional<Region, double> extratropicalstormspar { get; }
        IParameter1Dimensional<Region, double> extratropicalstormsbasedead { get; }
        Double extratropicalstormsdeadel { get; }
        Double extratropicalstormsnl { get; }

        IParameter1Dimensional<Region, double> gdp90 { get; }
        IParameter1Dimensional<Region, double> pop90 { get; }

        IParameter2Dimensional<Timestep, Region, double> population { get; }
        IParameter2Dimensional<Timestep, Region, double> income { get; }

        IParameter1Dimensional<Timestep, double> acco2 { get; }
        Double co2pre { get; }
    }


    public class ImpactExtratropicalStormsComponent
    {
        public void Run(Clock clock, IImpactExtratropicalStormsState state, IDimensions dimensions)
        {
            var t = clock.Current;
            var s = state;

            foreach (var r in dimensions.GetValues<Region>())
            {
                double ypc = s.income[t, r] / s.population[t, r] * 1000.0;
                double ypc90 = s.gdp90[r] / s.pop90[r] * 1000.0;

                s.extratropicalstormsdam[t, r] = s.extratropicalstormsbasedam[r] * s.income[t, r] * Math.Pow(ypc / ypc90, s.extratropicalstormsdamel) * (Math.Pow(1.0 + (s.extratropicalstormspar[r] * (s.acco2[t] / s.co2pre)), s.extratropicalstormsnl) - 1.0);
                s.extratropicalstormsdead[t, r] = 1000.0 * s.extratropicalstormsbasedead[r] * s.population[t, r] * Math.Pow(ypc / ypc90, s.extratropicalstormsdeadel) * (Math.Pow(1.0 + (s.extratropicalstormspar[r] * (s.acco2[t] / s.co2pre)), s.extratropicalstormsnl) - 1.0);
            }
        }
    }
}
