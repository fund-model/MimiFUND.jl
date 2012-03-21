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
    public class Parameter2Dimensional<T> : Parameter
    {
        private ParameterElement<T>[,] _value;
        private JaggedArrayWrapper2<T> _cachedConstantValues;

        public Parameter2Dimensional(string name, int id, ParameterElement<T>[,] value)
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
            int l0 = _value.GetLength(0);
            int l1 = _value.GetLength(1);

            for (int i = 0; i < l0; i++)
            {
                for (int l = 0; l < l1; l++)
                {
                    yield return _value[i, l];
                }
            }
        }

        public JaggedArrayWrapper2<T> GetBestGuessValues()
        {
            if (_cachedConstantValues == null)
            {
                int l0 = _value.GetLength(0);
                int l1 = _value.GetLength(1);

                JaggedArrayWrapper2<T> values = new JaggedArrayWrapper2<T>(l0, l1);

                for (int i = 0; i < l0; i++)
                {
                    for (int l = 0; l < l1; l++)
                    {
                        values[i, l] = _value[i, l].GetBestGuessValue();
                    }
                }

                return values;
            }
            else
                return _cachedConstantValues;
        }

        public JaggedArrayWrapper2<T> GetRandomValues(Random rand)
        {
            if (_cachedConstantValues == null)
            {
                int l0 = _value.GetLength(0);
                int l1 = _value.GetLength(1);

                JaggedArrayWrapper2<T> values = new JaggedArrayWrapper2<T>(l0, l1);

                for (int i = 0; i < l0; i++)
                {
                    for (int l = 0; l < l1; l++)
                    {
                        values[i, l] = _value[i, l].GetRandomValue(rand);
                    }
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
                int l0 = _value.GetLength(0);
                int l1 = _value.GetLength(1);

                for (int i = 0; i < l0; i++)
                {
                    for (int l = 0; l < l1; l++)
                    {
                        _value[i, l].GetRandomValue(rand);
                    }
                }
            }
        }
    }
}