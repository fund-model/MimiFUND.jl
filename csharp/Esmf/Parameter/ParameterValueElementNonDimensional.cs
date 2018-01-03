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
    public class ParameterValueElementNonDimensional<T> : ParameterValueElement<T>
    {
        public ParameterValueElementNonDimensional(ParameterElementKey key, string name, int id, T value)
            : base(key)
        {
            Name = name;
            Id = id;
            Value = value;
        }

        public string Name { get; private set; }
        public int Id { get; private set; }
    }
}
