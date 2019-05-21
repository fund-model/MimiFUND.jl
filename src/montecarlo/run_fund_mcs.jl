using Dates

"""
Runs a Monte Carlo simulation with the FUND model over it's distributional parameters.
`trials`: the number of trials to run.
`ntimesteps`: how many timesteps to run.
`output_dir`: an output directory; if none provided, will create and use "output/yyyy-mm-dd HH-MM-SS MCtrials". 
`save_trials`: whether or not to generate and save the MC trial values up front to a file.
"""
function run_fund_mcs(trials = 10000; ntimesteps = MimiFUND.default_nsteps + 1, output_dir = nothing, save_trials = false)

    # Set up output directories
    output_dir = output_dir == nothing ? joinpath(@__DIR__, "../../output/", "SCC $(Dates.format(now(), "yyyy-mm-dd HH-MM-SS")) MC$trials") : output_dir
    mkpath("$output_dir/results")

    # Get an instance of FUND's mcs
    mcs = getmcs()

    # Generate trials
    if save_trials
        filename = joinpath("$output_dir/trials.csv")
        generate_trials!(mcs, trials; filename=filename)
    else
        generate_trials!(mcs, trials)
    end

    # Run monte carlo trials
    set_models!(mcs, get_model())
    run_sim(mcs; trials = trials, ntimesteps = ntimesteps, output_dir = "$output_dir/results")

    return nothing
end
