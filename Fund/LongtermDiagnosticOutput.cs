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

namespace Fund
{
    public class LongtermDiagnosticOutput
    {
        public abstract class DiagnosticOutput
        {
            protected DateTime _date;

            protected DiagnosticOutput(DateTime date)
            {
                _date = date;
            }

            public abstract void WriteDataPoint(string variableName, double data);
        }

        public class FileDiagnosticOutput : DiagnosticOutput
        {
            private StreamWriter _file;
            private bool _consoleOutput;

            public FileDiagnosticOutput(StreamWriter file, DateTime date, bool consoleOutput)
                : base(date)
            {
                _file = file;
                _consoleOutput = consoleOutput;

                _file.WriteLine("\"{0}\";\"{1}\";{2:f15}", "Date", "Variable", "Value");
            }

            public override void WriteDataPoint(string variableName, double data)
            {
                _file.WriteLine("\"{0}\";\"{1}\";{2:f15}", _date, variableName, data);

                if (_consoleOutput)
                    Console.WriteLine("{0,-20} {1,10:f2}", variableName, data);
            }
        }

        public class DatabaseDiagnosticOutput : DiagnosticOutput
        {
            private DbConnection _connection;

            public DatabaseDiagnosticOutput(DbConnection connection, DateTime date)
                : base(date)
            {
                _connection = connection;
            }

            public override void WriteDataPoint(string variableName, double data)
            {
                var cmd = _connection.CreateCommand();
                cmd.CommandText = "INSERT INTO FundLongtermDiagnosticOutput VALUES (@date, @variableName, @data)";

                var p1 = cmd.CreateParameter();
                p1.ParameterName = "date";
                p1.Value = _date;
                cmd.Parameters.Add(p1);

                var p3 = cmd.CreateParameter();
                p3.ParameterName = "variableName";
                p3.Value = variableName;
                cmd.Parameters.Add(p3);

                var p4 = cmd.CreateParameter();
                p4.ParameterName = "data";
                p4.Value = data;
                cmd.Parameters.Add(p4);

                cmd.ExecuteNonQuery();
            }

        }

        public static void Run(DiagnosticOutput diagOut, int level)
        {
            diagOut.WriteDataPoint("SCC-2010-0prtp", GetSCC(0.0, false));
            diagOut.WriteDataPoint("SCC-2010-1prtp", GetSCC(0.01, false));
            diagOut.WriteDataPoint("SCC-2010-3prtp", GetSCC(0.03, false));

            diagOut.WriteDataPoint("SCC-2010-0prtp-AvgEw", GetSCC(0.0, true));
            diagOut.WriteDataPoint("SCC-2010-1prtp-AvgEw", GetSCC(0.01, true));
            diagOut.WriteDataPoint("SCC-2010-3prtp-AvgEw", GetSCC(0.03, true));
        }

        private static double GetSCC(double prtp, bool equityWeights)
        {
            var parameters = new Parameters();
            parameters.ReadExcelFile(@"Data\Parameter - base.xlsm");

            var m = new MarginalDamage3()
                {
                    EmissionYear = Timestep.FromYear(2010),
                    Eta = 1.0,
                    Gas = MarginalGas.C,
                    Parameters = parameters.GetBestGuess(),
                    Prtp = prtp,
                    UseEquityWeights = equityWeights
                };

            double scc = m.Start();

            return scc;
        }
    }
}
