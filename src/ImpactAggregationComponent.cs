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

namespace Fund.Components
{

    public interface IImpactAggregationState
    {
        IVariable2Dimensional<Timestep, Region, double> eloss { get; }
        IVariable2Dimensional<Timestep, Region, double> sloss { get; }
        IVariable2Dimensional<Timestep, Region, double> loss { get; }

        IParameter2Dimensional<Timestep, Region, double> income { get; }

        IParameter2Dimensional<Timestep, Region, double> water { get; }
        IParameter2Dimensional<Timestep, Region, double> forests { get; }
        IParameter2Dimensional<Timestep, Region, double> heating { get; }
        IParameter2Dimensional<Timestep, Region, double> cooling { get; }
        IParameter2Dimensional<Timestep, Region, double> agcost { get; }
        IParameter2Dimensional<Timestep, Region, double> drycost { get; }
        IParameter2Dimensional<Timestep, Region, double> protcost { get; }
        IParameter2Dimensional<Timestep, Region, double> entercost { get; }
        IParameter2Dimensional<Timestep, Region, double> hurrdam { get; }
        IParameter2Dimensional<Timestep, Region, double> extratropicalstormsdam { get; }
        IParameter2Dimensional<Timestep, Region, double> species { get; }
        IParameter2Dimensional<Timestep, Region, double> deadcost { get; }
        IParameter2Dimensional<Timestep, Region, double> morbcost { get; }
        IParameter2Dimensional<Timestep, Region, double> wetcost { get; }
        IParameter2Dimensional<Timestep, Region, double> leavecost { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffwater { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffforests { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffheating { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffcooling { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffagcost { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffdrycost { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffprotcost { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffentercost { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffhurrdam { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffextratropicalstormsdam { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffspecies { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffdeadcost { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffmorbcost { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffwetcost { get; }

        [DefaultParameterValue(false)]
        Boolean switchoffleavecost { get; }
    }

    public class ImpactAggregationComponent
    {
        public void Run(Clock clock, IImpactAggregationState state, IDimensions dimensions)
        {
            var s = state;
            var t = clock.Current;

            if (clock.IsFirstTimestep)
            {
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.eloss[t, r] = 0.0;
                    s.sloss[t, r] = 0.0;
                }
            }
            else
            {
                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.eloss[t, r] = Math.Min(
                        0.0
                        - (s.switchoffwater ? 0.0 : s.water[t, r])
                        - (s.switchoffforests ? 0.0 : s.forests[t, r])
                        - (s.switchoffheating ? 0.0 : s.heating[t, r])
                        - (s.switchoffcooling ? 0.0 : s.cooling[t, r])
                        - (s.switchoffagcost ? 0.0 : s.agcost[t, r])
                        + (s.switchoffdrycost ? 0.0 : s.drycost[t, r])
                        + (s.switchoffprotcost ? 0.0 : s.protcost[t, r])
                        + (s.switchoffentercost ? 0.0 : s.entercost[t, r])
                        + (s.switchoffhurrdam ? 0.0 : s.hurrdam[t, r])
                        + (s.switchoffextratropicalstormsdam ? 0.0 : s.extratropicalstormsdam[t, r]),
                        s.income[t, r]);
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.sloss[t, r] = 0.0
                        + (s.switchoffspecies ? 0.0 : s.species[t, r])
                        + (s.switchoffdeadcost ? 0.0 : s.deadcost[t, r])
                        + (s.switchoffmorbcost ? 0.0 : s.morbcost[t, r])
                        + (s.switchoffwetcost ? 0.0 : s.wetcost[t, r])
                        + (s.switchoffleavecost ? 0.0 : s.leavecost[t, r]);
                }

                foreach (var r in dimensions.GetValues<Region>())
                {
                    s.loss[t, r] = (s.eloss[t, r] + s.sloss[t, r]) * 1000000000.0;
                }
            }
        }
    }
}
