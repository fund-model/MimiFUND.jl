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
    public abstract class ParameterElement<T> : NonTypedParameterElement
    {
        protected ParameterElement(ParameterElementKey key)
            : base(key)
        {
        }

        public abstract T GetBestGuessValue();

        public abstract T GetRandomValue(Random rand);

        public abstract double Mean { get; }
        public abstract double StdDev { get; }

        public override Type GetElementType()
        {
            return typeof(T);
        }
    }
}
