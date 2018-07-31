using Mimi

@defcomp geography begin
    regions = Index()

    area = Variable(index=[time,regions])

    landloss = Parameter(index=[time,regions])
    area0 = Parameter(index=[regions], default = [9.207581e6, 9.872483e6, 3.70504e6, 472934.0, 9.235211e6, 2.186128e6, 2.1716915e7, 6.313671e6, 2.440575e6, 1.7451759e7, 5.137567e6, 4.958546e6, 1.0958008e7, 3.953026e6, 2.4329621e7, 332534.0])

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
