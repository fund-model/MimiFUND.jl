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

    /// <summary>State for the climate forcing component</summary>
    public interface IClimateForcingState
    {
        /// <summary>Atmospheric CO2 concentration</summary>
        IParameter1Dimensional<Timestep, double> acco2 { get; }

        /// <summary>Pre-industrial atmospheric CO2 concentration</summary>
        double co2pre { get; }

        /// <summary>Atmospheric CH4 concentration</summary>
        IParameter1Dimensional<Timestep, double> acch4 { get; }

        /// <summary>Pre-industrial atmospheric CH4 concentration</summary>
        double ch4pre { get; }

        /// <summary>Indirect radiative forcing increase for CH4</summary>
        double ch4ind { get; }

        /// <summary>Atmospheric N2O concentration</summary>
        IParameter1Dimensional<Timestep, double> acn2o { get; }

        /// <summary>Pre-industrial atmospheric N2O concentration</summary>
        double n2opre { get; }

        /// <summary>Atmospheric SF6 concentrations</summary>
        IParameter1Dimensional<Timestep, double> acsf6 { get; }

        /// <summary>Pre-industrial atmospheric SF6 concentration</summary>
        double sf6pre { get; }

        /// <summary>Radiative forcing from CO2</summary>
        IVariable1Dimensional<Timestep, double> rfCO2 { get; }

        /// <summary>Radiative forcing from CH4</summary>
        IVariable1Dimensional<Timestep, double> rfCH4 { get; }

        /// <summary>Radiative forcing from N2O</summary>
        IVariable1Dimensional<Timestep, double> rfN2O { get; }

        /// <summary>Radiative forcing from N2O</summary>
        IVariable1Dimensional<Timestep, double> rfSF6 { get; }

        /// <summary>Radiative forcing from SO2</summary>
        IParameter1Dimensional<Timestep, double> rfSO2 { get; }

        /// <summary>Radiative forcing</summary>
        IVariable1Dimensional<Timestep, double> radforc { get; }

        /// <summary>EMF22 radiative forcing</summary>
        IVariable1Dimensional<Timestep, double> rfEMF22 { get; }
    }

    public class ClimateForcingComponent
    {

        private double Interact(double M, double N)
        {
            double d;

            d = 1.0 + Math.Pow(M * N, 0.75) * 2.01E-5 + Math.Pow(M * N, 1.52) * M * 5.31E-15;
            return 0.47 * Math.Log(d);
        }

        public void Run(Clock clock, IClimateForcingState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {

            }
            else
            {
                double ch4n2o = Interact(s.ch4pre, s.n2opre);

                s.rfCO2[t] = 5.35 * Math.Log(s.acco2[t] / s.co2pre);

                s.rfCH4[t] = 0.036 * (1.0 + s.ch4ind) * (Math.Sqrt(s.acch4[t]) - Math.Sqrt(s.ch4pre)) - Interact(s.acch4[t], s.n2opre) + ch4n2o;

                s.rfN2O[t] = 0.12 * (Math.Sqrt(s.acn2o[t]) - Math.Sqrt(s.n2opre)) - Interact(s.ch4pre, s.acn2o[t]) + ch4n2o;

                s.rfSF6[t] = 0.00052 * (s.acsf6[t] - s.sf6pre);

                s.radforc[t] = s.rfCO2[t] + s.rfCH4[t] + s.rfN2O[t] + s.rfSF6[t] + s.rfSO2[t];

                s.rfEMF22[t] = s.rfCO2[t] + s.rfCH4[t] + s.rfN2O[t];
            }
        }
    }
}
