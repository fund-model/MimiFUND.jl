// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
namespace Fund
{

    // Discounting with a constant pure rate of time preference
    public class RamseyRegionalDiscounting : WeightingScheme
    {
        private double m_pureRateOfTimePreference;
        private double m_riskAversion;
        private int m_baseYear;
        private string m_name;

        public override void CalculateWeights(ModelOutput ModelOutput)
        {
            double i_newPerCapitaIncome, i_oldPerCapitaIncome;
            double i_perCapitaGrowthRate;
            double df;

            for (int r = 0; r < LegacyConstants.NoReg; r++)
            {
                df = 1;

                for (int t = m_baseYear; t <= LegacyConstants.NYear; t++)
                {
                    AddWeight(t, r, df);
                    i_newPerCapitaIncome = ModelOutput.Incomes[t, r] / ModelOutput.Populations[t, r];
                    i_oldPerCapitaIncome = ModelOutput.Incomes[t - 1, r] / ModelOutput.Populations[t - 1, r];
                    i_perCapitaGrowthRate = (i_newPerCapitaIncome - i_oldPerCapitaIncome) / i_oldPerCapitaIncome;
                    df = df / (1.00 + m_pureRateOfTimePreference + m_riskAversion * i_perCapitaGrowthRate);
                }
            }

        }

        public RamseyRegionalDiscounting(double PureRateOfTimePreference)
            : base()
        {
            m_pureRateOfTimePreference = PureRateOfTimePreference;
            m_baseYear = 50;
            m_riskAversion = 1.0;
            m_name = string.Format("ramseyregionalrate [prtp={0}%, risk aversion={1}, base year={2}]", m_pureRateOfTimePreference * 100, m_riskAversion, m_baseYear + 1950);
        }

        public RamseyRegionalDiscounting(double PureRateOfTimePreference, int baseYear)
            : base()
        {
            m_pureRateOfTimePreference = PureRateOfTimePreference;
            m_baseYear = baseYear;
            m_riskAversion = 1.0;
            m_name = string.Format("ramseyregionalrate [prtp={0}%, risk aversion={1}, base year={2}]", m_pureRateOfTimePreference * 100, m_riskAversion, m_baseYear + 1950);
        }

        protected override string GetName()
        {
            return m_name;
        }

        public RamseyRegionalDiscounting(double PureRateOfTimePreference, int baseYear, string name)
            : base()
        {
            m_pureRateOfTimePreference = PureRateOfTimePreference;
            m_baseYear = baseYear;
            m_name = name;
            m_riskAversion = 1.0;
        }

        public RamseyRegionalDiscounting(double PureRateOfTimePreference, double RiskAversion, int baseYear)
        {
            m_pureRateOfTimePreference = PureRateOfTimePreference;
            m_riskAversion = RiskAversion;
            m_baseYear = baseYear;
            m_name = string.Format("ramsey regional [prtp={0}%, risk aversion={1}, base year={2}]", m_pureRateOfTimePreference * 100, m_riskAversion, m_baseYear + 1950);
        }
    }
}