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
    public class FieldVariable2DimensionalTimeEfficient<D2, T> : IParameter2DimensionalTypeless<T>, IVariable2Dimensional<Timestep, D2, T>, IParameter2Dimensional<Timestep, D2, T>
        where D2 : IDimension
    {
        private bool[,] _valuesHasBeenSet;
        private Timestep _currentYear;

        private bool _switchtedOrder;
        private T[] _rollingValueCurr;
        private T[] _rollingValuePrev;
        private Timestep _rollingCurrentYear;

        private ModelOutput _parent;

        public FieldVariable2DimensionalTimeEfficient(ModelOutput parent)
        {
            _parent = parent;
            _rollingCurrentYear = Timestep.FromSimulationYear(1);
            _currentYear = Timestep.FromSimulationYear(-1);
            _switchtedOrder = false;

            var d2 = _parent.Dimensions.GetDimension<D2>();

            _rollingValueCurr = new T[d2.Count];
            _rollingValuePrev = new T[d2.Count];

            ControlledConstructor(_parent.Clock.Timestepcount, d2.Count, false);

        }

        [Conditional("FUNDCHECKED")]
        private void ControlledConstructor(int count0, int count1, bool setAllToSet)
        {
            _valuesHasBeenSet = new bool[count0, count1];

            if (setAllToSet)
            {
                for (int i = 0; i < count0; i++)
                {
                    for (int l = 0; l < count1; l++)
                    {
                        _valuesHasBeenSet[i, l] = true;
                    }
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
        private void CheckForValidIndex(Timestep index1, D2 index2)
        {
            if (_valuesHasBeenSet[index1.Value, index2.Index] == false)
                throw new ArgumentOutOfRangeException("Value for this index has not been set");

            if (!(index1 == _currentYear || index1 == (_currentYear - 1)))
                throw new ArgumentOutOfRangeException("Can only read values for current or previous year");
        }

        [Conditional("FUNDCHECKED")]
        private void ValueSetForIndex(Timestep index1, D2 index2)
        {
            _valuesHasBeenSet[index1.Value, index2.Index] = true;
            _currentYear = index1;
        }

        public T this[Timestep index1, D2 index2]
        {
            get
            {
                CheckForValidIndex(index1, index2);

                if (!(index1 == _rollingCurrentYear || index1 == _rollingCurrentYear - 1))
                    throw new ArgumentOutOfRangeException();

                if ((_switchtedOrder == false && index1 == _rollingCurrentYear) || (_switchtedOrder == true && index1 == _rollingCurrentYear - 1))
                    return _rollingValueCurr[index2.Index];
                else
                    return _rollingValuePrev[index2.Index];
            }
            set
            {
                CheckForValidValue(value);
                ValueSetForIndex(index1, index2);


                if (index1 == _rollingCurrentYear + 1)
                {
                    _switchtedOrder = !_switchtedOrder;
                    _rollingCurrentYear = index1;
                }


                if ((_switchtedOrder == false && index1 == _rollingCurrentYear) || (_switchtedOrder == true && index1 == _rollingCurrentYear - 1))
                    _rollingValueCurr[index2.Index] = value;
                else
                    _rollingValuePrev[index2.Index] = value;
            }
        }

        T IVariable2Dimensional<Timestep, D2, T>.this[Timestep index1, D2 index2]
        {
            get
            {
                return this[index1, index2];
            }
            set
            {
                this[index1, index2] = value;
            }
        }

        T IParameter2Dimensional<Timestep, D2, T>.this[Timestep index1, D2 index2]
        {
            get
            {
                return this[index1, index2];
            }
        }

        IEnumerable<Parameter2DimensionalMember<T>> IParameter2DimensionalTypeless<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
