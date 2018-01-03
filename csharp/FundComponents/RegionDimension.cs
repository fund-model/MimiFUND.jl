// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmf;

namespace Fund.CommonDimensions
{
    public struct Region : IDimension
    {
        private int _value;
        private Dimension<Region> _parent;

        public Region(int value, Dimension<Region> parent)
        {
            _value = value;
            _parent = parent;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is Region))
                return false;

            Region o = (Region)obj;

            if (o._parent != this._parent)
            {
                return o._parent.Names[o._value] == this._parent.Names[this._value];
            }
            else
            {
                return o._value == this._value;
            }
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(Region t1, Region t2)
        {
            if (t1._parent != t2._parent)
                throw new InvalidOperationException("Need to think about this case");

            return t1._value == t2._value;
        }

        public static bool operator !=(Region t1, Region t2)
        {
            if (t1._parent != t2._parent)
                throw new InvalidOperationException("Need to think about this case");

            return t1._value != t2._value;
        }

        public int Index { get { return _value; } }

        public override string ToString()
        {
            return _parent.Names[_value];
        }
    }
}
