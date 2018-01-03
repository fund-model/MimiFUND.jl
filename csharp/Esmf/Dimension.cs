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
    public interface IDimensions
    {
        T[] GetValues<T>() where T : IDimension;
    }

    public class Dimensions : IDimensions
    {
        private Dictionary<Type, object> _dim = new Dictionary<Type, object>();

        public Dimension<T> Add<T>(int size)
            where T : IDimension
        {
            var d = new Dimension<T>(size);
            _dim.Add(typeof(T), d);
            return d;
        }

        public Dimension<T> GetDimension<T>()
            where T : IDimension
        {
            return (Dimension<T>)_dim[typeof(T)];
        }

        public T[] GetValues<T>()
            where T : IDimension
        {
            var d = (Dimension<T>)_dim[typeof(T)];
            return d.Values;
        }
    }

    public class Dimension<T>
        where T : IDimension
    {
        private T[] _values;
        private string[] _names;

        public Dimension(int size)
        {
            _values = new T[size];
            _names = new string[size];
        }

        public void Set(int index, T value, string name)
        {
            _values[index] = value;
            _names[index] = name;
        }

        public int Count
        {
            get
            {
                return _values.Length;
            }
        }

        public T[] Values
        {
            get
            {
                return _values;
            }
        }

        public string[] Names
        {
            get
            {
                return _names;
            }
        }

        //public static string GetName(int i)
        //{
        //    return _names[i - 1];
        //}

        public T FromName(string name)
        {
            int id = -1;

            for (int i = 0; i < _names.Length; i++)
                if (_names[i] == name)
                {
                    id = i + 1;
                    break;
                }

            if (id == -1)
                throw new ArgumentException("There is no region with the name '{0}'.", name);

            return _values[id];
        }

        //public static int GetIdFromName(string name)
        //{
        //    int id = -1;

        //    for (int i = 0; i < _names.Value.Length; i++)
        //        if (_names.Value[i] == name)
        //        {
        //            id = i + 1;
        //            break;
        //        }

        //    if (id == -1)
        //        throw new ArgumentException("There is no region with the name '{0}'.", name);

        //    return id;
        //}

    }
}
