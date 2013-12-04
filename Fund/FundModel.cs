// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmf.Model;
using Fund.Components;

namespace Fund
{
    public static class FundModel
    {
        public static Model GetModel(bool storeFullVariablesByDefault = true, int years = 1049, int logLevel = 1)
        {
            var m = new Model(storeFullVariablesByDefault: storeFullVariablesByDefault, years: years, logLevel: logLevel);

            m.AddComponent<ScenarioUncertaintyComponent>("ScenarioUncertainty");

            m.AddComponent<GeographyComponent>("Geography");
            m["Geography"].Parameters["landloss"].Bind("impactsealevelrise");

            m.AddComponent<PopulationComponent>("Population");
            m["Population"].Parameters["pgrowth"].Bind("scenariouncertainty");
            m["Population"].Parameters["leave"].Bind("impactsealevelrise");
            m["Population"].Parameters["enter"].Bind("impactsealevelrise");
            m["Population"].Parameters["dead"].Bind("impactdeathmorbidity");

            m.AddComponent<SocioEconomicComponent>("SocioEconomic");
            m["SocioEconomic"].Parameters["ypcgrowth"].Bind("scenariouncertainty");
            m["SocioEconomic"].Parameters["pgrowth"].Bind("scenariouncertainty");
            m["SocioEconomic"].Parameters["eloss"].Bind("impactaggregation");
            m["SocioEconomic"].Parameters["sloss"].Bind("impactaggregation");
            m["SocioEconomic"].Parameters["mitigationcost"].Bind("emissions");
            m["SocioEconomic"].Parameters["area"].Bind("geography");
            m["SocioEconomic"].Parameters["globalpopulation"].Bind("population");
            m["SocioEconomic"].Parameters["population"].Bind("population");
            m["SocioEconomic"].Parameters["populationin1"].Bind("population");

            m.AddComponent<EmissionsComponent>("Emissions");
            m["Emissions"].Parameters["aeei"].Bind("scenariouncertainty");
            m["Emissions"].Parameters["acei"].Bind("scenariouncertainty");
            m["Emissions"].Parameters["forestemm"].Bind("scenariouncertainty");
            m["Emissions"].Parameters["pgrowth"].Bind("scenariouncertainty");
            m["Emissions"].Parameters["ypcgrowth"].Bind("scenariouncertainty");
            m["Emissions"].Parameters["income"].Bind("socioeconomic");
            m["Emissions"].Parameters["population"].Bind("socioeconomic");

            m.AddComponent<ClimateCO2CycleComponent>("ClimateCO2Cycle");
            m["ClimateCO2Cycle"].Parameters["mco2"].Bind("emissions");
            m["ClimateCO2Cycle"].Parameters["temp"].Bind("climatedynamics");

            m.AddComponent<ClimateCH4CycleComponent>("ClimateCH4Cycle");
            m["ClimateCH4Cycle"].Parameters["globch4"].Bind("emissions");

            m.AddComponent<ClimateN2OCycleComponent>("ClimateN2OCycle");
            m["ClimateN2OCycle"].Parameters["globn2o"].Bind("emissions");

            m.AddComponent<ClimateSF6CycleComponent>("ClimateSF6Cycle");
            m["ClimateSF6Cycle"].Parameters["globsf6"].Bind("emissions");

            m.AddComponent<ClimateForcingComponent>("ClimateForcing");
            m["ClimateForcing"].Parameters["acco2"].Bind("climateco2cycle");
            m["ClimateForcing"].Parameters["acch4"].Bind("climatech4cycle");
            m["ClimateForcing"].Parameters["acn2o"].Bind("climaten2ocycle");
            m["ClimateForcing"].Parameters["acsf6"].Bind("climatesf6cycle");

            m.AddComponent<ClimateDynamicsComponent>("ClimateDynamics");
            m["ClimateDynamics"].Parameters["radforc"].Bind("climateforcing");

            m.AddComponent<ClimateRegionalComponent>("ClimateRegional");
            m["ClimateRegional"].Parameters["inputtemp"].Bind("climatedynamics", "temp");

            m.AddComponent<OceanComponent>("Ocean");
            m["Ocean"].Parameters["temp"].Bind("climatedynamics");

            m.AddComponent<BioDiversityComponent>("BioDiversity");
            m["BioDiversity"].Parameters["temp"].Bind("climatedynamics");

            m.AddComponent<ImpactBioDiversityComponent>("ImpactBioDiversity");
            m["ImpactBioDiversity"].Parameters["temp"].Bind("climateregional");
            m["ImpactBioDiversity"].Parameters["nospecies"].Bind("biodiversity");
            m["ImpactBioDiversity"].Parameters["income"].Bind("socioeconomic");
            m["ImpactBioDiversity"].Parameters["population"].Bind("socioeconomic");

            m.AddComponent<ImpactHeatingComponent>("ImpactHeating");
            m["ImpactHeating"].Parameters["population"].Bind("socioeconomic");
            m["ImpactHeating"].Parameters["income"].Bind("socioeconomic");
            m["ImpactHeating"].Parameters["temp"].Bind("climateregional");
            m["ImpactHeating"].Parameters["cumaeei"].Bind("emissions");

            m.AddComponent<ImpactCoolingComponent>("ImpactCooling");
            m["ImpactCooling"].Parameters["population"].Bind("socioeconomic");
            m["ImpactCooling"].Parameters["income"].Bind("socioeconomic");
            m["ImpactCooling"].Parameters["temp"].Bind("climateregional");
            m["ImpactCooling"].Parameters["cumaeei"].Bind("emissions");

            m.AddComponent<ImpactAgricultureComponent>("ImpactAgriculture");
            m["ImpactAgriculture"].Parameters["population"].Bind("socioeconomic");
            m["ImpactAgriculture"].Parameters["income"].Bind("socioeconomic");
            m["ImpactAgriculture"].Parameters["temp"].Bind("climateregional");
            m["ImpactAgriculture"].Parameters["acco2"].Bind("climateco2cycle");

            m.AddComponent<ImpactWaterResourcesComponent>("ImpactWaterResources");
            m["ImpactWaterResources"].Parameters["population"].Bind("socioeconomic");
            m["ImpactWaterResources"].Parameters["income"].Bind("socioeconomic");
            m["ImpactWaterResources"].Parameters["temp"].Bind("climateregional");

            m.AddComponent<ImpactDiarrhoeaComponent>("ImpactDiarrhoea");
            m["ImpactDiarrhoea"].Parameters["population"].Bind("socioeconomic");
            m["ImpactDiarrhoea"].Parameters["income"].Bind("socioeconomic");
            m["ImpactDiarrhoea"].Parameters["regtmp"].Bind("climateregional");

            m.AddComponent<ImpactTropicalStormsComponent>("ImpactTropicalStorms");
            m["ImpactTropicalStorms"].Parameters["population"].Bind("socioeconomic");
            m["ImpactTropicalStorms"].Parameters["income"].Bind("socioeconomic");
            m["ImpactTropicalStorms"].Parameters["regstmp"].Bind("climateregional");

            m.AddComponent<ImpactExtratropicalStormsComponent>("ImpactExtratropicalStorms");
            m["ImpactExtratropicalStorms"].Parameters["population"].Bind("socioeconomic");
            m["ImpactExtratropicalStorms"].Parameters["income"].Bind("socioeconomic");
            m["ImpactExtratropicalStorms"].Parameters["acco2"].Bind("climateco2cycle");

            m.AddComponent<ImpactSeaLevelRiseComponent>("ImpactSeaLevelRise");
            m["ImpactSeaLevelRise"].Parameters["population"].Bind("socioeconomic");
            m["ImpactSeaLevelRise"].Parameters["income"].Bind("socioeconomic");
            m["ImpactSeaLevelRise"].Parameters["sea"].Bind("ocean");
            m["ImpactSeaLevelRise"].Parameters["area"].Bind("geography");

            m.AddComponent<ImpactForests>("ImpactForests");
            m["ImpactForests"].Parameters["population"].Bind("socioeconomic");
            m["ImpactForests"].Parameters["income"].Bind("socioeconomic");
            m["ImpactForests"].Parameters["temp"].Bind("climateregional");
            m["ImpactForests"].Parameters["acco2"].Bind("climateco2cycle");

            m.AddComponent<ImpactVectorBorneDiseasesComponent>("ImpactVectorBorneDiseases");
            m["ImpactVectorBorneDiseases"].Parameters["population"].Bind("socioeconomic");
            m["ImpactVectorBorneDiseases"].Parameters["income"].Bind("socioeconomic");
            m["ImpactVectorBorneDiseases"].Parameters["temp"].Bind("climateregional");

            m.AddComponent<ImpactCardiovascularRespiratoryComponent>("ImpactCardiovascularRespiratory");
            m["ImpactCardiovascularRespiratory"].Parameters["population"].Bind("socioeconomic");
            m["ImpactCardiovascularRespiratory"].Parameters["temp"].Bind("climateregional");
            m["ImpactCardiovascularRespiratory"].Parameters["plus"].Bind("socioeconomic");
            m["ImpactCardiovascularRespiratory"].Parameters["urbpop"].Bind("socioeconomic");

            m.AddComponent<ImpactDeathMorbidityComponent>("ImpactDeathMorbidity");
            m["ImpactDeathMorbidity"].Parameters["population"].Bind("socioeconomic");
            m["ImpactDeathMorbidity"].Parameters["income"].Bind("socioeconomic");
            m["ImpactDeathMorbidity"].Parameters["dengue"].Bind("impactvectorbornediseases");
            m["ImpactDeathMorbidity"].Parameters["schisto"].Bind("impactvectorbornediseases");
            m["ImpactDeathMorbidity"].Parameters["malaria"].Bind("impactvectorbornediseases");
            m["ImpactDeathMorbidity"].Parameters["cardheat"].Bind("impactcardiovascularrespiratory");
            m["ImpactDeathMorbidity"].Parameters["cardcold"].Bind("impactcardiovascularrespiratory");
            m["ImpactDeathMorbidity"].Parameters["resp"].Bind("impactcardiovascularrespiratory");
            m["ImpactDeathMorbidity"].Parameters["diadead"].Bind("impactdiarrhoea");
            m["ImpactDeathMorbidity"].Parameters["hurrdead"].Bind("impacttropicalstorms");
            m["ImpactDeathMorbidity"].Parameters["extratropicalstormsdead"].Bind("impactextratropicalstorms");
            m["ImpactDeathMorbidity"].Parameters["diasick"].Bind("impactdiarrhoea");

            m.AddComponent<ImpactAggregationComponent>("ImpactAggregation");
            m["ImpactAggregation"].Parameters["income"].Bind("socioeconomic");
            m["ImpactAggregation"].Parameters["heating"].Bind("impactheating");
            m["ImpactAggregation"].Parameters["cooling"].Bind("impactcooling");
            m["ImpactAggregation"].Parameters["agcost"].Bind("impactagriculture");
            m["ImpactAggregation"].Parameters["species"].Bind("impactbiodiversity");
            m["ImpactAggregation"].Parameters["water"].Bind("impactwaterresources");
            m["ImpactAggregation"].Parameters["hurrdam"].Bind("impacttropicalstorms");
            m["ImpactAggregation"].Parameters["extratropicalstormsdam"].Bind("impactextratropicalstorms");
            m["ImpactAggregation"].Parameters["forests"].Bind("impactforests");
            m["ImpactAggregation"].Parameters["drycost"].Bind("impactsealevelrise");
            m["ImpactAggregation"].Parameters["protcost"].Bind("impactsealevelrise");
            m["ImpactAggregation"].Parameters["entercost"].Bind("impactsealevelrise");
            m["ImpactAggregation"].Parameters["deadcost"].Bind("impactdeathmorbidity");
            m["ImpactAggregation"].Parameters["morbcost"].Bind("impactdeathmorbidity");
            m["ImpactAggregation"].Parameters["wetcost"].Bind("impactsealevelrise");
            m["ImpactAggregation"].Parameters["leavecost"].Bind("impactsealevelrise");

            return m;
        }
    }
}
