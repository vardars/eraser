
using System;

namespace alglib
{
    public class testschurunit
    {
        /*************************************************************************
        Testing Schur decomposition subroutine
        *************************************************************************/
        public static bool testschur(bool silent)
        {
            bool result = new bool();
            double[,] a = new double[0,0];
            int n = 0;
            int maxn = 0;
            int i = 0;
            int j = 0;
            int pass = 0;
            int passcount = 0;
            bool waserrors = new bool();
            bool errstruct = new bool();
            bool wfailed = new bool();
            double materr = 0;
            double orterr = 0;
            double threshold = 0;

            materr = 0;
            orterr = 0;
            errstruct = false;
            wfailed = false;
            waserrors = false;
            maxn = 70;
            passcount = 1;
            threshold = 5*100*AP.Math.MachineEpsilon;
            a = new double[maxn-1+1, maxn-1+1];
            
            //
            // zero matrix, several cases
            //
            for(i=0; i<=maxn-1; i++)
            {
                for(j=0; j<=maxn-1; j++)
                {
                    a[i,j] = 0;
                }
            }
            for(n=1; n<=maxn; n++)
            {
                if( n>30 & n%2==0 )
                {
                    continue;
                }
                testschurproblem(ref a, n, ref materr, ref orterr, ref errstruct, ref wfailed);
            }
            
            //
            // Dense matrix
            //
            for(pass=1; pass<=passcount; pass++)
            {
                for(n=1; n<=maxn; n++)
                {
                    if( n>30 & n%2==0 )
                    {
                        continue;
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 2*AP.Math.RandomReal()-1;
                        }
                    }
                    testschurproblem(ref a, n, ref materr, ref orterr, ref errstruct, ref wfailed);
                }
            }
            
            //
            // Sparse matrices, very sparse matrices, incredible sparse matrices
            //
            for(pass=1; pass<=1; pass++)
            {
                for(n=1; n<=maxn; n++)
                {
                    if( n>30 & n%3!=0 )
                    {
                        continue;
                    }
                    fillsparsea(ref a, n, 0.8);
                    testschurproblem(ref a, n, ref materr, ref orterr, ref errstruct, ref wfailed);
                    fillsparsea(ref a, n, 0.9);
                    testschurproblem(ref a, n, ref materr, ref orterr, ref errstruct, ref wfailed);
                    fillsparsea(ref a, n, 0.95);
                    testschurproblem(ref a, n, ref materr, ref orterr, ref errstruct, ref wfailed);
                    fillsparsea(ref a, n, 0.997);
                    testschurproblem(ref a, n, ref materr, ref orterr, ref errstruct, ref wfailed);
                }
            }
            
            //
            // report
            //
            waserrors = (double)(materr)>(double)(threshold) | (double)(orterr)>(double)(threshold) | errstruct | wfailed;
            if( !silent )
            {
                System.Console.Write("TESTING SCHUR DECOMPOSITION");
                System.Console.WriteLine();
                System.Console.Write("Schur decomposition error:               ");
                System.Console.Write("{0,5:E3}",materr);
                System.Console.WriteLine();
                System.Console.Write("Schur orthogonality error:               ");
                System.Console.Write("{0,5:E3}",orterr);
                System.Console.WriteLine();
                System.Console.Write("T matrix structure:                      ");
                if( !errstruct )
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
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
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


        private static void fillsparsea(ref double[,] a,
            int n,
            double sparcity)
        {
            int i = 0;
            int j = 0;

            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( (double)(AP.Math.RandomReal())>=(double)(sparcity) )
                    {
                        a[i,j] = 2*AP.Math.RandomReal()-1;
                    }
                    else
                    {
                        a[i,j] = 0;
                    }
                }
            }
        }


        private static void testschurproblem(ref double[,] a,
            int n,
            ref double materr,
            ref double orterr,
            ref bool errstruct,
            ref bool wfailed)
        {
            double[,] s = new double[0,0];
            double[,] t = new double[0,0];
            double[] sr = new double[0];
            double[] astc = new double[0];
            double[] sastc = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            double locerr = 0;
            int i_ = 0;

            sr = new double[n-1+1];
            astc = new double[n-1+1];
            sastc = new double[n-1+1];
            
            //
            // Schur decomposition, convergence test
            //
            t = new double[n-1+1, n-1+1];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    t[i,j] = a[i,j];
                }
            }
            if( !schur.rmatrixschur(ref t, n, ref s) )
            {
                wfailed = true;
                return;
            }
            
            //
            // decomposition error
            //
            locerr = 0;
            for(j=0; j<=n-1; j++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    sr[i_] = s[j,i_];
                }
                for(k=0; k<=n-1; k++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += t[k,i_]*sr[i_];
                    }
                    astc[k] = v;
                }
                for(k=0; k<=n-1; k++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += s[k,i_]*astc[i_];
                    }
                    sastc[k] = v;
                }
                for(k=0; k<=n-1; k++)
                {
                    locerr = Math.Max(locerr, Math.Abs(sastc[k]-a[k,j]));
                }
            }
            materr = Math.Max(materr, locerr);
            
            //
            // orthogonality error
            //
            locerr = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += s[i_,i]*s[i_,j];
                    }
                    if( i!=j )
                    {
                        locerr = Math.Max(locerr, Math.Abs(v));
                    }
                    else
                    {
                        locerr = Math.Max(locerr, Math.Abs(v-1));
                    }
                }
            }
            orterr = Math.Max(orterr, locerr);
            
            //
            // T matrix structure
            //
            for(j=0; j<=n-1; j++)
            {
                for(i=j+2; i<=n-1; i++)
                {
                    if( (double)(t[i,j])!=(double)(0) )
                    {
                        errstruct = true;
                    }
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testschurunit_test_silent()
        {
            bool result = new bool();

            result = testschur(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testschurunit_test()
        {
            bool result = new bool();

            result = testschur(false);
            return result;
        }
    }
}
