
using System;

namespace alglib
{
    public class testcinvunit
    {
        public static bool inverrors = new bool();
        public static double threshold = 0;


        /*************************************************************************
        Main unittest subroutine
        *************************************************************************/
        public static bool testcinv(bool silent)
        {
            bool result = new bool();
            int maxn = 0;
            int gpasscount = 0;
            AP.Complex[,] a = new AP.Complex[0,0];
            int n = 0;
            int gpass = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            bool waserrors = new bool();

            inverrors = false;
            threshold = 5*1000*AP.Math.MachineEpsilon;
            waserrors = false;
            maxn = 10;
            gpasscount = 30;
            
            //
            // Different problems
            //
            for(n=1; n<=maxn; n++)
            {
                a = new AP.Complex[n-1+1, n-1+1];
                for(gpass=1; gpass<=gpasscount; gpass++)
                {
                    
                    //
                    // diagonal matrix, several cases
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        a[i,i].x = 2*AP.Math.RandomReal()-1;
                        a[i,i].y = 2*AP.Math.RandomReal()-1;
                    }
                    testproblem(ref a, n);
                    
                    //
                    // shifted diagonal matrix, several cases
                    //
                    k = AP.Math.RandomInteger(n);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        a[i,(i+k)%n].x = 2*AP.Math.RandomReal()-1;
                        a[i,(i+k)%n].y = 2*AP.Math.RandomReal()-1;
                    }
                    testproblem(ref a, n);
                    
                    //
                    // Dense matrices
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j].x = 2*AP.Math.RandomReal()-1;
                            a[i,j].y = 2*AP.Math.RandomReal()-1;
                        }
                    }
                    testproblem(ref a, n);
                }
            }
            
            //
            // report
            //
            waserrors = inverrors;
            if( !silent )
            {
                System.Console.Write("TESTING COMPLEX INVERSE");
                System.Console.WriteLine();
                System.Console.Write("INVERSE ERRORS                           ");
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
        Problem testing
        *************************************************************************/
        private static void testproblem(ref AP.Complex[,] a,
            int n)
        {
            AP.Complex[,] b = new AP.Complex[0,0];
            AP.Complex[,] blu = new AP.Complex[0,0];
            AP.Complex[,] t1 = new AP.Complex[0,0];
            int[] p = new int[0];
            int i = 0;
            int j = 0;
            AP.Complex v = 0;

            
            //
            // Decomposition
            //
            makeacopy(ref a, n, n, ref b);
            cinverse.cmatrixinverse(ref b, n);
            makeacopy(ref a, n, n, ref blu);
            trfac.cmatrixlu(ref blu, n, n, ref p);
            cinverse.cmatrixluinverse(ref blu, ref p, n);
            
            //
            // Test
            //
            t1 = new AP.Complex[n-1+1, n-1+1];
            internalmatrixmatrixmultiply(ref a, 0, n-1, 0, n-1, false, ref b, 0, n-1, 0, n-1, false, ref t1, 0, n-1, 0, n-1);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = t1[i,j];
                    if( i==j )
                    {
                        v = v-1;
                    }
                    inverrors = inverrors | (double)(AP.Math.AbsComplex(v))>(double)(threshold);
                }
            }
            internalmatrixmatrixmultiply(ref a, 0, n-1, 0, n-1, false, ref blu, 0, n-1, 0, n-1, false, ref t1, 0, n-1, 0, n-1);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = t1[i,j];
                    if( i==j )
                    {
                        v = v-1;
                    }
                    inverrors = inverrors | (double)(AP.Math.AbsComplex(v))>(double)(threshold);
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
        Copy
        *************************************************************************/
        private static void makeacopyoldmem(ref AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[,] b)
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
        private static double matrixdiff(ref AP.Complex[,] a,
            ref AP.Complex[,] b,
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
                    result = Math.Max(result, AP.Math.AbsComplex(b[i,j]-a[i,j]));
                }
            }
            return result;
        }


        /*************************************************************************
        Matrix multiplication
        *************************************************************************/
        private static void internalmatrixmatrixmultiply(ref AP.Complex[,] a,
            int ai1,
            int ai2,
            int aj1,
            int aj2,
            bool transa,
            ref AP.Complex[,] b,
            int bi1,
            int bi2,
            int bj1,
            int bj2,
            bool transb,
            ref AP.Complex[,] c,
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
            AP.Complex v = 0;
            AP.Complex[] work = new AP.Complex[0];
            AP.Complex beta = 0;
            AP.Complex alpha = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Pre-setup
            //
            k = Math.Max(ai2-ai1+1, aj2-aj1+1);
            k = Math.Max(k, bi2-bi1+1);
            k = Math.Max(k, bj2-bj1+1);
            work = new AP.Complex[k+1];
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
            if( beta==0 )
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
        public static bool testcinvunit_test_silent()
        {
            bool result = new bool();

            result = testcinv(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testcinvunit_test()
        {
            bool result = new bool();

            result = testcinv(false);
            return result;
        }
    }
}
