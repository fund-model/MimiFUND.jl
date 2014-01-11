// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using System.Threading;
using System.Globalization;
using System.IO;

namespace Fund
{
    class FundArguments
    {
#pragma warning disable 0649
        [Argument(ArgumentType.AtMostOnce)]
        public bool Quiet;

        [Argument(ArgumentType.AtMostOnce)]
        public string Diagnostic;

        [Argument(ArgumentType.AtMostOnce)]
        public bool MPI;
#pragma warning restore 0649
    }

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Set the thread to the invariant culture, so that Write and WriteLn
            // format all floating point variables with a . as the decimal separator
            // That way the Windows setting for locals is overriden

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // Set thread priority to below normal, so that other programs can be used
            // while FUND is running
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            Program.ShowVersionInfo();

            var lParsedCmdArgs = new FundArguments();
            if (Parser.ParseArgumentsWithUsage(args, lParsedCmdArgs))
            {
                if (lParsedCmdArgs.MPI)
                {
                    using (var mpiEnv = new MPI.Environment(ref args))
                    {
                        Main2(lParsedCmdArgs);
                    }
                }
                else
                    Main2(lParsedCmdArgs);
            }

        }

        private static void Main2(FundArguments lParsedCmdArgs)
        {
            if (lParsedCmdArgs.Diagnostic != null)
            {
                Console.WriteLine();
                Console.WriteLine("DIAGNOSTIC MODE");
                Console.WriteLine();

                LongtermDiagnosticOutput.Run(lParsedCmdArgs.Diagnostic);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("PLAYGROUND MODE");
                Console.WriteLine();
                var p = new Playground();
                p.Run();
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Finished");

            if (!lParsedCmdArgs.Quiet)
            {
                Console.WriteLine("Press Enter to quit");
                Console.ReadLine();
            }
        }

        static void ShowVersionInfo()
        {
            Console.WriteLine("FUND - Climate Framework for Uncertainty, Negotiation and Distribution");
            Console.WriteLine("Version {0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine();
        }
    }
}
