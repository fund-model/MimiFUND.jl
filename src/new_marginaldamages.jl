import Mimi.compinstance

"""
compute_scc(m::Model=get_model(); emissionyear::Int = nothing, gas::Symbol = :C, yearstorun::Int = 1050, useequityweights::Bool = false, eta::Float64 = 0., prtp::Float64 = 0.03)

Computes the social cost of carbon (or other gas if specified) for an emissions pulse in `emissionyear`
for the provided MimiFUND model. If no model is provided, the default model from MimiFUND.get_model() is used.
The discount factor is computed from the specified `eta` and pure rate of time preference `prtp`.
Optional regional equity weighting can be used by specifying `useequityweights=true`. 

TODO: 
- what should the default eta and prtp be?
- This returns social cost of CO2 not carbon, should we multiply by 12/44 if gas==:C ?
- `emissionyear` or `year` keyword
- Should we keep the yearstorun keyword, or should we allow users to specify a `horizon` keyword representing the final year instead?
"""
function compute_scc(m::Model=get_model(); emissionyear::Int = nothing, gas::Symbol = :C, yearstorun::Int = 1050, useequityweights::Bool = false, eta::Float64 = 0., prtp::Float64 = 0.03)

    emissionyear === nothing ? error("Must specify an emissionyear. Try `compute_scc(m, emissionyear=2020)`.") : nothing

    mm = get_marginal_model(m; emissionyear = emissionyear, gas = gas)

    return compute_scc(mm, emissionyear=emissionyear, gas=gas, yearstorun=yearstorun, useequityweights=useequityweights, eta=eta, prtp=prtp)
end

"""
compute_scc(mm::MarginalModel; emissionyear::Int = nothing, gas::Symbol = :C, yearstorun::Int = 1050, useequityweights::Bool = false, eta::Float64 = 0., prtp::Float64 = 0.03)

Computes the social cost of carbon (or other gas if specified) for an emissions pulse in `emissionyear`
for the provided MimiFUND MarginalModel. The discount factor is computed from the specified `eta` and pure rate of time preference `prtp`.
Optional regional equity weighting can be used by specifying useequityweights=true. 
"""
function compute_scc(mm::MarginalModel; emissionyear::Int = nothing, gas::Symbol = :C, yearstorun::Int = 1050, useequityweights::Bool = false, eta::Float64 = 0., prtp::Float64 = 0.03)

    # Run the model
    run(mm; ntimesteps = yearstorun + 1)
    m1, m2 = mm.base, mm.marginal

    damage1 = m1[:impactaggregation, :loss]
    # Take out growth effect effect of run 2 by transforming the damage from run 2 into % of GDP of run 2, and then multiplying that with GDP of run 1
    damage2 = m2[:impactaggregation, :loss] ./ m2[:socioeconomic, :income] .* m1[:socioeconomic, :income]

    # Calculate the marginal damage between run 1 and 2 for each year/region
    marginaldamage = (damage2 .- damage1) / 10000000.0  # The pulse was 1 MtCO2 for ten years, so divide by 10^7

    ypc = m1[:socioeconomic, :ypc]

    # Compute discount factor with or without equityweights
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

    # Compute global SCC
    scc = sum(marginaldamage[2:yearstorun, :] .* df[2:yearstorun, :]) #* (gas == :C ? 12./44. : 1) # convert to carbon from co2?
    return scc
end

"""
get_marginal_model(m::Model = get_model(); emissionyear::Int = nothing, gas::Symbol = :C)

Creates a Mimi MarginalModel where the provided m is the base model, and the marginal model has additional emissions of gas `gas` in year `emissionyear`.
If no Model m is provided, the default model from MimiFUND.get_model() is used as the base model.
"""
function get_marginal_model(m::Model = get_model(); emissionyear::Int = nothing, gas::Symbol = :C)
    emissionyear == nothing ? error("Must specify emissionyear. Try `get_marginal_models(m, emissionyear=2020)`.") : nothing 

    mm = create_marginal_model(m)
    add_marginal_emissions!(mm.marginal, emissionyear; gas = gas)

    return mm
end

"""
Creates a MarginalModel of FUND with additional emissions in the specified year for the specified gas. 
"""
function create_marginal_FUND_model(; gas = :C, emissionyear = 2010, parameters = nothing, yearstorun = 1050)

    # Get default FUND model
    FUND = get_model(nsteps = yearstorun, params = parameters)

    # Build marginal model
    mm = create_marginal_model(FUND)
    m1, m2 = mm.base, mm.marginal

    add_marginal_emissions!(m2, emissionyear; gas = gas)

    Mimi.build(m1)
    Mimi.build(m2)
    return mm 
end 

"""
Adds a marginalemission component to m, and sets the additional emissions if a year is specified.
"""
function add_marginal_emissions!(m, emissionyear = nothing; gas = :C)

    # Add additional emissions to m
    add_comp!(m, Mimi.adder, :marginalemission, before = :climateco2cycle, first = 1951)
    nyears = length(Mimi.time_labels(m))
    addem = zeros(nyears - 1)   # starts one year later, in 1951
    if emissionyear != nothing 
        addem[getindexfromyear(emissionyear)-1:getindexfromyear(emissionyear) + 8] .= 1.0
    end
    set_param!(m, :marginalemission, :add, addem)

    # Reconnect the appropriate emissions in m
    if gas == :C
        connect_param!(m, :marginalemission, :input, :emissions, :mco2)
        connect_param!(m, :climateco2cycle, :mco2, :marginalemission, :output, repeat([missing], nyears))
    elseif gas == :CH4
        connect_param!(m, :marginalemission, :input, :emissions, :globch4)
        connect_param!(m, :climatech4cycle, :globch4, :marginalemission, :output, repeat([missing], nyears))
    elseif gas == :N2O
        connect_param!(m, :marginalemission, :input, :emissions, :globn2o)
        connect_param!(m, :climaten2ocycle, :globn2o, :marginalemission, :output, repeat([missing], nyears))
    elseif gas == :SF6
        connect_param!(m, :marginalemission, :input, :emissions,:globsf6)
        connect_param!(m, :climatesf6cycle, :globsf6, :marginalemission, :output, repeat([missing], nyears))
    else
        error("Unknown gas: $gas")
    end

end 

"""
Helper function to set the marginal emissions in the specified year.
"""
function perturb_marginal_emissions!(m::Model, emissionyear; comp_name = :marginalemission)

    ci = compinstance(m, comp_name)
    emissions = Mimi.get_param_value(ci, :add)

    nyears = length(Mimi.dimension(m, :time))
    new_em = zeros(nyears - 1)
    new_em[getindexfromyear(emissionyear)-1:getindexfromyear(emissionyear) + 8] .= 1.0
    emissions[:] = new_em

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