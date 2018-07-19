include("../helper.jl")

# Potential pre_trial_func for setting emissions pulse or other scenario parameters?
function pre_trial_func(mcs::MonteCarloSimulation, trialnum::Int, ntimesteps::Int, tup::Tuple)

    base, marginal = mcs.models

    ci = marginal.mi.components[:marginalemission]
    emissions = Mimi.get_parameter_value(ci, :add)
    emissions[:] = 0
    emissions[getindexfromyear(_emissionyear):getindexfromyear(_emissionyear) + 9] = 1.0

    return nothing
end 

# Function to be used as a post_trial_func in the SCC MCS calcualtion
function scc_calculation(mcs::MonteCarloSimulation, trialnum::Int, ntimesteps::Int, tup::Tuple)

    # Unpack scenario tuple argument
    # if tup != nothing
    #     (rate, emissionyear) = tup
    # else
    #     rate = _rate
    #     emissionyear = _emissionyear
    # end
    (rate,) = tup
    emissionyear = _emissionyear

    # Get marginal damages
    base, marginal = mcs.models
    marginaldamages = (marginal[:impactaggregation, :loss] - base[:impactaggregation, :loss]) / 10000000.0

    # Calculate discount factor
    T = ntimesteps == typemax(Int) ? length(dimension(base, :time)) : ntimesteps
    discount_factor = zeros(T)
    idx = getindexfromyear(emissionyear)
    discount_factor[idx:T] = [1 / ((1 + rate) ^ t) for t in 0:T-idx]

    # Sum discounted damages to scc
    scc = sum(sum(marginaldamages, 2)[2:end] .* discount_factor[2:end])
    println(scc)
    return nothing
end

function _scenario_func(mcs::MonteCarloSimulation, tup::Tuple)
        
    # Unpack scenario tuple argument
    # (rate, emissionyear) = tup

    # base, marginal = mcs.models

    # ci = marginal.mi.components[:marginalemission]
    # emissions = Mimi.get_parameter_value(ci, :add)
    # emissions[:] = 0
    # for idx = getindexfromyear(emissionyear):getindexfromyear(emissionyear) + 9
    #     emissions[Int(idx)] = 1.0
    # end

    # addem = zeros(length(dimension(marginal, :time)))
    # addem[getindexfromyear(emissionyear):getindexfromyear(emissionyear) + 9] = 1.0
    # set_parameter!(marginal, :marginalemission, :add, addem)

    nothing
end