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
    public class FieldParameter1DimensionalTime<T> : IParameter1DimensionalTypeless<T>, IParameter1Dimensional<Timestep, T>
    {
        private JaggedArrayWrapper<T> _values;
        private ModelOutput _parent;

        public FieldParameter1DimensionalTime(ModelOutput parent, ParameterValue1Dimensional<T> values)
        {
            _parent = parent;
            _values = values.GetPointerToData();
        }

        public T this[Timestep index]
        {
            get
            {

                return _values[index.Value];
            }
        }

        T IParameter1Dimensional<Timestep, T>.this[Timestep index]
        {
            get
            {
                return this[index];
            }
        }

        IEnumerable<Parameter1DimensionalMember<T>> IParameter1DimensionalTypeless<T>.GetEnumerator()
        {
            for (int i = 0; i < _values.Length; i++)
            {
                yield return new Parameter1DimensionalMember<T>(Timestep.FromSimulationYear(i), _values[i], _parent.AttachedDimensions);
            }
        }
    }
}
