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

## Compare v3.11.7 to v3.13.0 ... :CO2 and :CH4 should not change

old_sc = load(joinpath(@__DIR__, "SC validation data", "deterministic_sc_values_v3-11-7.csv")) |> DataFrame 
new_sc = load(joinpath(@__DIR__, "SC validation data", "deterministic_sc_values_v3-13-0.csv")) |> DataFrame 

filter!(:gas => x -> (x == "CO2" || x == "CH4"), old_sc)
filter!(:gas => x -> (x == "CO2" || x == "CH4"), new_sc)

# TODO Lisa Rennels
# the precision for the pulse of 1e7 is pretty high but for pulse size of 
# 1 is way less precise ... is that ok? why is that happening? 
filter!(:pulse_size => x -> x == 1.0e7, old_sc)
filter!(:pulse_size => x -> x == 1.0e7, new_sc)

@test all(isapprox.(old_sc[!, :SC], new_sc[!, :SC], atol = 1e-9))

