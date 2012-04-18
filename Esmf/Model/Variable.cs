// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmf.Model
{
    public class Variable
    {
        private string _name;
        private Type[] _dimensionTypes;
        private Type _dataType;

        public Variable(string name, Type[] dimensionTypes, Type dataType)
        {
            // TODO: Complete member initialization
            this._name = name;
            this._dimensionTypes = dimensionTypes;
            this._dataType = dataType;
            this.StoreOutput = true;
        }

        public string Name { get { return _name; } }

        public IEnumerable<Type> DimensionTypes { get { return _dimensionTypes; } }

        public Type DataType { get { return _dataType; } }

        public bool StoreOutput { get; set; }

        public override string ToString()
        {
            return _name;
        }
    }
}
