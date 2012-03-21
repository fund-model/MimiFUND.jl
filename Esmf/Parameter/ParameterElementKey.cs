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
    public class ParameterElementKey
    {
        protected int _id;
        protected string _name;

        public ParameterElementKey(int id, string name)
        {
            _id = id;
            _name = name;
        }

        public int Id { get { return _id; } }
        public string Name { get { return _name; } }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            ParameterElementKey p = obj as ParameterElementKey;
            if (p == null)
            {
                return false;
            }

            return _id == p._id;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode() * 31;
        }

        public override string ToString()
        {
            return _name;
        }
    }

    public class ParameterElementKey1Dimensional : ParameterElementKey
    {
        private int _dim1;

        public ParameterElementKey1Dimensional(int id, string name, int dim1)
            : base(id, name)
        {
            _dim1 = dim1;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            ParameterElementKey1Dimensional p = obj as ParameterElementKey1Dimensional;
            if (p == null)
            {
                return false;
            }

            return _id == p._id && _dim1 == p._dim1;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode() * 31 + _dim1;
        }

        public override string ToString()
        {
            return string.Format("{0};{1}", _name, _dim1);
        }

        public int D1 { get { return _dim1; } }

    }

    public class ParameterElementKey2Dimensional : ParameterElementKey
    {
        private int _dim1;
        private int _dim2;

        public ParameterElementKey2Dimensional(int id, string name, int dim1, int dim2)
            : base(id, name)
        {
            _dim1 = dim1;
            _dim2 = dim2;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            ParameterElementKey2Dimensional p = obj as ParameterElementKey2Dimensional;
            if (p == null)
            {
                return false;
            }

            return _id == p._id && _dim1 == p._dim1 && _dim2 == p._dim2;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode() * 31 + _dim1 + _dim2 * 11;
        }

        public override string ToString()
        {
            return string.Format("{0};{1};{2}", _name, _dim1, _dim2);
        }

        public int D1 { get { return _dim1; } }
        public int D2 { get { return _dim2; } }

    }
}
