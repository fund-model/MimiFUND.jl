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
    public class FieldVariable2Dimensional<D1, D2, T> : IParameter2DimensionalTypeless<T>, IVariable2Dimensional<D1, D2, T>, IParameter2Dimensional<D1, D2, T>, IVariableWriter
        where D1 : IDimension
        where D2 : IDimension
    {
        private JaggedArrayWrapper2<T> _values;
        private bool[,] _valuesHasBeenSet;
        private ModelOutput _parent;

        public FieldVariable2Dimensional(ModelOutput parent)
        {
            _parent = parent;

            var d1 = _parent.Dimensions.GetDimension<D1>();
            var d2 = _parent.Dimensions.GetDimension<D2>();

            _values = new JaggedArrayWrapper2<T>(d1.Count, d2.Count);

            ControlledConstructor(d1.Count, d2.Count, false);
        }

        [Conditional("FUNDCHECKED")]
        private void ControlledConstructor(int count0, int count1, bool setAllToSet)
        {
            _valuesHasBeenSet = new bool[count0, count1];

            if (setAllToSet)
            {
                for (int i = 0; i < count0; i++)
                {
                    for (int l = 0; l < count1; l++)
                    {
                        _valuesHasBeenSet[i, l] = true;
                    }
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
        private void CheckForValidIndex(D1 index1, D2 index2)
        {
            if (_valuesHasBeenSet[index1.Index, index2.Index] == false)
                throw new ArgumentOutOfRangeException("Value for this index has not been set");
        }

        [Conditional("FUNDCHECKED")]
        private void ValueSetForIndex(D1 index1, D2 index2)
        {
            _valuesHasBeenSet[index1.Index, index2.Index] = true;
        }

        public T this[D1 index1, D2 index2]
        {
            get
            {
                CheckForValidIndex(index1, index2);
                return _values[index1.Index, index2.Index];
            }
            set
            {
                CheckForValidValue(value);
                ValueSetForIndex(index1, index2);

                _values[index1.Index, index2.Index] = value;
            }
        }

        T IVariable2Dimensional<D1, D2, T>.this[D1 index1, D2 index2]
        {
            get
            {
                return this[index1, index2];
            }
            set
            {
                this[index1, index2] = value;
            }
        }

        T IParameter2Dimensional<D1, D2, T>.this[D1 index1, D2 index2]
        {
            get
            {
                return this[index1, index2];
            }
        }

        IEnumerable<Parameter2DimensionalMember<T>> IParameter2DimensionalTypeless<T>.GetEnumerator()
        {
            var dimensionValues1 = _parent.Dimensions.GetValues<D1>();
            var dimensionValues2 = _parent.Dimensions.GetValues<D2>();
            int l1 = dimensionValues1.Length;
            int l2 = dimensionValues2.Length;

            for (int i = 0; i < l1; i++)
            {
                for (int l = 0; l < l2; l++)
                {
                    yield return new Parameter2DimensionalMember<T>(dimensionValues1[i], dimensionValues2[l], _values[i, l], _parent.AttachedDimensions);
                }
            }
        }

        void IVariableWriter.WriteData(StreamWriter file)
        {
            for (int i = 0; i < _values.Length0; i++)
            {
                for (int j = 0; j < _values.Length1; j++)
                {
                    if(j>0)
                    {
                        file.Write(",");
                    }
                    file.Write(_values[i, j]);
                }
                file.WriteLine();
            }
        }
    }
}
