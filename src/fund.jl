module fund

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
    FUND,       # Default version of fund
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

    FUND = Model()

    # ---------------------------------------------
    # Set dimensions
    # ---------------------------------------------

    set_dimension!(FUND, :time, collect(1950:1950+nsteps))
    set_dimension!(FUND, :regions, ["USA", "CAN", "WEU", "JPK", "ANZ", "EEU", "FSU", "MDE", "CAM", "LAM", "SAS", "SEA", "CHI", "MAF", "SSA", "SIS"])
    set_dimension!(FUND, :cpools, 1:5)

    # ---------------------------------------------
    # Create components
    # ---------------------------------------------

    add_comp!(FUND, scenariouncertainty, :scenariouncertainty)
    add_comp!(FUND, population, :population)
    add_comp!(FUND, geography, :geography)
    add_comp!(FUND, socioeconomic, :socioeconomic)
    add_comp!(FUND, emissions, :emissions)
    add_comp!(FUND, climateco2cycle, :climateco2cycle)
    add_comp!(FUND, climatech4cycle, :climatech4cycle)
    add_comp!(FUND, climaten2ocycle, :climaten2ocycle)
    add_comp!(FUND, climatesf6cycle, :climatesf6cycle)
    add_comp!(FUND, climateforcing, :climateforcing)
    add_comp!(FUND, climatedynamics, :climatedynamics)
    add_comp!(FUND, biodiversity, :biodiversity)
    add_comp!(FUND, climateregional, :climateregional)
    add_comp!(FUND, ocean, :ocean)
    add_comp!(FUND, impactagriculture, :impactagriculture)
    add_comp!(FUND, impactbiodiversity, :impactbiodiversity)
    add_comp!(FUND, impactcardiovascularrespiratory, :impactcardiovascularrespiratory)
    add_comp!(FUND, impactcooling, :impactcooling)
    add_comp!(FUND, impactdiarrhoea, :impactdiarrhoea)
    add_comp!(FUND, impactextratropicalstorms, :impactextratropicalstorms)
    add_comp!(FUND, impactforests, :impactforests)
    add_comp!(FUND, impactheating, :impactheating)
    add_comp!(FUND, impactvectorbornediseases, :impactvectorbornediseases)
    add_comp!(FUND, impacttropicalstorms, :impacttropicalstorms)
    add_comp!(FUND, vslvmorb, :vslvmorb)
    add_comp!(FUND, impactdeathmorbidity, :impactdeathmorbidity)
    add_comp!(FUND, impactwaterresources, :impactwaterresources)
    add_comp!(FUND, impactsealevelrise, :impactsealevelrise)
    add_comp!(FUND, impactaggregation, :impactaggregation)

    # ---------------------------------------------
    # Connect parameters to variables
    # ---------------------------------------------

    connect_param!(FUND, :geography, :landloss, :impactsealevelrise, :landloss, offset = 1)

    connect_param!(FUND, :population, :pgrowth, :scenariouncertainty, :pgrowth, offset = 0)
    connect_param!(FUND, :population, :enter, :impactsealevelrise, :enter, offset = 1)
    connect_param!(FUND, :population, :leave, :impactsealevelrise, :leave, offset = 1)
    connect_param!(FUND, :population, :dead, :impactdeathmorbidity, :dead, offset = 1)

    connect_param!(FUND, :socioeconomic, :area, :geography, :area, offset = 0)
    connect_param!(FUND, :socioeconomic, :globalpopulation, :population, :globalpopulation, offset = 0)
    connect_param!(FUND, :socioeconomic, :populationin1, :population, :populationin1, offset = 0)
    connect_param!(FUND, :socioeconomic, :population, :population, :population, offset = 0)
    connect_param!(FUND, :socioeconomic, :pgrowth, :scenariouncertainty, :pgrowth, offset = 0)
    connect_param!(FUND, :socioeconomic, :ypcgrowth, :scenariouncertainty, :ypcgrowth, offset = 0)
    connect_param!(FUND, :socioeconomic, :eloss, :impactaggregation, :eloss, offset = 1)
    connect_param!(FUND, :socioeconomic, :sloss, :impactaggregation, :sloss, offset = 1)
    connect_param!(FUND, :socioeconomic, :mitigationcost, :emissions, :mitigationcost, offset = 1)

    connect_param!(FUND, :emissions, :income, :socioeconomic, :income, offset = 0)
    connect_param!(FUND, :emissions, :population, :population, :population, offset = 0)
    connect_param!(FUND, :emissions, :forestemm, :scenariouncertainty, :forestemm, offset = 0)
    connect_param!(FUND, :emissions, :aeei, :scenariouncertainty, :aeei, offset = 0)
    connect_param!(FUND, :emissions, :acei, :scenariouncertainty, :acei, offset = 0)
    connect_param!(FUND, :emissions, :ypcgrowth, :scenariouncertainty, :ypcgrowth, offset = 0)

    connect_param!(FUND, :climateco2cycle, :mco2, :emissions, :mco2, offset = 0)

    connect_param!(FUND, :climatech4cycle, :globch4, :emissions, :globch4, offset = 0)

    connect_param!(FUND, :climaten2ocycle, :globn2o, :emissions, :globn2o, offset = 0)

    connect_param!(FUND, :climateco2cycle, :temp, :climatedynamics, :temp, offset = 1)

    connect_param!(FUND, :climatesf6cycle, :globsf6, :emissions, :globsf6, offset = 0)

    connect_param!(FUND, :climateforcing, :acco2, :climateco2cycle, :acco2, offset = 0)
    connect_param!(FUND, :climateforcing, :acch4, :climatech4cycle, :acch4, offset = 0)
    connect_param!(FUND, :climateforcing, :acn2o, :climaten2ocycle, :acn2o, offset = 0)
    connect_param!(FUND, :climateforcing, :acsf6, :climatesf6cycle, :acsf6, offset = 0)

    connect_param!(FUND, :climatedynamics, :radforc, :climateforcing, :radforc, offset = 0)

    connect_param!(FUND, :climateregional, :inputtemp, :climatedynamics, :temp, offset = 0)

    connect_param!(FUND, :biodiversity, :temp, :climatedynamics, :temp, offset = 0)

    connect_param!(FUND, :ocean, :temp, :climatedynamics, :temp, offset = 0)

    connect_param!(FUND, :impactagriculture, :population, :population, :population, offset = 0)
    connect_param!(FUND, :impactagriculture, :income, :socioeconomic, :income, offset = 0)
    connect_param!(FUND, :impactagriculture, :temp, :climateregional, :temp, offset = 0)
    connect_param!(FUND, :impactagriculture, :acco2, :climateco2cycle, :acco2, offset = 0)

    connect_param!(FUND, :impactbiodiversity, :temp, :climateregional, :temp, offset = 0)
    connect_param!(FUND, :impactbiodiversity, :nospecies, :biodiversity, :nospecies, offset = 0)
    connect_param!(FUND, :impactbiodiversity, :income, :socioeconomic, :income, offset = 0)
    connect_param!(FUND, :impactbiodiversity, :population, :population, :population, offset = 0)

    connect_param!(FUND, :impactcardiovascularrespiratory, :population, :population, :population, offset = 0)
    connect_param!(FUND, :impactcardiovascularrespiratory, :temp, :climateregional, :temp, offset = 0)
    connect_param!(FUND, :impactcardiovascularrespiratory, :plus, :socioeconomic, :plus, offset = 0)
    connect_param!(FUND, :impactcardiovascularrespiratory, :urbpop, :socioeconomic, :urbpop, offset = 0)

    connect_param!(FUND, :impactcooling, :population, :population, :population, offset = 0)
    connect_param!(FUND, :impactcooling, :income, :socioeconomic, :income, offset = 0)
    connect_param!(FUND, :impactcooling, :temp, :climateregional, :temp, offset = 0)
    connect_param!(FUND, :impactcooling, :cumaeei, :emissions, :cumaeei, offset = 0)

    connect_param!(FUND, :impactdiarrhoea, :population, :population, :population, offset = 0)
    connect_param!(FUND, :impactdiarrhoea, :income, :socioeconomic, :income, offset = 0)
    connect_param!(FUND, :impactdiarrhoea, :regtmp, :climateregional, :regtmp, offset = 0)

    connect_param!(FUND, :impactextratropicalstorms, :population, :population, :population, offset = 0)
    connect_param!(FUND, :impactextratropicalstorms, :income, :socioeconomic, :income, offset = 0)
    connect_param!(FUND, :impactextratropicalstorms, :acco2, :climateco2cycle, :acco2, offset = 0)

    connect_param!(FUND, :impactforests, :population, :population, :population, offset = 0)
    connect_param!(FUND, :impactforests, :income, :socioeconomic, :income, offset = 0)
    connect_param!(FUND, :impactforests, :temp, :climateregional, :temp, offset = 0)
    connect_param!(FUND, :impactforests, :acco2, :climateco2cycle, :acco2, offset = 0)

    connect_param!(FUND, :impactheating, :population, :population, :population, offset = 0)
    connect_param!(FUND, :impactheating, :income, :socioeconomic, :income, offset = 0)
    connect_param!(FUND, :impactheating, :temp, :climateregional, :temp, offset = 0)
    connect_param!(FUND, :impactheating, :cumaeei, :emissions, :cumaeei, offset = 0)

    connect_param!(FUND, :impactvectorbornediseases, :population, :population, :population, offset = 0)
    connect_param!(FUND, :impactvectorbornediseases, :income, :socioeconomic, :income, offset = 0)
    connect_param!(FUND, :impactvectorbornediseases, :temp, :climateregional, :temp, offset = 0)

    connect_param!(FUND, :impacttropicalstorms, :population, :population, :population, offset = 0)
    connect_param!(FUND, :impacttropicalstorms, :income, :socioeconomic, :income, offset = 0)
    connect_param!(FUND, :impacttropicalstorms, :regstmp, :climateregional, :regstmp, offset = 0)

    connect_param!(FUND, :vslvmorb, :population, :population, :population, offset = 0)
    connect_param!(FUND, :vslvmorb, :income, :socioeconomic, :income, offset = 0)

    connect_param!(FUND, :impactdeathmorbidity, :vsl, :vslvmorb, :vsl, offset = 0)
    connect_param!(FUND, :impactdeathmorbidity, :vmorb, :vslvmorb, :vmorb, offset = 0)
    connect_param!(FUND, :impactdeathmorbidity, :population, :population, :population, offset = 0)
    connect_param!(FUND, :impactdeathmorbidity, :dengue, :impactvectorbornediseases, :dengue, offset = 0)
    connect_param!(FUND, :impactdeathmorbidity, :schisto, :impactvectorbornediseases, :schisto, offset = 0)
    connect_param!(FUND, :impactdeathmorbidity, :malaria, :impactvectorbornediseases, :malaria, offset = 0)
    connect_param!(FUND, :impactdeathmorbidity, :cardheat, :impactcardiovascularrespiratory, :cardheat, offset = 0)
    connect_param!(FUND, :impactdeathmorbidity, :cardcold, :impactcardiovascularrespiratory, :cardcold, offset = 0)
    connect_param!(FUND, :impactdeathmorbidity, :resp, :impactcardiovascularrespiratory, :resp, offset = 0)
    connect_param!(FUND, :impactdeathmorbidity, :diadead, :impactdiarrhoea, :diadead, offset = 0)
    connect_param!(FUND, :impactdeathmorbidity, :hurrdead, :impacttropicalstorms, :hurrdead, offset = 0)
    connect_param!(FUND, :impactdeathmorbidity, :extratropicalstormsdead, :impactextratropicalstorms, :extratropicalstormsdead, offset = 0)
    connect_param!(FUND, :impactdeathmorbidity, :diasick, :impactdiarrhoea, :diasick, offset = 0)

    connect_param!(FUND, :impactwaterresources, :population, :population, :population, offset = 0)
    connect_param!(FUND, :impactwaterresources, :income, :socioeconomic, :income, offset = 0)
    connect_param!(FUND, :impactwaterresources, :temp, :climateregional, :temp, offset = 0)

    connect_param!(FUND, :impactsealevelrise, :population, :population, :population, offset = 0)
    connect_param!(FUND, :impactsealevelrise, :income, :socioeconomic, :income, offset = 0)
    connect_param!(FUND, :impactsealevelrise, :sea, :ocean, :sea, offset = 0)
    connect_param!(FUND, :impactsealevelrise, :area, :geography, :area, offset = 0)

    connect_param!(FUND, :impactaggregation, :income, :socioeconomic, :income, offset = 0)
    connect_param!(FUND, :impactaggregation, :heating, :impactheating, :heating, offset = 0)
    connect_param!(FUND, :impactaggregation, :cooling, :impactcooling, :cooling, offset = 0)
    connect_param!(FUND, :impactaggregation, :agcost, :impactagriculture, :agcost, offset = 0)
    connect_param!(FUND, :impactaggregation, :species, :impactbiodiversity, :species, offset = 0)
    connect_param!(FUND, :impactaggregation, :water, :impactwaterresources, :water, offset = 0)
    connect_param!(FUND, :impactaggregation, :hurrdam, :impacttropicalstorms, :hurrdam, offset = 0)
    connect_param!(FUND, :impactaggregation, :extratropicalstormsdam, :impactextratropicalstorms, :extratropicalstormsdam, offset = 0)
    connect_param!(FUND, :impactaggregation, :forests, :impactforests, :forests, offset = 0)
    connect_param!(FUND, :impactaggregation, :drycost, :impactsealevelrise, :drycost, offset = 0)
    connect_param!(FUND, :impactaggregation, :protcost, :impactsealevelrise, :protcost, offset = 0)
    connect_param!(FUND, :impactaggregation, :entercost, :impactsealevelrise, :entercost, offset = 0)
    connect_param!(FUND, :impactaggregation, :deadcost, :impactdeathmorbidity, :deadcost, offset = 0)
    connect_param!(FUND, :impactaggregation, :morbcost, :impactdeathmorbidity, :morbcost, offset = 0)
    connect_param!(FUND, :impactaggregation, :wetcost, :impactsealevelrise, :wetcost, offset = 0)
    connect_param!(FUND, :impactaggregation, :leavecost, :impactsealevelrise, :leavecost, offset = 0)

    add_connector_comps(FUND)

    # ---------------------------------------------
    # Set leftover parameters
    # ---------------------------------------------

    set_leftover_params!(FUND, parameters)

    return FUND

end 

#
# N.B. See fund-defmodel.jl for the @defmodel version of the following
#

FUND = getfund()

end #module
