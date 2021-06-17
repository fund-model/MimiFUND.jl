# This file can be used to create the validation file for scc files. It will create
# a validation file containing all possibilities of parameter values defined in the 
# specs dictionary below produce the same results. A subset of these values are 
# tested in the "runtests.jl" file deployed by Travis, and the full set can be tested
# manually using the scc_validation_full.jl file

using MimiFUND
using DataFrames
using Query
using CSVFiles
using Test

specs = Dict([
    :gas => [:CO2, :CH4, :N2O, :SF6],
    :year => [2020, 2055],
    :eta => [0, 1.45],
    :prtp => [0.015, 0.03],
    :equity_weights => [true, false],
    :equity_weights_normalization_region => [0, 1, 10],
    :last_year => [2300, 3000],
    :pulse_size => [1, 1e7]
])

results = DataFrame(gas = [], year = [], eta = [], prtp = [], equity_weights = [], equity_weights_normalization_region = [], last_year = [], pulse_size = [], SC = [])

for gas in specs[:gas]
    for year in specs[:year]
        for eta in specs[:eta]
            for prtp in specs[:prtp]
                for equity_weights in specs[:equity_weights]
                    for equity_weights_normalization_region in specs[:equity_weights_normalization_region]
                        for last_year in specs[:last_year]
                            for pulse_size in specs[:pulse_size]
                                sc = MimiFUND.compute_sc(gas=gas, year=year, eta=eta, prtp=prtp, equity_weights=equity_weights, equity_weights_normalization_region=equity_weights_normalization_region, last_year=last_year, pulse_size=pulse_size)
                                push!(results, (gas, year, eta, prtp, equity_weights, equity_weights_normalization_region, last_year, pulse_size, sc))
                            end
                        end
                    end
                end
            end
        end
    end
end

path = joinpath(@__DIR__, "SC validation data", "deterministic_sc_values_v3-13-0.csv")
save(path, results)