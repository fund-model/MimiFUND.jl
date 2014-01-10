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
    public class ParameterElementConstant<T> : ParameterElement<T>
    {
        private T _value;

        public ParameterElementConstant(ParameterElementKey key, T value)
            : base(key)
        {
            _value = value;
        }

        public override string ToString()
        {
            if (_value.GetType() == typeof(Timestep))
            {
                return String.Format("{0}y", _value);
            }
            else
                return _value.ToString();
        }

        public override T GetBestGuessValue()
        {
            return _value;
        }

        public override T GetRandomValue(Random rand)
        {
            return _value;
        }

        public override double Mean
        {
            get
            {
                if (typeof(T) != typeof(double))
                {
                    throw new InvalidOperationException();
                }
                return Convert.ToDouble((object)_value);
            }
        }

        public override double StdDev { get { return 0; } }
    }
}
