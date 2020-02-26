﻿@defcomp impactforests begin
    regions = Index()

    forests = Variable(index=[time,regions])    # Economic damages: positive value means damages, negative value means benefits

    forbm = Parameter(index=[regions])
    gdp90 = Parameter(index=[regions])
    pop90 = Parameter(index=[regions])

    acco2 = Parameter(index=[time])

    income = Parameter(index=[time,regions])
    population = Parameter(index=[time,regions])
    temp = Parameter(index=[time,regions])

    forel = Parameter(default = -0.31)
    fornl = Parameter(default = 1)
    co2pre = Parameter(default = 275.0)
    forco2 = Parameter(default = 2.2972)

    function run_timestep(p, v, d, t)

        if !is_first(t)
            for r in d.regions
                ypc = 1000.0 * p.income[t, r] / p.population[t, r]
                ypc90 = p.gdp90[r] / p.pop90[r] * 1000.0

                # TODO -oDavid Anthoff: RT uses -lP.forel for ppp case
                v.forests[t, r] = max(
                    -1 * p.forbm[r] * p.income[t, r] * (ypc / ypc90)^p.forel * (0.5 * p.temp[t, r]^p.fornl + 0.5 * log(p.acco2[t - 1] / p.co2pre) * p.forco2),
                    -0.1 * p.income[t, r]
                )
            end
        end
    end
end
