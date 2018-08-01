using Mimi

@defcomp impactforests begin
    regions = Index()

    forests = Variable(index=[time,regions])

    forbm = Parameter(index=[regions], default = [5.325e-5, 1.123e-5, 2.528e-5, 4.173e-5, -0.0001211, 5.462e-5, -2.348e-5, 0.0, 1.806e-5, 2.411e-5, 6.237e-5, 6.674e-5, 8.723e-5, 0.0, 1.141e-5, 0.0])
    gdp90 = Parameter(index=[regions], default = [6343.0, 532.5, 8285.4, 5100.7, 359.5, 338.1, 915.1, 447.3, 296.0, 1078.0, 351.5, 456.2, 511.4, 137.3, 275.1, 37.2])
    pop90 = Parameter(index=[regions], default = [254.1, 27.8, 376.6, 166.4, 20.2, 124.3, 289.6, 189.2, 106.3, 300.2, 1131.7, 444.1, 1184.1, 117.8, 494.4, 39.9])

    acco2 = Parameter(index=[time])

    income = Parameter(index=[time,regions])
    population = Parameter(index=[time,regions])
    temp = Parameter(index=[time,regions])

    forel = Parameter(default = -0.31)
    fornl = Parameter(default = 1.)
    co2pre = Parameter(default = 275.0)
    forco2 = Parameter(default = 2.2972)

    function run_timestep(p, v, d, t)

        if !is_first(t)
            for r in d.regions
                ypc = 1000.0 * p.income[t, r] / p.population[t, r]
                ypc90 = p.gdp90[r] / p.pop90[r] * 1000.0

                # TODO -oDavid Anthoff: RT uses -lP.forel for ppp case
                v.forests[t, r] = p.forbm[r] * p.income[t, r] * (ypc / ypc90)^p.forel * (0.5 * p.temp[t, r]^p.fornl + 0.5 * log(p.acco2[t - 1] / p.co2pre) * p.forco2)

                if v.forests[t, r] > 0.1 * p.income[t, r]
                    v.forests[t, r] = 0.1 * p.income[t, r]
                end
            end
        end
    end
end
