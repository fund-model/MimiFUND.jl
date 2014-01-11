// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Esmf;
using System.Data.Common;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;

namespace Fund
{
    public class LongtermDiagnosticOutput
    {
        public static void Run(string argument)
        {
            // This set contains the names of the variables
            // that are to be computed in this run
            var toCompute = new HashSet<string>();

            // If a level is specified add the variables of
            // that level, otherwise use the name of the variable
            // that was passed on the command line
            int level;
            if (Int32.TryParse(argument, out level))
            {
                switch (level)
                {
                    case 1:
                        toCompute.Add("SCC-2010-0prtp");
                        toCompute.Add("SCC-2010-1prtp");
                        toCompute.Add("SCC-2010-3prtp");

                        toCompute.Add("SCC-2010-0prtp-AvgEw");
                        toCompute.Add("SCC-2010-1prtp-AvgEw");
                        toCompute.Add("SCC-2010-3prtp-AvgEw");

                        toCompute.Add("SCCH4-2010-1prtp");
                        toCompute.Add("SCCH4-2010-1prtp-AvgEw");

                        toCompute.Add("SCN2O-2010-1prtp");
                        toCompute.Add("SCN2O-2010-1prtp-AvgEw");

                        toCompute.Add("SCSF6-2010-1prtp");
                        toCompute.Add("SCSF6-2010-1prtp-AvgEw");

                        break;
                    case 2:
                        toCompute.Add("ESCC-1000-2010-1prtp");
                        toCompute.Add("ESCC-1000-2010-1prtp-SE");

                        break;
                    case 3:
                        toCompute.Add("ESCC-10000-2010-1prtp");
                        toCompute.Add("ESCC-10000-2010-1prtp-SE");

                        break;
                }
            }
            else
                toCompute.Add(argument);

            // This hashtable contains the computed output
            var computedOutput = new Dictionary<string, double>();

            foreach (var v in toCompute)
            {
                if (!computedOutput.ContainsKey(v))
                {
                    switch (v)
                    {
                        case "SCC-2010-0prtp":
                            computedOutput.Add(v, GetSCGas(MarginalGas.C, 0.0, false));
                            break;
                        case "SCC-2010-1prtp":
                            computedOutput.Add(v, GetSCGas(MarginalGas.C, 0.01, false));
                            break;
                        case "SCC-2010-3prtp":
                            computedOutput.Add(v, GetSCGas(MarginalGas.C, 0.03, false));
                            break;
                        case "SCC-2010-0prtp-AvgEw":
                            computedOutput.Add(v, GetSCGas(MarginalGas.C, 0.0, true));
                            break;
                        case "SCC-2010-1prtp-AvgEw":
                            computedOutput.Add(v, GetSCGas(MarginalGas.C, 0.01, true));
                            break;
                        case "SCC-2010-3prtp-AvgEw":
                            computedOutput.Add(v, GetSCGas(MarginalGas.C, 0.03, true));
                            break;
                        case "SCCH4-2010-1prtp":
                            computedOutput.Add(v, GetSCGas(MarginalGas.CH4, 0.01, false));
                            break;
                        case "SCCH4-2010-1prtp-AvgEw":
                            computedOutput.Add(v, GetSCGas(MarginalGas.CH4, 0.01, true));
                            break;
                        case "SCN2O-2010-1prtp":
                            computedOutput.Add(v, GetSCGas(MarginalGas.N2O, 0.01, false));
                            break;
                        case "SCN2O-2010-1prtp-AvgEw":
                            computedOutput.Add(v, GetSCGas(MarginalGas.N2O, 0.01, true));
                            break;
                        case "SCSF6-2010-1prtp":
                            computedOutput.Add(v, GetSCGas(MarginalGas.SF6, 0.01, false));
                            break;
                        case "SCSF6-2010-1prtp-AvgEw":
                            computedOutput.Add(v, GetSCGas(MarginalGas.SF6, 0.01, true));
                            break;
                        case "ESCC-100-2010-1prtp":
                        case "ESCC-100-2010-1prtp-SE":
                            {
                                var stats = GetSCGasMonteCarlo(MarginalGas.C, 0.01, false, 100);
                                computedOutput.Add("ESCC-100-2010-1prtp", stats.Item1);
                                computedOutput.Add("ESCC-100-2010-1prtp-SE", stats.Item2);
                            }
                            break;
                        case "ESCC-1000-2010-1prtp":
                        case "ESCC-1000-2010-1prtp-SE":
                            {
                                var stats = GetSCGasMonteCarlo(MarginalGas.C, 0.01, false, 1000);
                                computedOutput.Add("ESCC-1000-2010-1prtp", stats.Item1);
                                computedOutput.Add("ESCC-1000-2010-1prtp-SE", stats.Item2);
                            }
                            break;
                        case "ESCC-10000-2010-1prtp":
                        case "ESCC-10000-2010-1prtp-SE":
                            {
                                var stats = GetSCGasMonteCarlo(MarginalGas.C, 0.01, false, 10000);
                                computedOutput.Add("ESCC-10000-2010-1prtp", stats.Item1);
                                computedOutput.Add("ESCC-10000-2010-1prtp-SE", stats.Item2);
                            }
                            break;
                    }
                }
            }

            // Write everything to file
            using (var f = File.CreateText("Data\\Output - LongtermDiagnostic.csv"))
            {
                var d = DateTime.UtcNow;
                f.WriteLine("\"{0}\";\"{1}\";{2:f15}", "Date", "Variable", "Value");

                foreach (var kv in computedOutput)
                {
                    f.WriteLine("\"{0}\";\"{1}\";{2:f15}", d, kv.Key, kv.Value);

                    Console.WriteLine("{0,-30} {1,10:f2}", kv.Key, kv.Value);
                }
            }
        }

        private static double GetSCGas(MarginalGas gas, double prtp, bool equityWeights)
        {
            var parameters = new Parameters();
            parameters.ReadDirectory(@"Data\Base");

            var m = new MarginalDamage3()
                {
                    EmissionYear = Timestep.FromYear(2010),
                    Eta = 1.0,
                    Gas = gas,
                    Parameters = parameters.GetBestGuess(),
                    Prtp = prtp,
                    UseEquityWeights = equityWeights
                };

            double scc = m.Start();

            return scc;
        }

        private static Tuple<double, double> GetSCGasMonteCarlo(MarginalGas gas, double prtp, bool equityWeights, int monteCarloRuns)
        {
            var parameters = new Parameters();
            parameters.ReadDirectory(@"Data\Base");

            var fm = FundModel.GetModel();
            fm.Run(parameters.GetBestGuess());

            var rand = new jp.takel.PseudoRandom.MersenneTwister();

            var sccs = new System.Collections.Concurrent.ConcurrentBag<double>();

            Parallel.ForEach(parameters.GetRandom(rand, monteCarloRuns), pv =>
            {
                var m = new MarginalDamage3()
                {
                    EmissionYear = Timestep.FromYear(2010),
                    Eta = 1.0,
                    Gas = gas,
                    Parameters = pv,
                    Prtp = prtp,
                    UseEquityWeights = equityWeights,
                    YearsToAggregate = 290
                };

                double scc = m.Start();

                sccs.Add(scc);
            });

            var stats = new DescriptiveStatistics(sccs);

            return Tuple.Create(stats.Mean, Math.Sqrt(stats.Variance) / Math.Sqrt(stats.Count));
        }

    }
}
