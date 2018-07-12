using Mimi

samplesize = 10

include(joinpath(dirname(@__FILE__), "defmcs.jl"))
include(joinpath(dirname(@__FILE__), "../fund.jl"))

# Import default fund model
using fund
default_fund = fund.FUND 
run(default_fund)

# Set up output directories
output = joinpath(dirname(@__FILE__), "../../output/", Dates.format(now(), "yyyy-mm-dd HH-MM-SS"))
mkdir(output)
mkdir("$output/trials")
mkdir("$output/results")

# Generate trials
generate_trials!(mcs, samplesize; filename = joinpath(@__DIR__, "$output/trials/fund_mc_trials_$samplesize.csv"))

# Run monte carlo trials
run_mcs(mcs, default_fund; output_dir = "$output/results")