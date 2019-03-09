using Mimi

@defcomp geography begin
    regions = Index()

    area = Variable(index=[time,regions])

    landloss = Parameter(index=[time,regions])
    area0 = Parameter(index=[regions])

    function run_timestep(p, v, d, t);

        if is_first(t)
            for r in d.regions
                v.area[t, r] = p.area0[r]
            end
        else
            for r in d.regions
                v.area[t, r] = v.area[t - 1, r] - p.landloss[t - 1, r]
            end
        end
    end
end
