// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System.Configuration;
using System.IO;
using System;
using Esmf;

namespace Fund
{

    public class TotalDamage
    {
        private TextWriter m_YearRegionSectorWeightingSchemeCsv;
        private TextWriter m_AggregateDamageCsv;
        private TextWriter m_GlobalInputCsv;
        private TextWriter m_RegionInputCsv;
        private TextWriter m_YearInputCsv;
        private TextWriter m_RegionYearInputCsv;

        private Run _run;
        private string _outputPath;
        private Parameters _parameters;
        private Random _rand;
        private Timestep _emissionYear;

        public TextWriter AggregateDamageCsv { get { return m_AggregateDamageCsv; } set { m_AggregateDamageCsv = value; } }
        public TextWriter YearRegionSectorWeightingSchemeCsv { get { return m_YearRegionSectorWeightingSchemeCsv; } set { m_YearRegionSectorWeightingSchemeCsv = value; } }
        public TextWriter GlobalInputCsv { get { return m_GlobalInputCsv; } set { m_GlobalInputCsv = value; } }
        public TextWriter RegionInputCsv { get { return m_RegionInputCsv; } set { m_RegionInputCsv = value; } }
        public TextWriter YearInputCsv { get { return m_YearInputCsv; } set { m_YearInputCsv = value; } }
        public TextWriter RegionYearInputCsv { get { return m_RegionYearInputCsv; } set { m_RegionYearInputCsv = value; } }

        public void WriteAggregateDamage(int RunId, int WeightingschemeId, double Damage, WeightingCombination[] WeightingCombinations)
        {
            if (!_run.CalculateMeanForMonteCarlo && (RunId == -1))
                return;
            else if (!_run.OutputAllMonteCarloRuns && (RunId > 0))
                return;

            if (m_AggregateDamageCsv != null)
            {
                m_AggregateDamageCsv.WriteLine(
                    (_run.OutputVerbal ? _run.Scenario.Name : _run.Scenario.Id.ToString()) +
                    ";" +
                    "" +
                    ";" +
                    "" +
                    ";" +
                    (_run.OutputVerbal ? (RunId == 0 ? "Best guess" : RunId == -1 ? "Mean" : RunId.ToString()) : RunId.ToString()) +
                    ";" +
                    (_run.OutputVerbal ? WeightingCombinations[WeightingschemeId].Name : WeightingschemeId.ToString()) +
                    ";" +
                    Damage.ToString("f15")
                    );


            }

        }

        //public void WriteDamage(int RunId, Damage i_Damage, int WeightingschemeId, double Weight, WeightingCombination[] WeightingCombinations)
        //{
        //    if (m_YearRegionSectorWeightingSchemeCsv != null)
        //    {
        //        m_YearRegionSectorWeightingSchemeCsv.WriteLine(
        //            (_run.OutputVerbal ? _run.Scenario.Name : _run.Scenario.Id.ToString()) +
        //            ";" +
        //            (_run.OutputVerbal ? (RunId == 0 ? "Best guess" : RunId == -1 ? "Mean" : RunId.ToString()) : RunId.ToString()) +
        //            ";" +
        //            "" +
        //            ";" +
        //            "" +
        //            ";" +
        //            (i_Damage.Year + 1950).ToString() +
        //            ";" +
        //            (_run.OutputVerbal ? LegacyRegionNames.GetName(i_Damage.Region) : i_Damage.Region.ToString()) +
        //            ";" +
        //            (_run.OutputVerbal ? Enum.GetName(typeof(Sector), i_Damage.Sector) : ((int)i_Damage.Sector).ToString()) +
        //            ";" +
        //            (_run.OutputVerbal ? WeightingCombinations[WeightingschemeId].Name : WeightingschemeId.ToString()) +
        //            ";" +
        //            (i_Damage.DamageValue * Weight).ToString("f15")
        //            );
        //    }
        //}

        public TotalDamage(Run runData, string outputPath, Parameters parameters, Timestep emissionYear, Random rand = null)
        {
            _parameters = parameters;
            _run = runData;
            _outputPath = outputPath;
            _emissionYear = emissionYear;
            _rand = rand;
        }

        public void Run()
        {
            WeightingCombination[] i_weightingCombinations;
            TextWriter lDimWeightingschemeCsv;

            var parameters = _parameters.GetBestGuess();

            Fund28LegacyWeightingCombinations.GetWeightingCombinationsFromName(_run.WeightingCombination, out i_weightingCombinations, _emissionYear);

            lDimWeightingschemeCsv = new StreamWriter(Path.Combine(_outputPath, "Output - Dim Weightingscheme.csv"));
            for (int i = 0; i < i_weightingCombinations.Length; i++)
                lDimWeightingschemeCsv.WriteLine(i.ToString() + ";" + i_weightingCombinations[i].Name);

            lDimWeightingschemeCsv.Close();

            for (int i = 0; i <= _run.MonteCarloRuns; i++)
            {
                if (i == 0)
                {
                    Console.WriteLine("Best guess run ");

                    parameters = _parameters.GetBestGuess();
                    //_parameters.WriteValuesToCsv(_run.Scenario.Id, i,
                    //     m_GlobalInputCsv, m_RegionInputCsv, m_YearInputCsv, m_RegionYearInputCsv);
                }
                else
                {
                    Console.WriteLine("Run " + i.ToString());

                    parameters = _parameters.GetRandom(_rand);
                    //_parameters.WriteValuesToCsv(_run.Scenario.Id, i,
                    //     m_GlobalInputCsv, m_RegionInputCsv, m_YearInputCsv, m_RegionYearInputCsv);
                }

                DoOneRun(i, i_weightingCombinations, parameters);
            }
        }

        public void DoOneRun(int RunId, WeightingCombination[] i_weightingCombinations, ParameterValues parameters)
        {
            double i_aggregatedDamage;
            ModelOutput i_output1;

            // Create Output object for run 1, set addmp to 0 so that
            // the extra greenhouse gases are not emitted and then run
            // the model
            i_output1 = new ModelOutput();

            var fundWorkflow = FundModel.GetModel();

            var result1 = fundWorkflow.Run(parameters);

            i_output1.Load(result1);

            for (int i = 0; i < i_weightingCombinations.Length; i++)
            {
                i_weightingCombinations[i].CalculateWeights(i_output1);
                i_aggregatedDamage = i_weightingCombinations[i].AddDamagesUp(i_output1.Damages, _run.YearsToAggregate, _emissionYear);

                WriteAggregateDamage(RunId, i, i_aggregatedDamage, i_weightingCombinations);

            }

            //for (int l = 0; l < i_output1.Damages.Count; l++)
            //{
            //    i_Damage = i_output1.Damages[l];
            //    if ((i_Damage.Year >= _emissionYear.Value) && (i_Damage.Year < _emissionYear.Value + _run.YearsToAggregate))
            //    {
            //        for (int k = 0; k < i_weightingCombinations.Length; k++)
            //            WriteDamage(RunId, i_Damage, k, i_weightingCombinations[k][i_Damage.Year, i_Damage.Region], i_weightingCombinations);
            //    }
            //}
        }

    }
}