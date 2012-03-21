// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmf;
using Fund.CommonDimensions;


namespace Fund.Components.Welfare
{

    public interface IGlobalWelfareState
    {
        Double prtp { get; }
        Double elasticityofmarginalutility { get; }

        [DefaultParameterValue(0.0)]
        Double utilitycalibrationadditive { get; }

        [DefaultParameterValue(1.0)]
        Double utilitycalibrationmultiplicative { get; }

        Timestep starttimestep { get; }
        Timestep stoptimestep { get; }

        IParameter1Dimensional<Timestep, double> population { get; }
        IParameter1Dimensional<Timestep, double> consumption { get; }

        IVariable1Dimensional<Timestep, double> cummulativewelfare { get; }
        IVariable1Dimensional<Timestep, double> marginalwelfare { get; }

        Double totalwelfare { get; set; }
    }

    public interface IUtilitarianWelfareState
    {
        Double prtp { get; }
        Double elasticityofmarginalutility { get; }

        [DefaultParameterValue(1.0)]
        Double utilitycalibrationadditive { get; }

        [DefaultParameterValue(1.0)]
        Double utilitycalibrationmultiplicative { get; }

        Timestep starttimestep { get; }
        Timestep stoptimestep { get; }

        IParameter2Dimensional<Timestep, Region, double> population { get; }
        IParameter2Dimensional<Timestep, Region, double> consumption { get; }
        IParameter2Dimensional<Timestep, Region, double> welfareweight { get; }

        IVariable1Dimensional<Timestep, double> cummulativewelfare { get; }
        IVariable2Dimensional<Timestep, Region, double> marginalwelfare { get; }

        Double totalwelfare { get; set; }
    }

    public interface IRegionalWelfareState
    {
        Double prtp { get; }
        Double elasticityofmarginalutility { get; }

        [DefaultParameterValue(1.0)]
        Double utilitycalibrationadditive { get; }

        [DefaultParameterValue(1.0)]
        Double utilitycalibrationmultiplicative { get; }

        Timestep starttimestep { get; }
        Timestep stoptimestep { get; }

        IParameter2Dimensional<Timestep, Region, double> population { get; }
        IParameter2Dimensional<Timestep, Region, double> consumption { get; }

        IVariable2Dimensional<Timestep, Region, double> cummulativewelfare { get; }
        IVariable2Dimensional<Timestep, Region, double> marginalwelfare { get; }

        IVariable1Dimensional<Region, double> totalwelfare { get; }
    }



    public class GlobalWelfareComponent
    {
        public void Run(Clock clock, IGlobalWelfareState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                s.cummulativewelfare[t] = 0;
            }
            else
            {
                if (t >= s.starttimestep)
                {

                    var U = Funcifier.Funcify(
                        (double consumption) =>
                        {
                            if (s.elasticityofmarginalutility == 1.0)
                                return s.utilitycalibrationadditive + s.utilitycalibrationmultiplicative * Math.Log(consumption);
                            else
                                return s.utilitycalibrationadditive + s.utilitycalibrationmultiplicative * Math.Pow(consumption, 1.0 - s.elasticityofmarginalutility) / (1.0 - s.elasticityofmarginalutility);
                        }
                        );

                    var DF = Funcifier.Funcify(
                        (Timestep year) =>
                        {
                            return Math.Pow(1.0 + s.prtp, -(year.Value - s.starttimestep.Value));
                        }
                        );

                    var perCapitaConsumption = s.consumption[t] / s.population[t];

                    if (perCapitaConsumption <= 0.0)
                        perCapitaConsumption = 1.0;

                    s.cummulativewelfare[t] = s.cummulativewelfare[t - 1] + (U(perCapitaConsumption) * s.population[t] * DF(t));
                    s.marginalwelfare[t] = DF(t) * s.utilitycalibrationmultiplicative / Math.Pow(perCapitaConsumption, s.elasticityofmarginalutility);

                    if (t == s.stoptimestep)
                    {
                        s.totalwelfare = s.cummulativewelfare[t];
                    }
                }
                else
                    s.cummulativewelfare[t] = 0;
            }
        }
    }

    public class UtilitarianWelfareComponent
    {
        public void Run(Clock clock, IUtilitarianWelfareState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                s.cummulativewelfare[t] = 0;
            }
            else
            {
                if (t >= s.starttimestep)
                {
                    var w = s.cummulativewelfare[t - 1];

                    var U = Funcifier.Funcify(
                        (double consumption) =>
                        {
                            if (s.elasticityofmarginalutility == 1.0)
                                return s.utilitycalibrationadditive + s.utilitycalibrationmultiplicative * Math.Log(consumption);
                            else
                                return s.utilitycalibrationadditive + s.utilitycalibrationmultiplicative * Math.Pow(consumption, 1.0 - s.elasticityofmarginalutility) / (1.0 - s.elasticityofmarginalutility);

                        }
                        );

                    var DF = Funcifier.Funcify(
                        (Timestep year) =>
                        {
                            return Math.Pow(1.0 + s.prtp, -(year.Value - s.starttimestep.Value));
                        }
                      );

                    foreach (var r in dimensions.GetValues<Region>())
                    {
                        var perCapitaConsumption = s.consumption[t, r] / s.population[t, r];

                        // This is a lower bound
                        if (perCapitaConsumption <= 0.0)
                            perCapitaConsumption = 1.0;

                        w = w + (s.welfareweight[t, r] * U(perCapitaConsumption) * s.population[t, r] * DF(t));
                        s.marginalwelfare[t, r] = DF(t) * s.welfareweight[t, r] * s.utilitycalibrationmultiplicative / Math.Pow(perCapitaConsumption, s.elasticityofmarginalutility);
                    }
                    s.cummulativewelfare[t] = w;

                    if (t == s.stoptimestep)
                        s.totalwelfare = s.cummulativewelfare[t];
                }
                else
                    s.cummulativewelfare[t] = 0;
            }
        }
    }

    public class RegionalWelfareComponent
    {
        public void Run(Clock clock, IRegionalWelfareState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.cummulativewelfare[t, r] = 0;
                }
            }
            else
            {
                if (t >= s.starttimestep)
                {

                    var U = Funcifier.Funcify(
                        (double consumption) =>
                        {
                            if (s.elasticityofmarginalutility == 1.0)
                                return s.utilitycalibrationadditive + s.utilitycalibrationmultiplicative * Math.Log(consumption);
                            else
                                return s.utilitycalibrationadditive + s.utilitycalibrationmultiplicative * Math.Pow(consumption, 1.0 - s.elasticityofmarginalutility) / (1.0 - s.elasticityofmarginalutility);

                        }
                        );

                    var DF = Funcifier.Funcify(
                        (Timestep year) =>
                        {
                            return Math.Pow(1.0 + s.prtp, -(year.Value - s.starttimestep.Value));
                        }
                      );

                    foreach (var r in dimensions.GetValues<Region>())
                    {
                        var w = s.cummulativewelfare[t - 1, r];

                        var perCapitaConsumption = s.consumption[t, r] / s.population[t, r];

                        // This is a lower bound
                        if (perCapitaConsumption <= 0.0)
                            perCapitaConsumption = 1.0;

                        w = w + (U(perCapitaConsumption) * s.population[t, r] * DF(t));
                        s.marginalwelfare[t, r] = DF(t) * s.utilitycalibrationmultiplicative / Math.Pow(perCapitaConsumption, s.elasticityofmarginalutility);

                        s.cummulativewelfare[t, r] = w;

                        if (t == s.stoptimestep)
                            s.totalwelfare[r] = s.cummulativewelfare[t, r];

                    }


                }
                else
                {
                    foreach (var r in dimensions.GetValues<Region>())
                    {
                        s.cummulativewelfare[t, r] = 0;
                    }
                }
            }
        }
    }

}
