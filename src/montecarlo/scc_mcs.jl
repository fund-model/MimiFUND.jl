using Mimi

include(joinpath(@__DIR__, "../fund.jl"))
include(joinpath(@__DIR__, "defmcs.jl"))                  # Defines FUND's distributional parameters
include(joinpath(@__DIR__, "../new_marginaldamages.jl"))  # Defines FUND's marginal emissions model

using fund

function run_fund_scc(trials = 10; year = 2020, years = nothing, rate = 0.03, rates = nothing, ntimesteps = 1051, save_trials = false)

    # Set up output directories
    output = joinpath(@__DIR__, "../../output/", "SCC $(Dates.format(now(), "yyyy-mm-dd HH-MM-SS"))")
    mkpath("$output/results")

    # Write header in SCC output file
    scc_file = joinpath(output, "scc.csv")
    open(scc_file, "w") do f 
        write(f, "trial, rate, year, scc\n")
    end 

    # Scenario set up
    scenario_args = [
        :rate           => rates != nothing ? rates : [rate]
        :emissionyear   => years != nothing ? years : [year]
    ]

    # Generate trials
    if save_trials
        mkpath("$output/trials")
        generate_trials!(mcs, trials; filename = joinpath(@__DIR__, "$output/trials/fund_mc_trials_$trials.csv"))
    else 
        generate_trials!(mcs, trials)
    end

    # Get FUND marginal model
    mm = create_marginal_FUND_model()
    set_model!(mcs, mm)

    # Define scenario function
    function _scenario_func(mcs::MonteCarloSimulation, tup::Tuple)
        
        # Unpack scenario tuple argument
        (rate, emissionyear) = tup
    
        # Get models
        base, marginal = mcs.models
    
        # Perturb emissons in the marginal model
        perturb_marginal_emissions!(marginal, emissionyear)
    
    end

    # Define post trial function
    function scc_calculation(mcs::MonteCarloSimulation, trialnum::Int, ntimesteps::Int, tup::Tuple)

        # Unpack scenario tuple argument
        (rate, emissionyear) = tup
    
        # Get marginal damages
        base, marginal = mcs.models
        marginaldamages = (marginal[:impactaggregation, :loss] - base[:impactaggregation, :loss]) / 10000000.0
    
        # Calculate discount factor
        T = ntimesteps == typemax(Int) ? length(Mimi.dimension(base, :time)) : ntimesteps
        discount_factor = zeros(T)
        idx = getindexfromyear(emissionyear)
        discount_factor[idx:T] = [1 / ((1 + rate) ^ t) for t in 0:T-idx]
    
        # Sum discounted damages to scc
        scc = sum(sum(marginaldamages, 2)[2:end] .* discount_factor[2:end])
    
        # Write output
        open(scc_file, "a") do f 
            write(f, "$trialnum, $rate, $emissionyear, $scc\n")
        end 
    
        return nothing
    end

    # Run monte carlo trials
    run_mcs(mcs, trials, 2; 
        ntimesteps = ntimesteps,
        output_dir = "$output/results",
        scenario_args = scenario_args, 
        scenario_func = _scenario_func,   
        post_trial_func = scc_calculation   
        )

    return nothing
end 

