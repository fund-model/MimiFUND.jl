module fund

using Mimi

include("helper.jl")

include("components/SocioEconomicComponent.jl")
include("components/PopulationComponent.jl")
include("components/EmissionsComponent.jl")
include("components/GeographyComponent.jl")
include("components/ScenarioUncertaintyComponent.jl")
include("components/ClimateCO2CycleComponent.jl")
include("components/ClimateCH4CycleComponent.jl")
include("components/ClimateN2OCycleComponent.jl")
include("components/ClimateSF6CycleComponent.jl")
include("components/ClimateForcingComponent.jl")
include("components/ClimateDynamicsComponent.jl")
include("components/BioDiversityComponent.jl")
include("components/ClimateRegionalComponent.jl")
include("components/OceanComponent.jl")
include("components/ImpactAgricultureComponent.jl")
include("components/ImpactBioDiversityComponent.jl")
include("components/ImpactCardiovascularRespiratoryComponent.jl")
include("components/ImpactCoolingComponent.jl")
include("components/ImpactDiarrhoeaComponent.jl")
include("components/ImpactExtratropicalStormsComponent.jl")
include("components/ImpactDeathMorbidityComponent.jl")
include("components/ImpactForestsComponent.jl")
include("components/ImpactHeatingComponent.jl")
include("components/ImpactVectorBorneDiseasesComponent.jl")
include("components/ImpactTropicalStormsComponent.jl")
include("components/ImpactWaterResourcesComponent.jl")
include("components/ImpactSeaLevelRiseComponent.jl")
include("components/ImpactAggregationComponent.jl")
include("components/VslVmorbComponent.jl")

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
        parameters = load_default_parameters(datadir)
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
    set_dimension!(FUND, :regions, [:USA, :CAN, :WEU, :JPK, :ANZ, :EEU, :FSU, :MDE, :CAM, :LAM, :SAS, :SEA, :CHI, :MAF, :SSA, :SIS])
    # set_dimension!(FUND, :regions, ["USA", "CAN", "WEU", "JPK", "ANZ", "EEU", "FSU", "MDE", "CAM", "LAM", "SAS", "SEA", "CHI", "MAF", "SSA", "SIS"])
    set_dimension!(FUND, :cpools, 1:5)

    # ---------------------------------------------
    # Create components
    # ---------------------------------------------

    addcomponent(FUND, scenariouncertainty, :scenariouncertainty)
    addcomponent(FUND, population, :population)
    addcomponent(FUND, geography, :geography)
    addcomponent(FUND, socioeconomic, :socioeconomic)
    addcomponent(FUND, emissions, :emissions)
    addcomponent(FUND, climateco2cycle, :climateco2cycle)
    addcomponent(FUND, climatech4cycle, :climatech4cycle)
    addcomponent(FUND, climaten2ocycle, :climaten2ocycle)
    addcomponent(FUND, climatesf6cycle, :climatesf6cycle)
    addcomponent(FUND, climateforcing, :climateforcing)
    addcomponent(FUND, climatedynamics, :climatedynamics)
    addcomponent(FUND, biodiversity, :biodiversity)
    addcomponent(FUND, climateregional, :climateregional)
    addcomponent(FUND, ocean, :ocean)
    addcomponent(FUND, impactagriculture, :impactagriculture)
    addcomponent(FUND, impactbiodiversity, :impactbiodiversity)
    addcomponent(FUND, impactcardiovascularrespiratory, :impactcardiovascularrespiratory)
    addcomponent(FUND, impactcooling, :impactcooling)
    addcomponent(FUND, impactdiarrhoea, :impactdiarrhoea)
    addcomponent(FUND, impactextratropicalstorms, :impactextratropicalstorms)
    addcomponent(FUND, impactforests, :impactforests)
    addcomponent(FUND, impactheating, :impactheating)
    addcomponent(FUND, impactvectorbornediseases, :impactvectorbornediseases)
    addcomponent(FUND, impacttropicalstorms, :impacttropicalstorms)
    addcomponent(FUND, vslvmorb, :vslvmorb)
    addcomponent(FUND, impactdeathmorbidity, :impactdeathmorbidity)
    addcomponent(FUND, impactwaterresources, :impactwaterresources)
    addcomponent(FUND, impactsealevelrise, :impactsealevelrise)
    addcomponent(FUND, impactaggregation, :impactaggregation)

    # ---------------------------------------------
    # Connect parameters to variables
    # ---------------------------------------------

    connect_parameter(FUND, :geography, :landloss, :impactsealevelrise, :landloss, offset = 1)

    connect_parameter(FUND, :population, :pgrowth, :scenariouncertainty, :pgrowth, offset = 0)
    connect_parameter(FUND, :population, :enter, :impactsealevelrise, :enter, offset = 1)
    connect_parameter(FUND, :population, :leave, :impactsealevelrise, :leave, offset = 1)
    connect_parameter(FUND, :population, :dead, :impactdeathmorbidity, :dead, offset = 1)

    connect_parameter(FUND, :socioeconomic, :area, :geography, :area, offset = 0)
    connect_parameter(FUND, :socioeconomic, :globalpopulation, :population, :globalpopulation, offset = 0)
    connect_parameter(FUND, :socioeconomic, :populationin1, :population, :populationin1, offset = 0)
    connect_parameter(FUND, :socioeconomic, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :socioeconomic, :pgrowth, :scenariouncertainty, :pgrowth, offset = 0)
    connect_parameter(FUND, :socioeconomic, :ypcgrowth, :scenariouncertainty, :ypcgrowth, offset = 0)
    connect_parameter(FUND, :socioeconomic, :eloss, :impactaggregation, :eloss, offset = 1)
    connect_parameter(FUND, :socioeconomic, :sloss, :impactaggregation, :sloss, offset = 1)
    connect_parameter(FUND, :socioeconomic, :mitigationcost, :emissions, :mitigationcost, offset = 1)

    connect_parameter(FUND, :emissions, :income, :socioeconomic, :income, offset = 0)
    connect_parameter(FUND, :emissions, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :emissions, :forestemm, :scenariouncertainty, :forestemm, offset = 0)
    connect_parameter(FUND, :emissions, :aeei, :scenariouncertainty, :aeei, offset = 0)
    connect_parameter(FUND, :emissions, :acei, :scenariouncertainty, :acei, offset = 0)
    connect_parameter(FUND, :emissions, :ypcgrowth, :scenariouncertainty, :ypcgrowth, offset = 0)

    connect_parameter(FUND, :climateco2cycle, :mco2, :emissions, :mco2, offset = 0)

    connect_parameter(FUND, :climatech4cycle, :globch4, :emissions, :globch4, offset = 0)

    connect_parameter(FUND, :climaten2ocycle, :globn2o, :emissions, :globn2o, offset = 0)

    connect_parameter(FUND, :climateco2cycle, :temp, :climatedynamics, :temp, offset = 1)

    connect_parameter(FUND, :climatesf6cycle, :globsf6, :emissions, :globsf6, offset = 0)

    connect_parameter(FUND, :climateforcing, :acco2, :climateco2cycle, :acco2, offset = 0)
    connect_parameter(FUND, :climateforcing, :acch4, :climatech4cycle, :acch4, offset = 0)
    connect_parameter(FUND, :climateforcing, :acn2o, :climaten2ocycle, :acn2o, offset = 0)
    connect_parameter(FUND, :climateforcing, :acsf6, :climatesf6cycle, :acsf6, offset = 0)

    connect_parameter(FUND, :climatedynamics, :radforc, :climateforcing, :radforc, offset = 0)

    connect_parameter(FUND, :climateregional, :inputtemp, :climatedynamics, :temp, offset = 0)

    connect_parameter(FUND, :biodiversity, :temp, :climatedynamics, :temp, offset = 0)

    connect_parameter(FUND, :ocean, :temp, :climatedynamics, :temp, offset = 0)

    connect_parameter(FUND, :impactagriculture, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :impactagriculture, :income, :socioeconomic, :income, offset = 0)
    connect_parameter(FUND, :impactagriculture, :temp, :climateregional, :temp, offset = 0)
    connect_parameter(FUND, :impactagriculture, :acco2, :climateco2cycle, :acco2, offset = 0)

    connect_parameter(FUND, :impactbiodiversity, :temp, :climateregional, :temp, offset = 0)
    connect_parameter(FUND, :impactbiodiversity, :nospecies, :biodiversity, :nospecies, offset = 0)
    connect_parameter(FUND, :impactbiodiversity, :income, :socioeconomic, :income, offset = 0)
    connect_parameter(FUND, :impactbiodiversity, :population, :population, :population, offset = 0)

    connect_parameter(FUND, :impactcardiovascularrespiratory, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :impactcardiovascularrespiratory, :temp, :climateregional, :temp, offset = 0)
    connect_parameter(FUND, :impactcardiovascularrespiratory, :plus, :socioeconomic, :plus, offset = 0)
    connect_parameter(FUND, :impactcardiovascularrespiratory, :urbpop, :socioeconomic, :urbpop, offset = 0)

    connect_parameter(FUND, :impactcooling, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :impactcooling, :income, :socioeconomic, :income, offset = 0)
    connect_parameter(FUND, :impactcooling, :temp, :climateregional, :temp, offset = 0)
    connect_parameter(FUND, :impactcooling, :cumaeei, :emissions, :cumaeei, offset = 0)

    connect_parameter(FUND, :impactdiarrhoea, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :impactdiarrhoea, :income, :socioeconomic, :income, offset = 0)
    connect_parameter(FUND, :impactdiarrhoea, :regtmp, :climateregional, :regtmp, offset = 0)

    connect_parameter(FUND, :impactextratropicalstorms, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :impactextratropicalstorms, :income, :socioeconomic, :income, offset = 0)
    connect_parameter(FUND, :impactextratropicalstorms, :acco2, :climateco2cycle, :acco2, offset = 0)

    connect_parameter(FUND, :impactforests, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :impactforests, :income, :socioeconomic, :income, offset = 0)
    connect_parameter(FUND, :impactforests, :temp, :climateregional, :temp, offset = 0)
    connect_parameter(FUND, :impactforests, :acco2, :climateco2cycle, :acco2, offset = 0)

    connect_parameter(FUND, :impactheating, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :impactheating, :income, :socioeconomic, :income, offset = 0)
    connect_parameter(FUND, :impactheating, :temp, :climateregional, :temp, offset = 0)
    connect_parameter(FUND, :impactheating, :cumaeei, :emissions, :cumaeei, offset = 0)

    connect_parameter(FUND, :impactvectorbornediseases, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :impactvectorbornediseases, :income, :socioeconomic, :income, offset = 0)
    connect_parameter(FUND, :impactvectorbornediseases, :temp, :climateregional, :temp, offset = 0)

    connect_parameter(FUND, :impacttropicalstorms, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :impacttropicalstorms, :income, :socioeconomic, :income, offset = 0)
    connect_parameter(FUND, :impacttropicalstorms, :regstmp, :climateregional, :regstmp, offset = 0)

    connect_parameter(FUND, :vslvmorb, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :vslvmorb, :income, :socioeconomic, :income, offset = 0)

    connect_parameter(FUND, :impactdeathmorbidity, :vsl, :vslvmorb, :vsl, offset = 0)
    connect_parameter(FUND, :impactdeathmorbidity, :vmorb, :vslvmorb, :vmorb, offset = 0)
    connect_parameter(FUND, :impactdeathmorbidity, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :impactdeathmorbidity, :dengue, :impactvectorbornediseases, :dengue, offset = 0)
    connect_parameter(FUND, :impactdeathmorbidity, :schisto, :impactvectorbornediseases, :schisto, offset = 0)
    connect_parameter(FUND, :impactdeathmorbidity, :malaria, :impactvectorbornediseases, :malaria, offset = 0)
    connect_parameter(FUND, :impactdeathmorbidity, :cardheat, :impactcardiovascularrespiratory, :cardheat, offset = 0)
    connect_parameter(FUND, :impactdeathmorbidity, :cardcold, :impactcardiovascularrespiratory, :cardcold, offset = 0)
    connect_parameter(FUND, :impactdeathmorbidity, :resp, :impactcardiovascularrespiratory, :resp, offset = 0)
    connect_parameter(FUND, :impactdeathmorbidity, :diadead, :impactdiarrhoea, :diadead, offset = 0)
    connect_parameter(FUND, :impactdeathmorbidity, :hurrdead, :impacttropicalstorms, :hurrdead, offset = 0)
    connect_parameter(FUND, :impactdeathmorbidity, :extratropicalstormsdead, :impactextratropicalstorms, :extratropicalstormsdead, offset = 0)
    connect_parameter(FUND, :impactdeathmorbidity, :diasick, :impactdiarrhoea, :diasick, offset = 0)

    connect_parameter(FUND, :impactwaterresources, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :impactwaterresources, :income, :socioeconomic, :income, offset = 0)
    connect_parameter(FUND, :impactwaterresources, :temp, :climateregional, :temp, offset = 0)

    connect_parameter(FUND, :impactsealevelrise, :population, :population, :population, offset = 0)
    connect_parameter(FUND, :impactsealevelrise, :income, :socioeconomic, :income, offset = 0)
    connect_parameter(FUND, :impactsealevelrise, :sea, :ocean, :sea, offset = 0)
    connect_parameter(FUND, :impactsealevelrise, :area, :geography, :area, offset = 0)

    connect_parameter(FUND, :impactaggregation, :income, :socioeconomic, :income, offset = 0)
    connect_parameter(FUND, :impactaggregation, :heating, :impactheating, :heating, offset = 0)
    connect_parameter(FUND, :impactaggregation, :cooling, :impactcooling, :cooling, offset = 0)
    connect_parameter(FUND, :impactaggregation, :agcost, :impactagriculture, :agcost, offset = 0)
    connect_parameter(FUND, :impactaggregation, :species, :impactbiodiversity, :species, offset = 0)
    connect_parameter(FUND, :impactaggregation, :water, :impactwaterresources, :water, offset = 0)
    connect_parameter(FUND, :impactaggregation, :hurrdam, :impacttropicalstorms, :hurrdam, offset = 0)
    connect_parameter(FUND, :impactaggregation, :extratropicalstormsdam, :impactextratropicalstorms, :extratropicalstormsdam, offset = 0)
    connect_parameter(FUND, :impactaggregation, :forests, :impactforests, :forests, offset = 0)
    connect_parameter(FUND, :impactaggregation, :drycost, :impactsealevelrise, :drycost, offset = 0)
    connect_parameter(FUND, :impactaggregation, :protcost, :impactsealevelrise, :protcost, offset = 0)
    connect_parameter(FUND, :impactaggregation, :entercost, :impactsealevelrise, :entercost, offset = 0)
    connect_parameter(FUND, :impactaggregation, :deadcost, :impactdeathmorbidity, :deadcost, offset = 0)
    connect_parameter(FUND, :impactaggregation, :morbcost, :impactdeathmorbidity, :morbcost, offset = 0)
    connect_parameter(FUND, :impactaggregation, :wetcost, :impactsealevelrise, :wetcost, offset = 0)
    connect_parameter(FUND, :impactaggregation, :leavecost, :impactsealevelrise, :leavecost, offset = 0)

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
