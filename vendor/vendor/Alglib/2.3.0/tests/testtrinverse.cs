
using System;

namespace alglib
{
    public class testtrinverse
    {
        /*************************************************************************
        Main unittest subroutine
        *************************************************************************/
        public static bool testtrinv(bool silent)
        {
            bool result = new bool();
            int shortmn = 0;
            int maxn = 0;
            int passcount = 0;
            double threshold = 0;
            double[,] a = new double[0,0];
            double[,] b = new double[0,0];
            int n = 0;
            int pass = 0;
            int i = 0;
            int j = 0;
            int task = 0;
            bool isupper = new bool();
            bool isunit = new bool();
            double v = 0;
            bool invfailed = new bool();
            bool inverrors = new bool();
            bool structerrors = new bool();
            bool waserrors = new bool();
            int i_ = 0;

            invfailed = false;
            inverrors = false;
            structerrors = false;
            waserrors = false;
            maxn = 15;
            passcount = 5;
            threshold = 5*100*AP.Math.MachineEpsilon;
            
            //
            // Test
            //
            for(n=1; n<=maxn; n++)
            {
                a = new double[n-1+1, n-1+1];
                b = new double[n-1+1, n-1+1];
                for(task=0; task<=3; task++)
                {
                    for(pass=1; pass<=passcount; pass++)
                    {
                        
                        //
                        // Determine task
                        //
                        isupper = task%2==0;
                        isunit = task/2%2==0;
                        
                        //
                        // Generate matrix
                        //
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                if( i==j )
                                {
                                    a[i,j] = 1.5+AP.Math.RandomReal();
                                }
                                else
                                {
                                    a[i,j] = 2*AP.Math.RandomReal()-1;
                                }
                                b[i,j] = a[i,j];
                            }
                        }
                        
                        //
                        // Inverse
                        //
                        if( !trinverse.rmatrixtrinverse(ref b, n, isupper, isunit) )
                        {
                            invfailed = true;
                            continue;
                        }
                        
                        //
                        // Structural test
                        //
                        if( isunit )
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                structerrors = structerrors | (double)(a[i,i])!=(double)(b[i,i]);
                            }
                        }
                        if( isupper )
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=i-1; j++)
                                {
                                    structerrors = structerrors | (double)(a[i,j])!=(double)(b[i,j]);
                                }
                            }
                        }
                        else
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=i+1; j<=n-1; j++)
                                {
                                    structerrors = structerrors | (double)(a[i,j])!=(double)(b[i,j]);
                                }
                            }
                        }
                        
                        //
                        // Inverse test
                        //
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                if( j<i & isupper | j>i & !isupper )
                                {
                                    a[i,j] = 0;
                                    b[i,j] = 0;
                                }
                            }
                        }
                        if( isunit )
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                a[i,i] = 1;
                                b[i,i] = 1;
                            }
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                v = 0.0;
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    v += a[i,i_]*b[i_,j];
                                }
                                if( j!=i )
                                {
                                    inverrors = inverrors | (double)(Math.Abs(v))>(double)(threshold);
                                }
                                else
                                {
                                    inverrors = inverrors | (double)(Math.Abs(v-1))>(double)(threshold);
                                }
                            }
                        }
                    }
                }
            }
            
            //
            // report
            //
            waserrors = inverrors | structerrors | invfailed;
            if( !silent )
            {
                System.Console.Write("TESTING TRIANGULAR INVERSE");
                System.Console.WriteLine();
                if( invfailed )
                {
                    System.Console.Write("SOME INVERSIONS FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("INVERSION TEST:                          ");
                if( inverrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("STRUCTURE TEST:                          ");
                if( structerrors )
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
        public static bool testtrinverse_test_silent()
        {
            bool result = new bool();

            result = testtrinv(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testtrinverse_test()
        {
            bool result = new bool();

            result = testtrinv(false);
            return result;
        }
    }
}
