// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using Esmf;
using System;
using System.Linq;
using Fund.CommonDimensions;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections;
using Esmf.Model;

namespace Fund
{

    public class FundWorkflow : ComposedComponent
    {
        [ModelState(typeof(Fund.Components.ScenarioUncertainty.ScenarioUncertaintyComponent))]
        public Fund.Components.ScenarioUncertainty.IScenarioUncertaintyState ScenarioUncertainty;

        [ModelState(typeof(Fund.Components.Geography.GeographyComponent))]
        [Binding("landloss", "impactsealevelrise")]
        public Fund.Components.Geography.IGeographyState Geography;

        [ModelState(typeof(Fund.Components.Population.PopulationComponent))]
        [Binding("pgrowth", "scenariouncertainty")]
        [Binding("leave", "impactsealevelrise")]
        [Binding("enter", "impactsealevelrise")]
        [Binding("dead", "impactdeathmorbidity")]
        public Fund.Components.Population.IPopulationState Population;

        [ModelState(typeof(Fund.Components.SocioEconomic.SocioEconomicComponent))]
        [Binding("ypcgrowth", "scenariouncertainty")]
        [Binding("pgrowth", "scenariouncertainty")]
        [Binding("eloss", "impactaggregation")]
        [Binding("sloss", "impactaggregation")]
        [Binding("mitigationcost", "emissions")]
        [Binding("area", "geography")]
        [Binding("globalpopulation", "population")]
        [Binding("population", "population")]
        [Binding("populationin1", "population")]
        public Fund.Components.SocioEconomic.ISocioEconomicState SocioEconomic;

        [ModelState(typeof(Fund.Components.Emissions.EmissionsComponent))]
        [Binding("aeei", "scenariouncertainty")]
        [Binding("acei", "scenariouncertainty")]
        [Binding("forestemm", "scenariouncertainty")]
        [Binding("pgrowth", "scenariouncertainty")]
        [Binding("ypcgrowth", "scenariouncertainty")]
        [Binding("income", "socioeconomic")]
        [Binding("population", "socioeconomic")]
        public Fund.Components.Emissions.IEmissionsState Emissions;

        [ModelState(typeof(Fund.Components.ClimateCO2Cycle.ClimateCO2CycleComponent))]
        [Binding("mco2", "emissions")]
        [Binding("temp", "climatedynamics")]
        public Fund.Components.ClimateCO2Cycle.IClimateCO2CycleState ClimateCO2Cycle;

        [ModelState(typeof(Fund.Components.ClimateCH4Cycle.ClimateCH4CycleComponent))]
        [Binding("globch4", "emissions")]
        public Fund.Components.ClimateCH4Cycle.IClimateCH4CycleState ClimateCH4Cycle;

        [ModelState(typeof(Fund.Components.ClimateN2OCycle.ClimateN2OCycleComponent))]
        [Binding("globn2o", "emissions")]
        public Fund.Components.ClimateN2OCycle.IClimateN2OCycleState ClimateN2OCycle;

        [ModelState(typeof(Fund.Components.ClimateSF6Cycle.ClimateSF6CycleComponent))]
        [Binding("globsf6", "emissions")]
        public Fund.Components.ClimateSF6Cycle.IClimateSF6CycleState ClimateSF6Cycle;

        [ModelState(typeof(Fund.Components.ClimateSO2Cycle.ClimateSO2CycleComponent))]
        [Binding("globso2", "emissions")]
        public Fund.Components.ClimateSO2Cycle.IClimateSO2CycleState ClimateSO2Cycle;

        [ModelState(typeof(Fund.Components.ClimateForcing.ClimateForcingComponent))]
        [Binding("acco2", "climateco2cycle")]
        [Binding("acch4", "climatech4cycle")]
        [Binding("acn2o", "climaten2ocycle")]
        [Binding("acsf6", "climatesf6cycle")]
        [Binding("acso2", "climateso2cycle")]
        public Fund.Components.ClimateForcing.IClimateForcingState ClimateForcing;

        [ModelState(typeof(Fund.Components.ClimateDynamics.ClimateDynamicsComponent))]
        [Binding("radforc", "climateforcing")]
        public Fund.Components.ClimateDynamics.IClimateDynamicsState ClimateDynamics;

        [ModelState(typeof(Fund.Components.ClimateRegional.ClimateRegionalComponent))]
        [Binding("inputtemp", "climatedynamics", "temp")]
        public Fund.Components.ClimateRegional.IClimateRegionalState ClimateRegional;

        [ModelState(typeof(Fund.Components.Ocean.OceanComponent))]
        [Binding("temp", "climatedynamics")]
        public Fund.Components.Ocean.IOceanState Ocean;

        [ModelState(typeof(Fund.Components.BioDiversity.BioDiversityComponent))]
        [Binding("temp", "climatedynamics")]
        public Fund.Components.BioDiversity.IBioDiversityState BioDiversity;

        [ModelState(typeof(Fund.Components.ImpactBioDiversity.ImpactBioDiversityComponent))]
        [Binding("temp", "climateregional")]
        [Binding("nospecies", "biodiversity")]
        [Binding("income", "socioeconomic")]
        [Binding("population", "socioeconomic")]
        public Fund.Components.ImpactBioDiversity.IImpactBioDiversityState ImpactBioDiversity;

        [ModelState(typeof(Fund.Components.ImpactHeating.ImpactHeatingComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("temp", "climateregional")]
        [Binding("cumaeei", "emissions")]
        public Fund.Components.ImpactHeating.IImpactHeatingState ImpactHeating;

        [ModelState(typeof(Fund.Components.ImpactCooling.ImpactCoolingComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("temp", "climateregional")]
        [Binding("cumaeei", "emissions")]
        public Fund.Components.ImpactCooling.IImpactCoolingState ImpactCooling;

        [ModelState(typeof(Fund.Components.ImpactAgriculture.ImpactAgricultureComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("temp", "climateregional")]
        [Binding("acco2", "climateco2cycle")]
        public Fund.Components.ImpactAgriculture.IImpactAgricultureState ImpactAgriculture;

        [ModelState(typeof(Fund.Components.ImpactWaterResources.ImpactWaterResourcesComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("temp", "climateregional")]
        public Fund.Components.ImpactWaterResources.IImpactWaterResourcesState ImpactWaterResources;

        [ModelState(typeof(Fund.Components.ImpactDiarrhoea.ImpactDiarrhoeaComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("regtmp", "climateregional")]
        public Fund.Components.ImpactDiarrhoea.IImpactDiarrhoeaState ImpactDiarrhoea;

        [ModelState(typeof(Fund.Components.ImpactTropicalStorms.ImpactTropicalStormsComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("regstmp", "climateregional")]
        public Fund.Components.ImpactTropicalStorms.IImpactTropicalStormsState ImpactTropicalStorms;

        [ModelState(typeof(Fund.Components.ImpactExtratropicalStorms.ImpactExtratropicalStormsComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("acco2", "climateco2cycle")]
        public Fund.Components.ImpactExtratropicalStorms.IImpactExtratropicalStormsState ImpactExtratropicalStorms;

        [ModelState(typeof(Fund.Components.ImpactSeaLevelRise.ImpactSeaLevelRiseComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("sea", "ocean")]
        [Binding("area", "geography")]
        public Fund.Components.ImpactSeaLevelRise.IImpactSeaLevelRiseState ImpactSeaLevelRise;

        [ModelState(typeof(Fund.Components.ImpactForests.ImpactForests))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("temp", "climateregional")]
        [Binding("acco2", "climateco2cycle")]
        public Fund.Components.ImpactForests.IImpactForestsState ImpactForests;

        [ModelState(typeof(Fund.Components.ImpactVectorBorneDiseases.ImpactVectorBorneDiseasesComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("temp", "climateregional")]
        public Fund.Components.ImpactVectorBorneDiseases.IImpactVectorBorneDiseasesState ImpactVectorBorneDiseases;

        [ModelState(typeof(Fund.Components.ImpactCardiovascularRespiratory.ImpactCardiovascularRespiratoryComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("temp", "climateregional")]
        [Binding("plus", "socioeconomic")]
        [Binding("urbpop", "socioeconomic")]
        public Fund.Components.ImpactCardiovascularRespiratory.IImpactCardiovascularRespiratoryState ImpactCardiovascularRespiratory;

        [ModelState(typeof(Fund.Components.ImpactDeathMorbidity.ImpactDeathMorbidityComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("dengue", "impactvectorbornediseases")]
        [Binding("schisto", "impactvectorbornediseases")]
        [Binding("malaria", "impactvectorbornediseases")]
        [Binding("cardheat", "impactcardiovascularrespiratory")]
        [Binding("cardcold", "impactcardiovascularrespiratory")]
        [Binding("resp", "impactcardiovascularrespiratory")]
        [Binding("diadead", "impactdiarrhoea")]
        [Binding("hurrdead", "impacttropicalstorms")]
        [Binding("extratropicalstormsdead", "impactextratropicalstorms")]
        [Binding("diasick", "impactdiarrhoea")]
        public Fund.Components.ImpactDeathMorbidity.IImpactDeathMorbidityState ImpactDeathMorbidity;

        [ModelState(typeof(Fund.Components.ImpactAggregation.ImpactAggregationComponent))]
        [Binding("income", "socioeconomic")]
        [Binding("heating", "impactheating")]
        [Binding("cooling", "impactcooling")]
        [Binding("agcost", "impactagriculture")]
        [Binding("species", "impactbiodiversity")]
        [Binding("water", "impactwaterresources")]
        [Binding("hurrdam", "impacttropicalstorms")]
        [Binding("extratropicalstormsdam", "impactextratropicalstorms")]
        [Binding("forests", "impactforests")]
        [Binding("drycost", "impactsealevelrise")]
        [Binding("protcost", "impactsealevelrise")]
        [Binding("entercost", "impactsealevelrise")]
        [Binding("deadcost", "impactdeathmorbidity")]
        [Binding("morbcost", "impactdeathmorbidity")]
        [Binding("wetcost", "impactsealevelrise")]
        [Binding("leavecost", "impactsealevelrise")]
        public Fund.Components.ImpactAggregation.IImpactAggregationState ImpactAggregation;
    }
}