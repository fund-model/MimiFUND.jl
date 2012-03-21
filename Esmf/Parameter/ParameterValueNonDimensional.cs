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
    public interface IParameterValueNonDimensionalTypeless
    {
        ParameterValueElementNonTyped GetElement();
    }

    public class ParameterValueNonDimensional<T> : ParameterValue, IParameterValueNonDimensionalTypeless
    {
        private T _value;

        public ParameterValueNonDimensional(string name, int id, T value)
            : base(name, id)
        {
            _value = value;
        }

        public override IEnumerable<ParameterValueElementNonTyped> GetAllElements()
        {
            var key = new ParameterElementKey(Id, this.Name);
            yield return new ParameterValueElementNonDimensional<T>(key, Name, Id, _value);
        }

        public T Value { get { return _value; } }

        public ParameterValueElementNonDimensional<T> Element
        {
            get
            {
                var key = new ParameterElementKey(Id, this.Name);
                return new ParameterValueElementNonDimensional<T>(key, Name, Id, _value);
            }
        }

        ParameterValueElementNonTyped IParameterValueNonDimensionalTypeless.GetElement()
        {
            return Element;
        }
    }
}
