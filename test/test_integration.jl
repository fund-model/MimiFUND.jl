# Check that the results from running Mimi v0.4.0 match those with Mimi v0.5.0 
using Mimi
using Base.Test
using DataFrames

Mimi.reset_compdefs()

include("../src/fund.jl")

using fund

m = fund.FUND
run(m)

@testset "test-integration" begin

    nullvalue = -999.999
    err_number = 1.0e-10
    err_array = 0.0
    
    for c in map(name, Mimi.compdefs(m)), v in Mimi.variable_names(m, c)
        
        #load data for comparison
        filename = "../contrib/validation_data/$c-$v.csv"        
        results = m[c, v]

        if typeof(results) <: Number
            validation_results = readtable(filename)[1,1]
            @test results ≈ validation_results atol = err_number #slight imprecision with these values due to rounding
            
        else
            validation_results = convert(Array, readtable(filename))

            #match dimensions
            if size(validation_results,1) == 1
                validation_results = validation_results'
            end

            #remove NaNs
            results[isnan.(results)] = nullvalue
            validation_results[isnan.(validation_results)] = nullvalue

            @test results ≈ validation_results atol = err_array
            
        end
    end
end #testset
