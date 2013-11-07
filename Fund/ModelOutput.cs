// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System.Collections;
using System.Collections.Generic;

namespace Fund
{

    // Class that holds all output from one model run
    public class ModelOutput
    {
        private Damages m_damages = new Fund.Damages();
        private Incomes m_incomes = new Fund.Incomes();
        private Populations m_populations = new Fund.Populations();

        public void Load(Esmf.ModelOutput mf, int years = 1049)
        {
            var clock = new Esmf.Clock(Esmf.Timestep.FromSimulationYear(0), Esmf.Timestep.FromSimulationYear(years));
            do
            {
                var t = clock.Current;

                foreach (var r in mf.Dimensions.GetValues<Fund.CommonDimensions.Region>())
                {
                    Damages[t.Value, r.Index, Sector.sWater] = -mf["ImpactWaterResources", "water"][clock.Current, r] * 1000000000;
                    Damages[t.Value, r.Index, Sector.sForests] = -mf["ImpactForests", "forests"][clock.Current, r] * 1000000000;
                    Damages[t.Value, r.Index, Sector.sHeating] = -mf["ImpactHeating", "heating"][clock.Current, r] * 1000000000;
                    Damages[t.Value, r.Index, Sector.sCooling] = -mf["ImpactCooling", "cooling"][clock.Current, r] * 1000000000;
                    Damages[t.Value, r.Index, Sector.sAgriculture] = -mf["ImpactAgriculture", "agcost"][clock.Current, r] * 1000000000;
                    Damages[t.Value, r.Index, Sector.sDryland] = mf["ImpactSeaLevelRise", "drycost"][clock.Current, r] * 1000000000;
                    Damages[t.Value, r.Index, Sector.sSeaProtection] = mf["ImpactSeaLevelRise", "protcost"][clock.Current, r] * 1000000000;
                    Damages[t.Value, r.Index, Sector.sImigration] = mf["ImpactSeaLevelRise", "entercost"][clock.Current, r] * 1000000000;
                    Damages[t.Value, r.Index, Sector.sHurrican] = mf["ImpactTropicalStorms", "hurrdam"][clock.Current, r] * 1000000000;
                    Damages[t.Value, r.Index, Sector.sExtratropicalStorms] = mf["ImpactExtratropicalStorms", "extratropicalstormsdam"][clock.Current, r] * 1000000000;

                    Damages[t.Value, r.Index, Sector.sSpecies] = mf["ImpactBioDiversity", "species"][clock.Current, r] * 1000000000;
                    Damages[t.Value, r.Index, Sector.sDeath] = mf["ImpactDeathMorbidity", "deadcost"][clock.Current, r] * 1000000000;
                    Damages[t.Value, r.Index, Sector.sMorbidity] = mf["ImpactDeathMorbidity", "morbcost"][clock.Current, r] * 1000000000;
                    Damages[t.Value, r.Index, Sector.sWetland] = mf["ImpactSeaLevelRise", "wetcost"][clock.Current, r] * 1000000000;
                    Damages[t.Value, r.Index, Sector.sEmigration] = mf["ImpactSeaLevelRise", "leavecost"][clock.Current, r] * 1000000000;

                    // Add GDP for that year and region to the Output object
                    // income is multiplied by 1 billion, since that is the unit
                    // of income
                    Incomes.Add(clock.Current.Value, r.Index, mf["SocioEconomic", "income"][clock.Current, r] * 1000000000);

                    // Add population for that year and region to the Output object
                    // population is multiplied by 1 million, since that is the unit of
                    // population
                    Populations.Add(clock.Current.Value, r.Index, mf["SocioEconomic", "population"][clock.Current, r] * 1000000);
                }

                clock.Advance();

            } while (!clock.IsDone);
        }

        public Damages Damages { get { return m_damages; } set { m_damages = value; } }
        public Incomes Incomes { get { return m_incomes; } set { m_incomes = value; } }
        public Populations Populations { get { return m_populations; } set { m_populations = value; } }
    }

    public class Damage
    {
        public int Year { get; set; }
        public int Region { get; set; }
        public int Sector { get; set; }
        public double DamageValue { get; set; }
    }

    // Collection of all Damages for one model run
    public class Damages : IEnumerable<Damage>
    {
        private Esmf.JaggedArrayWrapper3<double> _damages = new Esmf.JaggedArrayWrapper3<double>(LegacyConstants.NYear + 1, LegacyConstants.NoReg, LegacyConstants.NoSector);

        public double this[int year, int region, Sector sector]
        {
            get
            {
                return _damages[year, region, (int)sector];
            }
            set
            {
                _damages[year, region, (int)sector] = value;
            }
        }

        public static Damages CalculateMarginalDamage(Damages Damages1, Damages Damages2)
        {
            Damages i_marginalDamages = new Damages();

            for (int year = 0; year < LegacyConstants.NYear + 1; year++)
            {
                for (int region = 0; region < LegacyConstants.NoReg; region++)
                {
                    for (int sector = 0; sector < LegacyConstants.NoSector; sector++)
                    {
                        i_marginalDamages._damages[year, region, sector] = (Damages2._damages[year, region, sector] - Damages1._damages[year, region, sector]) / 10000000;
                    }
                }
            }

            return i_marginalDamages;
        }

        public static Damages CalculateMarginalDamage(Damages Damages1, Damages Damages2, double normalization)
        {
            Damages i_marginalDamages = new Damages();

            for (int year = 0; year < LegacyConstants.NYear + 1; year++)
            {
                for (int region = 0; region < LegacyConstants.NoReg; region++)
                {
                    for (int sector = 0; sector < LegacyConstants.NoSector; sector++)
                    {
                        i_marginalDamages._damages[year, region, sector] = (Damages2._damages[year, region, sector] - Damages1._damages[year, region, sector]) / normalization;
                    }
                }
            }

            return i_marginalDamages;
        }

        public Damages()
        {

        }


        public IEnumerator<Damage> GetEnumerator()
        {
            for (int year = 0; year < LegacyConstants.NYear + 1; year++)
            {
                for (int region = 0; region < LegacyConstants.NoReg; region++)
                {
                    for (int sector = 0; sector < LegacyConstants.NoSector; sector++)
                    {
                        yield return new Damage()
                            {
                                Year = year,
                                Region = region,
                                Sector = sector,
                                DamageValue = _damages[year, region, sector]
                            };
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }

    // GDP for each year and region in US$
    public class Incomes
    {
        private Esmf.JaggedArrayWrapper2<double> m_incomes = new Esmf.JaggedArrayWrapper2<double>(LegacyConstants.NYear + 1, LegacyConstants.NoReg);
        public double this[int Year, int Region] { get { return GetItem(Year, Region); } }

        public void Add(int Year, int Region, double Income)
        {
            m_incomes[Year, Region] = Income;
        }

        public Incomes()
        {
        }

        public double GetItem(int Year, int Region)
        {
            return m_incomes[Year, Region];
        }

    }

    // Population for each year and region in absolute numbers
    public class Populations
    {
        private Esmf.JaggedArrayWrapper2<double> m_populations = new Esmf.JaggedArrayWrapper2<double>(LegacyConstants.NYear + 1, LegacyConstants.NoReg);
        public double this[int Year, int Region] { get { return GetItem(Year, Region); } }

        public void Add(int Year, int Region, double Population)
        {
            m_populations[Year, Region] = Population;
        }

        public Populations()
        {
        }

        public double GetItem(int Year, int Region)
        {
            return m_populations[Year, Region];
        }

    }

}