using Mimi

@defcomp impactdiarrhoea begin
    regions = Index()

    diadead = Variable(index=[time,regions])
    diasick = Variable(index=[time,regions])

    diamort = Parameter(index=[regions], default = [41.154, 41.154, 14.551, 8.896, 1.346, 18.085, 121.694, 29.799, 161.717, 168.068, 229.167, 135.411, 33.091, 414.929, 3167.301, 252.443])
    diamortel = Parameter(default = -1.578625)
    diamortnl = Parameter(default = 1.141714)

    diayld = Parameter(index=[regions], default = [1704.156, 1704.156, 631.878, 166.292, 82.96, 846.775, 6734.631, 166.214, 643.001, 649.877, 896.137, 630.93, 400.604, 989.751, 5706.631, 1092.314])
    diayldel = Parameter(default = -0.418406)
    diayldnl = Parameter(default = 0.699241)

    income = Parameter(index=[time,regions])
    population = Parameter(index=[time,regions])
    gdp90 = Parameter(index=[regions], default = [6343.0, 532.5, 8285.4, 5100.7, 359.5, 338.1, 915.1, 447.3, 296.0, 1078.0, 351.5, 456.2, 511.4, 137.3, 275.1, 37.2])
    pop90 = Parameter(index=[regions], default = [254.1, 27.8, 376.6, 166.4, 20.2, 124.3, 289.6, 189.2, 106.3, 300.2, 1131.7, 444.1, 1184.1, 117.8, 494.4, 39.9])

    temp90 = Parameter(index=[regions], default = [10.79, -8.03, 9.03, 9.15, 18.84, 7.75, 4.5, 16.48, 20.67, 21.25, 21.78, 23.7, 4.54, 20.57, 23.95, 24.48])
    bregtmp = Parameter(index=[regions], default = [1.1941, 1.4712, 1.1248, 1.0555, 0.9676, 1.1676, 1.2866, 1.1546, 0.8804, 0.8504, 0.9074, 0.7098, 1.1847, 1.143, 0.878, 0.7517])
    regtmp = Parameter(index=[time,regions])

    function run_timestep(p, v, d, t)

        for r in d.regions
            ypc = 1000.0 * p.income[t, r] / p.population[t, r]
            ypc90 = 1000.0 * p.gdp90[r] / p.pop90[r]

            # 0.49 is the increase in global temperature from pre-industrial to 1990
            absoluteRegionalTempPreIndustrial = p.temp90[r] - 0.49 * p.bregtmp[r]

            if absoluteRegionalTempPreIndustrial > 0.0
                v.diadead[t, r] = p.diamort[r] * p.population[t, r] * (ypc / ypc90)^p.diamortel * (((absoluteRegionalTempPreIndustrial + p.regtmp[t, r]) / absoluteRegionalTempPreIndustrial)^p.diamortnl - 1.0)

                v.diasick[t, r] = p.diayld[r] * p.population[t, r] * (ypc / ypc90)^p.diayldel * (((absoluteRegionalTempPreIndustrial + p.regtmp[t, r]) / absoluteRegionalTempPreIndustrial)^p.diayldnl - 1.0)
            else
                v.diadead[t, r] = 0.0
                v.diasick[t, r] = 0.0
            end
        end
    end
end