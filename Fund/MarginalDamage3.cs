// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System.Collections.Generic;
using Esmf;
using Fund.CommonDimensions;
using System;

namespace Fund
{
    public class MarginalDamage3
    {
        public delegate void AdditionalInitMethodHandler(Esmf.Model.Model model);

        public ParameterValues Parameters { get; set; }
        public MarginalGas Gas { get; set; }
        public Timestep EmissionYear { get; set; }
        public bool UseEquityWeights { get; set; }
        public AdditionalInitMethodHandler AdditionalInitMethod { get; set; }
        public double Prtp { get; set; }
        public double Eta { get; set; }
        public int YearsToAggregate { get; set; }

        public MarginalDamage3()
        {
            Prtp = 0.001;
            Eta = 1.0;
            YearsToAggregate = 1000;
        }

        public double Start()
        {
            int yearsToRun = Math.Min(1049, EmissionYear.Value + YearsToAggregate);

            var f1 = FundModel.GetModel(storeFullVariablesByDefault: false, years: yearsToRun);
            f1["impactwaterresources"].Variables["water"].StoreOutput = true;
            f1["ImpactForests"].Variables["forests"].StoreOutput = true;
            f1["ImpactHeating"].Variables["heating"].StoreOutput = true;
            f1["ImpactCooling"].Variables["cooling"].StoreOutput = true;
            f1["ImpactAgriculture"].Variables["agcost"].StoreOutput = true;
            f1["ImpactSeaLevelRise"].Variables["drycost"].StoreOutput = true;
            f1["ImpactSeaLevelRise"].Variables["protcost"].StoreOutput = true;
            f1["ImpactSeaLevelRise"].Variables["entercost"].StoreOutput = true;
            f1["ImpactTropicalStorms"].Variables["hurrdam"].StoreOutput = true;
            f1["ImpactExtratropicalStorms"].Variables["extratropicalstormsdam"].StoreOutput = true;
            f1["ImpactBioDiversity"].Variables["species"].StoreOutput = true;
            f1["ImpactDeathMorbidity"].Variables["deadcost"].StoreOutput = true;
            f1["ImpactDeathMorbidity"].Variables["morbcost"].StoreOutput = true;
            f1["ImpactSeaLevelRise"].Variables["wetcost"].StoreOutput = true;
            f1["ImpactSeaLevelRise"].Variables["leavecost"].StoreOutput = true;
            f1["socioeconomic"].Variables["income"].StoreOutput = true;
            f1["Population"].Variables["population"].StoreOutput = true;


            if (AdditionalInitMethod != null)
                AdditionalInitMethod(f1);

            var result1 = f1.Run(Parameters);

            var i_output1 = new ModelOutput();
            i_output1.Load(result1, years: yearsToRun);

            var f2 = FundModel.GetModel(storeFullVariablesByDefault: false, years: yearsToRun);
            f2["impactwaterresources"].Variables["water"].StoreOutput = true;
            f2["ImpactForests"].Variables["forests"].StoreOutput = true;
            f2["ImpactHeating"].Variables["heating"].StoreOutput = true;
            f2["ImpactCooling"].Variables["cooling"].StoreOutput = true;
            f2["ImpactAgriculture"].Variables["agcost"].StoreOutput = true;
            f2["ImpactSeaLevelRise"].Variables["drycost"].StoreOutput = true;
            f2["ImpactSeaLevelRise"].Variables["protcost"].StoreOutput = true;
            f2["ImpactSeaLevelRise"].Variables["entercost"].StoreOutput = true;
            f2["ImpactTropicalStorms"].Variables["hurrdam"].StoreOutput = true;
            f2["ImpactExtratropicalStorms"].Variables["extratropicalstormsdam"].StoreOutput = true;
            f2["ImpactBioDiversity"].Variables["species"].StoreOutput = true;
            f2["ImpactDeathMorbidity"].Variables["deadcost"].StoreOutput = true;
            f2["ImpactDeathMorbidity"].Variables["morbcost"].StoreOutput = true;
            f2["ImpactSeaLevelRise"].Variables["wetcost"].StoreOutput = true;
            f2["ImpactSeaLevelRise"].Variables["leavecost"].StoreOutput = true;
            f2["socioeconomic"].Variables["income"].StoreOutput = true;
            f2["Population"].Variables["population"].StoreOutput = true;

            if (AdditionalInitMethod != null)
                AdditionalInitMethod(f2);

            f2.AddComponent("marginalemission", typeof(Fund.Components.MarginalEmissionComponent), "emissions");
            f2["marginalemission"].Parameters["emissionperiod"].SetValue(EmissionYear);
            switch (Gas)
            {
                case MarginalGas.C:
                    f2["marginalemission"].Parameters["emission"].Bind("emissions", "mco2");
                    f2["climateco2cycle"].Parameters["mco2"].Bind("marginalemission", "modemission");
                    break;
                case MarginalGas.CH4:
                    f2["marginalemission"].Parameters["emission"].Bind("emissions", "globch4");
                    f2["climatech4cycle"].Parameters["globch4"].Bind("marginalemission", "modemission");
                    break;
                case MarginalGas.N2O:
                    f2["marginalemission"].Parameters["emission"].Bind("emissions", "globn2o");
                    f2["climaten2ocycle"].Parameters["globn2o"].Bind("marginalemission", "modemission");
                    break;
                case MarginalGas.SF6:
                    f2["marginalemission"].Parameters["emission"].Bind("emissions", "globsf6");
                    f2["climatesf6cycle"].Parameters["globsf6"].Bind("marginalemission", "modemission");
                    break;
                default:
                    throw new NotImplementedException();
            }

            var result2 = f2.Run(Parameters);

            var i_output2 = new ModelOutput();
            i_output2.Load(result2, years: yearsToRun);

            // Take out growth effect effect of run 2 by transforming
            // the damage from run 2 into % of GDP of run 2, and then
            // multiplying that with GDP of run 1
            for (int year = 1; year < LegacyConstants.NYear; year++)
            {
                for (int region = 0; region < LegacyConstants.NoReg; region++)
                {
                    for (int sector = 0; sector < LegacyConstants.NoSector; sector++)
                    {
                        i_output2.Damages[year, region, (Sector)sector] = (i_output2.Damages[year, region, (Sector)sector] / i_output2.Incomes[year, region]) * i_output1.Incomes[year, region];
                    }
                }
            }

            // Calculate the marginal damage between run 1 and 2 for each
            // year/region/sector
            var i_marginalDamages = Damages.CalculateMarginalDamage(i_output1.Damages, i_output2.Damages);

            var weightingcom = new WeightingCombination();
            if (UseEquityWeights)
            {
                weightingcom.Add(new ConstantDiscountrate(Prtp));
                weightingcom.Add(new EquityWeighting(EmissionYear.Value, -1, Eta));
            }
            else
            {
                weightingcom.Add(new RamseyRegionalDiscounting(Prtp, Eta, EmissionYear.Value));
            }

            weightingcom.CalculateWeights(i_output1);
            var i_aggregatedDamage = weightingcom.AddDamagesUp(i_marginalDamages, YearsToAggregate, EmissionYear);

            return i_aggregatedDamage;
        }
    }
}