// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Esmf;
using Fund.CommonDimensions;

namespace Fund
{
    public class Playground
    {
        public void Run()
        {
            // Load parameters
            var parameters = new Parameters();
            parameters.ReadDirectory(@"Data\Base");

            // Get best guess parameter values
            var parameterValues = parameters.GetBestGuess();

            // Create a new model that inits itself from the parameters just loaded
            var model = FundModel.GetModel();

            // Run the model
            var rs = model.Run(parameterValues);

            // Display all variables in interactive window
            OutputHelper.ShowModel(rs);
        }

    }
}