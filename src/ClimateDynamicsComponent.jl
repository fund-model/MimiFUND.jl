using Mimi

@defcomp climatedynamics begin
    # Total radiative forcing
    radforc = Parameter(index=[time])

    # Average global temperature
    temp = Variable(index=[time])

    # LifeTempConst
    LifeTempConst = Parameter()

    # LifeTempLin
    LifeTempLin = Parameter()

    # LifeTempQd
    LifeTempQd = Parameter()

    # Climate sensitivity
    ClimateSensitivity = Parameter()
end

function run_timestep(s::climatedynamics, t::Int)
    v = s.Variables
    p = s.Parameters
    d = s.Dimensions

    if t==1
        v.temp[t] = 0.20
    else
        LifeTemp = max(p.LifeTempConst + p.LifeTempLin * p.ClimateSensitivity + p.LifeTempQd * p.ClimateSensitivity^2.0, 1.0)

        delaytemp = 1.0 / LifeTemp

        temps = p.ClimateSensitivity / 5.35 / log(2.0)

        # Calculate temperature
        dtemp = delaytemp * temps * p.radforc[t] - delaytemp * v.temp[t - 1]

        v.temp[t] = v.temp[t - 1] + dtemp
    end
end
