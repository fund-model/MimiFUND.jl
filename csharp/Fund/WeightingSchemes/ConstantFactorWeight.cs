// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
namespace Fund
{

    public class ConstantFactorWeight : WeightingScheme
    {
        private double m_weight;
        public string m_name;

        protected override string GetName()
        {
            return m_name;
        }

        protected override double GetWeight(int Year, int Region)
        {
            return m_weight;
        }

        public override void CalculateWeights(ModelOutput ModelOutput)
        {
        }

        public ConstantFactorWeight(double weight, string name)
        {
            m_weight = weight;
            m_name = name;
        }

    }
}