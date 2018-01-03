// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmf;

namespace Fund.Components
{

    /// <summary>State for the climate component</summary>
    public interface IClimateCO2CycleState
    {
        /// <summary>Anthropogenic CO2 emissions in Mt of C</summary>
        IParameter1Dimensional<Timestep, double> mco2 { get; }

        /// <summary>Terrestrial biosphere CO2 emissions in Mt of C</summary>
        IVariable1Dimensional<Timestep, double> TerrestrialCO2 { get; }

        /// <summary>Net CO2 emissions in Mt of C</summary>
        IVariable1Dimensional<Timestep, double> globc { get; }

        /// <summary>Carbon box 1</summary>
        IVariable1Dimensional<Timestep, double> cbox1 { get; }

        /// <summary>Carbon box 2</summary>
        IVariable1Dimensional<Timestep, double> cbox2 { get; }

        /// <summary>Carbon box 3</summary>
        IVariable1Dimensional<Timestep, double> cbox3 { get; }

        /// <summary>Carbon box 4</summary>
        IVariable1Dimensional<Timestep, double> cbox4 { get; }

        /// <summary>Carbon box 5</summary>
        IVariable1Dimensional<Timestep, double> cbox5 { get; }

        /// <summary>Initial carbon box 1</summary>
        double cbox10 { get; }

        /// <summary>Initial carbon box 2</summary>
        double cbox20 { get; }

        /// <summary>Initial carbon box 3</summary>
        double cbox30 { get; }

        /// <summary>Initial carbon box 4</summary>
        double cbox40 { get; }

        /// <summary>Initial carbon box 5</summary>
        double cbox50 { get; }

        /// <summary>Carbon decay in box 1</summary>
        double co2decay1 { get; set; }

        /// <summary>Carbon decay in box 2</summary>
        double co2decay2 { get; set; }

        /// <summary>Carbon decay in box 3</summary>
        double co2decay3 { get; set; }

        /// <summary>Carbon decay in box 4</summary>
        double co2decay4 { get; set; }

        /// <summary>Carbon decay in box 5</summary>
        double co2decay5 { get; set; }

        /// <summary>Carbon decay in box 1</summary>
        double lifeco1 { get; }

        /// <summary>Carbon decay in box 2</summary>
        double lifeco2 { get; }

        /// <summary>Carbon decay in box 3</summary>
        double lifeco3 { get; }

        /// <summary>Carbon decay in box 4</summary>
        double lifeco4 { get; }

        /// <summary>Carbon decay in box 5</summary>
        double lifeco5 { get; }

        /// <summary>Fraction of carbon emission in box 1</summary>  
        double co2frac1 { get; }

        /// <summary>Fraction of carbon emission in box 2</summary>  
        double co2frac2 { get; }

        /// <summary>Fraction of carbon emission in box 3</summary>  
        double co2frac3 { get; }

        /// <summary>Fraction of carbon emission in box 4</summary>  
        double co2frac4 { get; }

        /// <summary>Fraction of carbon emission in box 5</summary>  
        double co2frac5 { get; }

        /// <summary>Atmospheric CO2 concentration</summary>
        IVariable1Dimensional<Timestep, double> acco2 { get; }

        /// <summary>Stock of CO2 in the terrestrial biosphere</summary>
        IVariable1Dimensional<Timestep, double> TerrCO2Stock { get; }

        /// <summary>Temperature</summary>
        IParameter1Dimensional<Timestep, double> temp { get; }

        double TerrCO2Sens { get; }
        double TerrCO2Stock0 { get; }

        double tempIn2010 { get; set; }

    }

    public class ClimateCO2CycleComponent
    {


        public void Run(Clock clock, IClimateCO2CycleState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                s.co2decay1 = s.lifeco1;
                s.co2decay2 = Math.Exp(-1.0 / s.lifeco2);
                s.co2decay3 = Math.Exp(-1.0 / s.lifeco3);
                s.co2decay4 = Math.Exp(-1.0 / s.lifeco4);
                s.co2decay5 = Math.Exp(-1.0 / s.lifeco5);

                s.TerrCO2Stock[t] = s.TerrCO2Stock0;

                s.cbox1[t] = s.cbox10;
                s.cbox2[t] = s.cbox20;
                s.cbox3[t] = s.cbox30;
                s.cbox4[t] = s.cbox40;
                s.cbox5[t] = s.cbox50;
                s.acco2[t] = s.cbox1[t] + s.cbox2[t] + s.cbox3[t] + s.cbox4[t] + s.cbox5[t];
            }
            else
            {
                if (t == Timestep.FromYear(2011))
                {
                    s.tempIn2010 = s.temp[Timestep.FromYear(2010)];
                }

                if (t > Timestep.FromYear(2010))
                {

                    s.TerrestrialCO2[t] = (s.temp[t - 1] - s.tempIn2010) * s.TerrCO2Sens * s.TerrCO2Stock[t - 1] / s.TerrCO2Stock0;
                }
                else
                    s.TerrestrialCO2[t] = 0;

                s.TerrCO2Stock[t] = Math.Max(s.TerrCO2Stock[t - 1] - s.TerrestrialCO2[t], 0.0);

                s.globc[t] = s.mco2[t] + s.TerrestrialCO2[t];

                // Calculate CO2 concentrations
                s.cbox1[t] = s.cbox1[t - 1] * s.co2decay1 + 0.000471 * s.co2frac1 * (s.globc[t]);
                s.cbox2[t] = s.cbox2[t - 1] * s.co2decay2 + 0.000471 * s.co2frac2 * (s.globc[t]);
                s.cbox3[t] = s.cbox3[t - 1] * s.co2decay3 + 0.000471 * s.co2frac3 * (s.globc[t]);
                s.cbox4[t] = s.cbox4[t - 1] * s.co2decay4 + 0.000471 * s.co2frac4 * (s.globc[t]);
                s.cbox5[t] = s.cbox5[t - 1] * s.co2decay5 + 0.000471 * s.co2frac5 * (s.globc[t]);

                s.acco2[t] = s.cbox1[t] + s.cbox2[t] + s.cbox3[t] + s.cbox4[t] + s.cbox5[t];
            }
        }
    }
}
