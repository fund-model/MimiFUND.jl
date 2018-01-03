// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmf
{
    [Serializable]
    public class ParameterElementEmpiricalDistribution : ParameterElement<double>
    {
        private double[] _values;
        private double _bestGuess;
        private int _currentPos;

        public ParameterElementEmpiricalDistribution(ParameterElementKey key, double bestGuess, IEnumerable<double> values)
            : base(key)
        {
            _bestGuess = bestGuess;
            _values = values.ToArray();
            _currentPos = -1;
        }

        public ParameterElementEmpiricalDistribution(ParameterElementKey key, double bestGuess, double[] values)
            : base(key)
        {
            _bestGuess = bestGuess;
            _values = (double[]) values.Clone();
            _currentPos = -1;
        }

        public override string ToString()
        {
            return string.Format("~Empirical({0})", _bestGuess);
        }


        public override double GetBestGuessValue()
        {
            return _bestGuess;
        }

        public override double GetRandomValue(Random rand)
        {
            _currentPos++;
            return _values[_currentPos];
        }

        public override double Mean { get { return _values.Average(); } }
        public override double StdDev { get { throw new NotImplementedException(); } }

    }
}
