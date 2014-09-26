include("helper.jl")

include("SocioEconomicComponent.jl")
include("PopulationComponent.jl")
include("EmissionsComponent.jl")
include("GeographyComponent.jl")
include("ScenarioUncertaintyComponent.jl")
include("ClimateCO2Cycle.jl")
include("ClimateCH4CycleComponent.jl")
include("ClimateN2OCycleComponent.jl")
include("ClimateSF6CycleComponent.jl")
include("ClimateForcingComponent.jl")
include("ClimateDynamicsComponent.jl")
include("BioDiversityComponent.jl")
include("ClimateRegionalComponent.jl")
include("OceanComponent.jl")
include("ImpactAgricultureComponent.jl")
include("ImpactBioDiversityComponent.jl")
include("ImpactCardiovascularRespiratoryComponent.jl")
include("ImpactCoolingComponent.jl")
include("ImpactDiarrhoeaComponent.jl")
include("ImpactExtratropicalStormsComponent.jl")
include("ImpactDeathMorbidityComponent.jl")
include("ImpactForests.jl")
include("ImpactHeatingComponent.jl")
include("ImpactVectorBorneDiseasesComponent.jl")
include("ImpactTropicalStormsComponent.jl")
include("ImpactWaterResourcesComponent.jl")
include("ImpactSeaLevelRiseComponent.jl")
include("ImpactAggregationComponent.jl")

function getfund(;nsteps=1050, datadir="../data", params=nothing)
    m = Model()

    setindex(m, :time, nsteps)
    setindex(m, :regions, 16)

    # ---------------------------------------------
    # Create components
    # ---------------------------------------------
    addcomponent(m, scenariouncertainty)
    addcomponent(m, population)
    addcomponent(m, geography)
    addcomponent(m, socioeconomic)
    addcomponent(m, emissions)
    addcomponent(m, climateco2cycle)
    addcomponent(m, climatech4cycle)
    addcomponent(m, climaten2ocycle)
    addcomponent(m, climatesf6cycle)
    addcomponent(m, climateforcing)
    addcomponent(m, climatedynamics)
    addcomponent(m, biodiversity)
    addcomponent(m, climateregional)
    addcomponent(m, ocean)
    addcomponent(m, impactagriculture)
    addcomponent(m, impactbiodiversity)
    addcomponent(m, impactcardiovascularrespiratory)
    addcomponent(m, impactcooling)
    addcomponent(m, impactdiarrhoea)
    addcomponent(m, impactextratropicalstorms)
    addcomponent(m, impactforests)
    addcomponent(m, impactheating)
    addcomponent(m, impactvectorbornediseases)
    addcomponent(m, impacttropicalstorms)
    addcomponent(m, impactdeathmorbidity)
    addcomponent(m, impactwaterresources)
    addcomponent(m, impactsealevelrise)
    addcomponent(m, impactaggregation)

    # ---------------------------------------------
    # Load parameters
    # ---------------------------------------------
    if params==nothing
        parameters = loadparameters(datadir)
    else
        parameters = params
    end

    # ---------------------------------------------
    # Set parameters
    # ---------------------------------------------

    setparameter(m, :population, :runwithoutpopulationperturbation, false)
    setparameter(m, :socioeconomic, :runwithoutdamage, false)

    # ---------------------------------------------
    # Connect parameters to variables
    # ---------------------------------------------

    bindparameter(m, :geography, :landloss, :impactsealevelrise)

    bindparameter(m, :population, :pgrowth, :scenariouncertainty)
    bindparameter(m, :population, :enter, :impactsealevelrise)
    bindparameter(m, :population, :leave, :impactsealevelrise)
    bindparameter(m, :population, :dead, :impactdeathmorbidity)

    bindparameter(m, :socioeconomic, :area, :geography)
    bindparameter(m, :socioeconomic, :globalpopulation, :population)
    bindparameter(m, :socioeconomic, :populationin1, :population)
    bindparameter(m, :socioeconomic, :population, :population)
    bindparameter(m, :socioeconomic, :pgrowth, :scenariouncertainty)
    bindparameter(m, :socioeconomic, :ypcgrowth, :scenariouncertainty)

    bindparameter(m, :socioeconomic, :eloss, :impactaggregation)
    bindparameter(m, :socioeconomic, :sloss, :impactaggregation)
    bindparameter(m, :socioeconomic, :mitigationcost, :emissions)

    bindparameter(m, :emissions, :income, :socioeconomic)
    bindparameter(m, :emissions, :population, :population)
    bindparameter(m, :emissions, :forestemm, :scenariouncertainty)
    bindparameter(m, :emissions, :aeei, :scenariouncertainty)
    bindparameter(m, :emissions, :acei, :scenariouncertainty)
    bindparameter(m, :emissions, :ypcgrowth, :scenariouncertainty)

    bindparameter(m, :climateco2cycle, :mco2, :emissions)

    bindparameter(m, :climatech4cycle, :globch4, :emissions)

    bindparameter(m, :climaten2ocycle, :globn2o, :emissions)
    bindparameter(m, :climateco2cycle, :temp, :climatedynamics)

    bindparameter(m, :climatesf6cycle, :globsf6, :emissions)

    bindparameter(m, :climateforcing, :acco2, :climateco2cycle)
    bindparameter(m, :climateforcing, :acch4, :climatech4cycle)
    bindparameter(m, :climateforcing, :acn2o, :climaten2ocycle)
    bindparameter(m, :climateforcing, :acsf6, :climatesf6cycle)

    bindparameter(m, :climatedynamics, :radforc, :climateforcing)

    bindparameter(m, :climateregional, :inputtemp, :climatedynamics, :temp)

    bindparameter(m, :biodiversity, :temp, :climatedynamics)

    bindparameter(m, :ocean, :temp, :climatedynamics)

    bindparameter(m, :impactagriculture, :population, :population)
    bindparameter(m, :impactagriculture, :income, :socioeconomic)
    bindparameter(m, :impactagriculture, :temp, :climateregional)
    bindparameter(m, :impactagriculture, :acco2, :climateco2cycle)

    bindparameter(m, :impactbiodiversity, :temp, :climateregional)
    bindparameter(m, :impactbiodiversity, :nospecies, :biodiversity)
    bindparameter(m, :impactbiodiversity, :income, :socioeconomic)
    bindparameter(m, :impactbiodiversity, :population, :population)

    bindparameter(m, :impactcardiovascularrespiratory, :population, :population)
    bindparameter(m, :impactcardiovascularrespiratory, :temp, :climateregional)
    bindparameter(m, :impactcardiovascularrespiratory, :plus, :socioeconomic)
    bindparameter(m, :impactcardiovascularrespiratory, :urbpop, :socioeconomic)

    bindparameter(m, :impactcooling, :population, :population)
    bindparameter(m, :impactcooling, :income, :socioeconomic)
    bindparameter(m, :impactcooling, :temp, :climateregional)
    bindparameter(m, :impactcooling, :cumaeei, :emissions)

    bindparameter(m, :impactdiarrhoea, :population, :population)
    bindparameter(m, :impactdiarrhoea, :income, :socioeconomic)
    bindparameter(m, :impactdiarrhoea, :regtmp, :climateregional)

    bindparameter(m, :impactextratropicalstorms, :population, :population)
    bindparameter(m, :impactextratropicalstorms, :income, :socioeconomic)
    bindparameter(m, :impactextratropicalstorms, :acco2, :climateco2cycle)

    bindparameter(m, :impactforests, :population, :population)
    bindparameter(m, :impactforests, :income, :socioeconomic)
    bindparameter(m, :impactforests, :temp, :climateregional)
    bindparameter(m, :impactforests, :acco2, :climateco2cycle)

    bindparameter(m, :impactheating, :population, :population)
    bindparameter(m, :impactheating, :income, :socioeconomic)
    bindparameter(m, :impactheating, :temp, :climateregional)
    bindparameter(m, :impactheating, :cumaeei, :emissions)

    bindparameter(m, :impactvectorbornediseases, :population, :population)
    bindparameter(m, :impactvectorbornediseases, :income, :socioeconomic)
    bindparameter(m, :impactvectorbornediseases, :temp, :climateregional)

    bindparameter(m, :impacttropicalstorms, :population, :population)
    bindparameter(m, :impacttropicalstorms, :income, :socioeconomic)
    bindparameter(m, :impacttropicalstorms, :regstmp, :climateregional)

    bindparameter(m, :impactdeathmorbidity, :population, :population)
    bindparameter(m, :impactdeathmorbidity, :income, :socioeconomic)
    bindparameter(m, :impactdeathmorbidity, :dengue, :impactvectorbornediseases)
    bindparameter(m, :impactdeathmorbidity, :schisto, :impactvectorbornediseases)
    bindparameter(m, :impactdeathmorbidity, :malaria, :impactvectorbornediseases)
    bindparameter(m, :impactdeathmorbidity, :cardheat, :impactcardiovascularrespiratory)
    bindparameter(m, :impactdeathmorbidity, :cardcold, :impactcardiovascularrespiratory)
    bindparameter(m, :impactdeathmorbidity, :resp, :impactcardiovascularrespiratory)
    bindparameter(m, :impactdeathmorbidity, :diadead, :impactdiarrhoea)
    bindparameter(m, :impactdeathmorbidity, :hurrdead, :impacttropicalstorms)
    bindparameter(m, :impactdeathmorbidity, :extratropicalstormsdead, :impactextratropicalstorms)
    bindparameter(m, :impactdeathmorbidity, :diasick, :impactdiarrhoea)

    bindparameter(m, :impactwaterresources, :population, :population)
    bindparameter(m, :impactwaterresources, :income, :socioeconomic)
    bindparameter(m, :impactwaterresources, :temp, :climateregional)

    bindparameter(m, :impactsealevelrise, :population, :population)
    bindparameter(m, :impactsealevelrise, :income, :socioeconomic)
    bindparameter(m, :impactsealevelrise, :sea, :ocean)
    bindparameter(m, :impactsealevelrise, :area, :geography)

    bindparameter(m, :impactaggregation, :income, :socioeconomic)
    bindparameter(m, :impactaggregation, :heating, :impactheating)
    bindparameter(m, :impactaggregation, :cooling, :impactcooling)
    bindparameter(m, :impactaggregation, :agcost, :impactagriculture)
    bindparameter(m, :impactaggregation, :species, :impactbiodiversity)
    bindparameter(m, :impactaggregation, :water, :impactwaterresources)
    bindparameter(m, :impactaggregation, :hurrdam, :impacttropicalstorms)
    bindparameter(m, :impactaggregation, :extratropicalstormsdam, :impactextratropicalstorms)
    bindparameter(m, :impactaggregation, :forests, :impactforests)
    bindparameter(m, :impactaggregation, :drycost, :impactsealevelrise)
    bindparameter(m, :impactaggregation, :protcost, :impactsealevelrise)
    bindparameter(m, :impactaggregation, :entercost, :impactsealevelrise)
    bindparameter(m, :impactaggregation, :deadcost, :impactdeathmorbidity)
    bindparameter(m, :impactaggregation, :morbcost, :impactdeathmorbidity)
    bindparameter(m, :impactaggregation, :wetcost, :impactsealevelrise)
    bindparameter(m, :impactaggregation, :leavecost, :impactsealevelrise)

    # ---------------------------------------------
    # Load remaining parameters from file
    # ---------------------------------------------
    setleftoverparameters(m, parameters)

    # ---------------------------------------------
    # Return model
    # ---------------------------------------------

    return m
end
