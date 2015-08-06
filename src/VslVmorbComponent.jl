using Mimi

@defcomp vslvmorb begin
    regions = Index()

    vsl = Variable(index=[time,regions])
    vmorb = Variable(index=[time,regions])

    population = Parameter(index=[time,regions])
    income = Parameter(index=[time,regions])

    vslbm = Parameter()
    vslel = Parameter()
    vmorbbm = Parameter()
    vmorbel = Parameter()
    vslypc0 = Parameter()
    vmorbypc0 = Parameter()
end

function timestep(s::vslvmorb, t::Int)
    v = s.Variables
    p = s.Parameters
    d = s.Dimensions

    if t>1
        for r in d.regions
            ypc = p.income[t, r] / p.population[t, r] * 1000.0

            v.vsl[t, r] = p.vslbm * (ypc / p.vslypc0)^p.vslel

            v.vmorb[t, r] = p.vmorbbm * (ypc / p.vmorbypc0)^p.vmorbel
        end
    end
end
