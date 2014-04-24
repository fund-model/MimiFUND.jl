using IAMF

@defcomp geography begin
    regions = Index()

    area = Variable(index=[time,regions])

    landloss = Parameter(index=[time,regions])
    area0 = Parameter(index=[regions])
end

function init(s::geography)    
    v = s.Variables
    p = s.Parameters
    d = s.Dimensions

    t = 1

    for r in d.regions
        v.area[t, r] = p.area0[r]
    end
end

function timestep(s::geography, t::Int)
    v = s.Variables
    p = s.Parameters
    d = s.Dimensions

    for r in d.regions
        v.area[t, r] = v.area[t - 1, r] - p.landloss[t - 1, r]
    end
end
