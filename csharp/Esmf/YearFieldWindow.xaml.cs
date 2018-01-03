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

namespace Esmf
{
    /// <summary>
    /// Interaction logic for YearFieldWindow.xaml
    /// </summary>
    public partial class YearFieldWindow : Window
    {
        private IParameter1Dimensional<Timestep, double> _parameter;
        private string _parameterName;

        public YearFieldWindow(string parameterName, IParameter1Dimensional<Timestep, double> parameterToDisplay)
        {
            InitializeComponent();

            _parameter = parameterToDisplay;
            _parameterName = parameterName;

            plotterHeader.Content = parameterName;
            //var l = new System.Windows.Controls.DataVisualization.Charting.LineSeries();
            var data = from p in parameterToDisplay.GetEnumerator()
                       where !Double.IsNaN(p.Value) && !double.IsInfinity(p.Value)
                       select new Point(p.Dimension1.Index + 1950, p.Value);


            var d = data.AsDataSource();

            plotter.AddLineGraph(d);
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
