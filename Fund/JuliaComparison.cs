using Esmf;
using Fund.CommonDimensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fund
{
    public static class JuliaComparison
    {
        public static void Run()
        {
            // Load parameters
            var parameters = new Parameters();
            parameters.ReadDirectory(@"Data\Base");

            // Get best guess parameter values
            var parameterValues = parameters.GetBestGuess();

            // Create a new model that inits itself from the parameters just loaded
            var model1 = FundModel.GetModel();

            // Run the model
            var rs1 = model1.Run(parameterValues);

            var d1 = from p in rs1.GetDimensionalFieldsOperator()
                    where p.Values is IVariableWriter
                    orderby p.ComponentName, p.FieldName
                    select new { ComponentName = p.ComponentName, FieldName = p.FieldName };

            // This will hold results from a BAU run
            Directory.CreateDirectory("JuliaComp1");

            foreach (var i in d1)
            {
                using (var file = new StreamWriter(@"JuliaComp1\" + i.ComponentName + "." + i.FieldName + ".csv"))
                {
                    (rs1[i.ComponentName, i.FieldName] as IVariableWriter).WriteData(file);
                }
            }

            // Create a new model that inits itself from the parameters just loaded
            var model2 = FundModel.GetModel();

            var tax_series = new List<double>();
            tax_series.AddRange(Enumerable.Range(0, 1050).Select(asdf => 25.0));

            model2["emissions"].Parameters["currtax"].SetValue<Timestep, Region, double>((i_t, i_r) => tax_series[i_t.Value]);
            model2["emissions"].Parameters["currtaxch4"].SetValue<Timestep, Region, double>((i_t, i_r) => tax_series[i_t.Value]);
            model2["emissions"].Parameters["currtaxn2o"].SetValue<Timestep, Region, double>((i_t, i_r) => tax_series[i_t.Value]);

            // Run the model
            var rs2 = model2.Run(parameterValues);

            var d2 = from p in rs2.GetDimensionalFieldsOperator()
                     where p.Values is IVariableWriter
                     orderby p.ComponentName, p.FieldName
                     select new { ComponentName = p.ComponentName, FieldName = p.FieldName };

            // This will hold results from a policy run
            Directory.CreateDirectory("JuliaComp2");

            foreach (var i in d2)
            {
                using (var file = new StreamWriter(@"JuliaComp2\" + i.ComponentName + "." + i.FieldName + ".csv"))
                {
                    (rs2[i.ComponentName, i.FieldName] as IVariableWriter).WriteData(file);
                }
            }
        }
    }
}
