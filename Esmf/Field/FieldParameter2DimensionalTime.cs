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
    public class FieldParameter2DimensionalTime<D2, T> : IParameter2DimensionalTypeless<T>, IParameter2Dimensional<Timestep, D2, T>
        where D2 : IDimension
    {
        private JaggedArrayWrapper2<T> _values;
        private ModelOutput _parent;

        public FieldParameter2DimensionalTime(ModelOutput parent, ParameterValue2Dimensional<T> values)
        {
            _parent = parent;
            _values = values.GetPointerToData();
        }

        public T this[Timestep index1, D2 index2]
        {
            get
            {
                return _values[index1.Value, index2.Index];
            }
        }

        T IParameter2Dimensional<Timestep, D2, T>.this[Timestep index1, D2 index2]
        {
            get
            {
                return this[index1, index2];
            }
        }

        IEnumerable<Parameter2DimensionalMember<T>> IParameter2DimensionalTypeless<T>.GetEnumerator()
        {
            var dimensionValues = _parent.Dimensions.GetValues<D2>();
            int dimCount = dimensionValues.Length;

            for (int i = 0; i < _values.Length0; i++)
            {
                for (int l = 0; l < dimCount; l++)
                {
                    yield return new Parameter2DimensionalMember<T>(Timestep.FromSimulationYear(i), dimensionValues[l], _values[i, l], _parent.AttachedDimensions);
                }
            }
        }
    }
}
