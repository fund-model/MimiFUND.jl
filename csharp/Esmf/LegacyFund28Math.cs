// FUND - Climate Framework for Uncertainty, Negotiation and Distribution
// Copyright (C) 2012 David Anthoff and Richard S.J. Tol
// http://www.fund-model.org
// Licensed under the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fund
{
    public static class LegacyFund28Math
    {
        public static void GenNormal(double mu, double sigma, out double a, out double b, Random rand)
        {
            double v1, v2, rsq, fac;

            do
            {
                v1 = 2.0 * rand.NextDouble() - 1.0;
                v2 = 2.0 * rand.NextDouble() - 1.0;
                rsq = v1 * v1 + v2 * v2;
            }
            while (rsq > 1 || rsq == 0.0);

            fac = Math.Sqrt(-2.0 * Math.Log(rsq) / rsq);
            a = mu + sigma * fac * v1;
            b = mu + sigma * fac * v2;
        }

        internal static void GenTriang(double a, double b, double c, out double x, Random rand)
        {
            double d, u, h;


            d = (c - a) / (b - a);
            u = rand.NextDouble();
            if (u <= d)
                h = Math.Sqrt(d * u);
            else
                h = 1 - Math.Sqrt((1 - d) * (1 - u));

            x = a + (b - a) * h;
        }

        internal static void GenExponential(double lambda, out double a, Random rand)
        {
            a = -lambda * Math.Log(rand.NextDouble());
        }

        internal static void GenGamma(double alpha, double beta, out double a, Random rand)
        {
            if (alpha < 1)
            {
                LegacyFund28Math.GamLTOne(alpha, out a, rand);
                a = beta * a;
            }
            else if (alpha == 1.0)
            {
                LegacyFund28Math.GenExponential(1.0, out a, rand);
                a = beta * a;
            }
            else
            {
                GamGTOne(alpha, out a, rand);
                a = beta * a;
            }
        }

        private static void GamLTOne(double alpha, out double a, Random rand)
        {
            double e, b, p, y;

            e = Math.Exp(1.0);
            b = (e + alpha) / e;
            p = b * rand.NextDouble();
            if (p < 1.0)
            {
                y = Math.Exp(Math.Log(p) / alpha);
                if (Math.Exp(-y) >= rand.NextDouble())
                    a = y;
                else
                    GamLTOne(alpha, out a, rand);
            }
            else
            {
                y = -Math.Log((b - p) / alpha);
                if (Math.Exp((alpha - 1.0) * Math.Log(y)) >= rand.NextDouble())
                    a = y;
                else
                    GamLTOne(alpha, out a, rand);
            }
        }

        private static void GamGTOne(double alpha, out double a, Random rand)
        {
            double ha, b, q, th, d, u1, u2, v, y, z, w;

            ha = 1.0 / Math.Sqrt(2.0 * alpha - 1.0);
            b = alpha - Math.Log(4.0);
            q = alpha + 1.0 / ha;
            th = 4.5;
            d = 1.0 + Math.Log(th);
            u1 = rand.NextDouble();
            u2 = rand.NextDouble();
            v = ha * Math.Log(u1 / (1.0 - u1));
            y = alpha * Math.Exp(v);
            z = u1 * u1 * u2;
            w = b + q * v - y;
            if ((w + d - th * z) >= 0.0)
                a = y;
            else if (w >= Math.Log(z))
                a = y;
            else
                GamGTOne(alpha, out a, rand);
        }
    }
}