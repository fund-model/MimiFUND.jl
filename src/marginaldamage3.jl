include("fund.jl")

function getmarginalmodels(;gas=:C,emissionyear=2010,parameters=nothing,yearstorun=1050)
    m1 = getfund(nsteps=yearstorun,params=parameters)

    m2 = getfund(nsteps=yearstorun,params=parameters)
    addcomponent(m2, adder, :marginalemission, before=:climateco2cycle)
    addem = zeros(yearstorun+1)
    addem[getindexfromyear(emissionyear):getindexfromyear(emissionyear)+9] = 1.0
    setparameter(m2,:marginalemission,:add,addem)
    if gas==:C
        connectparameter(m2,:marginalemission,:input,:emissions,:mco2)
        connectparameter(m2, :climateco2cycle,:mco2,:marginalemission,:output)
    elseif gas==:CH4
        connectparameter(m2,:marginalemission,:input,:emissions,:globch4)
        connectparameter(m2, :climatech4cycle,:globch4,:marginalemission,:output)
    elseif gas==:N2O
        connectparameter(m2,:marginalemission,:input,:emissions,:globn2o)
        connectparameter(m2, :climaten2ocycle,:globn2o,:marginalemission,:output)
    elseif gas==:SF6
        connectparameter(m2,:marginalemission,:input,:emissions,:globsf6)
        connectparameter(m2, :climatesf6cycle,:globsf6,:marginalemission,:output)
    else
        error("Unknown gas.")
    end

    run(m1)
    run(m2)

    return m1, m2
end

function marginaldamage3(;emissionyear=2010,parameters=nothing,yearstoaggregate=1000,gas=:C,useequityweights=false,eta=1.0,prtp=0.001)
    yearstorun = min(1050, getindexfromyear(emissionyear) + yearstoaggregate)

    m1, m2 = getmarginalmodels(emissionyear=emissionyear, parameters=parameters,yearstorun=yearstorun,gas=gas)

    damage1 = m1[:impactaggregation,:loss]
    # Take out growth effect effect of run 2 by transforming
    # the damage from run 2 into % of GDP of run 2, and then
    # multiplying that with GDP of run 1
    damage2 = m2[:impactaggregation,:loss]./m2[:socioeconomic,:income].*m1[:socioeconomic,:income]

    # Calculate the marginal damage between run 1 and 2 for each
    # year/region
    marginaldamage = (damage2.-damage1)/10000000.0

    ypc = m1[:socioeconomic,:ypc]

    df = zeros(yearstorun+1,16)
    if !useequityweights
        for r=1:16
            x = 1.
            for t=getindexfromyear(emissionyear):yearstorun
                df[t,r] = x
                gr = (ypc[t,r]-ypc[t-1,r])/ypc[t-1,r]
                x = x / (1. + prtp + eta * gr)
            end
        end
    else
        globalypc = m1[:socioeconomic,:globalypc]
        df = float64([t>=getindexfromyear(emissionyear) ? (globalypc[getindexfromyear(emissionyear)]/ypc[t,r])^eta / (1.0+prtp)^(t-getindexfromyear(emissionyear)) : 0.0 for t=1:yearstorun,r=1:16])
    end

    scc = sum(marginaldamage[2:end,:].*df[2:end,:])

    return scc
end
