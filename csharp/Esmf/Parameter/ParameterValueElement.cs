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

    public abstract class ParameterValueElementNonTyped
    {
        public ParameterElementKey Key { get; private set; }

        protected ParameterValueElementNonTyped(ParameterElementKey key)
        {
            Key = key;
        }

        public abstract object GetUntypedValue();
    }

    public abstract class ParameterValueElement<T> : ParameterValueElementNonTyped
    {
        protected ParameterValueElement(ParameterElementKey key)
            : base(key)
        {
        }

        public T Value { get; protected set; }

        public override object GetUntypedValue()
        {
            return Value;
        }
    }

}
