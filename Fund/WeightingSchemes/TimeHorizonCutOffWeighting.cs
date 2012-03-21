// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using Esmf;

namespace Fund
{

    public class TimeHorizonCutOffWeighting : WeightingScheme
    {
        private Timestep m_cutoffYear;
        private string m_name;
        private int m_baseYear;

        protected override string GetName()
        {
            return m_name;
        }

        public TimeHorizonCutOffWeighting(Timestep cutoffYear)
            : base()
        {
            m_baseYear = 50;
            m_cutoffYear = cutoffYear;
            m_name = string.Format("cut off [year={0}]", m_cutoffYear);
        }

        public TimeHorizonCutOffWeighting(Timestep cutoffYear, string name)
            : base()
        {
            m_baseYear = 50;
            m_cutoffYear = cutoffYear;
            m_name = name;
        }

        public override void CalculateWeights(ModelOutput ModelOutput)
        {
            for (int i = 0; i < LegacyConstants.NoReg; i++)
            {
                for (int l = m_baseYear; l <= LegacyConstants.NYear; l++)
                {
                    if (l >= m_cutoffYear.Value)
                        AddWeight(l, i, 0.0);
                    else
                        AddWeight(l, i, 1.0);
                }
            }
        }
    }
}