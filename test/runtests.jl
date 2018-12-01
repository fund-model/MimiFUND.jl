using Mimi
using Test
using DataFrames
using CSVFiles

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

    df = load(filename) |> DataFrame
    if typeof(results) <: Number
        validation_results = df[1,1]
        @test results ≈ validation_results atol = err_number #slight imprecision with these values due to rounding
        
    else
        validation_results = convert(Array, df)

        #some values have been purposefully changed from missing to zero:

        # because of a recent update we added to Mimi, that doesn't allow other 
        # components to access missing values. This was causing a problem within 
        # the marginal damages functions, because the adder component's input 
        # parameter is connected to one of the global gas variables in the emissions 
        # component, which previously had missing values in the first timestep.
        zeroparams = [:mco2, :globch4, :globn2o, :globsf6]
        if c == :emissions && (v in zeroparams)
            validation_results[1] = 0.0
        end

        #remove NaNs and Missings
        results[ismissing.(results)] .= nullvalue
        results[isnan.(results)] .= nullvalue
        validation_results[ismissing.(validation_results)] .= nullvalue
        validation_results[isnan.(validation_results)] .= nullvalue

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

include("../src/marginaldamages.jl")
md = getmarginaldamages()

include("../src/marginaldamage3.jl")
md3 = marginaldamage3()

include("../src/new_marginaldamages.jl")
scc = get_social_cost()
md = getmarginaldamages()


end #marginaldamages testset

end #fund testset

nothing
