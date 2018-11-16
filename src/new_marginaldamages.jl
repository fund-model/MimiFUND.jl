using Mimi

include("helper.jl")
include("fund.jl")
using Fund 


"""
Creates a MarginalModel of FUND with additional emissions in the specified year for the specified gas. 
"""
function create_marginal_FUND_model(; gas = :C, emissionyear = 2010, parameters = nothing, yearstorun = 1050)

    # Get default FUND model
    FUND = getfund(nsteps = yearstorun, params = parameters)

    # Build marginal model
    mm = create_marginal_model(FUND)
    m1, m2 = mm.base, mm.marginal

    # Add additional emissions to m2
    add_comp!(m2, adder, :marginalemission, before = :climateco2cycle)
    addem = zeros(yearstorun + 1)
    addem[getindexfromyear(emissionyear):getindexfromyear(emissionyear) + 9] = 1.0
    set_param!(m2, :marginalemission, :add, addem)

    # Reconnect the appropriate emissions in m2
    if gas == :C
        connect_param!(m2, :marginalemission, :input, :emissions, :mco2)
        connect_param!(m2, :climateco2cycle, :mco2, :marginalemission, :output)
    elseif gas == :CH4
        connect_param!(m2, :marginalemission, :input, :emissions, :globch4)
        connect_param!(m2, :climatech4cycle, :globch4, :marginalemission, :output)
    elseif gas == :N2O
        connect_param!(m2, :marginalemission, :input, :emissions, :globn2o)
        connect_param!(m2, :climaten2ocycle, :globn2o, :marginalemission, :output)
    elseif gas == :SF6
        connect_param!(m2, :marginalemission, :input, :emissions,:globsf6)
        connect_param!(m2, :climatesf6cycle, :globsf6, :marginalemission, :output)
    else
        error("Unknown gas.")
    end

    Mimi.build(m1)
    Mimi.build(m2)
    return mm 
end 


"""
Returns the social cost per one ton of additional emissions of the specified gas in the specified year. 
Uses the specified eta and prtp for discounting, with the option to use equity weights.
"""
function get_social_cost(; emissionyear = 2010, parameters = nothing, yearstoaggregate = 1000, gas = :C, useequityweights = false, eta = 1.0, prtp = 0.001)

    # Get marginal model
    yearstorun = min(1050, getindexfromyear(emissionyear) + yearstoaggregate)
    mm = create_marginal_FUND_model(emissionyear = emissionyear, parameters = parameters, yearstorun = yearstorun, gas = :C)
    run(mm)
    m1, m2 = mm.base, mm.marginal

    damage1 = m1[:impactaggregation, :loss]
    # Take out growth effect effect of run 2 by transforming the damage from run 2 into % of GDP of run 2, and then multiplying that with GDP of run 1
    damage2 = m2[:impactaggregation, :loss] ./ m2[:socioeconomic, :income] .* m1[:socioeconomic, :income]

    # Calculate the marginal damage between run 1 and 2 for each
    # year/region
    marginaldamage = (damage2 .- damage1) / 10000000.0

    ypc = m1[:socioeconomic, :ypc]

    df = zeros(yearstorun + 1, 16)
    if !useequityweights
        for r = 1:16
            x = 1.
            for t = getindexfromyear(emissionyear):yearstorun
                df[t, r] = x
                gr = (ypc[t, r] - ypc[t - 1, r]) / ypc[t - 1,r]
                x = x / (1. + prtp + eta * gr)
            end
        end
    else
        globalypc = m1[:socioeconomic, :globalypc]
        df = Float64[t >= getindexfromyear(emissionyear) ? (globalypc[getindexfromyear(emissionyear)] / ypc[t, r]) ^ eta / (1.0 + prtp) ^ (t - getindexfromyear(emissionyear)) : 0.0 for t = 1:yearstorun + 1, r = 1:16]
    end 

    scc = sum(marginaldamage[2:end, :] .* df[2:end, :])
    return scc

end


"""
Returns a matrix of marginal damages per one ton of additional emissions of the specified gas in the specified year.
"""
function getmarginaldamages(; emissionyear=2010, parameters = nothing, yearstoaggregate = 1000, gas = :C) 

    # Get marginal model
    yearstorun = min(1050, getindexfromyear(emissionyear) + yearstoaggregate)
    mm = create_marginal_FUND_model(emissionyear = emissionyear, parameters = parameters, yearstorun = yearstorun, gas = :C)
    run(mm)

    # Get damages
    marginaldamages = mm[:impactaggregation, :loss] / 10000000.0
    return marginaldamages
end