
using System;

namespace alglib
{
    public class testfhtunit
    {
        /*************************************************************************
        Test
        *************************************************************************/
        public static bool testfht(bool silent)
        {
            bool result = new bool();
            int n = 0;
            int i = 0;
            double[] r1 = new double[0];
            double[] r2 = new double[0];
            double[] r3 = new double[0];
            int maxn = 0;
            double bidierr = 0;
            double referr = 0;
            double errtol = 0;
            bool referrors = new bool();
            bool bidierrors = new bool();
            bool waserrors = new bool();

            maxn = 128;
            errtol = 100000*Math.Pow(maxn, (double)(3)/(double)(2))*AP.Math.MachineEpsilon;
            bidierrors = false;
            referrors = false;
            waserrors = false;
            
            //
            // Test bi-directional error: norm(x-invFHT(FHT(x)))
            //
            bidierr = 0;
            for(n=1; n<=maxn; n++)
            {
                
                //
                // FHT/invFHT
                //
                r1 = new double[n];
                r2 = new double[n];
                r3 = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    r1[i] = 2*AP.Math.RandomReal()-1;
                    r2[i] = r1[i];
                    r3[i] = r1[i];
                }
                fht.fhtr1d(ref r2, n);
                fht.fhtr1dinv(ref r2, n);
                fht.fhtr1dinv(ref r3, n);
                fht.fhtr1d(ref r3, n);
                for(i=0; i<=n-1; i++)
                {
                    bidierr = Math.Max(bidierr, Math.Abs(r1[i]-r2[i]));
                    bidierr = Math.Max(bidierr, Math.Abs(r1[i]-r3[i]));
                }
            }
            bidierrors = bidierrors | (double)(bidierr)>(double)(errtol);
            
            //
            // Test against reference O(N^2) implementation
            //
            referr = 0;
            for(n=1; n<=maxn; n++)
            {
                
                //
                // FHT
                //
                r1 = new double[n];
                r2 = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    r1[i] = 2*AP.Math.RandomReal()-1;
                    r2[i] = r1[i];
                }
                fht.fhtr1d(ref r1, n);
                reffhtr1d(ref r2, n);
                for(i=0; i<=n-1; i++)
                {
                    referr = Math.Max(referr, Math.Abs(r1[i]-r2[i]));
                }
                
                //
                // inverse FHT
                //
                r1 = new double[n];
                r2 = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    r1[i] = 2*AP.Math.RandomReal()-1;
                    r2[i] = r1[i];
                }
                fht.fhtr1dinv(ref r1, n);
                reffhtr1dinv(ref r2, n);
                for(i=0; i<=n-1; i++)
                {
                    referr = Math.Max(referr, Math.Abs(r1[i]-r2[i]));
                }
            }
            referrors = referrors | (double)(referr)>(double)(errtol);
            
            //
            // end
            //
            waserrors = bidierrors | referrors;
            if( !silent )
            {
                System.Console.Write("TESTING FHT");
                System.Console.WriteLine();
                System.Console.Write("FINAL RESULT:                             ");
                if( waserrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* BI-DIRECTIONAL TEST:                    ");
                if( bidierrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* AGAINST REFERENCE FHT:                  ");
                if( referrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                if( waserrors )
                {
                    System.Console.Write("TEST FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("TEST PASSED");
                    System.Console.WriteLine();
                }
            }
            result = !waserrors;
            return result;
        }


        /*************************************************************************
        Reference FHT
        *************************************************************************/
        private static void reffhtr1d(ref double[] a,
            int n)
        {
            double[] buf = new double[0];
            int i = 0;
            int j = 0;
            double v = 0;

            System.Diagnostics.Debug.Assert(n>0, "RefFHTR1D: incorrect N!");
            buf = new double[n];
            for(i=0; i<=n-1; i++)
            {
                v = 0;
                for(j=0; j<=n-1; j++)
                {
                    v = v+a[j]*(Math.Cos(2*Math.PI*i*j/n)+Math.Sin(2*Math.PI*i*j/n));
                }
                buf[i] = v;
            }
            for(i=0; i<=n-1; i++)
            {
                a[i] = buf[i];
            }
        }


        /*************************************************************************
        Reference inverse FHT
        *************************************************************************/
        private static void reffhtr1dinv(ref double[] a,
            int n)
        {
            int i = 0;

            System.Diagnostics.Debug.Assert(n>0, "RefFHTR1DInv: incorrect N!");
            reffhtr1d(ref a, n);
            for(i=0; i<=n-1; i++)
            {
                a[i] = a[i]/n;
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testfhtunit_test_silent()
        {
            bool result = new bool();

            result = testfht(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testfhtunit_test()
        {
            bool result = new bool();

            result = testfht(false);
            return result;
        }
    }
}
