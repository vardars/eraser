
using System;

namespace alglib
{
    public class testinvcholeskyunit
    {
        public static bool testinvcholesky(bool silent)
        {
            bool result = new bool();
            double[,] l = new double[0,0];
            double[,] a = new double[0,0];
            double[,] a2 = new double[0,0];
            int n = 0;
            int pass = 0;
            int i = 0;
            int j = 0;
            int minij = 0;
            bool upperin = new bool();
            bool cr = new bool();
            double v = 0;
            double err = 0;
            bool wf = new bool();
            bool waserrors = new bool();
            int passcount = 0;
            int maxn = 0;
            int htask = 0;
            double threshold = 0;
            int i_ = 0;

            err = 0;
            wf = false;
            passcount = 10;
            maxn = 20;
            threshold = 1000*AP.Math.MachineEpsilon;
            waserrors = false;
            
            //
            // Test
            //
            for(n=1; n<=maxn; n++)
            {
                l = new double[n-1+1, n-1+1];
                a = new double[n-1+1, n-1+1];
                a2 = new double[n-1+1, n-1+1];
                for(htask=0; htask<=1; htask++)
                {
                    for(pass=1; pass<=passcount; pass++)
                    {
                        upperin = htask==0;
                        
                        //
                        // Prepare task:
                        // * A contains upper (or lower) half of SPD matrix
                        // * L contains its Cholesky factor (upper or lower)
                        // * A2 contains copy of A
                        //
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=i+1; j<=n-1; j++)
                            {
                                l[i,j] = 2*AP.Math.RandomReal()-1;
                                l[j,i] = l[i,j];
                            }
                            l[i,i] = 1.1+AP.Math.RandomReal();
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
                                a[i,j] = v;
                                a2[i,j] = v;
                            }
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                if( upperin )
                                {
                                    if( j<i )
                                    {
                                        l[i,j] = 0;
                                        a2[i,j] = 0;
                                    }
                                }
                                else
                                {
                                    if( i<j )
                                    {
                                        l[i,j] = 0;
                                        a2[i,j] = 0;
                                    }
                                }
                            }
                        }
                        
                        //
                        // test inv(A):
                        // 1. invert
                        // 2. complement with missing triangle
                        // 3. test
                        //
                        if( !spdinverse.spdmatrixinverse(ref a2, n, upperin) )
                        {
                            wf = true;
                            continue;
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                if( upperin )
                                {
                                    if( j<i )
                                    {
                                        a2[i,j] = a2[j,i];
                                    }
                                }
                                else
                                {
                                    if( i<j )
                                    {
                                        a2[i,j] = a2[j,i];
                                    }
                                }
                            }
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                v = 0.0;
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    v += a[i,i_]*a2[i_,j];
                                }
                                if( j==i )
                                {
                                    err = Math.Max(err, Math.Abs(v-1));
                                }
                                else
                                {
                                    err = Math.Max(err, Math.Abs(v));
                                }
                            }
                        }
                        
                        //
                        // test inv(cholesky(A)):
                        // 1. invert
                        // 2. complement with missing triangle
                        // 3. test
                        //
                        if( !spdinverse.spdmatrixcholeskyinverse(ref l, n, upperin) )
                        {
                            wf = true;
                            continue;
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                if( upperin )
                                {
                                    if( j<i )
                                    {
                                        l[i,j] = l[j,i];
                                    }
                                }
                                else
                                {
                                    if( i<j )
                                    {
                                        l[i,j] = l[j,i];
                                    }
                                }
                            }
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                v = 0.0;
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    v += a[i,i_]*l[i_,j];
                                }
                                if( j==i )
                                {
                                    err = Math.Max(err, Math.Abs(v-1));
                                }
                                else
                                {
                                    err = Math.Max(err, Math.Abs(v));
                                }
                            }
                        }
                    }
                }
            }
            
            //
            // report
            //
            waserrors = (double)(err)>(double)(threshold) | wf;
            if( !silent )
            {
                System.Console.Write("TESTING SPD INVERSE");
                System.Console.WriteLine();
                System.Console.Write("ERROR:                                   ");
                System.Console.Write("{0,5:E3}",err);
                System.Console.WriteLine();
                System.Console.Write("ALWAYS SUCCEDED:                         ");
                if( wf )
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
                System.Console.WriteLine();
                System.Console.WriteLine();
            }
            result = !waserrors;
            return result;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testinvcholeskyunit_test_silent()
        {
            bool result = new bool();

            result = testinvcholesky(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testinvcholeskyunit_test()
        {
            bool result = new bool();

            result = testinvcholesky(false);
            return result;
        }
    }
}
