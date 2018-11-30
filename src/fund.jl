module Fund

using Mimi
using DelimitedFiles #base.DelimitedFiles

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

    connect_param!(m, :geography, :landloss, :impactsealevelrise, :landloss)

    connect_param!(m, :population, :pgrowth, :scenariouncertainty, :pgrowth)
    connect_param!(m, :population, :enter, :impactsealevelrise, :enter)
    connect_param!(m, :population, :leave, :impactsealevelrise, :leave)
    connect_param!(m, :population, :dead, :impactdeathmorbidity, :dead)

    connect_param!(m, :socioeconomic, :area, :geography, :area)
    connect_param!(m, :socioeconomic, :globalpopulation, :population, :globalpopulation)
    connect_param!(m, :socioeconomic, :populationin1, :population, :populationin1)
    connect_param!(m, :socioeconomic, :population, :population, :population)
    connect_param!(m, :socioeconomic, :pgrowth, :scenariouncertainty, :pgrowth)
    connect_param!(m, :socioeconomic, :ypcgrowth, :scenariouncertainty, :ypcgrowth)
    connect_param!(m, :socioeconomic, :eloss, :impactaggregation, :eloss)
    connect_param!(m, :socioeconomic, :sloss, :impactaggregation, :sloss)
    connect_param!(m, :socioeconomic, :mitigationcost, :emissions, :mitigationcost)

    connect_param!(m, :emissions, :income, :socioeconomic, :income)
    connect_param!(m, :emissions, :population, :population, :population)
    connect_param!(m, :emissions, :forestemm, :scenariouncertainty, :forestemm)
    connect_param!(m, :emissions, :aeei, :scenariouncertainty, :aeei)
    connect_param!(m, :emissions, :acei, :scenariouncertainty, :acei)
    connect_param!(m, :emissions, :ypcgrowth, :scenariouncertainty, :ypcgrowth)

    connect_param!(m, :climateco2cycle, :mco2, :emissions, :mco2)

    connect_param!(m, :climatech4cycle, :globch4, :emissions, :globch4)

    connect_param!(m, :climaten2ocycle, :globn2o, :emissions, :globn2o)

    connect_param!(m, :climateco2cycle, :temp, :climatedynamics, :temp)

    connect_param!(m, :climatesf6cycle, :globsf6, :emissions, :globsf6)

    connect_param!(m, :climateforcing, :acco2, :climateco2cycle, :acco2)
    connect_param!(m, :climateforcing, :acch4, :climatech4cycle, :acch4)
    connect_param!(m, :climateforcing, :acn2o, :climaten2ocycle, :acn2o)
    connect_param!(m, :climateforcing, :acsf6, :climatesf6cycle, :acsf6)

    connect_param!(m, :climatedynamics, :radforc, :climateforcing, :radforc)

    connect_param!(m, :climateregional, :inputtemp, :climatedynamics, :temp)

    connect_param!(m, :biodiversity, :temp, :climatedynamics, :temp)

    connect_param!(m, :ocean, :temp, :climatedynamics, :temp)

    connect_param!(m, :impactagriculture, :population, :population, :population)
    connect_param!(m, :impactagriculture, :income, :socioeconomic, :income)
    connect_param!(m, :impactagriculture, :temp, :climateregional, :temp)
    connect_param!(m, :impactagriculture, :acco2, :climateco2cycle, :acco2)

    connect_param!(m, :impactbiodiversity, :temp, :climateregional, :temp)
    connect_param!(m, :impactbiodiversity, :nospecies, :biodiversity, :nospecies)
    connect_param!(m, :impactbiodiversity, :income, :socioeconomic, :income)
    connect_param!(m, :impactbiodiversity, :population, :population, :population)

    connect_param!(m, :impactcardiovascularrespiratory, :population, :population, :population)
    connect_param!(m, :impactcardiovascularrespiratory, :temp, :climateregional, :temp)
    connect_param!(m, :impactcardiovascularrespiratory, :plus, :socioeconomic, :plus)
    connect_param!(m, :impactcardiovascularrespiratory, :urbpop, :socioeconomic, :urbpop)

    connect_param!(m, :impactcooling, :population, :population, :population)
    connect_param!(m, :impactcooling, :income, :socioeconomic, :income)
    connect_param!(m, :impactcooling, :temp, :climateregional, :temp)
    connect_param!(m, :impactcooling, :cumaeei, :emissions, :cumaeei)

    connect_param!(m, :impactdiarrhoea, :population, :population, :population)
    connect_param!(m, :impactdiarrhoea, :income, :socioeconomic, :income)
    connect_param!(m, :impactdiarrhoea, :regtmp, :climateregional, :regtmp)

    connect_param!(m, :impactextratropicalstorms, :population, :population, :population)
    connect_param!(m, :impactextratropicalstorms, :income, :socioeconomic, :income)
    connect_param!(m, :impactextratropicalstorms, :acco2, :climateco2cycle, :acco2)

    connect_param!(m, :impactforests, :population, :population, :population)
    connect_param!(m, :impactforests, :income, :socioeconomic, :income)
    connect_param!(m, :impactforests, :temp, :climateregional, :temp)
    connect_param!(m, :impactforests, :acco2, :climateco2cycle, :acco2)

    connect_param!(m, :impactheating, :population, :population, :population)
    connect_param!(m, :impactheating, :income, :socioeconomic, :income)
    connect_param!(m, :impactheating, :temp, :climateregional, :temp)
    connect_param!(m, :impactheating, :cumaeei, :emissions, :cumaeei)

    connect_param!(m, :impactvectorbornediseases, :population, :population, :population)
    connect_param!(m, :impactvectorbornediseases, :income, :socioeconomic, :income)
    connect_param!(m, :impactvectorbornediseases, :temp, :climateregional, :temp)

    connect_param!(m, :impacttropicalstorms, :population, :population, :population)
    connect_param!(m, :impacttropicalstorms, :income, :socioeconomic, :income)
    connect_param!(m, :impacttropicalstorms, :regstmp, :climateregional, :regstmp)

    connect_param!(m, :vslvmorb, :population, :population, :population)
    connect_param!(m, :vslvmorb, :income, :socioeconomic, :income)

    connect_param!(m, :impactdeathmorbidity, :vsl, :vslvmorb, :vsl)
    connect_param!(m, :impactdeathmorbidity, :vmorb, :vslvmorb, :vmorb)
    connect_param!(m, :impactdeathmorbidity, :population, :population, :population)
    connect_param!(m, :impactdeathmorbidity, :dengue, :impactvectorbornediseases, :dengue)
    connect_param!(m, :impactdeathmorbidity, :schisto, :impactvectorbornediseases, :schisto)
    connect_param!(m, :impactdeathmorbidity, :malaria, :impactvectorbornediseases, :malaria)
    connect_param!(m, :impactdeathmorbidity, :cardheat, :impactcardiovascularrespiratory, :cardheat)
    connect_param!(m, :impactdeathmorbidity, :cardcold, :impactcardiovascularrespiratory, :cardcold)
    connect_param!(m, :impactdeathmorbidity, :resp, :impactcardiovascularrespiratory, :resp)
    connect_param!(m, :impactdeathmorbidity, :diadead, :impactdiarrhoea, :diadead)
    connect_param!(m, :impactdeathmorbidity, :hurrdead, :impacttropicalstorms, :hurrdead)
    connect_param!(m, :impactdeathmorbidity, :extratropicalstormsdead, :impactextratropicalstorms, :extratropicalstormsdead)
    connect_param!(m, :impactdeathmorbidity, :diasick, :impactdiarrhoea, :diasick)

    connect_param!(m, :impactwaterresources, :population, :population, :population)
    connect_param!(m, :impactwaterresources, :income, :socioeconomic, :income)
    connect_param!(m, :impactwaterresources, :temp, :climateregional, :temp)

    connect_param!(m, :impactsealevelrise, :population, :population, :population)
    connect_param!(m, :impactsealevelrise, :income, :socioeconomic, :income)
    connect_param!(m, :impactsealevelrise, :sea, :ocean, :sea)
    connect_param!(m, :impactsealevelrise, :area, :geography, :area)

    connect_param!(m, :impactaggregation, :income, :socioeconomic, :income)
    connect_param!(m, :impactaggregation, :heating, :impactheating, :heating)
    connect_param!(m, :impactaggregation, :cooling, :impactcooling, :cooling)
    connect_param!(m, :impactaggregation, :agcost, :impactagriculture, :agcost)
    connect_param!(m, :impactaggregation, :species, :impactbiodiversity, :species)
    connect_param!(m, :impactaggregation, :water, :impactwaterresources, :water)
    connect_param!(m, :impactaggregation, :hurrdam, :impacttropicalstorms, :hurrdam)
    connect_param!(m, :impactaggregation, :extratropicalstormsdam, :impactextratropicalstorms, :extratropicalstormsdam)
    connect_param!(m, :impactaggregation, :forests, :impactforests, :forests)
    connect_param!(m, :impactaggregation, :drycost, :impactsealevelrise, :drycost)
    connect_param!(m, :impactaggregation, :protcost, :impactsealevelrise, :protcost)
    connect_param!(m, :impactaggregation, :entercost, :impactsealevelrise, :entercost)
    connect_param!(m, :impactaggregation, :deadcost, :impactdeathmorbidity, :deadcost)
    connect_param!(m, :impactaggregation, :morbcost, :impactdeathmorbidity, :morbcost)
    connect_param!(m, :impactaggregation, :wetcost, :impactsealevelrise, :wetcost)
    connect_param!(m, :impactaggregation, :leavecost, :impactsealevelrise, :leavecost)

    # ---------------------------------------------
    # Set leftover parameters
    # ---------------------------------------------

    set_leftover_params!(m, parameters)

    return m

end 

end #module
