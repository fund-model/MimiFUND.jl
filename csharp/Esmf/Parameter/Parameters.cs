// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Esmf
{
    [Serializable]
    public class Parameters : IEnumerable<Parameter>
    {
        private List<Parameter> _parameters = new List<Parameter>();
        private Dictionary<int, Parameter> _parametersById = new Dictionary<int, Parameter>();
        private Dictionary<string, Parameter> _parametersByName = new Dictionary<string, Parameter>();
        private Dictionary<string, int> _parameterIdsByName = new Dictionary<string, int>();

        private int _nextFreeId = 0;

        public void ReadExcelFile(string FileName)
        {
            if (FileName.EndsWith(".xlsm"))
                ReadExcel2007File(FileName);
            else
                throw new Exception("Unknown file format");

        }

        public void SaveBinary(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                var formatter = new BinaryFormatter();

                formatter.Serialize(stream, this);
            }
        }

        public static Parameters ReadBinaryFile(string FileName)
        {
            using (var stream = new FileStream(FileName, FileMode.Open))
            {
                var formatter = new BinaryFormatter();

                return (Parameters)formatter.Deserialize(stream);
            }
        }

        public ParameterValues GetBestGuess()
        {
            return new ParameterValues(this);
        }

        public ParameterValues GetRandom(Random rand)
        {
            return new ParameterValues(this, rand);
        }

        public IEnumerable<ParameterValues> GetRandom(Random rand, int count, int runIdOffset = 0)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new ParameterValues(this, rand, i + runIdOffset + 1);
            }
        }

        public void SkipNRandomRuns(Random rand, int n)
        {
            for (int i = 0; i < n; i++)
            {
                foreach (var p in this)
                {
                    if (p is Esmf.ParameterNonDimensional<double>)
                    {
                        var typedP = (Esmf.ParameterNonDimensional<double>)p;

                        typedP.SkipRandomValues(rand);
                    }
                    else if (p is Esmf.ParameterOneDimensional<double>)
                    {
                        var typedP = (Esmf.ParameterOneDimensional<double>)p;

                        typedP.SkipRandomValues(rand);
                    }
                    else if (p is Esmf.Parameter2Dimensional<double>)
                    {
                        var typedP = (Esmf.Parameter2Dimensional<double>)p;

                        typedP.SkipRandomValues(rand);
                    }
                    else if (p is Esmf.ParameterNonDimensional<Timestep>)
                    {
                        var typedP = (Esmf.ParameterNonDimensional<Timestep>)p;

                        typedP.SkipRandomValues(rand);
                    }
                    else if (p is Esmf.ParameterOneDimensional<string>)
                    {
                        var typedP = (Esmf.ParameterOneDimensional<string>)p;

                        typedP.SkipRandomValues(rand);
                    }
                    else
                        throw new InvalidOperationException();
                }
            }
        }

        private const string c_normalDistributionName = "NormalDistribution";
        private const string c_triangularDistributionName = "TriangularDistribution";
        private const string c_exponentialDistributionName = "ExponentialDistribution";
        private const string c_gammaDistributionName = "GammaDistribution";

        private NonTypedParameterElement GetParameterValueDefinition(ParameterElementKey key, string type, string value)
        {
            double i_fake = 0.0;
            int year = 0;

            string trimmedValue = value.Trim();

            bool canParseAsNumber = Double.TryParse(trimmedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out i_fake);

            if (type != null)
            {
                if (type.ToLowerInvariant() == "s")
                {
                    return new ParameterElementConstant<string>(key, value);
                }
                else
                    throw new NotImplementedException();
            }
            else if (canParseAsNumber)
            {
                return new ParameterElementConstant<double>(key, i_fake);
            }
            else if (trimmedValue.StartsWith(c_normalDistributionName + "(") && trimmedValue.EndsWith(")"))
            {
                string parameterPartOfValue = trimmedValue.Substring(c_normalDistributionName.Length + 1, trimmedValue.Length - (c_normalDistributionName.Length + 2));

                string[] arguments = parameterPartOfValue.Split(';');

                double bestGuess = Convert.ToDouble(arguments[0], CultureInfo.InvariantCulture);
                double standardDeviation = Convert.ToDouble(arguments[1], CultureInfo.InvariantCulture);
                double? lowerBound = null;
                double? upperBound = null;

                if (arguments.Length > 2)
                {
                    if (arguments[2].Trim().Length > 0)
                    {
                        lowerBound = Convert.ToDouble(arguments[2], CultureInfo.InvariantCulture);
                    }

                    if (arguments.Length > 3)
                    {
                        if (arguments[3].Trim().Length > 0)
                        {
                            upperBound = Convert.ToDouble(arguments[3], CultureInfo.InvariantCulture);
                        }
                    }
                }

                return new ParameterElementNormalDistribution(key, bestGuess, standardDeviation, lowerBound, upperBound);
            }
            else if (trimmedValue.StartsWith(c_triangularDistributionName + "(") && trimmedValue.EndsWith(")"))
            {
                string parameterPartOfValue = trimmedValue.Substring(c_triangularDistributionName.Length + 1, trimmedValue.Length - (c_triangularDistributionName.Length + 2));

                string[] arguments = parameterPartOfValue.Split(';');

                double min = Convert.ToDouble(arguments[0], CultureInfo.InvariantCulture);
                double bestGuess = Convert.ToDouble(arguments[1], CultureInfo.InvariantCulture);
                double max = Convert.ToDouble(arguments[2], CultureInfo.InvariantCulture);

                return new ParameterElementTriangularDistribution(key, bestGuess, min, max);

            }
            else if (trimmedValue.StartsWith(c_exponentialDistributionName + "(") && trimmedValue.EndsWith(")"))
            {
                string parameterPartOfValue = trimmedValue.Substring(c_exponentialDistributionName.Length + 1, trimmedValue.Length - (c_exponentialDistributionName.Length + 2));

                string[] arguments = parameterPartOfValue.Split(';');

                double lambda = Convert.ToDouble(arguments[0], CultureInfo.InvariantCulture);
                double? lowerBound = null;
                double? upperBound = null;

                if (arguments.Length > 1)
                {
                    if (arguments[1].Trim().Length > 0)
                    {
                        lowerBound = Convert.ToDouble(arguments[1], CultureInfo.InvariantCulture);
                    }

                    if (arguments.Length > 2)
                    {
                        if (arguments[2].Trim().Length > 0)
                        {
                            upperBound = Convert.ToDouble(arguments[2], CultureInfo.InvariantCulture);
                        }
                    }
                }

                return new ParameterElementExponentialDistribution(key, lambda, lowerBound, upperBound);
            }
            else if (trimmedValue.StartsWith(c_gammaDistributionName + "(") && trimmedValue.EndsWith(")"))
            {
                string parameterPartOfValue = trimmedValue.Substring(c_gammaDistributionName.Length + 1, trimmedValue.Length - (c_gammaDistributionName.Length + 2));

                string[] arguments = parameterPartOfValue.Split(';');

                double alpha = Convert.ToDouble(arguments[0], CultureInfo.InvariantCulture);
                double beta = Convert.ToDouble(arguments[1], CultureInfo.InvariantCulture);
                double? lowerBound = null;
                double? upperBound = null;

                if (arguments.Length > 2)
                {
                    if (arguments[2].Trim().Length > 0)
                    {
                        lowerBound = Convert.ToDouble(arguments[2], CultureInfo.InvariantCulture);
                    }

                    if (arguments.Length > 3)
                    {
                        if (arguments[3].Trim().Length > 0)
                        {
                            upperBound = Convert.ToDouble(arguments[3], CultureInfo.InvariantCulture);
                        }
                    }
                }

                return new ParameterElementGammaDistribution(key, alpha, beta, lowerBound, upperBound);
            }
            else if (trimmedValue.EndsWith("y") && Int32.TryParse(trimmedValue.Substring(0, trimmedValue.Length - 1), out year))
            {
                return new ParameterElementConstant<Timestep>(key, Timestep.FromYear(year));
            }
            else
                throw new ApplicationException(string.Format("Syntax error: '[0}' is not a valid value for {1}.", trimmedValue/*, ParameterKey.Name*/));
        }

        // This implementation uses the new ~ syntax for distributions
        private NonTypedParameterElement GetParameterValueDefinition2(ParameterElementKey key, string value)
        {
            string trimmedValue = value.Trim().ToLowerInvariant();

            double valueasnumber;
            bool canParseAsNumber = Double.TryParse(trimmedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out valueasnumber);

            int year;

            if (trimmedValue.StartsWith("~"))
            {
                int pos_openbracket = trimmedValue.IndexOf('(');
                int pos_closebracket = trimmedValue.IndexOf(')');

                string distributionname = trimmedValue.Substring(1, pos_openbracket - 1);
                string argumentstring = trimmedValue.Substring(pos_openbracket + 1, pos_closebracket - pos_openbracket - 1);
                string[] arguments = argumentstring.Split(';').Select(s => s.Trim()).ToArray();

                string[] positionalargs = arguments.Where(s => !s.Contains('=')).ToArray();

                var namedargs = (from a in arguments
                                 where a.Contains('=')
                                 let splitindex = a.IndexOf('=')
                                 let argname = a.Substring(0, splitindex).ToLowerInvariant()
                                 let argvalue = a.Substring(splitindex + 1)
                                 select new
                                 {
                                     argname,
                                     argvalue
                                 }).ToDictionary(i => i.argname, i => i.argvalue);

                if (distributionname == "n")
                {
                    double bestGuess = Double.Parse(positionalargs[0], CultureInfo.InvariantCulture);
                    double standardDeviation = Double.Parse(positionalargs[1], CultureInfo.InvariantCulture);
                    double? lowerBound = namedargs.ContainsKey("min") ?
                        (double?)Double.Parse(namedargs["min"], CultureInfo.InvariantCulture) :
                        null;
                    double? upperBound = namedargs.ContainsKey("max") ?
                        (double?)Double.Parse(namedargs["max"], CultureInfo.InvariantCulture) :
                        null;

                    return new ParameterElementNormalDistribution(key, bestGuess, standardDeviation, lowerBound, upperBound);
                }
                else if (distributionname == "gamma")
                {
                    double alpha = Double.Parse(positionalargs[0], CultureInfo.InvariantCulture);
                    double beta = Double.Parse(positionalargs[1], CultureInfo.InvariantCulture);
                    double? lowerBound = namedargs.ContainsKey("min") ?
                        (double?)Double.Parse(namedargs["min"], CultureInfo.InvariantCulture) :
                        null;
                    double? upperBound = namedargs.ContainsKey("max") ?
                        (double?)Double.Parse(namedargs["max"], CultureInfo.InvariantCulture) :
                        null;

                    return new ParameterElementGammaDistribution(key, alpha, beta, lowerBound, upperBound);
                }
                else if (distributionname == "exp")
                {
                    double lambda = Double.Parse(positionalargs[0], CultureInfo.InvariantCulture);
                    double? lowerBound = namedargs.ContainsKey("min") ?
                        (double?)Double.Parse(namedargs["min"], CultureInfo.InvariantCulture) :
                        null;
                    double? upperBound = namedargs.ContainsKey("max") ?
                        (double?)Double.Parse(namedargs["max"], CultureInfo.InvariantCulture) :
                        null;

                    return new ParameterElementExponentialDistribution(key, lambda, lowerBound, upperBound);
                }
                else if (distributionname == "triangular")
                {
                    double min = Double.Parse(positionalargs[0], CultureInfo.InvariantCulture);
                    double max = Double.Parse(positionalargs[1], CultureInfo.InvariantCulture);
                    double bestGuess = Double.Parse(positionalargs[2], CultureInfo.InvariantCulture);

                    return new ParameterElementTriangularDistribution(key, bestGuess, min, max);
                }

                else
                    throw new InvalidOperationException("Invalid distribution name");
            }
            else if (canParseAsNumber)
            {
                return new ParameterElementConstant<double>(key, valueasnumber);
            }
            else if (trimmedValue.EndsWith("y") && Int32.TryParse(trimmedValue.Substring(0, trimmedValue.Length - 1), out year))
            {
                return new ParameterElementConstant<Timestep>(key, Timestep.FromYear(year));
            }
            else
            {
                return new ParameterElementConstant<string>(key, value);
            }
        }

        private void ReadExcel2007File(string filename)
        {
            var excelParameters = new Esmf.FundExcelParameterFile(filename);
            excelParameters.Load();

            foreach (Esmf.FundExcelParameterFile.Parameter p in excelParameters.Parameters)
            {
                string parameterName = p.Name.ToLower();
                int parameterId = -1;

                if (!_parameterIdsByName.TryGetValue(parameterName, out parameterId))
                {
                    parameterId = _nextFreeId;
                    _parameterIdsByName.Add(parameterName, parameterId);
                    _nextFreeId++;
                }

                if (p is Esmf.FundExcelParameterFile.NonDimensionalParameter)
                {
                    var typedExcelParameter = (Esmf.FundExcelParameterFile.NonDimensionalParameter)p;

                    var key = new ParameterElementKey(parameterId, parameterName);

                    var parameterValueDef = GetParameterValueDefinition(key, p.Type, typedExcelParameter.Value);

                    if (parameterValueDef.GetElementType() == typeof(double))
                    {
                        var parameter = new ParameterNonDimensional<double>(parameterName, parameterId, (ParameterElement<double>)parameterValueDef);

                        Add(parameter);
                    }
                    else if (parameterValueDef.GetElementType() == typeof(Timestep))
                    {
                        var parameter = new ParameterNonDimensional<Timestep>(parameterName, parameterId, (ParameterElement<Timestep>)parameterValueDef);

                        Add(parameter);
                    }
                    else
                        throw new InvalidOperationException();

                }
                else if (p is Esmf.FundExcelParameterFile.OneDimensionalParameter)
                {
                    var pp = (Esmf.FundExcelParameterFile.OneDimensionalParameter)p;

                    var parameterValueDef0 = GetParameterValueDefinition(null, p.Type, pp[0]);

                    if (parameterValueDef0.GetElementType() == typeof(double))
                    {
                        ParameterElement<double>[] values = new ParameterElement<double>[pp.Length];

                        for (int i = 0; i < pp.Length; i++)
                        {
                            var key = new ParameterElementKey1Dimensional(parameterId, parameterName, i);
                            values[i] = (ParameterElement<double>)GetParameterValueDefinition(key, p.Type, pp[i]);
                        }
                        var parameter = new ParameterOneDimensional<double>(parameterName, parameterId, values);

                        Add(parameter);
                    }
                    else if (parameterValueDef0.GetElementType() == typeof(Timestep))
                    {
                        ParameterElement<Timestep>[] values = new ParameterElement<Timestep>[pp.Length];

                        for (int i = 0; i < pp.Length; i++)
                        {
                            var key = new ParameterElementKey1Dimensional(parameterId, parameterName, i);
                            values[i] = (ParameterElement<Timestep>)GetParameterValueDefinition(key, p.Type, pp[i]);
                        }
                        var parameter = new ParameterOneDimensional<Timestep>(parameterName, parameterId, values);

                        Add(parameter);

                    }
                    else if (parameterValueDef0.GetElementType() == typeof(string))
                    {
                        ParameterElement<string>[] values = new ParameterElement<string>[pp.Length];

                        for (int i = 0; i < pp.Length; i++)
                        {
                            var key = new ParameterElementKey1Dimensional(parameterId, parameterName, i);
                            values[i] = (ParameterElement<string>)GetParameterValueDefinition(key, p.Type, pp[i]);
                        }
                        var parameter = new ParameterOneDimensional<string>(parameterName, parameterId, values);

                        Add(parameter);

                    }
                    else
                        throw new InvalidOperationException();


                }
                else if (p is Esmf.FundExcelParameterFile.TwoDimensionalParameter)
                {
                    var pp = (Esmf.FundExcelParameterFile.TwoDimensionalParameter)p;

                    var parameterValueDef0 = GetParameterValueDefinition(null, p.Type, pp[0, 0]);

                    if (parameterValueDef0.GetElementType() == typeof(double))
                    {
                        ParameterElement<double>[,] values = new ParameterElement<double>[pp.Count0, pp.Count1];

                        for (int i = 0; i < pp.Count0; i++)
                        {
                            for (int l = 0; l < pp.Count1; l++)
                            {
                                var key = new ParameterElementKey2Dimensional(parameterId, parameterName, i, l);
                                values[i, l] = (ParameterElement<double>)GetParameterValueDefinition(key, p.Type, pp[i, l]);
                            }
                        }

                        var parameter = new Parameter2Dimensional<double>(parameterName, parameterId, values);

                        Add(parameter);
                    }
                    else if (parameterValueDef0.GetElementType() == typeof(Timestep))
                    {
                        ParameterElement<Timestep>[,] values = new ParameterElement<Timestep>[pp.Count0, pp.Count1];

                        for (int i = 0; i < pp.Count0; i++)
                        {
                            for (int l = 0; l < pp.Count1; l++)
                            {
                                var key = new ParameterElementKey2Dimensional(parameterId, parameterName, i, l);
                                values[i, l] = (ParameterElement<Timestep>)GetParameterValueDefinition(key, p.Type, pp[i, l]);
                            }
                        }

                        var parameter = new Parameter2Dimensional<Timestep>(parameterName, parameterId, values);

                        Add(parameter);
                    }
                    else
                        throw new NotImplementedException();
                };
            };
        }

        private void Add(Parameter parameter)
        {
            if (_parametersById.ContainsKey(parameter.Id))
            {
                _parameters.Remove(_parametersById[parameter.Id]);
                _parameters.Add(parameter);

                _parametersById[parameter.Id] = parameter;
                _parametersByName[parameter.Name] = parameter;
            }
            else
            {
                _parameters.Add(parameter);
                _parametersById.Add(parameter.Id, parameter);
                _parametersByName.Add(parameter.Name, parameter);
            }
        }

        public void AddNonDimensional<T>(string name, ParameterElement<T> element)
        {
            string parameterName = name.ToLower();
            int parameterId = -1;

            if (!_parameterIdsByName.TryGetValue(parameterName, out parameterId))
            {
                parameterId = _nextFreeId;
                _parameterIdsByName.Add(parameterName, parameterId);
                _nextFreeId++;
            }

            var parameter = new ParameterNonDimensional<T>(parameterName, parameterId, element);

            Add(parameter);

        }

        public void Add1Dimensional(string name, IEnumerable<double> data)
        {
            string parameterName = name.ToLower();
            int parameterId = -1;

            if (!_parameterIdsByName.TryGetValue(parameterName, out parameterId))
            {
                parameterId = _nextFreeId;
                _parameterIdsByName.Add(parameterName, parameterId);
                _nextFreeId++;
            }

            var data2 = new List<ParameterElement<double>>();

            foreach (double v in data)
            {
                data2.Add(new ParameterElementConstant<double>(new ParameterElementKey(parameterId, parameterName), v));
            }

            var parameter = new ParameterOneDimensional<double>(parameterName, parameterId, data2.ToArray());

            Add(parameter);

        }

        public void Add1Dimensional(string name, double[] data)
        {
            Add1Dimensional(name, data as IEnumerable<double>);
        }

        public IEnumerator<Parameter> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        public void SetParameter<T>(string name, T value)
        {
            int parameterId = -1;

            if (!_parameterIdsByName.TryGetValue(name, out parameterId))
            {
                parameterId = _nextFreeId;
                _parameterIdsByName.Add(name, parameterId);
                _nextFreeId++;
            }

            var key = new ParameterElementKey(parameterId, name);

            var parameter = new ParameterNonDimensional<T>(name, parameterId, new ParameterElementConstant<T>(key, value));

            Add(parameter);
        }

        public IEnumerable<NonTypedParameterElement> GetElements()
        {
            foreach (var p in _parameters)
            {
                foreach (var pe in p.GetAllElements())
                {
                    yield return pe;
                }
            }
        }

        public Parameter this[string name]
        {
            get
            {
                return _parametersByName[name];
            }
        }

        public void Save(string directoryname, string comment=null)
        {
            foreach (var p in this)
            {
                p.Save(Path.Combine(directoryname, String.Format("{0}.csv", p.Name)), comment);
            }
        }

        public void ReadDirectory(string directoryname)
        {
            var filenames = Directory.EnumerateFiles(directoryname);

            foreach (var filename in filenames)
            {
                string parameterName = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
                int parameterId = -1;

                if (!_parameterIdsByName.TryGetValue(parameterName, out parameterId))
                {
                    parameterId = _nextFreeId;
                    _parameterIdsByName.Add(parameterName, parameterId);
                    _nextFreeId++;
                }

                var lines = (from line in File.ReadLines(filename)
                             let uncommented = line.Contains('#') ? line.Substring(0, line.IndexOf('#')) : line
                             let trimmed = uncommented.Trim()
                             where trimmed.Length > 0
                             let columns = trimmed.Split(',')
                             select columns).ToArray();

                if (lines.Select(l => l.Length).Distinct().Count() > 1)
                    throw new InvalidOperationException(String.Format("Parameter file {0} has different columns counts in different lines.", filename));

                int columncount = lines.Select(l => l.Length).Distinct().First();

                if (columncount == 1)
                {
                    var key = new ParameterElementKey(parameterId, parameterName);

                    var parameterValueDef = GetParameterValueDefinition2(key, lines[0][0]);

                    if (parameterValueDef.GetElementType() == typeof(double))
                    {
                        var parameter = new ParameterNonDimensional<double>(parameterName, parameterId, (ParameterElement<double>)parameterValueDef);

                        Add(parameter);
                    }
                    else if (parameterValueDef.GetElementType() == typeof(Timestep))
                    {
                        var parameter = new ParameterNonDimensional<Timestep>(parameterName, parameterId, (ParameterElement<Timestep>)parameterValueDef);

                        Add(parameter);
                    }
                    else
                        throw new InvalidOperationException();

                }
                else if (columncount == 2)
                {
                    var values = (from i in Enumerable.Range(0, lines.Length)
                                  let key = new ParameterElementKey1Dimensional(parameterId, parameterName, i)
                                  select GetParameterValueDefinition2(key, lines[i][1])).ToArray();

                    if (values.Select(i => i.GetElementType()).Distinct().Count() != 1)
                        throw new InvalidOperationException("Cannot load 1 dimensional parameter with different data types");

                    Type datatype = values.Select(i => i.GetElementType()).First();

                    if (datatype == typeof(double))
                    {
                        var parameter = new ParameterOneDimensional<double>(parameterName, parameterId, values.Cast<ParameterElement<double>>().ToArray());
                        Add(parameter);
                    }
                    else if (datatype == typeof(string))
                    {
                        var parameter = new ParameterOneDimensional<string>(parameterName, parameterId, values.Cast<ParameterElement<string>>().ToArray());
                        Add(parameter);
                    }
                    else
                        throw new InvalidOperationException();
                }
                else if (columncount == 3)
                {
                    var index0 = lines.Select(i => i[0]).Distinct().ToList();
                    var index1 = lines.Select(i => i[1]).Distinct().ToList();

                    var values = (from i in Enumerable.Range(0, lines.Length)
                                  let key = new ParameterElementKey2Dimensional(parameterId, parameterName, index0.IndexOf(lines[i][0]), index1.IndexOf(lines[i][1]))
                                  select GetParameterValueDefinition2(key, lines[i][2])).ToArray();

                    if (values.Select(i => i.GetElementType()).Distinct().Count() != 1)
                        throw new InvalidOperationException("Cannot load 2 dimensional parameter with different data types");

                    Type datatype = values.Select(i => i.GetElementType()).First();

                    if (datatype == typeof(double))
                    {
                        ParameterElement<double>[,] vals = new ParameterElement<double>[index0.Count, index1.Count];

                        foreach (var v in values)
                        {
                            var key = (ParameterElementKey2Dimensional)v.Key;
                            vals[key.D1, key.D2] = (ParameterElement<double>)v;
                        }

                        for (int i = 0; i < index0.Count; i++)
                        {
                            for (int l = 0; l < index1.Count; l++)
                            {
                                if (vals[i, l] == null)
                                    throw new InvalidOperationException("Missing element in 2 dimensional parameter");
                            }
                        }

                        var parameter = new Parameter2Dimensional<double>(parameterName, parameterId, vals);
                        Add(parameter);
                    }
                    else
                        throw new InvalidOperationException();
                }
                else
                {
                    throw new InvalidOperationException("Cannot read parameter files with more than three columns.");
                }
            };
        }
    }
}
