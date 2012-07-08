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
    public abstract class FieldVariable0DimensionalTypeless
    {
        public abstract object GetFieldGetter();
        public abstract object GetFieldSetter();
    }

    public class FieldVariable0Dimensional<T> : FieldVariable0DimensionalTypeless
        where T : struct
    {
        private T _value;
        private bool _hasValue;
        private ModelOutput _parent;
        private NonDimensionalFieldGetter<T> _getter;
        private NonDimensionalFieldSetter<T> _setter;

        public FieldVariable0Dimensional(ModelOutput modelOutput)
        {
            _parent = modelOutput;

            _getter = delegate
            {
                CheckForValidIndex();
                return _value;
            };

            _setter = delegate(T value)
            {
                CheckForValidValue(value);
                ValueSetForIndex();
                _value = value;
            };
        }

        [Conditional("FUNDCHECKED")]
        private void CheckForValidValue(object v)
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
        private void CheckForValidIndex()
        {
            if (!_hasValue)
                throw new ArgumentOutOfRangeException("Value for this field has not been set");
        }

        [Conditional("FUNDCHECKED")]
        private void ValueSetForIndex()
        {
            _hasValue = true;
        }


        public T Value
        {
            get
            {
                CheckForValidIndex();
                return _value;
            }
            set
            {
                CheckForValidValue(value);
                ValueSetForIndex();
                _value = value;

            }
        }

        public override object GetFieldGetter()
        {
            return _getter;
        }

        public override object GetFieldSetter()
        {
            return _setter;
        }
    }
}
