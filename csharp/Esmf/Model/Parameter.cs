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
    public class Parameter
    {
        private string _name;
        private Type[] _dimensionTypes;
        private Type _dataType;
        private Esmf.Model.ParameterValue _value;

        public Parameter(string name, Type[] dimensionTypes, Type dataType)
        {
            this._name = name;
            this._dimensionTypes = dimensionTypes;
            this._dataType = dataType;
            _value = new ParameterValueFile();
        }

        public void Bind(string componentName, string variableName)
        {
            var b = new ParameterValueBound(componentName, variableName);
            _value = b;
        }

        public void Unbind()
        {
            _value = new ParameterValueFile();
        }

        public void Bind(string componentName)
        {
            Bind(componentName, _name);
        }

        public void SetValue<T>(T value)
        {
            var b = new ParameterValueManualConstant(value);
            _value = b;
        }

        public void SetValue<D1, D2, T>(Func<D1, D2, T> parameter)
        {
            var b = new ParameterValueManuelLambda(parameter);
            _value = b;
        }

        public void SetValue<D1, T>(Func<D1, T> parameter)
        {
            var b = new ParameterValueManuelLambda(parameter);
            _value = b;
        }

        public string Name { get { return _name; } }

        public ParameterValue Binding { get { return _value; } }

        public IEnumerable<Type> DimensionTypes { get { return _dimensionTypes; } }

        public Type DataType { get { return _dataType; } }

        public override string ToString()
        {
            return _name;
        }
    }
}
