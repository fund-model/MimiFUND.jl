// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fund;

namespace Esmf
{
    [Serializable]
    public abstract class NonTypedParameterElement
    {
        protected NonTypedParameterElement(ParameterElementKey key)
        {
            Key = key;
        }

        public ParameterElementKey Key { get; private set; }
        public abstract Type GetElementType();
    }
}
