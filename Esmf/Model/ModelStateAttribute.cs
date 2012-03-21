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
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class ModelStateAttribute : Attribute
    {
        readonly Type _componentClass;


        // This is a positional argument
        public ModelStateAttribute(Type componentClass)
        {
            _componentClass = componentClass;
        }

        public Type ComponentClass
        {
            get { return _componentClass; }
        }
    }
}
