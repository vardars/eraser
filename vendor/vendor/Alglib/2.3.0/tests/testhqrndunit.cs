
using System;

namespace alglib
{
    public class testhqrndunit
    {
        public static void calculatemv(ref double[] x,
            int n,
            ref double mean,
            ref double means,
            ref double stddev,
            ref double stddevs)
        {
            int i = 0;
            double v = 0;
            double v1 = 0;
            double v2 = 0;
            double variance = 0;

            mean = 0;
            means = 1;
            stddev = 0;
            stddevs = 1;
            variance = 0;
            if( n<=1 )
            {
                return;
            }
            
            //
            // Mean
            //
            for(i=0; i<=n-1; i++)
            {
                mean = mean+x[i];
            }
            mean = mean/n;
            
            //
            // Variance (using corrected two-pass algorithm)
            //
            if( n!=1 )
            {
                v1 = 0;
                for(i=0; i<=n-1; i++)
                {
                    v1 = v1+AP.Math.Sqr(x[i]-mean);
                }
                v2 = 0;
                for(i=0; i<=n-1; i++)
                {
                    v2 = v2+(x[i]-mean);
                }
                v2 = AP.Math.Sqr(v2)/n;
                variance = (v1-v2)/(n-1);
                if( (double)(variance)<(double)(0) )
                {
                    variance = 0;
                }
                stddev = Math.Sqrt(variance);
            }
            
            //
            // Errors
            //
            means = stddev/Math.Sqrt(n);
            stddevs = stddev*Math.Sqrt(2)/Math.Sqrt(n-1);
        }


        public static bool testhqrnd(bool silent)
        {
            bool result = new bool();
            bool waserrors = new bool();
            int samplesize = 0;
            double sigmathreshold = 0;
            int passcount = 0;
            int n = 0;
            int i = 0;
            int pass = 0;
            int s1 = 0;
            int s2 = 0;
            int i1 = 0;
            int i2 = 0;
            double r1 = 0;
            double r2 = 0;
            double[] x = new double[0];
            double mean = 0;
            double means = 0;
            double stddev = 0;
            double stddevs = 0;
            double lambda = 0;
            bool seederrors = new bool();
            bool urerrors = new bool();
            double ursigmaerr = 0;
            bool uierrors = new bool();
            double uisigmaerr = 0;
            bool normerrors = new bool();
            double normsigmaerr = 0;
            bool experrors = new bool();
            double expsigmaerr = 0;
            hqrnd.hqrndstate state = new hqrnd.hqrndstate();

            waserrors = false;
            sigmathreshold = 7;
            samplesize = 100000;
            passcount = 50;
            x = new double[samplesize-1+1];
            
            //
            // Test seed errors
            //
            seederrors = false;
            for(pass=1; pass<=passcount; pass++)
            {
                s1 = 1+AP.Math.RandomInteger(32000);
                s2 = 1+AP.Math.RandomInteger(32000);
                unsetstate(ref state);
                hqrnd.hqrndseed(s1, s2, ref state);
                i1 = hqrnd.hqrnduniformi(100, ref state);
                unsetstate(ref state);
                hqrnd.hqrndseed(s1, s2, ref state);
                i2 = hqrnd.hqrnduniformi(100, ref state);
                seederrors = seederrors | i1!=i2;
                unsetstate(ref state);
                hqrnd.hqrndseed(s1, s2, ref state);
                r1 = hqrnd.hqrnduniformr(ref state);
                unsetstate(ref state);
                hqrnd.hqrndseed(s1, s2, ref state);
                r2 = hqrnd.hqrnduniformr(ref state);
                seederrors = seederrors | (double)(r1)!=(double)(r2);
            }
            
            //
            // Test HQRNDRandomize() and real uniform generator
            //
            unsetstate(ref state);
            hqrnd.hqrndrandomize(ref state);
            urerrors = false;
            ursigmaerr = 0;
            for(i=0; i<=samplesize-1; i++)
            {
                x[i] = hqrnd.hqrnduniformr(ref state);
            }
            for(i=0; i<=samplesize-1; i++)
            {
                urerrors = urerrors | (double)(x[i])<=(double)(0) | (double)(x[i])>=(double)(1);
            }
            calculatemv(ref x, samplesize, ref mean, ref means, ref stddev, ref stddevs);
            if( (double)(means)!=(double)(0) )
            {
                ursigmaerr = Math.Max(ursigmaerr, Math.Abs((mean-0.5)/means));
            }
            else
            {
                urerrors = true;
            }
            if( (double)(stddevs)!=(double)(0) )
            {
                ursigmaerr = Math.Max(ursigmaerr, Math.Abs((stddev-Math.Sqrt((double)(1)/(double)(12)))/stddevs));
            }
            else
            {
                urerrors = true;
            }
            urerrors = urerrors | (double)(ursigmaerr)>(double)(sigmathreshold);
            
            //
            // Test HQRNDRandomize() and integer uniform
            //
            unsetstate(ref state);
            hqrnd.hqrndrandomize(ref state);
            uierrors = false;
            uisigmaerr = 0;
            for(n=2; n<=10; n++)
            {
                for(i=0; i<=samplesize-1; i++)
                {
                    x[i] = hqrnd.hqrnduniformi(n, ref state);
                }
                for(i=0; i<=samplesize-1; i++)
                {
                    uierrors = uierrors | (double)(x[i])<(double)(0) | (double)(x[i])>=(double)(n);
                }
                calculatemv(ref x, samplesize, ref mean, ref means, ref stddev, ref stddevs);
                if( (double)(means)!=(double)(0) )
                {
                    uisigmaerr = Math.Max(uisigmaerr, Math.Abs((mean-0.5*(n-1))/means));
                }
                else
                {
                    uierrors = true;
                }
                if( (double)(stddevs)!=(double)(0) )
                {
                    uisigmaerr = Math.Max(uisigmaerr, Math.Abs((stddev-Math.Sqrt((AP.Math.Sqr(n)-1)/12))/stddevs));
                }
                else
                {
                    uierrors = true;
                }
            }
            uierrors = uierrors | (double)(uisigmaerr)>(double)(sigmathreshold);
            
            //
            // Special 'close-to-limit' test on uniformity of integers
            // (straightforward implementation like 'RND mod N' will return
            //  non-uniform numbers for N=2/3*LIMIT)
            //
            unsetstate(ref state);
            hqrnd.hqrndrandomize(ref state);
            uierrors = false;
            uisigmaerr = 0;
            n = (int)Math.Round(2.0/3.0*2147483563.0);
            for(i=0; i<=samplesize-1; i++)
            {
                x[i] = hqrnd.hqrnduniformi(n, ref state);
            }
            for(i=0; i<=samplesize-1; i++)
            {
                uierrors = uierrors | (double)(x[i])<(double)(0) | (double)(x[i])>=(double)(n);
            }
            calculatemv(ref x, samplesize, ref mean, ref means, ref stddev, ref stddevs);
            if( (double)(means)!=(double)(0) )
            {
                uisigmaerr = Math.Max(uisigmaerr, Math.Abs((mean-0.5*(n-1))/means));
            }
            else
            {
                uierrors = true;
            }
            if( (double)(stddevs)!=(double)(0) )
            {
                uisigmaerr = Math.Max(uisigmaerr, Math.Abs((stddev-Math.Sqrt((AP.Math.Sqr(n)-1)/12))/stddevs));
            }
            else
            {
                uierrors = true;
            }
            uierrors = uierrors | (double)(uisigmaerr)>(double)(sigmathreshold);
            
            //
            // Test normal
            //
            unsetstate(ref state);
            hqrnd.hqrndrandomize(ref state);
            normerrors = false;
            normsigmaerr = 0;
            i = 0;
            while( i<samplesize )
            {
                hqrnd.hqrndnormal2(ref state, ref r1, ref r2);
                x[i] = r1;
                if( i+1<samplesize )
                {
                    x[i+1] = r2;
                }
                i = i+2;
            }
            calculatemv(ref x, samplesize, ref mean, ref means, ref stddev, ref stddevs);
            if( (double)(means)!=(double)(0) )
            {
                normsigmaerr = Math.Max(normsigmaerr, Math.Abs((mean-0)/means));
            }
            else
            {
                normerrors = true;
            }
            if( (double)(stddevs)!=(double)(0) )
            {
                normsigmaerr = Math.Max(normsigmaerr, Math.Abs((stddev-1)/stddevs));
            }
            else
            {
                normerrors = true;
            }
            normerrors = normerrors | (double)(normsigmaerr)>(double)(sigmathreshold);
            
            //
            // Test exponential
            //
            unsetstate(ref state);
            hqrnd.hqrndrandomize(ref state);
            experrors = false;
            expsigmaerr = 0;
            lambda = 2+5*AP.Math.RandomReal();
            for(i=0; i<=samplesize-1; i++)
            {
                x[i] = hqrnd.hqrndexponential(lambda, ref state);
            }
            for(i=0; i<=samplesize-1; i++)
            {
                uierrors = uierrors | (double)(x[i])<(double)(0);
            }
            calculatemv(ref x, samplesize, ref mean, ref means, ref stddev, ref stddevs);
            if( (double)(means)!=(double)(0) )
            {
                expsigmaerr = Math.Max(expsigmaerr, Math.Abs((mean-1.0/lambda)/means));
            }
            else
            {
                experrors = true;
            }
            if( (double)(stddevs)!=(double)(0) )
            {
                expsigmaerr = Math.Max(expsigmaerr, Math.Abs((stddev-1.0/lambda)/stddevs));
            }
            else
            {
                experrors = true;
            }
            experrors = experrors | (double)(expsigmaerr)>(double)(sigmathreshold);
            
            //
            // Final report
            //
            waserrors = seederrors | urerrors | uierrors | normerrors | experrors;
            if( !silent )
            {
                System.Console.Write("RNG TEST");
                System.Console.WriteLine();
                System.Console.Write("SEED TEST:                               ");
                if( !seederrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("UNIFORM CONTINUOUS:                      ");
                if( !urerrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("UNIFORM INTEGER:                         ");
                if( !uierrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("NORMAL:                                  ");
                if( !normerrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("EXPONENTIAL:                             ");
                if( !experrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                if( waserrors )
                {
                    System.Console.Write("TEST SUMMARY: FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("TEST SUMMARY: PASSED");
                    System.Console.WriteLine();
                }
                System.Console.WriteLine();
                System.Console.WriteLine();
            }
            result = !waserrors;
            return result;
        }


        /*************************************************************************
        Unsets HQRNDState structure
        *************************************************************************/
        private static void unsetstate(ref hqrnd.hqrndstate state)
        {
            state.s1 = 0;
            state.s2 = 0;
            state.v = 0;
            state.magicv = 0;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testhqrndunit_test_silent()
        {
            bool result = new bool();

            result = testhqrnd(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testhqrndunit_test()
        {
            bool result = new bool();

            result = testhqrnd(false);
            return result;
        }
    }
}
