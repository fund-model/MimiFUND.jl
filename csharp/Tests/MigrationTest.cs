// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fund;
using Esmf;
using Fund.CommonDimensions;

namespace Fund.Tests
{
    /// <summary>
    /// Summary description for MigrationTest
    /// </summary>
    [TestClass]
    public class MigrationTest
    {
        public MigrationTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [DeploymentItem("Fund\\Data\\Parameter - Base.xlsm")]
        public void TestMigrationDeterministic()
        {
            var parameterDefinition = new Parameters();
            parameterDefinition.ReadExcelFile("Parameter - Base.xlsm");
            var p = parameterDefinition.GetBestGuess();

            var m = FundModel.GetModel();
            var res = m.Run(p);

            var c = new Clock(Timestep.FromYear(1951), Timestep.FromYear(2999));

            while (!c.IsLastTimestep)
            {
                var regions = res.Dimensions.GetValues<Region>();

                double totalLeave = (from r in regions select (double)res["ImpactSeaLevelRise", "leave"][c.Current, r]).Sum();

                double totalEnter = (from r in regions select (double)res["ImpactSeaLevelRise", "enter"][c.Current, r]).Sum();

                Assert.IsTrue(Math.Abs(totalLeave - totalEnter) < 1.0, "Migration doesn't even out");
                c.Advance();
            }
        }

        [TestMethod]
        [DeploymentItem("Fund\\Data\\Parameter - Base.xlsm")]
        public void TestMigrationProbabalistic()
        {
            var parameterDefinition = new Parameters();
            parameterDefinition.ReadExcelFile("Parameter - Base.xlsm");
            var p = parameterDefinition.GetBestGuess();

            var m = FundModel.GetModel();
            var res = m.Run(p);

            var c = new Clock(Timestep.FromYear(1951), Timestep.FromYear(2999));

            while (!c.IsLastTimestep)
            {
                var regions = res.Dimensions.GetValues<Region>();

                double totalLeave = (from r in regions select (double)res["ImpactSeaLevelRise", "leave"][c.Current, r]).Sum();

                double totalEnter = (from r in regions select (double)res["ImpactSeaLevelRise", "enter"][c.Current, r]).Sum();

                Assert.IsTrue(Math.Abs(totalLeave - totalEnter) < 1.0, "Migration doesn't even out");
                c.Advance();
            }
        }

    }
}
