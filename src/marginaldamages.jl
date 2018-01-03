include("fund.jl")

# This function returns a matrix of marginal damages per one t of carbon emission in the
# emissionyear parameter year.
function getmarginaldamages(;emissionyear=2010,parameters=nothing,yearstoaggregate=1000,gas=:C)
    yearstorun = min(1050, getindexfromyear(emissionyear) + yearstoaggregate)

    m1 = getfund(nsteps=yearstorun,params=parameters)
    m2 = getfund(nsteps=yearstorun,params=parameters)
    addcomponent(m2, adder, :marginalemission, before=:climateco2cycle)
    addem = zeros(yearstorun+1)
    addem[getindexfromyear(emissionyear):getindexfromyear(emissionyear)+9] = 1.0
    setparameter(m2,:marginalemission,:add,addem)
    if gas==:C
        connectparameter(m2,:marginalemission,:input,:emissions,:mco2)
        connectparameter(m2, :climateco2cycle,:mco2,:marginalemission,:output)
    elseif gas==:CH4
        connectparameter(m2,:marginalemission,:input,:emissions,:globch4)
        connectparameter(m2, :climatech4cycle,:globch4,:marginalemission,:output)
    elseif gas==:N2O
        connectparameter(m2,:marginalemission,:input,:emissions,:globn2o)
        connectparameter(m2, :climaten2ocycle,:globn2o,:marginalemission,:output)
    elseif gas==:SF6
        connectparameter(m2,:marginalemission,:input,:emissions,:globsf6)
        connectparameter(m2, :climatesf6cycle,:globsf6,:marginalemission,:output)
    else
        error("Unknown gas.")
    end

    run(m1)
    run(m2)

    damage1 = m1[:impactaggregation,:loss]
    damage2 = m2[:impactaggregation,:loss]

    # Calculate the marginal damage between run 1 and 2 for each
    # year/region
    marginaldamage = (damage2.-damage1)/10000000.0

    return marginaldamage
end
