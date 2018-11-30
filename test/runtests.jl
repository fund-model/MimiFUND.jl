using Mimi
using Test
using DataFrames
using CSV

@testset "fund" begin

#------------------------------------------------------------------------------
#   1. Run the whole model
#------------------------------------------------------------------------------

@testset "fund-model" begin

include("../src/fund.jl")
using .Fund

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
#   2. Run tests to make sure integration version (Mimi v0.5.0)
#   values match Mimi 0.4.0 values
#------------------------------------------------------------------------------
@testset "test-integration" begin

Mimi.reset_compdefs()

include("../src/fund.jl")
using .Fund

global m = getfund()
run(m)

nullvalue = -999.999
err_number = 1.0e-9
err_array = 0.0

for c in map(name, Mimi.compdefs(m)), v in Mimi.variable_names(m, c)
    
    #load data for comparison
    filename = joinpath(@__DIR__, "../contrib/validation_data_v040/$c-$v.csv")    
    results = m[c, v]

    if typeof(results) <: Number
        validation_results = CSV.read(filename)[1,1]
        @test results ≈ validation_results atol = err_number #slight imprecision with these values due to rounding
        
    else
        validation_results = convert(Array, CSV.read(filename))

        #remove NaNs
        results[ismissing.(results)] .= nullvalue
        results[isnan.(results)] .= nullvalue
        validation_results[isnan.(validation_results)] .= nullvalue

        #match dimensions
        if size(validation_results,1) == 1
            validation_results = validation_results'
        end
        
        @test results ≈ validation_results atol = err_array
        
    end
end

end #fund-integration testset

end #fund testset

nothing
