// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System;
using Esmf;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using Fund.CommonDimensions;
using MathNet.Numerics.Statistics;


namespace Fund
{

    public enum MarginalGas { C, CH4, N2O, SF6 }

    public class TMarginalRun
    {
        private TextWriter m_YearRegionSectorWeightingSchemeCsv;
        private TextWriter m_AggregateDamageCsv;
        private TextWriter m_GlobalInputCsv;
        private TextWriter m_RegionInputCsv;
        private TextWriter m_YearInputCsv;
        private TextWriter m_RegionYearInputCsv;

        private MarginalGas _gas;
        private Timestep _emissionyear;
        private string _outputPath;
        private Parameters _parameters;

        private Random _rand;

        public TextWriter AggregateDamageCsv { get { return m_AggregateDamageCsv; } set { m_AggregateDamageCsv = value; } }
        public TextWriter YearRegionSectorWeightingSchemeCsv { get { return m_YearRegionSectorWeightingSchemeCsv; } set { m_YearRegionSectorWeightingSchemeCsv = value; } }
        public TextWriter GlobalInputCsv { get { return m_GlobalInputCsv; } set { m_GlobalInputCsv = value; } }
        public TextWriter RegionInputCsv { get { return m_RegionInputCsv; } set { m_RegionInputCsv = value; } }
        public TextWriter YearInputCsv { get { return m_YearInputCsv; } set { m_YearInputCsv = value; } }
        public TextWriter RegionYearInputCsv { get { return m_RegionYearInputCsv; } set { m_RegionYearInputCsv = value; } }
        public TextWriter SummaryCsv { get; set; }
        public Action<Esmf.Model.Model> AdditionalInitCode { get; set; }
        public string WeightingCombination { get; set; }
        public int MonteCarloRuns { get; set; }
        public int YearsToAggregate { get; set; }
        public bool OutputVerbal { get; set; }
        public string ScenarioName { get; set; }
        public int ScenarioId { get; set; }
        public bool CalculateMeanForMonteCarlo { get; set; }
        public bool OutputAllMonteCarloRuns { get; set; }


        public TMarginalRun(MarginalGas gas, Timestep emissionyear, string outputPath, Parameters parameters, string weightingCombination, Random rand = null)
        {
            _parameters = parameters;
            _gas = gas;
            _emissionyear = emissionyear;
            _outputPath = outputPath;
            _rand = rand;
            this.WeightingCombination = weightingCombination;
            YearsToAggregate = 1000;
        }

        // DA: Run the model once, write away income (GDP), population, market impacts, non market
        // impacts (prn files)
        // Run the model again, with a slightly higher emissions, additional emissions are in the file
        // addco2.001 (million ton of carbons) Write the results of the second run out to (out files)
        public double[] Run()
        {
            WeightingCombination[] i_weightingCombinations;

            Fund28LegacyWeightingCombinations.GetWeightingCombinationsFromName(this.WeightingCombination, out i_weightingCombinations, _emissionyear);

            var sccResults = new ConcurrentBag<double>[i_weightingCombinations.Length];
            sccResults = new ConcurrentBag<double>[i_weightingCombinations.Length];

            for (int l = 0; l < i_weightingCombinations.Length; l++)
                sccResults[l] = new ConcurrentBag<double>();

            Console.Write("Best guess run ");

            var parameters = _parameters.GetBestGuess();

            var bgRes = DoOneRun(0, i_weightingCombinations, parameters);

            int currentRun = 0;

            int batchSize = 100;

            int batchCount = (MonteCarloRuns / batchSize) + 1;

            for (int currentBatch = 0; currentBatch < batchCount; currentBatch++)
            {
                int runsForThisBatch = currentBatch == batchCount - 1 ? MonteCarloRuns % batchSize : batchSize;

                Parallel.ForEach(
                    _parameters.GetRandom(_rand, runsForThisBatch, currentBatch * batchSize),
                    () =>
                    {
                        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                        Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                        return 0;
                    },
                    (p, pls, dummy) =>
                    {
                        int tempCurrentCount = Interlocked.Increment(ref currentRun);
                        Console.Write("\rRun {0}                ", tempCurrentCount);

                        var mcRes = DoOneRun(p.RunId.Value, i_weightingCombinations, p);

                        if (!double.IsNaN(mcRes[0]) && !double.IsInfinity(mcRes[0]))
                        {
                            for (int l = 0; l < i_weightingCombinations.Length; l++)
                                sccResults[l].Add(mcRes[l]);
                        }

                        return 0;
                    },
                    (dummy) => { });
                GC.Collect();
            }
            Console.WriteLine();

            double[] res2 = new double[i_weightingCombinations.Length];

            if (MonteCarloRuns > 0)
            {
                for (int l = 0; l < i_weightingCombinations.Length; l++)
                {
                    res2[l] = sccResults[l].Average();

                    WriteAggregateDamage(-1, l, res2[l], i_weightingCombinations);

                    WriteSummaryDamage(l, bgRes[l], sccResults[l], i_weightingCombinations);
                }
            }

            return res2;
        }

        public double[] DoOneRun(int RunId, WeightingCombination[] i_weightingCombinations, ParameterValues parameters)
        {
            ModelOutput i_output2;
            Damages i_marginalDamages;
            double i_aggregatedDamage;
            ModelOutput i_output1;

            // Create Output object for run 1, set addmp to 0 so that
            // the extra greenhouse gases are not emitted and then run
            // the model
            i_output1 = new ModelOutput();

            var f1 = FundModel.GetModel();
            f1["ImpactWaterResources"].Variables["water"].StoreOutput = true;
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
            f1["SocioEconomic"].Variables["income"].StoreOutput = true;
            f1["Population"].Variables["population"].StoreOutput = true;

            if (AdditionalInitCode != null)
                AdditionalInitCode(f1);

            var result1 = f1.Run(parameters);

            i_output1.Load(result1);

            // Create Output object for run 2, set addmp to 1 so that
            // the extra greenhouse gases for the marginal run are
            // emitted and then run the model
            i_output2 = new ModelOutput();

            var f2 = FundModel.GetModel();
            f2["ImpactWaterResources"].Variables["water"].StoreOutput = true;
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
            f2["SocioEconomic"].Variables["income"].StoreOutput = true;
            f2["Population"].Variables["population"].StoreOutput = true;

            if (AdditionalInitCode != null)
                AdditionalInitCode(f2);

            f2.AddComponent("marginalemission", typeof(Fund.Components.MarginalEmissionComponent), "emissions");
            f2["marginalemission"].Parameters["emissionperiod"].SetValue(_emissionyear);
            switch (_gas)
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

            var result2 = f2.Run(parameters);

            i_output2.Load(result2);

            Fund28LegacyWeightingCombinations.GetWeightingCombinationsFromName(this.WeightingCombination, out i_weightingCombinations, _emissionyear);

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
            i_marginalDamages = Damages.CalculateMarginalDamage(i_output1.Damages, i_output2.Damages);

            double[] i_weightedAggregatedDamages = new double[i_weightingCombinations.Length];

            for (int i = 0; i < i_weightingCombinations.Length; i++)
            {
                i_weightingCombinations[i].CalculateWeights(i_output1);
                i_aggregatedDamage = i_weightingCombinations[i].AddDamagesUp(i_marginalDamages, YearsToAggregate, _emissionyear);

                i_weightedAggregatedDamages[i] = i_aggregatedDamage;

                WriteAggregateDamage(RunId, i, i_aggregatedDamage, i_weightingCombinations);

                // Console.Write(i_weightingCombinations[i].Name + ": ");
                // Console.WriteLine(Convert.ToString(i_aggregatedDamage));
            }

            if (m_YearRegionSectorWeightingSchemeCsv != null)
            {
                foreach (var i_Damage in i_marginalDamages)
                {
                    if ((i_Damage.Year >= _emissionyear.Value) && (i_Damage.Year < _emissionyear.Value + this.YearsToAggregate))
                    {
                        for (int k = 0; k < i_weightingCombinations.Length; k++)
                            WriteMarginalDamage(RunId, i_Damage, k, i_weightingCombinations[k][i_Damage.Year, i_Damage.Region], i_weightingCombinations);
                    }
                }
            }

            return i_weightedAggregatedDamages;
        }

        public void WriteMarginalDamage(int RunId, Damage i_Damage, int WeightingschemeId, double Weight, WeightingCombination[] WeightingCombinations)
        {

            if (m_YearRegionSectorWeightingSchemeCsv != null)
            {
                int gas = Convert.ToInt32(_gas);

                m_YearRegionSectorWeightingSchemeCsv.WriteLine(
                    (OutputVerbal ? ScenarioName : ScenarioId.ToString()) +
                    ";" +
                    (OutputVerbal ? (RunId == 0 ? "Best guess" : RunId == -1 ? "Mean" : RunId.ToString()) : RunId.ToString()) +
                    ";" +
                    (OutputVerbal ? (gas == 0 ? "C" : gas == 1 ? "CH4" : gas == 2 ? "N2O" : gas == 3 ? "SF6" : "ERROR") : ((int)_gas).ToString()) +
                    ";" +
                    _emissionyear.ToString() +
                    ";" +
                    (i_Damage.Year + 1950).ToString() +
                    ";" +
                    (OutputVerbal ? i_Damage.Region.ToString() : i_Damage.Region.ToString()) +
                    ";" +
                    (OutputVerbal ? Enum.GetName(typeof(Sector), i_Damage.Sector) : ((int)i_Damage.Sector).ToString()) +
                    ";" +
                    (OutputVerbal ? WeightingCombinations[WeightingschemeId].Name : WeightingschemeId.ToString()) +
                    ";" +
                    (i_Damage.DamageValue * Weight).ToString("f15")
                    );
            }
        }

        public void WriteAggregateDamage(int RunId, int WeightingschemeId, double Damage, WeightingCombination[] WeightingCombinations)
        {
            if (!CalculateMeanForMonteCarlo && (RunId == -1))
                return;
            else if (!OutputAllMonteCarloRuns && (RunId > 0))
                return;

            if (m_AggregateDamageCsv != null)
            {
                int gas = (int)_gas;

                m_AggregateDamageCsv.WriteLine(
                    (OutputVerbal ? ScenarioName : ScenarioId.ToString()) +
                    ";" +
                    (OutputVerbal ? (gas == 0 ? "C" : gas == 1 ? "CH4" : gas == 2 ? "N2O" : gas == 3 ? "SF6" : "ERROR") : ((int)_gas).ToString()) +
                    ";" +
                    _emissionyear.ToString() +
                    ";" +
                    (OutputVerbal ? (RunId == 0 ? "Best guess" : RunId == -1 ? "Mean" : RunId.ToString()) : RunId.ToString()) +
                    ";" +
                    (OutputVerbal ? WeightingCombinations[WeightingschemeId].Name : WeightingschemeId.ToString()) +
                    ";" +
                    Damage.ToString("f15")
                    );


            }
        }

        public void WriteSummaryDamage(int WeightingschemeId, double bgDamage, IEnumerable<double> damages, WeightingCombination[] WeightingCombinations)
        {
            if (SummaryCsv != null)
            {
                int gas = (int)_gas;

                var stats = new DescriptiveStatistics(damages);

                var sortedDamages = damages.OrderBy(i => i).ToArray();

                double count = sortedDamages.Length;

                int skiptake0_001 = (int)(count * 0.001 / 2.0);
                int skiptake0_01 = (int)(count * 0.01 / 2.0);
                int skiptake0_05 = (int)(count * 0.05 / 2.0);

                var trimmedMean0_001 = sortedDamages.Skip(skiptake0_001).Take(sortedDamages.Length - 2 * skiptake0_001).Mean();

                var trimmedMean0_01 = sortedDamages.Skip(skiptake0_01).Take(sortedDamages.Length - 2 * skiptake0_01).Mean();

                var trimmedMean0_05 = sortedDamages.Skip(skiptake0_05).Take(sortedDamages.Length - 2 * skiptake0_05).Mean();


                SummaryCsv.WriteLine("{0};{1};{2};{3};{4:f15};{5:f15};{6:f15};{7:f15};{8:f15};{9:f15};{10:f15};{11:f15};{12:f15};{13:f15};{14:f15};{15:f15};{16:f15}",
                    OutputVerbal ? ScenarioName : ScenarioId.ToString(),
                    OutputVerbal ? (gas == 0 ? "C" : gas == 1 ? "CH4" : gas == 2 ? "N2O" : gas == 3 ? "SF6" : "ERROR") : ((int)_gas).ToString(),
                    _emissionyear,
                    OutputVerbal ? WeightingCombinations[WeightingschemeId].Name : WeightingschemeId.ToString(),
                    bgDamage,
                    stats.Mean,
                    trimmedMean0_001,
                    trimmedMean0_01,
                    trimmedMean0_05,
                    stats.Median,
                    stats.StandardDeviation,
                    stats.Variance,
                    stats.Skewness,
                    stats.Kurtosis,
                    stats.Minimum,
                    stats.Maximum,
                    Math.Sqrt(stats.Variance) / Math.Sqrt(stats.Count)
                    );


            }
        }
    }
}