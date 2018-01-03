using Mimi

@defcomp impactdiarrhoea begin
    regions = Index()

    diadead = Variable(index=[time,regions])
    diasick = Variable(index=[time,regions])

    diamort = Parameter(index=[regions])
    diamortel = Parameter()
    diamortnl = Parameter()

    diayld = Parameter(index=[regions])
    diayldel = Parameter()
    diayldnl = Parameter()

    income = Parameter(index=[time,regions])
    population = Parameter(index=[time,regions])
    gdp90 = Parameter(index=[regions])
    pop90 = Parameter(index=[regions])

    temp90 = Parameter(index=[regions])
    bregtmp = Parameter(index=[regions])
    regtmp = Parameter(index=[time,regions])
end

function run_timestep(s::impactdiarrhoea, t::Int)
    v = s.Variables
    p = s.Parameters
    d = s.Dimensions

    for r in d.regions
        ypc = 1000.0 * p.income[t, r] / p.population[t, r]
        ypc90 = 1000.0 * p.gdp90[r] / p.pop90[r]

        # 0.49 is the increase in global temperature from pre-industrial to 1990
        absoluteRegionalTempPreIndustrial = p.temp90[r] - 0.49 * p.bregtmp[r]

        if absoluteRegionalTempPreIndustrial > 0.0
            v.diadead[t, r] = p.diamort[r] * p.population[t, r] * (ypc / ypc90)^p.diamortel * (((absoluteRegionalTempPreIndustrial + p.regtmp[t, r]) / absoluteRegionalTempPreIndustrial)^p.diamortnl - 1.0)

            v.diasick[t, r] = p.diayld[r] * p.population[t, r] * (ypc / ypc90)^p.diayldel * (((absoluteRegionalTempPreIndustrial + p.regtmp[t, r]) / absoluteRegionalTempPreIndustrial)^p.diayldnl - 1.0)
        else
            v.diadead[t, r] = 0.0
            v.diasick[t, r] = 0.0
        end
    end
end
