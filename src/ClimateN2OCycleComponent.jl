using Mimi

@defcomp climaten2ocycle begin
    # Global N2O emissions in Mt of N
    globn2o = Parameter(index=[time])

    # Atmospheric N2O concentration
    acn2o = Variable(index=[time])

    # N2O decay
    n2odecay = Variable()

    #
    lifen2o = Parameter()

    # N2o pre industrial
    n2opre = Parameter()
end

function timestep(s::climaten2ocycle, t::Int)
    v = s.Variables
    p = s.Parameters
    d = s.Dimensions

    if t==1
        v.n2odecay = 1.0 / p.lifen2o

        v.acn2o[t] = 296
    else
        # Calculate N2O concentrations
        v.acn2o[t] = v.acn2o[t - 1] + 0.2079 * p.globn2o[t] - v.n2odecay * (v.acn2o[t - 1] - p.n2opre)

        if v.acn2o[t] < 0
            error("n2o atmospheric concentration out of range")
        end
    end
end
