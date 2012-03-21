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
    public class ParameterNonDimensional<T> : Parameter
    {
        private ParameterElement<T> _value;

        public ParameterNonDimensional(string name, int id, ParameterElement<T> value)
            : base(name, id)
        {
            _value = value;
        }

        public override IEnumerable<NonTypedParameterElement> GetAllElements()
        {
            yield return _value;
        }

        public T GetBestGuessValue()
        {
            return _value.GetBestGuessValue();
        }

        public ParameterElement<T> Definition { get { return _value; } }

        public T GetRandomValue(Random rand)
        {
            return _value.GetRandomValue(rand);
        }

        internal void SkipRandomValues(Random rand)
        {
            _value.GetRandomValue(rand);
        }
    }
}
