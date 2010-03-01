
using System;

namespace alglib
{
    public class testinvldltunit
    {
        public static bool testinvldlt(bool silent)
        {
            bool result = new bool();
            double[,] a = new double[0,0];
            double[,] a2 = new double[0,0];
            double[,] a3 = new double[0,0];
            int[] p = new int[0];
            int n = 0;
            int pass = 0;
            int mtask = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int minij = 0;
            bool upperin = new bool();
            bool cr = new bool();
            double v = 0;
            double err = 0;
            bool waserrors = new bool();
            int passcount = 0;
            int maxn = 0;
            int htask = 0;
            double threshold = 0;
            int i_ = 0;

            err = 0;
            passcount = 10;
            maxn = 20;
            threshold = 10000000*AP.Math.MachineEpsilon;
            waserrors = false;
            
            //
            // Test
            //
            for(n=1; n<=maxn; n++)
            {
                a = new double[n-1+1, n-1+1];
                a2 = new double[n-1+1, n-1+1];
                a3 = new double[n-1+1, n-1+1];
                for(mtask=2; mtask<=2; mtask++)
                {
                    for(htask=0; htask<=1; htask++)
                    {
                        for(pass=1; pass<=passcount; pass++)
                        {
                            upperin = htask==0;
                            
                            //
                            // Prepare task:
                            // * A contains symmetric matrix
                            // * A2, A3 contains its upper (or lower) half
                            //
                            generatematrix(ref a, n, mtask);
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    a2[i,j] = a[i,j];
                                    a3[i,j] = a[i,j];
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
                                            a2[i,j] = 0;
                                            a3[i,j] = 0;
                                        }
                                    }
                                    else
                                    {
                                        if( i<j )
                                        {
                                            a2[i,j] = 0;
                                            a3[i,j] = 0;
                                        }
                                    }
                                }
                            }
                            
                            //
                            // Test 1: inv(A2)
                            //
                            sinverse.smatrixinverse(ref a2, n, upperin);
                            restorematrix(ref a2, n, upperin);
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    v = 0.0;
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        v += a[i,i_]*a2[i_,j];
                                    }
                                    if( i==j )
                                    {
                                        v = v-1;
                                    }
                                    err = Math.Max(err, Math.Abs(v));
                                }
                            }
                            
                            //
                            // Test 2: inv(LDLt(A3))
                            //
                            ldlt.smatrixldlt(ref a3, n, upperin, ref p);
                            sinverse.smatrixldltinverse(ref a3, ref p, n, upperin);
                            restorematrix(ref a3, n, upperin);
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    v = 0.0;
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        v += a[i,i_]*a3[i_,j];
                                    }
                                    if( i==j )
                                    {
                                        v = v-1;
                                    }
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
            waserrors = (double)(err)>(double)(threshold);
            if( !silent )
            {
                System.Console.Write("TESTING LDLT INVERSE");
                System.Console.WriteLine();
                System.Console.Write("ERROR:                                   ");
                System.Console.Write("{0,5:E3}",err);
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


        private static void generatematrix(ref double[,] a,
            int n,
            int task)
        {
            int i = 0;
            int j = 0;

            if( task==0 )
            {
                
                //
                // Zero matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a[i,j] = 0;
                    }
                }
            }
            if( task==1 )
            {
                
                //
                // Sparse matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    for(j=i+1; j<=n-1; j++)
                    {
                        if( (double)(AP.Math.RandomReal())>(double)(0.95) )
                        {
                            a[i,j] = 2*AP.Math.RandomReal()-1;
                        }
                        else
                        {
                            a[i,j] = 0;
                        }
                        a[j,i] = a[i,j];
                    }
                    if( (double)(AP.Math.RandomReal())>(double)(0.95) )
                    {
                        a[i,i] = (2*AP.Math.RandomInteger(2)-1)*(0.8+AP.Math.RandomReal());
                    }
                    else
                    {
                        a[i,i] = 0;
                    }
                }
            }
            if( task==2 )
            {
                
                //
                // Dense matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    for(j=i+1; j<=n-1; j++)
                    {
                        a[i,j] = 2*AP.Math.RandomReal()-1;
                        a[j,i] = a[i,j];
                    }
                    a[i,i] = (2*AP.Math.RandomInteger(2)-1)*(0.7+AP.Math.RandomReal());
                }
            }
        }


        private static void restorematrix(ref double[,] a,
            int n,
            bool upperin)
        {
            int i = 0;
            int j = 0;

            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( upperin )
                    {
                        if( j<i )
                        {
                            a[i,j] = a[j,i];
                        }
                    }
                    else
                    {
                        if( i<j )
                        {
                            a[i,j] = a[j,i];
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testinvldltunit_test_silent()
        {
            bool result = new bool();

            result = testinvldlt(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testinvldltunit_test()
        {
            bool result = new bool();

            result = testinvldlt(false);
            return result;
        }
    }
}
