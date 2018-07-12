include("helper.jl")
include("fund.jl")
using fund

"""
Returns a matrix of marginal damages per one ton of additional emissions of the specified gas in the specified year.
"""
function getmarginaldamages(; emissionyear=2010, yearstoaggregate = 1000, gas = :C)
# function getmarginaldamages(; emissionyear=2010, parameters = nothing, yearstoaggregate = 1000, gas = :C) 
    # TODO: do we need to support the option to use an alternate parameters dictionary?
    # not currently supported with how the integrated fund file has been structured.

    # Calculate number of years to run the models
    yearstorun = min(1050, getindexfromyear(emissionyear) + yearstoaggregate)

    # Get default FUND model
    m1 = fund.FUND # (nsteps=yearstorun,params=parameters)

    # Get model to add marginal emissions to
    m2 = fund.FUND # (nsteps=yearstorun,params=parameters)
    addcomponent(m2, adder, :marginalemission, before = :climateco2cycle)
    addem = zeros(yearstorun + 1)
    addem[getindexfromyear(emissionyear):getindexfromyear(emissionyear) + 9] = 1.0
    set_parameter!(m2, :marginalemission, :add, addem)

    # Reconnect the appropriate emissions in the marginal model
    if gas == :C
        connect_parameter(m2, :marginalemission, :input, :emissions, :mco2)
        connect_parameter(m2, :climateco2cycle, :mco2, :marginalemission, :output)
    elseif gas == :CH4
        connect_parameter(m2, :marginalemission, :input, :emissions, :globch4)
        connect_parameter(m2, :climatech4cycle, :globch4, :marginalemission, :output)
    elseif gas == :N2O
        connect_parameter(m2, :marginalemission, :input, :emissions, :globn2o)
        connect_parameter(m2, :climaten2ocycle, :globn2o, :marginalemission, :output)
    elseif gas == :SF6
        connect_parameter(m2, :marginalemission, :input, :emissions,:globsf6)
        connect_parameter(m2, :climatesf6cycle, :globsf6, :marginalemission, :output)
    else
        error("Unknown gas.")
    end

    # Run both models
    run(m1; ntimesteps = yearstorun)
    run(m2; ntimesteps = yearstorun)

    # Get damages
    damage1 = m1[:impactaggregation, :loss]
    damage2 = m2[:impactaggregation, :loss]

    # Calculate the marginal damage between run 1 and 2 for each year/region
    marginaldamages = (damage2 .- damage1) / 10000000.0

    return marginaldamages
end
