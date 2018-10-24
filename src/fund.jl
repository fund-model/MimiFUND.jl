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

    connect_param!(FUND, :geography, :landloss, :impactsealevelrise, :landloss)

    connect_param!(FUND, :population, :pgrowth, :scenariouncertainty, :pgrowth)
    connect_param!(FUND, :population, :enter, :impactsealevelrise, :enter)
    connect_param!(FUND, :population, :leave, :impactsealevelrise, :leave)
    connect_param!(FUND, :population, :dead, :impactdeathmorbidity, :dead)

    connect_param!(FUND, :socioeconomic, :area, :geography, :area)
    connect_param!(FUND, :socioeconomic, :globalpopulation, :population, :globalpopulation)
    connect_param!(FUND, :socioeconomic, :populationin1, :population, :populationin1)
    connect_param!(FUND, :socioeconomic, :population, :population, :population)
    connect_param!(FUND, :socioeconomic, :pgrowth, :scenariouncertainty, :pgrowth)
    connect_param!(FUND, :socioeconomic, :ypcgrowth, :scenariouncertainty, :ypcgrowth)
    connect_param!(FUND, :socioeconomic, :eloss, :impactaggregation, :eloss)
    connect_param!(FUND, :socioeconomic, :sloss, :impactaggregation, :sloss)
    connect_param!(FUND, :socioeconomic, :mitigationcost, :emissions, :mitigationcost)

    connect_param!(FUND, :emissions, :income, :socioeconomic, :income)
    connect_param!(FUND, :emissions, :population, :population, :population)
    connect_param!(FUND, :emissions, :forestemm, :scenariouncertainty, :forestemm)
    connect_param!(FUND, :emissions, :aeei, :scenariouncertainty, :aeei)
    connect_param!(FUND, :emissions, :acei, :scenariouncertainty, :acei)
    connect_param!(FUND, :emissions, :ypcgrowth, :scenariouncertainty, :ypcgrowth)

    connect_param!(FUND, :climateco2cycle, :mco2, :emissions, :mco2)

    connect_param!(FUND, :climatech4cycle, :globch4, :emissions, :globch4)

    connect_param!(FUND, :climaten2ocycle, :globn2o, :emissions, :globn2o)

    connect_param!(FUND, :climateco2cycle, :temp, :climatedynamics, :temp)

    connect_param!(FUND, :climatesf6cycle, :globsf6, :emissions, :globsf6)

    connect_param!(FUND, :climateforcing, :acco2, :climateco2cycle, :acco2)
    connect_param!(FUND, :climateforcing, :acch4, :climatech4cycle, :acch4)
    connect_param!(FUND, :climateforcing, :acn2o, :climaten2ocycle, :acn2o)
    connect_param!(FUND, :climateforcing, :acsf6, :climatesf6cycle, :acsf6)

    connect_param!(FUND, :climatedynamics, :radforc, :climateforcing, :radforc)

    connect_param!(FUND, :climateregional, :inputtemp, :climatedynamics, :temp)

    connect_param!(FUND, :biodiversity, :temp, :climatedynamics, :temp)

    connect_param!(FUND, :ocean, :temp, :climatedynamics, :temp)

    connect_param!(FUND, :impactagriculture, :population, :population, :population)
    connect_param!(FUND, :impactagriculture, :income, :socioeconomic, :income)
    connect_param!(FUND, :impactagriculture, :temp, :climateregional, :temp)
    connect_param!(FUND, :impactagriculture, :acco2, :climateco2cycle, :acco2)

    connect_param!(FUND, :impactbiodiversity, :temp, :climateregional, :temp)
    connect_param!(FUND, :impactbiodiversity, :nospecies, :biodiversity, :nospecies)
    connect_param!(FUND, :impactbiodiversity, :income, :socioeconomic, :income)
    connect_param!(FUND, :impactbiodiversity, :population, :population, :population)

    connect_param!(FUND, :impactcardiovascularrespiratory, :population, :population, :population)
    connect_param!(FUND, :impactcardiovascularrespiratory, :temp, :climateregional, :temp)
    connect_param!(FUND, :impactcardiovascularrespiratory, :plus, :socioeconomic, :plus)
    connect_param!(FUND, :impactcardiovascularrespiratory, :urbpop, :socioeconomic, :urbpop)

    connect_param!(FUND, :impactcooling, :population, :population, :population)
    connect_param!(FUND, :impactcooling, :income, :socioeconomic, :income)
    connect_param!(FUND, :impactcooling, :temp, :climateregional, :temp)
    connect_param!(FUND, :impactcooling, :cumaeei, :emissions, :cumaeei)

    connect_param!(FUND, :impactdiarrhoea, :population, :population, :population)
    connect_param!(FUND, :impactdiarrhoea, :income, :socioeconomic, :income)
    connect_param!(FUND, :impactdiarrhoea, :regtmp, :climateregional, :regtmp)

    connect_param!(FUND, :impactextratropicalstorms, :population, :population, :population)
    connect_param!(FUND, :impactextratropicalstorms, :income, :socioeconomic, :income)
    connect_param!(FUND, :impactextratropicalstorms, :acco2, :climateco2cycle, :acco2)

    connect_param!(FUND, :impactforests, :population, :population, :population)
    connect_param!(FUND, :impactforests, :income, :socioeconomic, :income)
    connect_param!(FUND, :impactforests, :temp, :climateregional, :temp)
    connect_param!(FUND, :impactforests, :acco2, :climateco2cycle, :acco2)

    connect_param!(FUND, :impactheating, :population, :population, :population)
    connect_param!(FUND, :impactheating, :income, :socioeconomic, :income)
    connect_param!(FUND, :impactheating, :temp, :climateregional, :temp)
    connect_param!(FUND, :impactheating, :cumaeei, :emissions, :cumaeei)

    connect_param!(FUND, :impactvectorbornediseases, :population, :population, :population)
    connect_param!(FUND, :impactvectorbornediseases, :income, :socioeconomic, :income)
    connect_param!(FUND, :impactvectorbornediseases, :temp, :climateregional, :temp)

    connect_param!(FUND, :impacttropicalstorms, :population, :population, :population)
    connect_param!(FUND, :impacttropicalstorms, :income, :socioeconomic, :income)
    connect_param!(FUND, :impacttropicalstorms, :regstmp, :climateregional, :regstmp)

    connect_param!(FUND, :vslvmorb, :population, :population, :population)
    connect_param!(FUND, :vslvmorb, :income, :socioeconomic, :income)

    connect_param!(FUND, :impactdeathmorbidity, :vsl, :vslvmorb, :vsl)
    connect_param!(FUND, :impactdeathmorbidity, :vmorb, :vslvmorb, :vmorb)
    connect_param!(FUND, :impactdeathmorbidity, :population, :population, :population)
    connect_param!(FUND, :impactdeathmorbidity, :dengue, :impactvectorbornediseases, :dengue)
    connect_param!(FUND, :impactdeathmorbidity, :schisto, :impactvectorbornediseases, :schisto)
    connect_param!(FUND, :impactdeathmorbidity, :malaria, :impactvectorbornediseases, :malaria)
    connect_param!(FUND, :impactdeathmorbidity, :cardheat, :impactcardiovascularrespiratory, :cardheat)
    connect_param!(FUND, :impactdeathmorbidity, :cardcold, :impactcardiovascularrespiratory, :cardcold)
    connect_param!(FUND, :impactdeathmorbidity, :resp, :impactcardiovascularrespiratory, :resp)
    connect_param!(FUND, :impactdeathmorbidity, :diadead, :impactdiarrhoea, :diadead)
    connect_param!(FUND, :impactdeathmorbidity, :hurrdead, :impacttropicalstorms, :hurrdead)
    connect_param!(FUND, :impactdeathmorbidity, :extratropicalstormsdead, :impactextratropicalstorms, :extratropicalstormsdead)
    connect_param!(FUND, :impactdeathmorbidity, :diasick, :impactdiarrhoea, :diasick)

    connect_param!(FUND, :impactwaterresources, :population, :population, :population)
    connect_param!(FUND, :impactwaterresources, :income, :socioeconomic, :income)
    connect_param!(FUND, :impactwaterresources, :temp, :climateregional, :temp)

    connect_param!(FUND, :impactsealevelrise, :population, :population, :population)
    connect_param!(FUND, :impactsealevelrise, :income, :socioeconomic, :income)
    connect_param!(FUND, :impactsealevelrise, :sea, :ocean, :sea)
    connect_param!(FUND, :impactsealevelrise, :area, :geography, :area)

    connect_param!(FUND, :impactaggregation, :income, :socioeconomic, :income)
    connect_param!(FUND, :impactaggregation, :heating, :impactheating, :heating)
    connect_param!(FUND, :impactaggregation, :cooling, :impactcooling, :cooling)
    connect_param!(FUND, :impactaggregation, :agcost, :impactagriculture, :agcost)
    connect_param!(FUND, :impactaggregation, :species, :impactbiodiversity, :species)
    connect_param!(FUND, :impactaggregation, :water, :impactwaterresources, :water)
    connect_param!(FUND, :impactaggregation, :hurrdam, :impacttropicalstorms, :hurrdam)
    connect_param!(FUND, :impactaggregation, :extratropicalstormsdam, :impactextratropicalstorms, :extratropicalstormsdam)
    connect_param!(FUND, :impactaggregation, :forests, :impactforests, :forests)
    connect_param!(FUND, :impactaggregation, :drycost, :impactsealevelrise, :drycost)
    connect_param!(FUND, :impactaggregation, :protcost, :impactsealevelrise, :protcost)
    connect_param!(FUND, :impactaggregation, :entercost, :impactsealevelrise, :entercost)
    connect_param!(FUND, :impactaggregation, :deadcost, :impactdeathmorbidity, :deadcost)
    connect_param!(FUND, :impactaggregation, :morbcost, :impactdeathmorbidity, :morbcost)
    connect_param!(FUND, :impactaggregation, :wetcost, :impactsealevelrise, :wetcost)
    connect_param!(FUND, :impactaggregation, :leavecost, :impactsealevelrise, :leavecost)

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
