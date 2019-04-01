using MimiFUND
using Mimi
using Test
using DataFrames
using CSVFiles

@testset "fund" begin

#------------------------------------------------------------------------------
#   1. Run the whole model
#------------------------------------------------------------------------------

@testset "fund-model" begin

#default model exported by fund module
global default_nsteps = 1050
global m = getfund()
run(m)
@test Mimi.time_labels(m) == collect(1950:1:1950+default_nsteps)

#default model created by getfund()
global m1 = getfund()
run(m1)
@test Mimi.time_labels(m1) == collect(1950:1:1950+default_nsteps)

#use optional args for getfund()
global new_nsteps = 10
@test_throws ErrorException m2 = getfund(nsteps = new_nsteps) #should error because parameter lenghts won't match time dim

end #fund-model testset

#------------------------------------------------------------------------------
#   2. Run tests to compare Mimi output to C# output from version 3.8
#------------------------------------------------------------------------------

include("validation.jl")

#------------------------------------------------------------------------------
# 3. Test marginal damages functions (test that each function does not error)
#------------------------------------------------------------------------------

@testset "test-marginaldamages" begin

# new_marginaldamages.jl
scc = MimiFUND.get_social_cost()
md = MimiFUND.getmarginaldamages()

end #marginaldamages testset

#------------------------------------------------------------------------------
# 4. Run basic test of MCS functionality
#------------------------------------------------------------------------------

@testset "test-mcs" begin

# mcs
MimiFUND.run_fund_mcs(10)        # Run 10 trials of basic FUND MCS
MimiFUND.run_fund_scc_mcs(10)    # Run 10 trials of FUND MCS SCC calculations

end #test-mcs testset

end #fund testset

nothing
