import Mimi.compinstance

"""
compute_scc(m::Model=get_model(); year::Int = nothing, gas::Symbol = :CO2, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015, pulse_size::Float64 = 1e7)

Debrecated function for calculating the social cost of carbon for a MimiFUND model. Use `compute_scco2`, `compute_scch4`, `compute_scn2o`, or `compute_scsf6` instead.
"""
function compute_scc(m::Model=get_model(); year::Union{Int, Nothing} = nothing, gas::Symbol = :CO2, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015, pulse_size::Float64 = 1e7)
    @warn("Deprecation warning: `compute_scc` is deprecated. Use `compute_scco2` or other gas-specific functions instead.")
    year === nothing ? error("Must specify an emission year. Try `compute_scc(year=2020)`.") : nothing
    return compute_social_cost(m, year = year, gas = gas, last_year = last_year, equity_weights = equity_weights, eta = eta, prtp = prtp)
end

"""
compute_scco2(m::Model=get_model(); year::Int = nothing, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015, pulse_size::Float64 = 1e7)

Computes the social cost of CO2 for the specified `year` for the provided MimiFUND model `m`. 
If no model is provided, the default model from MimiFUND.get_model() is used.
The discount factor is computed from the specified `eta` and pure rate of time preference `prtp`, with defaults of 1.45 and 0.015.
Units of the returned value are [\$ per tonne CO2] in 1995USD.
Optional regional equity weighting can be used by specifying `equity_weights = true`. 
The size of the marginal emission pulse can be modified with the `pulse_size` keyword argument, in metric tonnes of CO2.
"""
function compute_scco2(m::Model=get_model(); year::Union{Int, Nothing} = nothing, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015, pulse_size::Float64 = 1e7)
    year === nothing ? error("Must specify an emission year. Try `compute_scco2(year=2020)`.") : nothing
    return compute_social_cost(m, year = year, gas = :CO2, last_year = last_year, equity_weights = equity_weights, eta = eta, prtp = prtp)
end

"""
compute_scch4(m::Model=get_model(); year::Int = nothing, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015, pulse_size::Float64 = 1e7)

Computes the social cost of CH4 for the specified `year` for the provided MimiFUND model `m`. 
If no model is provided, the default model from MimiFUND.get_model() is used.
The discount factor is computed from the specified `eta` and pure rate of time preference `prtp`, with defaults of 1.45 and 0.015.
Units of the returned value are [\$ per tonne CH4] in 1995USD.
Optional regional equity weighting can be used by specifying `equity_weights = true`. 
The size of the marginal emission pulse can be modified with the `pulse_size` keyword argument, in metric tonnes of CH4.
"""
function compute_scch4(m::Model=get_model(); year::Union{Int, Nothing} = nothing, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015, pulse_size::Float64 = 1e7)
    year === nothing ? error("Must specify an emission year with keyword argument `year`. Try `compute_scch4(year=2020)`.") : nothing
    return compute_social_cost(m, year = year, gas = :CH4, last_year = last_year, equity_weights = equity_weights, eta = eta, prtp = prtp)
end

"""
compute_scn2o(m::Model=get_model(); year::Int = nothing, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015, pulse_size::Float64 = 1e7)

Computes the social cost of N2O for the specified `year` for the provided MimiFUND model `m`. 
If no model is provided, the default model from MimiFUND.get_model() is used.
The discount factor is computed from the specified `eta` and pure rate of time preference `prtp`, with defaults of 1.45 and 0.015.
Units of the returned value are [\$ per tonne N2O] in 1995USD.
Optional regional equity weighting can be used by specifying `equity_weights = true`. 
The size of the marginal emission pulse can be modified with the `pulse_size` keyword argument, in metric tonnes of N2O.
"""
function compute_scn2o(m::Model=get_model(); year::Union{Int, Nothing} = nothing, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015, pulse_size::Float64 = 1e7)
    year === nothing ? error("Must specify an emission year with keyword argument `year`. Try `compute_scn2o(year=2020)`.") : nothing
    return compute_social_cost(m, year = year, gas = :N2O, last_year = last_year, equity_weights = equity_weights, eta = eta, prtp = prtp)
end

"""
compute_scsf6(m::Model=get_model(); year::Int = nothing, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015, pulse_size::Float64 = 1e7)

Computes the social cost of SF6 for the specified `year` for the provided MimiFUND model `m`. 
If no model is provided, the default model from MimiFUND.get_model() is used.
The discount factor is computed from the specified `eta` and pure rate of time preference `prtp`, with defaults of 1.45 and 0.015.
Units of the returned value are [\$ per tonne SF6] in 1995USD.
Optional regional equity weighting can be used by specifying `equity_weights = true`. 
The size of the marginal emission pulse can be modified with the `pulse_size` keyword argument, in metric tonnes of SF6.
"""
function compute_scsf6(m::Model=get_model(); year::Union{Int, Nothing} = nothing, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015, pulse_size::Float64 = 1e7)
    year === nothing ? error("Must specify an emission year with keyword argument `year`. Try `compute_scn2o(year=2020)`.") : nothing
    return compute_social_cost(m, year = year, gas = :SF6, last_year = last_year, equity_weights = equity_weights, eta = eta, prtp = prtp)
end

# helper function called by each gas-specific function for computing the social cost
function compute_social_cost(m::Model=get_model(); year::Union{Int, Nothing} = nothing, gas::Symbol = :CO2, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015, pulse_size::Float64 = 1e7)
    year === nothing ? error("Must specify an emission year. Try `compute_social_cost(year=2020)`.") : nothing
    !(last_year in 1950:3000) ? error("Invlaid value for `last_year`: $last_year. `last_year` must be within the model's time index 1950:3000.") : nothing
    !(year in 1950:last_year) ? error("Invalid value for `year`: $year. `year` must be within the model's time index 1950:$last_year.") : nothing

    mm = get_marginal_model(m; year = year, gas = gas, pulse_size = pulse_size)

    return _compute_scc(mm, year = year, gas = gas, last_year = last_year, equity_weights = equity_weights, eta = eta, prtp = prtp)
end

"""
compute_sc_mm(m::Model=get_model(); year::Union{Int, Nothing} = nothing, gas::Symbol = :CO2, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015, pulse_size::Float64 = 1e7)

Returns a NamedTuple (scc=scc, mm=mm) of the social cost of carbon and the MarginalModel used to compute it.
Computes the social cost of CO2 (or other gas if specified) for an emissions pulse in `year`
for the provided MimiFUND model. If no model is provided, the default model from MimiFUND.get_model() is used.
The discount factor is computed from the specified `eta` and pure rate of time preference `prtp`.
Optional regional equity weighting can be used by specifying `equity_weights=true`. 
The size of the marginal emission pulse can be modified with the `pulse_size` keyword argument, in metric tonnes.
"""
function compute_sc_mm(m::Model=get_model(); year::Union{Int, Nothing} = nothing, gas::Symbol = :CO2, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015, pulse_size::Float64 = 1e7)
    year === nothing ? error("Must specify an emission year. Try `compute_scc_mm(m, year=2020)`.") : nothing
    !(last_year in 1950:3000) ? error("Invlaid value of $last_year for last_year. last_year must be within the model's time index 1950:3000.") : nothing
    !(year in 1950:last_year) ? error("Cannot compute the scc for year $year, year must be within the model's time index 1950:$last_year.") : nothing

    mm = get_marginal_model(m; year = year, gas = gas, pulse_size = pulse_size)
    scc = _compute_scc(mm; year=year, gas=gas, last_year=last_year, equity_weights=equity_weights, eta=eta, prtp=prtp)
    
    return (scc = scc, mm = mm)
end

# helper function for computing SCC from a MarginalModel, not to be exported
function _compute_scc(mm::MarginalModel; year::Int, gas::Symbol, last_year::Int, equity_weights::Bool, eta::Float64, prtp::Float64)
    ntimesteps = getindexfromyear(last_year)
    run(mm; ntimesteps = ntimesteps)

    # Calculate the marginal damage between run 1 and 2 for each year/region
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
    scc = sum(marginaldamage[2:ntimesteps, :] .* df[2:ntimesteps, :])
    return scc
end

"""
get_marginal_model(m::Model = get_model(); year::Int = nothing, gas::Symbol = :CO2, pulse_size::Float64 = 1e7)

Creates a Mimi MarginalModel where the provided m is the base model, and the marginal model has additional emissions of gas `gas` in year `year`.
If no year is provided, the marginal emissions component will be added without an additional pulse.
If no Model m is provided, the default model from MimiFUND.get_model() is used as the base model.
The size of the marginal emission pulse can be modified with the `pulse_size` keyword argument, in metric tonnes.
"""
function get_marginal_model(m::Model = get_model(); year::Union{Int, Nothing} = nothing, gas::Symbol = :CO2, pulse_size::Float64 = 1e7)
    year !== nothing && !(year in 1950:3000) ? error("Cannot add marginal emissions in $year, year must be within the model's time index 1950:3000.") : nothing

    mm = create_marginal_model(m, pulse_size)
    add_marginal_emissions!(mm.marginal, year; gas = gas, pulse_size = pulse_size)

    return mm
end

"""
Adds a marginalemission component to `m`, and sets the additional emissions if a year is specified.
The size of the marginal emission pulse can be modified with the `pulse_size` keyword argument, in metric tonnes.
"""
function add_marginal_emissions!(m, year::Union{Int, Nothing} = nothing; gas::Symbol = :CO2, pulse_size::Float64 = 1e7)

    # Add additional emissions to m
    add_comp!(m, Mimi.adder, :marginalemission, before = :climateco2cycle, first = 1951)
    nyears = length(Mimi.time_labels(m))
    addem = zeros(nyears - 1)   # starts one year later, in 1951
    if year != nothing 
        # pulse is spread over ten years, and emissions components is in Mt so divide by 1e7, and convert from CO2 to C if gas==:CO2 because emissions component is in MtC
        addem[getindexfromyear(year)-1:getindexfromyear(year) + 8] .= pulse_size / 1e7 * (gas == :CO2 ? 12/44 : 1)
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
        connect_param!(m, :marginalemission, :input, :emissions, :globsf6)
        connect_param!(m, :climatesf6cycle, :globsf6, :marginalemission, :output, repeat([missing], nyears))
    else
        error("Unknown gas: $gas")
    end

end 

"""
Helper function to set the marginal emissions in the specified year.
"""
function perturb_marginal_emissions!(m::Model, year; comp_name::Symbol = :marginalemission, pulse_size::Float64 = 1e7, gas::Symbol = :CO2)

    ci = compinstance(m, comp_name)
    emissions = Mimi.get_param_value(ci, :add)

    nyears = length(Mimi.dimension(m, :time))
    new_em = zeros(nyears - 1)
    new_em[getindexfromyear(year)-1:getindexfromyear(year) + 8] .= pulse_size / 1e7 * (gas == :CO2 ? 12/44 : 1)
    emissions[:] = new_em

end


"""
Returns a matrix of marginal damages per one metric tonne of additional emissions of the specified gas in the specified year.
"""
function getmarginaldamages(; year=2010, parameters = nothing, gas = :CO2, pulse_size::Float64 = 1e7) 

    # Get marginal model
    m = get_model(params = parameters)
    mm = get_marginal_model(m, year = year, gas = gas, pulse_size = pulse_size)
    run(mm)

    # Return marginal damages
    return mm[:impactaggregation, :loss]
end