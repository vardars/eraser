
using System;

namespace alglib
{
    public class testsvdunit
    {
        public static int failcount = 0;
        public static int succcount = 0;


        /*************************************************************************
        Testing SVD decomposition subroutine
        *************************************************************************/
        public static bool testsvd(bool silent)
        {
            bool result = new bool();
            double[,] a = new double[0,0];
            int m = 0;
            int n = 0;
            int maxmn = 0;
            int i = 0;
            int j = 0;
            int gpass = 0;
            int pass = 0;
            bool waserrors = new bool();
            bool wsorted = new bool();
            bool wfailed = new bool();
            double materr = 0;
            double orterr = 0;
            double othererr = 0;
            double threshold = 0;
            double failthreshold = 0;
            double failr = 0;

            failcount = 0;
            succcount = 0;
            materr = 0;
            orterr = 0;
            othererr = 0;
            wsorted = true;
            wfailed = false;
            waserrors = false;
            maxmn = 30;
            threshold = 5*100*AP.Math.MachineEpsilon;
            failthreshold = 5.0E-3;
            a = new double[maxmn-1+1, maxmn-1+1];
            
            //
            // TODO: div by zero fail, convergence fail
            //
            for(gpass=1; gpass<=1; gpass++)
            {
                
                //
                // zero matrix, several cases
                //
                for(i=0; i<=maxmn-1; i++)
                {
                    for(j=0; j<=maxmn-1; j++)
                    {
                        a[i,j] = 0;
                    }
                }
                for(i=1; i<=Math.Min(5, maxmn); i++)
                {
                    for(j=1; j<=Math.Min(5, maxmn); j++)
                    {
                        testsvdproblem(ref a, i, j, ref materr, ref orterr, ref othererr, ref wsorted, ref wfailed);
                    }
                }
                
                //
                // Long dense matrix
                //
                for(i=0; i<=maxmn-1; i++)
                {
                    for(j=0; j<=Math.Min(5, maxmn)-1; j++)
                    {
                        a[i,j] = 2*AP.Math.RandomReal()-1;
                    }
                }
                for(i=1; i<=maxmn; i++)
                {
                    for(j=1; j<=Math.Min(5, maxmn); j++)
                    {
                        testsvdproblem(ref a, i, j, ref materr, ref orterr, ref othererr, ref wsorted, ref wfailed);
                    }
                }
                for(i=0; i<=Math.Min(5, maxmn)-1; i++)
                {
                    for(j=0; j<=maxmn-1; j++)
                    {
                        a[i,j] = 2*AP.Math.RandomReal()-1;
                    }
                }
                for(i=1; i<=Math.Min(5, maxmn); i++)
                {
                    for(j=1; j<=maxmn; j++)
                    {
                        testsvdproblem(ref a, i, j, ref materr, ref orterr, ref othererr, ref wsorted, ref wfailed);
                    }
                }
                
                //
                // Dense matrices
                //
                for(m=1; m<=Math.Min(10, maxmn); m++)
                {
                    for(n=1; n<=Math.Min(10, maxmn); n++)
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                a[i,j] = 2*AP.Math.RandomReal()-1;
                            }
                        }
                        testsvdproblem(ref a, m, n, ref materr, ref orterr, ref othererr, ref wsorted, ref wfailed);
                    }
                }
                
                //
                // Sparse matrices, very sparse matrices, incredible sparse matrices
                //
                for(m=1; m<=10; m++)
                {
                    for(n=1; n<=10; n++)
                    {
                        for(pass=1; pass<=2; pass++)
                        {
                            fillsparsea(ref a, m, n, 0.8);
                            testsvdproblem(ref a, m, n, ref materr, ref orterr, ref othererr, ref wsorted, ref wfailed);
                            fillsparsea(ref a, m, n, 0.9);
                            testsvdproblem(ref a, m, n, ref materr, ref orterr, ref othererr, ref wsorted, ref wfailed);
                            fillsparsea(ref a, m, n, 0.95);
                            testsvdproblem(ref a, m, n, ref materr, ref orterr, ref othererr, ref wsorted, ref wfailed);
                        }
                    }
                }
            }
            
            //
            // report
            //
            failr = (double)(failcount)/((double)(succcount+failcount));
            waserrors = (double)(materr)>(double)(threshold) | (double)(orterr)>(double)(threshold) | (double)(othererr)>(double)(threshold) | !wsorted | (double)(failr)>(double)(failthreshold);
            if( !silent )
            {
                System.Console.Write("TESTING SVD DECOMPOSITION");
                System.Console.WriteLine();
                System.Console.Write("SVD decomposition error:                 ");
                System.Console.Write("{0,5:E3}",materr);
                System.Console.WriteLine();
                System.Console.Write("SVD orthogonality error:                 ");
                System.Console.Write("{0,5:E3}",orterr);
                System.Console.WriteLine();
                System.Console.Write("SVD with different parameters error:     ");
                System.Console.Write("{0,5:E3}",othererr);
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
            int m,
            int n,
            double sparcity)
        {
            int i = 0;
            int j = 0;

            for(i=0; i<=m-1; i++)
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


        private static void getsvderror(ref double[,] a,
            int m,
            int n,
            ref double[,] u,
            ref double[] w,
            ref double[,] vt,
            ref double materr,
            ref double orterr,
            ref bool wsorted)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int minmn = 0;
            double locerr = 0;
            double sm = 0;
            int i_ = 0;

            minmn = Math.Min(m, n);
            
            //
            // decomposition error
            //
            locerr = 0;
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    sm = 0;
                    for(k=0; k<=minmn-1; k++)
                    {
                        sm = sm+w[k]*u[i,k]*vt[k,j];
                    }
                    locerr = Math.Max(locerr, Math.Abs(a[i,j]-sm));
                }
            }
            materr = Math.Max(materr, locerr);
            
            //
            // orthogonality error
            //
            locerr = 0;
            for(i=0; i<=minmn-1; i++)
            {
                for(j=i; j<=minmn-1; j++)
                {
                    sm = 0.0;
                    for(i_=0; i_<=m-1;i_++)
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
            for(i=1; i<=minmn-1; i++)
            {
                if( (double)(w[i])>(double)(w[i-1]) )
                {
                    wsorted = false;
                }
            }
        }


        private static void testsvdproblem(ref double[,] a,
            int m,
            int n,
            ref double materr,
            ref double orterr,
            ref double othererr,
            ref bool wsorted,
            ref bool wfailed)
        {
            double[,] u = new double[0,0];
            double[,] vt = new double[0,0];
            double[,] u2 = new double[0,0];
            double[,] vt2 = new double[0,0];
            double[] w = new double[0];
            double[] w2 = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            int ujob = 0;
            int vtjob = 0;
            int memjob = 0;
            int ucheck = 0;
            int vtcheck = 0;
            double v = 0;
            double mx = 0;

            
            //
            // Main SVD test
            //
            if( !svd.rmatrixsvd(a, m, n, 2, 2, 2, ref w, ref u, ref vt) )
            {
                failcount = failcount+1;
                wfailed = true;
                return;
            }
            getsvderror(ref a, m, n, ref u, ref w, ref vt, ref materr, ref orterr, ref wsorted);
            
            //
            // Additional SVD tests
            //
            for(ujob=0; ujob<=2; ujob++)
            {
                for(vtjob=0; vtjob<=2; vtjob++)
                {
                    for(memjob=0; memjob<=2; memjob++)
                    {
                        if( !svd.rmatrixsvd(a, m, n, ujob, vtjob, memjob, ref w2, ref u2, ref vt2) )
                        {
                            failcount = failcount+1;
                            wfailed = true;
                            return;
                        }
                        ucheck = 0;
                        if( ujob==1 )
                        {
                            ucheck = Math.Min(m, n);
                        }
                        if( ujob==2 )
                        {
                            ucheck = m;
                        }
                        vtcheck = 0;
                        if( vtjob==1 )
                        {
                            vtcheck = Math.Min(m, n);
                        }
                        if( vtjob==2 )
                        {
                            vtcheck = n;
                        }
                        for(i=0; i<=m-1; i++)
                        {
                            for(j=0; j<=ucheck-1; j++)
                            {
                                othererr = Math.Max(othererr, Math.Abs(u[i,j]-u2[i,j]));
                            }
                        }
                        for(i=0; i<=vtcheck-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                othererr = Math.Max(othererr, Math.Abs(vt[i,j]-vt2[i,j]));
                            }
                        }
                        for(i=0; i<=Math.Min(m, n)-1; i++)
                        {
                            othererr = Math.Max(othererr, Math.Abs(w[i]-w2[i]));
                        }
                    }
                }
            }
            
            //
            // update counter
            //
            succcount = succcount+1;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testsvdunit_test_silent()
        {
            bool result = new bool();

            result = testsvd(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testsvdunit_test()
        {
            bool result = new bool();

            result = testsvd(false);
            return result;
        }
    }
}
