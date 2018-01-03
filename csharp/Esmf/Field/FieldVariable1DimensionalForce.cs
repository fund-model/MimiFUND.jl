// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Esmf
{
    public abstract class FieldVariable1DimensionalForceTypeless
    {
        public abstract void SetSource(object value);
    }

    public class FieldVariable1DimensionalForce<D1, T> : FieldVariable1DimensionalForceTypeless, IParameter1DimensionalTypeless<T>, IVariable1Dimensional<D1, T>, IParameter1Dimensional<D1, T>
        where D1 : IDimension
    {
        private IParameter1Dimensional<D1, T> _values;
        private ModelOutput _parent;

        public FieldVariable1DimensionalForce(ModelOutput parent)
        {
            _parent = parent;
        }

        public override void SetSource(object value)
        {
            _values = (IParameter1Dimensional<D1, T>)value;
        }

        public T this[D1 index]
        {
            get
            {
                return _values[index];
            }
            set
            {
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
            foreach (var i in _values.GetEnumerator())
            {
                yield return i;
            }
        }

        public IEnumerable<T> EnumerateValues()
        {
            foreach (var i in _values.GetEnumerator())
            {
                yield return i.Value;
            }
        }
    }
}
