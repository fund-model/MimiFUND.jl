// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System.Collections.Generic;
using System;
using System.Xml;
using System.IO;
using Esmf;

namespace Fund
{
    public enum RandomNumberGenerator { DotNet, MersenneTwister }

    public class SimulationManager
    {
        private bool _randomize;
        private RandomNumberGenerator _rng;
        private string _fileName;
        private List<Run> _runs = new List<Run>();
        private List<Scenario> _scenarios = new List<Scenario>();
        private bool _outputVerbal;
        private bool _outputInputParameters;

        public bool Randomize { get { return _randomize; } }
        public RandomNumberGenerator Rng { get { return _rng; } }
        public List<Run> Runs { get { return _runs; } }
        public List<Scenario> Scenarios { get { return _scenarios; } }
        public bool OutputVerbal { get { return _outputVerbal; } }
        public bool OutputInputParameters { get { return _outputInputParameters; } }
        public bool SameRandomStreamPerRun { get; private set; }
        public bool RunParallel { get; private set; }

        public SimulationManager(string fileName)
        {
            _fileName = fileName;
        }

        public void Load()
        {
            var dom = new System.Xml.XmlDocument();

            dom.Load(_fileName);

            var root = dom.DocumentElement;

            _randomize = Convert.ToBoolean(root.SelectSingleNode("/Simulation/GlobalParameters/Randomize").InnerText);

            if (root.SelectSingleNode("/Simulation/GlobalParameters/RandomNumberGenerator") != null)
            {
                var rngAsString = root.SelectSingleNode("/Simulation/GlobalParameters/RandomNumberGenerator").InnerText.ToLowerInvariant();

                switch (rngAsString)
                {
                    case "dotnet": _rng = RandomNumberGenerator.DotNet; break;
                    case "mersennetwister": _rng = RandomNumberGenerator.MersenneTwister; break;
                    default: throw new InvalidOperationException("The configuration value for the RandomNumberGenerator is not supported");
                }

            }
            else
                _rng = RandomNumberGenerator.DotNet;

            var defaultYearsToAggregate = Convert.ToInt32(root.SelectSingleNode("/Simulation/GlobalParameters/YearsToAggregate").InnerText);
            var defaultOutputDisaggregated = Convert.ToBoolean(root.SelectSingleNode("/Simulation/GlobalParameters/OutputDisaggregatedData").InnerText);
            var defaultWeightingCombination = root.SelectSingleNode("/Simulation/GlobalParameters/WeightingCombination").InnerText;
            var defaultMonteCarloRuns = Convert.ToInt32(root.SelectSingleNode("/Simulation/GlobalParameters/MonteCarloRuns").InnerText);
            _outputVerbal = Convert.ToBoolean(root.SelectSingleNode("/Simulation/GlobalParameters/OutputVerbal").InnerText);
            _outputInputParameters = Convert.ToBoolean(root.SelectSingleNode("/Simulation/GlobalParameters/OutputInputParameters").InnerText);

            if (root.SelectSingleNode("/Simulation/GlobalParameters/SameRandomStreamPerRun") != null)
                SameRandomStreamPerRun = Convert.ToBoolean(root.SelectSingleNode("/Simulation/GlobalParameters/SameRandomStreamPerRun").InnerText);
            else
                SameRandomStreamPerRun = false;

            if (root.SelectSingleNode("/Simulation/GlobalParameters/RunParallel") != null)
                RunParallel = Convert.ToBoolean(root.SelectSingleNode("/Simulation/GlobalParameters/RunParallel").InnerText);
            else
                RunParallel = false;

            var scenarioNodes = root.SelectNodes("/Simulation/Scenarios/Scenario");
            var lScenarios = new Dictionary<string, Scenario>();

            var currentScenarioId = 0;
            foreach (XmlNode scenarioNode in scenarioNodes)
            {
                var scenario = new Scenario();
                scenario.Name = (scenarioNode as XmlElement).GetAttribute("name");
                scenario.Id = currentScenarioId;

                var excelfileNodes = scenarioNode.SelectNodes("ExcelFile");
                foreach (XmlNode excelfileNode in excelfileNodes)
                {
                    string excelfilename = Path.IsPathRooted(excelfileNode.InnerText) ? excelfileNode.InnerText : Path.Combine(Path.GetDirectoryName(Path.GetFullPath(_fileName)), excelfileNode.InnerText);
                    scenario.ExcelFiles.Add(excelfilename);
                }

                lScenarios.Add(scenario.Name, scenario);
                _scenarios.Add(scenario);

                currentScenarioId++;
            }

            var runNodes = root.SelectNodes("/Simulation/Runs/Run");

            foreach (XmlNode runNode in runNodes)
            {
                var run = new Run();

                run.Scenario = lScenarios[runNode.SelectSingleNode("Scenario").InnerText];

                if (runNode.SelectSingleNode("MonteCarloRuns") != null)
                    run.MonteCarloRuns = Convert.ToInt32(runNode.SelectSingleNode("MonteCarloRuns").InnerText);
                else
                    run.MonteCarloRuns = defaultMonteCarloRuns;

                switch (runNode.SelectSingleNode("Mode").InnerText)
                {
                    case "Marginal": run.Mode = RunMode.MarginalRun; break;
                    case "Total": run.Mode = RunMode.TotalRun; break;
                    case "FullMarginal": run.Mode = RunMode.FullMarginalRun; break;
                    default: throw new ApplicationException("Invalid value for Mode in configuration file");
                }

                if (runNode.SelectSingleNode("YearsToAggregate") != null)
                    run.YearsToAggregate = Convert.ToInt32(runNode.SelectSingleNode("YearsToAggregate").InnerText);
                else
                    run.YearsToAggregate = defaultYearsToAggregate;

                if (runNode.SelectSingleNode("CalculateMeanForMonteCarlo") != null)
                    run.CalculateMeanForMonteCarlo = Convert.ToBoolean(runNode.SelectSingleNode("CalculateMeanForMonteCarlo").InnerText);
                else
                    run.CalculateMeanForMonteCarlo = false;

                if (runNode.SelectSingleNode("OutputAllMonteCarloRuns") != null)
                    run.OutputAllMonteCarloRuns = Convert.ToBoolean(runNode.SelectSingleNode("OutputAllMonteCarloRuns").InnerText);
                else
                    run.OutputAllMonteCarloRuns = true;

                if (runNode.SelectSingleNode("WeightingCombination") != null)
                    run.WeightingCombination = runNode.SelectSingleNode("WeightingCombination").InnerText;
                else
                    run.WeightingCombination = defaultWeightingCombination;

                if (runNode.SelectSingleNode("OutputDisaggregatedData") != null)
                    run.OutputDisaggregatedData = Convert.ToBoolean(runNode.SelectSingleNode("OutputDisaggregatedData").InnerText);
                else
                    run.OutputDisaggregatedData = defaultOutputDisaggregated;

                if (runNode.SelectSingleNode("EmissionYear") != null)
                    run.EmissionYear = Timestep.FromYear(Convert.ToInt32(runNode.SelectSingleNode("EmissionYear").InnerText));
                else
                    run.EmissionYear = Timestep.FromYear(2005);

                if (runNode.SelectSingleNode("MarginalGas") != null)
                {
                    switch (runNode.SelectSingleNode("MarginalGas").InnerText)
                    {
                        case "C": run.MarginalGas = MarginalGas.C; break;
                        case "CH4": run.MarginalGas = MarginalGas.CH4; break;
                        case "N2O": run.MarginalGas = MarginalGas.N2O; break;
                        case "SF6": run.MarginalGas = MarginalGas.SF6; break;
                        default: throw new ApplicationException("Invalid value for MarginalGas in configuration file");
                    }
                }
                else
                    run.MarginalGas = MarginalGas.C;

                if (runNode.SelectSingleNode("InitialTax") != null)
                    run.InitialTax = Convert.ToDouble(runNode.SelectSingleNode("InitialTax").InnerText);
                else
                    run.InitialTax = 0.0;

                run.OutputVerbal = _outputVerbal;

                _runs.Add(run);
            }
        }
    }

    public enum RunMode
    {
        MarginalRun,
        TotalRun,
        FullMarginalRun
    }

    public class Run
    {
        public MarginalGas MarginalGas { get; set; }
        public Scenario Scenario { get; set; }
        public int MonteCarloRuns { get; set; }
        public RunMode Mode { get; set; }
        public int YearsToAggregate { get; set; }
        public string WeightingCombination { get; set; }
        public bool OutputVerbal { get; set; }
        public bool CalculateMeanForMonteCarlo { get; set; }
        public bool OutputAllMonteCarloRuns { get; set; }
        public bool OutputDisaggregatedData { get; set; }
        public Timestep EmissionYear { get; set; }
        public double InitialTax { get; set; }
    }

    public class Scenario
    {
        private List<string> _excelFiles = new List<string>();

        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> ExcelFiles { get { return _excelFiles; } }
    }



}