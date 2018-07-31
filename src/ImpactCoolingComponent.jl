using Mimi

@defcomp impactcooling begin
    regions = Index()

    cooling = Variable(index=[time,regions])

    cebm = Parameter(index=[regions], default = [-0.00212, -0.00186, -0.00372, -0.00029, -0.00021, -0.00185, -0.00674, -0.00233, -0.00239, -0.00259, -0.00384, -0.0074, -0.02891, -0.01892, -0.00797, -0.00239])
    gdp90 = Parameter(index=[regions], default = [6343.0, 532.5, 8285.4, 5100.7, 359.5, 338.1, 915.1, 447.3, 296.0, 1078.0, 351.5, 456.2, 511.4, 137.3, 275.1, 37.2])

    population = Parameter(index=[time,regions])
    pop90 = Parameter(index=[regions], default = [254.1, 27.8, 376.6, 166.4, 20.2, 124.3, 289.6, 189.2, 106.3, 300.2, 1131.7, 444.1, 1184.1, 117.8, 494.4, 39.9])

    income = Parameter(index=[time,regions])
    ceel = Parameter(default = 0.8)

    temp = Parameter(index=[time,regions])
    cenl = Parameter(default = 1.5)

    cumaeei = Parameter(index=[time,regions])

    function run_timestep(p, v, d, t)

        if !is_first(t)
            for r in d.regions
                ypc = p.income[t, r] / p.population[t, r] * 1000.0
                ypc90 = p.gdp90[r] / p.pop90[r] * 1000.0

                v.cooling[t, r] = p.cebm[r] * p.cumaeei[t, r] * p.gdp90[r] * (p.temp[t, r] / 1.0)^p.cenl * (ypc / ypc90)^p.ceel * p.population[t, r] / p.pop90[r]
            end
        end
    end
end
