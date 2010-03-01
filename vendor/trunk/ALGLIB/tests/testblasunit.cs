
using System;

namespace alglib
{
    public class testblasunit
    {
        public static bool testblas(bool silent)
        {
            bool result = new bool();
            int pass = 0;
            int passcount = 0;
            int n = 0;
            int i = 0;
            int i1 = 0;
            int i2 = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            int l = 0;
            int k = 0;
            int r = 0;
            int i3 = 0;
            int j3 = 0;
            int col1 = 0;
            int col2 = 0;
            int row1 = 0;
            int row2 = 0;
            double[] x1 = new double[0];
            double[] x2 = new double[0];
            double[,] a = new double[0,0];
            double[,] b = new double[0,0];
            double[,] c1 = new double[0,0];
            double[,] c2 = new double[0,0];
            double err = 0;
            double e1 = 0;
            double e2 = 0;
            double e3 = 0;
            double v = 0;
            double scl1 = 0;
            double scl2 = 0;
            double scl3 = 0;
            bool was1 = new bool();
            bool was2 = new bool();
            bool trans1 = new bool();
            bool trans2 = new bool();
            double threshold = 0;
            bool n2errors = new bool();
            bool hsnerrors = new bool();
            bool amaxerrors = new bool();
            bool mverrors = new bool();
            bool iterrors = new bool();
            bool cterrors = new bool();
            bool mmerrors = new bool();
            bool waserrors = new bool();
            int i_ = 0;

            n2errors = false;
            amaxerrors = false;
            hsnerrors = false;
            mverrors = false;
            iterrors = false;
            cterrors = false;
            mmerrors = false;
            waserrors = false;
            threshold = 10000*AP.Math.MachineEpsilon;
            
            //
            // Test Norm2
            //
            passcount = 1000;
            e1 = 0;
            e2 = 0;
            e3 = 0;
            scl2 = 0.5*AP.Math.MaxRealNumber;
            scl3 = 2*AP.Math.MinRealNumber;
            for(pass=1; pass<=passcount; pass++)
            {
                n = 1+AP.Math.RandomInteger(1000);
                i1 = AP.Math.RandomInteger(10);
                i2 = n+i1-1;
                x1 = new double[i2+1];
                x2 = new double[i2+1];
                for(i=i1; i<=i2; i++)
                {
                    x1[i] = 2*AP.Math.RandomReal()-1;
                }
                v = 0;
                for(i=i1; i<=i2; i++)
                {
                    v = v+AP.Math.Sqr(x1[i]);
                }
                v = Math.Sqrt(v);
                e1 = Math.Max(e1, Math.Abs(v-blas.vectornorm2(ref x1, i1, i2)));
                for(i=i1; i<=i2; i++)
                {
                    x2[i] = scl2*x1[i];
                }
                e2 = Math.Max(e2, Math.Abs(v*scl2-blas.vectornorm2(ref x2, i1, i2)));
                for(i=i1; i<=i2; i++)
                {
                    x2[i] = scl3*x1[i];
                }
                e3 = Math.Max(e3, Math.Abs(v*scl3-blas.vectornorm2(ref x2, i1, i2)));
            }
            e2 = e2/scl2;
            e3 = e3/scl3;
            n2errors = (double)(e1)>=(double)(threshold) | (double)(e2)>=(double)(threshold) | (double)(e3)>=(double)(threshold);
            
            //
            // Testing VectorAbsMax, Column/Row AbsMax
            //
            x1 = new double[5+1];
            x1[1] = 2.0;
            x1[2] = 0.2;
            x1[3] = -1.3;
            x1[4] = 0.7;
            x1[5] = -3.0;
            amaxerrors = blas.vectoridxabsmax(ref x1, 1, 5)!=5 | blas.vectoridxabsmax(ref x1, 1, 4)!=1 | blas.vectoridxabsmax(ref x1, 2, 4)!=3;
            n = 30;
            x1 = new double[n+1];
            a = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    a[i,j] = 2*AP.Math.RandomReal()-1;
                }
            }
            was1 = false;
            was2 = false;
            for(pass=1; pass<=1000; pass++)
            {
                j = 1+AP.Math.RandomInteger(n);
                i1 = 1+AP.Math.RandomInteger(n);
                i2 = i1+AP.Math.RandomInteger(n+1-i1);
                for(i_=i1; i_<=i2;i_++)
                {
                    x1[i_] = a[i_,j];
                }
                if( blas.vectoridxabsmax(ref x1, i1, i2)!=blas.columnidxabsmax(ref a, i1, i2, j) )
                {
                    was1 = true;
                }
                i = 1+AP.Math.RandomInteger(n);
                j1 = 1+AP.Math.RandomInteger(n);
                j2 = j1+AP.Math.RandomInteger(n+1-j1);
                for(i_=j1; i_<=j2;i_++)
                {
                    x1[i_] = a[i,i_];
                }
                if( blas.vectoridxabsmax(ref x1, j1, j2)!=blas.rowidxabsmax(ref a, j1, j2, i) )
                {
                    was2 = true;
                }
            }
            amaxerrors = amaxerrors | was1 | was2;
            
            //
            // Testing upper Hessenberg 1-norm
            //
            a = new double[3+1, 3+1];
            x1 = new double[3+1];
            a[1,1] = 2;
            a[1,2] = 3;
            a[1,3] = 1;
            a[2,1] = 4;
            a[2,2] = -5;
            a[2,3] = 8;
            a[3,1] = 99;
            a[3,2] = 3;
            a[3,3] = 1;
            hsnerrors = (double)(Math.Abs(blas.upperhessenberg1norm(ref a, 1, 3, 1, 3, ref x1)-11))>(double)(threshold);
            
            //
            // Testing MatrixVectorMultiply
            //
            a = new double[3+1, 5+1];
            x1 = new double[3+1];
            x2 = new double[2+1];
            a[2,3] = 2;
            a[2,4] = -1;
            a[2,5] = -1;
            a[3,3] = 1;
            a[3,4] = -2;
            a[3,5] = 2;
            x1[1] = 1;
            x1[2] = 2;
            x1[3] = 1;
            x2[1] = -1;
            x2[2] = -1;
            blas.matrixvectormultiply(ref a, 2, 3, 3, 5, false, ref x1, 1, 3, 1.0, ref x2, 1, 2, 1.0);
            blas.matrixvectormultiply(ref a, 2, 3, 3, 5, true, ref x2, 1, 2, 1.0, ref x1, 1, 3, 1.0);
            e1 = Math.Abs(x1[1]+5)+Math.Abs(x1[2]-8)+Math.Abs(x1[3]+1)+Math.Abs(x2[1]+2)+Math.Abs(x2[2]+2);
            x1[1] = 1;
            x1[2] = 2;
            x1[3] = 1;
            x2[1] = -1;
            x2[2] = -1;
            blas.matrixvectormultiply(ref a, 2, 3, 3, 5, false, ref x1, 1, 3, 1.0, ref x2, 1, 2, 0.0);
            blas.matrixvectormultiply(ref a, 2, 3, 3, 5, true, ref x2, 1, 2, 1.0, ref x1, 1, 3, 0.0);
            e2 = Math.Abs(x1[1]+3)+Math.Abs(x1[2]-3)+Math.Abs(x1[3]+1)+Math.Abs(x2[1]+1)+Math.Abs(x2[2]+1);
            mverrors = (double)(e1+e2)>=(double)(threshold);
            
            //
            // testing inplace transpose
            //
            n = 10;
            a = new double[n+1, n+1];
            b = new double[n+1, n+1];
            x1 = new double[n-1+1];
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    a[i,j] = AP.Math.RandomReal();
                }
            }
            passcount = 10000;
            was1 = false;
            for(pass=1; pass<=passcount; pass++)
            {
                i1 = 1+AP.Math.RandomInteger(n);
                i2 = i1+AP.Math.RandomInteger(n-i1+1);
                j1 = 1+AP.Math.RandomInteger(n-(i2-i1));
                j2 = j1+(i2-i1);
                blas.copymatrix(ref a, i1, i2, j1, j2, ref b, i1, i2, j1, j2);
                blas.inplacetranspose(ref b, i1, i2, j1, j2, ref x1);
                for(i=i1; i<=i2; i++)
                {
                    for(j=j1; j<=j2; j++)
                    {
                        if( (double)(a[i,j])!=(double)(b[i1+(j-j1),j1+(i-i1)]) )
                        {
                            was1 = true;
                        }
                    }
                }
            }
            iterrors = was1;
            
            //
            // testing copy and transpose
            //
            n = 10;
            a = new double[n+1, n+1];
            b = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    a[i,j] = AP.Math.RandomReal();
                }
            }
            passcount = 10000;
            was1 = false;
            for(pass=1; pass<=passcount; pass++)
            {
                i1 = 1+AP.Math.RandomInteger(n);
                i2 = i1+AP.Math.RandomInteger(n-i1+1);
                j1 = 1+AP.Math.RandomInteger(n);
                j2 = j1+AP.Math.RandomInteger(n-j1+1);
                blas.copyandtranspose(ref a, i1, i2, j1, j2, ref b, j1, j2, i1, i2);
                for(i=i1; i<=i2; i++)
                {
                    for(j=j1; j<=j2; j++)
                    {
                        if( (double)(a[i,j])!=(double)(b[j,i]) )
                        {
                            was1 = true;
                        }
                    }
                }
            }
            cterrors = was1;
            
            //
            // Testing MatrixMatrixMultiply
            //
            n = 10;
            a = new double[2*n+1, 2*n+1];
            b = new double[2*n+1, 2*n+1];
            c1 = new double[2*n+1, 2*n+1];
            c2 = new double[2*n+1, 2*n+1];
            x1 = new double[n+1];
            x2 = new double[n+1];
            for(i=1; i<=2*n; i++)
            {
                for(j=1; j<=2*n; j++)
                {
                    a[i,j] = AP.Math.RandomReal();
                    b[i,j] = AP.Math.RandomReal();
                }
            }
            passcount = 1000;
            was1 = false;
            for(pass=1; pass<=passcount; pass++)
            {
                for(i=1; i<=2*n; i++)
                {
                    for(j=1; j<=2*n; j++)
                    {
                        c1[i,j] = 2.1*i+3.1*j;
                        c2[i,j] = c1[i,j];
                    }
                }
                l = 1+AP.Math.RandomInteger(n);
                k = 1+AP.Math.RandomInteger(n);
                r = 1+AP.Math.RandomInteger(n);
                i1 = 1+AP.Math.RandomInteger(n);
                j1 = 1+AP.Math.RandomInteger(n);
                i2 = 1+AP.Math.RandomInteger(n);
                j2 = 1+AP.Math.RandomInteger(n);
                i3 = 1+AP.Math.RandomInteger(n);
                j3 = 1+AP.Math.RandomInteger(n);
                trans1 = (double)(AP.Math.RandomReal())>(double)(0.5);
                trans2 = (double)(AP.Math.RandomReal())>(double)(0.5);
                if( trans1 )
                {
                    col1 = l;
                    row1 = k;
                }
                else
                {
                    col1 = k;
                    row1 = l;
                }
                if( trans2 )
                {
                    col2 = k;
                    row2 = r;
                }
                else
                {
                    col2 = r;
                    row2 = k;
                }
                scl1 = AP.Math.RandomReal();
                scl2 = AP.Math.RandomReal();
                blas.matrixmatrixmultiply(ref a, i1, i1+row1-1, j1, j1+col1-1, trans1, ref b, i2, i2+row2-1, j2, j2+col2-1, trans2, scl1, ref c1, i3, i3+l-1, j3, j3+r-1, scl2, ref x1);
                naivematrixmatrixmultiply(ref a, i1, i1+row1-1, j1, j1+col1-1, trans1, ref b, i2, i2+row2-1, j2, j2+col2-1, trans2, scl1, ref c2, i3, i3+l-1, j3, j3+r-1, scl2);
                err = 0;
                for(i=1; i<=l; i++)
                {
                    for(j=1; j<=r; j++)
                    {
                        err = Math.Max(err, Math.Abs(c1[i3+i-1,j3+j-1]-c2[i3+i-1,j3+j-1]));
                    }
                }
                if( (double)(err)>(double)(threshold) )
                {
                    was1 = true;
                    break;
                }
            }
            mmerrors = was1;
            
            //
            // report
            //
            waserrors = n2errors | amaxerrors | hsnerrors | mverrors | iterrors | cterrors | mmerrors;
            if( !silent )
            {
                System.Console.Write("TESTING BLAS");
                System.Console.WriteLine();
                System.Console.Write("VectorNorm2:                             ");
                if( n2errors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("AbsMax (vector/row/column):              ");
                if( amaxerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("UpperHessenberg1Norm:                    ");
                if( hsnerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("MatrixVectorMultiply:                    ");
                if( mverrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("InplaceTranspose:                        ");
                if( iterrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("CopyAndTranspose:                        ");
                if( cterrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("MatrixMatrixMultiply:                    ");
                if( mmerrors )
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


        private static void naivematrixmatrixmultiply(ref double[,] a,
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
            double alpha,
            ref double[,] c,
            int ci1,
            int ci2,
            int cj1,
            int cj2,
            double beta)
        {
            int arows = 0;
            int acols = 0;
            int brows = 0;
            int bcols = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int l = 0;
            int r = 0;
            double v = 0;
            double[] x1 = new double[0];
            double[] x2 = new double[0];
            int i_ = 0;
            int i1_ = 0;

            
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
            System.Diagnostics.Debug.Assert(acols==brows, "NaiveMatrixMatrixMultiply: incorrect matrix sizes!");
            if( arows<=0 | acols<=0 | brows<=0 | bcols<=0 )
            {
                return;
            }
            l = arows;
            r = bcols;
            k = acols;
            x1 = new double[k+1];
            x2 = new double[k+1];
            for(i=1; i<=l; i++)
            {
                for(j=1; j<=r; j++)
                {
                    if( !transa )
                    {
                        if( !transb )
                        {
                            i1_ = (aj1)-(bi1);
                            v = 0.0;
                            for(i_=bi1; i_<=bi2;i_++)
                            {
                                v += b[i_,bj1+j-1]*a[ai1+i-1,i_+i1_];
                            }
                        }
                        else
                        {
                            i1_ = (aj1)-(bj1);
                            v = 0.0;
                            for(i_=bj1; i_<=bj2;i_++)
                            {
                                v += b[bi1+j-1,i_]*a[ai1+i-1,i_+i1_];
                            }
                        }
                    }
                    else
                    {
                        if( !transb )
                        {
                            i1_ = (ai1)-(bi1);
                            v = 0.0;
                            for(i_=bi1; i_<=bi2;i_++)
                            {
                                v += b[i_,bj1+j-1]*a[i_+i1_,aj1+i-1];
                            }
                        }
                        else
                        {
                            i1_ = (ai1)-(bj1);
                            v = 0.0;
                            for(i_=bj1; i_<=bj2;i_++)
                            {
                                v += b[bi1+j-1,i_]*a[i_+i1_,aj1+i-1];
                            }
                        }
                    }
                    if( (double)(beta)==(double)(0) )
                    {
                        c[ci1+i-1,cj1+j-1] = alpha*v;
                    }
                    else
                    {
                        c[ci1+i-1,cj1+j-1] = beta*c[ci1+i-1,cj1+j-1]+alpha*v;
                    }
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testblasunit_test_silent()
        {
            bool result = new bool();

            result = testblas(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testblasunit_test()
        {
            bool result = new bool();

            result = testblas(false);
            return result;
        }
    }
}
