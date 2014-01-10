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
    public class ParameterElementGammaDistribution : ParameterElement<double>
    {
        private double _alpha, _beta;
        private double? _lowerBound;
        private double? _upperBound;

        public ParameterElementGammaDistribution(ParameterElementKey key, double alpha, double beta, double? lowerBound, double? upperBound)
            : base(key)
        {
            _alpha = alpha;
            _beta = beta;
            _lowerBound = lowerBound;
            _upperBound = upperBound;
        }

        public override string ToString()
        {
            string trim = FormatTrim(_lowerBound, _upperBound);
            return string.Format("~Gamma({0};{1}{2})", _alpha, _beta, trim);
        }

        public override double GetBestGuessValue()
        {
            return _beta * (_alpha - 1.0);
        }

        public override double GetRandomValue(Random rand)
        {
            double i_helper;

            LegacyFund28Math.GenGamma(_alpha, _beta, out i_helper, rand);
            double value = i_helper;

            while ((_lowerBound.HasValue && (value < _lowerBound.Value)) ||
                (_upperBound.HasValue && (value > _upperBound.Value)))
            {
                LegacyFund28Math.GenGamma(_alpha, _beta, out i_helper, rand);
                value = i_helper;
            }

            return value;
        }

        public override double Mean { get { return _alpha * _beta; } }
        public override double StdDev { get { return Math.Sqrt(_alpha * _beta * _beta); } }

    }
}
