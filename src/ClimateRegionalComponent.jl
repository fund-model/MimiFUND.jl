using Mimi

@defcomp climateregional begin
    regions = Index()
    inputtemp = Parameter(index=[time])
    bregtmp = Parameter(index=[regions], default = [1.1941, 1.4712, 1.1248, 1.0555, 0.9676, 1.1676, 1.2866, 1.1546, 0.8804, 0.8504, 0.9074, 0.7098, 1.1847, 1.143, 0.878, 0.7517])
    bregstmp = Parameter(index=[regions], default = [0.806731417004106, 0.806731417004106, 0.82493168808336, 0.792371381530722, 0.833346282293551, 1.0, 0.898352292487305, 1.03966017376275, 0.806731417004106, 0.742522668880902, 1.03966017376275, 0.865849287390427, 0.792371381530722, 1.0, 0.905134210436234, 0.814964431252555])
    scentemp = Parameter(index=[time, regions])

    temp = Variable(index=[time,regions])
    regtmp = Variable(index=[time,regions])
    regstmp = Variable(index=[time,regions])

    function run_timestep(p, v, d, t)

        for r in d.regions
            v.regtmp[t, r] = p.inputtemp[t] * p.bregtmp[r] + p.scentemp[t, r]
        end

        for r in d.regions
            v.temp[t, r] = v.regtmp[t, r] / p.bregtmp[r]
        end

        for r in d.regions
            v.regstmp[t, r] = p.inputtemp[t] * p.bregstmp[r] + p.scentemp[t, r]
        end
    end
end
