using IAMF

@defcomp socioeconomic begin
	addIndex(region)

	addVariable(income, Float64, index=[time,region])
	addVariable(consumption, Float64, index=[time,region])
	addVariable(ypc, Float64, index=[time,region])
	addVariable(ygrowth, Float64, index=[time,region])

	addVariable(plus, Float64, index=[time,region])
	addVariable(urbpop, Float64, index=[time,region])
	addVariable(popdens, Float64, index=[time,region])

	addVariable(globalconsumption, Float64, index=[time])
	addVariable(globalypc, Float64, index=[time])
	addVariable(globalincome, Float64, index=[time])

	addVariable(ypc90, Float64, index=[region])


	addParameter(pgrowth, Float64, index=[time,region])
	addParameter(ypcgrowth, Float64, index=[time,region])
	addParameter(eloss, Float64, index=[time,region])
	addParameter(sloss, Float64, index=[time,region])
	addParameter(mitigationcost, Float64, index=[time,region])
	addParameter(area, Float64, index=[time,region])
	addParameter(globalpopulation, Float64, index=[time])
	addParameter(population, Float64, index=[time,region])
	addParameter(populationin1, Float64, index=[time,region])

	addParameter(plus90, Float64, index=[region])
	addParameter(gdp90, Float64, index=[region])
	addParameter(pop90, Float64, index=[region])
	addParameter(urbcorr, Float64, index=[region])
	addParameter(gdp0, Float64, index=[region])


	addParameter(runwithoutdamage, Bool)

	addParameter(consleak, Float64)
	addParameter(plusel, Float64)
end


    public class SocioEconomicComponent
    {
        public void Run(Clock clock, ISocioEconomicState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            double savingsrate = 0.2;

            if (clock.IsFirstTimestep)
            {
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.income[t, r] = s.gdp0[r];
                    s.ypc[t, r] = s.income[t, r] / s.population[t, r] * 1000.0;
                    s.consumption[t, r] = s.income[t, r] * 1000000000.0 * (1.0 - savingsrate);
                }

                s.globalconsumption[t] = dimensions.GetValues<Region>().Select(r => s.consumption[t, r]).Sum();

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.ypc90[r] = s.gdp90[r] / s.pop90[r] * 1000;
                }
            }
            else
            {

                // Calculate income growth rate  
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.ygrowth[t, r] = (1 + 0.01 * s.pgrowth[t - 1, r]) * (1 + 0.01 * s.ypcgrowth[t - 1, r]) - 1;
                }

                // Calculate income  
                foreach (var r in dimensions.GetValues<Region>())
                {
                    double oldincome = s.income[t - 1, r] - ((t >= Timestep.FromSimulationYear(40)) && !s.runwithoutdamage ? s.consleak * s.eloss[t - 1, r] / 10.0 : 0);

                    s.income[t, r] = (1 + s.ygrowth[t, r]) * oldincome - s.mitigationcost[t - 1, r];
                }

                // Check for unrealistic values
                foreach (var r in dimensions.GetValues<Region>())
                {
                    if (s.income[t, r] < 0.01 * s.population[t, r])
                        s.income[t, r] = 0.1 * s.population[t, r];

                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.ypc[t, r] = s.income[t, r] / s.population[t, r] * 1000.0;
                }

                var totalConsumption = 0.0;
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.consumption[t, r] = Math.Max(
                        s.income[t, r] * 1000000000.0 * (1.0 - savingsrate) - (s.runwithoutdamage ? 0.0 : (s.eloss[t - 1, r] + s.sloss[t - 1, r]) * 1000000000.0),
                      0.0);

                    totalConsumption = totalConsumption + s.consumption[t, r];
                }
                s.globalconsumption[t] = totalConsumption;

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.plus[t, r] = s.plus90[r] * Math.Pow(s.ypc[t, r] / s.ypc90[r], s.plusel);

                    if (s.plus[t, r] > 1)
                        s.plus[t, r] = 1.0;
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.popdens[t, r] = s.population[t, r] / s.area[t, r] * 1000000.0;
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.urbpop[t, r] = (0.031 * Math.Sqrt(s.ypc[t, r]) - 0.011 * Math.Sqrt(s.popdens[t, r])) / (1.0 + 0.031 * Math.Sqrt(s.ypc[t, r]) - 0.011 * Math.Sqrt(s.popdens[t, r]))
                        / (1 + s.urbcorr[r] / (1 + 0.001 * Math.Pow(Convert.ToDouble(t.Value) - 40.0, 2))); // (* DA: urbcorr needs to be changed to a function if this is to be made uncertain *)

                }

                s.globalincome[t] = dimensions.GetValues<Region>().Select(r => s.income[t, r]).Sum();

                s.globalypc[t] = dimensions.GetValues<Region>().Select(r => s.income[t, r] * 1000000000.0).Sum() /
                    dimensions.GetValues<Region>().Select(r => s.populationin1[t, r]).Sum();
            }
        }
    }
}
