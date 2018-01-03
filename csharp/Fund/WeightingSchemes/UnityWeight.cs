// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
namespace Fund
{

    public class UnityWeight : WeightingScheme
    {
        protected override string GetName()
        {
            return "No weighting";
        }

        protected override double GetWeight(int Year, int Region)
        {
            return 1.0;
        }

        public override void CalculateWeights(ModelOutput ModelOutput)
        {
        }
    }
}