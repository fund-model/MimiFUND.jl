// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using CommandLine;
using System.Threading;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Esmf;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using Fund.CommonDimensions;

namespace Fund
{

    public static class ConsoleApp
    {
        private static Lazy<string> _outputPath = new Lazy<string>(() => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Data"));
        public static string OutputPath { get { return _outputPath.Value; } }

        public static void Run(string configurationFileName)
        {
            TextWriter gDisaggregatedCsvFile = null;

            TextWriter lAggregateMarginalDamage = null;

            TextWriter lGlobalInputCsv = null;
            TextWriter lRegionInputCsv = null;
            TextWriter lYearInputCsv = null;
            TextWriter lRegionYearInputCsv = null;

            SimulationManager lSimulationManager;

            var lDefaultConfigurationFile = Path.Combine(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Data"), "DefaultSimulation.xml");
            lSimulationManager = new SimulationManager(configurationFileName == null ? lDefaultConfigurationFile : configurationFileName);
            lSimulationManager.Load();

            if (!Directory.Exists(ConsoleApp.OutputPath))
                Directory.CreateDirectory(ConsoleApp.OutputPath);

            var lRandom = GetNewRandom(lSimulationManager);

            lAggregateMarginalDamage = TextWriter.Synchronized(new StreamWriter(Path.Combine(ConsoleApp.OutputPath, "Output - Aggregate marginal damage.csv")));
            var TempOutputFile = new StreamWriter(Path.Combine(ConsoleApp.OutputPath, "Output - Temp.csv"));

            if (lSimulationManager.OutputVerbal)
            {
                lAggregateMarginalDamage.Write("Scenario");
                lAggregateMarginalDamage.Write(";");
                lAggregateMarginalDamage.Write("Gas");
                lAggregateMarginalDamage.Write(";");
                lAggregateMarginalDamage.Write("Emissionyear");
                lAggregateMarginalDamage.Write(";");
                lAggregateMarginalDamage.Write("Run");
                lAggregateMarginalDamage.Write(";");
                lAggregateMarginalDamage.Write("Weightingscheme");
                lAggregateMarginalDamage.Write(";");
                lAggregateMarginalDamage.Write("Marginal damage");
                lAggregateMarginalDamage.WriteLine();
            }

            TextWriter lSummaryDamage = TextWriter.Synchronized(new StreamWriter(Path.Combine(ConsoleApp.OutputPath, "Output - Summary damage.csv")));

            if (lSimulationManager.OutputVerbal)
            {
                lSummaryDamage.WriteLine("Scenario;Gas;Emissionyear;Weightingscheme;Bestguess;Mean;TrimMean0.1%;TrimMean1%;TrimMean5%;Median;Std;Var;Skew;Kurt;Min;Max;SE");
            }


            if (lSimulationManager.Runs.Exists((Run r) => r.OutputDisaggregatedData))
            {
                gDisaggregatedCsvFile = TextWriter.Synchronized(new StreamWriter(Path.Combine(ConsoleApp.OutputPath, "Output - Fact YearRegionSectorWeightingscheme.csv")));

                if (lSimulationManager.OutputVerbal)
                {
                    gDisaggregatedCsvFile.Write("Scenario");
                    gDisaggregatedCsvFile.Write(";");
                    gDisaggregatedCsvFile.Write("Run");
                    gDisaggregatedCsvFile.Write(";");
                    gDisaggregatedCsvFile.Write("Marginal Gas");
                    gDisaggregatedCsvFile.Write(";");
                    gDisaggregatedCsvFile.Write("Marginal Emission Year");
                    gDisaggregatedCsvFile.Write(";");
                    gDisaggregatedCsvFile.Write("Year");
                    gDisaggregatedCsvFile.Write(";");
                    gDisaggregatedCsvFile.Write("Region");
                    gDisaggregatedCsvFile.Write(";");
                    gDisaggregatedCsvFile.Write("Sector");
                    gDisaggregatedCsvFile.Write(";");
                    gDisaggregatedCsvFile.Write("Weightingscheme");
                    gDisaggregatedCsvFile.Write(";");
                    gDisaggregatedCsvFile.Write("Damage");
                    gDisaggregatedCsvFile.WriteLine();
                }
            }

            using (var lDimGasCsv = new StreamWriter(Path.Combine(ConsoleApp.OutputPath, "Output - Dim Gas.csv")))
            {
                lDimGasCsv.WriteLine("0;C");
                lDimGasCsv.WriteLine("1;CH4");
                lDimGasCsv.WriteLine("2;N2O");
                lDimGasCsv.WriteLine("3;SF6");
            }

            using (var lDimSectorCsv = new StreamWriter(Path.Combine(ConsoleApp.OutputPath, "Output - Dim Sector.csv")))
            {
                lDimSectorCsv.WriteLine("0;eloss;Water");
                lDimSectorCsv.WriteLine("1;eloss;Forests");
                lDimSectorCsv.WriteLine("2;eloss;Heating");
                lDimSectorCsv.WriteLine("3;eloss;Cooling");
                lDimSectorCsv.WriteLine("4;eloss;Agriculture");
                lDimSectorCsv.WriteLine("5;eloss;Dryland");
                lDimSectorCsv.WriteLine("6;eloss;SeaProtection");
                lDimSectorCsv.WriteLine("7;eloss;Imigration");
                lDimSectorCsv.WriteLine("8;sloss;Species");
                lDimSectorCsv.WriteLine("9;sloss;Death");
                lDimSectorCsv.WriteLine("10;sloss;Morbidity");
                lDimSectorCsv.WriteLine("11;sloss;Wetland");
                lDimSectorCsv.WriteLine("12;sloss;Emigration");
                lDimSectorCsv.WriteLine("13;eloss;Hurricane");
                lDimSectorCsv.WriteLine("14;eloss;ExtratropicalStorms");
            }


            using (var lDimScenarioCsv = new StreamWriter(Path.Combine(ConsoleApp.OutputPath, "Output - Dim Scenario.csv")))
            {
                foreach (Scenario lScenario in lSimulationManager.Scenarios)
                {
                    // Write Scenario dimension file
                    lDimScenarioCsv.Write(lScenario.Id);
                    lDimScenarioCsv.Write(";");
                    lDimScenarioCsv.Write(lScenario.Name);
                    lDimScenarioCsv.WriteLine();
                }
            }


            using (var lDimYearCsv = new StreamWriter(Path.Combine(ConsoleApp.OutputPath, "Output - Dim Year.csv")))
            {
                for (int i = 1950; i <= 2300; i++)
                {
                    string lYearStr = i.ToString();
                    lDimYearCsv.Write(lYearStr);
                    lDimYearCsv.Write(";");
                    lDimYearCsv.Write(lYearStr.Substring(0, 2));
                    lDimYearCsv.Write("xx;");
                    lDimYearCsv.Write(lYearStr.Substring(0, 3));
                    lDimYearCsv.Write("x;");
                    lDimYearCsv.Write(lYearStr.Substring(0, 4));
                    lDimYearCsv.WriteLine();
                }
            }

            var lDimEmissionYear = new ConcurrentBag<Timestep>();

            if (lSimulationManager.OutputInputParameters)
            {
                lGlobalInputCsv = TextWriter.Synchronized(new StreamWriter(Path.Combine(ConsoleApp.OutputPath, "Output - Fact Parameter.csv")));
                lRegionInputCsv = TextWriter.Synchronized(new StreamWriter(Path.Combine(ConsoleApp.OutputPath, "Output - Fact ParameterRegion.csv")));
                lYearInputCsv = TextWriter.Synchronized(new StreamWriter(Path.Combine(ConsoleApp.OutputPath, "Output - Fact ParameterYear.csv")));
                lRegionYearInputCsv = TextWriter.Synchronized(new StreamWriter(Path.Combine(ConsoleApp.OutputPath, "Output - Fact ParameterYearRegion.csv")));
            }

            if (lSimulationManager.RunParallel && !lSimulationManager.SameRandomStreamPerRun)
            {
                throw new ArgumentException("Cannot run in parallel but without random stream per run");
            }

            var parallelOptions = new System.Threading.Tasks.ParallelOptions()
            {
                MaxDegreeOfParallelism = lSimulationManager.RunParallel ? -1 : 1
            };

            if (lSimulationManager.RunParallel)
            {
                var parameterDefinition = new Parameters();
                parameterDefinition.ReadDirectory(@"Data\Base");

                // Create a new model that inits itself from the parameters just loaded
                var model = FundModel.GetModel();
                model.Run(parameterDefinition.GetBestGuess());
            }

            System.Threading.Tasks.Parallel.ForEach<Run, object>(
                lSimulationManager.Runs,
                parallelOptions,
                () =>
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                    return null;
                },
                (lRun, loopState, dummy) =>
                {
                    var tlRandom = lSimulationManager.SameRandomStreamPerRun ? GetNewRandom(lSimulationManager) : lRandom;

                    var lParam = new Parameters();

                    foreach (string filename in lRun.Scenario.ExcelFiles)
                        lParam.ReadExcelFile(filename);

                    Console.WriteLine(lRun.Scenario.Name);

                    if (lRun.Mode == RunMode.MarginalRun)
                    {
                        var lMarginalRun = new TMarginalRun(lRun, lRun.MarginalGas, lRun.EmissionYear, ConsoleApp.OutputPath, lParam, lRandom);

                        lDimEmissionYear.Add(lRun.EmissionYear);

                        lMarginalRun.AggregateDamageCsv = lAggregateMarginalDamage;
                        lMarginalRun.SummaryCsv = lSummaryDamage;

                        if (lRun.OutputDisaggregatedData)
                        {
                            lMarginalRun.YearRegionSectorWeightingSchemeCsv = gDisaggregatedCsvFile;
                        }

                        if (lSimulationManager.OutputInputParameters)
                        {
                            lMarginalRun.GlobalInputCsv = lGlobalInputCsv;
                            lMarginalRun.RegionInputCsv = lRegionInputCsv;
                            lMarginalRun.YearInputCsv = lYearInputCsv;
                            lMarginalRun.RegionYearInputCsv = lRegionYearInputCsv;
                        }

                        var watch = new System.Diagnostics.Stopwatch();
                        watch.Start();
                        lMarginalRun.Run();
                        watch.Stop();
                        //Console.WriteLine("Elapsed time: {0}", watch.Elapsed);
                    }
                    else if (lRun.Mode == RunMode.FullMarginalRun)
                    {
                        MarginalGas[] gases = { MarginalGas.C };
                        foreach (MarginalGas gas in gases)
                        {
                            for (int emissionyear = 2010; emissionyear <= 2100; emissionyear += 5)
                            {
                                lDimEmissionYear.Add(Timestep.FromYear(emissionyear));

                                Console.WriteLine("Now doing year {0} and gas {1}", emissionyear, gas);
                                // DA: Use MargMain for marginal cost, use Main for total cost and optimisation
                                // modes
                                var lMarginalRun = new TMarginalRun(lRun, gas, Timestep.FromYear(emissionyear), ConsoleApp.OutputPath, lParam, lRandom);

                                lMarginalRun.AggregateDamageCsv = lAggregateMarginalDamage;

                                if (lRun.OutputDisaggregatedData)
                                {
                                    lMarginalRun.YearRegionSectorWeightingSchemeCsv = gDisaggregatedCsvFile;
                                }

                                if (lSimulationManager.OutputInputParameters)
                                {
                                    lMarginalRun.GlobalInputCsv = lGlobalInputCsv;
                                    lMarginalRun.RegionInputCsv = lRegionInputCsv;
                                    lMarginalRun.YearInputCsv = lYearInputCsv;
                                    lMarginalRun.RegionYearInputCsv = lRegionYearInputCsv;
                                }

                                var watch = new System.Diagnostics.Stopwatch();
                                watch.Start();
                                lMarginalRun.Run();
                                watch.Stop();
                                //Console.WriteLine("Elapsed time: {0}", watch.Elapsed);
                            }
                        }
                    }
                    else if (lRun.Mode == RunMode.TotalRun)
                    {
                        // DA: Use MargMain for marginal cost, use Main for total cost and optimisation
                        // modes
                        var lTotalDamageRun = new TotalDamage(lRun, ConsoleApp.OutputPath, lParam, lRun.EmissionYear, lRandom);

                        lTotalDamageRun.AggregateDamageCsv = lAggregateMarginalDamage;

                        if (lRun.OutputDisaggregatedData)
                        {
                            lTotalDamageRun.YearRegionSectorWeightingSchemeCsv = gDisaggregatedCsvFile;
                        }

                        if (lSimulationManager.OutputInputParameters)
                        {
                            lTotalDamageRun.GlobalInputCsv = lGlobalInputCsv;
                            lTotalDamageRun.RegionInputCsv = lRegionInputCsv;
                            lTotalDamageRun.YearInputCsv = lYearInputCsv;
                            lTotalDamageRun.RegionYearInputCsv = lRegionYearInputCsv;
                        }

                        lTotalDamageRun.Run();

                    }
                    return null;
                },
                (dummy) => { return; });

            lSummaryDamage.Close();
            lAggregateMarginalDamage.Close();
            TempOutputFile.Close();

            using (var lDimEmissionYearCsv = new StreamWriter(Path.Combine(ConsoleApp.OutputPath, "Output - Dim Emissionyear.csv")))
            {
                foreach (Timestep emissionyear in lDimEmissionYear.Distinct().OrderBy(i => i.Value))
                    lDimEmissionYearCsv.WriteLine("{0};{1}", emissionyear, emissionyear);
            }

            if (lSimulationManager.Runs.Exists((Run run) => run.OutputDisaggregatedData))
            {
                gDisaggregatedCsvFile.Close();
            }

            if (lSimulationManager.OutputInputParameters)
            {
                lGlobalInputCsv.Close();
                lRegionInputCsv.Close();
                lYearInputCsv.Close();
                lRegionYearInputCsv.Close();
            }

        }

        private static Random GetNewRandom(SimulationManager lSimulationManager)
        {
            Random lRandom;
            switch (lSimulationManager.Rng)
            {
                case RandomNumberGenerator.DotNet:
                    if (lSimulationManager.Randomize)
                        lRandom = new Random();
                    else
                        lRandom = new Random(34514325);
                    break;
                case RandomNumberGenerator.MersenneTwister:
                    if (lSimulationManager.Randomize)
                        throw new Exception("Support for randomization is not there for mersenne twister");
                    else
                        lRandom = new jp.takel.PseudoRandom.MersenneTwister();
                    break;
                default: throw new Exception("You specified a RNG that is not implemented");
            }
            return lRandom;
        }

        public static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception)args.ExceptionObject;

            Console.WriteLine("An unhandled exception occurred that is handled by the the ConsoleApp.UnhandledExceptionHandler:");
            Console.WriteLine();
            Console.WriteLine(e.GetType().Name);
            Console.WriteLine(e.Message);
            Console.WriteLine(e.Source);
            Console.WriteLine(e.StackTrace);
        }
    }

}