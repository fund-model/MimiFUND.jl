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
    public abstract class ParameterValue
    {
        public object DefaultValue { get; private set; }

        public void SetDefaultValue(object dv)
        {
            DefaultValue = dv;
        }
    }

    public class ParameterValueBound : ParameterValue
    {
        public ParameterValueBound(string componentName, string variableName)
        {
            ComponentName = componentName;
            VariableName = variableName;
        }

        public string ComponentName { get; private set; }
        public string VariableName { get; private set; }
    }

    public class ParameterValueFile : ParameterValue
    {
        public ParameterValueFile(string parametername = null)
        {
            Parametername = parametername;
        }

        public string Parametername { get; private set; }
    }

    public abstract class ParameterValueManual : ParameterValue
    {
    }

    public class ParameterValueManualConstant : ParameterValueManual
    {
        public ParameterValueManualConstant(object value)
        {
            Value = value;
        }

        public object Value { get; set; }
    }

    public class ParameterValueManuelLambda : ParameterValueManual
    {
        public ParameterValueManuelLambda(object lambda)
        {
            Lambda = lambda;
        }

        public object Lambda { get; set; }
    }
}
