
# Potential pre_trial_func for setting emissions pulse or other scenario parameters?
function pre_trial_func(mcs::MonteCarloSimulation, trialnum::Int, ntimesteps::Int, tup::Tuple)
    # Set scenario values for marginal emissions?

    
end 

# Function to be used as a post_trial_func in the SCC MCS calcualtion
function scc_calculation(mcs::MonteCarloSimulation, trialnum::Int, ntimesteps::Int, tup::Tuple)
    # Calculate and write/output scc values?
    base, marginal = mcs.models

end