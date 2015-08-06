using Mimi

@defcomp impacttropicalstorms begin
    regions = Index()

    hurrdam = Variable(index=[time,regions])
    hurrdead = Variable(index=[time,regions])

    hurrbasedam = Parameter(index=[regions])
    hurrdamel = Parameter()
    hurrnl = Parameter()
    hurrpar = Parameter()
    hurrbasedead = Parameter(index=[regions])
    hurrdeadel = Parameter()

    gdp90 = Parameter(index=[regions])
    pop90 = Parameter(index=[regions])

    population = Parameter(index=[time,regions])
    income = Parameter(index=[time,regions])

    regstmp = Parameter(index=[time,regions])
end

function timestep(s::impacttropicalstorms, t::Int)
    v = s.Variables
    p = s.Parameters
    d = s.Dimensions

    for r in d.regions
        ypc = p.income[t, r] / p.population[t, r] * 1000.0
        ypc90 = p.gdp90[r] / p.pop90[r] * 1000.0

        # This is hurrican damage
        v.hurrdam[t, r] = 0.001 * p.hurrbasedam[r] * p.income[t, r] * (ypc / ypc90)^p.hurrdamel * ((1.0 + p.hurrpar * p.regstmp[t, r])^p.hurrnl - 1.0)

        v.hurrdead[t, r] = 1000.0 * p.hurrbasedead[r] * p.population[t, r] * (ypc / ypc90)^p.hurrdeadel * ((1.0 + p.hurrpar * p.regstmp[t, r])^p.hurrnl - 1.0)
    end
end
