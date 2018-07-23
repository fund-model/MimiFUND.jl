using Mimi

include(joinpath(dirname(@__FILE__), "defmcs.jl"))
include(joinpath(dirname(@__FILE__), "../fund.jl"))
using fund

trials = 10

# Set up output directories
output = joinpath(dirname(@__FILE__), "../../output/", Dates.format(now(), "yyyy-mm-dd HH-MM-SS"))
mkpath("$output/trials")
mkpath("$output/results")

# Generate trials
generate_trials!(mcs, trials; filename = joinpath(@__DIR__, "$output/trials/fund_mc_trials_$trials.csv"))

# TEMPORARY post trial function to test apply trial data
function post(mcs::MonteCarloSimulation, trialnum::Int, ntimesteps::Int, tup::Void)
    m = mcs.models[1]
    println(m[:climatedynamics, :climatesensitivity]) # should be printing out the trial values but it's the same default value every time
    return nothing
end

# Run monte carlo trials
set_model!(mcs, FUND)
run_mcs(mcs; 
    post_trial_func = post, 
    output_dir = "$output/results")