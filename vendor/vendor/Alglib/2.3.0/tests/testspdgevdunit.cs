
using System;

namespace alglib
{
    public class testspdgevdunit
    {
        /*************************************************************************
        Testing bidiagonal SVD decomposition subroutine
        *************************************************************************/
        public static bool testspdgevd(bool silent)
        {
            bool result = new bool();
            int pass = 0;
            int n = 0;
            int passcount = 0;
            int maxn = 0;
            int atask = 0;
            int btask = 0;
            double[] d = new double[0];
            double[] t1 = new double[0];
            double[,] a = new double[0,0];
            double[,] b = new double[0,0];
            double[,] afull = new double[0,0];
            double[,] bfull = new double[0,0];
            double[,] l = new double[0,0];
            double[,] z = new double[0,0];
            bool isuppera = new bool();
            bool isupperb = new bool();
            int i = 0;
            int j = 0;
            int minij = 0;
            double v = 0;
            double v1 = 0;
            double v2 = 0;
            bool cw = new bool();
            double err = 0;
            double valerr = 0;
            double threshold = 0;
            bool waserrors = new bool();
            bool wfailed = new bool();
            bool wnsorted = new bool();
            int i_ = 0;

            threshold = 10000*AP.Math.MachineEpsilon;
            valerr = 0;
            wfailed = false;
            wnsorted = false;
            maxn = 20;
            passcount = 5;
            
            //
            // Main cycle
            //
            for(n=1; n<=maxn; n++)
            {
                for(pass=1; pass<=passcount; pass++)
                {
                    for(atask=0; atask<=1; atask++)
                    {
                        for(btask=0; btask<=1; btask++)
                        {
                            isuppera = atask==0;
                            isupperb = btask==0;
                            
                            //
                            // Initialize A, B, AFull, BFull
                            //
                            t1 = new double[n-1+1];
                            a = new double[n-1+1, n-1+1];
                            b = new double[n-1+1, n-1+1];
                            afull = new double[n-1+1, n-1+1];
                            bfull = new double[n-1+1, n-1+1];
                            l = new double[n-1+1, n-1+1];
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    a[i,j] = 2*AP.Math.RandomReal()-1;
                                    a[j,i] = a[i,j];
                                    afull[i,j] = a[i,j];
                                    afull[j,i] = a[i,j];
                                }
                            }
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=i+1; j<=n-1; j++)
                                {
                                    l[i,j] = AP.Math.RandomReal();
                                    l[j,i] = l[i,j];
                                }
                                l[i,i] = 1.5+AP.Math.RandomReal();
                            }
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    minij = Math.Min(i, j);
                                    v = 0.0;
                                    for(i_=0; i_<=minij;i_++)
                                    {
                                        v += l[i,i_]*l[i_,j];
                                    }
                                    b[i,j] = v;
                                    b[j,i] = v;
                                    bfull[i,j] = v;
                                    bfull[j,i] = v;
                                }
                            }
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    if( isuppera )
                                    {
                                        if( j<i )
                                        {
                                            a[i,j] = 2*AP.Math.RandomReal()-1;
                                        }
                                    }
                                    else
                                    {
                                        if( i<j )
                                        {
                                            a[i,j] = 2*AP.Math.RandomReal()-1;
                                        }
                                    }
                                    if( isupperb )
                                    {
                                        if( j<i )
                                        {
                                            b[i,j] = 2*AP.Math.RandomReal()-1;
                                        }
                                    }
                                    else
                                    {
                                        if( i<j )
                                        {
                                            b[i,j] = 2*AP.Math.RandomReal()-1;
                                        }
                                    }
                                }
                            }
                            
                            //
                            // Problem 1
                            //
                            if( !spdgevd.smatrixgevd(a, n, isuppera, ref b, isupperb, 1, 1, ref d, ref z) )
                            {
                                wfailed = true;
                                continue;
                            }
                            err = 0;
                            for(j=0; j<=n-1; j++)
                            {
                                for(i=0; i<=n-1; i++)
                                {
                                    v1 = 0.0;
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        v1 += afull[i,i_]*z[i_,j];
                                    }
                                    v2 = 0.0;
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        v2 += bfull[i,i_]*z[i_,j];
                                    }
                                    err = Math.Max(err, Math.Abs(v1-d[j]*v2));
                                }
                            }
                            valerr = Math.Max(err, valerr);
                            
                            //
                            // Problem 2
                            //
                            if( !spdgevd.smatrixgevd(a, n, isuppera, ref b, isupperb, 1, 2, ref d, ref z) )
                            {
                                wfailed = true;
                                continue;
                            }
                            err = 0;
                            for(j=0; j<=n-1; j++)
                            {
                                for(i=0; i<=n-1; i++)
                                {
                                    v1 = 0.0;
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        v1 += bfull[i,i_]*z[i_,j];
                                    }
                                    t1[i] = v1;
                                }
                                for(i=0; i<=n-1; i++)
                                {
                                    v2 = 0.0;
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        v2 += afull[i,i_]*t1[i_];
                                    }
                                    err = Math.Max(err, Math.Abs(v2-d[j]*z[i,j]));
                                }
                            }
                            valerr = Math.Max(err, valerr);
                            
                            //
                            // Test problem 3
                            //
                            if( !spdgevd.smatrixgevd(a, n, isuppera, ref b, isupperb, 1, 3, ref d, ref z) )
                            {
                                wfailed = true;
                                continue;
                            }
                            err = 0;
                            for(j=0; j<=n-1; j++)
                            {
                                for(i=0; i<=n-1; i++)
                                {
                                    v1 = 0.0;
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        v1 += afull[i,i_]*z[i_,j];
                                    }
                                    t1[i] = v1;
                                }
                                for(i=0; i<=n-1; i++)
                                {
                                    v2 = 0.0;
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        v2 += bfull[i,i_]*t1[i_];
                                    }
                                    err = Math.Max(err, Math.Abs(v2-d[j]*z[i,j]));
                                }
                            }
                            valerr = Math.Max(err, valerr);
                        }
                    }
                }
            }
            
            //
            // report
            //
            waserrors = (double)(valerr)>(double)(threshold) | wfailed | wnsorted;
            if( !silent )
            {
                System.Console.Write("TESTING SYMMETRIC GEVD");
                System.Console.WriteLine();
                System.Console.Write("Av-lambdav error (generalized):          ");
                System.Console.Write("{0,5:E3}",valerr);
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
        Silent unit test
        *************************************************************************/
        public static bool testspdgevdunit_test_silent()
        {
            bool result = new bool();

            result = testspdgevd(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testspdgevdunit_test()
        {
            bool result = new bool();

            result = testspdgevd(false);
            return result;
        }
    }
}
