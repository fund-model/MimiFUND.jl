using Mimi

@defcomp biodiversity begin
    # Number of species
    nospecies = Variable(index=[time])

    # additive parameter
    bioloss = Parameter()

    # multiplicative parameter
    biosens = Parameter()

    # Temperature
    temp = Parameter(index=[time])

    # benchmark temperature change
    dbsta = Parameter()

    # Number of species in the year 2000
    nospecbase = Parameter()
end

function timestep(s::biodiversity, t::Int)
    v = s.Variables
    p = s.Parameters
    d = s.Dimensions

    if t > getindexfromyear(2000)
        dt = abs(p.temp[t] - p.temp[t - 1])

        v.nospecies[t] = max(
          p.nospecbase / 100,
          v.nospecies[t - 1] * (1.0 - p.bioloss - p.biosens * dt * dt / p.dbsta / p.dbsta)
          )
    else
        v.nospecies[t] = p.nospecbase
    end
end
