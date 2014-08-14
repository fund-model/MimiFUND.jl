using IAMF

@defcomp emissions begin
    regions = Index()

    mitigationcost = Variable(index=[time,regions])
    ch4cost = Variable(index=[time,regions])
    ch4costindollar = Variable(index=[time,regions])
    n2ocost = Variable(index=[time,regions])
    ryg = Variable(index=[time,regions])
    energint = Variable(index=[time,regions])
    emissint = Variable(index=[time,regions])
    emission = Variable(index=[time,regions])
    emissionwithforestry = Variable(index=[time,regions])
    sf6 = Variable(index=[time,regions])
    reei = Variable(index=[time,regions])
    rcei = Variable(index=[time,regions])
    energuse = Variable(index=[time,regions])
    seei = Variable(index=[time,regions])
    scei = Variable(index=[time,regions])
    ch4 = Variable(index=[time,regions])
    n2o = Variable(index=[time,regions])
    n2ored = Variable(index=[time,regions])
    taxpar = Variable(index=[time,regions])
    co2red = Variable(index=[time,regions])
    know = Variable(index=[time,regions])
    perm = Variable(index=[time,regions])
    cumaeei = Variable(index=[time,regions])
    ch4red = Variable(index=[time,regions])

    minint = Variable(index=[time])
    globknow = Variable(index=[time])

    mco2 = Variable(index=[time])
    globch4 = Variable(index=[time])
    globn2o = Variable(index=[time])
    globsf6 = Variable(index=[time])

    cumglobco2 = Variable(index=[time])
    cumglobch4 = Variable(index=[time])
    cumglobn2o = Variable(index=[time])
    cumglobsf6 = Variable(index=[time])

    taxmp = Parameter(index=[regions])
    sf60 = Parameter(index=[regions])
    GDP90 = Parameter(index=[regions])
    pop90 = Parameter(index=[regions])
    ch4par1 = Parameter(index=[regions])
    ch4par2 = Parameter(index=[regions])
    n2opar1 = Parameter(index=[regions])
    n2opar2 = Parameter(index=[regions])
    gdp0 = Parameter(index=[regions])
    emissint0 = Parameter(index=[regions])

    forestemm = Parameter(index=[time,regions])
    aeei = Parameter(index=[time,regions])
    acei = Parameter(index=[time,regions])
    ch4em = Parameter(index=[time,regions])
    n2oem = Parameter(index=[time,regions])
    currtax = Parameter(index=[time,regions])
    currtaxch4 = Parameter(index=[time,regions])
    currtaxn2o = Parameter(index=[time,regions])
#    pgrowth = Parameter(index=[time,regions])
    ypcgrowth = Parameter(index=[time,regions])
    income = Parameter(index=[time,regions])
    population = Parameter(index=[time,regions])

    sf6gdp = Parameter()
    sf6ypc = Parameter()
    knowpar = Parameter()
    knowgpar = Parameter()
    gwpch4 = Parameter()
    gwpn2o = Parameter()

    TaxConstant = Parameter()
    TaxEmInt = Parameter()
    TaxThreshold = Parameter()
    TaxDepreciation = Parameter()
    MaxCostFall = Parameter()

    ch4add = Parameter()
    n2oadd = Parameter()
    sf6add = Parameter()
end

function timestep(s::emissions, t::Int)
    v = s.Variables
    p = s.Parameters
    d = s.Dimensions

    if t==1
        for r in d.regions
            v.energint[t, r] = 1
            v.energuse[t, r] = p.gdp0[r]
            v.emissint[t, r] = p.emissint0[r]
            v.emission[t, r] = v.emissint[t, r] / v.energuse[t, r]
            v.ch4cost[t, r] = 0
            v.n2ocost[t, r] = 0
            v.ryg[t, r] = 0
            v.reei[t, r] = 0
            v.rcei[t, r] = 0
            v.seei[t, r] = 0
            v.scei[t, r] = 0
            v.co2red[t, r] = 0
            v.know[t, r] = 1
            v.ch4red[t, r] = 0
            v.n2ored[t, r] = 0
            v.mitigationcost[t, r] = 0
        end

        v.globknow[t] = 1
        v.cumglobco2[t] = 0.0
        v.cumglobch4[t] = 0.0
        v.cumglobn2o[t] = 0.0
        v.cumglobsf6[t] = 0.0

        # SocioEconomicState.minint[t]=Inf
        minint = Inf
        for r in d.regions
            if v.emission[t, r] / p.income[t, r] < minint
                minint = v.emission[t, r] / p.income[t, r]
            end
        end
        # v.minint[t] = minint
        v.minint[t] = 0
    else
        # Calculate emission and carbon intensity
        for r in d.regions
            v.energint[t, r] = (1.0 - 0.01 * p.aeei[t, r] - v.reei[t - 1, r]) * v.energint[t - 1, r]
            v.emissint[t, r] = (1.0 - 0.01 * p.acei[t, r] - v.rcei[t - 1, r]) * v.emissint[t - 1, r]
        end

        # Calculate sf6 emissions
        for r in d.regions
            v.sf6[t, r] = (p.sf60[r] + p.sf6gdp * (p.income[t, r] - p.GDP90[r]) + p.sf6ypc * (p.income[t - 1, r] / p.population[t - 1, r] - p.GDP90[r] / p.pop90[r])) * (t <= 60 ? 1 + (t - 40.0) / 40.0 : 1.0 + (60.0 - 40.0) / 40.0) * (t > 60 ? 0.99^(t - 60.0) : 1.0)
        end

        # Check for unrealistic values
        for r in d.regions
            if v.sf6[t, r] < 0.0
                v.sf6[t, r] = 0
            end
        end

        # Calculate energy use
        for r in d.regions
            v.energuse[t, r] = (1 - v.seei[t - 1, r]) * v.energint[t, r] * p.income[t, r]
        end

        # Calculate co2 emissions
        for r in d.regions
            v.emission[t, r] = (1 - v.scei[t - 1, r]) * v.emissint[t, r] * v.energuse[t, r]
            v.emissionwithforestry[t, r] = v.emission[t, r] + p.forestemm[t, r]
        end

        # Calculate ch4 emissions
        for r in d.regions
            v.ch4[t, r] = p.ch4em[t, r] * (1 - v.ch4red[t - 1, r])
        end

        # Calculate n2o emissions
        for r in d.regions
            v.n2o[t, r] = p.n2oem[t, r] * (1 - v.n2ored[t - 1, r])
        end


        # TODO RT check
        for r in d.regions
            if (v.emission[t, r] / p.income[t, r] - v.minint[t - 1] <= 0)
                v.taxpar[t, r] = p.TaxConstant
            else
                v.taxpar[t, r] = p.TaxConstant - p.TaxEmInt * sqrt(v.emission[t, r] / p.income[t, r] - v.minint[t - 1])
            end
        end

        for r in d.regions
            v.co2red[t, r] = p.currtax[t, r] * v.emission[t, r] * v.know[t - 1, r] * v.globknow[t - 1] / 2 / v.taxpar[t, r] / p.income[t, r] / 1000

            if (v.co2red[t, r] < 0)
                v.co2red[t, r] = 0
            elseif (v.co2red[t, r] > 0.99)
                v.co2red[t, r] = 0.99
            end
        end

        for r in d.regions
            v.ryg[t, r] = v.taxpar[t, r] * v.co2red[t, r]^2 / v.know[t - 1, r] / v.globknow[t - 1]
        end

        # TODO RT check
        for r in d.regions
            v.perm[t, r] = 1.0 - 1.0 / p.TaxThreshold * p.currtax[t, r] / (1 + 1.0 / p.TaxThreshold * p.currtax[t, r])
        end

        for r in d.regions
            v.reei[t, r] = v.perm[t, r] * 0.5 * v.co2red[t, r]
        end

        # TODO RT check
        for r in d.regions
            if (p.currtax[t, r] < p.TaxThreshold)
                v.rcei[t, r] = v.perm[t, r] * 0.5 * v.co2red[t, r]^2
            else
                v.rcei[t, r] = v.perm[t, r] * 0.5 * v.co2red[t, r]
            end
        end

        # TODO RT check
        # TODO RT what is the 1.7?
        for r in d.regions
            v.seei[t, r] = (1.0 - p.TaxDepreciation) * v.seei[t - 1, r] + (1.0 - v.perm[t, r]) * 0.5 * v.co2red[t, r] * 1.7
        end

        for r in d.regions
            if (p.currtax[t, r] < 100)
                v.scei[t, r] = 0.9 * v.scei[t - 1, r] + (1 - v.perm[t, r]) * 0.5 * v.co2red[t, r]^2
            else
                v.scei[t, r] = 0.9 * v.scei[t - 1, r] + (1 - v.perm[t, r]) * 0.5 * v.co2red[t, r] * 1.7
            end
        end

        # TODO RT check
        for r in d.regions
            v.know[t, r] = v.know[t - 1, r] * sqrt(1 + p.knowpar * v.co2red[t, r])

            if (v.know[t, r] > sqrt(p.MaxCostFall))
                v.know[t, r] = sqrt(p.MaxCostFall)
            end
        end

        v.globknow[t] = v.globknow[t - 1]
        for r in d.regions
            v.globknow[t] = v.globknow[t] * sqrt(1 + p.knowgpar * v.co2red[t, r])
        end

        if (v.globknow[t] > 3.16)
            v.globknow[t] = 3.16
        end

        for r in d.regions
            v.ch4red[t, r] = p.currtaxch4[t, r] * p.ch4em[t, r] / 2 / p.ch4par1[r] / p.ch4par2[r] / p.ch4par2[r] / p.income[t, r] / 1000

            if (v.ch4red[t, r] < 0)
                v.ch4red[t, r] = 0
            elseif (v.ch4red[t, r] > 0.99)
                v.ch4red[t, r] = 0.99
            end
        end

        for r in d.regions
            v.n2ored[t, r] = p.gwpn2o * p.currtaxn2o[t, r] * p.n2oem[t, r] / 2 / p.n2opar1[r] / p.n2opar2[r] / p.n2opar2[r] / p.income[t, r] / 1000

            if (v.n2ored[t, r] < 0)
                v.n2ored[t, r] = 0
            elseif (v.n2ored[t, r] > 0.99)
                v.n2ored[t, r] = 0.99
            end
        end

        for r in d.regions
            v.ch4cost[t, r] = p.ch4par1[r] * p.ch4par2[r]^2 * v.ch4red[t, r]^2
            v.ch4costindollar[t, r] = v.ch4cost[t, r] * p.income[t, r]
        end

        for r in d.regions
            v.n2ocost[t, r] = p.n2opar1[r] * p.n2opar2[r]^2 * v.n2ored[t, r]^2
        end

        minint = Inf
        for r in d.regions
            if (v.emission[t, r] / p.income[t, r] < minint)
                minint = v.emission[t, r] / p.income[t, r]
            end
        end
        v.minint[t] = minint

        for r in d.regions
            if t > 60
                v.cumaeei[t, r] = v.cumaeei[t - 1, r] * (1.0 - 0.01 * p.aeei[t, r] - v.reei[t, r] + v.seei[t - 1, r] - v.seei[t, r])
            else
                v.cumaeei[t, r] = 1.0
            end
        end

        for r in d.regions
            #v.mitigationcost[t, r] = (p.taxmp[r] * v.ryg[t, r] /*+ v.ch4cost[t, r]*/ + v.n2ocost[t, r]) * p.income[t, r]
            v.mitigationcost[t, r] = (p.taxmp[r] * v.ryg[t, r] + v.n2ocost[t, r]) * p.income[t, r]
        end

        globco2 = 0.
        globch4 = 0.
        globn2o = 0.
        globsf6 = 0.

        for r in d.regions
            globco2 = globco2 + v.emissionwithforestry[t, r]
            globch4 = globch4 + v.ch4[t, r]
            globn2o = globn2o + v.n2o[t, r]
            globsf6 = globsf6 + v.sf6[t, r]
        end

        v.mco2[t] = globco2
        v.globch4[t] = max(0.0, globch4 + (t > 50 ? p.ch4add * (t - 50) : 0.0))
        v.globn2o[t] = max(0.0, globn2o + (t > 50 ? p.n2oadd * (t - 50) : 0))
        v.globsf6[t] = max(0.0, globsf6 + (t > 50 ? p.sf6add * (t - 50) : 0.0))

        v.cumglobco2[t] = v.cumglobco2[t - 1] + v.mco2[t]
        v.cumglobch4[t] = v.cumglobch4[t - 1] + v.globch4[t]
        v.cumglobn2o[t] = v.cumglobn2o[t - 1] + v.globn2o[t]
        v.cumglobsf6[t] = v.cumglobsf6[t - 1] + v.globsf6[t]
    end
end
