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
    public class ParameterElementTriangularDistribution : ParameterElement<double>
    {
        private double _bestGuess;
        private double _min, _max;

        public ParameterElementTriangularDistribution(ParameterElementKey key, double bestGuess, double min, double max)
            : base(key)
        {
            _bestGuess = bestGuess;
            _min = min;
            _max = max;
        }

        public override string ToString()
        {
            return string.Format("~Triangular({0};{1};{2})", _min, _max, _bestGuess);
        }


        public override double GetBestGuessValue()
        {
            return _bestGuess;
        }

        public override double GetRandomValue(Random rand)
        {
            double helper;

            LegacyFund28Math.GenTriang(_min, _max, _bestGuess, out helper, rand);

            return helper;
        }

        public override double Mean { get { return (_bestGuess + _min + _max) / 3.0; } }
        public override double StdDev { get { return Math.Sqrt((_bestGuess * _bestGuess + _min * _min + _max * _max - _bestGuess * _min - _bestGuess * _max - _min * _max) / 18.0); } }

    }
}
