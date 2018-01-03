// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Linq.Expressions;
using System.Drawing.Imaging;
using System.Drawing;

namespace Esmf
{
    public class OutputHelper
    {
        public static void ShowModel(ModelOutput modelFields)
        {
            var app = new System.Windows.Application();

            var w = new ModelFieldsWindow(modelFields);

            app.Run(w);
        }

        public static void WriteParameterToFile<T>(TextWriter file, IEnumerable<Parameter1DimensionalMember<T>> parameter)
        {
            string listSeperator = System.Globalization.CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
            foreach (var x in parameter)
            {
                string attachedDimensions = x.AttachedDimensions.Aggregate("", (workingString, next) => workingString + next + listSeperator);
                file.WriteLine("{0}{1}{2}{3}", attachedDimensions, x.Dimension1, listSeperator, x.Value);
            }
            file.Flush();
        }

        public static void WriteParameterToFile<T>(string filename, IEnumerable<Parameter1DimensionalMember<T>> parameter)
        {
            using (var f = File.CreateText(filename))
            {
                OutputHelper.WriteParameterToFile<T>(f, parameter);
            }
        }

        public static void WriteParameterToFile<T>(TextWriter file, IParameter1DimensionalTypeless<T> parameter)
        {
            OutputHelper.WriteParameterToFile<T>(file, parameter.GetEnumerator());
        }

        public static void WriteParameterToFile<D1, T>(TextWriter file, IVariable1Dimensional<D1, T> parameter)
        {
            OutputHelper.WriteParameterToFile(file, parameter as IParameter1Dimensional<D1, T>);
        }

        public static void WriteParameterToFile<D1, T>(string filename, IParameter1Dimensional<D1, T> parameter)
        {
            using (var f = File.CreateText(filename))
            {
                OutputHelper.WriteParameterToFile(f, parameter);
            }
        }

        public static void WriteParameterToFile<D1, T>(string filename, IVariable1Dimensional<D1, T> parameter)
        {
            OutputHelper.WriteParameterToFile(filename, parameter as IParameter1Dimensional<D1, T>);
        }

        public static void WriteParameterToFile<T>(TextWriter file, IEnumerable<Parameter2DimensionalMember<T>> parameter)
        {
            string listSeperator = System.Globalization.CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
            foreach (var x in parameter)
            {
                string attachedDimensions = x.AttachedDimensions.Aggregate("", (workingString, next) => workingString + next + listSeperator);
                file.WriteLine("{0}{1}{2}{3}{4}{5}", attachedDimensions, x.Dimension1, listSeperator, x.Dimension2, listSeperator, x.Value);
            }
            file.Flush();
        }

        public static void WriteParametersToFile<D1, D2, T>(string filename, params IEnumerable<Parameter2DimensionalMember<T>>[] parameters)
        {
            using (var f = File.CreateText(filename))
            {
                OutputHelper.WriteParametersToFile<D1, D2, T>(f, parameters);
            }

        }

        public static void WriteParametersToFile<D1, D2, T>(TextWriter file, params IEnumerable<Parameter2DimensionalMember<T>>[] parameters)
        {
            string listSeperator = System.Globalization.CultureInfo.CurrentUICulture.TextInfo.ListSeparator;

            var keys = new HashSet<Tuple<string, D1, D2>>();

            var parametersAsDict = new List<Dictionary<Tuple<string, D1, D2>, Parameter2DimensionalMember<T>>>();

            foreach (var p in parameters)
            {
                var lkeys = from i in p
                            select Tuple.Create(i.AttachedDimensions.Aggregate("", (workingString, next) => workingString + next + listSeperator), (D1)i.Dimension1, (D2)i.Dimension2);
                foreach (var key in lkeys)
                    keys.Add(key);

                var currentDict = new Dictionary<Tuple<string, D1, D2>, Parameter2DimensionalMember<T>>();
                parametersAsDict.Add(currentDict);
                foreach (var x in p)
                    currentDict.Add(Tuple.Create(x.AttachedDimensions.Aggregate("", (workingString, next) => workingString + next + listSeperator), (D1)x.Dimension1, (D2)x.Dimension2), x);
            }
            Console.WriteLine("We have {0} keys with {1} parameters", keys.Count, parameters.Length);

            foreach (var key in keys)
            {

                file.Write("{0}{1}{2}{3}", key.Item1, key.Item2, listSeperator, key.Item3);
                foreach (var dict in parametersAsDict)
                {
                    Parameter2DimensionalMember<T> val;
                    if (dict.TryGetValue(key, out val))
                    {
                        file.Write("{0}{1}", listSeperator, val.Value);
                    }
                }
                file.WriteLine();
            }
            file.Flush();

        }

        public static void WriteParametersToFile<D1, D2, T>(string filename, params IParameter2Dimensional<D1, D2, T>[] parameters)
        {
            var d = parameters.Select(i => i.GetEnumerator()).ToArray();
            OutputHelper.WriteParametersToFile<D1, D2, T>(filename, d);
        }

        public static void WriteParametersToFile<D1, D2, T>(string filename, params IVariable2Dimensional<D1, D2, T>[] parameters)
        {
            var d = parameters.Cast<IParameter2Dimensional<D1, D2, T>>().Select(i => i.GetEnumerator()).ToArray();
            OutputHelper.WriteParametersToFile<D1, D2, T>(filename, d);
        }


        public static void WriteParameterToFile<T>(string filename, IEnumerable<Parameter2DimensionalMember<T>> parameter)
        {
            using (var f = File.CreateText(filename))
            {
                OutputHelper.WriteParameterToFile<T>(f, parameter);
            }
        }

        public static void WriteParameterToFile<T>(TextWriter file, IParameter2DimensionalTypeless<T> parameter)
        {
            OutputHelper.WriteParameterToFile<T>(file, parameter.GetEnumerator());
        }

        public static void WriteParameterToFile<T>(string filename, IParameter2DimensionalTypeless<T> parameter)
        {
            using (var f = File.CreateText(filename))
            {
                OutputHelper.WriteParameterToFile<T>(f, parameter);
            }
        }

        public static void WriteParameterToFile<T>(string filename, IParameter1DimensionalTypeless<T> parameter)
        {
            using (var f = File.CreateText(filename))
            {
                OutputHelper.WriteParameterToFile<T>(f, parameter);
            }
        }

        public static void WriteParameterToFile<D1, D2, T>(string filename, IVariable2Dimensional<D1, D2, T> parameter)
        {
            OutputHelper.WriteParameterToFile<T>(filename, parameter as IParameter2DimensionalTypeless<T>);
        }

        public static void WriteParameterToFile<D1, D2, T>(TextWriter file, IVariable2Dimensional<D1, D2, T> parameter)
        {
            OutputHelper.WriteParameterToFile<T>(file, parameter as IParameter2DimensionalTypeless<T>);
        }
    }
}
