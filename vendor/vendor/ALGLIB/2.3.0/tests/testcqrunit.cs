
using System;

namespace alglib
{
    public class testcqrunit
    {
        public static double threshold = 0;
        public static bool structerrors = new bool();
        public static bool decomperrors = new bool();
        public static bool othererrors = new bool();


        /*************************************************************************
        Main unittest subroutine
        *************************************************************************/
        public static bool testcqr(bool silent)
        {
            bool result = new bool();
            int shortmn = 0;
            int maxmn = 0;
            int gpasscount = 0;
            AP.Complex[,] a = new AP.Complex[0,0];
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
            a = new AP.Complex[maxmn-1+1, maxmn-1+1];
            
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
                        a[i,j].x = 2*AP.Math.RandomReal()-1;
                        a[i,j].y = 2*AP.Math.RandomReal()-1;
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
                        a[i,j].x = 2*AP.Math.RandomReal()-1;
                        a[i,j].y = 2*AP.Math.RandomReal()-1;
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
                                a[i,j].x = 2*AP.Math.RandomReal()-1;
                                a[i,j].y = 2*AP.Math.RandomReal()-1;
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
                System.Console.Write("TESTING CMatrixQR");
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
        private static void fillsparsea(ref AP.Complex[,] a,
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
                        a[i,j].x = 2*AP.Math.RandomReal()-1;
                        a[i,j].y = 2*AP.Math.RandomReal()-1;
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
        private static void makeacopy(ref AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[,] b)
        {
            int i = 0;
            int j = 0;

            b = new AP.Complex[m-1+1, n-1+1];
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
        private static void testproblem(ref AP.Complex[,] a,
            int m,
            int n)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            double mx = 0;
            AP.Complex[,] b = new AP.Complex[0,0];
            AP.Complex[] taub = new AP.Complex[0];
            AP.Complex[,] q = new AP.Complex[0,0];
            AP.Complex[,] r = new AP.Complex[0,0];
            AP.Complex[,] q2 = new AP.Complex[0,0];
            AP.Complex v = 0;
            int i_ = 0;

            
            //
            // MX - estimate of the matrix norm
            //
            mx = 0;
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( (double)(AP.Math.AbsComplex(a[i,j]))>(double)(mx) )
                    {
                        mx = AP.Math.AbsComplex(a[i,j]);
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
            cqr.cmatrixqr(ref b, m, n, ref taub);
            cqr.cmatrixqrunpackq(ref b, m, n, ref taub, m, ref q);
            cqr.cmatrixqrunpackr(ref b, m, n, ref r);
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=m-1;i_++)
                    {
                        v += q[i,i_]*r[i_,j];
                    }
                    decomperrors = decomperrors | (double)(AP.Math.AbsComplex(v-a[i,j]))>=(double)(threshold);
                }
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=Math.Min(i, n-1)-1; j++)
                {
                    structerrors = structerrors | r[i,j]!=0;
                }
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=m-1;i_++)
                    {
                        v += q[i,i_]*AP.Math.Conj(q[j,i_]);
                    }
                    if( i==j )
                    {
                        structerrors = structerrors | (double)(AP.Math.AbsComplex(v-1))>=(double)(threshold);
                    }
                    else
                    {
                        structerrors = structerrors | (double)(AP.Math.AbsComplex(v))>=(double)(threshold);
                    }
                }
            }
            
            //
            // Test for other errors
            //
            for(k=1; k<=m-1; k++)
            {
                cqr.cmatrixqrunpackq(ref b, m, n, ref taub, k, ref q2);
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=k-1; j++)
                    {
                        othererrors = othererrors | (double)(AP.Math.AbsComplex(q2[i,j]-q[i,j]))>(double)(10*AP.Math.MachineEpsilon);
                    }
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testcqrunit_test_silent()
        {
            bool result = new bool();

            result = testcqr(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testcqrunit_test()
        {
            bool result = new bool();

            result = testcqr(false);
            return result;
        }
    }
}
