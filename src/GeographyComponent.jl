using Mimi

@defcomp geography begin
    regions = Index()

    area = Variable(index=[time,regions])

    landloss = Parameter(index=[time,regions])
    area0 = Parameter(index=[regions])
end

function run_timestep(s::geography, t::Int)
    v = s.Variables
    p = s.Parameters
    d = s.Dimensions

    if t==1
        for r in d.regions
            v.area[t, r] = p.area0[r]
        end
    else
        for r in d.regions
            v.area[t, r] = v.area[t - 1, r] - p.landloss[t - 1, r]
        end
    end
end
