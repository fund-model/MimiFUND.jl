using Mimi

@defcomp climateco2cycle begin
    # Anthropogenic CO2 emissions in Mt of C
    mco2 = Parameter(index=[time])

    # Terrestrial biosphere CO2 emissions in Mt of C
    TerrestrialCO2 = Variable(index=[time])

    # Net CO2 emissions in Mt of C
    globc = Variable(index=[time])

    # Carbon boxes
    cbox = Variable(index=[time,5])

    # Initial carbon box 1
    cbox10 = Parameter()

    # Initial carbon box 2
    cbox20 = Parameter()

    # Initial carbon box 3
    cbox30 = Parameter()

    # Initial carbon box 4
    cbox40 = Parameter()

    # Initial carbon box 5
    cbox50 = Parameter()

    # Carbon decay in box 1
    co2decay1 = Variable()

    # Carbon decay in box 2
    co2decay2 = Variable()

    # Carbon decay in box 3
    co2decay3 = Variable()

    # Carbon decay in box 4
    co2decay4 = Variable()

    # Carbon decay in box 5
    co2decay5 = Variable()

    # Carbon decay in box 1
    lifeco1 = Parameter()

    # Carbon decay in box 2
    lifeco2 = Parameter()

    # Carbon decay in box 3
    lifeco3 = Parameter()

    # Carbon decay in box 4
    lifeco4 = Parameter()

    # Carbon decay in box 5
    lifeco5 = Parameter()

    # Fraction of carbon emission in box 1
    co2frac1 = Parameter()

    # Fraction of carbon emission in box 2
    co2frac2 = Parameter()

    # Fraction of carbon emission in box 3
    co2frac3 = Parameter()

    # Fraction of carbon emission in box 4
    co2frac4 = Parameter()

    # Fraction of carbon emission in box 5
    co2frac5 = Parameter()

    # Atmospheric CO2 concentration
    acco2 = Variable(index=[time])

    # Stock of CO2 in the terrestrial biosphere
    TerrCO2Stock = Variable(index=[time])

    # Temperature
    temp = Parameter(index=[time])

    TerrCO2Sens = Parameter()
    TerrCO2Stock0 = Parameter()

    tempIn2010 = Variable()

    function run_timestep(p, v, d, t)

        if t==1
            v.co2decay1 = p.lifeco1
            v.co2decay2 = exp(-1.0 / p.lifeco2)
            v.co2decay3 = exp(-1.0 / p.lifeco3)
            v.co2decay4 = exp(-1.0 / p.lifeco4)
            v.co2decay5 = exp(-1.0 / p.lifeco5)

            v.TerrCO2Stock[t] = p.TerrCO2Stock0

            v.cbox[t,1] = p.cbox10
            v.cbox[t,2] = p.cbox20
            v.cbox[t,3] = p.cbox30
            v.cbox[t,4] = p.cbox40
            v.cbox[t,5] = p.cbox50
            v.acco2[t] = v.cbox[t,1] + v.cbox[t,2] + v.cbox[t,3] + v.cbox[t,4] + v.cbox[t,5]
        else

            if t == getindexfromyear(2011)
                v.tempIn2010 = p.temp[getindexfromyear(2010)]
            end

            if t > getindexfromyear(2010)
                v.TerrestrialCO2[t] = (p.temp[t - 1] - v.tempIn2010) * p.TerrCO2Sens * v.TerrCO2Stock[t - 1] / p.TerrCO2Stock0
            else
                v.TerrestrialCO2[t] = 0
            end

            v.TerrCO2Stock[t] = max(v.TerrCO2Stock[t - 1] - v.TerrestrialCO2[t], 0.0)

            v.globc[t] = p.mco2[t] + v.TerrestrialCO2[t]

            # Calculate CO2 concentrations
            v.cbox[t,1] = v.cbox[t - 1,1] * v.co2decay1 + 0.000471 * p.co2frac1 * (v.globc[t])
            v.cbox[t,2] = v.cbox[t - 1,2] * v.co2decay2 + 0.000471 * p.co2frac2 * (v.globc[t])
            v.cbox[t,3] = v.cbox[t - 1,3] * v.co2decay3 + 0.000471 * p.co2frac3 * (v.globc[t])
            v.cbox[t,4] = v.cbox[t - 1,4] * v.co2decay4 + 0.000471 * p.co2frac4 * (v.globc[t])
            v.cbox[t,5] = v.cbox[t - 1,5] * v.co2decay5 + 0.000471 * p.co2frac5 * (v.globc[t])

            v.acco2[t] = v.cbox[t,1] + v.cbox[t,2] + v.cbox[t,3] + v.cbox[t,4] + v.cbox[t,5]
        end
    end
end