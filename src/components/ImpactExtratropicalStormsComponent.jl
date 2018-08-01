using Mimi

@defcomp impactextratropicalstorms begin
    regions = Index()

    extratropicalstormsdam = Variable(index=[time,regions])
    extratropicalstormsdead = Variable(index=[time,regions])

    extratropicalstormsbasedam = Parameter(index=[regions], default = [0.000120686088558705, 0.000169725473424971, 0.000209184976098256, 1.04095585911881e-5, 0.000276263769143954, 4.58675473470203e-5, 4.40559748289577e-5, 1.56246897280654e-5, 4.40559748289577e-5, 3.57676023128835e-6, 0.000550631334687187, 6.27063594877531e-5, 0.000167734149328056, 2.81278071016012e-7, 0.000550631334687187, 0.00042688685396462])
    extratropicalstormsdamel = Parameter(default = -0.514)
    extratropicalstormspar = Parameter(index=[regions], default = [0.04, 0.04, 0.04, 0.04, 0.21, 0.04, 0.04, 0.04, 0.04, 0.21, 0.21, 0.04, 0.04, 0.04, 0.04, 0.13])
    extratropicalstormsbasedead = Parameter(index=[regions], default = [0.000291214400406601, 6.31174562156123e-5, 0.000121209462453345, 0.000114939830669896, 0.000116317931842844, 5.00813934569767e-5, 0.000126842679816114, 5.29869050563746e-5, 0.000126842679816114, 4.65277938646014e-5, 0.000204864801256902, 8.57220395204113e-5, 0.000114203457196663, 3.83465156512044e-5, 0.000204864801256902, 0.00157792749557269])
    extratropicalstormsdeadel = Parameter(default = -0.501)
    extratropicalstormsnl = Parameter(default = 1)

    gdp90 = Parameter(index=[regions], default = [6343.0, 532.5, 8285.4, 5100.7, 359.5, 338.1, 915.1, 447.3, 296.0, 1078.0, 351.5, 456.2, 511.4, 137.3, 275.1, 37.2])
    pop90 = Parameter(index=[regions], default = [254.1, 27.8, 376.6, 166.4, 20.2, 124.3, 289.6, 189.2, 106.3, 300.2, 1131.7, 444.1, 1184.1, 117.8, 494.4, 39.9])

    population = Parameter(index=[time,regions])
    income = Parameter(index=[time,regions])

    acco2 = Parameter(index=[time])
    co2pre = Parameter(default = 275.0)

    function run_timestep(p, v, d, t)

        for r in d.regions
            ypc = p.income[t, r] / p.population[t, r] * 1000.0
            ypc90 = p.gdp90[r] / p.pop90[r] * 1000.0

            v.extratropicalstormsdam[t, r] = p.extratropicalstormsbasedam[r] * p.income[t, r] * (ypc / ypc90)^p.extratropicalstormsdamel * ((1.0 + (p.extratropicalstormspar[r] * (p.acco2[t] / p.co2pre)))^p.extratropicalstormsnl - 1.0)
            v.extratropicalstormsdead[t, r] = 1000.0 * p.extratropicalstormsbasedead[r] * p.population[t, r] * (ypc / ypc90)^p.extratropicalstormsdeadel * ((1.0 + (p.extratropicalstormspar[r] * (p.acco2[t] / p.co2pre)))^p.extratropicalstormsnl - 1.0)
        end
    end
end
