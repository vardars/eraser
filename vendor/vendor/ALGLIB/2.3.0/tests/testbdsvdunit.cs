
using System;

namespace alglib
{
    public class testbdsvdunit
    {
        public static int failcount = 0;
        public static int succcount = 0;


        /*************************************************************************
        Testing bidiagonal SVD decomposition subroutine
        *************************************************************************/
        public static bool testbdsvd(bool silent)
        {
            bool result = new bool();
            double[] d = new double[0];
            double[] e = new double[0];
            double[,] mempty = new double[0,0];
            int n = 0;
            int maxn = 0;
            int i = 0;
            int j = 0;
            int gpass = 0;
            int pass = 0;
            bool waserrors = new bool();
            bool wsorted = new bool();
            bool wfailed = new bool();
            bool failcase = new bool();
            double materr = 0;
            double orterr = 0;
            double threshold = 0;
            double failthreshold = 0;
            double failr = 0;

            failcount = 0;
            succcount = 0;
            materr = 0;
            orterr = 0;
            wsorted = true;
            wfailed = false;
            waserrors = false;
            maxn = 15;
            threshold = 5*100*AP.Math.MachineEpsilon;
            failthreshold = 1.0E-2;
            d = new double[maxn-1+1];
            e = new double[maxn-2+1];
            
            //
            // special case: fail matrix
            //
            n = 5;
            d[0] = -8.27448347422711894000e-01;
            d[1] = -8.16705832087160854600e-01;
            d[2] = -2.53974358904729382800e-17;
            d[3] = -1.24626684881972815700e+00;
            d[4] = -4.64744131545637651000e-01;
            e[0] = -3.25785088656270038800e-01;
            e[1] = -1.03732413708914436580e-01;
            e[2] = -9.57365642262031357700e-02;
            e[3] = -2.71564153973817390400e-01;
            failcase = bdsvd.rmatrixbdsvd(ref d, e, n, true, false, ref mempty, 0, ref mempty, 0, ref mempty, 0);
            
            //
            // special case: zero divide matrix
            // unfixed LAPACK routine should fail on this problem
            //
            n = 7;
            d[0] = -6.96462904751731892700e-01;
            d[1] = 0.00000000000000000000e+00;
            d[2] = -5.73827770385971991400e-01;
            d[3] = -6.62562624399371191700e-01;
            d[4] = 5.82737148001782223600e-01;
            d[5] = 3.84825263580925003300e-01;
            d[6] = 9.84087420830525472200e-01;
            e[0] = -7.30307931760612871800e-02;
            e[1] = -2.30079042939542843800e-01;
            e[2] = -6.87824621739351216300e-01;
            e[3] = -1.77306437707837570600e-02;
            e[4] = 1.78285126526551632000e-15;
            e[5] = -4.89434737751289969400e-02;
            bdsvd.rmatrixbdsvd(ref d, e, n, true, false, ref mempty, 0, ref mempty, 0, ref mempty, 0);
            
            //
            // zero matrix, several cases
            //
            for(i=0; i<=maxn-1; i++)
            {
                d[i] = 0;
            }
            for(i=0; i<=maxn-2; i++)
            {
                e[i] = 0;
            }
            for(n=1; n<=maxn; n++)
            {
                testbdsvdproblem(ref d, ref e, n, ref materr, ref orterr, ref wsorted, ref wfailed);
            }
            
            //
            // Dense matrix
            //
            for(n=1; n<=maxn; n++)
            {
                for(pass=1; pass<=10; pass++)
                {
                    for(i=0; i<=maxn-1; i++)
                    {
                        d[i] = 2*AP.Math.RandomReal()-1;
                    }
                    for(i=0; i<=maxn-2; i++)
                    {
                        e[i] = 2*AP.Math.RandomReal()-1;
                    }
                    testbdsvdproblem(ref d, ref e, n, ref materr, ref orterr, ref wsorted, ref wfailed);
                }
            }
            
            //
            // Sparse matrices, very sparse matrices, incredible sparse matrices
            //
            for(n=1; n<=maxn; n++)
            {
                for(pass=1; pass<=10; pass++)
                {
                    fillsparsede(ref d, ref e, n, 0.5);
                    testbdsvdproblem(ref d, ref e, n, ref materr, ref orterr, ref wsorted, ref wfailed);
                    fillsparsede(ref d, ref e, n, 0.8);
                    testbdsvdproblem(ref d, ref e, n, ref materr, ref orterr, ref wsorted, ref wfailed);
                    fillsparsede(ref d, ref e, n, 0.9);
                    testbdsvdproblem(ref d, ref e, n, ref materr, ref orterr, ref wsorted, ref wfailed);
                    fillsparsede(ref d, ref e, n, 0.95);
                    testbdsvdproblem(ref d, ref e, n, ref materr, ref orterr, ref wsorted, ref wfailed);
                }
            }
            
            //
            // report
            //
            failr = (double)(failcount)/((double)(succcount+failcount));
            waserrors = (double)(materr)>(double)(threshold) | (double)(orterr)>(double)(threshold) | !wsorted | (double)(failr)>(double)(failthreshold);
            if( !silent )
            {
                System.Console.Write("TESTING BIDIAGONAL SVD DECOMPOSITION");
                System.Console.WriteLine();
                System.Console.Write("SVD decomposition error:                 ");
                System.Console.Write("{0,5:E3}",materr);
                System.Console.WriteLine();
                System.Console.Write("SVD orthogonality error:                 ");
                System.Console.Write("{0,5:E3}",orterr);
                System.Console.WriteLine();
                System.Console.Write("Singular values order:                   ");
                if( wsorted )
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
                System.Console.Write("Fail matrix test:                        ");
                if( !failcase )
                {
                    System.Console.Write("AS EXPECTED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("CONVERGED (UNEXPECTED)");
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


        private static void fillidentity(ref double[,] a,
            int n)
        {
            int i = 0;
            int j = 0;

            a = new double[n-1+1, n-1+1];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( i==j )
                    {
                        a[i,j] = 1;
                    }
                    else
                    {
                        a[i,j] = 0;
                    }
                }
            }
        }


        private static void fillsparsede(ref double[] d,
            ref double[] e,
            int n,
            double sparcity)
        {
            int i = 0;
            int j = 0;

            d = new double[n-1+1];
            e = new double[Math.Max(0, n-2)+1];
            for(i=0; i<=n-1; i++)
            {
                if( (double)(AP.Math.RandomReal())>=(double)(sparcity) )
                {
                    d[i] = 2*AP.Math.RandomReal()-1;
                }
                else
                {
                    d[i] = 0;
                }
            }
            for(i=0; i<=n-2; i++)
            {
                if( (double)(AP.Math.RandomReal())>=(double)(sparcity) )
                {
                    e[i] = 2*AP.Math.RandomReal()-1;
                }
                else
                {
                    e[i] = 0;
                }
            }
        }


        private static void getbdsvderror(ref double[] d,
            ref double[] e,
            int n,
            bool isupper,
            ref double[,] u,
            ref double[,] c,
            ref double[] w,
            ref double[,] vt,
            ref double materr,
            ref double orterr,
            ref bool wsorted)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            double locerr = 0;
            double sm = 0;
            int i_ = 0;

            
            //
            // decomposition error
            //
            locerr = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    sm = 0;
                    for(k=0; k<=n-1; k++)
                    {
                        sm = sm+w[k]*u[i,k]*vt[k,j];
                    }
                    if( isupper )
                    {
                        if( i==j )
                        {
                            locerr = Math.Max(locerr, Math.Abs(d[i]-sm));
                        }
                        else
                        {
                            if( i==j-1 )
                            {
                                locerr = Math.Max(locerr, Math.Abs(e[i]-sm));
                            }
                            else
                            {
                                locerr = Math.Max(locerr, Math.Abs(sm));
                            }
                        }
                    }
                    else
                    {
                        if( i==j )
                        {
                            locerr = Math.Max(locerr, Math.Abs(d[i]-sm));
                        }
                        else
                        {
                            if( i-1==j )
                            {
                                locerr = Math.Max(locerr, Math.Abs(e[j]-sm));
                            }
                            else
                            {
                                locerr = Math.Max(locerr, Math.Abs(sm));
                            }
                        }
                    }
                }
            }
            materr = Math.Max(materr, locerr);
            
            //
            // check for C = U'
            // we consider it as decomposition error
            //
            locerr = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    locerr = Math.Max(locerr, Math.Abs(u[i,j]-c[j,i]));
                }
            }
            materr = Math.Max(materr, locerr);
            
            //
            // orthogonality error
            //
            locerr = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=i; j<=n-1; j++)
                {
                    sm = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        sm += u[i_,i]*u[i_,j];
                    }
                    if( i!=j )
                    {
                        locerr = Math.Max(locerr, Math.Abs(sm));
                    }
                    else
                    {
                        locerr = Math.Max(locerr, Math.Abs(sm-1));
                    }
                    sm = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        sm += vt[i,i_]*vt[j,i_];
                    }
                    if( i!=j )
                    {
                        locerr = Math.Max(locerr, Math.Abs(sm));
                    }
                    else
                    {
                        locerr = Math.Max(locerr, Math.Abs(sm-1));
                    }
                }
            }
            orterr = Math.Max(orterr, locerr);
            
            //
            // values order error
            //
            for(i=1; i<=n-1; i++)
            {
                if( (double)(w[i])>(double)(w[i-1]) )
                {
                    wsorted = false;
                }
            }
        }


        private static void testbdsvdproblem(ref double[] d,
            ref double[] e,
            int n,
            ref double materr,
            ref double orterr,
            ref bool wsorted,
            ref bool wfailed)
        {
            double[,] u = new double[0,0];
            double[,] vt = new double[0,0];
            double[,] c = new double[0,0];
            double[] w = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            double mx = 0;

            mx = 0;
            for(i=0; i<=n-1; i++)
            {
                if( (double)(Math.Abs(d[i]))>(double)(mx) )
                {
                    mx = Math.Abs(d[i]);
                }
            }
            for(i=0; i<=n-2; i++)
            {
                if( (double)(Math.Abs(e[i]))>(double)(mx) )
                {
                    mx = Math.Abs(e[i]);
                }
            }
            if( (double)(mx)==(double)(0) )
            {
                mx = 1;
            }
            
            //
            // Upper BDSVD tests
            //
            w = new double[n-1+1];
            fillidentity(ref u, n);
            fillidentity(ref vt, n);
            fillidentity(ref c, n);
            for(i=0; i<=n-1; i++)
            {
                w[i] = d[i];
            }
            if( !bdsvd.rmatrixbdsvd(ref w, e, n, true, false, ref u, n, ref c, n, ref vt, n) )
            {
                failcount = failcount+1;
                wfailed = true;
                return;
            }
            getbdsvderror(ref d, ref e, n, true, ref u, ref c, ref w, ref vt, ref materr, ref orterr, ref wsorted);
            fillidentity(ref u, n);
            fillidentity(ref vt, n);
            fillidentity(ref c, n);
            for(i=0; i<=n-1; i++)
            {
                w[i] = d[i];
            }
            if( !bdsvd.rmatrixbdsvd(ref w, e, n, true, true, ref u, n, ref c, n, ref vt, n) )
            {
                failcount = failcount+1;
                wfailed = true;
                return;
            }
            getbdsvderror(ref d, ref e, n, true, ref u, ref c, ref w, ref vt, ref materr, ref orterr, ref wsorted);
            
            //
            // Lower BDSVD tests
            //
            w = new double[n-1+1];
            fillidentity(ref u, n);
            fillidentity(ref vt, n);
            fillidentity(ref c, n);
            for(i=0; i<=n-1; i++)
            {
                w[i] = d[i];
            }
            if( !bdsvd.rmatrixbdsvd(ref w, e, n, false, false, ref u, n, ref c, n, ref vt, n) )
            {
                failcount = failcount+1;
                wfailed = true;
                return;
            }
            getbdsvderror(ref d, ref e, n, false, ref u, ref c, ref w, ref vt, ref materr, ref orterr, ref wsorted);
            fillidentity(ref u, n);
            fillidentity(ref vt, n);
            fillidentity(ref c, n);
            for(i=0; i<=n-1; i++)
            {
                w[i] = d[i];
            }
            if( !bdsvd.rmatrixbdsvd(ref w, e, n, false, true, ref u, n, ref c, n, ref vt, n) )
            {
                failcount = failcount+1;
                wfailed = true;
                return;
            }
            getbdsvderror(ref d, ref e, n, false, ref u, ref c, ref w, ref vt, ref materr, ref orterr, ref wsorted);
            
            //
            // update counter
            //
            succcount = succcount+1;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testbdsvdunit_test_silent()
        {
            bool result = new bool();

            result = testbdsvd(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testbdsvdunit_test()
        {
            bool result = new bool();

            result = testbdsvd(false);
            return result;
        }
    }
}
