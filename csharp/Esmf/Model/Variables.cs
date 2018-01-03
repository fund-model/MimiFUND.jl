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
    public class Variables : IEnumerable<Variable>
    {
        private Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();

        internal void Add(string name, Variable variable)
        {
            _variables.Add(name, variable);
        }

        public Variable this[string name]
        {
            get { return _variables[name]; }
        }

        IEnumerator<Variable> IEnumerable<Variable>.GetEnumerator()
        {
            return _variables.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _variables.Values.GetEnumerator();
        }
    }
}
