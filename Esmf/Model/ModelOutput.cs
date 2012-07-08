// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;

namespace Esmf
{
    public class ModelOutput
    {
        private Dimensions _dimensions = new Dimensions();

        public Dimensions Dimensions { get { return _dimensions; } }

        private Clock _clock;

        public Clock Clock
        {
            get
            {
                return _clock;
            }
            set
            {
                _clock = value;
            }
        }

        public class Field
        {
            public string ComponentName { get; set; }
            public string FieldName { get; set; }
            public object Values { get; set; }
        }

        private class DelegateToParameter1Dimensional<D1, T> : IParameter1DimensionalTypeless<T>, IParameter1Dimensional<D1, T>
        {
            Func<D1, T> _delegate;

            public DelegateToParameter1Dimensional(Func<D1, T> function)
            {
                _delegate = function;
            }

            T IParameter1Dimensional<D1, T>.this[D1 index]
            {
                get { return _delegate(index); }
            }

            IEnumerable<Parameter1DimensionalMember<T>> IParameter1DimensionalTypeless<T>.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }


        private class DelegateToParameter2Dimensional<D1, D2, T> : IParameter2DimensionalTypeless<T>, IParameter2Dimensional<D1, D2, T>
        {
            private ModelOutput _parent;
            Func<D1, D2, T> _delegate;

            public DelegateToParameter2Dimensional(Func<D1, D2, T> function, ModelOutput parent)
            {
                _delegate = function;
                _parent = parent;
            }

            T IParameter2Dimensional<D1, D2, T>.this[D1 index1, D2 index2]
            {
                get
                {
                    return _delegate(index1, index2);
                }
            }


            IEnumerable<Parameter2DimensionalMember<T>> IParameter2DimensionalTypeless<T>.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public ModelOutput()
        {
        }

        public bool DoesFieldExist(string componentName, string fieldName)
        {
            var key = new Tuple<string, string>(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant());

            return _variables.ContainsKey(key);
        }

        #region NonDimensionalVariables

        public void Add0DimensionalVariable(string componentName, string fieldName, Type type, object initialValue)
        {
            var key = new Tuple<string, string>(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant());

            if (_variables.ContainsKey(key))
            {
                throw new ArgumentException();
            }

            Type genericType = typeof(FieldVariable0Dimensional<>).MakeGenericType(type);
            object v = Activator.CreateInstance(genericType, this);

            if (initialValue != null)
            {
                var property = genericType.GetProperty("Value");
                property.SetValue(v, initialValue, null);
            }

            _variables.Add(key, v);
            _variables.ContainsKey(key);
        }

        public object GetNonDimensionalVariableGetter(string componentName, string fieldName)
        {
            var key = Tuple.Create(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant());
            var f = (FieldVariable0DimensionalTypeless)_variables[key];
            return f.GetFieldGetter();
        }

        public object GetNonDimensionalVariableSetter(string componentName, string fieldName)
        {
            var key = Tuple.Create(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant());
            var f = (FieldVariable0DimensionalTypeless)_variables[key];
            return f.GetFieldSetter();
        }

        #endregion

        #region DimensionalVariables

        Dictionary<Tuple<string, string>, object> _variables = new Dictionary<Tuple<string, string>, object>();

        public void Add1DimensionalVariable<D1, T>(string componentName, string fieldName, bool useEfficientField)
            where D1 : IDimension
        {
            var key = new Tuple<string, string>(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant());

            if (_variables.ContainsKey(key))
            {
                throw new ArgumentException();
            }

            object v;
            if (typeof(D1) == typeof(Timestep))
            {
                if (useEfficientField)
                    v = new FieldVariable1DimensionalTimeEfficient<T>(this);
                else
                    v = new FieldVariable1DimensionalTime<T>(this);
            }
            else
            {
                v = new FieldVariable1Dimensional<D1, T>(this);
            }

            _variables.Add(key, v);
        }


        public void Add2DimensionalVariable<D1, D2, T>(string componentName, string fieldName, bool useEfficientField)
            where D1 : IDimension
            where D2 : IDimension
        {
            var key = new Tuple<string, string>(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant());

            if (_variables.ContainsKey(key))
            {
                throw new ArgumentException();
            }

            object v;
            if (typeof(D1) == typeof(Timestep))
            {
                if (useEfficientField)
                    v = new FieldVariable2DimensionalTimeEfficient<D2, T>(this);
                else
                    v = new FieldVariable2DimensionalTime<D2, T>(this);
            }
            else
            {
                v = new FieldVariable2Dimensional<D1, D2, T>(this);
            }

            _variables.Add(key, v);
        }

        public void Add1DimensionalParameter<D1, T>(string componentName, string fieldName, IParameter1Dimensional<D1, T> parameter)
        {
            var key = new Tuple<string, string>(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant());

            if (_variables.ContainsKey(key))
            {
                throw new ArgumentException();
            }

            _variables.Add(key, parameter);
        }

        public void Add1DimensionalParameter<D1, T>(string componentName, string fieldName, Func<D1, T> parameter)
        {
            Add1DimensionalParameter(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant(), new DelegateToParameter1Dimensional<D1, T>(parameter));
        }

        public void Add1DimensionalParameterLambda<D1, T>(string componentName, string fieldName, Func<D1, T> parameter)
        {
            Add1DimensionalParameter(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant(), new DelegateToParameter1Dimensional<D1, T>(parameter));
        }


        public void Add2DimensionalParameter<D1, D2, T>(string componentName, string fieldName, IParameter2Dimensional<D1, D2, T> parameter)
        {
            var key = new Tuple<string, string>(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant());

            if (_variables.ContainsKey(key))
            {
                throw new ArgumentException();
            }

            _variables.Add(key, parameter);
        }

        public void Add2DimensionalParameter<D1, D2, T>(string componentName, string fieldName, Func<D1, D2, T> parameter)
        {
            Add2DimensionalParameter(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant(), new DelegateToParameter2Dimensional<D1, D2, T>(parameter, this));
        }

        public void Add2DimensionalParameterLambda<D1, D2, T>(string componentName, string fieldName, Func<D1, D2, T> parameter)
        {
            Add2DimensionalParameter(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant(), new DelegateToParameter2Dimensional<D1, D2, T>(parameter, this));
        }

        public void ConnectParameterToVariable(string parameterComponentName, string parameterFieldName, string variableComponentName, string variableFieldName)
        {
            var key = new Tuple<string, string>(parameterComponentName.ToLowerInvariant(), parameterFieldName.ToLowerInvariant());
            var sourceKey = new Tuple<string, string>(variableComponentName.ToLowerInvariant(), variableFieldName.ToLowerInvariant());

            if (_variables.ContainsKey(key))
            {
                throw new ArgumentException();
            }

            _variables.Add(key, _variables[sourceKey]);
        }

        public object GetDimensionalField(string componentName, string fieldName)
        {
            var key = new Tuple<string, string>(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant());
            if (!_variables.ContainsKey(key))
            {
                throw new ArgumentOutOfRangeException(String.Format("{0}.{1} is not in the list of dimensional fields", componentName.ToLowerInvariant(), fieldName.ToLowerInvariant()));
            }

            return _variables[key];
        }

        #endregion

        #region Attached dimensions

        private List<string> _attachedDimensions = new List<string>();

        public IList<string> AttachedDimensions
        {
            get
            {
                return _attachedDimensions;
            }
        }

        #endregion

        public IEnumerable<Field> GetDimensionalFieldsOperator()
        {
            foreach (var f in _variables.Keys)
            {
                yield return new Field() { ComponentName = f.Item1, FieldName = f.Item2, Values = _variables[f] };
            }
        }

        internal void LoadVariableFromParameter(string componentName, string fieldName, ParameterValues parameters, Type dataType, Type[] dimensionTypes)
        {
            if (dimensionTypes.Length == 0)
            {
                object value = ((IParameterValueNonDimensionalTypeless)parameters[fieldName.ToLowerInvariant()]).GetElement().GetUntypedValue();

                Add0DimensionalVariable(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant(), dataType, value);
            }
            else if (dimensionTypes.Length == 1)
            {
                //var p = (ParameterValue1Dimensional<T>)parameters[fieldName];
                var p = parameters[fieldName.ToLowerInvariant()];

                if (dimensionTypes[0] == typeof(Timestep))
                {
                    Type d1 = typeof(FieldParameter1DimensionalTime<>);
                    Type[] typeArgs = { dataType };
                    Type d1Typed = d1.MakeGenericType(typeArgs);
                    object[] args = { this, p };


                    object value = Activator.CreateInstance(d1Typed, args);

                    //var value = new FieldParameter1DimensionalTime<T>(this, p);

                    var key = new Tuple<string, string>(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant());

                    if (_variables.ContainsKey(key))
                    {
                        throw new ArgumentException();
                    }

                    _variables.Add(key, value);
                }
                else if (typeof(IDimension).IsAssignableFrom(dimensionTypes[0]))
                {
                    Type d1 = typeof(FieldParameter1Dimensional<,>);
                    Type[] typeArgs = { dimensionTypes[0], dataType };
                    Type d1Typed = d1.MakeGenericType(typeArgs);
                    object[] args = { this, p };


                    object value = Activator.CreateInstance(d1Typed, args);

                    //var value = new FieldParameter1DimensionalTime<T>(this, p);

                    var key = new Tuple<string, string>(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant());

                    if (_variables.ContainsKey(key))
                    {
                        throw new ArgumentException();
                    }

                    _variables.Add(key, value);
                }
                else
                    throw new ArgumentException("Unknown dimension type");

            }
            else if (dimensionTypes.Length == 2)
            {
                var p = parameters[fieldName.ToLowerInvariant()];

                if (dimensionTypes[0] == typeof(Timestep) && typeof(IDimension).IsAssignableFrom(dimensionTypes[1]))
                {
                    Type d1 = typeof(FieldParameter2DimensionalTime<,>);
                    Type[] typeArgs = { dimensionTypes[1], dataType };
                    Type d1Typed = d1.MakeGenericType(typeArgs);
                    object[] args = { this, p };


                    object value = Activator.CreateInstance(d1Typed, args);

                    var key = new Tuple<string, string>(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant());

                    if (_variables.ContainsKey(key))
                    {
                        throw new ArgumentException();
                    }

                    _variables.Add(key, value);
                }
                else if (typeof(IDimension).IsAssignableFrom(dimensionTypes[0]) && typeof(IDimension).IsAssignableFrom(dimensionTypes[1]))
                {
                    Type d1 = typeof(FieldParameter2Dimensional<,,>);
                    Type[] typeArgs = { dimensionTypes[0], dimensionTypes[1], dataType };
                    Type d1Typed = d1.MakeGenericType(typeArgs);
                    object[] args = { this, p };


                    object value = Activator.CreateInstance(d1Typed, args);

                    var key = new Tuple<string, string>(componentName.ToLowerInvariant(), fieldName.ToLowerInvariant());

                    if (_variables.ContainsKey(key))
                    {
                        throw new ArgumentException();
                    }

                    _variables.Add(key, value);
                }
                else
                    throw new ArgumentException("Unknown dimension type");
            }
            else
                throw new ArgumentOutOfRangeException();
        }

        public dynamic this[string componentName, string fieldname]
        {
            get
            {
                if (!DoesFieldExist(componentName.ToLowerInvariant(), fieldname.ToLowerInvariant()))
                    throw new ArgumentOutOfRangeException();

                if (_variables.ContainsKey(Tuple.Create(componentName.ToLowerInvariant(), fieldname.ToLowerInvariant())))
                {
                    return _variables[Tuple.Create(componentName.ToLowerInvariant(), fieldname.ToLowerInvariant())];
                }
                else
                {
                    dynamic getter = GetNonDimensionalVariableGetter(componentName.ToLowerInvariant(), fieldname.ToLowerInvariant());
                    return getter();
                }
            }
        }

        public void SwitchOffChecks()
        {
            foreach (var f in _variables.Values)
            {
                if (f is IFieldInternal)
                {
                    var typedF = (IFieldInternal)f;
                    typedF.SwitchOffChecks();
                }
            }
        }
    }
}
