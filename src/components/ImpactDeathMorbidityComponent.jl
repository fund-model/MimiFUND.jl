using Mimi

@defcomp impactdeathmorbidity begin
    regions = Index()

    dead = Variable(index=[time,regions])
    yll = Variable(index=[time,regions])
    yld = Variable(index=[time,regions])
    deadcost = Variable(index=[time,regions])
    morbcost = Variable(index=[time,regions])
    vsl = Parameter(index=[time,regions])
    vmorb = Parameter(index=[time,regions])

    d2ld = Parameter(index=[regions], default = [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 36.6667, 36.0, 29.0, 0.0, 21.0, 0.0])
    d2ls = Parameter(index=[regions], default = [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 21.5, 10.0, 10.0, 0.0, 7.0, 16.0, 21.5, 20.0, 10.0])
    d2lm = Parameter(index=[regions], default = [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 29.1429, 28.0, 28.0, 29.5769, 29.5714, 0.0, 29.1429, 33.3128, 28.0])
    d2lc = Parameter(index=[regions], default = [4.8151, 4.8151, 4.8151, 4.8151, 4.8151, 6.0782, 6.0782, 11.5846, 8.6426, 8.6426, 8.9594, 11.9348, 7.6706, 11.5846, 12.9166, 8.6426])
    d2lr = Parameter(index=[regions], default = [5.1545, 5.1545, 5.1545, 5.1545, 5.1545, 6.9114, 6.9114, 17.135, 11.9483, 11.9483, 12.0112, 12.6351, 6.2451, 17.135, 14.6857, 11.9483])
    d2dd = Parameter(index=[regions], default = [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.25, 0.4286, 0.0, 0.0, 0.0, 0.0])
    d2ds = Parameter(index=[regions], default = [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 51.5, 69.0, 69.0, 0.0, 6.0, 11.0, 51.5, 293.75, 69.0])
    d2dm = Parameter(index=[regions], default = [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 24.8571, 4.5714, 4.5714, 16.3462, 3.2727, 0.0, 24.8571, 3.694, 4.5714])
    d2dc = Parameter(index=[regions], default = [0.9609, 0.9609, 0.9609, 0.9609, 0.9609, 0.8986, 0.8986, 1.3459, 1.2548, 1.2548, 1.3879, 1.3729, 1.2399, 1.3459, 1.3301, 1.2548])
    d2dr = Parameter(index=[regions], default = [8.7638, 8.7638, 8.7638, 8.7638, 8.7638, 11.8101, 11.8101, 21.8098, 22.1552, 22.1552, 16.5094, 20.0541, 8.3072, 21.8098, 21.5857, 22.1552])

    dengue = Parameter(index=[time,regions])
    schisto = Parameter(index=[time,regions])
    malaria = Parameter(index=[time,regions])
    cardheat = Parameter(index=[time,regions])
    cardcold = Parameter(index=[time,regions])
    resp = Parameter(index=[time,regions])
    diadead = Parameter(index=[time,regions])
    hurrdead = Parameter(index=[time,regions])
    extratropicalstormsdead = Parameter(index=[time,regions])
    population = Parameter(index=[time,regions])
    diasick = Parameter(index=[time,regions])
    # Other sources of death
    dead_other = Parameter(index=[time,regions])    # has default, but is 2-dimensional
    # Other sources of sickness
    sick_other = Parameter(index=[time,regions])    # has default, but is 2-dimensional

    function run_timestep(p, v, d, t)

        if !is_first(t)
            for r in d.regions
                v.dead[t, r] = p.dengue[t, r] + p.schisto[t, r] + p.malaria[t, r] + p.cardheat[t, r] + p.cardcold[t, r] + p.resp[t, r] + p.diadead[t, r] + p.hurrdead[t, r] + p.extratropicalstormsdead[t, r] + p.dead_other[t,r]
                if v.dead[t, r] > p.population[t, r] * 1000000.0
                    v.dead[t, r] = p.population[t, r] / 1000000.0
                end

                v.yll[t, r] = p.d2ld[r] * p.dengue[t, r] + p.d2ls[r] * p.schisto[t, r] + p.d2lm[r] * p.malaria[t, r] + p.d2lc[r] * p.cardheat[t, r] + p.d2lc[r] * p.cardcold[t, r] + p.d2lr[r] * p.resp[t, r]

                v.yld[t, r] = p.d2dd[r] * p.dengue[t, r] + p.d2ds[r] * p.schisto[t, r] + p.d2dm[r] * p.malaria[t, r] + p.d2dc[r] * p.cardheat[t, r] + p.d2dc[r] * p.cardcold[t, r] + p.d2dr[r] * p.resp[t, r] + p.diasick[t, r] + p.sick_other[t,r]

                v.deadcost[t, r] = p.vsl[t, r] * v.dead[t, r] / 1000000000.0
                # deadcost:= vyll*ypc*yll/1000000000

                v.morbcost[t, r] = p.vmorb[t, r] * v.yld[t, r] / 1000000000.0
            end
        end
    end
end