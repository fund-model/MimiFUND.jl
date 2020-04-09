include("../ScenarioUncertaintyComponent.jl")
include("../PopulationComponent.jl")
include("../GeographyComponent.jl")
include("../SocioEconomicComponent.jl")
include("../EmissionsComponent.jl")

@defcomposite socioeconomiccomposite begin
    
    Component(scenariouncertainty)
    Component(population)
    Component(geography)
    Component(socioeconomic)
    Component(emissions)

    gdp90 = Parameter(emissions.gdp90, socioeconomic.gdp90)
    pop90 = Parameter(emissions.pop90, socioeconomic.pop90)

    connect(population.pgrowth, scenariouncertainty.pgrowth)
    connect(socioeconomic.area, geography.area)
    connect(socioeconomic.globalpopulation, population.globalpopulation)
    connect(socioeconomic.populationin1, population.populationin1)
    connect(socioeconomic.population, population.population)
    connect(socioeconomic.pgrowth, scenariouncertainty.pgrowth)
    connect(socioeconomic.ypcgrowth, scenariouncertainty.ypcgrowth)
    connect(socioeconomic.mitigationcost, emissions.mitigationcost)
    connect(emissions.income, socioeconomic.income)
    connect(emissions.population, population.population)
    connect(emissions.forestemm, scenariouncertainty.forestemm)
    connect(emissions.aeei, scenariouncertainty.aeei)
    connect(emissions.acei, scenariouncertainty.acei)
    connect(emissions.ypcgrowth, scenariouncertainty.ypcgrowth)

    mco2 = Variable(emissions.mco2)
    globch4 = Variable(emissions.globch4)
    globn2o = Variable(emissions.globn2o)
    globsf6 = Variable(emissions.globsf6)

    population_var = Variable(population.population)
    income = Variable(socioeconomic.income)
    plus = Variable(socioeconomic.plus)
    urbpop = Variable(socioeconomic.urbpop)
    cumaeei = Variable(emissions.cumaeei)
    area = Variable(geography.area)

end