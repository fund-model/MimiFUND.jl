// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.Collections.Concurrent;
using MPI;

namespace Esmf
{
    public static class ParallelMonteCarlo
    {
        public static R DoParallelRun<RI, R>(Parameters parameters, int runs, Func<ParameterValues, RI> main, Func<IEnumerable<RI>, R> agg)
        {
            // Get best guess parameter values
            var parameterValues = parameters.GetBestGuess();

            var rand = new jp.takel.PseudoRandom.MersenneTwister();

            //int currentRun = 0;

            var results = new ConcurrentBag<RI>();

            Parallel.ForEach(
                parameters.GetRandom(rand, runs),
                () =>
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                    return 0;
                },
                (p, pls, dummy) =>
                {
                    var r = main(p);
                    results.Add(r);
                    return 0;
                },
                (dummy) => { });

            var res = agg(results);

            return res;
        }

        public static R DoMPIRun<RI, R>(Parameters parameters, int runs, Func<ParameterValues, RI> main, Func<IEnumerable<RI>, R> agg)
        {
            Intracommunicator comm = Communicator.world;

            int runsPerProc = runs / comm.Size;

            int startRun = comm.Rank * runsPerProc;

            var rand = new jp.takel.PseudoRandom.MersenneTwister();

            parameters.SkipNRandomRuns(rand, startRun);

            var results = new List<RI>();

            //Console.WriteLine("{0}: Doing {1} runs starting at {2}", comm.Rank, runsPerProc, startRun);

            foreach (var p in parameters.GetRandom(rand, runsPerProc, startRun))
            {
                var r = main(p);
                results.Add(r);
                //Console.WriteLine("{0}: Run {1} done", comm.Rank, p.RunId);
            }

            var allResults = comm.Allgather(results);

            //Console.WriteLine("{0}: Allgather", comm.Rank);

            var allResultsFlat = new List<RI>();

            foreach (var r in allResults)
            {
                foreach (var r2 in r)
                {
                    allResultsFlat.Add(r2);
                }
            }

            var aggResult = agg(allResultsFlat);


            return aggResult;
        }
    }
}
