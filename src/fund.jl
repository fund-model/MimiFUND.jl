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

    setindex(m, :time, collect(1950:1950+nsteps))
    setindex(m, :regions, ["USA", "CAN", "WEU", "JPK", "ANZ", "EEU", "FSU", "MDE", "CAM", "LAM", "SAS", "SEA", "CHI", "MAF", "SSA", "SIS"])

    # ---------------------------------------------
    # Create components
    # ---------------------------------------------
    addcomponent(m, scenariouncertainty, :scenariouncertainty)
    addcomponent(m, population, :population)
    addcomponent(m, geography, :geography)
    addcomponent(m, socioeconomic, :socioeconomic)
    addcomponent(m, emissions, :emissions)
    addcomponent(m, climateco2cycle, :climateco2cycle)
    addcomponent(m, climatech4cycle, :climatech4cycle)
    addcomponent(m, climaten2ocycle, :climaten2ocycle)
    addcomponent(m, climatesf6cycle, :climatesf6cycle)
    addcomponent(m, climateforcing, :climateforcing)
    addcomponent(m, climatedynamics, :climatedynamics)
    addcomponent(m, biodiversity, :biodiversity)
    addcomponent(m, climateregional, :climateregional)
    addcomponent(m, ocean, :ocean)
    addcomponent(m, impactagriculture, :impactagriculture)
    addcomponent(m, impactbiodiversity, :impactbiodiversity)
    addcomponent(m, impactcardiovascularrespiratory, :impactcardiovascularrespiratory)
    addcomponent(m, impactcooling, :impactcooling)
    addcomponent(m, impactdiarrhoea, :impactdiarrhoea)
    addcomponent(m, impactextratropicalstorms, :impactextratropicalstorms)
    addcomponent(m, impactforests, :impactforests)
    addcomponent(m, impactheating, :impactheating)
    addcomponent(m, impactvectorbornediseases, :impactvectorbornediseases)
    addcomponent(m, impacttropicalstorms, :impacttropicalstorms)
    addcomponent(m, vslvmorb, :vslvmorb)
    addcomponent(m, impactdeathmorbidity, :impactdeathmorbidity)
    addcomponent(m, impactwaterresources, :impactwaterresources)
    addcomponent(m, impactsealevelrise, :impactsealevelrise)
    addcomponent(m, impactaggregation, :impactaggregation)

    # ---------------------------------------------
    # Connect parameters to variables
    # ---------------------------------------------

    connectparameter(m, :geography, :landloss, :impactsealevelrise, :landloss)

    connectparameter(m, :population, :pgrowth, :scenariouncertainty, :pgrowth)
    connectparameter(m, :population, :enter, :impactsealevelrise, :enter)
    connectparameter(m, :population, :leave, :impactsealevelrise, :leave)
    connectparameter(m, :population, :dead, :impactdeathmorbidity, :dead)

    connectparameter(m, :socioeconomic, :area, :geography, :area)
    connectparameter(m, :socioeconomic, :globalpopulation, :population, :globalpopulation)
    connectparameter(m, :socioeconomic, :populationin1, :population, :populationin1)
    connectparameter(m, :socioeconomic, :population, :population, :population)
    connectparameter(m, :socioeconomic, :pgrowth, :scenariouncertainty, :pgrowth)
    connectparameter(m, :socioeconomic, :ypcgrowth, :scenariouncertainty, :ypcgrowth)

    connectparameter(m, :socioeconomic, :eloss, :impactaggregation, :eloss)
    connectparameter(m, :socioeconomic, :sloss, :impactaggregation, :sloss)
    connectparameter(m, :socioeconomic, :mitigationcost, :emissions, :mitigationcost)

    connectparameter(m, :emissions, :income, :socioeconomic, :income)
    connectparameter(m, :emissions, :population, :population, :population)
    connectparameter(m, :emissions, :forestemm, :scenariouncertainty, :forestemm)
    connectparameter(m, :emissions, :aeei, :scenariouncertainty, :aeei)
    connectparameter(m, :emissions, :acei, :scenariouncertainty, :acei)
    connectparameter(m, :emissions, :ypcgrowth, :scenariouncertainty, :ypcgrowth)

    connectparameter(m, :climateco2cycle, :mco2, :emissions, :mco2)

    connectparameter(m, :climatech4cycle, :globch4, :emissions, :globch4)

    connectparameter(m, :climaten2ocycle, :globn2o, :emissions, :globn2o)
    connectparameter(m, :climateco2cycle, :temp, :climatedynamics, :temp)

    connectparameter(m, :climatesf6cycle, :globsf6, :emissions, :globsf6)

    connectparameter(m, :climateforcing, :acco2, :climateco2cycle, :acco2)
    connectparameter(m, :climateforcing, :acch4, :climatech4cycle, :acch4)
    connectparameter(m, :climateforcing, :acn2o, :climaten2ocycle, :acn2o)
    connectparameter(m, :climateforcing, :acsf6, :climatesf6cycle, :acsf6)

    connectparameter(m, :climatedynamics, :radforc, :climateforcing, :radforc)

    connectparameter(m, :climateregional, :inputtemp, :climatedynamics, :temp)

    connectparameter(m, :biodiversity, :temp, :climatedynamics, :temp)

    connectparameter(m, :ocean, :temp, :climatedynamics, :temp)

    connectparameter(m, :impactagriculture, :population, :population, :population)
    connectparameter(m, :impactagriculture, :income, :socioeconomic, :income)
    connectparameter(m, :impactagriculture, :temp, :climateregional, :temp)
    connectparameter(m, :impactagriculture, :acco2, :climateco2cycle, :acco2)

    connectparameter(m, :impactbiodiversity, :temp, :climateregional, :temp)
    connectparameter(m, :impactbiodiversity, :nospecies, :biodiversity, :nospecies)
    connectparameter(m, :impactbiodiversity, :income, :socioeconomic, :income)
    connectparameter(m, :impactbiodiversity, :population, :population, :population)

    connectparameter(m, :impactcardiovascularrespiratory, :population, :population, :population)
    connectparameter(m, :impactcardiovascularrespiratory, :temp, :climateregional, :temp)
    connectparameter(m, :impactcardiovascularrespiratory, :plus, :socioeconomic, :plus)
    connectparameter(m, :impactcardiovascularrespiratory, :urbpop, :socioeconomic, :urbpop)

    connectparameter(m, :impactcooling, :population, :population, :population)
    connectparameter(m, :impactcooling, :income, :socioeconomic, :income)
    connectparameter(m, :impactcooling, :temp, :climateregional, :temp)
    connectparameter(m, :impactcooling, :cumaeei, :emissions, :cumaeei)

    connectparameter(m, :impactdiarrhoea, :population, :population, :population)
    connectparameter(m, :impactdiarrhoea, :income, :socioeconomic, :income)
    connectparameter(m, :impactdiarrhoea, :regtmp, :climateregional, :regtmp)

    connectparameter(m, :impactextratropicalstorms, :population, :population, :population)
    connectparameter(m, :impactextratropicalstorms, :income, :socioeconomic, :income)
    connectparameter(m, :impactextratropicalstorms, :acco2, :climateco2cycle, :acco2)

    connectparameter(m, :impactforests, :population, :population, :population)
    connectparameter(m, :impactforests, :income, :socioeconomic, :income)
    connectparameter(m, :impactforests, :temp, :climateregional, :temp)
    connectparameter(m, :impactforests, :acco2, :climateco2cycle, :acco2)

    connectparameter(m, :impactheating, :population, :population, :population)
    connectparameter(m, :impactheating, :income, :socioeconomic, :income)
    connectparameter(m, :impactheating, :temp, :climateregional, :temp)
    connectparameter(m, :impactheating, :cumaeei, :emissions, :cumaeei)

    connectparameter(m, :impactvectorbornediseases, :population, :population, :population)
    connectparameter(m, :impactvectorbornediseases, :income, :socioeconomic, :income)
    connectparameter(m, :impactvectorbornediseases, :temp, :climateregional, :temp)

    connectparameter(m, :impacttropicalstorms, :population, :population, :population)
    connectparameter(m, :impacttropicalstorms, :income, :socioeconomic, :income)
    connectparameter(m, :impacttropicalstorms, :regstmp, :climateregional, :regstmp)

    connectparameter(m, :vslvmorb, :population, :population, :population)
    connectparameter(m, :vslvmorb, :income, :socioeconomic, :income)

    connectparameter(m, :impactdeathmorbidity, :vsl, :vslvmorb, :vsl)
    connectparameter(m, :impactdeathmorbidity, :vmorb, :vslvmorb, :vmorb)
    connectparameter(m, :impactdeathmorbidity, :population, :population, :population)
    connectparameter(m, :impactdeathmorbidity, :dengue, :impactvectorbornediseases, :dengue)
    connectparameter(m, :impactdeathmorbidity, :schisto, :impactvectorbornediseases, :schisto)
    connectparameter(m, :impactdeathmorbidity, :malaria, :impactvectorbornediseases, :malaria)
    connectparameter(m, :impactdeathmorbidity, :cardheat, :impactcardiovascularrespiratory, :cardheat)
    connectparameter(m, :impactdeathmorbidity, :cardcold, :impactcardiovascularrespiratory, :cardcold)
    connectparameter(m, :impactdeathmorbidity, :resp, :impactcardiovascularrespiratory, :resp)
    connectparameter(m, :impactdeathmorbidity, :diadead, :impactdiarrhoea, :diadead)
    connectparameter(m, :impactdeathmorbidity, :hurrdead, :impacttropicalstorms, :hurrdead)
    connectparameter(m, :impactdeathmorbidity, :extratropicalstormsdead, :impactextratropicalstorms, :extratropicalstormsdead)
    connectparameter(m, :impactdeathmorbidity, :diasick, :impactdiarrhoea, :diasick)
    setparameter(m, :impactdeathmorbidity, :dead_other, zeros(nsteps+1, 16)) # This is a connection point for other impact components
    setparameter(m, :impactdeathmorbidity, :sick_other, zeros(nsteps+1, 16)) # This is a connection point for other impact components

    connectparameter(m, :impactwaterresources, :population, :population, :population)
    connectparameter(m, :impactwaterresources, :income, :socioeconomic, :income)
    connectparameter(m, :impactwaterresources, :temp, :climateregional, :temp)

    connectparameter(m, :impactsealevelrise, :population, :population, :population)
    connectparameter(m, :impactsealevelrise, :income, :socioeconomic, :income)
    connectparameter(m, :impactsealevelrise, :sea, :ocean, :sea)
    connectparameter(m, :impactsealevelrise, :area, :geography, :area)

    connectparameter(m, :impactaggregation, :income, :socioeconomic, :income)
    connectparameter(m, :impactaggregation, :heating, :impactheating, :heating)
    connectparameter(m, :impactaggregation, :cooling, :impactcooling, :cooling)
    connectparameter(m, :impactaggregation, :agcost, :impactagriculture, :agcost)
    connectparameter(m, :impactaggregation, :species, :impactbiodiversity, :species)
    connectparameter(m, :impactaggregation, :water, :impactwaterresources, :water)
    connectparameter(m, :impactaggregation, :hurrdam, :impacttropicalstorms, :hurrdam)
    connectparameter(m, :impactaggregation, :extratropicalstormsdam, :impactextratropicalstorms, :extratropicalstormsdam)
    connectparameter(m, :impactaggregation, :forests, :impactforests, :forests)
    connectparameter(m, :impactaggregation, :drycost, :impactsealevelrise, :drycost)
    connectparameter(m, :impactaggregation, :protcost, :impactsealevelrise, :protcost)
    connectparameter(m, :impactaggregation, :entercost, :impactsealevelrise, :entercost)
    connectparameter(m, :impactaggregation, :deadcost, :impactdeathmorbidity, :deadcost)
    connectparameter(m, :impactaggregation, :morbcost, :impactdeathmorbidity, :morbcost)
    connectparameter(m, :impactaggregation, :wetcost, :impactsealevelrise, :wetcost)
    connectparameter(m, :impactaggregation, :leavecost, :impactsealevelrise, :leavecost)

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


    setbestguess(m)
    # ---------------------------------------------
    # Return model
    # ---------------------------------------------

    return m
end
