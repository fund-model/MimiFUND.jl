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
    [Serializable]
    public abstract class Parameter
    {
        protected Parameter(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; private set; }
        public int Id { get; private set; }

        public abstract IEnumerable<NonTypedParameterElement> GetAllElements();

        public abstract void Save(string filename, string comment=null);
    }
}
