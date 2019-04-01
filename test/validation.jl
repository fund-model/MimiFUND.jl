# Validation for the Julia version of FUND 3.8 by comparing it to 25 saved output values from the C# 3.8 version

using Test
using DelimitedFiles
using Mimi
using MimiFUND

Precision = 1.0e-10
cs_data_dir = joinpath(@__DIR__, "c#_validation_data")  # holds the saved output files from the C# version


"""
Converts c# data files into arrays. 
If the data has a region index, the data is converted from long to wide format.
"""
function convertcsdata(inputfile::String)
    d = readdlm(inputfile, ',')
    if size(d)[2]==3    # has a region index
        numcols = 16
        numrows = Int(size(d)[1]/16)
        d = convert(Array, reshape(d[:, end], numcols, numrows)')
    elseif size(d)[2]==2 # only has one index
        d = d[:, end]
    else
        error("unknown parameter file structure")
    end 
    return d
end 


@testset "C# 3.8 Validation" begin

# Get the mimi version and run it
m = getfund()
run(m) 

cs_files = readdir(cs_data_dir)

for f in cs_files 
    # println(f)
    cs_data = convertcsdata(joinpath(cs_data_dir, f))

    comp, var, _ = split(f, ".")    # each file name is formatted as "compname.varname.csv"
    var = var == "so2withseeiandscei" ? "so2WithSeeiAndScei" : var  # just one variable has different capitalization between the C# and Mimi version

    mimi_data = m[Symbol(comp), Symbol(var)]
    if length(size(mimi_data)) == 2
        mimi_data = mimi_data[1:end-1, :] # because C# version has one less timestep
    else
        mimi_data = mimi_data[1:end-1] # because C# version has one less timestep
    end
    mimi_data[ismissing.(mimi_data)] .= 0   # the "missings" in the julia version when the first step isn't calculated are zeros in the C# version

    @testset "$comp $var" begin
        if comp == "impactaggregation"
            @test all(isapprox.(cs_data, mimi_data, rtol = Precision))  # Test relative tolerance for the variables in impactaggregation component because they are large (and only saved to two decimal places in the C# files)
        else
            @test all(isapprox.(cs_data, mimi_data, atol = Precision))  # Test absolute tolerance for the rest of the values
        end
    end

end

end 

nothing