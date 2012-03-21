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
    public class FieldVariable1DimensionalTime<T> : IParameter1DimensionalTypeless<T>, IVariable1Dimensional<Timestep, T>, IParameter1Dimensional<Timestep, T>
    {
        private JaggedArrayWrapper<T> _values;
        private bool[] _valuesHasBeenSet;

        private Timestep _currentYear;
        private ModelOutput _parent;

        public FieldVariable1DimensionalTime(ModelOutput parent)
        {
            _parent = parent;
            _currentYear = Timestep.FromSimulationYear(-1);

            // TODO Change this to properly find out how many years the model run will have
            _values = new JaggedArrayWrapper<T>(_parent.Clock.Timestepcount);

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

                return _values[index.Value];
            }
            set
            {
                CheckForValidValue(value);
                ValueSetForIndex(index);

                _values[index.Value] = value;
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
            for (int i = 0; i < _values.Length; i++)
            {
                yield return new Parameter1DimensionalMember<T>(Timestep.FromSimulationYear(i), _values[i], _parent.AttachedDimensions);
            }
        }
    }
}
