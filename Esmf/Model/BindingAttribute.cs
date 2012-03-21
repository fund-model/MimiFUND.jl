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
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class BindingAttribute : Attribute
    {
        readonly string _fieldName;
        readonly string _sourceComponentName;
        readonly string _sourceFieldName;

        // This is a positional argument
        public BindingAttribute(string fieldName, string sourceComponentName, string sourceFieldName)
        {
            _fieldName = fieldName;
            _sourceComponentName = sourceComponentName;
            _sourceFieldName = sourceFieldName ?? fieldName;
        }

        public BindingAttribute(string fieldName, string sourceComponentName)
            : this(fieldName, sourceComponentName, fieldName)
        {
        }

        public string FieldName
        {
            get { return _fieldName; }
        }

        public string SourceComponentName
        {
            get { return _sourceComponentName; }
        }

        public string SourceFieldName
        {
            get { return _sourceFieldName; }
        }

    }
}
