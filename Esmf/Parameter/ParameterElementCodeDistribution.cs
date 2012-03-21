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
    public class ParameterElementCodeDistribution : ParameterElement<double>
    {
        private IEnumerator<double> _enumerator;
        private double _bestGuess;

        public ParameterElementCodeDistribution(ParameterElementKey key, double bestGuess, IEnumerable<double> enumerator)
            : base(key)
        {
            _bestGuess = bestGuess;
            _enumerator = enumerator.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Format("~Code({0})", _bestGuess);
        }


        public override double GetBestGuessValue()
        {
            return _bestGuess;
        }

        public override double GetRandomValue(Random rand)
        {
            if (!_enumerator.MoveNext())
                throw new InvalidOperationException();

            return _enumerator.Current;
        }

        public override double Mean { get { throw new NotImplementedException(); } }
        public override double StdDev { get { throw new NotImplementedException(); } }

    }
}