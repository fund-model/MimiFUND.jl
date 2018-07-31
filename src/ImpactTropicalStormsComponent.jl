using Mimi

@defcomp impacttropicalstorms begin
    regions = Index()

    hurrdam = Variable(index=[time,regions])
    hurrdead = Variable(index=[time,regions])

    hurrbasedam = Parameter(index=[regions], default = [1.46956723905549, 0.0073550933539639, 1.72941225772836e-5, 0.328676007550673, 0.100281593611573, 0.0, 0.0171639383449058, 0.0, 1.77259981548537, 0.0130629900020999, 0.936454011524578, 0.414318875437606, 1.97291717404422, 0.0, 0.0591056686144018, 5.73135007408064])
    hurrdamel = Parameter(default = -0.514)
    hurrnl = Parameter(default = 3)
    hurrpar = Parameter(default = 0.04)
    hurrbasedead = Parameter(index=[regions], default = [0.000390601570796675, 4.86079995979859e-6, 2.12623547836067e-6, 0.00054339759458889, 6.68406734149395e-5, 0.0, 7.09182566979657e-6, 1.39312354219572e-6, 0.00821967419855805, 2.36702723444742e-5, 0.00691678053157295, 0.00239814928899338, 0.000286767363184808, 0.0, 0.000143920516535688, 0.00491454455882495])
    hurrdeadel = Parameter(default = -0.501)

    gdp90 = Parameter(index=[regions], default = [6343.0, 532.5, 8285.4, 5100.7, 359.5, 338.1, 915.1, 447.3, 296.0, 1078.0, 351.5, 456.2, 511.4, 137.3, 275.1, 37.2])
    pop90 = Parameter(index=[regions], default = [254.1, 27.8, 376.6, 166.4, 20.2, 124.3, 289.6, 189.2, 106.3, 300.2, 1131.7, 444.1, 1184.1, 117.8, 494.4, 39.9])

    population = Parameter(index=[time,regions])
    income = Parameter(index=[time,regions])

    regstmp = Parameter(index=[time,regions])

    function run_timestep(p, v, d, t)

        for r in d.regions
            ypc = p.income[t, r] / p.population[t, r] * 1000.0
            ypc90 = p.gdp90[r] / p.pop90[r] * 1000.0

            # This is hurrican damage
            v.hurrdam[t, r] = 0.001 * p.hurrbasedam[r] * p.income[t, r] * (ypc / ypc90)^p.hurrdamel * ((1.0 + p.hurrpar * p.regstmp[t, r])^p.hurrnl - 1.0)

            v.hurrdead[t, r] = 1000.0 * p.hurrbasedead[r] * p.population[t, r] * (ypc / ypc90)^p.hurrdeadel * ((1.0 + p.hurrpar * p.regstmp[t, r])^p.hurrnl - 1.0)
        end
    end
end
