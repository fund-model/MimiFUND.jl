// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace Esmf
{
    public interface IStateObjectConnections
    {
        void AddNonDimensionalField(string name, object getterMethod);
        void AddNonDimensionalField(string name, object getterMethod, object setterMethod);
        // void AddNonDimensionalField<T>(string name, NonDimensionalFieldGetter<T> getterMethod, NonDimensionalFieldSetter<T> setterMethod);
        void AddDimensionalField(string name, object field);
        //void AddReadWriteTimeDimensionalField(string name, ITimeDimensionalReadWriteVariable<double> field);

    }
}
