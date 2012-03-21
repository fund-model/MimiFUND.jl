// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Microsoft.Win32;
using System.Windows.Controls.DataVisualization.Charting;

namespace Esmf
{
    /// <summary>
    /// Interaction logic for YearFieldWindow.xaml
    /// </summary>
    public partial class RegionFieldWindow : Window
    {
        private IParameter1DimensionalTypeless<double> _parameter;
        private string _parameterName;

        public RegionFieldWindow(string parameterName, IParameter1DimensionalTypeless<double> parameterToDisplay)
        {
            InitializeComponent();

            _parameter = parameterToDisplay;
            _parameterName = parameterName;

            //plotterHeader.Content = parameterName;
            //var l = new System.Windows.Controls.DataVisualization.Charting.LineSeries();
            var data = from p in parameterToDisplay.GetEnumerator()
                       where !Double.IsNaN(p.Value) && !double.IsInfinity(p.Value)
                       select new Tuple<string, double>(p.Dimension1.ToString(), p.Value);

            chart1.Title = parameterName;

            var mySeries = new ColumnSeries();
            mySeries.IndependentValueBinding = new Binding("Item1");
            mySeries.DependentValueBinding = new Binding("Item2");
            mySeries.ItemsSource = data;
            chart1.Series.Add(mySeries);

            var axis = new LinearAxis();
            axis.Orientation = AxisOrientation.Y;
            axis.Minimum = data.Select(i => i.Item2).Min();
            axis.Maximum = data.Select(i => i.Item2).Max();
            chart1.Axes.Add(axis);

        }


        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV Files (*.csv)|*.csv";
            dlg.FileName = String.Format("{0}.csv", _parameterName);

            bool? result = dlg.ShowDialog(this);

            if (result == true)
            {
                OutputHelper.WriteParameterToFile(dlg.FileName, _parameter);
            }
        }
    }
}
