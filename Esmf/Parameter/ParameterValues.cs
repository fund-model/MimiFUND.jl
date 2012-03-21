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
    public class ParameterValues : IEnumerable<ParameterValue>
    {

        private List<ParameterValue> _parameters = new List<ParameterValue>();
        private Dictionary<int, ParameterValue> _parametersById = new Dictionary<int, ParameterValue>();
        private Dictionary<string, ParameterValue> _parametersByName = new Dictionary<string, ParameterValue>();

        internal ParameterValues(IEnumerable<Esmf.Parameter> parameters)
        {
            RunId = null;

            foreach (var p in parameters)
            {
                if (p is Esmf.ParameterNonDimensional<double>)
                {
                    var typedP = (Esmf.ParameterNonDimensional<double>)p;

                    var parameterValue = new ParameterValueNonDimensional<double>(p.Name, p.Id, typedP.GetBestGuessValue());

                    Add(parameterValue);
                }
                else if (p is Esmf.ParameterOneDimensional<double>)
                {
                    var typedP = (Esmf.ParameterOneDimensional<double>)p;

                    var parameterValue = new ParameterValue1Dimensional<double>(p.Name, p.Id, typedP.GetBestGuessValues());

                    Add(parameterValue);
                }
                else if (p is Esmf.ParameterOneDimensional<string>)
                {
                    var typedP = (Esmf.ParameterOneDimensional<string>)p;

                    var parameterValue = new ParameterValue1Dimensional<string>(p.Name, p.Id, typedP.GetBestGuessValues());

                    Add(parameterValue);
                }
                else if (p is Esmf.Parameter2Dimensional<double>)
                {
                    var typedP = (Esmf.Parameter2Dimensional<double>)p;

                    var parameterValue = new ParameterValue2Dimensional<double>(p.Name, p.Id, typedP.GetBestGuessValues());

                    Add(parameterValue);
                }
                else if (p is Esmf.ParameterNonDimensional<Timestep>)
                {
                    var typedP = (Esmf.ParameterNonDimensional<Timestep>)p;

                    var parameterValue = new ParameterValueNonDimensional<Timestep>(p.Name, p.Id, typedP.GetBestGuessValue());

                    Add(parameterValue);
                }
                else
                    throw new InvalidOperationException();
            }
        }

        internal ParameterValues(IEnumerable<Esmf.Parameter> parameters, Random rand, int? runId = null)
        {
            if (runId.HasValue && runId < 1)
                throw new ArgumentException("runId cannot be <=0");

            RunId = runId;

            foreach (var p in parameters)
            {
                if (p is Esmf.ParameterNonDimensional<double>)
                {
                    var typedP = (Esmf.ParameterNonDimensional<double>)p;

                    var parameterValue = new ParameterValueNonDimensional<double>(p.Name, p.Id, typedP.GetRandomValue(rand));

                    Add(parameterValue);
                }
                else if (p is Esmf.ParameterOneDimensional<double>)
                {
                    var typedP = (Esmf.ParameterOneDimensional<double>)p;

                    var parameterValue = new ParameterValue1Dimensional<double>(p.Name, p.Id, typedP.GetRandomValues(rand));

                    Add(parameterValue);
                }
                else if (p is Esmf.ParameterOneDimensional<string>)
                {
                    var typedP = (Esmf.ParameterOneDimensional<string>)p;

                    var parameterValue = new ParameterValue1Dimensional<string>(p.Name, p.Id, typedP.GetRandomValues(rand));

                    Add(parameterValue);

                }
                else if (p is Esmf.Parameter2Dimensional<double>)
                {
                    var typedP = (Esmf.Parameter2Dimensional<double>)p;

                    var parameterValue = new ParameterValue2Dimensional<double>(p.Name, p.Id, typedP.GetRandomValues(rand));

                    Add(parameterValue);
                }
                else if (p is Esmf.ParameterNonDimensional<Timestep>)
                {
                    var typedP = (Esmf.ParameterNonDimensional<Timestep>)p;

                    var parameterValue = new ParameterValueNonDimensional<Timestep>(p.Name, p.Id, typedP.GetRandomValue(rand));

                    Add(parameterValue);
                }
                else
                    throw new InvalidOperationException();
            }

        }

        private void Add(ParameterValue parameter)
        {
            if (_parametersById.ContainsKey(parameter.Id))
            {
                _parameters.Remove(_parametersById[parameter.Id]);
                _parameters.Add(parameter);

                _parametersById[parameter.Id] = parameter;
                _parametersByName[parameter.Name] = parameter;
            }
            else
            {
                _parameters.Add(parameter);
                _parametersById.Add(parameter.Id, parameter);
                _parametersByName.Add(parameter.Name, parameter);
            }
        }


        public IEnumerator<ParameterValue> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        public bool Contains(string name)
        {
            return _parametersByName.ContainsKey(name.ToLowerInvariant());
        }

        public ParameterValue this[string name]
        {
            get
            {
                return _parametersByName[name.ToLower()];
            }
        }

        public IEnumerable<ParameterValueElement<T>> GetElements<T>()
        {
            foreach (var p in _parameters)
            {
                if (p is ParameterValueNonDimensional<T>)
                {
                    yield return ((ParameterValueNonDimensional<T>)p).Element;
                }
                else if (p is ParameterValue1Dimensional<T>)
                {
                    var typedP = (ParameterValue1Dimensional<T>)p;

                    foreach (var e in typedP)
                    {
                        yield return e;
                    }
                }
                else if (p is ParameterValue2Dimensional<T>)
                {
                    var typedP = (ParameterValue2Dimensional<T>)p;

                    foreach (var e in typedP)
                    {
                        yield return e;
                    }
                }
                else
                    throw new InvalidOperationException();
            }
        }

        public ParameterValueElementNonTyped GetElementByKey(ParameterElementKey key)
        {
            var p = _parametersById[key.Id];

            if (p is IParameterValue1DimensionalTypeless)
            {
                var typedKey = (ParameterElementKey1Dimensional)key;

                var typedParameterValue = (IParameterValue1DimensionalTypeless)p;
                return typedParameterValue.GetElement(typedKey.D1);
            }
            else if (p is IParameterValue2DimensionalTypeless)
            {
                var typedKey = (ParameterElementKey2Dimensional)key;

                var typedParameterValue = (IParameterValue2DimensionalTypeless)p;
                return typedParameterValue.GetElement(typedKey.D1, typedKey.D2);
            }
            else if (p is IParameterValueNonDimensionalTypeless)
            {
                var typedParameterValue = (IParameterValueNonDimensionalTypeless)p;
                return typedParameterValue.GetElement();
            }
            else
                throw new InvalidOperationException();
        }

        public int? RunId { get; private set; }

    }
}
