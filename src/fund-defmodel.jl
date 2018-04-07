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

export FUND

const global nsteps = 1050
const global datadir = joinpath(dirname(@__FILE__), "..", "data")
const global params = nothing

@defmodel FUND begin 

    # ---------------------------------------------
    # Set indexes 
    # ---------------------------------------------

    index[time]     = collect(1950:1950+nsteps)
    index[regions]  = ["USA", "CAN", "WEU", "JPK", "ANZ", "EEU", "FSU", "MDE", "CAM", "LAM", "SAS", "SEA", "CHI", "MAF", "SSA", "SIS"]
    index[cpools]   = 1:5

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
    populatoin.globalpopulation     => socioeconomic.globalpopulation
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
    climateregional.regtmp  => impactdiarrhoea.regtemp

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
    climateregional.temp    => impacttropicalstorms.temp

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
    impactaggregation.agcost        => impactaggregation.agcost
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
    
    add_connector_comps(FUND)
    
    # ---------------------------------------------
    # Set leftover parameters
    # ---------------------------------------------

    setleftoverparameters(FUND, parameters)

end #defmodel

end #module
