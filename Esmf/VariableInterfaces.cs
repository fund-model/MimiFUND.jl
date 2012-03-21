// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace Esmf
{
    public interface IFieldInternal
    {
        void SwitchOffChecks();
    }

    public struct Parameter1DimensionalMember<T>
    {
        private IEnumerable<string> _attachedDimensions;
        IDimension _dimension1;
        T _value;

        public Parameter1DimensionalMember(IDimension dimension, T value, IEnumerable<string> attachedDimensions)
        {
            _dimension1 = dimension;
            _value = value;
            _attachedDimensions = attachedDimensions;
        }

        public IDimension Dimension1 { get { return _dimension1; } }
        public T Value { get { return _value; } }
        public IEnumerable<string> AttachedDimensions { get { return _attachedDimensions; } }
    }

    public struct Parameter2DimensionalMember<T>
    {
        private IEnumerable<string> _attachedDimensions;
        IDimension _dimension1;
        IDimension _dimension2;
        T _value;

        public Parameter2DimensionalMember(IDimension dimension1, IDimension dimension2, T value, IEnumerable<string> attachedDimensions)
        {
            _dimension1 = dimension1;
            _dimension2 = dimension2;
            _value = value;
            _attachedDimensions = attachedDimensions;
        }

        public IDimension Dimension1 { get { return _dimension1; } }

        public IDimension Dimension2 { get { return _dimension2; } }

        public T Value { get { return _value; } }

        public IEnumerable<string> AttachedDimensions { get { return _attachedDimensions; } }
    }

    public interface IParameter1DimensionalTypeless<T>
    {
        IEnumerable<Parameter1DimensionalMember<T>> GetEnumerator();
    }

    public interface IParameter1Dimensional<D1, T> : IParameter1DimensionalTypeless<T>
    {
        T this[D1 index] { get; }
    }

    public interface IVariable1Dimensional<D1, T>
    {
        T this[D1 index] { get; set; }
    }

    public interface IParameter2DimensionalTypeless<T>
    {
        IEnumerable<Parameter2DimensionalMember<T>> GetEnumerator();
    }

    public interface IParameter2Dimensional<D1, D2, T> : IParameter2DimensionalTypeless<T>
    {
        T this[D1 index1, D2 index2] { get; }
    }

    public interface IVariable2Dimensional<D1, D2, T>
    {
        T this[D1 index1, D2 index2] { get; set; }
    }
}
