
using System;

namespace alglib
{
    public class testbdunit
    {
        public static bool decomperrors = new bool();
        public static bool properrors = new bool();
        public static bool parterrors = new bool();
        public static bool mulerrors = new bool();
        public static double threshold = 0;


        /*************************************************************************
        Main unittest subroutine
        *************************************************************************/
        public static bool testbd(bool silent)
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
            properrors = false;
            mulerrors = false;
            parterrors = false;
            threshold = 5*100*AP.Math.MachineEpsilon;
            waserrors = false;
            shortmn = 5;
            maxmn = 10;
            gpasscount = 5;
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
            waserrors = decomperrors | properrors | parterrors | mulerrors;
            if( !silent )
            {
                System.Console.Write("TESTING 2BIDIAGONAL");
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
                System.Console.Write("PARTIAL UNPACKING                        ");
                if( parterrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("MULTIPLICATION TEST                      ");
                if( mulerrors )
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
            int m,
            int n)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            double mx = 0;
            double[,] t = new double[0,0];
            double[,] pt = new double[0,0];
            double[,] q = new double[0,0];
            double[,] r = new double[0,0];
            double[,] bd = new double[0,0];
            double[,] x = new double[0,0];
            double[,] r1 = new double[0,0];
            double[,] r2 = new double[0,0];
            double[] taup = new double[0];
            double[] tauq = new double[0];
            double[] d = new double[0];
            double[] e = new double[0];
            bool up = new bool();
            double v = 0;
            int mtsize = 0;
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
            // Bidiagonal decomposition error
            //
            makeacopy(ref a, m, n, ref t);
            bidiagonal.rmatrixbd(ref t, m, n, ref tauq, ref taup);
            bidiagonal.rmatrixbdunpackq(ref t, m, n, ref tauq, m, ref q);
            bidiagonal.rmatrixbdunpackpt(ref t, m, n, ref taup, n, ref pt);
            bidiagonal.rmatrixbdunpackdiagonals(ref t, m, n, ref up, ref d, ref e);
            bd = new double[m-1+1, n-1+1];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    bd[i,j] = 0;
                }
            }
            for(i=0; i<=Math.Min(m, n)-1; i++)
            {
                bd[i,i] = d[i];
            }
            if( up )
            {
                for(i=0; i<=Math.Min(m, n)-2; i++)
                {
                    bd[i,i+1] = e[i];
                }
            }
            else
            {
                for(i=0; i<=Math.Min(m, n)-2; i++)
                {
                    bd[i+1,i] = e[i];
                }
            }
            r = new double[m-1+1, n-1+1];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=m-1;i_++)
                    {
                        v += q[i,i_]*bd[i_,j];
                    }
                    r[i,j] = v;
                }
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += r[i,i_]*pt[i_,j];
                    }
                    decomperrors = decomperrors | (double)(Math.Abs(v-a[i,j]))>(double)(threshold);
                }
            }
            
            //
            // Orthogonality test for Q/PT
            //
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=m-1;i_++)
                    {
                        v += q[i_,i]*q[i_,j];
                    }
                    if( i==j )
                    {
                        properrors = properrors | (double)(Math.Abs(v-1))>(double)(threshold);
                    }
                    else
                    {
                        properrors = properrors | (double)(Math.Abs(v))>(double)(threshold);
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
                        v += pt[i,i_]*pt[j,i_];
                    }
                    if( i==j )
                    {
                        properrors = properrors | (double)(Math.Abs(v-1))>(double)(threshold);
                    }
                    else
                    {
                        properrors = properrors | (double)(Math.Abs(v))>(double)(threshold);
                    }
                }
            }
            
            //
            // Partial unpacking test
            //
            for(k=1; k<=m-1; k++)
            {
                bidiagonal.rmatrixbdunpackq(ref t, m, n, ref tauq, k, ref r);
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=k-1; j++)
                    {
                        parterrors = parterrors | (double)(Math.Abs(r[i,j]-q[i,j]))>(double)(10*AP.Math.MachineEpsilon);
                    }
                }
            }
            for(k=1; k<=n-1; k++)
            {
                bidiagonal.rmatrixbdunpackpt(ref t, m, n, ref taup, k, ref r);
                for(i=0; i<=k-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        parterrors = parterrors | (double)(r[i,j]-pt[i,j])!=(double)(0);
                    }
                }
            }
            
            //
            // Multiplication test
            //
            x = new double[Math.Max(m, n)-1+1, Math.Max(m, n)-1+1];
            r = new double[Math.Max(m, n)-1+1, Math.Max(m, n)-1+1];
            r1 = new double[Math.Max(m, n)-1+1, Math.Max(m, n)-1+1];
            r2 = new double[Math.Max(m, n)-1+1, Math.Max(m, n)-1+1];
            for(i=0; i<=Math.Max(m, n)-1; i++)
            {
                for(j=0; j<=Math.Max(m, n)-1; j++)
                {
                    x[i,j] = 2*AP.Math.RandomReal()-1;
                }
            }
            mtsize = 1+AP.Math.RandomInteger(Math.Max(m, n));
            makeacopyoldmem(ref x, mtsize, m, ref r);
            internalmatrixmatrixmultiply(ref r, 0, mtsize-1, 0, m-1, false, ref q, 0, m-1, 0, m-1, false, ref r1, 0, mtsize-1, 0, m-1);
            makeacopyoldmem(ref x, mtsize, m, ref r2);
            bidiagonal.rmatrixbdmultiplybyq(ref t, m, n, ref tauq, ref r2, mtsize, m, true, false);
            mulerrors = mulerrors | (double)(matrixdiff(ref r1, ref r2, mtsize, m))>(double)(threshold);
            makeacopyoldmem(ref x, mtsize, m, ref r);
            internalmatrixmatrixmultiply(ref r, 0, mtsize-1, 0, m-1, false, ref q, 0, m-1, 0, m-1, true, ref r1, 0, mtsize-1, 0, m-1);
            makeacopyoldmem(ref x, mtsize, m, ref r2);
            bidiagonal.rmatrixbdmultiplybyq(ref t, m, n, ref tauq, ref r2, mtsize, m, true, true);
            mulerrors = mulerrors | (double)(matrixdiff(ref r1, ref r2, mtsize, m))>(double)(threshold);
            makeacopyoldmem(ref x, m, mtsize, ref r);
            internalmatrixmatrixmultiply(ref q, 0, m-1, 0, m-1, false, ref r, 0, m-1, 0, mtsize-1, false, ref r1, 0, m-1, 0, mtsize-1);
            makeacopyoldmem(ref x, m, mtsize, ref r2);
            bidiagonal.rmatrixbdmultiplybyq(ref t, m, n, ref tauq, ref r2, m, mtsize, false, false);
            mulerrors = mulerrors | (double)(matrixdiff(ref r1, ref r2, m, mtsize))>(double)(threshold);
            makeacopyoldmem(ref x, m, mtsize, ref r);
            internalmatrixmatrixmultiply(ref q, 0, m-1, 0, m-1, true, ref r, 0, m-1, 0, mtsize-1, false, ref r1, 0, m-1, 0, mtsize-1);
            makeacopyoldmem(ref x, m, mtsize, ref r2);
            bidiagonal.rmatrixbdmultiplybyq(ref t, m, n, ref tauq, ref r2, m, mtsize, false, true);
            mulerrors = mulerrors | (double)(matrixdiff(ref r1, ref r2, m, mtsize))>(double)(threshold);
            makeacopyoldmem(ref x, mtsize, n, ref r);
            internalmatrixmatrixmultiply(ref r, 0, mtsize-1, 0, n-1, false, ref pt, 0, n-1, 0, n-1, true, ref r1, 0, mtsize-1, 0, n-1);
            makeacopyoldmem(ref x, mtsize, n, ref r2);
            bidiagonal.rmatrixbdmultiplybyp(ref t, m, n, ref taup, ref r2, mtsize, n, true, false);
            mulerrors = mulerrors | (double)(matrixdiff(ref r1, ref r2, mtsize, n))>(double)(threshold);
            makeacopyoldmem(ref x, mtsize, n, ref r);
            internalmatrixmatrixmultiply(ref r, 0, mtsize-1, 0, n-1, false, ref pt, 0, n-1, 0, n-1, false, ref r1, 0, mtsize-1, 0, n-1);
            makeacopyoldmem(ref x, mtsize, n, ref r2);
            bidiagonal.rmatrixbdmultiplybyp(ref t, m, n, ref taup, ref r2, mtsize, n, true, true);
            mulerrors = mulerrors | (double)(matrixdiff(ref r1, ref r2, mtsize, n))>(double)(threshold);
            makeacopyoldmem(ref x, n, mtsize, ref r);
            internalmatrixmatrixmultiply(ref pt, 0, n-1, 0, n-1, true, ref r, 0, n-1, 0, mtsize-1, false, ref r1, 0, n-1, 0, mtsize-1);
            makeacopyoldmem(ref x, n, mtsize, ref r2);
            bidiagonal.rmatrixbdmultiplybyp(ref t, m, n, ref taup, ref r2, n, mtsize, false, false);
            mulerrors = mulerrors | (double)(matrixdiff(ref r1, ref r2, n, mtsize))>(double)(threshold);
            makeacopyoldmem(ref x, n, mtsize, ref r);
            internalmatrixmatrixmultiply(ref pt, 0, n-1, 0, n-1, false, ref r, 0, n-1, 0, mtsize-1, false, ref r1, 0, n-1, 0, mtsize-1);
            makeacopyoldmem(ref x, n, mtsize, ref r2);
            bidiagonal.rmatrixbdmultiplybyp(ref t, m, n, ref taup, ref r2, n, mtsize, false, true);
            mulerrors = mulerrors | (double)(matrixdiff(ref r1, ref r2, n, mtsize))>(double)(threshold);
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
        public static bool testbdunit_test_silent()
        {
            bool result = new bool();

            result = testbd(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testbdunit_test()
        {
            bool result = new bool();

            result = testbd(false);
            return result;
        }
    }
}
