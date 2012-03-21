// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;

namespace Fund
{

    public class ConstantDiscountrate : WeightingScheme
    {
        private double m_rate;
        private int m_baseYear;

        protected override string GetName()
        {
            return string.Format("constantdiscountrate [{0}%]", m_rate * 100);
        }

        protected override double GetWeight(int Year, int Region)
        {
            if (Year < m_baseYear)
                throw new Exception("this is not supported");

            return 1.0 / Math.Pow(1 + m_rate, Year - m_baseYear);
        }

        public ConstantDiscountrate(double discountRate)
        {
            m_rate = discountRate;
            m_baseYear = 50;
        }

        public override void CalculateWeights(ModelOutput ModelOutput)
        {
        }

        public ConstantDiscountrate(double discountRate, int baseYear)
        {
            m_rate = discountRate;
            m_baseYear = baseYear;
        }
    }
}