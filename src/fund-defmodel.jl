module fund

using Mimi
import Mimi.@defmodel

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

export FUND

const global nsteps = 1050
const global datadir = joinpath(dirname(@__FILE__), "..", "data")
const global params = nothing

@defmodel FUND begin 

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
    # Set indexes 
    # ---------------------------------------------

    # index[time]     = collect(1950:1950+nsteps)
    index[time]     = collect(1950:3000)
    index[regions]  = ["USA", "CAN", "WEU", "JPK", "ANZ", "EEU", "FSU", "MDE", "CAM", "LAM", "SAS", "SEA", "CHI", "MAF", "SSA", "SIS"]
    index[cpools]   = 1:5

    # ---------------------------------------------
    # Create components
    # ---------------------------------------------
    component(scenariouncertainty)
    component(population)
    component(geography)
    component(socioeconomic)
    component(emissions)
    component(climateco2cycle)
    component(climatech4cycle)
    component(climaten2ocycle)
    component(climatesf6cycle)
    component(climateforcing)
    component(climatedynamics)
    component(biodiversity)
    component(climateregional)
    component(ocean)
    component(impactagriculture)
    component(impactbiodiversity)
    component(impactcardiovascularrespiratory)
    component(impactcooling)
    component(impactdiarrhoea)
    component(impactextratropicalstorms)
    component(impactforests)
    component(impactheating)
    component(impactvectorbornediseases)
    component(impacttropicalstorms)
    component(vslvmorb)
    component(impactdeathmorbidity)
    component(impactwaterresources)
    component(impactsealevelrise)
    component(impactaggregation)

    # ---------------------------------------------
    # Connect parameters to variables
    # ---------------------------------------------

    impactsealevelrise.landloss[t-1] => geography.landloss

    scenariouncertainty.pgrowth     => population.pgrowth
    impactsealevelrise.enter[t-1]   => population.enter
    impactsealevelrise.leave[t-1]   => population.leave
    impactdeathmorbidity.dead[t-1]  => population.dead

    geography.area                  => socioeconomic.area
    population.globalpopulation     => socioeconomic.globalpopulation
    population.populationin1        => socioeconomic.populationin1
    population.population           => socioeconomic.population
    scenariouncertainty.pgrowth     => socioeconomic.pgrowth
    scenariouncertainty.ypcgrowth   => socioeconomic.ypcgrowth
    impactaggregation.eloss[t-1]    => socioeconomic.eloss
    impactaggregation.sloss[t-1]    => socioeconomic.sloss
    emissions.mitigationcost[t-1]   => socioeconomic.mitigationcost

    socioeconomic.income            => emissions.income
    population.population           => emissions.population
    scenariouncertainty.forestemm   => emissions.forestemm
    scenariouncertainty.aeei        => emissions.aeei
    scenariouncertainty.acei        => emissions.acei
    scenariouncertainty.ypcgrowth   => emissions.ypcgrowth

    emissions.mco2 => climateco2cycle.mco2

    emissions.globch4 => climatech4cycle.globch4

    emissions.globn2o => climaten2ocycle.globn2o

    climatedynamics.temp[t-1] => climateco2cycle.temp

    emissions.globsf6 => climatesf6cycle.globsf6

    climateco2cycle.acco2 => climateforcing.acco2
    climatech4cycle.acch4 => climateforcing.acch4
    climaten2ocycle.acn2o => climateforcing.acn2o
    climatesf6cycle.acsf6 => climateforcing.acsf6

    climateforcing.radforc => climatedynamics.radforc

    climatedynamics.temp => climateregional.inputtemp

    climatedynamics.temp => biodiversity.temp

    climatedynamics.temp => ocean.temp

    population.population   => impactagriculture.population
    socioeconomic.income    => impactagriculture.income
    climateregional.temp    => impactagriculture.temp
    climateco2cycle.acco2   => impactagriculture.acco2

    climateregional.temp    => impactbiodiversity.temp
    biodiversity.nospecies  => impactbiodiversity.nospecies
    socioeconomic.income    => impactbiodiversity.income
    population.population   => impactbiodiversity.population

    population.population   => impactcardiovascularrespiratory.population
    climateregional.temp    => impactcardiovascularrespiratory.temp
    socioeconomic.plus      => impactcardiovascularrespiratory.plus
    socioeconomic.urbpop    => impactcardiovascularrespiratory.urbpop

    population.population   => impactcooling.population
    socioeconomic.income    => impactcooling.income
    climateregional.temp    => impactcooling.temp
    emissions.cumaeei       => impactcooling.cumaeei

    population.population   => impactdiarrhoea.population
    socioeconomic.income    => impactdiarrhoea.income
    climateregional.regtmp  => impactdiarrhoea.regtmp

    population.population   => impactextratropicalstorms.population
    socioeconomic.income    => impactextratropicalstorms.income
    climateco2cycle.acco2   => impactextratropicalstorms.acco2

    population.population   => impactforests.population
    socioeconomic.income    => impactforests.income
    climateregional.temp    => impactforests.temp
    climateco2cycle.acco2   => impactforests.acco2

    population.population   => impactheating.population
    socioeconomic.income    => impactheating.income
    climateregional.temp    => impactheating.temp
    emissions.cumaeei       => impactheating.cumaeei

    population.population   => impactvectorbornediseases.population
    socioeconomic.income    => impactvectorbornediseases.income
    climateregional.temp    => impactvectorbornediseases.temp

    population.population   => impacttropicalstorms.population
    socioeconomic.income    => impacttropicalstorms.income
    climateregional.regstmp => impacttropicalstorms.regstmp

    population.population   => vslvmorb.population
    socioeconomic.income    => vslvmorb.income

    vslvmorb.vsl                        => impactdeathmorbidity.vsl
    vslvmorb.vmorb                      => impactdeathmorbidity.vmorb
    population.population               => impactdeathmorbidity.population
    impactvectorbornediseases.dengue    => impactdeathmorbidity.dengue
    impactvectorbornediseases.schisto   => impactdeathmorbidity.schisto
    impactvectorbornediseases.malaria   => impactdeathmorbidity.malaria
    impactcardiovascularrespiratory.cardheat    => impactdeathmorbidity.cardheat
    impactcardiovascularrespiratory.cardcold    => impactdeathmorbidity.cardcold
    impactcardiovascularrespiratory.resp        => impactdeathmorbidity.resp
    impactdiarrhoea.diadead             => impactdeathmorbidity.diadead
    impacttropicalstorms.hurrdead       => impactdeathmorbidity.hurrdead
    impactextratropicalstorms.extratropicalstormsdead => impactdeathmorbidity.extratropicalstormsdead
    impactdiarrhoea.diasick             => impactdeathmorbidity.diasick
    
    population.population   => impactwaterresources.population
    socioeconomic.income    => impactwaterresources.income
    climateregional.temp    => impactwaterresources.temp

    population.population   => impactsealevelrise.population
    socioeconomic.income    => impactsealevelrise.income
    ocean.sea               => impactsealevelrise.sea
    geography.area          => impactsealevelrise.area

    socioeconomic.income            => impactaggregation.income
    impactheating.heating           => impactaggregation.heating
    impactcooling.cooling           => impactaggregation.cooling
    impactagriculture.agcost        => impactaggregation.agcost
    impactbiodiversity.species      => impactaggregation.species
    impactwaterresources.water      => impactaggregation.water
    impacttropicalstorms.hurrdam    => impactaggregation.hurrdam
    impactextratropicalstorms.extratropicalstormsdam => impactaggregation.extratropicalstormsdam
    impactforests.forests           => impactaggregation.forests
    impactsealevelrise.drycost      => impactaggregation.drycost
    impactsealevelrise.protcost     => impactaggregation.protcost
    impactsealevelrise.entercost    => impactaggregation.entercost
    impactdeathmorbidity.deadcost   => impactaggregation.deadcost
    impactdeathmorbidity.morbcost   => impactaggregation.morbcost
    impactsealevelrise.wetcost      => impactaggregation.wetcost
    impactsealevelrise.leavecost    => impactaggregation.leavecost
        
    # ---------------------------------------------
    # Set leftover parameters
    # ---------------------------------------------

    set_leftover_params!(FUND, parameters)

end #defmodel

end #module
