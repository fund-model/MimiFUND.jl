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
    public class FieldParameter2Dimensional<D1, D2, T> : IParameter2DimensionalTypeless<T>, IParameter2Dimensional<D1, D2, T>
        where D1 : IDimension
        where D2 : IDimension
    {
        private JaggedArrayWrapper2<T> _values;
        private ModelOutput _parent;


        public FieldParameter2Dimensional(ModelOutput parent, ParameterValue2Dimensional<T> values)
        {
            _parent = parent;
            _values = values.GetPointerToData();
        }

        public T this[D1 index1, D2 index2]
        {
            get
            {
                return _values[index1.Index, index2.Index];
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
    }
}
