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
    public interface IGlobalHotellingTaxationState
    {
        double basetax { get; }
        Timestep baseyear { get; }

        Double prtp { get; }
        Double elasticityofmarginalutility { get; }

        IParameter1Dimensional<Timestep, double> population { get; }
        IParameter1Dimensional<Timestep, double> consumption { get; }

        IVariable2Dimensional<Timestep, Region, double> currtax { get; }
        IVariable2Dimensional<Timestep, Region, double> currtaxch4 { get; }
        IVariable2Dimensional<Timestep, Region, double> currtaxn2o { get; }
    }

    public interface IRegionalHotellingTaxationState
    {
        IParameter1Dimensional<Region, double> basetax { get; }
        Timestep baseyear { get; }

        Double prtp { get; }
        Double elasticityofmarginalutility { get; }

        IParameter2Dimensional<Timestep, Region, double> population { get; }
        IParameter2Dimensional<Timestep, Region, double> consumption { get; }

        IVariable2Dimensional<Timestep, Region, double> currtax { get; }
        IVariable2Dimensional<Timestep, Region, double> currtaxch4 { get; }
        IVariable2Dimensional<Timestep, Region, double> currtaxn2o { get; }
    }


    public class GlobalHotellingTaxationComponent
    {
        public void Run(Clock clock, IGlobalHotellingTaxationState state, IDimensions dimensions)
        {
            var t = clock.Current;
            var s = state;

            var perCapitaConsumptionNow = s.consumption[t] / s.population[t];
            var perCapitaConsumptionPrevious = s.consumption[t - 1] / s.population[t - 1];

            var perCapitaGrowthRate = perCapitaConsumptionNow / perCapitaConsumptionPrevious - 1.0;

            // TODO Think whether this makes sense
            if (perCapitaConsumptionNow == 0.0 && perCapitaConsumptionPrevious == 0.0)
                perCapitaGrowthRate = 0.0;

            var discountrate = perCapitaGrowthRate * s.elasticityofmarginalutility + s.prtp;

            foreach (var r in dimensions.GetValues<Region>())
            {
                if (t < s.baseyear)
                    s.currtax[t, r] = 0.0;
                else if (t == s.baseyear)
                    s.currtax[t, r] = s.basetax;
                else
                    s.currtax[t, r] = s.currtax[t - 1, r] * (1.0 + discountrate);


                s.currtaxn2o[t, r] = s.currtax[t, r];
                s.currtaxch4[t, r] = s.currtax[t, r];

            }
        }
    }

    public class RegionalHotellingTaxationComponent
    {
        public void Run(Clock clock, IRegionalHotellingTaxationState state, IDimensions dimensions)
        {
            var t = clock.Current;
            var s = state;

            foreach (var r in dimensions.GetValues<Region>())
            {
                var perCapitaConsumptionNow = s.consumption[t, r] / s.population[t, r];
                var perCapitaConsumptionPrevious = s.consumption[t - 1, r] / s.population[t - 1, r];

                var perCapitaGrowthRate = perCapitaConsumptionNow / perCapitaConsumptionPrevious - 1.0;

                var discountrate = perCapitaGrowthRate * s.elasticityofmarginalutility + s.prtp;

                if (t < s.baseyear)
                    s.currtax[t, r] = 0.0;
                else if (t == s.baseyear)
                    s.currtax[t, r] = s.basetax[r];
                else
                    s.currtax[t, r] = s.currtax[t - 1, r] * (1.0 + discountrate);


                s.currtaxn2o[t, r] = s.currtax[t, r];
                s.currtaxch4[t, r] = s.currtax[t, r];

            }
        }
    }
}
