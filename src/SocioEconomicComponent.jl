using Mimi

@defcomp socioeconomic begin
	regions = Index()

	income = Variable(index=[time,regions])
	consumption = Variable(index=[time,regions])
	ypc = Variable(index=[time,regions])
	ygrowth = Variable(index=[time,regions])

	plus = Variable(index=[time,regions])
	urbpop = Variable(index=[time,regions])
	popdens = Variable(index=[time,regions])

	globalconsumption = Variable(index=[time])
	globalypc = Variable(index=[time])
	globalincome = Variable(index=[time])

	ypc90 = Variable(index=[regions])


	pgrowth             = Parameter(index=[time,regions])
	ypcgrowth           = Parameter(index=[time,regions])
	eloss               = Parameter(index=[time,regions])
	sloss               = Parameter(index=[time,regions])
	mitigationcost      = Parameter(index=[time,regions])
	area                = Parameter(index=[time,regions])
	globalpopulation    = Parameter(index=[time])
	population          = Parameter(index=[time,regions])
	populationin1       = Parameter(index=[time,regions])

	plus90  = Parameter(index=[regions], default = [0.1239, 0.1122, 0.1473, 0.149, 0.1115, 0.1034, 0.0931, 0.0358, 0.0384, 0.0486, 0.0406, 0.04, 0.0856, 0.0373, 0.0291, 0.0604])
	gdp90   = Parameter(index=[regions], default = [6343.0, 532.5, 8285.4, 5100.7, 359.5, 338.1, 915.1, 447.3, 296.0, 1078.0, 351.5, 456.2, 511.4, 137.3, 275.1, 37.2])
	pop90   = Parameter(index=[regions], default = [254.1, 27.8, 376.6, 166.4, 20.2, 124.3, 289.6, 189.2, 106.3, 300.2, 1131.7, 444.1, 1184.1, 117.8, 494.4, 39.9])
	urbcorr = Parameter(index=[regions], default = [0.1163, 0.0659, 0.1578, 0.1558, -0.0516, 0.0051, -0.0121, -0.0391, -0.0892, -0.1592, 0.3132, 0.8026, 0.6088, 0.0611, 0.3879, 0.0095])
	gdp0    = Parameter(index=[regions], default = [1643.89944236897, 84.8224831391005, 1913.31508308279, 616.021980672332, 119.057728115324, 87.9192108941911, 167.308878761848, 76.0649829670573, 40.5139010341431, 193.138523290388, 57.9713701148353, 25.6942546016562, 18.8014266052303, 13.4482462152325, 94.6859977634632, 6.82113737296979])


	runwithoutdamage::Bool = Parameter(default = false)
	consleak    = Parameter(default = 0.25)
	plusel      = Parameter(default = 0.25)
    savingsrate = Parameter(default = 0.2)

    function run_timestep(p, v, d, t)

        if is_first(t)
            for r in d.regions
                v.income[t, r] = p.gdp0[r]
                v.ypc[t, r] = v.income[t, r] / p.population[t, r] * 1000.0
                v.consumption[t, r] = v.income[t, r] * 1000000000.0 * (1.0 - p.savingsrate)
            end

            v.globalconsumption[t] = sum(v.consumption[t,:])

            for r in d.regions
                v.ypc90[r] = p.gdp90[r] / p.pop90[r] * 1000
            end
        else

            # Calculate income growth rate
            for r in d.regions
                v.ygrowth[t, r] = (1 + 0.01 * p.pgrowth[t - 1, r]) * (1 + 0.01 * p.ypcgrowth[t - 1, r]) - 1
            end

            # Calculate income
            for r in d.regions
                oldincome = v.income[t - 1, r] - (gettime(t) >= 1990 && !p.runwithoutdamage ? p.consleak * p.eloss[t - 1, r] / 10.0 : 0)

                v.income[t, r] = (1 + v.ygrowth[t, r]) * oldincome - p.mitigationcost[t - 1, r]
            end

            # Check for unrealistic values
            for r in d.regions
                if v.income[t, r] < 0.01 * p.population[t, r]
                    v.income[t, r] = 0.1 * p.population[t, r]
                end
            end

            for r in d.regions
                v.ypc[t, r] = v.income[t, r] / p.population[t, r] * 1000.0
            end

            for r in d.regions
                v.consumption[t, r] = max(v.income[t, r] * 1000000000.0 * (1.0 - p.savingsrate) - (p.runwithoutdamage ? 0.0 :   (p.eloss[t - 1, r] + p.sloss[t - 1, r]) * 1000000000.0),0.0)
            end
            v.globalconsumption[t] = sum(v.consumption[t,:])

            for r in d.regions
                v.plus[t, r] = p.plus90[r] * (v.ypc[t, r] / v.ypc90[r])^p.plusel

                if v.plus[t, r] > 1
                    v.plus[t, r] = 1.0
                end
            end

            for r in d.regions
                v.popdens[t, r] = p.population[t, r] / p.area[t, r] * 1000000.0
            end

            for r in d.regions
                # ERROR This doesn't make sense for t < 40
                v.urbpop[t, r] = (0.031 * sqrt(v.ypc[t, r]) - 0.011 * sqrt(v.popdens[t, r])) / (1.0 + 0.031 * sqrt(v.ypc[t,r]) - 0.011 * sqrt(v.popdens[t, r])) / (1 + p.urbcorr[r] / (1 + 0.001 * (gettime(t) - 1990)^2.))
                # DA: urbcorr needs to be changed to a function if this is to be made uncertain
            end

            v.globalincome[t] = sum(v.income[t,:])

            v.globalypc[t] = sum(v.income[t,:] .* 1000000000.0) / sum(p.populationin1[t,:])
        end
    end
end