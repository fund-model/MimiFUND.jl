using IAMF

@defcomp ocean begin
    # Sea-level rise in cm
    sea = Variable(index=[time])

    lifesea = Parameter()

    seas = Parameter()

    delaysea = Variable()

    # Temperature incrase in C°
    temp = Parameter(index=[time])
end

function timestep(s::ocean, t::Int)
    v = s.Variables
    p = s.Parameters
    d = s.Dimensions

    if t==1
        # Delay in sea-level rise
        v.delaysea = 1.0 / p.lifesea
        v.sea[t] = 0.0
    else
        # Calculate sea level rise  
        ds = v.delaysea * p.seas * p.temp[t] - v.delaysea * v.sea[t - 1]

        v.sea[t] = v.sea[t - 1] + ds
    end
end
