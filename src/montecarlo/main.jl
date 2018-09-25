using Mimi

include(joinpath(@__DIR__, "defmcs.jl"))
include(joinpath(@__DIR__, "../fund.jl"))
using fund


function run_fund_mcs(trials = 10; output = nothing, save_trials = false)

    # Set up output directories
    output = joinpath(@__DIR__, "../../output/", Dates.format(now(), "yyyy-mm-dd HH-MM-SS"))
    mkpath("$output/results")

    # Generate trials
    if save_trials
        mkpath("$output/trials")
        generate_trials!(mcs, trials; filename = joinpath(output, "trials/fund_mc_trials_$trials.csv"))
    end

    # Run monte carlo trials
    set_model!(mcs, FUND)
    run_mcs(mcs, trials; output_dir = "$output/results")

end