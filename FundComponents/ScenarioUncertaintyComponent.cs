// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmf;
using Fund.CommonDimensions;

namespace Fund.Components.ScenarioUncertainty
{

    public interface IScenarioUncertaintyState
    {
        IVariable2Dimensional<Timestep, Region, double> pgrowth { get; }
        IVariable2Dimensional<Timestep, Region, double> ypcgrowth { get; }
        IVariable2Dimensional<Timestep, Region, double> aeei { get; }
        IVariable2Dimensional<Timestep, Region, double> acei { get; }
        IVariable2Dimensional<Timestep, Region, double> forestemm { get; }

        Timestep timeofuncertaintystart { get; }

        IParameter2Dimensional<Timestep, Region, double> scenpgrowth { get; }
        IParameter2Dimensional<Timestep, Region, double> scenypcgrowth { get; }
        IParameter2Dimensional<Timestep, Region, double> scenaeei { get; }
        IParameter2Dimensional<Timestep, Region, double> scenacei { get; }
        IParameter2Dimensional<Timestep, Region, double> scenforestemm { get; }

        IParameter1Dimensional<Region, double> ecgradd { get; }
        IParameter1Dimensional<Region, double> pgadd { get; }
        IParameter1Dimensional<Region, double> aeeiadd { get; }
        IParameter1Dimensional<Region, double> aceiadd { get; }
        IParameter1Dimensional<Region, double> foremadd { get; }
    }


    public class ScenarioUncertaintyComponent
    {
        public void Run(Clock clock, IScenarioUncertaintyState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            Double yearsFromUncertaintyStart = t.Value - s.timeofuncertaintystart.Value;
            var sdTimeFactor = (yearsFromUncertaintyStart / 50.0) / (1.0 + (yearsFromUncertaintyStart / 50.0));

            foreach (var r in dimensions.GetValues<Region>())
            {

                s.ypcgrowth[t, r] = s.scenypcgrowth[t, r] + (t >= s.timeofuncertaintystart ? s.ecgradd[r] * sdTimeFactor : 0.0);
                s.pgrowth[t, r] = s.scenpgrowth[t, r] + (t >= s.timeofuncertaintystart ? s.pgadd[r] * sdTimeFactor : 0.0);
                s.aeei[t, r] = s.scenaeei[t, r] + (t >= s.timeofuncertaintystart ? s.aeeiadd[r] * sdTimeFactor : 0.0);
                s.acei[t, r] = s.scenacei[t, r] + (t >= s.timeofuncertaintystart ? s.aceiadd[r] * sdTimeFactor : 0.0);
                s.forestemm[t, r] = s.scenforestemm[t, r] + (t >= s.timeofuncertaintystart ? s.foremadd[r] * sdTimeFactor : 0.0);
            }

        }
    }
}
