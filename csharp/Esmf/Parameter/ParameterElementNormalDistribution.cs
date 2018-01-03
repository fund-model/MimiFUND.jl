// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fund;

namespace Esmf
{
    [Serializable]
    public class ParameterElementNormalDistribution : ParameterElement<double>
    {
        private double _bestGuess;
        private double _standardDeviation;
        private double? _lowerBound, _upperBound;

        public ParameterElementNormalDistribution(ParameterElementKey key, double bestGuess, double standardDeviation, double? lowerBound, double? upperBound)
            : base(key)
        {
            _bestGuess = bestGuess;
            _standardDeviation = standardDeviation;
            _lowerBound = lowerBound;
            _upperBound = upperBound;
        }

        public override string ToString()
        {
            string trim = FormatTrim(_lowerBound, _upperBound);
            return string.Format("~N({0};{1}{2})", _bestGuess, _standardDeviation, trim);
        }

        public override double GetBestGuessValue()
        {
            return _bestGuess;
        }

        public override double GetRandomValue(Random rand)
        {
            double i_helper1, i_helper2;

            LegacyFund28Math.GenNormal(_bestGuess, _standardDeviation, out i_helper1, out i_helper2, rand);
            double value = i_helper1;

            while ((_lowerBound.HasValue && (value < _lowerBound.Value)) || (_upperBound.HasValue && (value > _upperBound.Value)))
            {
                LegacyFund28Math.GenNormal(_bestGuess, _standardDeviation, out i_helper1, out i_helper2, rand);
                value = i_helper1;
            }

            return value;
        }

        public override double Mean { get { return _bestGuess; } }
        public override double StdDev { get { return _standardDeviation; } }
    }
}
