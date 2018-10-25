module Fund

using Mimi

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

export 
    getfund     # Function that returns a version of fund allowing for different user specifications


const global default_nsteps = 1050
const global default_datadir = joinpath(dirname(@__FILE__), "..", "data")
const global default_params = nothing
  
function getfund(; nsteps = default_nsteps, datadir = default_datadir, params = default_params)

    # ---------------------------------------------
    # Load parameters
    # ---------------------------------------------

    if params == nothing
        parameters = loadparameters(datadir)
    else
        parameters = params
    end

    # ---------------------------------------------
    # Create model
    # ---------------------------------------------

    m = Model()

    # ---------------------------------------------
    # Set dimensions
    # ---------------------------------------------

    set_dimension!(m, :time, collect(1950:1950+nsteps))
    set_dimension!(m, :regions, ["USA", "CAN", "WEU", "JPK", "ANZ", "EEU", "FSU", "MDE", "CAM", "LAM", "SAS", "SEA", "CHI", "MAF", "SSA", "SIS"])
    set_dimension!(m, :cpools, 1:5)
    # ---------------------------------------------
    # Create components
    # ---------------------------------------------

    add_comp!(m, scenariouncertainty)
    add_comp!(m, population)
    add_comp!(m, geography)
    add_comp!(m, socioeconomic)
    add_comp!(m, emissions)
    add_comp!(m, climateco2cycle)
    add_comp!(m, climatech4cycle)
    add_comp!(m, climaten2ocycle)
    add_comp!(m, climatesf6cycle)
    add_comp!(m, climateforcing)
    add_comp!(m, climatedynamics)
    add_comp!(m, biodiversity)
    add_comp!(m, climateregional)
    add_comp!(m, ocean)
    add_comp!(m, impactagriculture)
    add_comp!(m, impactbiodiversity)
    add_comp!(m, impactcardiovascularrespiratory)
    add_comp!(m, impactcooling)
    add_comp!(m, impactdiarrhoea)
    add_comp!(m, impactextratropicalstorms)
    add_comp!(m, impactforests)
    add_comp!(m, impactheating)
    add_comp!(m, impactvectorbornediseases)
    add_comp!(m, impacttropicalstorms)
    add_comp!(m, vslvmorb)
    add_comp!(m, impactdeathmorbidity)
    add_comp!(m, impactwaterresources)
    add_comp!(m, impactsealevelrise)
    add_comp!(m, impactaggregation)

    # ---------------------------------------------
    # Connect parameters to variables
    # ---------------------------------------------

    connect_param!(m, :geography, :landloss, :impactsealevelrise, :landloss, offset = 1)

    connect_param!(m, :population, :pgrowth, :scenariouncertainty, :pgrowth, offset = 0)
    connect_param!(m, :population, :enter, :impactsealevelrise, :enter, offset = 1)
    connect_param!(m, :population, :leave, :impactsealevelrise, :leave, offset = 1)
    connect_param!(m, :population, :dead, :impactdeathmorbidity, :dead, offset = 1)

    connect_param!(m, :socioeconomic, :area, :geography, :area, offset = 0)
    connect_param!(m, :socioeconomic, :globalpopulation, :population, :globalpopulation, offset = 0)
    connect_param!(m, :socioeconomic, :populationin1, :population, :populationin1, offset = 0)
    connect_param!(m, :socioeconomic, :population, :population, :population, offset = 0)
    connect_param!(m, :socioeconomic, :pgrowth, :scenariouncertainty, :pgrowth, offset = 0)
    connect_param!(m, :socioeconomic, :ypcgrowth, :scenariouncertainty, :ypcgrowth, offset = 0)
    connect_param!(m, :socioeconomic, :eloss, :impactaggregation, :eloss, offset = 1)
    connect_param!(m, :socioeconomic, :sloss, :impactaggregation, :sloss, offset = 1)
    connect_param!(m, :socioeconomic, :mitigationcost, :emissions, :mitigationcost, offset = 1)

    connect_param!(m, :emissions, :income, :socioeconomic, :income, offset = 0)
    connect_param!(m, :emissions, :population, :population, :population, offset = 0)
    connect_param!(m, :emissions, :forestemm, :scenariouncertainty, :forestemm, offset = 0)
    connect_param!(m, :emissions, :aeei, :scenariouncertainty, :aeei, offset = 0)
    connect_param!(m, :emissions, :acei, :scenariouncertainty, :acei, offset = 0)
    connect_param!(m, :emissions, :ypcgrowth, :scenariouncertainty, :ypcgrowth, offset = 0)

    connect_param!(m, :climateco2cycle, :mco2, :emissions, :mco2, offset = 0)

    connect_param!(m, :climatech4cycle, :globch4, :emissions, :globch4, offset = 0)

    connect_param!(m, :climaten2ocycle, :globn2o, :emissions, :globn2o, offset = 0)

    connect_param!(m, :climateco2cycle, :temp, :climatedynamics, :temp, offset = 1)

    connect_param!(m, :climatesf6cycle, :globsf6, :emissions, :globsf6, offset = 0)

    connect_param!(m, :climateforcing, :acco2, :climateco2cycle, :acco2, offset = 0)
    connect_param!(m, :climateforcing, :acch4, :climatech4cycle, :acch4, offset = 0)
    connect_param!(m, :climateforcing, :acn2o, :climaten2ocycle, :acn2o, offset = 0)
    connect_param!(m, :climateforcing, :acsf6, :climatesf6cycle, :acsf6, offset = 0)

    connect_param!(m, :climatedynamics, :radforc, :climateforcing, :radforc, offset = 0)

    connect_param!(m, :climateregional, :inputtemp, :climatedynamics, :temp, offset = 0)

    connect_param!(m, :biodiversity, :temp, :climatedynamics, :temp, offset = 0)

    connect_param!(m, :ocean, :temp, :climatedynamics, :temp, offset = 0)

    connect_param!(m, :impactagriculture, :population, :population, :population, offset = 0)
    connect_param!(m, :impactagriculture, :income, :socioeconomic, :income, offset = 0)
    connect_param!(m, :impactagriculture, :temp, :climateregional, :temp, offset = 0)
    connect_param!(m, :impactagriculture, :acco2, :climateco2cycle, :acco2, offset = 0)

    connect_param!(m, :impactbiodiversity, :temp, :climateregional, :temp, offset = 0)
    connect_param!(m, :impactbiodiversity, :nospecies, :biodiversity, :nospecies, offset = 0)
    connect_param!(m, :impactbiodiversity, :income, :socioeconomic, :income, offset = 0)
    connect_param!(m, :impactbiodiversity, :population, :population, :population, offset = 0)

    connect_param!(m, :impactcardiovascularrespiratory, :population, :population, :population, offset = 0)
    connect_param!(m, :impactcardiovascularrespiratory, :temp, :climateregional, :temp, offset = 0)
    connect_param!(m, :impactcardiovascularrespiratory, :plus, :socioeconomic, :plus, offset = 0)
    connect_param!(m, :impactcardiovascularrespiratory, :urbpop, :socioeconomic, :urbpop, offset = 0)

    connect_param!(m, :impactcooling, :population, :population, :population, offset = 0)
    connect_param!(m, :impactcooling, :income, :socioeconomic, :income, offset = 0)
    connect_param!(m, :impactcooling, :temp, :climateregional, :temp, offset = 0)
    connect_param!(m, :impactcooling, :cumaeei, :emissions, :cumaeei, offset = 0)

    connect_param!(m, :impactdiarrhoea, :population, :population, :population, offset = 0)
    connect_param!(m, :impactdiarrhoea, :income, :socioeconomic, :income, offset = 0)
    connect_param!(m, :impactdiarrhoea, :regtmp, :climateregional, :regtmp, offset = 0)

    connect_param!(m, :impactextratropicalstorms, :population, :population, :population, offset = 0)
    connect_param!(m, :impactextratropicalstorms, :income, :socioeconomic, :income, offset = 0)
    connect_param!(m, :impactextratropicalstorms, :acco2, :climateco2cycle, :acco2, offset = 0)

    connect_param!(m, :impactforests, :population, :population, :population, offset = 0)
    connect_param!(m, :impactforests, :income, :socioeconomic, :income, offset = 0)
    connect_param!(m, :impactforests, :temp, :climateregional, :temp, offset = 0)
    connect_param!(m, :impactforests, :acco2, :climateco2cycle, :acco2, offset = 0)

    connect_param!(m, :impactheating, :population, :population, :population, offset = 0)
    connect_param!(m, :impactheating, :income, :socioeconomic, :income, offset = 0)
    connect_param!(m, :impactheating, :temp, :climateregional, :temp, offset = 0)
    connect_param!(m, :impactheating, :cumaeei, :emissions, :cumaeei, offset = 0)

    connect_param!(m, :impactvectorbornediseases, :population, :population, :population, offset = 0)
    connect_param!(m, :impactvectorbornediseases, :income, :socioeconomic, :income, offset = 0)
    connect_param!(m, :impactvectorbornediseases, :temp, :climateregional, :temp, offset = 0)

    connect_param!(m, :impacttropicalstorms, :population, :population, :population, offset = 0)
    connect_param!(m, :impacttropicalstorms, :income, :socioeconomic, :income, offset = 0)
    connect_param!(m, :impacttropicalstorms, :regstmp, :climateregional, :regstmp, offset = 0)

    connect_param!(m, :vslvmorb, :population, :population, :population, offset = 0)
    connect_param!(m, :vslvmorb, :income, :socioeconomic, :income, offset = 0)

    connect_param!(m, :impactdeathmorbidity, :vsl, :vslvmorb, :vsl, offset = 0)
    connect_param!(m, :impactdeathmorbidity, :vmorb, :vslvmorb, :vmorb, offset = 0)
    connect_param!(m, :impactdeathmorbidity, :population, :population, :population, offset = 0)
    connect_param!(m, :impactdeathmorbidity, :dengue, :impactvectorbornediseases, :dengue, offset = 0)
    connect_param!(m, :impactdeathmorbidity, :schisto, :impactvectorbornediseases, :schisto, offset = 0)
    connect_param!(m, :impactdeathmorbidity, :malaria, :impactvectorbornediseases, :malaria, offset = 0)
    connect_param!(m, :impactdeathmorbidity, :cardheat, :impactcardiovascularrespiratory, :cardheat, offset = 0)
    connect_param!(m, :impactdeathmorbidity, :cardcold, :impactcardiovascularrespiratory, :cardcold, offset = 0)
    connect_param!(m, :impactdeathmorbidity, :resp, :impactcardiovascularrespiratory, :resp, offset = 0)
    connect_param!(m, :impactdeathmorbidity, :diadead, :impactdiarrhoea, :diadead, offset = 0)
    connect_param!(m, :impactdeathmorbidity, :hurrdead, :impacttropicalstorms, :hurrdead, offset = 0)
    connect_param!(m, :impactdeathmorbidity, :extratropicalstormsdead, :impactextratropicalstorms, :extratropicalstormsdead, offset = 0)
    connect_param!(m, :impactdeathmorbidity, :diasick, :impactdiarrhoea, :diasick, offset = 0)

    connect_param!(m, :impactwaterresources, :population, :population, :population, offset = 0)
    connect_param!(m, :impactwaterresources, :income, :socioeconomic, :income, offset = 0)
    connect_param!(m, :impactwaterresources, :temp, :climateregional, :temp, offset = 0)

    connect_param!(m, :impactsealevelrise, :population, :population, :population, offset = 0)
    connect_param!(m, :impactsealevelrise, :income, :socioeconomic, :income, offset = 0)
    connect_param!(m, :impactsealevelrise, :sea, :ocean, :sea, offset = 0)
    connect_param!(m, :impactsealevelrise, :area, :geography, :area, offset = 0)

    connect_param!(m, :impactaggregation, :income, :socioeconomic, :income, offset = 0)
    connect_param!(m, :impactaggregation, :heating, :impactheating, :heating, offset = 0)
    connect_param!(m, :impactaggregation, :cooling, :impactcooling, :cooling, offset = 0)
    connect_param!(m, :impactaggregation, :agcost, :impactagriculture, :agcost, offset = 0)
    connect_param!(m, :impactaggregation, :species, :impactbiodiversity, :species, offset = 0)
    connect_param!(m, :impactaggregation, :water, :impactwaterresources, :water, offset = 0)
    connect_param!(m, :impactaggregation, :hurrdam, :impacttropicalstorms, :hurrdam, offset = 0)
    connect_param!(m, :impactaggregation, :extratropicalstormsdam, :impactextratropicalstorms, :extratropicalstormsdam, offset = 0)
    connect_param!(m, :impactaggregation, :forests, :impactforests, :forests, offset = 0)
    connect_param!(m, :impactaggregation, :drycost, :impactsealevelrise, :drycost, offset = 0)
    connect_param!(m, :impactaggregation, :protcost, :impactsealevelrise, :protcost, offset = 0)
    connect_param!(m, :impactaggregation, :entercost, :impactsealevelrise, :entercost, offset = 0)
    connect_param!(m, :impactaggregation, :deadcost, :impactdeathmorbidity, :deadcost, offset = 0)
    connect_param!(m, :impactaggregation, :morbcost, :impactdeathmorbidity, :morbcost, offset = 0)
    connect_param!(m, :impactaggregation, :wetcost, :impactsealevelrise, :wetcost, offset = 0)
    connect_param!(m, :impactaggregation, :leavecost, :impactsealevelrise, :leavecost, offset = 0)

    # ---------------------------------------------
    # Set leftover parameters
    # ---------------------------------------------

    set_leftover_params!(m, parameters)

    return m

end 

end #module
