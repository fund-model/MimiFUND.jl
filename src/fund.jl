include("SocioEconomicComponent.jl")
include("PopulationComponent.jl")
include("EmissionsComponent.jl")

function getfund(nsteps=566)
	regions = 16
    indices = {:time=>nsteps, :regions=>regions}
    # ---------------------------------------------
    # Create components
    # ---------------------------------------------

    c_population = population(indices)
    c_socioeconomic = socioeconomic(indices)
    c_emissions = emissions(indices)

    # ---------------------------------------------
    # Set parameters
    # ---------------------------------------------

    c_socioeconomic.Parameters.runwithoutdamage = true
    c_socioeconomic.Parameters.consleak = 0.025
    c_socioeconomic.Parameters.plusel = 1.0
    c_socioeconomic.Parameters.savingsrate = 0.2

    c_socioeconomic.Parameters.plus90 = ones(regions) * 2.0
	c_socioeconomic.Parameters.gdp90 = ones(regions) * 2.0
	c_socioeconomic.Parameters.pop90 = ones(regions) * 2.0
	c_socioeconomic.Parameters.urbcorr = ones(regions) * 2.0
	c_socioeconomic.Parameters.gdp0 = ones(regions) * 2.0

	c_socioeconomic.Parameters.pgrowth = ones(nsteps, regions) * 2.0
	c_socioeconomic.Parameters.ypcgrowth = ones(nsteps, regions) * 2.0
	c_socioeconomic.Parameters.eloss = ones(nsteps, regions) * 0.0
	c_socioeconomic.Parameters.sloss = ones(nsteps, regions) * 0.0
	c_socioeconomic.Parameters.mitigationcost = ones(nsteps, regions) * 0.0
	c_socioeconomic.Parameters.area = ones(nsteps, regions) * 2.0

    c_population.Parameters.pgrowth = ones(nsteps, regions) * 2.0
    c_population.Parameters.enter = ones(nsteps, regions) * 2.0
    c_population.Parameters.leave = ones(nsteps, regions) * 2.0
    c_population.Parameters.dead = ones(nsteps, regions) * 2.0
    c_population.Parameters.pop0 = ones(nsteps) * 2.0
    c_population.Parameters.runwithoutpopulationperturbation = false

    # ---------------------------------------------
    # Connect parameters to variables
    # ---------------------------------------------

    c_socioeconomic.Parameters.globalpopulation = c_population.Variables.globalpopulation
    c_socioeconomic.Parameters.populationin1 = c_population.Variables.populationin1
    c_socioeconomic.Parameters.population = c_population.Variables.population

    # ---------------------------------------------
    # Return model
    # ---------------------------------------------

    comps::Vector{ComponentState} = [c_socioeconomic, c_socioeconomic, c_emissions]
    return comps
end

m = getfund()

resetvariables(m)

run(566, m)