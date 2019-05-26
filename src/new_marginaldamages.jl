import Mimi.compinstance

"""
compute_scc(m::Model=get_model(); year::Int = nothing, gas::Symbol = :CO2, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015)

Computes the social cost of CO2 (or other gas if specified) for an emissions pulse in `year`
for the provided MimiFUND model. If no model is provided, the default model from MimiFUND.get_model() is used.
The discount factor is computed from the specified `eta` and pure rate of time preference `prtp`.
Optional regional equity weighting can be used by specifying `equity_weights=true`. 
"""
function compute_scc(m::Model=get_model(); year::Union{Int, Nothing} = nothing, gas::Symbol = :CO2, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015)

    year === nothing ? error("Must specify an emission year. Try `compute_scc(m, year=2020)`.") : nothing
    !(last_year in 1950:3000) ? error("Invlaid value of $last_year for last_year. last_year must be within the model's time index 1950:3000.") : nothing
    !(year in 1950:last_year) ? error("Cannot compute the scc for year $year, year must be within the model's time index 1950:$last_year.") : nothing

    mm = get_marginal_model(m; year = year, gas = gas)

    return _compute_scc(mm, year=year, gas=gas, last_year=last_year, equity_weights=equity_weights, eta=eta, prtp=prtp)
end

"""
compute_scc_mm(m::Model=get_model(); year::Union{Int, Nothing} = nothing, gas::Symbol = :CO2, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015)

Returns a NamedTuple (scc=scc, mm=mm) of the social cost of carbon and the MarginalModel used to compute it.
Computes the social cost of CO2 (or other gas if specified) for an emissions pulse in `year`
for the provided MimiFUND model. If no model is provided, the default model from MimiFUND.get_model() is used.
The discount factor is computed from the specified `eta` and pure rate of time preference `prtp`.
Optional regional equity weighting can be used by specifying `equity_weights=true`. 
"""
function compute_scc_mm(m::Model=get_model(); year::Union{Int, Nothing} = nothing, gas::Symbol = :CO2, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015)
    year === nothing ? error("Must specify an emission year. Try `compute_scc_mm(m, year=2020)`.") : nothing
    !(last_year in 1950:3000) ? error("Invlaid value of $last_year for last_year. last_year must be within the model's time index 1950:3000.") : nothing
    !(year in 1950:last_year) ? error("Cannot compute the scc for year $year, year must be within the model's time index 1950:$last_year.") : nothing

    mm = get_marginal_model(m; year = year, gas = gas)
    scc = _compute_scc(mm; year=year, gas=gas, last_year=last_year, equity_weights=equity_weights, eta=eta, prtp=prtp)
    
    return (scc = scc, mm = mm)
end

# helper function for computing SCC from a MarginalModel, not to be exported
function _compute_scc(mm::MarginalModel; year::Int, gas::Symbol, last_year::Int, equity_weights::Bool, eta::Float64, prtp::Float64)
    ntimesteps = getindexfromyear(last_year)
    run(mm; ntimesteps = ntimesteps)

    marginaldamage = mm[:impactaggregation, :loss]

    ypc = mm.base[:socioeconomic, :ypc]

    # Compute discount factor with or without equityweights
    df = zeros(ntimesteps, 16)
    if !equity_weights
        for r = 1:16
            x = 1.
            for t = getindexfromyear(year):ntimesteps
                df[t, r] = x
                gr = (ypc[t, r] - ypc[t - 1, r]) / ypc[t - 1,r]
                x = x / (1. + prtp + eta * gr)
            end
        end
    else
        globalypc = mm.base[:socioeconomic, :globalypc]
        df = Float64[t >= getindexfromyear(year) ? (globalypc[getindexfromyear(year)] / ypc[t, r]) ^ eta / (1.0 + prtp) ^ (t - getindexfromyear(year)) : 0.0 for t = 1:ntimesteps, r = 1:16]
    end 

    # Compute global SCC
    scc = sum(marginaldamage[2:ntimesteps, :] .* df[2:ntimesteps, :]) #* (gas == :C ? 44./12. : 1) # convert to carbon from co2?
    return scc
end

"""
get_marginal_model(m::Model = get_model(); year::Int = nothing, gas::Symbol = :CO2)

Creates a Mimi MarginalModel where the provided m is the base model, and the marginal model has additional emissions of gas `gas` in year `year`.
If no Model m is provided, the default model from MimiFUND.get_model() is used as the base model.
"""
function get_marginal_model(m::Model = get_model(); year::Int = nothing, gas::Symbol = :CO2)
    year == nothing ? error("Must specify emission year. Try `get_marginal_model(m, year=2020)`.") : nothing 
    !(year in 1950:3000) ? error("Cannot add marginal emissions in $year, year must be within the model's time index 1950:3000.") : nothing

    mm = create_marginal_model(m, 1e7)  # pulse size is 1MtC for ten years
    add_marginal_emissions!(mm.marginal, year; gas = gas)

    return mm
end

"""
Adds a marginalemission component to m, and sets the additional emissions if a year is specified.
"""
function add_marginal_emissions!(m, year = nothing; gas = :CO2)

    # Add additional emissions to m
    add_comp!(m, Mimi.adder, :marginalemission, before = :climateco2cycle, first = 1951)
    nyears = length(Mimi.time_labels(m))
    addem = zeros(nyears - 1)   # starts one year later, in 1951
    if year != nothing 
        addem[getindexfromyear(year)-1:getindexfromyear(year) + 8] .= 1.0
    end
    set_param!(m, :marginalemission, :add, addem)

    # Reconnect the appropriate emissions in m
    if gas == :CO2
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
function perturb_marginal_emissions!(m::Model, year; comp_name = :marginalemission)

    ci = compinstance(m, comp_name)
    emissions = Mimi.get_param_value(ci, :add)

    nyears = length(Mimi.dimension(m, :time))
    new_em = zeros(nyears - 1)
    new_em[getindexfromyear(year)-1:getindexfromyear(year) + 8] .= 1.0
    emissions[:] = new_em

end

"""
Creates a MarginalModel of FUND with additional emissions in the specified year for the specified gas. 
"""
function create_marginal_FUND_model(; gas = :CO2, year = 2010, parameters = nothing, yearstorun = 1050)

    # Get default FUND model
    FUND = get_model(nsteps = yearstorun, params = parameters)

    # Build marginal model
    mm = create_marginal_model(FUND)
    m1, m2 = mm.base, mm.marginal

    add_marginal_emissions!(m2, year; gas = gas)

    Mimi.build(m1)
    Mimi.build(m2)
    return mm 
end 

"""
Returns the social cost per one ton of additional emissions of the specified gas in the specified year. 
Uses the specified eta and prtp for discounting, with the option to use equity weights.
"""
function get_social_cost(; year = 2010, parameters = nothing, yearstoaggregate = 1000, gas = :CO2, equity_weights = false, eta = 1.0, prtp = 0.001)

    # Get marginal model
    yearstorun = min(1050, getindexfromyear(year) + yearstoaggregate)
    mm = create_marginal_FUND_model(year = year, parameters = parameters, yearstorun = yearstorun, gas = gas)
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
    if !equity_weights
        for r = 1:16
            x = 1.
            for t = getindexfromyear(year):yearstorun
                df[t, r] = x
                gr = (ypc[t, r] - ypc[t - 1, r]) / ypc[t - 1,r]
                x = x / (1. + prtp + eta * gr)
            end
        end
    else
        globalypc = m1[:socioeconomic, :globalypc]
        df = Float64[t >= getindexfromyear(year) ? (globalypc[getindexfromyear(year)] / ypc[t, r]) ^ eta / (1.0 + prtp) ^ (t - getindexfromyear(year)) : 0.0 for t = 1:yearstorun + 1, r = 1:16]
    end 

    scc = sum(marginaldamage[2:end, :] .* df[2:end, :])
    return scc

end

"""
Returns a matrix of marginal damages per one ton of additional emissions of the specified gas in the specified year.
"""
function getmarginaldamages(; year=2010, parameters = nothing, yearstoaggregate = 1000, gas = :CO2) 

    # Get marginal model
    yearstorun = min(1050, getindexfromyear(year) + yearstoaggregate)
    mm = create_marginal_FUND_model(year = year, parameters = parameters, yearstorun = yearstorun, gas = gas)
    run(mm)

    # Get damages
    marginaldamages = mm[:impactaggregation, :loss] / 10000000.0
    return marginaldamages
end