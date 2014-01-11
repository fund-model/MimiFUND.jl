// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmf;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;

namespace Fund
{
    public static class PerformanceTest
    {
        public static void Run()
        {
            int monteCarloRuns = 10000;

            // Load parameters
            var parameters = new Parameters();
            parameters.ReadDirectory(@"Data\Base");

            // Do one best guess run
            {
                var model = FundModel.GetModel();
                //var model = new FundWorkflow(parameters.GetBestGuess());
                model.Run(parameters.GetBestGuess());
            }

            int currentRun = 0;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            ParallelMonteCarlo.DoParallelRun(
                parameters,
                monteCarloRuns,
                p =>
                {
                    var m = FundModel.GetModel();
                    m.Run(p);

                    int tempCurrentCount = Interlocked.Increment(ref currentRun);
                    Console.Write("\rRun {0}                ", tempCurrentCount);

                    return 0.0;
                },
                d => 0.0);

            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine(stopwatch.Elapsed);
        }
    }
}
