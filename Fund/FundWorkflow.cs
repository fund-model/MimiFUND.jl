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
using Fund.Components;

namespace Fund
{

    public class FundWorkflow : ComposedComponent
    {
        [ModelState(typeof(ScenarioUncertaintyComponent))]
        public IScenarioUncertaintyState ScenarioUncertainty;

        [ModelState(typeof(GeographyComponent))]
        [Binding("landloss", "impactsealevelrise")]
        public IGeographyState Geography;

        [ModelState(typeof(PopulationComponent))]
        [Binding("pgrowth", "scenariouncertainty")]
        [Binding("leave", "impactsealevelrise")]
        [Binding("enter", "impactsealevelrise")]
        [Binding("dead", "impactdeathmorbidity")]
        public IPopulationState Population;

        [ModelState(typeof(SocioEconomicComponent))]
        [Binding("ypcgrowth", "scenariouncertainty")]
        [Binding("pgrowth", "scenariouncertainty")]
        [Binding("eloss", "impactaggregation")]
        [Binding("sloss", "impactaggregation")]
        [Binding("mitigationcost", "emissions")]
        [Binding("area", "geography")]
        [Binding("globalpopulation", "population")]
        [Binding("population", "population")]
        [Binding("populationin1", "population")]
        public ISocioEconomicState SocioEconomic;

        [ModelState(typeof(EmissionsComponent))]
        [Binding("aeei", "scenariouncertainty")]
        [Binding("acei", "scenariouncertainty")]
        [Binding("forestemm", "scenariouncertainty")]
        [Binding("pgrowth", "scenariouncertainty")]
        [Binding("ypcgrowth", "scenariouncertainty")]
        [Binding("income", "socioeconomic")]
        [Binding("population", "socioeconomic")]
        public IEmissionsState Emissions;

        [ModelState(typeof(ClimateCO2CycleComponent))]
        [Binding("mco2", "emissions")]
        [Binding("temp", "climatedynamics")]
        public IClimateCO2CycleState ClimateCO2Cycle;

        [ModelState(typeof(ClimateCH4CycleComponent))]
        [Binding("globch4", "emissions")]
        public IClimateCH4CycleState ClimateCH4Cycle;

        [ModelState(typeof(ClimateN2OCycleComponent))]
        [Binding("globn2o", "emissions")]
        public IClimateN2OCycleState ClimateN2OCycle;

        [ModelState(typeof(ClimateSF6CycleComponent))]
        [Binding("globsf6", "emissions")]
        public IClimateSF6CycleState ClimateSF6Cycle;

        [ModelState(typeof(ClimateSO2CycleComponent))]
        [Binding("globso2", "emissions")]
        public IClimateSO2CycleState ClimateSO2Cycle;

        [ModelState(typeof(ClimateForcingComponent))]
        [Binding("acco2", "climateco2cycle")]
        [Binding("acch4", "climatech4cycle")]
        [Binding("acn2o", "climaten2ocycle")]
        [Binding("acsf6", "climatesf6cycle")]
        [Binding("acso2", "climateso2cycle")]
        public IClimateForcingState ClimateForcing;

        [ModelState(typeof(ClimateDynamicsComponent))]
        [Binding("radforc", "climateforcing")]
        public IClimateDynamicsState ClimateDynamics;

        [ModelState(typeof(ClimateRegionalComponent))]
        [Binding("inputtemp", "climatedynamics", "temp")]
        public IClimateRegionalState ClimateRegional;

        [ModelState(typeof(OceanComponent))]
        [Binding("temp", "climatedynamics")]
        public IOceanState Ocean;

        [ModelState(typeof(BioDiversityComponent))]
        [Binding("temp", "climatedynamics")]
        public IBioDiversityState BioDiversity;

        [ModelState(typeof(ImpactBioDiversityComponent))]
        [Binding("temp", "climateregional")]
        [Binding("nospecies", "biodiversity")]
        [Binding("income", "socioeconomic")]
        [Binding("population", "socioeconomic")]
        public IImpactBioDiversityState ImpactBioDiversity;

        [ModelState(typeof(ImpactHeatingComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("temp", "climateregional")]
        [Binding("cumaeei", "emissions")]
        public IImpactHeatingState ImpactHeating;

        [ModelState(typeof(ImpactCoolingComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("temp", "climateregional")]
        [Binding("cumaeei", "emissions")]
        public IImpactCoolingState ImpactCooling;

        [ModelState(typeof(ImpactAgricultureComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("temp", "climateregional")]
        [Binding("acco2", "climateco2cycle")]
        public IImpactAgricultureState ImpactAgriculture;

        [ModelState(typeof(ImpactWaterResourcesComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("temp", "climateregional")]
        public IImpactWaterResourcesState ImpactWaterResources;

        [ModelState(typeof(ImpactDiarrhoeaComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("regtmp", "climateregional")]
        public IImpactDiarrhoeaState ImpactDiarrhoea;

        [ModelState(typeof(ImpactTropicalStormsComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("regstmp", "climateregional")]
        public IImpactTropicalStormsState ImpactTropicalStorms;

        [ModelState(typeof(ImpactExtratropicalStormsComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("acco2", "climateco2cycle")]
        public IImpactExtratropicalStormsState ImpactExtratropicalStorms;

        [ModelState(typeof(ImpactSeaLevelRiseComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("sea", "ocean")]
        [Binding("area", "geography")]
        public IImpactSeaLevelRiseState ImpactSeaLevelRise;

        [ModelState(typeof(ImpactForests))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("temp", "climateregional")]
        [Binding("acco2", "climateco2cycle")]
        public IImpactForestsState ImpactForests;

        [ModelState(typeof(ImpactVectorBorneDiseasesComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("income", "socioeconomic")]
        [Binding("temp", "climateregional")]
        public IImpactVectorBorneDiseasesState ImpactVectorBorneDiseases;

        [ModelState(typeof(ImpactCardiovascularRespiratoryComponent))]
        [Binding("population", "socioeconomic")]
        [Binding("temp", "climateregional")]
        [Binding("plus", "socioeconomic")]
        [Binding("urbpop", "socioeconomic")]
        public IImpactCardiovascularRespiratoryState ImpactCardiovascularRespiratory;

        [ModelState(typeof(ImpactDeathMorbidityComponent))]
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
        public IImpactDeathMorbidityState ImpactDeathMorbidity;

        [ModelState(typeof(ImpactAggregationComponent))]
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
        public IImpactAggregationState ImpactAggregation;
    }
}