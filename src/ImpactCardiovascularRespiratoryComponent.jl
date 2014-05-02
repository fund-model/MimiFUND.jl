using IAMF

@defcomp impactcardiovascularrespiratory begin
    regions = Index()

    basecardvasc = Variable(index=[time,regions])
    baseresp = Variable(index=[time,regions])

    cardheat = Variable(index=[time,regions])
    resp = Variable(index=[time,regions])
    cardcold = Variable(index=[time,regions])

    IParameter1Dimensional<Region, double> cardvasc90 = Parameter(index=[regions])
    IParameter1Dimensional<Region, double> plus90 = Parameter(index=[regions])
    IParameter1Dimensional<Region, double> resp90 = Parameter(index=[regions])
    IParameter1Dimensional<Region, double> chplbm = Parameter(index=[regions])
    IParameter1Dimensional<Region, double> chmlbm = Parameter(index=[regions])
    IParameter1Dimensional<Region, double> chpqbm = Parameter(index=[regions])
    IParameter1Dimensional<Region, double> chmqbm = Parameter(index=[regions])
    IParameter1Dimensional<Region, double> rlbm = Parameter(index=[regions])
    IParameter1Dimensional<Region, double> rqbm = Parameter(index=[regions])
    IParameter1Dimensional<Region, double> ccplbm = Parameter(index=[regions])
    IParameter1Dimensional<Region, double> ccmlbm = Parameter(index=[regions])
    IParameter1Dimensional<Region, double> ccpqbm = Parameter(index=[regions])
    IParameter1Dimensional<Region, double> ccmqbm = Parameter(index=[regions])

    IParameter2Dimensional<Timestep, Region, double> plus { get; }
    IParameter2Dimensional<Timestep, Region, double> temp { get; }
    IParameter2Dimensional<Timestep, Region, double> urbpop { get; }
    IParameter2Dimensional<Timestep, Region, double> population { get; }

    double cvlin { get; }
    double rlin { get; }
    double maxcardvasc { get; }
end

    public class ImpactCardiovascularRespiratoryComponent
    {
        public void Run(Clock clock, IImpactCardiovascularRespiratoryState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {

            }
            else
            {
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.basecardvasc[t, r] = s.cardvasc90[r] + s.cvlin * (s.plus[t, r] - s.plus90[r]);
                    if (s.basecardvasc[t, r] > 1.0)
                        s.basecardvasc[t, r] = 1.0;

                    s.baseresp[t, r] = s.resp90[r] + s.rlin * (s.plus[t, r] - s.plus90[r]);
                    if (s.baseresp[t, r] > 1.0)
                        s.baseresp[t, r] = 1.0;

                    s.cardheat[t, r] = (s.chplbm[r] * s.plus[t, r] + s.chmlbm[r] * (1.0 - s.plus[t, r])) * s.temp[t, r] +
                               (s.chpqbm[r] * s.plus[t, r] + s.chmqbm[r] * (1.0 - s.plus[t, r])) * Math.Pow(s.temp[t, r], 2);
                    s.cardheat[t, r] = s.cardheat[t, r] * s.urbpop[t, r] * s.population[t, r] * 10;
                    if (s.cardheat[t, r] > 1000.0 * s.maxcardvasc * s.basecardvasc[t, r] * s.urbpop[t, r] * s.population[t, r])
                        s.cardheat[t, r] = 1000 * s.maxcardvasc * s.basecardvasc[t, r] * s.urbpop[t, r] * s.population[t, r];
                    if (s.cardheat[t, r] < 0.0)
                        s.cardheat[t, r] = 0;

                    s.resp[t, r] = s.rlbm[r] * s.temp[t, r] + s.rqbm[r] * Math.Pow(s.temp[t, r], 2);
                    s.resp[t, r] = s.resp[t, r] * s.urbpop[t, r] * s.population[t, r] * 10;
                    if (s.resp[t, r] > 1000 * s.maxcardvasc * s.baseresp[t, r] * s.urbpop[t, r] * s.population[t, r])
                        s.resp[t, r] = 1000 * s.maxcardvasc * s.baseresp[t, r] * s.urbpop[t, r] * s.population[t, r];
                    if (s.resp[t, r] < 0)
                        s.resp[t, r] = 0;

                    s.cardcold[t, r] = (s.ccplbm[r] * s.plus[t, r] + s.ccmlbm[r] * (1.0 - s.plus[t, r])) * s.temp[t, r] +
                               (s.ccpqbm[r] * s.plus[t, r] + s.ccmqbm[r] * (1.0 - s.plus[t, r])) * Math.Pow(s.temp[t, r], 2);
                    s.cardcold[t, r] = s.cardcold[t, r] * s.population[t, r] * 10;
                    if (s.cardcold[t, r] < -1000 * s.maxcardvasc * s.basecardvasc[t, r] * s.population[t, r])
                        s.cardcold[t, r] = -1000 * s.maxcardvasc * s.basecardvasc[t, r] * s.population[t, r];
                    if (s.cardcold[t, r] > 0)
                        s.cardcold[t, r] = 0;
                }
            }
        }

    }
}
