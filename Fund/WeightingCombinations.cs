// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System.Collections.Generic;
using System;
using Esmf;

namespace Fund
{

    public static class Fund28LegacyWeightingCombinations
    {
        public static void GetWeightingCombinationsFromName(string name, out WeightingCombination[] i_WeightingCombinations, Timestep baseYear)
        {
            switch (name)
            {
                case "Standard": StandardWeightingCombinations(out i_WeightingCombinations, baseYear); break;
                case "Raw": RawWeightingCombinations(out i_WeightingCombinations, baseYear); break;
                default: throw new ApplicationException("Invalid weighting scheme");
            }
        }

        private static void StandardWeightingCombinations(out WeightingCombination[] WeightingCombinations, Timestep baseYear)
        {
            var i_weightingCombinations = new List<WeightingCombination>();

            WeightingCombination w;

            w = new WeightingCombination();
            w.Add(new RamseyRegionalDiscounting(0.00));
            i_weightingCombinations.Add(w);

            w = new WeightingCombination();
            w.Add(new ConstantDiscountrate(0.0));
            w.Add(new EquityWeighting(50, -1, 1.0));
            i_weightingCombinations.Add(w);

            w = new WeightingCombination();
            w.Add(new RamseyRegionalDiscounting(0.01));
            i_weightingCombinations.Add(w);

            w = new WeightingCombination();
            w.Add(new ConstantDiscountrate(0.01));
            w.Add(new EquityWeighting(50, -1, 1.0));
            i_weightingCombinations.Add(w);

            w = new WeightingCombination();
            w.Add(new RamseyRegionalDiscounting(0.03));
            i_weightingCombinations.Add(w);

            w = new WeightingCombination();
            w.Add(new ConstantDiscountrate(0.03));
            w.Add(new EquityWeighting(50, -1, 1.0));
            i_weightingCombinations.Add(w);

            WeightingCombinations = i_weightingCombinations.ToArray();
        }

        private static void RawWeightingCombinations(out WeightingCombination[] WeightingCombinations, Timestep baseYear)
        {
            var i_weightingCombinations = new List<WeightingCombination>();

            WeightingCombination w;

            w = new WeightingCombination("RAW");
            w.Add(new UnityWeight());
            i_weightingCombinations.Add(w);

            WeightingCombinations = i_weightingCombinations.ToArray();
        }

    }
}
