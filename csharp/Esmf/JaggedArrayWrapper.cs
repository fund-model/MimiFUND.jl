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
    public class JaggedArrayWrapper3<T>
    {
        private T[][,] _values;

        public JaggedArrayWrapper3(int count0, int count1, int count2)
        {
            _values = new T[count0][,];

            for (int i = 0; i < count0; i++)
            {
                _values[i] = new T[count1, count2];
            }
        }

        public T this[int index0, int index1, int index2]
        {
            get { return _values[index0][index1, index2]; }
            set { _values[index0][index1, index2] = value; }
        }
    }

    public class JaggedArrayWrapper2<T>
    {
        private T[][] _values;

        public JaggedArrayWrapper2(int count0, int count1)
        {
            _values = new T[count0][];

            for (int i = 0; i < count0; i++)
            {
                _values[i] = new T[count1];
            }
        }

        public int Length0 { get { return _values.Length; } }
        public int Length1 { get { return _values[0].Length; } }

        public JaggedArrayWrapper2(JaggedArrayWrapper2<T> values)
        {
            int count0 = values.Length0;
            int count1 = values.Length1;

            _values = new T[count0][];

            for (int i = 0; i < count0; i++)
            {
                _values[i] = new T[count1];
                Array.Copy(values._values[i], _values[i], count1);
            }
        }

        public void CopyTo(JaggedArrayWrapper2<T> array)
        {
            if (Length0 != array.Length0 || Length1 != array.Length1)
                throw new ArgumentOutOfRangeException("Arrays need to have same length");

            int count0 = Length0;
            int count1 = Length1;


            for (int i = 0; i < count0; i++)
            {
                Array.Copy(_values[i], array._values[i], count1);
            }

        }

        public T this[int index1, int index2]
        {
            get
            {
                return _values[index1][index2];
            }
            set
            {
                _values[index1][index2] = value;
            }
        }

    }

    public class JaggedArrayWrapper<T>
    {
        private T[][] _values;
        private int _count;
        private const int _limitValue = 999;

        public JaggedArrayWrapper(int count)
        {
            int requiredBuckets = (count / _limitValue) + 1;

            _values = new T[requiredBuckets][];

            for (int i = 0; i < requiredBuckets - 1; i++)
            {
                _values[i] = new T[_limitValue];
            }

            int countOfFinalBucket = count % _limitValue;
            _values[requiredBuckets - 1] = new T[countOfFinalBucket];
            _count = count;
        }

        public int Length { get { return _count; } }

        public JaggedArrayWrapper(JaggedArrayWrapper<T> values)
        {
            int requiredBuckets = (values._count / _limitValue) + 1;

            _values = new T[requiredBuckets][];

            for (int i = 0; i < requiredBuckets - 1; i++)
            {
                _values[i] = new T[_limitValue];
                Array.Copy(values._values[i], _values[i], _limitValue);
            }

            int countOfFinalBucket = values._count % _limitValue;
            _values[requiredBuckets - 1] = new T[countOfFinalBucket];
            Array.Copy(values._values[requiredBuckets - 1], _values[requiredBuckets - 1], countOfFinalBucket);
            _count = values._count;
        }

        public void CopyTo(JaggedArrayWrapper<T> array)
        {
            if (_count != array._count)
                throw new ArgumentOutOfRangeException("Arrays need to have same length");

            int requiredBuckets = (array._count / _limitValue) + 1;

            for (int i = 0; i < requiredBuckets - 1; i++)
            {
                Array.Copy(_values[i], array._values[i], _limitValue);
            }

            int countOfFinalBucket = _count % _limitValue;
            Array.Copy(_values[requiredBuckets - 1], array._values[requiredBuckets - 1], countOfFinalBucket);

        }

        public T this[int index]
        {
            get
            {
                int bucketIndex = index / _limitValue;
                int index2 = index % _limitValue;
                return _values[bucketIndex][index2];
            }
            set
            {
                int bucketIndex = index / _limitValue;
                int index2 = index % _limitValue;
                _values[bucketIndex][index2] = value;
            }
        }
    }
}
