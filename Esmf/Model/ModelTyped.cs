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

    public class ModelTyped<T> : Model
    where T : ComposedComponent
    {
        public ModelTyped(int years = 1049, bool storeFullVariablesByDefault = true)
            : base(years, storeFullVariablesByDefault)
        {
            AddLocalComponentsToModel(typeof(T));
        }

        public ModelOutputTyped<T> Run(ParameterValues parameters)
        {
            var mf = new ModelOutputTyped<T>();

            LoadDimensions(parameters, mf);

            InitVariables(mf);

            ConnectBindings(mf);

            ConnectLeftoversToParameters(mf, parameters);

            ReCreateStateVariables(mf);

            mf.ReconnectStateVariables(mf);

            RunComponents(mf);

            mf.SwitchOffChecks();

            return mf;
        }
    }

}
