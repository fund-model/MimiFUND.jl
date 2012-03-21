// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Esmf
{
    public class FieldVariable1DimensionalTimeEfficient<T> : IParameter1DimensionalTypeless<T>, IVariable1Dimensional<Timestep, T>, IParameter1Dimensional<Timestep, T>
    {
        private bool[] _valuesHasBeenSet;

        private bool _switchtedOrder;
        private T _rollingValueCurr;
        private T _rollingValuePrev;
        private Timestep _rollingCurrentYear;

        private Timestep _currentYear;
        private ModelOutput _parent;

        public FieldVariable1DimensionalTimeEfficient(ModelOutput parent)
        {
            _parent = parent;
            _rollingCurrentYear = Timestep.FromSimulationYear(1);
            _currentYear = Timestep.FromSimulationYear(-1);
            _switchtedOrder = false;

            ControlledConstructor(_parent.Clock.Timestepcount, false);

        }

        [Conditional("FUNDCHECKED")]
        private void ControlledConstructor(int count, bool setAllToSet)
        {
            _valuesHasBeenSet = new bool[count];

            if (setAllToSet)
            {
                for (int i = 0; i < _valuesHasBeenSet.Length; i++)
                {
                    _valuesHasBeenSet[i] = true;
                }
            }
        }

        [Conditional("FUNDCHECKED")]
        private void CheckForValidValue(T v)
        {
            if (v.GetType() == typeof(double))
            {
                double d = ((double)((object)v));

                if (double.IsNaN(d))
                    throw new ArgumentOutOfRangeException("NaN is not allowed as a value");
                else if (double.IsInfinity(d))
                    throw new ArgumentOutOfRangeException("Infinity is not allowed as a value");
            }
        }

        [Conditional("FUNDCHECKED")]
        private void CheckForValidIndex(Timestep index)
        {
            if (_valuesHasBeenSet[index.Value] == false)
                throw new ArgumentOutOfRangeException("Value for this index has not been set");

            if (!(index == _currentYear || index == (_currentYear - 1)))
                throw new ArgumentOutOfRangeException("Can only read values for current or previous year");
        }

        [Conditional("FUNDCHECKED")]
        private void ValueSetForIndex(Timestep index)
        {
            _valuesHasBeenSet[index.Value] = true;
            _currentYear = index;
        }


        public T this[Timestep index]
        {
            get
            {
                CheckForValidIndex(index);

                if (!(index == _rollingCurrentYear || index == _rollingCurrentYear - 1))
                    throw new ArgumentOutOfRangeException();

                if ((_switchtedOrder == false && index == _rollingCurrentYear) || (_switchtedOrder == true && index == _rollingCurrentYear - 1))
                    return _rollingValueCurr;
                else
                    return _rollingValuePrev;
            }
            set
            {
                CheckForValidValue(value);
                ValueSetForIndex(index);

                if (index == _rollingCurrentYear + 1)
                {
                    _switchtedOrder = !_switchtedOrder;
                    _rollingCurrentYear = index;
                }


                if ((_switchtedOrder == false && index == _rollingCurrentYear) || (_switchtedOrder == true && index == _rollingCurrentYear - 1))
                    _rollingValueCurr = value;
                else
                    _rollingValuePrev = value;
            }
        }

        T IVariable1Dimensional<Timestep, T>.this[Timestep index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = value;
            }
        }

        T IParameter1Dimensional<Timestep, T>.this[Timestep index]
        {
            get
            {
                return this[index];
            }
        }

        IEnumerable<Parameter1DimensionalMember<T>> IParameter1DimensionalTypeless<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
