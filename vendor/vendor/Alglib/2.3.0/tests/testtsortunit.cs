
using System;

namespace alglib
{
    public class testtsortunit
    {
        /*************************************************************************
        Testing tag sort
        *************************************************************************/
        public static bool testsort(bool silent)
        {
            bool result = new bool();
            bool waserrors = new bool();
            int n = 0;
            int i = 0;
            int pass = 0;
            int passcount = 0;
            int maxn = 0;
            double[] a = new double[0];
            double[] a0 = new double[0];
            double[] a2 = new double[0];
            int[] p1 = new int[0];
            int[] p2 = new int[0];

            waserrors = false;
            maxn = 100;
            passcount = 10;
            
            //
            // Test
            //
            for(n=1; n<=maxn; n++)
            {
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // (probably) distinct sort
                    //
                    unset1di(ref p1);
                    unset1di(ref p2);
                    a = new double[n-1+1];
                    a0 = new double[n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        a[i] = 2*AP.Math.RandomReal()-1;
                        a0[i] = a[i];
                    }
                    tsort.tagsort(ref a0, n, ref p1, ref p2);
                    testsortresults(ref a0, ref p1, ref p2, ref a, n, ref waserrors);
                    
                    //
                    // non-distinct sort
                    //
                    unset1di(ref p1);
                    unset1di(ref p2);
                    a = new double[n-1+1];
                    a0 = new double[n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        a[i] = i/2;
                        a0[i] = a[i];
                    }
                    tsort.tagsort(ref a0, n, ref p1, ref p2);
                    testsortresults(ref a0, ref p1, ref p2, ref a, n, ref waserrors);
                }
            }
            
            //
            // report
            //
            if( !silent )
            {
                System.Console.Write("TESTING TAGSORT");
                System.Console.WriteLine();
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
                System.Console.WriteLine();
                System.Console.WriteLine();
            }
            result = !waserrors;
            return result;
        }


        /*************************************************************************
        Unsets 2D array.
        *************************************************************************/
        private static void unset2d(ref AP.Complex[,] a)
        {
            a = new AP.Complex[0+1, 0+1];
            a[0,0] = 2*AP.Math.RandomReal()-1;
        }


        /*************************************************************************
        Unsets 1D array.
        *************************************************************************/
        private static void unset1d(ref double[] a)
        {
            a = new double[0+1];
            a[0] = 2*AP.Math.RandomReal()-1;
        }


        /*************************************************************************
        Unsets 1D array.
        *************************************************************************/
        private static void unset1di(ref int[] a)
        {
            a = new int[0+1];
            a[0] = AP.Math.RandomInteger(3)-1;
        }


        private static void testsortresults(ref double[] asorted,
            ref int[] p1,
            ref int[] p2,
            ref double[] aoriginal,
            int n,
            ref bool waserrors)
        {
            int i = 0;
            double[] a2 = new double[0];
            double t = 0;
            int[] f = new int[0];

            a2 = new double[n-1+1];
            f = new int[n-1+1];
            
            //
            // is set ordered?
            //
            for(i=0; i<=n-2; i++)
            {
                waserrors = waserrors | (double)(asorted[i])>(double)(asorted[i+1]);
            }
            
            //
            // P1 correctness
            //
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors | (double)(asorted[i])!=(double)(aoriginal[p1[i]]);
            }
            for(i=0; i<=n-1; i++)
            {
                f[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                f[p1[i]] = f[p1[i]]+1;
            }
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors | f[i]!=1;
            }
            
            //
            // P2 correctness
            //
            for(i=0; i<=n-1; i++)
            {
                a2[i] = aoriginal[i];
            }
            for(i=0; i<=n-1; i++)
            {
                if( p2[i]!=i )
                {
                    t = a2[i];
                    a2[i] = a2[p2[i]];
                    a2[p2[i]] = t;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors | (double)(asorted[i])!=(double)(a2[i]);
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testtsortunit_test_silent()
        {
            bool result = new bool();

            result = testsort(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testtsortunit_test()
        {
            bool result = new bool();

            result = testsort(false);
            return result;
        }
    }
}
