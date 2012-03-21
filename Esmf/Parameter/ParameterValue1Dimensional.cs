// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmf
{
    public interface IParameterValue1DimensionalTypeless
    {
        ParameterValueElementNonTyped GetElement(int index);
    }

    public class ParameterValue1Dimensional<T> : ParameterValue, IEnumerable<ParameterValueElement1Dimensional<T>>, IParameterValue1DimensionalTypeless
    {
        private JaggedArrayWrapper<T> _values;

        public ParameterValue1Dimensional(string name, int id, JaggedArrayWrapper<T> values)
            : base(name, id)
        {
            _values = new JaggedArrayWrapper<T>(values);
        }

        public override IEnumerable<ParameterValueElementNonTyped> GetAllElements()
        {
            for (int i = 0; i < _values.Length; i++)
            {
                var key = new ParameterElementKey1Dimensional(Id, this.Name, i);
                yield return new ParameterValueElement1Dimensional<T>(key, Name, Id, i, _values[i]);
            }
        }

        public T this[int index]
        {
            get
            {
                return _values[index];
            }
        }

        public int Length
        {
            get
            {
                return _values.Length;
            }
        }

        public void CopyTo(JaggedArrayWrapper<T> array)
        {
            _values.CopyTo(array);
        }

        public JaggedArrayWrapper<T> GetPointerToData()
        {
            return _values;
        }

        public IEnumerator<ParameterValueElement1Dimensional<T>> GetEnumerator()
        {
            for (int i = 0; i < _values.Length; i++)
            {
                var key = new ParameterElementKey1Dimensional(Id, this.Name, i);
                yield return new ParameterValueElement1Dimensional<T>(key, Name, Id, i, _values[i]);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        ParameterValueElementNonTyped IParameterValue1DimensionalTypeless.GetElement(int index)
        {
            var key = new ParameterElementKey1Dimensional(Id, this.Name, index);
            return new ParameterValueElement1Dimensional<T>(key, Name, Id, index, _values[index]);
        }
    }
}
