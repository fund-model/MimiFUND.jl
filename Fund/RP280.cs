// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
namespace Fund
{

    public static class LegacyConstants
    {
        public const int NYear = 1049;      // number of years
        public const int NoReg = 16;       // number of regions
        public const int NoSector = 15;    // number of sectors
        public const double DBsT = 0.04;     // base case yearly warming
    }

    // Enum that can be used to categorise damage. If the number of options here
    // is changed, make sure to also adjust NoSector!
    public enum Sector
    {
        sWater,
        sForests,
        sHeating,
        sCooling,
        sAgriculture,
        sDryland,
        sSeaProtection,
        sImigration,
        sSpecies,
        sDeath,
        sMorbidity,
        sWetland,
        sEmigration,
        sHurrican,
        sExtratropicalStorms
    }

}