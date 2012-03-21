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
    public class ParameterValueElement2Dimensional<T> : ParameterValueElement<T>
    {
        public ParameterValueElement2Dimensional(ParameterElementKey key, string name, int id, int index1, int index2, T value)
            : base(key)
        {
            Name = name;
            Id = id;
            Index1 = index1;
            Index2 = index2;
            Value = value;
        }

        public string Name { get; private set; }
        public int Id { get; private set; }
        public int Index1 { get; private set; }
        public int Index2 { get; private set; }
    }
}
