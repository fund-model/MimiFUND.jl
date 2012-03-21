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
    public class ParameterValueElement1Dimensional<T> : ParameterValueElement<T>
    {
        public ParameterValueElement1Dimensional(ParameterElementKey key, string name, int id, int index, T value)
            : base(key)
        {
            Name = name;
            Id = id;
            Index = index;
            Value = value;
        }

        public string Name { get; private set; }
        public int Id { get; private set; }
        public int Index { get; private set; }
    }
}
