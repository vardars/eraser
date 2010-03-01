
using System;

namespace alglib
{
    public class testtdevdunit
    {
        /*************************************************************************
        Testing bidiagonal SVD decomposition subroutine
        *************************************************************************/
        public static bool testtdevd(bool silent)
        {
            bool result = new bool();
            double[] d = new double[0];
            double[] e = new double[0];
            int pass = 0;
            int n = 0;
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
                d = new double[n-1+1];
                if( n>1 )
                {
                    e = new double[n-2+1];
                }
                
                //
                // Different tasks
                //
                for(mkind=0; mkind<=4; mkind++)
                {
                    fillde(ref d, ref e, n, mkind);
                    testevdproblem(ref d, ref e, n, ref materr, ref valerr, ref orterr, ref wnsorted, ref failc);
                    runs = runs+1;
                }
            }
            
            //
            // report
            //
            failr = (double)(failc)/(double)(runs);
            wfailed = (double)(failr)>(double)(failthreshold);
            waserrors = (double)(materr)>(double)(threshold) | (double)(valerr)>(double)(threshold) | (double)(orterr)>(double)(threshold) | wnsorted | wfailed;
            if( !silent )
            {
                System.Console.Write("TESTING TRIDIAGONAL EVD");
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
        Fills D and E
        *************************************************************************/
        private static void fillde(ref double[] d,
            ref double[] e,
            int n,
            int filltype)
        {
            int i = 0;
            int j = 0;

            if( filltype==0 )
            {
                
                //
                // Zero matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    d[i] = 0;
                }
                for(i=0; i<=n-2; i++)
                {
                    e[i] = 0;
                }
                return;
            }
            if( filltype==1 )
            {
                
                //
                // Diagonal matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    d[i] = 2*AP.Math.RandomReal()-1;
                }
                for(i=0; i<=n-2; i++)
                {
                    e[i] = 0;
                }
                return;
            }
            if( filltype==2 )
            {
                
                //
                // Off-diagonal matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    d[i] = 0;
                }
                for(i=0; i<=n-2; i++)
                {
                    e[i] = 2*AP.Math.RandomReal()-1;
                }
                return;
            }
            if( filltype==3 )
            {
                
                //
                // Dense matrix with blocks
                //
                for(i=0; i<=n-1; i++)
                {
                    d[i] = 2*AP.Math.RandomReal()-1;
                }
                for(i=0; i<=n-2; i++)
                {
                    e[i] = 2*AP.Math.RandomReal()-1;
                }
                j = 1;
                i = 2;
                while( j<=n-2 )
                {
                    e[j] = 0;
                    j = j+i;
                    i = i+1;
                }
                return;
            }
            
            //
            // dense matrix
            //
            for(i=0; i<=n-1; i++)
            {
                d[i] = 2*AP.Math.RandomReal()-1;
            }
            for(i=0; i<=n-2; i++)
            {
                e[i] = 2*AP.Math.RandomReal()-1;
            }
        }


        /*************************************************************************
        Tests Z*Lambda*Z' against tridiag(D,E).
        Returns relative error.
        *************************************************************************/
        private static double testproduct(ref double[] d,
            ref double[] e,
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
                    if( Math.Abs(i-j)==0 )
                    {
                        result = Math.Max(result, Math.Abs(v-d[i]));
                    }
                    if( Math.Abs(i-j)==1 )
                    {
                        result = Math.Max(result, Math.Abs(v-e[Math.Min(i, j)]));
                    }
                    if( Math.Abs(i-j)>1 )
                    {
                        result = Math.Max(result, Math.Abs(v));
                    }
                }
            }
            mx = 0;
            for(i=0; i<=n-1; i++)
            {
                mx = Math.Max(mx, Math.Abs(d[i]));
            }
            for(i=0; i<=n-2; i++)
            {
                mx = Math.Max(mx, Math.Abs(e[i]));
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
        private static void testevdproblem(ref double[] d,
            ref double[] e,
            int n,
            ref double materr,
            ref double valerr,
            ref double orterr,
            ref bool wnsorted,
            ref int failc)
        {
            double[] lambda = new double[0];
            double[] ee = new double[0];
            double[] lambda2 = new double[0];
            double[,] z = new double[0,0];
            double[,] zref = new double[0,0];
            double[,] a1 = new double[0,0];
            double[,] a2 = new double[0,0];
            bool wsucc = new bool();
            int i = 0;
            int j = 0;
            double v = 0;
            int i_ = 0;

            lambda = new double[n-1+1];
            lambda2 = new double[n-1+1];
            zref = new double[n-1+1, n-1+1];
            a1 = new double[n-1+1, n-1+1];
            a2 = new double[n-1+1, n-1+1];
            if( n>1 )
            {
                ee = new double[n-2+1];
            }
            
            //
            // Test simple EVD: values and full vectors
            //
            for(i=0; i<=n-1; i++)
            {
                lambda[i] = d[i];
            }
            for(i=0; i<=n-2; i++)
            {
                ee[i] = e[i];
            }
            wsucc = tdevd.smatrixtdevd(ref lambda, ee, n, 2, ref z);
            if( !wsucc )
            {
                failc = failc+1;
                return;
            }
            materr = Math.Max(materr, testproduct(ref d, ref e, n, ref z, ref lambda));
            orterr = Math.Max(orterr, testort(ref z, n));
            for(i=0; i<=n-2; i++)
            {
                if( (double)(lambda[i+1])<(double)(lambda[i]) )
                {
                    wnsorted = true;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    zref[i,j] = z[i,j];
                }
            }
            
            //
            // Test values only variant
            //
            for(i=0; i<=n-1; i++)
            {
                lambda2[i] = d[i];
            }
            for(i=0; i<=n-2; i++)
            {
                ee[i] = e[i];
            }
            wsucc = tdevd.smatrixtdevd(ref lambda2, ee, n, 0, ref z);
            if( !wsucc )
            {
                failc = failc+1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                valerr = Math.Max(valerr, Math.Abs(lambda2[i]-lambda[i]));
            }
            
            //
            // Test multiplication variant
            //
            for(i=0; i<=n-1; i++)
            {
                lambda2[i] = d[i];
            }
            for(i=0; i<=n-2; i++)
            {
                ee[i] = e[i];
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a1[i,j] = 2*AP.Math.RandomReal()-1;
                    a2[i,j] = a1[i,j];
                }
            }
            wsucc = tdevd.smatrixtdevd(ref lambda2, ee, n, 1, ref a1);
            if( !wsucc )
            {
                failc = failc+1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                valerr = Math.Max(valerr, Math.Abs(lambda2[i]-lambda[i]));
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += a2[i,i_]*zref[i_,j];
                    }
                    materr = Math.Max(materr, Math.Abs(v-a1[i,j]));
                }
            }
            
            //
            // Test first row variant
            //
            for(i=0; i<=n-1; i++)
            {
                lambda2[i] = d[i];
            }
            for(i=0; i<=n-2; i++)
            {
                ee[i] = e[i];
            }
            wsucc = tdevd.smatrixtdevd(ref lambda2, ee, n, 3, ref z);
            if( !wsucc )
            {
                failc = failc+1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                valerr = Math.Max(valerr, Math.Abs(lambda2[i]-lambda[i]));
                materr = Math.Max(materr, Math.Abs(z[0,i]-zref[0,i]));
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testtdevdunit_test_silent()
        {
            bool result = new bool();

            result = testtdevd(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testtdevdunit_test()
        {
            bool result = new bool();

            result = testtdevd(false);
            return result;
        }
    }
}
