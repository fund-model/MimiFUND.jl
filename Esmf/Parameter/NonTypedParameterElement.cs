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

        public static string FormatTrim(double? lowerBound, double? upperBound)
        {
            string trim0 = lowerBound.HasValue ? String.Format(";min={0}", lowerBound.Value) : "";
            string trim1 = upperBound.HasValue ? String.Format(";max={0}", upperBound.Value) : "";

            return trim0 + trim1;
        }
    }
}
