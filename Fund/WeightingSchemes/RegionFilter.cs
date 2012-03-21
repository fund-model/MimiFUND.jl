// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Fund
{
    public class RegionFilter : WeightingScheme
    {
        private HashSet<int> _regions;

        protected override string GetName()
        {
            return string.Format("regionfilter[{0}]", string.Join(",", _regions.Cast<string>()));
        }

        protected override double GetWeight(int Year, int Region)
        {
            if (_regions.Contains(Region))
                return 1.0;
            else
                return 0.0;
        }

        public override void CalculateWeights(ModelOutput ModelOutput)
        {
        }

        public RegionFilter(int[] regions)
        {
            _regions = new HashSet<int>(regions);
        }

    }
}