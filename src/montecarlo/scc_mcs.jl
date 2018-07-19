using Mimi

samplesize = 10

include(joinpath(dirname(@__FILE__), "defmcs.jl"))
include(joinpath(dirname(@__FILE__), "scc_mcs_tools.jl"))
include(joinpath(dirname(@__FILE__), "../new_marginaldamages.jl"))



# Scenario set up
scenario_args = [
    :rate           => [0.03]
    # :emissionyear   => [2010]
]
# _rate           = 0.03
_emissionyear   = 2010

# Set up output directories
output = joinpath(dirname(@__FILE__), "../../output/", "SCC $(Dates.format(now(), "yyyy-mm-dd HH-MM-SS"))")
mkpath("$output/trials")
mkpath("$output/results")

# Get FUND marginal model
mm = create_marginal_FUND_model()

# Generate trials
generate_trials!(mcs, samplesize; filename = joinpath(@__DIR__, "$output/trials/fund_mc_trials_$samplesize.csv"))

# Run monte carlo trials
set_model!(mcs, mm)
run_mcs(mcs; 
    output_dir = "$output/results",
    scenario_args = scenario_args, 
    scenario_func = _scenario_func,
    pre_trial_func = pre_trial_func,
    post_trial_func = scc_calculation
    )

