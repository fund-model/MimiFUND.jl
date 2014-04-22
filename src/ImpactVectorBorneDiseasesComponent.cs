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
    public interface IImpactVectorBorneDiseasesState
    {
        IVariable2Dimensional<Timestep, Region, double> dengue { get; }
        IVariable2Dimensional<Timestep, Region, double> schisto { get; }
        IVariable2Dimensional<Timestep, Region, double> malaria { get; }

        IParameter1Dimensional<Region, double> dfbs { get; }
        IParameter1Dimensional<Region, double> dfch { get; }
        IParameter1Dimensional<Region, double> smbs { get; }
        IParameter1Dimensional<Region, double> smch { get; }
        IParameter1Dimensional<Region, double> malbs { get; }
        IParameter1Dimensional<Region, double> malch { get; }


        IParameter1Dimensional<Region, double> gdp90 { get; }
        IParameter1Dimensional<Region, double> pop90 { get; }

        IParameter2Dimensional<Timestep, Region, double> income { get; }
        IParameter2Dimensional<Timestep, Region, double> population { get; }
        IParameter2Dimensional<Timestep, Region, double> temp { get; }

        double dfnl { get; }
        double vbel { get; }
        double smnl { get; }
        double malnl { get; }
    }

    public class ImpactVectorBorneDiseasesComponent
    {
        public void Run(Clock clock, IImpactVectorBorneDiseasesState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            foreach (var r in dimensions.GetValues<Region>())
            {
                double ypc = 1000.0 * s.income[t, r] / s.population[t, r];
                double ypc90 = s.gdp90[r] / s.pop90[r] * 1000.0;

                s.dengue[t, r] = s.dfbs[r] * s.population[t, r] * s.dfch[r] * Math.Pow(s.temp[t, r], s.dfnl) * Math.Pow(ypc / ypc90, s.vbel);

                s.schisto[t, r] = s.smbs[r] * s.population[t, r] * s.smch[r] * Math.Pow(s.temp[t, r], s.smnl) * Math.Pow(ypc / ypc90, s.vbel);

                if (s.schisto[t, r] < -s.smbs[r] * s.population[t, r] * Math.Pow(ypc / ypc90, s.vbel))
                    s.schisto[t, r] = -s.smbs[r] * s.population[t, r] * Math.Pow(ypc / ypc90, s.vbel);

                s.malaria[t, r] = s.malbs[r] * s.population[t, r] * s.malch[r] * Math.Pow(s.temp[t, r], s.malnl) * Math.Pow(ypc / ypc90, s.vbel);
            }

        }
    }
}
