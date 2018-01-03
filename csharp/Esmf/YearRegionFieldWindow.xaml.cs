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
    /// Interaction logic for YearRegionFieldWindow.xaml
    /// </summary>
    public partial class YearRegionFieldWindow : Window
    {
        private IParameter2DimensionalTypeless<double> _parameter;
        private string _parameterName;

        public YearRegionFieldWindow(string parameterName, IParameter2DimensionalTypeless<double> parameterToDisplay)
        {
            InitializeComponent();

            _parameter = parameterToDisplay;
            _parameterName = parameterName;

            plotterHeader.Content = parameterName;

            var data2 = from p in parameterToDisplay.GetEnumerator()
                        where !Double.IsNaN(p.Value) && !Double.IsInfinity(p.Value)
                        group p by p.Dimension2;

            foreach (var series in data2)
            {
                var data = from p in series
                           select new Point(p.Dimension1.Index + 1950, p.Value);

                var dd = data.ToArray();
                var d = dd.AsDataSource();

                int remainder1 = series.Key.Index % 4;
                var color = remainder1 == 0 ? Brushes.Blue : remainder1 == 1 ? Brushes.Red : remainder1 == 2 ? Brushes.Green : Brushes.Gold;

                int tt = series.Key.Index / 4;
                var dashline = tt == 0 ? DashStyles.Solid : tt == 1 ? DashStyles.Dash : tt == 2 ? DashStyles.Dot : DashStyles.DashDot;
                var linedescr = tt == 0 ? "solid" : tt == 1 ? "dash" : tt == 2 ? "dot" : "dash-dot";

                var pen = new Pen(color, 2);
                pen.DashStyle = dashline;
                plotter.AddLineGraph(d, pen, new PenDescription(String.Format("{0} ({1})", series.Key, linedescr)));
            }
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
