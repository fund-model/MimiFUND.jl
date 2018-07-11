# Check that the results from running Mimi v0.4.0 match those with Mimi v0.5.0 
using Mimi
using Base.Test
using DataFrames

include("../src/fund.jl")

using fund

m = fund.FUND
run(m)

@testset begin

    err = 0.0     # Assuming 0 error for now since using same params        

    for c in Mimi.components(m), v in Mimi.variables(m, c)
        filename = "../contrib/validation_data/$c.$v.csv"        
        results = m[:$c, :$v][2:end, :]
        validation_results = convert(Array, readtable("filename"))[2:end, :]
        @test results ≈ validation_results atol = err
    end
    
end #testset
