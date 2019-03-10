@defcomp impactaggregation begin
    regions = Index()

    eloss = Variable(index=[time,regions])
    sloss = Variable(index=[time,regions])
    loss = Variable(index=[time,regions])

    income = Parameter(index=[time,regions])

    water = Parameter(index=[time,regions])
    forests = Parameter(index=[time,regions])
    heating = Parameter(index=[time,regions])
    cooling = Parameter(index=[time,regions])
    agcost = Parameter(index=[time,regions])
    drycost = Parameter(index=[time,regions])
    protcost = Parameter(index=[time,regions])
    entercost = Parameter(index=[time,regions])
    hurrdam = Parameter(index=[time,regions])
    extratropicalstormsdam = Parameter(index=[time,regions])
    species = Parameter(index=[time,regions])
    deadcost = Parameter(index=[time,regions])
    morbcost = Parameter(index=[time,regions])
    wetcost = Parameter(index=[time,regions])
    leavecost = Parameter(index=[time,regions])

    # Other economic losses
    eloss_other = Parameter(index=[time,regions])
    # Other non-economic losses
    sloss_other = Parameter(index=[time,regions])

    function run_timestep(p, v, d, t)
        
        if is_first(t)
            for r in d.regions
                v.eloss[t, r] = 0.0
                v.sloss[t, r] = 0.0
            end
        else
            for r in d.regions
                v.eloss[t, r] = min(
                    0.0 -
                    p.water[t, r] -
                    p.forests[t, r] -
                    p.heating[t, r] -
                    p.cooling[t, r] -
                    p.agcost[t, r] +
                    p.drycost[t, r] +
                    p.protcost[t, r] +
                    p.entercost[t, r] +
                    p.hurrdam[t, r] +
                    p.extratropicalstormsdam[t, r] +
                    p.eloss_other[t,r],
                    p.income[t, r])

                v.sloss[t, r] = 0.0 +
                    p.species[t, r] +
                    p.deadcost[t, r] +
                    p.morbcost[t, r] +
                    p.wetcost[t, r] +
                    p.leavecost[t, r] +
                    p.sloss_other[t,r]

                v.loss[t, r] = (v.eloss[t, r] + v.sloss[t, r]) * 1000000000.0
            end
        end
    end
end
