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
    public class FieldParameter1Dimensional<D1, T> : IParameter1DimensionalTypeless<T>, IParameter1Dimensional<D1, T>
        where D1 : IDimension
    {
        private JaggedArrayWrapper<T> _values;
        private ModelOutput _parent;

        public FieldParameter1Dimensional(ModelOutput parent, ParameterValue1Dimensional<T> values)
        {
            _parent = parent;
            _values = values.GetPointerToData();
        }

        public T this[D1 index]
        {
            get
            {
                return _values[index.Index];
            }
        }

        T IParameter1Dimensional<D1, T>.this[D1 index]
        {
            get
            {
                return this[index];

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
    }
}
