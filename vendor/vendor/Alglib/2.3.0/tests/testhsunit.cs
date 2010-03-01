
using System;

namespace alglib
{
    public class testhsunit
    {
        public static bool decomperrors = new bool();
        public static bool properrors = new bool();
        public static double threshold = 0;


        /*************************************************************************
        Main unittest subroutine
        *************************************************************************/
        public static bool tesths(bool silent)
        {
            bool result = new bool();
            int maxn = 0;
            int gpasscount = 0;
            double[,] a = new double[0,0];
            int n = 0;
            int gpass = 0;
            int i = 0;
            int j = 0;
            bool waserrors = new bool();

            decomperrors = false;
            properrors = false;
            threshold = 5*100*AP.Math.MachineEpsilon;
            waserrors = false;
            maxn = 10;
            gpasscount = 30;
            
            //
            // Different problems
            //
            for(n=1; n<=maxn; n++)
            {
                a = new double[n-1+1, n-1+1];
                for(gpass=1; gpass<=gpasscount; gpass++)
                {
                    
                    //
                    // zero matrix, several cases
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                    testproblem(ref a, n);
                    
                    //
                    // Dense matrices
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 2*AP.Math.RandomReal()-1;
                        }
                    }
                    testproblem(ref a, n);
                    
                    //
                    // Sparse matrices, very sparse matrices, incredible sparse matrices
                    //
                    fillsparsea(ref a, n, n, 0.8);
                    testproblem(ref a, n);
                    fillsparsea(ref a, n, n, 0.9);
                    testproblem(ref a, n);
                    fillsparsea(ref a, n, n, 0.95);
                    testproblem(ref a, n);
                }
            }
            
            //
            // report
            //
            waserrors = decomperrors | properrors;
            if( !silent )
            {
                System.Console.Write("TESTING 2HESSENBERG");
                System.Console.WriteLine();
                System.Console.Write("DECOMPOSITION ERRORS                     ");
                if( decomperrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("MATRIX PROPERTIES                        ");
                if( properrors )
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
        Sparse matrix
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
        Problem testing
        *************************************************************************/
        private static void testproblem(ref double[,] a,
            int n)
        {
            double[,] b = new double[0,0];
            double[,] h = new double[0,0];
            double[,] q = new double[0,0];
            double[,] t1 = new double[0,0];
            double[,] t2 = new double[0,0];
            double[] tau = new double[0];
            int i = 0;
            int j = 0;
            double v = 0;
            int i_ = 0;

            makeacopy(ref a, n, n, ref b);
            
            //
            // Decomposition
            //
            hessenberg.rmatrixhessenberg(ref b, n, ref tau);
            hessenberg.rmatrixhessenbergunpackq(ref b, n, ref tau, ref q);
            hessenberg.rmatrixhessenbergunpackh(ref b, n, ref h);
            
            //
            // Matrix properties
            //
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += q[i_,i]*q[i_,j];
                    }
                    if( i==j )
                    {
                        v = v-1;
                    }
                    properrors = properrors | (double)(Math.Abs(v))>(double)(threshold);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=i-2; j++)
                {
                    properrors = properrors | (double)(h[i,j])!=(double)(0);
                }
            }
            
            //
            // Decomposition error
            //
            t1 = new double[n-1+1, n-1+1];
            t2 = new double[n-1+1, n-1+1];
            internalmatrixmatrixmultiply(ref q, 0, n-1, 0, n-1, false, ref h, 0, n-1, 0, n-1, false, ref t1, 0, n-1, 0, n-1);
            internalmatrixmatrixmultiply(ref t1, 0, n-1, 0, n-1, false, ref q, 0, n-1, 0, n-1, true, ref t2, 0, n-1, 0, n-1);
            decomperrors = decomperrors | (double)(matrixdiff(ref t2, ref a, n, n))>(double)(threshold);
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
        Copy
        *************************************************************************/
        private static void makeacopyoldmem(ref double[,] a,
            int m,
            int n,
            ref double[,] b)
        {
            int i = 0;
            int j = 0;

            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    b[i,j] = a[i,j];
                }
            }
        }


        /*************************************************************************
        Diff
        *************************************************************************/
        private static double matrixdiff(ref double[,] a,
            ref double[,] b,
            int m,
            int n)
        {
            double result = 0;
            int i = 0;
            int j = 0;

            result = 0;
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    result = Math.Max(result, Math.Abs(b[i,j]-a[i,j]));
                }
            }
            return result;
        }


        /*************************************************************************
        Matrix multiplication
        *************************************************************************/
        private static void internalmatrixmatrixmultiply(ref double[,] a,
            int ai1,
            int ai2,
            int aj1,
            int aj2,
            bool transa,
            ref double[,] b,
            int bi1,
            int bi2,
            int bj1,
            int bj2,
            bool transb,
            ref double[,] c,
            int ci1,
            int ci2,
            int cj1,
            int cj2)
        {
            int arows = 0;
            int acols = 0;
            int brows = 0;
            int bcols = 0;
            int crows = 0;
            int ccols = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int l = 0;
            int r = 0;
            double v = 0;
            double[] work = new double[0];
            double beta = 0;
            double alpha = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Pre-setup
            //
            k = Math.Max(ai2-ai1+1, aj2-aj1+1);
            k = Math.Max(k, bi2-bi1+1);
            k = Math.Max(k, bj2-bj1+1);
            work = new double[k+1];
            beta = 0;
            alpha = 1;
            
            //
            // Setup
            //
            if( !transa )
            {
                arows = ai2-ai1+1;
                acols = aj2-aj1+1;
            }
            else
            {
                arows = aj2-aj1+1;
                acols = ai2-ai1+1;
            }
            if( !transb )
            {
                brows = bi2-bi1+1;
                bcols = bj2-bj1+1;
            }
            else
            {
                brows = bj2-bj1+1;
                bcols = bi2-bi1+1;
            }
            System.Diagnostics.Debug.Assert(acols==brows, "MatrixMatrixMultiply: incorrect matrix sizes!");
            if( arows<=0 | acols<=0 | brows<=0 | bcols<=0 )
            {
                return;
            }
            crows = arows;
            ccols = bcols;
            
            //
            // Test WORK
            //
            i = Math.Max(arows, acols);
            i = Math.Max(brows, i);
            i = Math.Max(i, bcols);
            work[1] = 0;
            work[i] = 0;
            
            //
            // Prepare C
            //
            if( (double)(beta)==(double)(0) )
            {
                for(i=ci1; i<=ci2; i++)
                {
                    for(j=cj1; j<=cj2; j++)
                    {
                        c[i,j] = 0;
                    }
                }
            }
            else
            {
                for(i=ci1; i<=ci2; i++)
                {
                    for(i_=cj1; i_<=cj2;i_++)
                    {
                        c[i,i_] = beta*c[i,i_];
                    }
                }
            }
            
            //
            // A*B
            //
            if( !transa & !transb )
            {
                for(l=ai1; l<=ai2; l++)
                {
                    for(r=bi1; r<=bi2; r++)
                    {
                        v = alpha*a[l,aj1+r-bi1];
                        k = ci1+l-ai1;
                        i1_ = (bj1) - (cj1);
                        for(i_=cj1; i_<=cj2;i_++)
                        {
                            c[k,i_] = c[k,i_] + v*b[r,i_+i1_];
                        }
                    }
                }
                return;
            }
            
            //
            // A*B'
            //
            if( !transa & transb )
            {
                if( arows*acols<brows*bcols )
                {
                    for(r=bi1; r<=bi2; r++)
                    {
                        for(l=ai1; l<=ai2; l++)
                        {
                            i1_ = (bj1)-(aj1);
                            v = 0.0;
                            for(i_=aj1; i_<=aj2;i_++)
                            {
                                v += a[l,i_]*b[r,i_+i1_];
                            }
                            c[ci1+l-ai1,cj1+r-bi1] = c[ci1+l-ai1,cj1+r-bi1]+alpha*v;
                        }
                    }
                    return;
                }
                else
                {
                    for(l=ai1; l<=ai2; l++)
                    {
                        for(r=bi1; r<=bi2; r++)
                        {
                            i1_ = (bj1)-(aj1);
                            v = 0.0;
                            for(i_=aj1; i_<=aj2;i_++)
                            {
                                v += a[l,i_]*b[r,i_+i1_];
                            }
                            c[ci1+l-ai1,cj1+r-bi1] = c[ci1+l-ai1,cj1+r-bi1]+alpha*v;
                        }
                    }
                    return;
                }
            }
            
            //
            // A'*B
            //
            if( transa & !transb )
            {
                for(l=aj1; l<=aj2; l++)
                {
                    for(r=bi1; r<=bi2; r++)
                    {
                        v = alpha*a[ai1+r-bi1,l];
                        k = ci1+l-aj1;
                        i1_ = (bj1) - (cj1);
                        for(i_=cj1; i_<=cj2;i_++)
                        {
                            c[k,i_] = c[k,i_] + v*b[r,i_+i1_];
                        }
                    }
                }
                return;
            }
            
            //
            // A'*B'
            //
            if( transa & transb )
            {
                if( arows*acols<brows*bcols )
                {
                    for(r=bi1; r<=bi2; r++)
                    {
                        for(i=1; i<=crows; i++)
                        {
                            work[i] = 0.0;
                        }
                        for(l=ai1; l<=ai2; l++)
                        {
                            v = alpha*b[r,bj1+l-ai1];
                            k = cj1+r-bi1;
                            i1_ = (aj1) - (1);
                            for(i_=1; i_<=crows;i_++)
                            {
                                work[i_] = work[i_] + v*a[l,i_+i1_];
                            }
                        }
                        i1_ = (1) - (ci1);
                        for(i_=ci1; i_<=ci2;i_++)
                        {
                            c[i_,k] = c[i_,k] + work[i_+i1_];
                        }
                    }
                    return;
                }
                else
                {
                    for(l=aj1; l<=aj2; l++)
                    {
                        k = ai2-ai1+1;
                        i1_ = (ai1) - (1);
                        for(i_=1; i_<=k;i_++)
                        {
                            work[i_] = a[i_+i1_,l];
                        }
                        for(r=bi1; r<=bi2; r++)
                        {
                            i1_ = (bj1)-(1);
                            v = 0.0;
                            for(i_=1; i_<=k;i_++)
                            {
                                v += work[i_]*b[r,i_+i1_];
                            }
                            c[ci1+l-aj1,cj1+r-bi1] = c[ci1+l-aj1,cj1+r-bi1]+alpha*v;
                        }
                    }
                    return;
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testhsunit_test_silent()
        {
            bool result = new bool();

            result = tesths(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testhsunit_test()
        {
            bool result = new bool();

            result = tesths(false);
            return result;
        }
    }
}
