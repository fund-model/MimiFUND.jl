using Mimi

@defcomp impactagriculture begin
    regions = Index()

    gdp90 = Parameter(index=[regions], default = [6343.0, 532.5, 8285.4, 5100.7, 359.5, 338.1, 915.1, 447.3, 296.0, 1078.0, 351.5, 456.2, 511.4, 137.3, 275.1, 37.2])
    income = Parameter(index=[time,regions])
    pop90 = Parameter(index=[regions], default = [254.1, 27.8, 376.6, 166.4, 20.2, 124.3, 289.6, 189.2, 106.3, 300.2, 1131.7, 444.1, 1184.1, 117.8, 494.4, 39.9])
    population = Parameter(index=[time,regions])

    agrish = Variable(index=[time,regions])
    agrish0 = Parameter(index=[regions], default = [0.0248, 0.0378, 0.0563, 0.0437, 0.0781, 0.1469, 0.2633, 0.1618, 0.1153, 0.1145, 0.3565, 0.2729, 0.3824, 0.2456, 0.1799, 0.1151])
    agel = Parameter(default = 0.31)

    agrate = Variable(index=[time,regions])
    aglevel = Variable(index=[time,regions])
    agco2 = Variable(index=[time,regions])
    agcost = Variable(index=[time,regions])

    agrbm = Parameter(index=[regions], default = [-0.00021, -0.00029, -0.00039, -0.00033, -0.00015, -0.00027, -0.00018, -0.00022, -0.00034, -9.0e-5, -0.00014, -9.0e-5, -0.00013, -0.00016, -0.00011, -0.0005])
    agtime = Parameter(index=[regions], default = [10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0])
    agnl = Parameter(default = 2.0)

    aglparl = Parameter(index=[regions], default = [0.0259810049019608, 0.0923560606060606, 0.021883487654321, 0.0462338709677419, 0.0396465746205965, 0.0475721982758621, 0.0418566470712197, 0.0421993430741693, 0.0638352272727273, 0.003045, 0.025470079787234, 0.0135138888888889, 0.0430619429569277, 0.03346875, 0.0244802631578947, 0.0425735877466748]) 
    aglparq = Parameter(index=[regions], default = [-0.0119178921568627, -0.0158143939393939, -0.0138503086419753, -0.0235887096774194, -0.0155903909385135, -0.0181573275862069, -0.0163118735427424, -0.0165914930103618, -0.0303977272727273, -0.00435, -0.0112699468085106, -0.00965277777777778, -0.0168265560310128, -0.0139453125, -0.0100328947368421, -0.0166147684550101])

    agcbm = Parameter(index=[regions], default = [0.089, 0.0402, 0.1541, 0.2319, 0.1048, 0.0952, 0.0671, 0.0943, 0.1641, 0.0596, 0.058, 0.0845, 0.1921, 0.0727, 0.0505, 0.2377])
    co2pre = Parameter(default = 275.0)

    temp = Parameter(index=[time,regions])
    acco2 = Parameter(index=[time])

    function run_timestep(p, v, d, t)
        
        const DBsT = 0.04     # base case yearly warming
        
        if is_first(t)
            for r in d.regions
                v.agrate[t, r] = p.agrbm[r] * (0.005 / DBsT)^p.agnl * p.agtime[r]
            end
        else
            for r in d.regions
                ypc = p.income[t, r] / p.population[t, r] * 1000.0
                ypc90 = p.gdp90[r] / p.pop90[r] * 1000.0

                v.agrish[t, r] = p.agrish0[r] * (ypc / ypc90)^(-p.agel)
            end

            for r in d.regions
                dtemp = abs(p.temp[t, r] - p.temp[t - 1, r])

                if isnan((dtemp / 0.04)^p.agnl)
                    v.agrate[t, r] = 0.0
                else
                    v.agrate[t, r] = p.agrbm[r] * (dtemp / 0.04)^p.agnl + (1.0 - 1.0 / p.agtime[r]) * v.agrate[t - 1, r]
                end
            end

            for r in d.regions
                v.aglevel[t, r] = p.aglparl[r] * p.temp[t, r] + p.aglparq[r] * p.temp[t, r]^2.0
            end

            for r in d.regions
                v.agco2[t, r] = p.agcbm[r] / log(2.0) * log(p.acco2[t - 1] / p.co2pre)
            end

            for r in d.regions
                v.agcost[t, r] = min(1.0, v.agrate[t, r] + v.aglevel[t, r] + v.agco2[t, r]) * v.agrish[t, r] * p.income[t, r]
            end
        end
    end
end
