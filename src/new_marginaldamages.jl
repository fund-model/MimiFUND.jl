import Mimi.compinstance

"""
    compute_scc(m::Model=get_model(); year::Union{Int, Nothing} = nothing, 
                gas::Symbol = :CO2, last_year::Int = 3000, equity_weights::Bool = false, 
                eta::Float64 = 1.45, prtp::Float64 = 0.015, equity_weights_normalization_region::Int=0)

Deprecated function for calculating the social cost of carbon for a MimiFUND model. Use `compute_sc` or gas-specific functions `compute_scco2`, `compute_scch4`, `compute_scn2o`, or `compute_scsf6` instead.
"""
function compute_scc(m::Model=get_model(); year::Union{Int, Nothing} = nothing, gas::Symbol = :CO2, last_year::Int = 3000, equity_weights::Bool = false, eta::Float64 = 1.45, prtp::Float64 = 0.015, equity_weights_normalization_region::Int=0)
    Base.depwarn("`compute_scc` is deprecated. Use `compute_sc` or other gas-specific functions instead.", :MimiFUND)
    year === nothing ? error("Must specify an emission year. Try `compute_scc(year=2020)`.") : nothing
    return compute_sc(m, year = year, gas = gas, last_year = last_year, equity_weights = equity_weights, eta = eta, prtp = prtp, equity_weights_normalization_region = equity_weights_normalization_region)
end

"""
    compute_scco2(
        m::Model=get_model(); 
        year::Union{Int, Nothing} = nothing, 
        eta::Float64 = 1.45, 
        prtp::Float64 = 0.015, 
        equity_weights::Bool = false, 
        equity_weights_normalization_region::Int=0, 
        last_year::Int = 3000, 
        pulse_size::Float64 = 1e7)
        return_mm::Bool = false, 
        n::Union{Int, Nothing} = nothing, 
        trials_output_filename::Union{String, Nothing} = nothing, 
        seed::Union{Int, Nothing} = nothing)

Returns the Social Cost of CO2 for the specified `year` for the provided MimiFUND model `m`. 
If no model is provided, the default model from MimiFUND.get_model() is used.
Units of the returned value are 1995\$ per metric tonne of CO2.

The size of the marginal emission pulse can be modified with the `pulse_size` keyword argument, in metric 
tonnes of the specified gas (this does not change the units of the returned value, which is always normalized
by the `pulse_size` used).

This is a wrapper function that calls the generic social cost function `compute_sc(m, gas = :CO2, args...)`. See docstring for
`compute_sc` for a full description of the available keyword arguments.
"""
function compute_scco2(m::Model=get_model(); year::Union{Int, Nothing} = nothing, eta::Float64 = 1.45, prtp::Float64 = 0.015, equity_weights::Bool = false, equity_weights_normalization_region::Int=0, last_year::Int = 3000, pulse_size::Float64 = 1e7, 
    return_mm::Bool = false, n::Union{Int, Nothing} = nothing, trials_output_filename::Union{String, Nothing} = nothing, seed::Union{Int, Nothing} = nothing)

    year === nothing ? error("Must specify an emission year. Try `compute_scco2(year=2020)`.") : nothing
    return compute_sc(m, gas = :CO2, year = year, eta = eta, prtp = prtp, equity_weights = equity_weights, last_year = last_year, pulse_size = pulse_size, 
                        return_mm = return_mm, n = n, trials_output_filename = trials_output_filename, seed = seed, equity_weights_normalization_region=equity_weights_normalization_region)
end

"""
    compute_scch4(m::Model=get_model(); 
        year::Union{Int, Nothing} = nothing, 
        eta::Float64 = 1.45, 
        prtp::Float64 = 0.015, 
        equity_weights::Bool = false, 
        last_year::Int = 3000, 
        pulse_size::Float64 = 1e7) ,
        return_mm::Bool = false, 
        n::Union{Int, Nothing} = nothing, 
        trials_output_filename::Union{String, Nothing} = nothing, 
        seed::Union{Int, Nothing} = nothing, 
        equity_weights_normalization_region::Int=0)

Returns the Social Cost of CH4 for the specified `year` for the provided MimiFUND model `m`. 
If no model is provided, the default model from MimiFUND.get_model() is used.
Units of the returned value are 1995\$ per metric tonne of CH4.

The size of the marginal emission pulse can be modified with the `pulse_size` keyword argument, in metric 
tonnes of the specified gas (this does not change the units of the returned value, which is always normalized
by the `pulse_size` used).

This is a wrapper function that calls the generic social cost function `compute_sc(m, gas = :CH4, args...)`. See docstring for
`compute_sc` for a full description of the available keyword arguments.
"""
function compute_scch4(m::Model=get_model(); year::Union{Int, Nothing} = nothing, eta::Float64 = 1.45, prtp::Float64 = 0.015, equity_weights::Bool = false, last_year::Int = 3000, pulse_size::Float64 = 1e7, 
    return_mm::Bool = false, n::Union{Int, Nothing} = nothing, trials_output_filename::Union{String, Nothing} = nothing, seed::Union{Int, Nothing} = nothing, equity_weights_normalization_region::Int=0)

    year === nothing ? error("Must specify an emission year. Try `compute_scch4(year=2020)`.") : nothing
    return compute_sc(m, gas = :CH4, year = year, eta = eta, prtp = prtp, equity_weights = equity_weights, last_year = last_year, pulse_size = pulse_size, 
                        return_mm = return_mm, n = n, trials_output_filename = trials_output_filename, seed = seed, equity_weights_normalization_region=equity_weights_normalization_region)
end

"""
    compute_scn2o(
        m::Model=get_model(); 
        year::Union{Int, Nothing} = nothing, 
        eta::Float64 = 1.45, prtp::Float64 = 0.015, 
        equity_weights::Bool = false, 
        last_year::Int = 3000, 
        pulse_size::Float64 = 1e7, 
        return_mm::Bool = false, 
        n::Union{Int, Nothing} = nothing, 
        trials_output_filename::Union{String, Nothing} = nothing, 
        seed::Union{Int, Nothing} = nothing, 
        equity_weights_normalization_region::Int=0)

Returns the Social Cost of N2O for the specified `year` for the provided MimiFUND model `m`. 
If no model is provided, the default model from MimiFUND.get_model() is used.
Units of the returned value are 1995\$ per metric tonne of N2O.

The size of the marginal emission pulse can be modified with the `pulse_size` keyword argument, in metric 
tonnes of the specified gas (this does not change the units of the returned value, which is always normalized
by the `pulse_size` used).

This is a wrapper function that calls the generic social cost function `compute_sc(m, gas = :N2O, args...)`. See docstring for
`compute_sc` for a full description of the available keyword arguments.
"""
function compute_scn2o(m::Model=get_model(); year::Union{Int, Nothing} = nothing, eta::Float64 = 1.45, prtp::Float64 = 0.015, equity_weights::Bool = false, last_year::Int = 3000, pulse_size::Float64 = 1e7, 
    return_mm::Bool = false, n::Union{Int, Nothing} = nothing, trials_output_filename::Union{String, Nothing} = nothing, seed::Union{Int, Nothing} = nothing, equity_weights_normalization_region::Int=0)

    year === nothing ? error("Must specify an emission year. Try `compute_scn2o(year=2020)`.") : nothing
    return compute_sc(m, gas = :N2O, year = year, eta = eta, prtp = prtp, equity_weights = equity_weights, last_year = last_year, pulse_size = pulse_size, 
                        return_mm = return_mm, n = n, trials_output_filename = trials_output_filename, seed = seed, equity_weights_normalization_region=equity_weights_normalization_region)
end

"""
    compute_scsf6(
        m::Model=get_model(); 
        year::Union{Int, Nothing} = nothing, 
        eta::Float64 = 1.45, 
        prtp::Float64 = 0.015, 
        equity_weights::Bool = false, 
        last_year::Int = 3000, 
        pulse_size::Float64 = 1e7, 
        return_mm::Bool = false, 
        n::Union{Int, Nothing} = nothing, 
        trials_output_filename::Union{String, Nothing} = nothing, 
        seed::Union{Int, Nothing} = nothing, 
        equity_weights_normalization_region::Int=0)

Returns the Social Cost of SF6 for the specified `year` for the provided MimiFUND model `m`. 
If no model is provided, the default model from MimiFUND.get_model() is used.
Units of the returned value are 1995\$ per metric tonne of SF6.

The size of the marginal emission pulse can be modified with the `pulse_size` keyword argument, in metric 
tonnes of the specified gas (this does not change the units of the returned value, which is always normalized
by the `pulse_size` used).

This is a wrapper function that calls the generic social cost function `compute_sc(m, gas = :SF6, args...)`. See docstring for
`compute_sc` for a full description of the available keyword arguments.
"""
function compute_scsf6(m::Model=get_model(); year::Union{Int, Nothing} = nothing, eta::Float64 = 1.45, prtp::Float64 = 0.015, equity_weights::Bool = false, last_year::Int = 3000, pulse_size::Float64 = 1e7, 
    return_mm::Bool = false, n::Union{Int, Nothing} = nothing, trials_output_filename::Union{String, Nothing} = nothing, seed::Union{Int, Nothing} = nothing, equity_weights_normalization_region::Int=0)

    year === nothing ? error("Must specify an emission year. Try `compute_scsf6(year=2020)`.") : nothing
    return compute_sc(m, gas = :SF6, year = year, eta = eta, prtp = prtp, equity_weights = equity_weights, last_year = last_year, pulse_size = pulse_size, 
                        return_mm = return_mm, n = n, trials_output_filename = trials_output_filename, seed = seed, equity_weights_normalization_region=equity_weights_normalization_region)
end

"""
    compute_sc(m::Model=get_model(); 
        gas::Symbol = :CO2, 
        year::Union{Int, Nothing} = nothing, 
        eta::Float64 = 1.45, 
        prtp::Float64 = 0.015, 
        equity_weights::Bool = false, 
        equity_weights_normalization_region::Int=0,
        last_year::Int = 3000, 
        pulse_size::Float64 = 1e7, 
        return_mm::Bool = false,
        n::Union{Int, Nothing} = nothing,
        trials_output_filename::Union{String, Nothing} = nothing,
        seed::Union{Int, Nothing} = nothing)

Returns the Social Cost of CO2 (or other gas if specified) for the specified `year`
for the provided MimiFUND model `m`. If no model is provided, the default model from MimiFUND.get_model() is used.
Units of the returned value are 1995\$ per metric tonne of the specified gas.

The discount factor is computed from the specified `eta` and pure rate of time preference `prtp`.
Optional regional equity weighting can be used by specifying `equity_weights = true`. If equity weights are used, one can specify
the normalization region used for equity weighting with the `equity_weights_normalization_region` parameter. A value of `0` uses world
average per capita values for the normalization, any other value uses per capita values of that region as the normalization.
By default, the social cost includes damages through the year 3000, but this time horizon can be modified 
by setting the keyword `last_year` to some other year within the model's time index.
The size of the marginal emission pulse can be modified with the `pulse_size` keyword argument, in metric 
tonnes of the specified gas (this does not change the units of the returned value, which is always normalized
by the `pulse_size` used).

If `return_mm` is set to `true`, then a NamedTuple (sc = sc, mm = mm) of the social cost value and the MarginalModel 
used to compute it is returned.

By default, `n = nothing`, and a single value for the "best guess" social cost is returned. If a positive 
value for keyword `n` is specified, then a Monte Carlo simulation with sample size `n` will run, sampling from 
all of FUND's random variables, and a vector of `n` social cost values will be returned.
Optionally providing a CSV file path to `trials_output_filename` will save all of the sampled trial data as a CSV file.
Optionally providing a `seed` value will set the random seed before running the simulation, allowing the 
results to be replicated.
"""
function compute_sc(m::Model=get_model(); 
    gas::Symbol = :CO2, 
    year::Union{Int, Nothing} = nothing, 
    eta::Float64 = 1.45, 
    prtp::Float64 = 0.015, 
    equity_weights::Bool = false, 
    equity_weights_normalization_region::Int = 0,
    last_year::Int = 3000, 
    pulse_size::Float64 = 1e7, 
    return_mm::Bool = false,
    n::Union{Int, Nothing} = nothing,
    trials_output_filename::Union{String, Nothing} = nothing,
    seed::Union{Int, Nothing} = nothing
    )

    year === nothing ? error("Must specify an emission year. Try `compute_sc(year=2020)`.") : nothing
    !(last_year in 1950:3000) ? error("Invlaid value for `last_year`: $last_year. `last_year` must be within the model's time index 1950:3000.") : nothing
    !(year in 1950:last_year) ? error("Invalid value for `year`: $year. `year` must be within the model's time index 1950:$last_year.") : nothing

    # note use of `pulse_size` as the `delta` in the creation of a marginal model,
    # which allows for normalization to $ per ton
    mm = get_marginal_model(m; year = year, gas = gas, pulse_size = pulse_size)

    ntimesteps = getindexfromyear(last_year)

    if n === nothing
        # Run the "best guess" social cost calculation
        run(mm; ntimesteps = ntimesteps)
        sc = _compute_sc_from_mm(mm, year = year, gas = gas, ntimesteps = ntimesteps, equity_weights = equity_weights, eta = eta, prtp = prtp, equity_weights_normalization_region=equity_weights_normalization_region)
    elseif n < 1
        error("Invalid n = $n. Number of trials must be a positive integer.")
    else
        # Run a Monte Carlo simulation
        simdef = getmcs()
        payload = (Array{Float64, 1}(undef, n), year, gas, ntimesteps, equity_weights, equity_weights_normalization_region, eta, prtp) # first item is an array to hold SC values calculated in each trial
        Mimi.set_payload!(simdef, payload) 
        seed !== nothing ? Random.seed!(seed) : nothing
        si = run(simdef, mm, n, ntimesteps = ntimesteps, post_trial_func = _fund_sc_post_trial, trials_output_filename = trials_output_filename)
        sc = Mimi.payload(si)[1]
    end

    if return_mm
        return (sc = sc, mm = mm)
    else
        return sc
    end
end

# helper function for computing SC from a MarginalModel that's already been run, not to be exported
function _compute_sc_from_mm(mm::MarginalModel; year::Int, gas::Symbol, ntimesteps::Int, equity_weights::Bool, equity_weights_normalization_region::Int, eta::Float64, prtp::Float64)

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
        normalization_ypc = equity_weights_normalization_region==0 ? mm.base[:socioeconomic, :globalypc][getindexfromyear(year)] : ypc[getindexfromyear(year), equity_weights_normalization_region]
        df = Float64[t >= getindexfromyear(year) ? (normalization_ypc / ypc[t, r]) ^ eta / (1.0 + prtp) ^ (t - getindexfromyear(year)) : 0.0 for t = 1:ntimesteps, r = 1:16]
    end 

    # Compute global social cost
    sc = sum(marginaldamage[2:ntimesteps, :] .* df[2:ntimesteps, :])   # need to start from second value because first value is missing
    return sc
end

# Post trial function used for computing monte carlo vector of social cost values
function _fund_sc_post_trial(sim::SimulationInstance, trialnum::Int, ntimesteps::Int, tup::Union{Tuple, Nothing})
    mm = sim.models[1]  # get the already-run MarginalModel
    (sc_results, year, gas, ntimesteps, equity_weights, equity_weights_normalization_region, eta, prtp) = Mimi.payload(sim)  # unpack the payload information
    sc = _compute_sc_from_mm(mm, year = year, gas = gas, ntimesteps = ntimesteps, equity_weights = equity_weights, eta = eta, prtp = prtp, equity_weights_normalization_region=equity_weights_normalization_region)
    sc_results[trialnum] = sc
end

"""
    get_marginal_model(m::Model = get_model(); gas::Symbol = :CO2, year::Int = nothing, pulse_size::Float64 = 1e7)

Creates a Mimi MarginalModel where the provided m is the base model, and the marginal model has additional emissions of gas `gas` in year `year`.
If no year is provided, the marginal emissions component will be added without an additional pulse.
If no Model m is provided, the default model from MimiFUND.get_model() is used as the base model.
The size of the marginal emission pulse can be modified with the `pulse_size` keyword argument, 
in metric tonnes (this does not change the units of the returned value, which is always normalized
by the `pulse_size` used).
"""
function get_marginal_model(m::Model = get_model(); gas::Symbol = :CO2, year::Union{Int, Nothing} = nothing, pulse_size::Float64 = 1e7)
    year !== nothing && !(year in 1950:3000) ? error("Cannot add marginal emissions in $year, year must be within the model's time index 1950:3000.") : nothing

    # note use of `pulse_size` as the `delta` in the creation of a marginal model,
    # which allows for normalization to $ per ton
    mm = create_marginal_model(m, pulse_size)
    add_marginal_emissions!(mm.modified, year; gas = gas, pulse_size = pulse_size)

    return mm
end

# A component for an emissions pulse to be used in social cost calculations. Computes the `output` vector by adding
# add` to `input`. This is similar to the Mimi.adder component, except that it allows missing values to be passed through.
@defcomp emissionspulse begin
    add    = Parameter(index=[time])
    input  = Parameter(index=[time])
    output = Variable(index=[time])

    function run_timestep(p, v, d, t)
        v.output[t] = Mimi.@allow_missing(p.input[t]) + p.add[t]
    end
end

function _gas_normalization(gas::Symbol)
    if gas == :CO2 
        return 12/44    # convert from tons CO2 to tons C
    elseif gas == :CH4
        return 1
    elseif gas == :N2O
        return 28/44    # convert from tons N2O to tons N
    elseif gas == :SF6 
        return 1
    else
        error("Unknown gas :$gas.")
    end
end

function _weight_normalization(gas::Symbol)
    if gas == :CO2 
        return 1e6   # convert from tonnes to Mt since component expects Mt
    elseif gas == :CH4
        return 1e6   # convert from tonnes to Mt since component expects Mt
    elseif gas == :N2O
        return 1e6   # convert from tonnes to Mt since component expects Mt
    elseif gas == :SF6 
        return 1e3   # convert from tonnes to kt since component expects Mt
    else
        error("Unknown gas :$gas.")
    end
end 
"""
Adds an emissionspulse component to `m`, and sets the additional emissions if a year is specified.
The size of the marginal emission pulse can be modified with the `pulse_size` keyword argument, in metric 
tonnes of the specified gas (this does not change the units of the returned value, which is always normalized
by the `pulse_size` used).
"""
function add_marginal_emissions!(m, year::Union{Int, Nothing} = nothing; gas::Symbol = :CO2, pulse_size::Float64 = 1e7)

    # Add additional emissions to m
    add_comp!(m, emissionspulse, before = :climateco2cycle)
    nyears = length(Mimi.time_labels(m))
    addem = zeros(nyears) 
    if year != nothing 
        # need to (1) divide by pulse spread over 10 years and weight adjustment 
        # from tons to expected units and then (2) normalize from entered gas 
        # units ie. CO2 to expected units ie. C
        addem[getindexfromyear(year):getindexfromyear(year) + 9] .= pulse_size / (10 * _weight_normalization(gas)) *  _gas_normalization(gas)
    end
    update_param!(m, :emissionspulse, :add, addem)

    # Reconnect the appropriate emissions in m
    if gas == :CO2
        connect_param!(m, :emissionspulse, :input, :emissions, :mco2)
        connect_param!(m, :climateco2cycle, :mco2, :emissionspulse, :output)
    elseif gas == :CH4
        connect_param!(m, :emissionspulse, :input, :emissions, :globch4)
        connect_param!(m, :climatech4cycle, :globch4, :emissionspulse, :output)
    elseif gas == :N2O
        connect_param!(m, :emissionspulse, :input, :emissions, :globn2o)
        connect_param!(m, :climaten2ocycle, :globn2o, :emissionspulse, :output)
    elseif gas == :SF6
        connect_param!(m, :emissionspulse, :input, :emissions, :globsf6)
        connect_param!(m, :climatesf6cycle, :globsf6, :emissionspulse, :output)
    else
        error("Unknown gas: $gas")
    end

end 

"""
Helper function to set the marginal emissions in the specified year.
"""
function perturb_marginal_emissions!(m::Model, year; comp_name::Symbol = :emissionspulse, pulse_size::Float64 = 1e7, gas::Symbol = :CO2)

    ci = compinstance(m, comp_name)
    emissions = Mimi.get_param_value(ci, :add)

    nyears = length(Mimi.dimension(m, :time))
    new_em = zeros(nyears)
    # need to (1) divide by pulse spread over 10 years and weight adjustment 
    # from tons to expected units and then  (2) normalize from entered gas 
    # units ie. CO2 to expected units ie. C
    new_em[getindexfromyear(year):getindexfromyear(year) + 9] .= pulse_size / (10 * _weight_normalization(gas)) * _gas_normalization(gas)
    emissions[:] = new_em

end


"""
Returns a matrix of marginal damages per one metric tonne of additional emissions of the specified gas in the specified year.
"""
function getmarginaldamages(; year=2020, parameters = nothing, gas = :CO2, pulse_size::Float64 = 1e7) 

    # Get marginal model
    m = get_model(params = parameters)

    # note use of `pulse_size` as the `delta` in the creation of a marginal model,
    # which allows for normalization to $ per ton
    mm = get_marginal_model(m, year = year, gas = gas, pulse_size = pulse_size)
    run(mm)

    # Return marginal damages
    return mm[:impactaggregation, :loss]
end