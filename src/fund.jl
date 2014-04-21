include("SocioEconomicComponent.jl")

function getfund(nsteps=566)
	regions = 16
    # ---------------------------------------------
    # Create components
    # ---------------------------------------------

    c_socioeconomic = socioeconomic({:time=>nsteps, :regions=>regions})

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
	c_socioeconomic.Parameters.globalpopulation = ones(nsteps) * 2.0
	c_socioeconomic.Parameters.population = ones(nsteps, regions) * 2.0
	c_socioeconomic.Parameters.populationin1 = ones(nsteps, regions) * 2.0


    # ---------------------------------------------
    # Connect parameters to variables
    # ---------------------------------------------

    #c_socioeconomic.Parameters.forcing = c_radforc.Variables.rf
    #c_ccm.Parameters.temp = c_socioeconomic.Variables.temp
    #c_radforc.Parameters.atmco2 = c_ccm.Variables.atmco2

    # ---------------------------------------------
    # Return model
    # ---------------------------------------------

    comps::Vector{ComponentState} = [c_socioeconomic]
    return comps
end

m = getfund()

resetvariables(m)

run(566, m)