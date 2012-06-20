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
    public class Component
    {
        private HashSet<string> _names = new HashSet<string>();

        private Type _componentType;
        private Type _stateinterfaceType;
        private string _name;
        private Variables _variables = new Variables();
        private Parameters _parameters = new Parameters();
        private bool _storeFullVariablesByDefault;

        public Component(string name, Type componentType, Type stateinterfaceType, bool storeFullVariablesByDefault)
        {
            _name = name;
            _componentType = componentType;
            _stateinterfaceType = stateinterfaceType;
            _storeFullVariablesByDefault = storeFullVariablesByDefault;

            var fields = from x in stateinterfaceType.GetProperties()
                         let isMultDim = x.PropertyType.IsGenericType ? (x.PropertyType.GetGenericTypeDefinition() == typeof(IVariable1Dimensional<,>) || x.PropertyType.GetGenericTypeDefinition() == typeof(IVariable2Dimensional<,,>) || x.PropertyType.GetGenericTypeDefinition() == typeof(IParameter1Dimensional<,>) || x.PropertyType.GetGenericTypeDefinition() == typeof(IParameter2Dimensional<,,>)) : false
                         let dimensionTypes = x.PropertyType.IsGenericType ? x.PropertyType.GetGenericArguments() : null
                         let defaultValuesCount = x.GetCustomAttributes(typeof(DefaultParameterValueAttribute), false).Length
                         select new
                         {
                             Name = x.Name,
                             IsVariable = !isMultDim ? x.CanWrite : (x.PropertyType.GetGenericTypeDefinition() == typeof(IVariable1Dimensional<,>) || x.PropertyType.GetGenericTypeDefinition() == typeof(IVariable2Dimensional<,,>)),
                             DefaultValue = defaultValuesCount == 0 ? null : x.GetCustomAttributes(typeof(DefaultParameterValueAttribute), false).Cast<DefaultParameterValueAttribute>().First(),
                             DimensionTypes = dimensionTypes == null ? new Type[] { } : dimensionTypes.Reverse().Skip(1).Reverse().ToArray(),
                             DataType = !x.PropertyType.IsGenericType ? x.PropertyType : x.PropertyType.GetGenericArguments().Reverse().First()
                         };

            foreach (var f in fields)
            {
                if (f.IsVariable)
                {
                    var v = new Variable(f.Name, f.DimensionTypes, f.DataType);
                    v.StoreOutput = _storeFullVariablesByDefault;
                    _variables.Add(f.Name, v);
                }
                else
                {
                    var p = new Parameter(f.Name, f.DimensionTypes, f.DataType);
                    _parameters.Add(f.Name, p);

                    if (f.DefaultValue != null)
                        p.Binding.SetDefaultValue(f.DefaultValue.DefaultValue);
                }
            }
        }

        public string Name { get { return _name; } }

        public Type StateInterfaceType { get { return _stateinterfaceType; } }

        public void RunComponent(Clock clock, object state, ModelOutput mf)
        {
            object c = Activator.CreateInstance(_componentType);

            var method = _componentType.GetMethod("Run");
            method.Invoke(c, new object[] { clock, state, mf.Dimensions });

            //c.Run(clock, state, mf.Dimensions);
        }

        public Parameters Parameters
        {
            get { return _parameters; }
        }

        public Variables Variables
        {
            get { return _variables; }
        }

        public override string ToString()
        {
            return _name;
        }

    }
}
