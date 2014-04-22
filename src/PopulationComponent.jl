using IAMF

@defcomp population begin
    addIndex(regions)

    addVariable(population, Float64, index=[time,regions])
    addVariable(populationin1, Float64, index=[time,regions])
    addVariable(globalpopulation, Float64, index=[time])

    addParameter(pgrowth, Float64, index=[time,regions])
    addParameter(enter, Float64, index=[time,regions])
    addParameter(leave, Float64, index=[time,regions])
    addParameter(dead, Float64, index=[time,regions])
    addParameter(pop0, Float64, index=[regions])
    addParameter(runwithoutpopulationperturbation, Bool)
end

function init(s::population)    
    v = s.Variables
    p = s.Parameters
    d = s.Dimensions

    t = 1

    for r in d.regions    
        v.population[t, r] = p.pop0[r]
        v.populationin1[t, r] = p.population[t, r] * 1000000.0
    end

    v.globalpopulation[t] = sum(s.populationin1[t,:])
end

function timestep(s::population, t::Int)
    v = s.Variables
    p = s.Parameters
    d = s.Dimensions

    for r in d.regions
        v.population[t, r] = (1.0 + 0.01 * s.pgrowth[t - 1, r]) * (s.population[t - 1, r] + ((t >= Timestep.FromSimulationYear(40)) && !s.runwithoutpopulationperturbation ? (s.enter[t - 1, r] / 1000000.0) - (s.leave[t - 1, r] / 1000000.0) - (s.dead[t - 1, r] >= 0 ? s.dead[t - 1, r] / 1000000.0 : 0) : 0))

        if v.population[t, r] < 0
            v.population[t, r] = 0.000001
        end

        v.populationin1[t, r] = v.population[t, r] * 1000000.0
    end

    v.globalpopulation[t] = sum(s.populationin1[t,:])
end
