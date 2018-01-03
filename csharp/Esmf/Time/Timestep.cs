// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace Esmf
{
    [Serializable]
    public struct Timestep : IConvertible, IEquatable<Timestep>, IDimension
    {
        private const int _baseYear = 1950;
        private int _year;

        private Timestep(int year)
        {
            _year = year;
        }

        public override bool Equals(object obj)
        {
            return (obj is Timestep) && (this._year == ((Timestep)obj)._year);
        }

        public bool Equals(Timestep other)
        {
            return this._year == other._year;
        }

        public override int GetHashCode()
        {
            return _year.GetHashCode();
        }

        private static int Comparison(Timestep t1, Timestep t2)
        {
            if (t1._year < t2._year)
                return -1;
            else if (t1._year == t2._year)
                return 0;
            else if (t1._year > t2._year)
                return 1;

            throw new Exception();
        }

        public static Timestep operator +(Timestep a, int b)
        {
            Timestep t = new Timestep();
            t._year = a._year + b;
            return t;
        }

        public static Timestep operator -(Timestep a, int b)
        {
            Timestep t = new Timestep();
            t._year = a._year - b;
            return t;
        }

        public static bool operator <(Timestep t1, Timestep t2)
        {
            return Comparison(t1, t2) < 0;
        }

        public static bool operator >(Timestep t1, Timestep t2)
        {
            return Comparison(t1, t2) > 0;
        }

        public static bool operator <=(Timestep t1, Timestep t2)
        {
            return Comparison(t1, t2) <= 0;
        }

        public static bool operator >=(Timestep t1, Timestep t2)
        {
            return Comparison(t1, t2) >= 0;
        }

        public static bool operator ==(Timestep t1, Timestep t2)
        {
            return t1._year == t2._year;
        }

        public static bool operator !=(Timestep t1, Timestep t2)
        {
            return t1._year != t2._year;
        }

        public static Timestep FromSimulationYear(int year)
        {
            return new Timestep(year);
        }

        public static Timestep FromYear(int year)
        {
            return new Timestep(year - _baseYear);
        }

        public int Value
        {
            get { return _year; }
        }

        public override string ToString()
        {
            return (_year + _baseYear).ToString();
        }

        #region IConvertible Members

        TypeCode IConvertible.GetTypeCode()
        {
            throw new NotImplementedException();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return _year + _baseYear;
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        #endregion


        int IDimension.Index
        {
            get { return _year; }
        }
    }
}
