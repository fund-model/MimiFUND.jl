using Mimi

@defcomp impactwaterresources begin
    regions = Index()

    watech = Variable(index=[time])
    watechrate = Parameter(default = 0.005)

    water = Variable(index=[time,regions])
    wrbm = Parameter(index=[regions], default = [-0.00065, -0.00057, -0.00027, 3.0e-6, 3.0e-6, -0.00697, -0.02754, -0.00133, -0.0013, -0.0014, -0.00156, -0.00314, 0.00569, -0.00902, -0.0036, -0.0013])
    wrel = Parameter(default = 0.85)
    wrnl = Parameter(default = 1)
    wrpl = Parameter(default = 0.85)

    gdp90 = Parameter(index=[regions], default = default = [6343.0, 532.5, 8285.4, 5100.7, 359.5, 338.1, 915.1, 447.3, 296.0, 1078.0, 351.5, 456.2, 511.4, 137.3, 275.1, 37.2])
    income = Parameter(index=[time,regions])

    population = Parameter(index=[time,regions])
    pop90 = Parameter(index=[regions], default = [254.1, 27.8, 376.6, 166.4, 20.2, 124.3, 289.6, 189.2, 106.3, 300.2, 1131.7, 444.1, 1184.1, 117.8, 494.4, 39.9])

    temp = Parameter(index=[time,regions])

    function run_timestep(p, v, d, t)

        if gettime(t) > 2000
            v.watech[t] = (1.0 - p.watechrate)^(gettime(t) - 2000)
        else
            v.watech[t] = 1.0
        end

        for r in d.regions
            ypc = p.income[t, r] / p.population[t, r] * 1000.0
            ypc90 = p.gdp90[r] / p.pop90[r] * 1000.0

            water = p.wrbm[r] * p.gdp90[r] * v.watech[t] * (ypc / ypc90)^p.wrel * (p.population[t, r] / p.pop90[r])^p.wrpl * p.temp[t, r]^p.wrnl

            if water > 0.1 * p.income[t, r]
                v.water[t, r] = 0.1 * p.income[t, r]
            else
                v.water[t, r] = water
            end
        end
    end
end