
using System;

namespace alglib
{
    public class testsstunit
    {
        /*************************************************************************
        Main unittest subroutine
        *************************************************************************/
        public static bool testsst(bool silent)
        {
            bool result = new bool();
            int maxmn = 0;
            int passcount = 0;
            double threshold = 0;
            double[,] aeffective = new double[0,0];
            double[,] aparam = new double[0,0];
            double[] xe = new double[0];
            double[] b = new double[0];
            int n = 0;
            int pass = 0;
            int i = 0;
            int j = 0;
            int cnts = 0;
            int cntu = 0;
            int cntt = 0;
            int cntm = 0;
            bool waserrors = new bool();
            bool isupper = new bool();
            bool istrans = new bool();
            bool isunit = new bool();
            double v = 0;
            double s = 0;
            int i_ = 0;

            waserrors = false;
            maxmn = 15;
            passcount = 15;
            threshold = 1000*AP.Math.MachineEpsilon;
            
            //
            // Different problems
            //
            for(n=1; n<=maxmn; n++)
            {
                aeffective = new double[n-1+1, n-1+1];
                aparam = new double[n-1+1, n-1+1];
                xe = new double[n-1+1];
                b = new double[n-1+1];
                for(pass=1; pass<=passcount; pass++)
                {
                    for(cnts=0; cnts<=1; cnts++)
                    {
                        for(cntu=0; cntu<=1; cntu++)
                        {
                            for(cntt=0; cntt<=1; cntt++)
                            {
                                for(cntm=0; cntm<=2; cntm++)
                                {
                                    isupper = cnts==0;
                                    isunit = cntu==0;
                                    istrans = cntt==0;
                                    
                                    //
                                    // Skip meaningless combinations of parameters:
                                    // (matrix is singular) AND (matrix is unit diagonal)
                                    //
                                    if( cntm==2 & isunit )
                                    {
                                        continue;
                                    }
                                    
                                    //
                                    // Clear matrices
                                    //
                                    for(i=0; i<=n-1; i++)
                                    {
                                        for(j=0; j<=n-1; j++)
                                        {
                                            aeffective[i,j] = 0;
                                            aparam[i,j] = 0;
                                        }
                                    }
                                    
                                    //
                                    // Prepare matrices
                                    //
                                    if( isupper )
                                    {
                                        for(i=0; i<=n-1; i++)
                                        {
                                            for(j=i; j<=n-1; j++)
                                            {
                                                aeffective[i,j] = 0.9*(2*AP.Math.RandomReal()-1);
                                                aparam[i,j] = aeffective[i,j];
                                            }
                                            aeffective[i,i] = (2*AP.Math.RandomInteger(2)-1)*(0.8+AP.Math.RandomReal());
                                            aparam[i,i] = aeffective[i,i];
                                        }
                                    }
                                    else
                                    {
                                        for(i=0; i<=n-1; i++)
                                        {
                                            for(j=0; j<=i; j++)
                                            {
                                                aeffective[i,j] = 0.9*(2*AP.Math.RandomReal()-1);
                                                aparam[i,j] = aeffective[i,j];
                                            }
                                            aeffective[i,i] = (2*AP.Math.RandomInteger(2)-1)*(0.8+AP.Math.RandomReal());
                                            aparam[i,i] = aeffective[i,i];
                                        }
                                    }
                                    if( isunit )
                                    {
                                        for(i=0; i<=n-1; i++)
                                        {
                                            aeffective[i,i] = 1;
                                            aparam[i,i] = 0;
                                        }
                                    }
                                    if( istrans )
                                    {
                                        if( isupper )
                                        {
                                            for(i=0; i<=n-1; i++)
                                            {
                                                for(j=i+1; j<=n-1; j++)
                                                {
                                                    aeffective[j,i] = aeffective[i,j];
                                                    aeffective[i,j] = 0;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for(i=0; i<=n-1; i++)
                                            {
                                                for(j=i+1; j<=n-1; j++)
                                                {
                                                    aeffective[i,j] = aeffective[j,i];
                                                    aeffective[j,i] = 0;
                                                }
                                            }
                                        }
                                    }
                                    
                                    //
                                    // Prepare task, solve, compare
                                    //
                                    for(i=0; i<=n-1; i++)
                                    {
                                        xe[i] = 2*AP.Math.RandomReal()-1;
                                    }
                                    for(i=0; i<=n-1; i++)
                                    {
                                        v = 0.0;
                                        for(i_=0; i_<=n-1;i_++)
                                        {
                                            v += aeffective[i,i_]*xe[i_];
                                        }
                                        b[i] = v;
                                    }
                                    trlinsolve.rmatrixtrsafesolve(ref aparam, n, ref b, ref s, isupper, istrans, isunit);
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        xe[i_] = s*xe[i_];
                                    }
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        xe[i_] = xe[i_] - b[i_];
                                    }
                                    v = 0.0;
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        v += xe[i_]*xe[i_];
                                    }
                                    v = Math.Sqrt(v);
                                    waserrors = waserrors | (double)(v)>(double)(threshold);
                                }
                            }
                        }
                    }
                }
            }
            
            //
            // report
            //
            if( !silent )
            {
                System.Console.Write("TESTING RMatrixTRSafeSolve");
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
        Copy
        *************************************************************************/
        private static void makeacopy(ref double[,] a,
            int m,
            int n,
            ref double[,] b)
        {
            int i = 0;
            int j = 0;

            b = new double[m-1+1, n-1+1];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    b[i,j] = a[i,j];
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testsstunit_test_silent()
        {
            bool result = new bool();

            result = testsst(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testsstunit_test()
        {
            bool result = new bool();

            result = testsst(false);
            return result;
        }
    }
}
