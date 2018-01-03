// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace Esmf
{
    public class Clock : IEnumerable<Timestep>
    {
        private Timestep _startTime;
        private Timestep? _stopTime;
        private Timestep _current;

        public Clock(Timestep startTime, Timestep? stopTime)
        {
            _startTime = startTime;
            _stopTime = stopTime;
            _current = startTime;
        }

        public Timestep StartTime
        {
            get { return _startTime; }
        }

        public Timestep? StopTime
        {
            get { return _stopTime; }
        }

        public Timestep Current
        {
            get
            {
                if (!_stopTime.HasValue)
                {
                    if (IsDone)
                    {
                        throw new InvalidOperationException("Cannot determine Current when clock is done");
                    }
                }
                return _current;
            }
        }

        public bool IsDone
        {
            get
            {
                if (!_stopTime.HasValue)
                {
                    throw new InvalidOperationException("Cannot determine IsDone for clocks witout a StopTime");
                }

                return _current > _stopTime.Value;
            }
        }

        public bool IsLastTimestep
        {
            get
            {
                if (!_stopTime.HasValue)
                {
                    throw new InvalidOperationException("Cannot determine IsLastTimestep for clocks witout a StopTime");
                }

                return _current == _stopTime.Value;
            }
        }

        public bool IsFirstTimestep
        {
            get
            {
                return _current == _startTime;
            }
        }

        public void Advance()
        {
            if (_stopTime.HasValue)
            {
                if (IsDone)
                {
                    throw new InvalidOperationException("Cannot advance clock beyond StopTime");
                }
            }
            _current = Timestep.FromSimulationYear(_current.Value + 1);
        }

        public void Reset()
        {
            _current = _startTime;
        }

        public int Timestepcount
        {
            get
            {
                return _stopTime.Value.Value - _startTime.Value + 1;
            }
        }

        public IEnumerator<Timestep> GetEnumerator()
        {
            for (int i = _startTime.Value; i < _stopTime.Value.Value; i++)
            {
                yield return Timestep.FromSimulationYear(i);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            for (int i = _startTime.Value; i < _stopTime.Value.Value; i++)
            {
                yield return Timestep.FromSimulationYear(i);
            }
        }
    }
}
