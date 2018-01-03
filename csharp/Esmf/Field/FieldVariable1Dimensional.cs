// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Esmf
{
    public class FieldVariable1Dimensional<D1, T> : IParameter1DimensionalTypeless<T>, IVariable1Dimensional<D1, T>, IParameter1Dimensional<D1, T>, IVariableWriter
        where D1 : IDimension
    {
        private JaggedArrayWrapper<T> _values;
        private bool[] _valuesHasBeenSet;
        private ModelOutput _parent;

        public FieldVariable1Dimensional(ModelOutput parent)
        {
            _parent = parent;
            var d1 = _parent.Dimensions.GetDimension<D1>();
            _values = new JaggedArrayWrapper<T>(d1.Count);
            ControlledConstructor(d1.Count, false);
        }

        [Conditional("FUNDCHECKED")]
        private void ControlledConstructor(int count, bool setAllToSet)
        {
            _valuesHasBeenSet = new bool[count];

            if (setAllToSet)
            {
                for (int i = 0; i < _valuesHasBeenSet.Length; i++)
                {
                    _valuesHasBeenSet[i] = true;
                }
            }
        }

        [Conditional("FUNDCHECKED")]
        private void CheckForValidValue(T v)
        {
            if (v.GetType() == typeof(double))
            {
                double d = ((double)((object)v));

                if (double.IsNaN(d))
                    throw new ArgumentOutOfRangeException("NaN is not allowed as a value");
                else if (double.IsInfinity(d))
                    throw new ArgumentOutOfRangeException("Infinity is not allowed as a value");
            }
        }

        [Conditional("FUNDCHECKED")]
        private void CheckForValidIndex(D1 index)
        {
            if (_valuesHasBeenSet[index.Index] == false)
                throw new ArgumentOutOfRangeException("Value for this index has not been set");
        }

        [Conditional("FUNDCHECKED")]
        private void ValueSetForIndex(D1 index)
        {
            _valuesHasBeenSet[index.Index] = true;
        }

        public T this[D1 index]
        {
            get
            {
                CheckForValidIndex(index);

                return _values[index.Index];
            }
            set
            {
                CheckForValidValue(value);
                ValueSetForIndex(index);

                _values[index.Index] = value;
            }
        }

        T IParameter1Dimensional<D1, T>.this[D1 index]
        {
            get
            {
                return this[index];

            }
        }

        T IVariable1Dimensional<D1, T>.this[D1 index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = value;
            }
        }


        IEnumerable<Parameter1DimensionalMember<T>> IParameter1DimensionalTypeless<T>.GetEnumerator()
        {
            var dimensionValues = _parent.Dimensions.GetValues<D1>();

            for (int i = 0; i < dimensionValues.Length; i++)
            {
                yield return new Parameter1DimensionalMember<T>(dimensionValues[i], _values[i], _parent.AttachedDimensions);
            }
        }

        public IEnumerable<T> EnumerateValues()
        {
            for (int i = 0; i < _values.Length; i++)
            {
                yield return _values[i];
            }
        }

        void IVariableWriter.WriteData(StreamWriter file)
        {
            for(int i=0;i<_values.Length;i++)
            {
                file.WriteLine(_values[i]);
            }
        }
    }
}
