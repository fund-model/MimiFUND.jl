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

@testset "test-compute_sc" begin

# Test functions from file "new_marginaldamages.jl"

# Test the compute_scco2 function with various keyword arguments
scc0 = MimiFUND.compute_sc(year = 2020, gas = :CO2)
scc1 = MimiFUND.compute_scco2(year = 2020) 
@test scc1 isa Float64   # test that it's not missing or a NaN
@test scc0 == scc1
scc2 = MimiFUND.compute_scco2(year = 2020, last_year=2300) 
@test scc2 < scc1  # test that shorter horizon made it smaller
scc3 = MimiFUND.compute_scco2(year = 2020, last_year=2300, equity_weights=true) 
@test scc3 > scc2  # test that equity weights made itbigger
scc4 = MimiFUND.compute_scco2(year = 2020, last_year=2300, equity_weights=true, eta=.8, prtp=0.01) 
@test scc4 > scc3   # test that lower eta and prtp make scc higher
scch4 = MimiFUND.compute_scch4(year = 2020) 
@test scch4 > scc1   # test social cost of methane is higher

# Test with a modified model
m_high_cs = MimiFUND.get_model()
update_param!(m_high_cs, :climatesensitivity, 5)    
scc6 = MimiFUND.compute_scco2(m_high_cs, year=2020, last_year=2300)
@test scc6 > scc2 # test that it's higher than the default because of a higher climate sensitivity 

# Test get_marginal_model
mm = MimiFUND.get_marginal_model(year=2020, gas=:CH4)
run(mm)

# Test return_mm = true
result = MimiFUND.compute_sc(year=2050, return_mm = true)
@test result.sc isa Float64
@test result.mm isa Mimi.MarginalModel

# Test other gases
scch4 = MimiFUND.compute_scch4(year = 2020)
scn2o = MimiFUND.compute_scn2o(year = 2020)
scsf6 = MimiFUND.compute_scsf6(year = 2020)
@test scsf6 < scch4 < scn2o

# Test that modifying the pulse_size keyword changes the values, but not by much
scch4_2 = MimiFUND.compute_scch4(year = 2020, pulse_size = 1e3)
scn2o_2 = MimiFUND.compute_scn2o(year = 2020, pulse_size = 1e3)
scsf6_2 = MimiFUND.compute_scsf6(year = 2020, pulse_size = 1e3)
@test scch4 != scch4_2
@test scch4 ≈ scch4_2 rtol = 1e-3
@test scn2o != scn2o_2
@test scn2o ≈ scn2o_2 rtol = 1e-3
@test scsf6 != scsf6_2
@test scsf6 ≈ scsf6_2 rtol = 1e-3

# Test monte carlo simulation
scco2_values = MimiFUND.compute_sc(year = 2020, gas = :CO2, n = 10)
@test all(!isnan, scco2_values)

# Test the seed functionality
results1 = MimiFUND.compute_sc(gas = :CH4, year = 2020, n = 10, seed = 350)
results2 = MimiFUND.compute_sc(gas = :CH4, year = 2020, n = 10, seed = 350, trials_output_filename = "tmp_trials.csv")  # test that saving trial values does not affec the seed/sampling order
rm("tmp_trials.csv")    # remove the save trials data
@test results1 == results2
results_shorter = MimiFUND.compute_sc(gas = :CH4, year = 2020, last_year = 2300, n = 10, seed = 350)
@test results_shorter != results1

# Test old exported marginal damage function
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
