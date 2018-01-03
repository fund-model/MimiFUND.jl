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
    public interface IParameterValue2DimensionalTypeless
    {
        ParameterValueElementNonTyped GetElement(int index1, int index2);
    }

    public class ParameterValue2Dimensional<T> : ParameterValue, IEnumerable<ParameterValueElement2Dimensional<T>>, IParameterValue2DimensionalTypeless
    {
        private JaggedArrayWrapper2<T> _values;

        public ParameterValue2Dimensional(string name, int id, JaggedArrayWrapper2<T> values)
            : base(name, id)
        {
            _values = new JaggedArrayWrapper2<T>(values);
        }

        public override IEnumerable<ParameterValueElementNonTyped> GetAllElements()
        {
            for (int i = 0; i < Length0; i++)
            {
                for (int l = 0; l < Length1; l++)
                {
                    var key = new ParameterElementKey2Dimensional(Id, this.Name, i, l);
                    yield return new ParameterValueElement2Dimensional<T>(key, Name, Id, i, l, _values[i, l]);
                }
            }
        }

        public T this[int index1, int index2]
        {
            get
            {
                return _values[index1, index2];
            }
        }

        public int Length0
        {
            get
            {
                return _values.Length0;
            }
        }

        public int Length1
        {
            get
            {
                return _values.Length1;
            }
        }

        public void CopyTo(JaggedArrayWrapper2<T> array)
        {
            _values.CopyTo(array);
        }

        public JaggedArrayWrapper2<T> GetPointerToData()
        {
            return _values;
        }

        public IEnumerator<ParameterValueElement2Dimensional<T>> GetEnumerator()
        {
            for (int i = 0; i < Length0; i++)
            {
                for (int l = 0; l < Length1; l++)
                {
                    var key = new ParameterElementKey2Dimensional(Id, this.Name, i, l);
                    yield return new ParameterValueElement2Dimensional<T>(key, Name, Id, i, l, _values[i, l]);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        ParameterValueElementNonTyped IParameterValue2DimensionalTypeless.GetElement(int index1, int index2)
        {
            var key = new ParameterElementKey2Dimensional(Id, this.Name, index1, index2);
            return new ParameterValueElement2Dimensional<T>(key, Name, Id, index1, index2, _values[index1, index2]);
        }
    }
}
