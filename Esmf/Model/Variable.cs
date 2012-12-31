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
        private Esmf.Model.ParameterValue _forced;

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

        public void ForceFromFile(string parametername=null)
        {
            _forced = new ParameterValueFile(parametername);
        }

        public void Force(string componentName, string variableName)
        {
            var b = new ParameterValueBound(componentName, variableName);
            _forced = b;
        }

        public void Force(string componentName)
        {
            Force(componentName, _name);
        }

        public ParameterValue Forced
        {
            get
            {
                return _forced;
            }
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
