using Mimi

@defcomp impactcardiovascularrespiratory begin
    regions = Index()

    basecardvasc = Variable(index=[time,regions])
    baseresp = Variable(index=[time,regions])

    cardheat = Variable(index=[time,regions])
    resp = Variable(index=[time,regions])
    cardcold = Variable(index=[time,regions])

    cardvasc90 = Parameter(index=[regions], default = [0.0035, 0.0035, 0.00436, 0.00263, 0.00263, 0.00481, 0.00874, 0.00198, 0.00182, 0.00182, 0.00281, 0.00203, 0.00205, 0.00237, 0.00149, 0.00182])
    plus90 = Parameter(index=[regions], default = [0.1239, 0.1122, 0.1473, 0.149, 0.1115, 0.1034, 0.0931, 0.0358, 0.0384, 0.0486, 0.0406, 0.04, 0.0856, 0.0373, 0.0291, 0.0604])
    resp90 = Parameter(index=[regions], default = [0.00053, 0.00053, 0.0005, 0.00037, 0.00037, 0.0003, 0.00052, 0.00019, 0.00039, 0.00039, 0.00039, 0.00046, 0.00011, 0.00037, 0.00038, 0.00039])
    chplbm = Parameter(index=[regions], default = [34.9374, 27.328, 25.757, 8.2986, 18.8372, 29.6249, 36.4415, 50.5493, 44.7697, 33.7621, 74.5092, -18.7223, 82.0355, 50.4842, 43.4397, 16.9938])
    chmlbm = Parameter(index=[regions], default = [1.0988, 1.0705, 0.4022, 1.0356, 0.4493, 0.6119, 0.6468, 1.0931, 0.9144, 0.5893, 1.6317, 0.8545, 0.7565, 1.0409, 0.8682, 1.0227])
    chpqbm = Parameter(index=[regions], default = [1.7285, 1.7285, 1.7966, 0.7493, 1.7286, 1.7531, 1.7285, 1.7011, 1.662, 1.7535, 1.7378, -0.6683, 1.2095, 1.7096, 1.6578, 0.4223])
    chmqbm = Parameter(index=[regions], default = [0.0471, 0.0471, 0.0467, 0.0559, 0.047, 0.047, 0.0471, 0.0452, 0.0471, 0.047, 0.047, 0.0411, 0.0474, 0.0471, 0.044, 0.0324])
    rlbm = Parameter(index=[regions], default = [0.9452, -1.9284, -0.765, 0.4185, 0.2579, -1.2946, 1.5277, 5.6711, 3.8894, 1.0893, 10.2485, 4.8562, 4.4083, 5.198, 3.6196, 4.1354])
    rqbm = Parameter(index=[regions], default = [0.4342, 0.4342, 0.4341, 0.4342, 0.4342, 0.4342, 0.4342, 0.4194, 0.4342, 0.4335, 0.4342, 0.4339, 0.4319, 0.4341, 0.411, 0.2522])
    ccplbm = Parameter(index=[regions], default = [-161.4521, -205.4176, -145.9539, -33.683, -91.0606, -201.8789, -190.3936, -136.8033, -54.1635, -78.4126, -80.232, 12.0899, -66.6796, -102.4339, -49.97, -10.4503])
    ccmlbm = Parameter(index=[regions], default = [151.6768, 195.6424, 19.2327, 65.5934, 67.1775, 61.484, -3.4422, -2.4508, -0.6855, 16.6942, -1.6072, -0.6838, 81.1077, -1.9826, -1.0407, 1.6035])
    ccpqbm = Parameter(index=[regions], default = [2.8314, 2.8314, 2.8279, 1.2018, 2.8314, 2.8314, 2.8314, 2.7443, 2.7085, 2.8094, 2.8314, -1.1081, 2.0193, 2.8314, 2.6771, 0.5138])
    ccmqbm = Parameter(index=[regions], default = [-155.1251, -199.0906, -21.7191, -67.185, -68.9576, -65.2217, 0.0473, 0.0457, -0.484, -18.2021, 0.0473, 0.0413, -84.8815, 0.0473, 0.0448, -2.3428])

    plus = Parameter(index=[time,regions])
    temp = Parameter(index=[time,regions])
    urbpop = Parameter(index=[time,regions])
    population = Parameter(index=[time,regions])

    cvlin = Parameter(default = 0.025901)
    rlin = Parameter(default = 0.001583)
    maxcardvasc = Parameter(default = 0.05)

    function run_timestep(p, v, d, t)

        if !is_first(t)
            for r in d.regions
                v.basecardvasc[t, r] = p.cardvasc90[r] + p.cvlin * (p.plus[t, r] - p.plus90[r])
                if v.basecardvasc[t, r] > 1.0
                    v.basecardvasc[t, r] = 1.0
                end

                v.baseresp[t, r] = p.resp90[r] + p.rlin * (p.plus[t, r] - p.plus90[r])
                if v.baseresp[t, r] > 1.0
                    v.baseresp[t, r] = 1.0
                end

                v.cardheat[t, r] = (p.chplbm[r] * p.plus[t, r] + p.chmlbm[r] * (1.0 - p.plus[t, r])) * p.temp[t, r] +
                        (p.chpqbm[r] * p.plus[t, r] + p.chmqbm[r] * (1.0 - p.plus[t, r])) * p.temp[t, r]^2
                v.cardheat[t, r] = v.cardheat[t, r] * p.urbpop[t, r] * p.population[t, r] * 10
                if v.cardheat[t, r] > 1000.0 * p.maxcardvasc * v.basecardvasc[t, r] * p.urbpop[t, r] * p.population[t, r]
                    v.cardheat[t, r] = 1000 * p.maxcardvasc * v.basecardvasc[t, r] * p.urbpop[t, r] * p.population[t, r]
                end
                if v.cardheat[t, r] < 0.0
                    v.cardheat[t, r] = 0
                end

                v.resp[t, r] = p.rlbm[r] * p.temp[t, r] + p.rqbm[r] * p.temp[t, r]^2
                v.resp[t, r] = v.resp[t, r] * p.urbpop[t, r] * p.population[t, r] * 10
                if v.resp[t, r] > 1000 * p.maxcardvasc * v.baseresp[t, r] * p.urbpop[t, r] * p.population[t, r]
                    v.resp[t, r] = 1000 * p.maxcardvasc * v.baseresp[t, r] * p.urbpop[t, r] * p.population[t, r]
                end
                if v.resp[t, r] < 0
                    v.resp[t, r] = 0
                end

                v.cardcold[t, r] = (p.ccplbm[r] * p.plus[t, r] + p.ccmlbm[r] * (1.0 - p.plus[t, r])) * p.temp[t, r] +
                        (p.ccpqbm[r] * p.plus[t, r] + p.ccmqbm[r] * (1.0 - p.plus[t, r])) * p.temp[t, r]^2
                v.cardcold[t, r] = v.cardcold[t, r] * p.population[t, r] * 10
                if v.cardcold[t, r] < -1000 * p.maxcardvasc * v.basecardvasc[t, r] * p.population[t, r]
                    v.cardcold[t, r] = -1000 * p.maxcardvasc * v.basecardvasc[t, r] * p.population[t, r]
                end
                if v.cardcold[t, r] > 0
                    v.cardcold[t, r] = 0
                end
            end
        end
    end
end