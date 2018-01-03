// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using Fund.CommonDimensions;
using Esmf;

namespace Fund
{

    public class EquityWeighting : WeightingScheme
    {
        private int m_baseYear;
        private int m_baseRegion;
        private double m_inequalityAversion;

        public override void CalculateWeights(ModelOutput ModelOutput)
        {
            double i_totalIncome, i_totalPopulation;
            double i_baseYpC;
            double i_YpC;

            if (m_baseRegion == -1)
            {
                i_totalIncome = 0;
                i_totalPopulation = 0;
                for (int r = 0; r < LegacyConstants.NoReg; r++)
                {
                    i_totalIncome = i_totalIncome + ModelOutput.Incomes[m_baseYear, r];
                    i_totalPopulation = i_totalPopulation + ModelOutput.Populations[m_baseYear, r];
                }
                i_baseYpC = i_totalIncome / i_totalPopulation;
            }
            else
                i_baseYpC = ModelOutput.Incomes[m_baseYear, m_baseRegion] / ModelOutput.Populations[m_baseYear, m_baseRegion];

            for (int r = 0; r < LegacyConstants.NoReg; r++)
            {
                // TODO -oDavid Anthoff: Change start of "real" period to constant
                for (int t = 50; t <= LegacyConstants.NYear; t++)
                {
                    i_YpC = ModelOutput.Incomes[t, r] / ModelOutput.Populations[t, r];
                    AddWeight(t, r, Math.Pow(i_baseYpC / i_YpC, m_inequalityAversion));
                }
            }
        }

        public EquityWeighting(int BaseYear, int BaseRegion, double InequalityAversion)
            : base()
        {
            m_baseYear = BaseYear;
            m_baseRegion = BaseRegion;
            m_inequalityAversion = InequalityAversion;
        }

        protected override string GetName()
        {
            // TODO -oDavid Anthoff: Exchange 1950 with constant
            return String.Format("equity weight [baseyear={0}, baseregion={1}, inequalityaversion={2}]", m_baseYear + 1950, m_baseRegion == -1 ? "world" : m_baseRegion.ToString(), m_inequalityAversion);
        }
    }
}