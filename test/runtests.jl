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
default_nsteps = 1050
m = MimiFUND.get_model()
run(m)
@test Mimi.time_labels(m) == collect(1950:1:1950+default_nsteps)

#default model created by MimiFUND.get_model()
m1 = MimiFUND.get_model()
run(m1)
@test Mimi.time_labels(m1) == collect(1950:1:1950+default_nsteps)

#use optional args for MimiFUND.get_model()
new_nsteps = 10
@test_throws ErrorException m2 = MimiFUND.get_model(nsteps = new_nsteps) #should error because parameter lenghts won't match time dim

end #fund-model testset

#------------------------------------------------------------------------------
#   2. Run tests to make sure integration version (Mimi v0.5.0)
#   values match Mimi 0.4.0 values
#------------------------------------------------------------------------------
@testset "test-integration" begin

Mimi.reset_compdefs()

m = MimiFUND.get_model()
run(m)

missingvalue = -999.999
err_number = 1.0e-9
err_array = 0.0

for c in map(name, Mimi.compdefs(m)), v in Mimi.variable_names(m, c)

    #load data for comparison
    filename = joinpath(@__DIR__, "../contrib/validation_data_v040/$c-$v.csv")
    results = m[c, v]

    df = load(filename) |> DataFrame
    if typeof(results) <: Number
        validation_results = df[1,1]
        @test results ≈ validation_results atol = err_number #slight imprecision with these values due to rounding

    else
        validation_results = convert(Matrix, df)

        #replace missings with missingvalue so they can be compared
        results[ismissing.(results)] .= missingvalue
        validation_results[ismissing.(validation_results)] .= missingvalue

        #match dimensions
        if size(validation_results,1) == 1
            validation_results = validation_results'
        end

        @test results ≈ validation_results atol = err_array

    end
end

end #fund-integration testset

#------------------------------------------------------------------------------
# 3. Test marginal damages functions (test that each function does not error)
#------------------------------------------------------------------------------

@testset "test-marginaldamages" begin

# Test functions from file "new_marginaldamages.jl"

# Test the compute_scc function with various keyword arguments
scc1 = MimiFUND.compute_scc(year = 2020) 
@test scc1 isa Float64   # test that it's not missing or a NaN
scc2 = MimiFUND.compute_scc(year = 2020, last_year=2300) 
@test scc2 < scc1  # test that shorter horizon made it smaller
scc3 = MimiFUND.compute_scc(year = 2020, last_year=2300, equity_weights=true) 
@test scc3 > scc2  # test that equity weights made itbigger
scc4 = MimiFUND.compute_scc(year = 2020, last_year=2300, equity_weights=true, eta=.8, prtp=0.01) 
@test scc4 > scc3   # test that lower eta and prtp make scc higher
scc5 = MimiFUND.compute_scc(year = 2020, gas=:CH4) 
@test scc5 > scc1   # test social cost of methane is higher

# Test with a modified model
m = MimiFUND.get_model()
update_param!(m, :climatesensitivity, 5)    
scc6 = MimiFUND.compute_scc(m, year=2020, last_year=2300)
@test scc6 > scc2 # test that it's higher than the default because of a higher climate sensitivity 

# Test get_marginal_model
mm = MimiFUND.get_marginal_model(year=2020, gas=:CH4)
run(mm)

# Test compute_scc_mm
result = MimiFUND.compute_scc_mm(year=2050)
@test result.scc isa Float64
@test result.mm isa Mimi.MarginalModel

# Test old exported versions of the functions
scc = MimiFUND.get_social_cost()
md = MimiFUND.getmarginaldamages()

end #marginaldamages testset

#------------------------------------------------------------------------------
# 4. Run basic test of Marginal Damages and MCS functionality
#------------------------------------------------------------------------------

@testset "test-mcs" begin

# mcs
MimiFUND.run_fund_mcs(10)        # Run 10 trials of basic FUND MCS
MimiFUND.run_fund_scc_mcs(10)    # Run 10 trials of FUND MCS SCC calculations

end #test-mcs testset

end #fund testset

nothing
