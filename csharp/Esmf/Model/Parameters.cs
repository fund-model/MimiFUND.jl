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
    public class Parameters : IEnumerable<Parameter>
    {
        private Dictionary<string, Parameter> _parameters = new Dictionary<string, Parameter>();

        internal void Add(string name, Parameter parameter)
        {
            _parameters.Add(name, parameter);
        }

        public Parameter this[string name]
        {
            get { return _parameters[name]; }
        }


        IEnumerator<Parameter> IEnumerable<Parameter>.GetEnumerator()
        {
            return _parameters.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _parameters.Values.GetEnumerator();
        }
    }

}
