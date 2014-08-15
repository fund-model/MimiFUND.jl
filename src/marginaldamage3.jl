include("fund.jl")

function marginaldamage3(;emissionyear=2010,parameters=nothing,yearstoaggregate=1000,gas=:C,useequityweights=false,eta=1.0,prtp=0.001)
    yearstorun = min(1049, getindexfromyear(emissionyear) + yearstoaggregate)


	m1 = getfund(nsteps=yearstorun,params=parameters)
	run(m1)

	m2 = getfund(nsteps=yearstorun,params=parameters)

    addcomponent(m2, adder, :marginalemission, before=:climateco2cycle)
    addem = zeros(yearstorun)
    addem[getindexfromyear(emissionyear):getindexfromyear(emissionyear)+9] = 1.0
    setparameter(m2,:marginalemission,:add,addem)
    if gas==:C
        bindparameter(m2,:marginalemission,:input,:emissions,:mco2)
        bindparameter(m2, :climateco2cycle,:mco2,:marginalemission,:output)
    else
        error("Not yet implemented")
    end
            #switch (Gas)
            #{
            #    case MarginalGas.C:
            #        f2["marginalemission"].Parameters["emission"].Bind("emissions", "mco2");
            #        f2["climateco2cycle"].Parameters["mco2"].Bind("marginalemission", "modemission");
            #        break;
            #    case MarginalGas.CH4:
            #        f2["marginalemission"].Parameters["emission"].Bind("emissions", "globch4");
            #        f2["climatech4cycle"].Parameters["globch4"].Bind("marginalemission", "modemission");
            #        break;
            #    case MarginalGas.N2O:
            #        f2["marginalemission"].Parameters["emission"].Bind("emissions", "globn2o");
            #        f2["climaten2ocycle"].Parameters["globn2o"].Bind("marginalemission", "modemission");
            #        break;
            #    case MarginalGas.SF6:
            #        f2["marginalemission"].Parameters["emission"].Bind("emissions", "globsf6");
            #        f2["climatesf6cycle"].Parameters["globsf6"].Bind("marginalemission", "modemission");
            #        break;
            #    default:
            #        throw new NotImplementedException();
            #}

    run(m2)

    damage1 = m1[:impactaggregation,:loss]
    # Take out growth effect effect of run 2 by transforming
    # the damage from run 2 into % of GDP of run 2, and then
    # multiplying that with GDP of run 1
    damage2 = m2[:impactaggregation,:loss]./m2[:socioeconomic,:income].*m1[:socioeconomic,:income]

    # Calculate the marginal damage between run 1 and 2 for each
    # year/region/sector
    marginaldamage = (damage2.-damage1)/10000000.0

    ypc = m1[:socioeconomic,:ypc]

    df = zeros(yearstorun,16)
    if !useequityweights
        for r=1:16
            x = 1.
            for t=getindexfromyear(emissionyear):yearstorun
                df[t,r] = x
                gr = (ypc[t,r]-ypc[t-1,r])/ypc[t-1,r]
                x = x / (1. + prtp + eta * gr)
            end
        end
        #df = float64([t>=getindexfromyear(emissionyear) ? (ypc[getindexfromyear(emissionyear),r]/ypc[t,r])^eta / (1.0+prtp)^(t-getindexfromyear(emissionyear)) : 0.0 for t=1:yearstorun,r=1:16])
    else
        error("not implemented")
    end

    scc = sum(marginaldamage[2:end,:].*df[2:end,:])

    return scc
end
