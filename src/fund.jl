include("helper.jl")

include("SocioEconomicComponent.jl")
include("PopulationComponent.jl")
include("EmissionsComponent.jl")
include("GeographyComponent.jl")
include("ScenarioUncertaintyComponent.jl")
include("ClimateCO2CycleComponent.jl")
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
include("ImpactForestsComponent.jl")
include("ImpactHeatingComponent.jl")
include("ImpactVectorBorneDiseasesComponent.jl")
include("ImpactTropicalStormsComponent.jl")
include("ImpactWaterResourcesComponent.jl")
include("ImpactSeaLevelRiseComponent.jl")
include("ImpactAggregationComponent.jl")
include("VslVmorbComponent.jl")

function constructfund(;nsteps=1050)
    m = Model()

    setindex(m, :time, [1950:1950+nsteps])
    setindex(m, :regions, ["USA", "CAN", "WEU", "JPK", "ANZ", "EEU", "FSU", "MDE", "CAM", "LAM", "SAS", "SEA", "CHI", "MAF", "SSA", "SIS"])

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
    addcomponent(m, vslvmorb)
    addcomponent(m, impactdeathmorbidity)
    addcomponent(m, impactwaterresources)
    addcomponent(m, impactsealevelrise)
    addcomponent(m, impactaggregation)

    # ---------------------------------------------
    # Connect parameters to variables
    # ---------------------------------------------

    connectparameter(m, :geography, :landloss, :impactsealevelrise)

    connectparameter(m, :population, :pgrowth, :scenariouncertainty)
    connectparameter(m, :population, :enter, :impactsealevelrise)
    connectparameter(m, :population, :leave, :impactsealevelrise)
    connectparameter(m, :population, :dead, :impactdeathmorbidity)

    connectparameter(m, :socioeconomic, :area, :geography)
    connectparameter(m, :socioeconomic, :globalpopulation, :population)
    connectparameter(m, :socioeconomic, :populationin1, :population)
    connectparameter(m, :socioeconomic, :population, :population)
    connectparameter(m, :socioeconomic, :pgrowth, :scenariouncertainty)
    connectparameter(m, :socioeconomic, :ypcgrowth, :scenariouncertainty)

    connectparameter(m, :socioeconomic, :eloss, :impactaggregation)
    connectparameter(m, :socioeconomic, :sloss, :impactaggregation)
    connectparameter(m, :socioeconomic, :mitigationcost, :emissions)

    connectparameter(m, :emissions, :income, :socioeconomic)
    connectparameter(m, :emissions, :population, :population)
    connectparameter(m, :emissions, :forestemm, :scenariouncertainty)
    connectparameter(m, :emissions, :aeei, :scenariouncertainty)
    connectparameter(m, :emissions, :acei, :scenariouncertainty)
    connectparameter(m, :emissions, :ypcgrowth, :scenariouncertainty)

    connectparameter(m, :climateco2cycle, :mco2, :emissions)

    connectparameter(m, :climatech4cycle, :globch4, :emissions)

    connectparameter(m, :climaten2ocycle, :globn2o, :emissions)
    connectparameter(m, :climateco2cycle, :temp, :climatedynamics)

    connectparameter(m, :climatesf6cycle, :globsf6, :emissions)

    connectparameter(m, :climateforcing, :acco2, :climateco2cycle)
    connectparameter(m, :climateforcing, :acch4, :climatech4cycle)
    connectparameter(m, :climateforcing, :acn2o, :climaten2ocycle)
    connectparameter(m, :climateforcing, :acsf6, :climatesf6cycle)

    connectparameter(m, :climatedynamics, :radforc, :climateforcing)

    connectparameter(m, :climateregional, :inputtemp, :climatedynamics, :temp)

    connectparameter(m, :biodiversity, :temp, :climatedynamics)

    connectparameter(m, :ocean, :temp, :climatedynamics)

    connectparameter(m, :impactagriculture, :population, :population)
    connectparameter(m, :impactagriculture, :income, :socioeconomic)
    connectparameter(m, :impactagriculture, :temp, :climateregional)
    connectparameter(m, :impactagriculture, :acco2, :climateco2cycle)

    connectparameter(m, :impactbiodiversity, :temp, :climateregional)
    connectparameter(m, :impactbiodiversity, :nospecies, :biodiversity)
    connectparameter(m, :impactbiodiversity, :income, :socioeconomic)
    connectparameter(m, :impactbiodiversity, :population, :population)

    connectparameter(m, :impactcardiovascularrespiratory, :population, :population)
    connectparameter(m, :impactcardiovascularrespiratory, :temp, :climateregional)
    connectparameter(m, :impactcardiovascularrespiratory, :plus, :socioeconomic)
    connectparameter(m, :impactcardiovascularrespiratory, :urbpop, :socioeconomic)

    connectparameter(m, :impactcooling, :population, :population)
    connectparameter(m, :impactcooling, :income, :socioeconomic)
    connectparameter(m, :impactcooling, :temp, :climateregional)
    connectparameter(m, :impactcooling, :cumaeei, :emissions)

    connectparameter(m, :impactdiarrhoea, :population, :population)
    connectparameter(m, :impactdiarrhoea, :income, :socioeconomic)
    connectparameter(m, :impactdiarrhoea, :regtmp, :climateregional)

    connectparameter(m, :impactextratropicalstorms, :population, :population)
    connectparameter(m, :impactextratropicalstorms, :income, :socioeconomic)
    connectparameter(m, :impactextratropicalstorms, :acco2, :climateco2cycle)

    connectparameter(m, :impactforests, :population, :population)
    connectparameter(m, :impactforests, :income, :socioeconomic)
    connectparameter(m, :impactforests, :temp, :climateregional)
    connectparameter(m, :impactforests, :acco2, :climateco2cycle)

    connectparameter(m, :impactheating, :population, :population)
    connectparameter(m, :impactheating, :income, :socioeconomic)
    connectparameter(m, :impactheating, :temp, :climateregional)
    connectparameter(m, :impactheating, :cumaeei, :emissions)

    connectparameter(m, :impactvectorbornediseases, :population, :population)
    connectparameter(m, :impactvectorbornediseases, :income, :socioeconomic)
    connectparameter(m, :impactvectorbornediseases, :temp, :climateregional)

    connectparameter(m, :impacttropicalstorms, :population, :population)
    connectparameter(m, :impacttropicalstorms, :income, :socioeconomic)
    connectparameter(m, :impacttropicalstorms, :regstmp, :climateregional)

    connectparameter(m, :vslvmorb, :population, :population)
    connectparameter(m, :vslvmorb, :income, :socioeconomic)

    connectparameter(m, :impactdeathmorbidity, :vsl, :vslvmorb)
    connectparameter(m, :impactdeathmorbidity, :vmorb, :vslvmorb)
    connectparameter(m, :impactdeathmorbidity, :population, :population)
    connectparameter(m, :impactdeathmorbidity, :dengue, :impactvectorbornediseases)
    connectparameter(m, :impactdeathmorbidity, :schisto, :impactvectorbornediseases)
    connectparameter(m, :impactdeathmorbidity, :malaria, :impactvectorbornediseases)
    connectparameter(m, :impactdeathmorbidity, :cardheat, :impactcardiovascularrespiratory)
    connectparameter(m, :impactdeathmorbidity, :cardcold, :impactcardiovascularrespiratory)
    connectparameter(m, :impactdeathmorbidity, :resp, :impactcardiovascularrespiratory)
    connectparameter(m, :impactdeathmorbidity, :diadead, :impactdiarrhoea)
    connectparameter(m, :impactdeathmorbidity, :hurrdead, :impacttropicalstorms)
    connectparameter(m, :impactdeathmorbidity, :extratropicalstormsdead, :impactextratropicalstorms)
    connectparameter(m, :impactdeathmorbidity, :diasick, :impactdiarrhoea)

    connectparameter(m, :impactwaterresources, :population, :population)
    connectparameter(m, :impactwaterresources, :income, :socioeconomic)
    connectparameter(m, :impactwaterresources, :temp, :climateregional)

    connectparameter(m, :impactsealevelrise, :population, :population)
    connectparameter(m, :impactsealevelrise, :income, :socioeconomic)
    connectparameter(m, :impactsealevelrise, :sea, :ocean)
    connectparameter(m, :impactsealevelrise, :area, :geography)

    connectparameter(m, :impactaggregation, :income, :socioeconomic)
    connectparameter(m, :impactaggregation, :heating, :impactheating)
    connectparameter(m, :impactaggregation, :cooling, :impactcooling)
    connectparameter(m, :impactaggregation, :agcost, :impactagriculture)
    connectparameter(m, :impactaggregation, :species, :impactbiodiversity)
    connectparameter(m, :impactaggregation, :water, :impactwaterresources)
    connectparameter(m, :impactaggregation, :hurrdam, :impacttropicalstorms)
    connectparameter(m, :impactaggregation, :extratropicalstormsdam, :impactextratropicalstorms)
    connectparameter(m, :impactaggregation, :forests, :impactforests)
    connectparameter(m, :impactaggregation, :drycost, :impactsealevelrise)
    connectparameter(m, :impactaggregation, :protcost, :impactsealevelrise)
    connectparameter(m, :impactaggregation, :entercost, :impactsealevelrise)
    connectparameter(m, :impactaggregation, :deadcost, :impactdeathmorbidity)
    connectparameter(m, :impactaggregation, :morbcost, :impactdeathmorbidity)
    connectparameter(m, :impactaggregation, :wetcost, :impactsealevelrise)
    connectparameter(m, :impactaggregation, :leavecost, :impactsealevelrise)

    return m
end

function getfund(;nsteps=1050, datadir="../data", params=nothing)
    # ---------------------------------------------
    # Load parameters
    # ---------------------------------------------
    if params==nothing
        parameters = loadparameters(datadir)
    else
        parameters = params
    end

    # ---------------------------------------------
    # Construct model
    # ---------------------------------------------
    m = constructfund(nsteps=nsteps)

    # ---------------------------------------------
    # Load remaining parameters from file
    # ---------------------------------------------
    setleftoverparameters(m, parameters)

    # ---------------------------------------------
    # Return model
    # ---------------------------------------------

    return m
end
