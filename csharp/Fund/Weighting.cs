// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System.Collections;
using System.Collections.Generic;
using Esmf;
using System;

namespace Fund
{

    // Abstract base class for a specific weighting scheme
    // To implement a specific weighting scheme, derive from this
    // class
    public abstract class WeightingScheme
    {
        private JaggedArrayWrapper2<double> m_weights = new JaggedArrayWrapper2<double>(LegacyConstants.NYear + 1, LegacyConstants.NoReg);

        // Return the name of the weighting scheme
        // The name is going to be used by output modules
        protected abstract string GetName();

        // Return the weight for the specified year and region
        protected virtual double GetWeight(int Year, int Region)
        {
            return m_weights[Year, Region];
        }

        protected void AddWeight(int Year, int Region, double WeightData)
        {
            m_weights[Year, Region] = WeightData;
        }

        public abstract void CalculateWeights(ModelOutput ModelOutput);

        public string Name { get { return GetName(); } }
        public double this[int Year, int Region] { get { return GetWeight(Year, Region); } }
    }

    public class WeightingCombination
    {
        private List<WeightingScheme> m_weightingSchemes = new List<WeightingScheme>();
        private string m_name = null;
        protected string GetName()
        {
            string i_name;

            if (m_name == null)
            {
                if (m_weightingSchemes.Count > 0)
                {
                    i_name = "";
                    for (int i = 0; i < m_weightingSchemes.Count; i++)
                    {
                        if (i > 0)
                            i_name = i_name + " + ";
                        i_name = i_name + m_weightingSchemes[i].Name;
                    }
                }
                else
                    i_name = "none";
            }
            else
                i_name = m_name;

            return i_name;
        }

        protected virtual double GetWeight(int Year, int Region)
        {
            double i_weight;

            i_weight = 1.0;
            for (int i = 0; i < m_weightingSchemes.Count; i++)
                i_weight = i_weight * m_weightingSchemes[i][Year, Region];

            return i_weight;

        }

        public string Name { get { return GetName(); } }
        public double this[int Year, int Region] { get { return GetWeight(Year, Region); } }

        public void Add(WeightingScheme WeightingScheme)
        {
            m_weightingSchemes.Add(WeightingScheme);
        }

        public double AddDamagesUp(Damages Damages, int yearsToAggregate, Timestep emissionPeriod)
        {
            double i_totalDamage = 0.0;

            for (int year = emissionPeriod.Value; year < Math.Min(LegacyConstants.NYear, emissionPeriod.Value + yearsToAggregate); year++)
            {
                for (int region = 0; region < LegacyConstants.NoReg; region++)
                {
                    for (int sector = 0; sector < LegacyConstants.NoSector; sector++)
                    {
                        i_totalDamage += Damages[year, region, (Sector)sector] * this[year, region];
                    }
                }
            }

            return i_totalDamage;
        }

        public void CalculateWeights(ModelOutput ModelOutput)
        {
            for (int i = 0; i < m_weightingSchemes.Count; i++)
                m_weightingSchemes[i].CalculateWeights(ModelOutput);
        }

        public WeightingCombination()
        {

        }

        public WeightingCombination(string name)
        {
            m_name = name;
        }


    }

}