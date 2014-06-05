using IAMF
using Winston

include("../src/helper.jl");
include("../src/SocioEconomicComponent.jl")
include("../src/PopulationComponent.jl")
include("../src/EmissionsComponent.jl")
include("../src/GeographyComponent.jl")
include("../src/ScenarioUncertaintyComponent.jl")
include("../src/ClimateCO2Cycle.jl")
include("../src/ClimateCH4CycleComponent.jl")
include("../src/ClimateN2OCycleComponent.jl")
include("../src/ClimateSF6CycleComponent.jl")
include("../src/ClimateForcingComponent.jl")
include("../src/ClimateDynamicsComponent.jl")
include("../src/BioDiversityComponent.jl")
include("../src/ClimateRegionalComponent.jl")
include("../src/OceanComponent.jl")
include("../src/ImpactAgricultureComponent.jl")
include("../src/ImpactBioDiversityComponent.jl")
include("../src/ImpactCardiovascularRespiratoryComponent.jl")
include("../src/ImpactCoolingComponent.jl")
include("../src/ImpactDiarrhoeaComponent.jl")
include("../src/ImpactExtratropicalStormsComponent.jl")
include("../src/ImpactDeathMorbidityComponent.jl")
include("../src/ImpactForests.jl")
include("../src/ImpactHeatingComponent.jl")
include("../src/ImpactVectorBorneDiseasesComponent.jl")
include("../src/ImpactTropicalStormsComponent.jl")
include("../src/ImpactWaterResourcesComponent.jl")
include("../src/ImpactSeaLevelRiseComponent.jl")
include("../src/ImpactAggregationComponent.jl")

# Read and prep data from files

files = readdir("../data")
parameters = {lowercase(splitext(file)[1]) => readdlm(joinpath("../data",file), ',') for file in files}
prepparameters!(parameters)

# Create the model object and set the number of timesteps and regions.

nsteps = 1049

m = Model()

setindex(m, :time, nsteps)
setindex(m, :regions, 16)

# Add components to model.

addcomponent(m, scenariouncertainty)
addcomponent(m, population)
addcomponent(m, geography)
addcomponent(m, socioeconomic)
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

# Set parameters.

setparameter(m, :population, :runwithoutpopulationperturbation, false)

setparameter(m, :socioeconomic, :runwithoutdamage, false)
setparameter(m, :socioeconomic, :savingsrate, 0.2)

# Connect components.

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

# For any parameter that is not yet set or bound to some other component, load value from file.

setleftoverparameters(m, parameters)

# Run model

run(m)

# Plot results

plot(getvariable(m, :climatedynamics, :temp))
title("temperature")
xlabel("year")
ylabel("°C")

plot(getvariable(m, :ocean, :sea))
title("sea-level rise")
xlabel("year")
ylabel("m")
