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
using System.Collections.ObjectModel;

namespace Esmf
{
    /// <summary>
    /// Interaction logic for ModelFieldsWindow.xaml
    /// </summary>
    public partial class ModelFieldsWindow : Window
    {
        public ObservableCollection<ModelOutput.Field> Fields { get; set; }

        public ModelFieldsWindow(ModelOutput fields)
        {
            Fields = new ObservableCollection<ModelOutput.Field>();
            InitializeComponent();

            var d = from p in fields.GetDimensionalFieldsOperator()
                    where (p.Values is IParameter1Dimensional<Timestep, double>) || (p.Values is IParameter1DimensionalTypeless<double>) || (p.Values is IParameter2DimensionalTypeless<double>)
                    orderby p.ComponentName, p.FieldName
                    select p;

            foreach (var i in d)
                Fields.Add(i);

            this.DataContext = Fields;
        }

        private void yearFieldsListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (yearFieldsListBox.SelectedIndex >= 0)
            {
                var selected = Fields[yearFieldsListBox.SelectedIndex];
                if (selected.Values is IParameter1Dimensional<Timestep, double>)
                {
                    var w = new YearFieldWindow(String.Format("{0}.{1}", selected.ComponentName, selected.FieldName), (IParameter1Dimensional<Timestep, double>)selected.Values);

                    w.Show();
                }
                else if (selected.Values is IParameter1DimensionalTypeless<double>)
                {
                    var w = new RegionFieldWindow(String.Format("{0}.{1}", selected.ComponentName, selected.FieldName), (IParameter1DimensionalTypeless<double>)selected.Values);

                    w.Show();

                }
                else if (selected.Values is IParameter2DimensionalTypeless<double>)
                {
                    var w = new YearRegionFieldWindow(String.Format("{0}.{1}", selected.ComponentName, selected.FieldName), (IParameter2DimensionalTypeless<double>)selected.Values);

                    w.Show();
                }
            }
        }
    }
}
