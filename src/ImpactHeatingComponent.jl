using Mimi

@defcomp impactheating begin
    regions = Index()

    heating = Variable(index=[time,regions])

    hebm = Parameter(index=[regions])
    gdp90 = Parameter(index=[regions], default = [6343.0, 532.5, 8285.4, 5100.7, 359.5, 338.1, 915.1, 447.3, 296.0, 1078.0, 351.5, 456.2, 511.4, 137.3, 275.1, 37.2])

    population = Parameter(index=[time,regions])
    pop90 = Parameter(index=[regions], default = [254.1, 27.8, 376.6, 166.4, 20.2, 124.3, 289.6, 189.2, 106.3, 300.2, 1131.7, 444.1, 1184.1, 117.8, 494.4, 39.9])

    income = Parameter(index=[time,regions])
    heel = Parameter(default = 0.8)

    temp = Parameter(index=[time,regions])
    henl = Parameter(default = 0.5)

    cumaeei = Parameter(index=[time,regions])

    function run_timestep(p, v, d, t)

        if !is_first(t)
            for r in d.regions
                ypc = p.income[t, r] / p.population[t, r] * 1000.0
                ypc90 = p.gdp90[r] / p.pop90[r] * 1000.0

                v.heating[t, r] = p.hebm[r] * p.cumaeei[t, r] * p.gdp90[r] * atan(p.temp[t, r]) / atan(1.0) * (ypc / ypc90)^p.heel * p.population[t, r] / p.pop90[r]
            end
        end
    end
end