// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace Esmf.ComponentStructure
{
    public class StateFieldDimensionStructure
    {
        public string Name;
        public Type Type;
    }

    public class StateFieldStructure
    {
        protected Type _Type;
        public Type Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        protected string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        protected bool _canWrite;
        public bool CanWrite
        {
            get { return _canWrite; }
            set { _canWrite = value; }
        }

        protected List<StateFieldDimensionStructure> _dimensions = new List<StateFieldDimensionStructure>();
        public List<StateFieldDimensionStructure> Dimensions
        {
            get { return _dimensions; }
        }
    }
}
