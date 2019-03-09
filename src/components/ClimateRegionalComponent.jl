using Mimi

@defcomp climateregional begin
    regions = Index()
    inputtemp = Parameter(index=[time])
    bregtmp = Parameter(index=[regions])
    bregstmp = Parameter(index=[regions])
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
