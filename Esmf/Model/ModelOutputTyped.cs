// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmf.Model;

namespace Esmf
{
    public class ModelOutputTyped<T> : ModelOutput
    {
        private T _rootComponent;

        public ModelOutputTyped()
            : base()
        {
            _rootComponent = Activator.CreateInstance<T>();
        }

        public void ReconnectStateVariables(ModelOutput mf)
        {
            var components = (from f in typeof(T).GetFields()
                              let a = (ModelStateAttribute)f.GetCustomAttributes(false).FirstOrDefault(x => x is ModelStateAttribute)
                              where a != null
                              select new
                              {
                                  Name = f.Name.ToLowerInvariant(),
                                  StateInterfaceType = f.FieldType,
                                  Field = f
                              }).ToArray();

            foreach (var c in components)
            {
                object o = _stateinterfaceOjbect[c.Name];
                c.Field.SetValue(_rootComponent, o);
            }

        }

        public T RootComponent { get { return _rootComponent; } }
    }
}
