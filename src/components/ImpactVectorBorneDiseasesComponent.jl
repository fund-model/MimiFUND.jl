using Mimi

@defcomp impactvectorbornediseases begin
    regions = Index()

    dengue = Variable(index=[time,regions])
    schisto = Variable(index=[time,regions])
    malaria = Variable(index=[time,regions])

    dfbs = Parameter(index=[regions], default = [0.0, 0.0, 0.0, 0.125, 0.0, 0.0, 0.0, 0.286, 0.508, 0.541, 6.896, 2.072, 0.593, 1.089, 0.351, 1.01])
    dfch = Parameter(index=[regions], default = [0.3534, 0.3534, 0.3534, 0.3534, 0.3534, 0.3534, 0.3534, 0.3534, 0.3534, 0.3534, 0.3534, 0.3534, 0.3534, 0.3534, 0.3534, 0.3534])
    smbs = Parameter(index=[regions], default = [0.007, 0.007, 0.02, 0.423, 0.037, 0.012, 0.003, 4.229, 1.235, 1.217, 0.898, 0.629, 1.43, 7.474, 8.275, 1.296])
    smch = Parameter(index=[regions], default = [-0.1149, -0.1149, -0.1149, -0.1149, -0.1149, -0.1149, -0.1149, -0.1149, -0.1149, -0.1149, -0.1149, -0.1149, -0.1149, -0.1149, -0.1149, -0.1149])
    malbs = Parameter(index=[regions], default = [0.023, 0.023, 0.24, 2.358, 0.069, 0.377, 0.133, 24.113, 2.913, 3.09, 48.413, 22.129, 8.987, 458.133, 1414.284, 116.586])
    malch = Parameter(index=[regions], default = [0.0794, 0.0794, 0.0794, 0.0794, 0.0794, 0.0794, 0.0794, 0.0794, 0.0794, 0.0794, 0.0794, 0.0794, 0.0794, 0.0794, 0.0794, 0.0794])

    gdp90 = Parameter(index=[regions], default = [6343.0, 532.5, 8285.4, 5100.7, 359.5, 338.1, 915.1, 447.3, 296.0, 1078.0, 351.5, 456.2, 511.4, 137.3, 275.1, 37.2])
    pop90 = Parameter(index=[regions], default = [254.1, 27.8, 376.6, 166.4, 20.2, 124.3, 289.6, 189.2, 106.3, 300.2, 1131.7, 444.1, 1184.1, 117.8, 494.4, 39.9])

    income = Parameter(index=[time,regions])
    population = Parameter(index=[time,regions])
    temp = Parameter(index=[time,regions])

    dfnl = Parameter(default = 1.)
    vbel = Parameter(default = -2.65)
    smnl = Parameter(default = 1.)
    malnl = Parameter(default = 1.)

    function run_timestep(p, v, d, t)

        for r in d.regions
            ypc = 1000.0 * p.income[t, r] / p.population[t, r]
            ypc90 = p.gdp90[r] / p.pop90[r] * 1000.0

            v.dengue[t, r] = p.dfbs[r] * p.population[t, r] * p.dfch[r] * p.temp[t, r]^p.dfnl * (ypc / ypc90)^p.vbel

            v.schisto[t, r] = p.smbs[r] * p.population[t, r] * p.smch[r] * p.temp[t, r]^p.smnl * (ypc / ypc90)^p.vbel

            if v.schisto[t, r] < -p.smbs[r] * p.population[t, r] * (ypc / ypc90)^p.vbel
                v.schisto[t, r] = -p.smbs[r] * p.population[t, r] * (ypc / ypc90)^p.vbel
            end

            v.malaria[t, r] = p.malbs[r] * p.population[t, r] * p.malch[r] * p.temp[t, r]^p.malnl * (ypc / ypc90)^p.vbel
        end
    end
end