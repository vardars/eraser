
using System;

namespace alglib
{
    public class testsevdunit
    {
        /*************************************************************************
        Testing symmetric EVD subroutine
        *************************************************************************/
        public static bool testsevd(bool silent)
        {
            bool result = new bool();
            double[,] a = new double[0,0];
            double[,] al = new double[0,0];
            double[,] au = new double[0,0];
            double[,] z = new double[0,0];
            int pass = 0;
            int n = 0;
            int i = 0;
            int j = 0;
            int mkind = 0;
            int passcount = 0;
            int maxn = 0;
            double materr = 0;
            double valerr = 0;
            double orterr = 0;
            bool wnsorted = new bool();
            int failc = 0;
            int runs = 0;
            double failr = 0;
            double failthreshold = 0;
            double threshold = 0;
            bool waserrors = new bool();
            bool wfailed = new bool();

            failthreshold = 0.005;
            threshold = 1000*AP.Math.MachineEpsilon;
            materr = 0;
            valerr = 0;
            orterr = 0;
            wnsorted = false;
            wfailed = false;
            failc = 0;
            runs = 0;
            maxn = 20;
            passcount = 10;
            
            //
            // Main cycle
            //
            for(n=1; n<=maxn; n++)
            {
                
                //
                // Prepare
                //
                a = new double[n-1+1, n-1+1];
                al = new double[n-1+1, n-1+1];
                au = new double[n-1+1, n-1+1];
                for(i=0; i<=n-1; i++)
                {
                    for(j=i+1; j<=n-1; j++)
                    {
                        
                        //
                        // A
                        //
                        a[i,j] = 2*AP.Math.RandomReal()-1;
                        a[j,i] = a[i,j];
                        
                        //
                        // A lower
                        //
                        al[i,j] = 2*AP.Math.RandomReal()-1;
                        al[j,i] = a[i,j];
                        
                        //
                        // A upper
                        //
                        au[i,j] = a[i,j];
                        au[j,i] = 2*AP.Math.RandomReal()-1;
                    }
                    a[i,i] = 2*AP.Math.RandomReal()-1;
                    al[i,i] = a[i,i];
                    au[i,i] = a[i,i];
                }
                
                //
                // Test
                //
                testevdproblem(ref a, ref al, ref au, n, ref materr, ref valerr, ref orterr, ref wnsorted, ref failc);
                runs = runs+1;
            }
            
            //
            // report
            //
            failr = (double)(failc)/(double)(runs);
            wfailed = (double)(failr)>(double)(failthreshold);
            waserrors = (double)(materr)>(double)(threshold) | (double)(valerr)>(double)(threshold) | (double)(orterr)>(double)(threshold) | wnsorted | wfailed;
            if( !silent )
            {
                System.Console.Write("TESTING SYMMETRIC EVD");
                System.Console.WriteLine();
                System.Console.Write("EVD matrix error:                        ");
                System.Console.Write("{0,5:E3}",materr);
                System.Console.WriteLine();
                System.Console.Write("EVD values error (different variants):   ");
                System.Console.Write("{0,5:E3}",valerr);
                System.Console.WriteLine();
                System.Console.Write("EVD orthogonality error:                 ");
                System.Console.Write("{0,5:E3}",orterr);
                System.Console.WriteLine();
                System.Console.Write("Eigen values order:                      ");
                if( !wnsorted )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("Always converged:                        ");
                if( !wfailed )
                {
                    System.Console.Write("YES");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("NO");
                    System.Console.WriteLine();
                    System.Console.Write("Fail ratio:                              ");
                    System.Console.Write("{0,5:F3}",failr);
                    System.Console.WriteLine();
                }
                System.Console.Write("Threshold:                               ");
                System.Console.Write("{0,5:E3}",threshold);
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
        private static void unset2d(ref double[,] a)
        {
            a = new double[0+1, 0+1];
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
        Tests Z*Lambda*Z' against tridiag(D,E).
        Returns relative error.
        *************************************************************************/
        private static double testproduct(ref double[,] a,
            int n,
            ref double[,] z,
            ref double[] lambda)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            double mx = 0;

            result = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    
                    //
                    // Calculate V = A[i,j], A = Z*Lambda*Z'
                    //
                    v = 0;
                    for(k=0; k<=n-1; k++)
                    {
                        v = v+z[i,k]*lambda[k]*z[j,k];
                    }
                    
                    //
                    // Compare
                    //
                    result = Math.Max(result, Math.Abs(v-a[i,j]));
                }
            }
            mx = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    mx = Math.Max(mx, Math.Abs(a[i,j]));
                }
            }
            if( (double)(mx)==(double)(0) )
            {
                mx = 1;
            }
            result = result/mx;
            return result;
        }


        /*************************************************************************
        Tests Z*Z' against diag(1...1)
        Returns absolute error.
        *************************************************************************/
        private static double testort(ref double[,] z,
            int n)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            int i_ = 0;

            result = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += z[i_,i]*z[i_,j];
                    }
                    if( i==j )
                    {
                        v = v-1;
                    }
                    result = Math.Max(result, Math.Abs(v));
                }
            }
            return result;
        }


        /*************************************************************************
        Tests EVD problem
        *************************************************************************/
        private static void testevdproblem(ref double[,] a,
            ref double[,] al,
            ref double[,] au,
            int n,
            ref double materr,
            ref double valerr,
            ref double orterr,
            ref bool wnsorted,
            ref int failc)
        {
            double[] lambda = new double[0];
            double[] lambdaref = new double[0];
            double[,] z = new double[0,0];
            bool wsucc = new bool();
            int i = 0;
            int j = 0;
            double v = 0;

            
            //
            // Test simple EVD: values and full vectors, lower A
            //
            unset1d(ref lambdaref);
            unset2d(ref z);
            wsucc = sevd.smatrixevd(al, n, 1, false, ref lambdaref, ref z);
            if( !wsucc )
            {
                failc = failc+1;
                return;
            }
            materr = Math.Max(materr, testproduct(ref a, n, ref z, ref lambdaref));
            orterr = Math.Max(orterr, testort(ref z, n));
            for(i=0; i<=n-2; i++)
            {
                if( (double)(lambdaref[i+1])<(double)(lambdaref[i]) )
                {
                    wnsorted = true;
                }
            }
            
            //
            // Test simple EVD: values and full vectors, upper A
            //
            unset1d(ref lambda);
            unset2d(ref z);
            wsucc = sevd.smatrixevd(au, n, 1, true, ref lambda, ref z);
            if( !wsucc )
            {
                failc = failc+1;
                return;
            }
            materr = Math.Max(materr, testproduct(ref a, n, ref z, ref lambda));
            orterr = Math.Max(orterr, testort(ref z, n));
            for(i=0; i<=n-2; i++)
            {
                if( (double)(lambda[i+1])<(double)(lambda[i]) )
                {
                    wnsorted = true;
                }
            }
            
            //
            // Test simple EVD: values only, lower A
            //
            unset1d(ref lambda);
            unset2d(ref z);
            wsucc = sevd.smatrixevd(al, n, 0, false, ref lambda, ref z);
            if( !wsucc )
            {
                failc = failc+1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                valerr = Math.Max(valerr, Math.Abs(lambda[i]-lambdaref[i]));
            }
            
            //
            // Test simple EVD: values only, upper A
            //
            unset1d(ref lambda);
            unset2d(ref z);
            wsucc = sevd.smatrixevd(au, n, 0, true, ref lambda, ref z);
            if( !wsucc )
            {
                failc = failc+1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                valerr = Math.Max(valerr, Math.Abs(lambda[i]-lambdaref[i]));
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testsevdunit_test_silent()
        {
            bool result = new bool();

            result = testsevd(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testsevdunit_test()
        {
            bool result = new bool();

            result = testsevd(false);
            return result;
        }
    }
}
