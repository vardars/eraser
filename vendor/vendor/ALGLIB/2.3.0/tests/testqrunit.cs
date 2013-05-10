
using System;

namespace alglib
{
    public class testqrunit
    {
        public static double threshold = 0;
        public static bool structerrors = new bool();
        public static bool decomperrors = new bool();
        public static bool othererrors = new bool();


        /*************************************************************************
        Main unittest subroutine
        *************************************************************************/
        public static bool testqr(bool silent)
        {
            bool result = new bool();
            int shortmn = 0;
            int maxmn = 0;
            int gpasscount = 0;
            double[,] a = new double[0,0];
            int m = 0;
            int n = 0;
            int gpass = 0;
            int i = 0;
            int j = 0;
            bool waserrors = new bool();

            decomperrors = false;
            othererrors = false;
            structerrors = false;
            waserrors = false;
            shortmn = 5;
            maxmn = 15;
            gpasscount = 5;
            threshold = 5*100*AP.Math.MachineEpsilon;
            a = new double[maxmn-1+1, maxmn-1+1];
            
            //
            // Different problems
            //
            for(gpass=1; gpass<=gpasscount; gpass++)
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
                for(i=1; i<=maxmn; i++)
                {
                    for(j=1; j<=maxmn; j++)
                    {
                        testproblem(ref a, i, j);
                    }
                }
                
                //
                // Long dense matrix
                //
                for(i=0; i<=maxmn-1; i++)
                {
                    for(j=0; j<=shortmn-1; j++)
                    {
                        a[i,j] = 2*AP.Math.RandomReal()-1;
                    }
                }
                for(i=shortmn+1; i<=maxmn; i++)
                {
                    testproblem(ref a, i, shortmn);
                }
                for(i=0; i<=shortmn-1; i++)
                {
                    for(j=0; j<=maxmn-1; j++)
                    {
                        a[i,j] = 2*AP.Math.RandomReal()-1;
                    }
                }
                for(j=shortmn+1; j<=maxmn; j++)
                {
                    testproblem(ref a, shortmn, j);
                }
                
                //
                // Dense matrices
                //
                for(m=1; m<=maxmn; m++)
                {
                    for(n=1; n<=maxmn; n++)
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                a[i,j] = 2*AP.Math.RandomReal()-1;
                            }
                        }
                        testproblem(ref a, m, n);
                    }
                }
                
                //
                // Sparse matrices, very sparse matrices, incredible sparse matrices
                //
                for(m=1; m<=maxmn; m++)
                {
                    for(n=1; n<=maxmn; n++)
                    {
                        fillsparsea(ref a, m, n, 0.8);
                        testproblem(ref a, m, n);
                        fillsparsea(ref a, m, n, 0.9);
                        testproblem(ref a, m, n);
                        fillsparsea(ref a, m, n, 0.95);
                        testproblem(ref a, m, n);
                    }
                }
            }
            
            //
            // report
            //
            waserrors = structerrors | decomperrors | othererrors;
            if( !silent )
            {
                System.Console.Write("TESTING RMatrixQR");
                System.Console.WriteLine();
                System.Console.Write("STRUCTURAL ERRORS:                       ");
                if( !structerrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("DECOMPOSITION ERRORS:                    ");
                if( !decomperrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("OTHER ERRORS:                            ");
                if( !othererrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
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
        Sparse fill
        *************************************************************************/
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
        Problem testing
        *************************************************************************/
        private static void testproblem(ref double[,] a,
            int m,
            int n)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            double mx = 0;
            double[,] b = new double[0,0];
            double[] taub = new double[0];
            double[,] q = new double[0,0];
            double[,] r = new double[0,0];
            double[,] q2 = new double[0,0];
            double v = 0;
            int i_ = 0;

            
            //
            // MX - estimate of the matrix norm
            //
            mx = 0;
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( (double)(Math.Abs(a[i,j]))>(double)(mx) )
                    {
                        mx = Math.Abs(a[i,j]);
                    }
                }
            }
            if( (double)(mx)==(double)(0) )
            {
                mx = 1;
            }
            
            //
            // Test decompose-and-unpack error
            //
            makeacopy(ref a, m, n, ref b);
            qr.rmatrixqr(ref b, m, n, ref taub);
            qr.rmatrixqrunpackq(ref b, m, n, ref taub, m, ref q);
            qr.rmatrixqrunpackr(ref b, m, n, ref r);
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=m-1;i_++)
                    {
                        v += q[i,i_]*r[i_,j];
                    }
                    decomperrors = decomperrors | (double)(Math.Abs(v-a[i,j]))>=(double)(threshold);
                }
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=Math.Min(i, n-1)-1; j++)
                {
                    structerrors = structerrors | (double)(r[i,j])!=(double)(0);
                }
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=m-1;i_++)
                    {
                        v += q[i,i_]*q[j,i_];
                    }
                    if( i==j )
                    {
                        structerrors = structerrors | (double)(Math.Abs(v-1))>=(double)(threshold);
                    }
                    else
                    {
                        structerrors = structerrors | (double)(Math.Abs(v))>=(double)(threshold);
                    }
                }
            }
            
            //
            // Test for other errors
            //
            for(k=1; k<=m-1; k++)
            {
                qr.rmatrixqrunpackq(ref b, m, n, ref taub, k, ref q2);
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=k-1; j++)
                    {
                        othererrors = othererrors | (double)(Math.Abs(q2[i,j]-q[i,j]))>(double)(10*AP.Math.MachineEpsilon);
                    }
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testqrunit_test_silent()
        {
            bool result = new bool();

            result = testqr(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testqrunit_test()
        {
            bool result = new bool();

            result = testqr(false);
            return result;
        }
    }
}