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
    public class ParameterElementExponentialDistribution : ParameterElement<double>
    {
        private double _lambda;
        private double? _lowerBound, _upperBound;

        public ParameterElementExponentialDistribution(ParameterElementKey key, double lambda, double? lowerBound, double? upperBound)
            : base(key)
        {
            _lambda = lambda;
            _lowerBound = lowerBound;
            _upperBound = upperBound;
        }

        public override string ToString()
        {
            string trim = FormatTrim(_lowerBound, _upperBound);
            return string.Format("~Exp({0}{1})", _lambda, trim);
        }


        public override double GetBestGuessValue()
        {
            return 0.0;
        }

        public override double GetRandomValue(Random rand)
        {
            double i_helper;

            LegacyFund28Math.GenExponential(_lambda, out i_helper, rand);
            double value = i_helper;

            while ((_lowerBound.HasValue && (value < _lowerBound.Value)) || (_upperBound.HasValue && (value > _upperBound.Value)))
            {
                LegacyFund28Math.GenExponential(_lambda, out i_helper, rand);
                value = i_helper;
            }

            return value;
        }

        public override double Mean { get { return 1.0 / _lambda; } }
        public override double StdDev { get { return 1.0 / _lambda; } }
    }
}
