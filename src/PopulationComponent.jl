using Mimi

@defcomp population begin
    regions             = Index()

    population          = Variable(index=[time,regions])
    populationin1       = Variable(index=[time,regions])
    globalpopulation    = Variable(index=[time])

    pgrowth             = Parameter(index=[time,regions])
    enter               = Parameter(index=[time,regions])
    leave               = Parameter(index=[time,regions])
    dead                = Parameter(index=[time,regions])
    pop0                = Parameter(index=[regions], default = [157.8, 13.7, 304.3, 104.0, 10.1, 87.7, 180.1, 58.6, 35.0, 114.9, 464.1, 183.2, 567.2, 44.1, 175.8, 19.8])
    runwithoutpopulationperturbation::Bool = Parameter(default = false)

    function run_timestep(p, v, d, t)
        if is_first(t)
            for r in d.regions
                v.population[t, r] = p.pop0[r]
                v.populationin1[t, r] = v.population[t, r] * 1000000.0
            end
    
            v.globalpopulation[t] = sum(v.populationin1[t,:])
        else
            for r in d.regions
                v.population[t, r] = (1.0 + 0.01 * p.pgrowth[t - 1, r]) * (v.population[t - 1, r] + ((gettime(t) >= 1990 && !p.runwithoutpopulationperturbation) ? (p.enter[t - 1, r] / 1000000.0) - (p.leave[t - 1, r] / 1000000.0) - (p.dead[t - 1, r] >= 0 ? p.dead[t - 1, r] / 1000000.0 : 0) : 0))
    
                if v.population[t, r] < 0
                    v.population[t, r] = 0.000001
                end
    
                v.populationin1[t, r] = v.population[t, r] * 1000000.0
            end
    
            v.globalpopulation[t] = sum(v.populationin1[t,:])
        end
    end
end