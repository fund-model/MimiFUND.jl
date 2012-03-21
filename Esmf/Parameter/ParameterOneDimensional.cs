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
    public class ParameterOneDimensional<T> : Parameter
    {
        private ParameterElement<T>[] _value;
        private JaggedArrayWrapper<T> _cachedConstantValues;

        public ParameterOneDimensional(string name, int id, ParameterElement<T>[] value)
            : base(name, id)
        {
            _value = value;

            bool uncertain = false;

            foreach (var e in GetAllElements())
            {
                if (!(e is ParameterElementConstant<T>))
                {
                    uncertain = true;
                }
            }

            if (!uncertain)
            {
                _cachedConstantValues = GetBestGuessValues();
            }
        }

        public override IEnumerable<NonTypedParameterElement> GetAllElements()
        {
            for (int i = 0; i < _value.Length; i++)
            {
                yield return _value[i];
            }
        }

        public JaggedArrayWrapper<T> GetBestGuessValues()
        {
            if (_cachedConstantValues == null)
            {
                JaggedArrayWrapper<T> values = new JaggedArrayWrapper<T>(_value.Length);

                for (int i = 0; i < _value.Length; i++)
                {
                    values[i] = _value[i].GetBestGuessValue();
                }

                return values;
            }
            else
                return _cachedConstantValues;
        }

        public ParameterElement<T>[] Definition { get { return _value; } }


        public JaggedArrayWrapper<T> GetRandomValues(Random rand)
        {
            if (_cachedConstantValues == null)
            {
                JaggedArrayWrapper<T> values = new JaggedArrayWrapper<T>(_value.Length);

                for (int i = 0; i < _value.Length; i++)
                {
                    values[i] = _value[i].GetRandomValue(rand);
                }

                return values;
            }
            else
                return _cachedConstantValues;
        }

        internal void SkipRandomValues(Random rand)
        {
            if (_cachedConstantValues == null)
            {
                for (int i = 0; i < _value.Length; i++)
                {
                    _value[i].GetRandomValue(rand);
                }
            }
        }
    }
}
