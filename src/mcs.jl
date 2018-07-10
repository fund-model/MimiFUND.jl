using Mimi
using Distributions

mcs = @defmcs begin


end 

function do_montecarlo_runs(samplesize::Int)
    generate_trials!(mcs, samplesize, joinpath(@__DIR__, "../output/fund_mc_trials.csv"))
    run_mcs(m, mcs, 100, joinpath(@__DIR__, "../output/fund_mc_output.csv"))
end