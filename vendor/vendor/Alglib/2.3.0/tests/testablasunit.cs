
using System;

namespace alglib
{
    public class testablasunit
    {
        public static bool testablas(bool silent)
        {
            bool result = new bool();
            double threshold = 0;
            bool trsmerrors = new bool();
            bool syrkerrors = new bool();
            bool gemmerrors = new bool();
            bool transerrors = new bool();
            bool rank1errors = new bool();
            bool mverrors = new bool();
            bool waserrors = new bool();
            double[,] ra = new double[0,0];

            trsmerrors = false;
            syrkerrors = false;
            gemmerrors = false;
            transerrors = false;
            rank1errors = false;
            mverrors = false;
            waserrors = false;
            threshold = 10000*AP.Math.MachineEpsilon;
            trsmerrors = trsmerrors | testtrsm(1, 3*ablas.ablasblocksize(ref ra));
            syrkerrors = syrkerrors | testsyrk(1, 3*ablas.ablasblocksize(ref ra));
            gemmerrors = gemmerrors | testgemm(1, 3*ablas.ablasblocksize(ref ra));
            transerrors = transerrors | testtrans(1, 3*ablas.ablasblocksize(ref ra));
            rank1errors = rank1errors | testrank1(1, 3*ablas.ablasblocksize(ref ra));
            mverrors = mverrors | testmv(1, 3*ablas.ablasblocksize(ref ra));
            
            //
            // report
            //
            waserrors = trsmerrors | syrkerrors | gemmerrors | transerrors | rank1errors | mverrors;
            if( !silent )
            {
                System.Console.Write("TESTING ABLAS");
                System.Console.WriteLine();
                System.Console.Write("* TRSM:                                  ");
                if( trsmerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* SYRK:                                  ");
                if( syrkerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* GEMM:                                  ");
                if( gemmerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* TRANS:                                 ");
                if( transerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* RANK1:                                 ");
                if( rank1errors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* MV:                                    ");
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
        Reference implementation

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void refcmatrixrighttrsm(int m,
            int n,
            ref AP.Complex[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref AP.Complex[,] x,
            int i2,
            int j2)
        {
            AP.Complex[,] a1 = new AP.Complex[0,0];
            AP.Complex[,] a2 = new AP.Complex[0,0];
            AP.Complex[] tx = new AP.Complex[0];
            int i = 0;
            int j = 0;
            AP.Complex vc = 0;
            bool rupper = new bool();
            int i_ = 0;
            int i1_ = 0;

            if( n*m==0 )
            {
                return;
            }
            a1 = new AP.Complex[n, n];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a1[i,j] = 0;
                }
            }
            if( isupper )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=i; j<=n-1; j++)
                    {
                        a1[i,j] = a[i1+i,j1+j];
                    }
                }
            }
            else
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=i; j++)
                    {
                        a1[i,j] = a[i1+i,j1+j];
                    }
                }
            }
            rupper = isupper;
            if( isunit )
            {
                for(i=0; i<=n-1; i++)
                {
                    a1[i,i] = 1;
                }
            }
            a2 = new AP.Complex[n, n];
            if( optype==0 )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a2[i,j] = a1[i,j];
                    }
                }
            }
            if( optype==1 )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a2[i,j] = a1[j,i];
                    }
                }
                rupper = !rupper;
            }
            if( optype==2 )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a2[i,j] = AP.Math.Conj(a1[j,i]);
                    }
                }
                rupper = !rupper;
            }
            internalcmatrixtrinverse(ref a2, n, rupper, false);
            tx = new AP.Complex[n];
            for(i=0; i<=m-1; i++)
            {
                i1_ = (j2) - (0);
                for(i_=0; i_<=n-1;i_++)
                {
                    tx[i_] = x[i2+i,i_+i1_];
                }
                for(j=0; j<=n-1; j++)
                {
                    vc = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        vc += tx[i_]*a2[i_,j];
                    }
                    x[i2+i,j2+j] = vc;
                }
            }
        }


        /*************************************************************************
        Reference implementation

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void refcmatrixlefttrsm(int m,
            int n,
            ref AP.Complex[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref AP.Complex[,] x,
            int i2,
            int j2)
        {
            AP.Complex[,] a1 = new AP.Complex[0,0];
            AP.Complex[,] a2 = new AP.Complex[0,0];
            AP.Complex[] tx = new AP.Complex[0];
            int i = 0;
            int j = 0;
            AP.Complex vc = 0;
            bool rupper = new bool();
            int i_ = 0;
            int i1_ = 0;

            if( n*m==0 )
            {
                return;
            }
            a1 = new AP.Complex[m, m];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    a1[i,j] = 0;
                }
            }
            if( isupper )
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=i; j<=m-1; j++)
                    {
                        a1[i,j] = a[i1+i,j1+j];
                    }
                }
            }
            else
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=i; j++)
                    {
                        a1[i,j] = a[i1+i,j1+j];
                    }
                }
            }
            rupper = isupper;
            if( isunit )
            {
                for(i=0; i<=m-1; i++)
                {
                    a1[i,i] = 1;
                }
            }
            a2 = new AP.Complex[m, m];
            if( optype==0 )
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        a2[i,j] = a1[i,j];
                    }
                }
            }
            if( optype==1 )
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        a2[i,j] = a1[j,i];
                    }
                }
                rupper = !rupper;
            }
            if( optype==2 )
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        a2[i,j] = AP.Math.Conj(a1[j,i]);
                    }
                }
                rupper = !rupper;
            }
            internalcmatrixtrinverse(ref a2, m, rupper, false);
            tx = new AP.Complex[m];
            for(j=0; j<=n-1; j++)
            {
                i1_ = (i2) - (0);
                for(i_=0; i_<=m-1;i_++)
                {
                    tx[i_] = x[i_+i1_,j2+j];
                }
                for(i=0; i<=m-1; i++)
                {
                    vc = 0.0;
                    for(i_=0; i_<=m-1;i_++)
                    {
                        vc += a2[i,i_]*tx[i_];
                    }
                    x[i2+i,j2+j] = vc;
                }
            }
        }


        /*************************************************************************
        Reference implementation

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void refrmatrixrighttrsm(int m,
            int n,
            ref double[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref double[,] x,
            int i2,
            int j2)
        {
            double[,] a1 = new double[0,0];
            double[,] a2 = new double[0,0];
            double[] tx = new double[0];
            int i = 0;
            int j = 0;
            double vr = 0;
            bool rupper = new bool();
            int i_ = 0;
            int i1_ = 0;

            if( n*m==0 )
            {
                return;
            }
            a1 = new double[n, n];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a1[i,j] = 0;
                }
            }
            if( isupper )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=i; j<=n-1; j++)
                    {
                        a1[i,j] = a[i1+i,j1+j];
                    }
                }
            }
            else
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=i; j++)
                    {
                        a1[i,j] = a[i1+i,j1+j];
                    }
                }
            }
            rupper = isupper;
            if( isunit )
            {
                for(i=0; i<=n-1; i++)
                {
                    a1[i,i] = 1;
                }
            }
            a2 = new double[n, n];
            if( optype==0 )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a2[i,j] = a1[i,j];
                    }
                }
            }
            if( optype==1 )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a2[i,j] = a1[j,i];
                    }
                }
                rupper = !rupper;
            }
            internalrmatrixtrinverse(ref a2, n, rupper, false);
            tx = new double[n];
            for(i=0; i<=m-1; i++)
            {
                i1_ = (j2) - (0);
                for(i_=0; i_<=n-1;i_++)
                {
                    tx[i_] = x[i2+i,i_+i1_];
                }
                for(j=0; j<=n-1; j++)
                {
                    vr = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        vr += tx[i_]*a2[i_,j];
                    }
                    x[i2+i,j2+j] = vr;
                }
            }
        }


        /*************************************************************************
        Reference implementation

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void refrmatrixlefttrsm(int m,
            int n,
            ref double[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref double[,] x,
            int i2,
            int j2)
        {
            double[,] a1 = new double[0,0];
            double[,] a2 = new double[0,0];
            double[] tx = new double[0];
            int i = 0;
            int j = 0;
            double vr = 0;
            bool rupper = new bool();
            int i_ = 0;
            int i1_ = 0;

            if( n*m==0 )
            {
                return;
            }
            a1 = new double[m, m];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    a1[i,j] = 0;
                }
            }
            if( isupper )
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=i; j<=m-1; j++)
                    {
                        a1[i,j] = a[i1+i,j1+j];
                    }
                }
            }
            else
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=i; j++)
                    {
                        a1[i,j] = a[i1+i,j1+j];
                    }
                }
            }
            rupper = isupper;
            if( isunit )
            {
                for(i=0; i<=m-1; i++)
                {
                    a1[i,i] = 1;
                }
            }
            a2 = new double[m, m];
            if( optype==0 )
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        a2[i,j] = a1[i,j];
                    }
                }
            }
            if( optype==1 )
            {
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        a2[i,j] = a1[j,i];
                    }
                }
                rupper = !rupper;
            }
            internalrmatrixtrinverse(ref a2, m, rupper, false);
            tx = new double[m];
            for(j=0; j<=n-1; j++)
            {
                i1_ = (i2) - (0);
                for(i_=0; i_<=m-1;i_++)
                {
                    tx[i_] = x[i_+i1_,j2+j];
                }
                for(i=0; i<=m-1; i++)
                {
                    vr = 0.0;
                    for(i_=0; i_<=m-1;i_++)
                    {
                        vr += a2[i,i_]*tx[i_];
                    }
                    x[i2+i,j2+j] = vr;
                }
            }
        }


        /*************************************************************************
        Internal subroutine.
        Triangular matrix inversion

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992
        *************************************************************************/
        public static bool internalcmatrixtrinverse(ref AP.Complex[,] a,
            int n,
            bool isupper,
            bool isunittriangular)
        {
            bool result = new bool();
            bool nounit = new bool();
            int i = 0;
            int j = 0;
            AP.Complex v = 0;
            AP.Complex ajj = 0;
            AP.Complex[] t = new AP.Complex[0];
            int i_ = 0;

            result = true;
            t = new AP.Complex[n-1+1];
            
            //
            // Test the input parameters.
            //
            nounit = !isunittriangular;
            if( isupper )
            {
                
                //
                // Compute inverse of upper triangular matrix.
                //
                for(j=0; j<=n-1; j++)
                {
                    if( nounit )
                    {
                        if( a[j,j]==0 )
                        {
                            result = false;
                            return result;
                        }
                        a[j,j] = 1/a[j,j];
                        ajj = -a[j,j];
                    }
                    else
                    {
                        ajj = -1;
                    }
                    
                    //
                    // Compute elements 1:j-1 of j-th column.
                    //
                    if( j>0 )
                    {
                        for(i_=0; i_<=j-1;i_++)
                        {
                            t[i_] = a[i_,j];
                        }
                        for(i=0; i<=j-1; i++)
                        {
                            if( i+1<j )
                            {
                                v = 0.0;
                                for(i_=i+1; i_<=j-1;i_++)
                                {
                                    v += a[i,i_]*t[i_];
                                }
                            }
                            else
                            {
                                v = 0;
                            }
                            if( nounit )
                            {
                                a[i,j] = v+a[i,i]*t[i];
                            }
                            else
                            {
                                a[i,j] = v+t[i];
                            }
                        }
                        for(i_=0; i_<=j-1;i_++)
                        {
                            a[i_,j] = ajj*a[i_,j];
                        }
                    }
                }
            }
            else
            {
                
                //
                // Compute inverse of lower triangular matrix.
                //
                for(j=n-1; j>=0; j--)
                {
                    if( nounit )
                    {
                        if( a[j,j]==0 )
                        {
                            result = false;
                            return result;
                        }
                        a[j,j] = 1/a[j,j];
                        ajj = -a[j,j];
                    }
                    else
                    {
                        ajj = -1;
                    }
                    if( j+1<n )
                    {
                        
                        //
                        // Compute elements j+1:n of j-th column.
                        //
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            t[i_] = a[i_,j];
                        }
                        for(i=j+1; i<=n-1; i++)
                        {
                            if( i>j+1 )
                            {
                                v = 0.0;
                                for(i_=j+1; i_<=i-1;i_++)
                                {
                                    v += a[i,i_]*t[i_];
                                }
                            }
                            else
                            {
                                v = 0;
                            }
                            if( nounit )
                            {
                                a[i,j] = v+a[i,i]*t[i];
                            }
                            else
                            {
                                a[i,j] = v+t[i];
                            }
                        }
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            a[i_,j] = ajj*a[i_,j];
                        }
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Internal subroutine.
        Triangular matrix inversion

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992
        *************************************************************************/
        public static bool internalrmatrixtrinverse(ref double[,] a,
            int n,
            bool isupper,
            bool isunittriangular)
        {
            bool result = new bool();
            bool nounit = new bool();
            int i = 0;
            int j = 0;
            double v = 0;
            double ajj = 0;
            double[] t = new double[0];
            int i_ = 0;

            result = true;
            t = new double[n-1+1];
            
            //
            // Test the input parameters.
            //
            nounit = !isunittriangular;
            if( isupper )
            {
                
                //
                // Compute inverse of upper triangular matrix.
                //
                for(j=0; j<=n-1; j++)
                {
                    if( nounit )
                    {
                        if( (double)(a[j,j])==(double)(0) )
                        {
                            result = false;
                            return result;
                        }
                        a[j,j] = 1/a[j,j];
                        ajj = -a[j,j];
                    }
                    else
                    {
                        ajj = -1;
                    }
                    
                    //
                    // Compute elements 1:j-1 of j-th column.
                    //
                    if( j>0 )
                    {
                        for(i_=0; i_<=j-1;i_++)
                        {
                            t[i_] = a[i_,j];
                        }
                        for(i=0; i<=j-1; i++)
                        {
                            if( i<j-1 )
                            {
                                v = 0.0;
                                for(i_=i+1; i_<=j-1;i_++)
                                {
                                    v += a[i,i_]*t[i_];
                                }
                            }
                            else
                            {
                                v = 0;
                            }
                            if( nounit )
                            {
                                a[i,j] = v+a[i,i]*t[i];
                            }
                            else
                            {
                                a[i,j] = v+t[i];
                            }
                        }
                        for(i_=0; i_<=j-1;i_++)
                        {
                            a[i_,j] = ajj*a[i_,j];
                        }
                    }
                }
            }
            else
            {
                
                //
                // Compute inverse of lower triangular matrix.
                //
                for(j=n-1; j>=0; j--)
                {
                    if( nounit )
                    {
                        if( (double)(a[j,j])==(double)(0) )
                        {
                            result = false;
                            return result;
                        }
                        a[j,j] = 1/a[j,j];
                        ajj = -a[j,j];
                    }
                    else
                    {
                        ajj = -1;
                    }
                    if( j<n-1 )
                    {
                        
                        //
                        // Compute elements j+1:n of j-th column.
                        //
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            t[i_] = a[i_,j];
                        }
                        for(i=j+1; i<=n-1; i++)
                        {
                            if( i>j+1 )
                            {
                                v = 0.0;
                                for(i_=j+1; i_<=i-1;i_++)
                                {
                                    v += a[i,i_]*t[i_];
                                }
                            }
                            else
                            {
                                v = 0;
                            }
                            if( nounit )
                            {
                                a[i,j] = v+a[i,i]*t[i];
                            }
                            else
                            {
                                a[i,j] = v+t[i];
                            }
                        }
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            a[i_,j] = ajj*a[i_,j];
                        }
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Reference SYRK subroutine.

          -- ALGLIB routine --
             16.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void refcmatrixsyrk(int n,
            int k,
            double alpha,
            ref AP.Complex[,] a,
            int ia,
            int ja,
            int optypea,
            double beta,
            ref AP.Complex[,] c,
            int ic,
            int jc,
            bool isupper)
        {
            AP.Complex[,] ae = new AP.Complex[0,0];
            int i = 0;
            int j = 0;
            AP.Complex vc = 0;
            int i_ = 0;

            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( isupper & j>=i | !isupper & j<=i )
                    {
                        if( (double)(beta)==(double)(0) )
                        {
                            c[i+ic,j+jc] = 0;
                        }
                        else
                        {
                            c[i+ic,j+jc] = c[i+ic,j+jc]*beta;
                        }
                    }
                }
            }
            if( (double)(alpha)==(double)(0) )
            {
                return;
            }
            if( n*k>0 )
            {
                ae = new AP.Complex[n, k];
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=k-1; j++)
                {
                    if( optypea==0 )
                    {
                        ae[i,j] = a[ia+i,ja+j];
                    }
                    if( optypea==2 )
                    {
                        ae[i,j] = AP.Math.Conj(a[ia+j,ja+i]);
                    }
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    vc = 0;
                    if( k>0 )
                    {
                        vc = 0.0;
                        for(i_=0; i_<=k-1;i_++)
                        {
                            vc += ae[i,i_]*AP.Math.Conj(ae[j,i_]);
                        }
                    }
                    vc = alpha*vc;
                    if( isupper & j>=i )
                    {
                        c[ic+i,jc+j] = vc+c[ic+i,jc+j];
                    }
                    if( !isupper & j<=i )
                    {
                        c[ic+i,jc+j] = vc+c[ic+i,jc+j];
                    }
                }
            }
        }


        /*************************************************************************
        Reference SYRK subroutine.

          -- ALGLIB routine --
             16.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void refrmatrixsyrk(int n,
            int k,
            double alpha,
            ref double[,] a,
            int ia,
            int ja,
            int optypea,
            double beta,
            ref double[,] c,
            int ic,
            int jc,
            bool isupper)
        {
            double[,] ae = new double[0,0];
            int i = 0;
            int j = 0;
            double vr = 0;
            int i_ = 0;

            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( isupper & j>=i | !isupper & j<=i )
                    {
                        if( (double)(beta)==(double)(0) )
                        {
                            c[i+ic,j+jc] = 0;
                        }
                        else
                        {
                            c[i+ic,j+jc] = c[i+ic,j+jc]*beta;
                        }
                    }
                }
            }
            if( (double)(alpha)==(double)(0) )
            {
                return;
            }
            if( n*k>0 )
            {
                ae = new double[n, k];
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=k-1; j++)
                {
                    if( optypea==0 )
                    {
                        ae[i,j] = a[ia+i,ja+j];
                    }
                    if( optypea==1 )
                    {
                        ae[i,j] = a[ia+j,ja+i];
                    }
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    vr = 0;
                    if( k>0 )
                    {
                        vr = 0.0;
                        for(i_=0; i_<=k-1;i_++)
                        {
                            vr += ae[i,i_]*ae[j,i_];
                        }
                    }
                    vr = alpha*vr;
                    if( isupper & j>=i )
                    {
                        c[ic+i,jc+j] = vr+c[ic+i,jc+j];
                    }
                    if( !isupper & j<=i )
                    {
                        c[ic+i,jc+j] = vr+c[ic+i,jc+j];
                    }
                }
            }
        }


        /*************************************************************************
        Reference GEMM,
        ALGLIB subroutine
        *************************************************************************/
        public static void refcmatrixgemm(int m,
            int n,
            int k,
            AP.Complex alpha,
            ref AP.Complex[,] a,
            int ia,
            int ja,
            int optypea,
            ref AP.Complex[,] b,
            int ib,
            int jb,
            int optypeb,
            AP.Complex beta,
            ref AP.Complex[,] c,
            int ic,
            int jc)
        {
            AP.Complex[,] ae = new AP.Complex[0,0];
            AP.Complex[,] be = new AP.Complex[0,0];
            int i = 0;
            int j = 0;
            AP.Complex vc = 0;
            int i_ = 0;

            ae = new AP.Complex[m, k];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=k-1; j++)
                {
                    if( optypea==0 )
                    {
                        ae[i,j] = a[ia+i,ja+j];
                    }
                    if( optypea==1 )
                    {
                        ae[i,j] = a[ia+j,ja+i];
                    }
                    if( optypea==2 )
                    {
                        ae[i,j] = AP.Math.Conj(a[ia+j,ja+i]);
                    }
                }
            }
            be = new AP.Complex[k, n];
            for(i=0; i<=k-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( optypeb==0 )
                    {
                        be[i,j] = b[ib+i,jb+j];
                    }
                    if( optypeb==1 )
                    {
                        be[i,j] = b[ib+j,jb+i];
                    }
                    if( optypeb==2 )
                    {
                        be[i,j] = AP.Math.Conj(b[ib+j,jb+i]);
                    }
                }
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    vc = 0.0;
                    for(i_=0; i_<=k-1;i_++)
                    {
                        vc += ae[i,i_]*be[i_,j];
                    }
                    vc = alpha*vc;
                    if( beta!=0 )
                    {
                        vc = vc+beta*c[ic+i,jc+j];
                    }
                    c[ic+i,jc+j] = vc;
                }
            }
        }


        /*************************************************************************
        Reference GEMM,
        ALGLIB subroutine
        *************************************************************************/
        public static void refrmatrixgemm(int m,
            int n,
            int k,
            double alpha,
            ref double[,] a,
            int ia,
            int ja,
            int optypea,
            ref double[,] b,
            int ib,
            int jb,
            int optypeb,
            double beta,
            ref double[,] c,
            int ic,
            int jc)
        {
            double[,] ae = new double[0,0];
            double[,] be = new double[0,0];
            int i = 0;
            int j = 0;
            double vc = 0;
            int i_ = 0;

            ae = new double[m, k];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=k-1; j++)
                {
                    if( optypea==0 )
                    {
                        ae[i,j] = a[ia+i,ja+j];
                    }
                    if( optypea==1 )
                    {
                        ae[i,j] = a[ia+j,ja+i];
                    }
                }
            }
            be = new double[k, n];
            for(i=0; i<=k-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( optypeb==0 )
                    {
                        be[i,j] = b[ib+i,jb+j];
                    }
                    if( optypeb==1 )
                    {
                        be[i,j] = b[ib+j,jb+i];
                    }
                }
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    vc = 0.0;
                    for(i_=0; i_<=k-1;i_++)
                    {
                        vc += ae[i,i_]*be[i_,j];
                    }
                    vc = alpha*vc;
                    if( (double)(beta)!=(double)(0) )
                    {
                        vc = vc+beta*c[ic+i,jc+j];
                    }
                    c[ic+i,jc+j] = vc;
                }
            }
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
        ?Matrix????TRSM tests

        Returns False for passed test, True - for failed
        *************************************************************************/
        private static bool testtrsm(int minn,
            int maxn)
        {
            bool result = new bool();
            int n = 0;
            int m = 0;
            int mx = 0;
            int i = 0;
            int j = 0;
            int optype = 0;
            int uppertype = 0;
            int unittype = 0;
            int xoffsi = 0;
            int xoffsj = 0;
            int aoffsitype = 0;
            int aoffsjtype = 0;
            int aoffsi = 0;
            int aoffsj = 0;
            double[,] refra = new double[0,0];
            double[,] refrxl = new double[0,0];
            double[,] refrxr = new double[0,0];
            AP.Complex[,] refca = new AP.Complex[0,0];
            AP.Complex[,] refcxl = new AP.Complex[0,0];
            AP.Complex[,] refcxr = new AP.Complex[0,0];
            double[,] ra = new double[0,0];
            AP.Complex[,] ca = new AP.Complex[0,0];
            double[,] rxr1 = new double[0,0];
            double[,] rxl1 = new double[0,0];
            AP.Complex[,] cxr1 = new AP.Complex[0,0];
            AP.Complex[,] cxl1 = new AP.Complex[0,0];
            double[,] rxr2 = new double[0,0];
            double[,] rxl2 = new double[0,0];
            AP.Complex[,] cxr2 = new AP.Complex[0,0];
            AP.Complex[,] cxl2 = new AP.Complex[0,0];
            double threshold = 0;

            threshold = AP.Math.Sqr(maxn)*100*AP.Math.MachineEpsilon;
            result = false;
            for(mx=minn; mx<=maxn; mx++)
            {
                
                //
                // Select random M/N in [1,MX] such that max(M,N)=MX
                //
                m = 1+AP.Math.RandomInteger(mx);
                n = 1+AP.Math.RandomInteger(mx);
                if( (double)(AP.Math.RandomReal())>(double)(0.5) )
                {
                    m = mx;
                }
                else
                {
                    n = mx;
                }
                
                //
                // Initialize RefRA/RefCA by random matrices whose upper
                // and lower triangle submatrices are non-degenerate
                // well-conditioned matrices.
                //
                // Matrix size is 2Mx2M (four copies of same MxM matrix
                // to test different offsets)
                //
                refra = new double[2*m, 2*m];
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        refra[i,j] = 0.2*AP.Math.RandomReal()-0.1;
                    }
                }
                for(i=0; i<=m-1; i++)
                {
                    refra[i,i] = (2*AP.Math.RandomInteger(1)-1)*(2*m+AP.Math.RandomReal());
                }
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        refra[i+m,j] = refra[i,j];
                        refra[i,j+m] = refra[i,j];
                        refra[i+m,j+m] = refra[i,j];
                    }
                }
                refca = new AP.Complex[2*m, 2*m];
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        refca[i,j].x = 0.2*AP.Math.RandomReal()-0.1;
                        refca[i,j].y = 0.2*AP.Math.RandomReal()-0.1;
                    }
                }
                for(i=0; i<=m-1; i++)
                {
                    refca[i,i].x = (2*AP.Math.RandomInteger(2)-1)*(2*m+AP.Math.RandomReal());
                    refca[i,i].y = (2*AP.Math.RandomInteger(2)-1)*(2*m+AP.Math.RandomReal());
                }
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        refca[i+m,j] = refca[i,j];
                        refca[i,j+m] = refca[i,j];
                        refca[i+m,j+m] = refca[i,j];
                    }
                }
                
                //
                // Generate random XL/XR.
                //
                // XR is NxM matrix (matrix for 'Right' subroutines)
                // XL is MxN matrix (matrix for 'Left' subroutines)
                //
                refrxr = new double[n, m];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        refrxr[i,j] = 2*AP.Math.RandomReal()-1;
                    }
                }
                refrxl = new double[m, n];
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        refrxl[i,j] = 2*AP.Math.RandomReal()-1;
                    }
                }
                refcxr = new AP.Complex[n, m];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        refcxr[i,j].x = 2*AP.Math.RandomReal()-1;
                        refcxr[i,j].y = 2*AP.Math.RandomReal()-1;
                    }
                }
                refcxl = new AP.Complex[m, n];
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        refcxl[i,j].x = 2*AP.Math.RandomReal()-1;
                        refcxl[i,j].y = 2*AP.Math.RandomReal()-1;
                    }
                }
                
                //
                // test different types of operations, offsets, and so on...
                //
                // to avoid unnecessary slowdown we don't test ALL possible
                // combinations of operation types. We just generate one random
                // set of parameters and test it.
                //
                ra = new double[2*m, 2*m];
                rxr1 = new double[n, m];
                rxr2 = new double[n, m];
                rxl1 = new double[m, n];
                rxl2 = new double[m, n];
                ca = new AP.Complex[2*m, 2*m];
                cxr1 = new AP.Complex[n, m];
                cxr2 = new AP.Complex[n, m];
                cxl1 = new AP.Complex[m, n];
                cxl2 = new AP.Complex[m, n];
                optype = AP.Math.RandomInteger(3);
                uppertype = AP.Math.RandomInteger(2);
                unittype = AP.Math.RandomInteger(2);
                xoffsi = AP.Math.RandomInteger(2);
                xoffsj = AP.Math.RandomInteger(2);
                aoffsitype = AP.Math.RandomInteger(2);
                aoffsjtype = AP.Math.RandomInteger(2);
                aoffsi = m*aoffsitype;
                aoffsj = m*aoffsjtype;
                
                //
                // copy A, XR, XL (fill unused parts with random garbage)
                //
                for(i=0; i<=2*m-1; i++)
                {
                    for(j=0; j<=2*m-1; j++)
                    {
                        if( i>=aoffsi & i<aoffsi+m & j>=aoffsj & j<aoffsj+m )
                        {
                            ca[i,j] = refca[i,j];
                            ra[i,j] = refra[i,j];
                        }
                        else
                        {
                            ca[i,j] = AP.Math.RandomReal();
                            ra[i,j] = AP.Math.RandomReal();
                        }
                    }
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        if( i>=xoffsi & j>=xoffsj )
                        {
                            cxr1[i,j] = refcxr[i,j];
                            cxr2[i,j] = refcxr[i,j];
                            rxr1[i,j] = refrxr[i,j];
                            rxr2[i,j] = refrxr[i,j];
                        }
                        else
                        {
                            cxr1[i,j] = AP.Math.RandomReal();
                            cxr2[i,j] = cxr1[i,j];
                            rxr1[i,j] = AP.Math.RandomReal();
                            rxr2[i,j] = rxr1[i,j];
                        }
                    }
                }
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( i>=xoffsi & j>=xoffsj )
                        {
                            cxl1[i,j] = refcxl[i,j];
                            cxl2[i,j] = refcxl[i,j];
                            rxl1[i,j] = refrxl[i,j];
                            rxl2[i,j] = refrxl[i,j];
                        }
                        else
                        {
                            cxl1[i,j] = AP.Math.RandomReal();
                            cxl2[i,j] = cxl1[i,j];
                            rxl1[i,j] = AP.Math.RandomReal();
                            rxl2[i,j] = rxl1[i,j];
                        }
                    }
                }
                
                //
                // Test CXR
                //
                ablas.cmatrixrighttrsm(n-xoffsi, m-xoffsj, ref ca, aoffsi, aoffsj, uppertype==0, unittype==0, optype, ref cxr1, xoffsi, xoffsj);
                refcmatrixrighttrsm(n-xoffsi, m-xoffsj, ref ca, aoffsi, aoffsj, uppertype==0, unittype==0, optype, ref cxr2, xoffsi, xoffsj);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        result = result | (double)(AP.Math.AbsComplex(cxr1[i,j]-cxr2[i,j]))>(double)(threshold);
                    }
                }
                
                //
                // Test CXL
                //
                ablas.cmatrixlefttrsm(m-xoffsi, n-xoffsj, ref ca, aoffsi, aoffsj, uppertype==0, unittype==0, optype, ref cxl1, xoffsi, xoffsj);
                refcmatrixlefttrsm(m-xoffsi, n-xoffsj, ref ca, aoffsi, aoffsj, uppertype==0, unittype==0, optype, ref cxl2, xoffsi, xoffsj);
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        result = result | (double)(AP.Math.AbsComplex(cxl1[i,j]-cxl2[i,j]))>(double)(threshold);
                    }
                }
                if( optype<2 )
                {
                    
                    //
                    // Test RXR
                    //
                    ablas.rmatrixrighttrsm(n-xoffsi, m-xoffsj, ref ra, aoffsi, aoffsj, uppertype==0, unittype==0, optype, ref rxr1, xoffsi, xoffsj);
                    refrmatrixrighttrsm(n-xoffsi, m-xoffsj, ref ra, aoffsi, aoffsj, uppertype==0, unittype==0, optype, ref rxr2, xoffsi, xoffsj);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            result = result | (double)(Math.Abs(rxr1[i,j]-rxr2[i,j]))>(double)(threshold);
                        }
                    }
                    
                    //
                    // Test RXL
                    //
                    ablas.rmatrixlefttrsm(m-xoffsi, n-xoffsj, ref ra, aoffsi, aoffsj, uppertype==0, unittype==0, optype, ref rxl1, xoffsi, xoffsj);
                    refrmatrixlefttrsm(m-xoffsi, n-xoffsj, ref ra, aoffsi, aoffsj, uppertype==0, unittype==0, optype, ref rxl2, xoffsi, xoffsj);
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            result = result | (double)(Math.Abs(rxl1[i,j]-rxl2[i,j]))>(double)(threshold);
                        }
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        SYRK tests

        Returns False for passed test, True - for failed
        *************************************************************************/
        private static bool testsyrk(int minn,
            int maxn)
        {
            bool result = new bool();
            int n = 0;
            int k = 0;
            int mx = 0;
            int i = 0;
            int j = 0;
            int uppertype = 0;
            int xoffsi = 0;
            int xoffsj = 0;
            int aoffsitype = 0;
            int aoffsjtype = 0;
            int aoffsi = 0;
            int aoffsj = 0;
            int alphatype = 0;
            int betatype = 0;
            double[,] refra = new double[0,0];
            double[,] refrc = new double[0,0];
            AP.Complex[,] refca = new AP.Complex[0,0];
            AP.Complex[,] refcc = new AP.Complex[0,0];
            double alpha = 0;
            double beta = 0;
            double[,] ra1 = new double[0,0];
            double[,] ra2 = new double[0,0];
            AP.Complex[,] ca1 = new AP.Complex[0,0];
            AP.Complex[,] ca2 = new AP.Complex[0,0];
            double[,] rc = new double[0,0];
            double[,] rct = new double[0,0];
            AP.Complex[,] cc = new AP.Complex[0,0];
            AP.Complex[,] cct = new AP.Complex[0,0];
            double threshold = 0;

            threshold = maxn*100*AP.Math.MachineEpsilon;
            result = false;
            for(mx=minn; mx<=maxn; mx++)
            {
                
                //
                // Select random M/N in [1,MX] such that max(M,N)=MX
                //
                k = 1+AP.Math.RandomInteger(mx);
                n = 1+AP.Math.RandomInteger(mx);
                if( (double)(AP.Math.RandomReal())>(double)(0.5) )
                {
                    k = mx;
                }
                else
                {
                    n = mx;
                }
                
                //
                // Initialize RefRA/RefCA by random Hermitian matrices,
                // RefRC/RefCC by random matrices
                //
                // RA/CA size is 2Nx2N (four copies of same NxN matrix
                // to test different offsets)
                //
                refra = new double[2*n, 2*n];
                refca = new AP.Complex[2*n, 2*n];
                for(i=0; i<=n-1; i++)
                {
                    refra[i,i] = 2*AP.Math.RandomReal()-1;
                    refca[i,i] = 2*AP.Math.RandomReal()-1;
                    for(j=i+1; j<=n-1; j++)
                    {
                        refra[i,j] = 2*AP.Math.RandomReal()-1;
                        refca[i,j].x = 2*AP.Math.RandomReal()-1;
                        refca[i,j].y = 2*AP.Math.RandomReal()-1;
                        refra[j,i] = refra[i,j];
                        refca[j,i] = AP.Math.Conj(refca[i,j]);
                    }
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        refra[i+n,j] = refra[i,j];
                        refra[i,j+n] = refra[i,j];
                        refra[i+n,j+n] = refra[i,j];
                        refca[i+n,j] = refca[i,j];
                        refca[i,j+n] = refca[i,j];
                        refca[i+n,j+n] = refca[i,j];
                    }
                }
                refrc = new double[n, k];
                refcc = new AP.Complex[n, k];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=k-1; j++)
                    {
                        refrc[i,j] = 2*AP.Math.RandomReal()-1;
                        refcc[i,j].x = 2*AP.Math.RandomReal()-1;
                        refcc[i,j].y = 2*AP.Math.RandomReal()-1;
                    }
                }
                
                //
                // test different types of operations, offsets, and so on...
                //
                // to avoid unnecessary slowdown we don't test ALL possible
                // combinations of operation types. We just generate one random
                // set of parameters and test it.
                //
                ra1 = new double[2*n, 2*n];
                ra2 = new double[2*n, 2*n];
                ca1 = new AP.Complex[2*n, 2*n];
                ca2 = new AP.Complex[2*n, 2*n];
                rc = new double[n, k];
                rct = new double[k, n];
                cc = new AP.Complex[n, k];
                cct = new AP.Complex[k, n];
                uppertype = AP.Math.RandomInteger(2);
                xoffsi = AP.Math.RandomInteger(2);
                xoffsj = AP.Math.RandomInteger(2);
                aoffsitype = AP.Math.RandomInteger(2);
                aoffsjtype = AP.Math.RandomInteger(2);
                alphatype = AP.Math.RandomInteger(2);
                betatype = AP.Math.RandomInteger(2);
                aoffsi = n*aoffsitype;
                aoffsj = n*aoffsjtype;
                alpha = alphatype*(2*AP.Math.RandomReal()-1);
                beta = betatype*(2*AP.Math.RandomReal()-1);
                
                //
                // copy A, C (fill unused parts with random garbage)
                //
                for(i=0; i<=2*n-1; i++)
                {
                    for(j=0; j<=2*n-1; j++)
                    {
                        if( i>=aoffsi & i<aoffsi+n & j>=aoffsj & j<aoffsj+n )
                        {
                            ca1[i,j] = refca[i,j];
                            ca2[i,j] = refca[i,j];
                            ra1[i,j] = refra[i,j];
                            ra2[i,j] = refra[i,j];
                        }
                        else
                        {
                            ca1[i,j] = AP.Math.RandomReal();
                            ca2[i,j] = ca1[i,j];
                            ra1[i,j] = AP.Math.RandomReal();
                            ra2[i,j] = ra1[i,j];
                        }
                    }
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=k-1; j++)
                    {
                        if( i>=xoffsi & j>=xoffsj )
                        {
                            rc[i,j] = refrc[i,j];
                            rct[j,i] = refrc[i,j];
                            cc[i,j] = refcc[i,j];
                            cct[j,i] = refcc[i,j];
                        }
                        else
                        {
                            rc[i,j] = AP.Math.RandomReal();
                            rct[j,i] = rc[i,j];
                            cc[i,j] = AP.Math.RandomReal();
                            cct[j,i] = cct[j,i];
                        }
                    }
                }
                
                //
                // Test complex
                // Only one of transform types is selected and tested
                //
                if( (double)(AP.Math.RandomReal())>(double)(0.5) )
                {
                    ablas.cmatrixsyrk(n-xoffsi, k-xoffsj, alpha, ref cc, xoffsi, xoffsj, 0, beta, ref ca1, aoffsi, aoffsj, uppertype==0);
                    refcmatrixsyrk(n-xoffsi, k-xoffsj, alpha, ref cc, xoffsi, xoffsj, 0, beta, ref ca2, aoffsi, aoffsj, uppertype==0);
                }
                else
                {
                    ablas.cmatrixsyrk(n-xoffsi, k-xoffsj, alpha, ref cct, xoffsj, xoffsi, 2, beta, ref ca1, aoffsi, aoffsj, uppertype==0);
                    refcmatrixsyrk(n-xoffsi, k-xoffsj, alpha, ref cct, xoffsj, xoffsi, 2, beta, ref ca2, aoffsi, aoffsj, uppertype==0);
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        result = result | (double)(AP.Math.AbsComplex(ca1[i,j]-ca2[i,j]))>(double)(threshold);
                    }
                }
                
                //
                // Test real
                // Only one of transform types is selected and tested
                //
                if( (double)(AP.Math.RandomReal())>(double)(0.5) )
                {
                    ablas.rmatrixsyrk(n-xoffsi, k-xoffsj, alpha, ref rc, xoffsi, xoffsj, 0, beta, ref ra1, aoffsi, aoffsj, uppertype==0);
                    refrmatrixsyrk(n-xoffsi, k-xoffsj, alpha, ref rc, xoffsi, xoffsj, 0, beta, ref ra2, aoffsi, aoffsj, uppertype==0);
                }
                else
                {
                    ablas.rmatrixsyrk(n-xoffsi, k-xoffsj, alpha, ref rct, xoffsj, xoffsi, 1, beta, ref ra1, aoffsi, aoffsj, uppertype==0);
                    refrmatrixsyrk(n-xoffsi, k-xoffsj, alpha, ref rct, xoffsj, xoffsi, 1, beta, ref ra2, aoffsi, aoffsj, uppertype==0);
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        result = result | (double)(Math.Abs(ra1[i,j]-ra2[i,j]))>(double)(threshold);
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        GEMM tests

        Returns False for passed test, True - for failed
        *************************************************************************/
        private static bool testgemm(int minn,
            int maxn)
        {
            bool result = new bool();
            int m = 0;
            int n = 0;
            int k = 0;
            int mx = 0;
            int i = 0;
            int j = 0;
            int aoffsi = 0;
            int aoffsj = 0;
            int aoptype = 0;
            int aoptyper = 0;
            int boffsi = 0;
            int boffsj = 0;
            int boptype = 0;
            int boptyper = 0;
            int coffsi = 0;
            int coffsj = 0;
            double[,] refra = new double[0,0];
            double[,] refrb = new double[0,0];
            double[,] refrc = new double[0,0];
            AP.Complex[,] refca = new AP.Complex[0,0];
            AP.Complex[,] refcb = new AP.Complex[0,0];
            AP.Complex[,] refcc = new AP.Complex[0,0];
            double alphar = 0;
            double betar = 0;
            AP.Complex alphac = 0;
            AP.Complex betac = 0;
            double[,] rc1 = new double[0,0];
            double[,] rc2 = new double[0,0];
            AP.Complex[,] cc1 = new AP.Complex[0,0];
            AP.Complex[,] cc2 = new AP.Complex[0,0];
            double threshold = 0;

            threshold = maxn*100*AP.Math.MachineEpsilon;
            result = false;
            for(mx=minn; mx<=maxn; mx++)
            {
                
                //
                // Select random M/N/K in [1,MX] such that max(M,N,K)=MX
                //
                m = 1+AP.Math.RandomInteger(mx);
                n = 1+AP.Math.RandomInteger(mx);
                k = 1+AP.Math.RandomInteger(mx);
                i = AP.Math.RandomInteger(3);
                if( i==0 )
                {
                    m = mx;
                }
                if( i==1 )
                {
                    n = mx;
                }
                if( i==2 )
                {
                    k = mx;
                }
                
                //
                // Initialize A/B/C by random matrices with size (MaxN+1)*(MaxN+1)
                //
                refra = new double[maxn+1, maxn+1];
                refrb = new double[maxn+1, maxn+1];
                refrc = new double[maxn+1, maxn+1];
                refca = new AP.Complex[maxn+1, maxn+1];
                refcb = new AP.Complex[maxn+1, maxn+1];
                refcc = new AP.Complex[maxn+1, maxn+1];
                for(i=0; i<=maxn; i++)
                {
                    for(j=0; j<=maxn; j++)
                    {
                        refra[i,j] = 2*AP.Math.RandomReal()-1;
                        refrb[i,j] = 2*AP.Math.RandomReal()-1;
                        refrc[i,j] = 2*AP.Math.RandomReal()-1;
                        refca[i,j].x = 2*AP.Math.RandomReal()-1;
                        refca[i,j].y = 2*AP.Math.RandomReal()-1;
                        refcb[i,j].x = 2*AP.Math.RandomReal()-1;
                        refcb[i,j].y = 2*AP.Math.RandomReal()-1;
                        refcc[i,j].x = 2*AP.Math.RandomReal()-1;
                        refcc[i,j].y = 2*AP.Math.RandomReal()-1;
                    }
                }
                
                //
                // test different types of operations, offsets, and so on...
                //
                // to avoid unnecessary slowdown we don't test ALL possible
                // combinations of operation types. We just generate one random
                // set of parameters and test it.
                //
                rc1 = new double[maxn+1, maxn+1];
                rc2 = new double[maxn+1, maxn+1];
                cc1 = new AP.Complex[maxn+1, maxn+1];
                cc2 = new AP.Complex[maxn+1, maxn+1];
                aoffsi = AP.Math.RandomInteger(2);
                aoffsj = AP.Math.RandomInteger(2);
                aoptype = AP.Math.RandomInteger(3);
                aoptyper = AP.Math.RandomInteger(2);
                boffsi = AP.Math.RandomInteger(2);
                boffsj = AP.Math.RandomInteger(2);
                boptype = AP.Math.RandomInteger(3);
                boptyper = AP.Math.RandomInteger(2);
                coffsi = AP.Math.RandomInteger(2);
                coffsj = AP.Math.RandomInteger(2);
                alphar = AP.Math.RandomInteger(2)*(2*AP.Math.RandomReal()-1);
                betar = AP.Math.RandomInteger(2)*(2*AP.Math.RandomReal()-1);
                if( (double)(AP.Math.RandomReal())>(double)(0.5) )
                {
                    alphac.x = 2*AP.Math.RandomReal()-1;
                    alphac.y = 2*AP.Math.RandomReal()-1;
                }
                else
                {
                    alphac = 0;
                }
                if( (double)(AP.Math.RandomReal())>(double)(0.5) )
                {
                    betac.x = 2*AP.Math.RandomReal()-1;
                    betac.y = 2*AP.Math.RandomReal()-1;
                }
                else
                {
                    betac = 0;
                }
                
                //
                // copy C
                //
                for(i=0; i<=maxn; i++)
                {
                    for(j=0; j<=maxn; j++)
                    {
                        rc1[i,j] = refrc[i,j];
                        rc2[i,j] = refrc[i,j];
                        cc1[i,j] = refcc[i,j];
                        cc2[i,j] = refcc[i,j];
                    }
                }
                
                //
                // Test complex
                //
                ablas.cmatrixgemm(m, n, k, alphac, ref refca, aoffsi, aoffsj, aoptype, ref refcb, boffsi, boffsj, boptype, betac, ref cc1, coffsi, coffsj);
                refcmatrixgemm(m, n, k, alphac, ref refca, aoffsi, aoffsj, aoptype, ref refcb, boffsi, boffsj, boptype, betac, ref cc2, coffsi, coffsj);
                for(i=0; i<=maxn; i++)
                {
                    for(j=0; j<=maxn; j++)
                    {
                        result = result | (double)(AP.Math.AbsComplex(cc1[i,j]-cc2[i,j]))>(double)(threshold);
                    }
                }
                
                //
                // Test real
                //
                ablas.rmatrixgemm(m, n, k, alphar, ref refra, aoffsi, aoffsj, aoptyper, ref refrb, boffsi, boffsj, boptyper, betar, ref rc1, coffsi, coffsj);
                refrmatrixgemm(m, n, k, alphar, ref refra, aoffsi, aoffsj, aoptyper, ref refrb, boffsi, boffsj, boptyper, betar, ref rc2, coffsi, coffsj);
                for(i=0; i<=maxn; i++)
                {
                    for(j=0; j<=maxn; j++)
                    {
                        result = result | (double)(Math.Abs(rc1[i,j]-rc2[i,j]))>(double)(threshold);
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        transpose tests

        Returns False for passed test, True - for failed
        *************************************************************************/
        private static bool testtrans(int minn,
            int maxn)
        {
            bool result = new bool();
            int m = 0;
            int n = 0;
            int mx = 0;
            int i = 0;
            int j = 0;
            int aoffsi = 0;
            int aoffsj = 0;
            int boffsi = 0;
            int boffsj = 0;
            double v1 = 0;
            double v2 = 0;
            double threshold = 0;
            double[,] refra = new double[0,0];
            double[,] refrb = new double[0,0];
            AP.Complex[,] refca = new AP.Complex[0,0];
            AP.Complex[,] refcb = new AP.Complex[0,0];

            result = false;
            threshold = 1000*AP.Math.MachineEpsilon;
            for(mx=minn; mx<=maxn; mx++)
            {
                
                //
                // Select random M/N in [1,MX] such that max(M,N)=MX
                // Generate random V1 and V2 which are used to fill
                // RefRB/RefCB with control values.
                //
                m = 1+AP.Math.RandomInteger(mx);
                n = 1+AP.Math.RandomInteger(mx);
                if( AP.Math.RandomInteger(2)==0 )
                {
                    m = mx;
                }
                else
                {
                    n = mx;
                }
                v1 = AP.Math.RandomReal();
                v2 = AP.Math.RandomReal();
                
                //
                // Initialize A by random matrix with size (MaxN+1)*(MaxN+1)
                // Fill B with control values
                //
                refra = new double[maxn+1, maxn+1];
                refrb = new double[maxn+1, maxn+1];
                refca = new AP.Complex[maxn+1, maxn+1];
                refcb = new AP.Complex[maxn+1, maxn+1];
                for(i=0; i<=maxn; i++)
                {
                    for(j=0; j<=maxn; j++)
                    {
                        refra[i,j] = 2*AP.Math.RandomReal()-1;
                        refca[i,j].x = 2*AP.Math.RandomReal()-1;
                        refca[i,j].y = 2*AP.Math.RandomReal()-1;
                        refrb[i,j] = i*v1+j*v2;
                        refcb[i,j] = i*v1+j*v2;
                    }
                }
                
                //
                // test different offsets (zero or one)
                //
                // to avoid unnecessary slowdown we don't test ALL possible
                // combinations of operation types. We just generate one random
                // set of parameters and test it.
                //
                aoffsi = AP.Math.RandomInteger(2);
                aoffsj = AP.Math.RandomInteger(2);
                boffsi = AP.Math.RandomInteger(2);
                boffsj = AP.Math.RandomInteger(2);
                ablas.rmatrixtranspose(m, n, ref refra, aoffsi, aoffsj, ref refrb, boffsi, boffsj);
                for(i=0; i<=maxn; i++)
                {
                    for(j=0; j<=maxn; j++)
                    {
                        if( i<boffsi | i>=boffsi+n | j<boffsj | j>=boffsj+m )
                        {
                            result = result | (double)(Math.Abs(refrb[i,j]-(v1*i+v2*j)))>(double)(threshold);
                        }
                        else
                        {
                            result = result | (double)(Math.Abs(refrb[i,j]-refra[aoffsi+j-boffsj,aoffsj+i-boffsi]))>(double)(threshold);
                        }
                    }
                }
                ablas.cmatrixtranspose(m, n, ref refca, aoffsi, aoffsj, ref refcb, boffsi, boffsj);
                for(i=0; i<=maxn; i++)
                {
                    for(j=0; j<=maxn; j++)
                    {
                        if( i<boffsi | i>=boffsi+n | j<boffsj | j>=boffsj+m )
                        {
                            result = result | (double)(AP.Math.AbsComplex(refcb[i,j]-(v1*i+v2*j)))>(double)(threshold);
                        }
                        else
                        {
                            result = result | (double)(AP.Math.AbsComplex(refcb[i,j]-refca[aoffsi+j-boffsj,aoffsj+i-boffsi]))>(double)(threshold);
                        }
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        rank-1tests

        Returns False for passed test, True - for failed
        *************************************************************************/
        private static bool testrank1(int minn,
            int maxn)
        {
            bool result = new bool();
            int m = 0;
            int n = 0;
            int mx = 0;
            int i = 0;
            int j = 0;
            int aoffsi = 0;
            int aoffsj = 0;
            int uoffs = 0;
            int voffs = 0;
            double threshold = 0;
            double[,] refra = new double[0,0];
            double[,] refrb = new double[0,0];
            AP.Complex[,] refca = new AP.Complex[0,0];
            AP.Complex[,] refcb = new AP.Complex[0,0];
            double[] ru = new double[0];
            double[] rv = new double[0];
            AP.Complex[] cu = new AP.Complex[0];
            AP.Complex[] cv = new AP.Complex[0];

            result = false;
            threshold = 1000*AP.Math.MachineEpsilon;
            for(mx=minn; mx<=maxn; mx++)
            {
                
                //
                // Select random M/N in [1,MX] such that max(M,N)=MX
                //
                m = 1+AP.Math.RandomInteger(mx);
                n = 1+AP.Math.RandomInteger(mx);
                if( AP.Math.RandomInteger(2)==0 )
                {
                    m = mx;
                }
                else
                {
                    n = mx;
                }
                
                //
                // Initialize A by random matrix with size (MaxN+1)*(MaxN+1)
                // Fill B with control values
                //
                refra = new double[maxn+maxn, maxn+maxn];
                refrb = new double[maxn+maxn, maxn+maxn];
                refca = new AP.Complex[maxn+maxn, maxn+maxn];
                refcb = new AP.Complex[maxn+maxn, maxn+maxn];
                for(i=0; i<=2*maxn-1; i++)
                {
                    for(j=0; j<=2*maxn-1; j++)
                    {
                        refra[i,j] = 2*AP.Math.RandomReal()-1;
                        refca[i,j].x = 2*AP.Math.RandomReal()-1;
                        refca[i,j].y = 2*AP.Math.RandomReal()-1;
                        refrb[i,j] = refra[i,j];
                        refcb[i,j] = refca[i,j];
                    }
                }
                ru = new double[2*m];
                cu = new AP.Complex[2*m];
                for(i=0; i<=2*m-1; i++)
                {
                    ru[i] = 2*AP.Math.RandomReal()-1;
                    cu[i].x = 2*AP.Math.RandomReal()-1;
                    cu[i].y = 2*AP.Math.RandomReal()-1;
                }
                rv = new double[2*n];
                cv = new AP.Complex[2*n];
                for(i=0; i<=2*n-1; i++)
                {
                    rv[i] = 2*AP.Math.RandomReal()-1;
                    cv[i].x = 2*AP.Math.RandomReal()-1;
                    cv[i].y = 2*AP.Math.RandomReal()-1;
                }
                
                //
                // test different offsets (zero or one)
                //
                // to avoid unnecessary slowdown we don't test ALL possible
                // combinations of operation types. We just generate one random
                // set of parameters and test it.
                //
                aoffsi = AP.Math.RandomInteger(maxn);
                aoffsj = AP.Math.RandomInteger(maxn);
                uoffs = AP.Math.RandomInteger(m);
                voffs = AP.Math.RandomInteger(n);
                ablas.cmatrixrank1(m, n, ref refca, aoffsi, aoffsj, ref cu, uoffs, ref cv, voffs);
                for(i=0; i<=2*maxn-1; i++)
                {
                    for(j=0; j<=2*maxn-1; j++)
                    {
                        if( i<aoffsi | i>=aoffsi+m | j<aoffsj | j>=aoffsj+n )
                        {
                            result = result | (double)(AP.Math.AbsComplex(refca[i,j]-refcb[i,j]))>(double)(threshold);
                        }
                        else
                        {
                            result = result | (double)(AP.Math.AbsComplex(refca[i,j]-(refcb[i,j]+cu[i-aoffsi+uoffs]*cv[j-aoffsj+voffs])))>(double)(threshold);
                        }
                    }
                }
                ablas.rmatrixrank1(m, n, ref refra, aoffsi, aoffsj, ref ru, uoffs, ref rv, voffs);
                for(i=0; i<=2*maxn-1; i++)
                {
                    for(j=0; j<=2*maxn-1; j++)
                    {
                        if( i<aoffsi | i>=aoffsi+m | j<aoffsj | j>=aoffsj+n )
                        {
                            result = result | (double)(Math.Abs(refra[i,j]-refrb[i,j]))>(double)(threshold);
                        }
                        else
                        {
                            result = result | (double)(Math.Abs(refra[i,j]-(refrb[i,j]+ru[i-aoffsi+uoffs]*rv[j-aoffsj+voffs])))>(double)(threshold);
                        }
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        MV tests

        Returns False for passed test, True - for failed
        *************************************************************************/
        private static bool testmv(int minn,
            int maxn)
        {
            bool result = new bool();
            int m = 0;
            int n = 0;
            int mx = 0;
            int i = 0;
            int j = 0;
            int aoffsi = 0;
            int aoffsj = 0;
            int xoffs = 0;
            int yoffs = 0;
            int opca = 0;
            int opra = 0;
            double threshold = 0;
            double rv1 = 0;
            double rv2 = 0;
            AP.Complex cv1 = 0;
            AP.Complex cv2 = 0;
            double[,] refra = new double[0,0];
            AP.Complex[,] refca = new AP.Complex[0,0];
            double[] rx = new double[0];
            double[] ry = new double[0];
            AP.Complex[] cx = new AP.Complex[0];
            AP.Complex[] cy = new AP.Complex[0];
            int i_ = 0;
            int i1_ = 0;

            result = false;
            threshold = 1000*AP.Math.MachineEpsilon;
            for(mx=minn; mx<=maxn; mx++)
            {
                
                //
                // Select random M/N in [1,MX] such that max(M,N)=MX
                //
                m = 1+AP.Math.RandomInteger(mx);
                n = 1+AP.Math.RandomInteger(mx);
                if( AP.Math.RandomInteger(2)==0 )
                {
                    m = mx;
                }
                else
                {
                    n = mx;
                }
                
                //
                // Initialize A by random matrix with size (MaxN+MaxN)*(MaxN+MaxN)
                // Initialize X by random vector with size (MaxN+MaxN)
                // Fill Y by control values
                //
                refra = new double[maxn+maxn, maxn+maxn];
                refca = new AP.Complex[maxn+maxn, maxn+maxn];
                for(i=0; i<=2*maxn-1; i++)
                {
                    for(j=0; j<=2*maxn-1; j++)
                    {
                        refra[i,j] = 2*AP.Math.RandomReal()-1;
                        refca[i,j].x = 2*AP.Math.RandomReal()-1;
                        refca[i,j].y = 2*AP.Math.RandomReal()-1;
                    }
                }
                rx = new double[2*maxn];
                cx = new AP.Complex[2*maxn];
                ry = new double[2*maxn];
                cy = new AP.Complex[2*maxn];
                for(i=0; i<=2*maxn-1; i++)
                {
                    rx[i] = 2*AP.Math.RandomReal()-1;
                    cx[i].x = 2*AP.Math.RandomReal()-1;
                    cx[i].y = 2*AP.Math.RandomReal()-1;
                    ry[i] = i;
                    cy[i] = i;
                }
                
                //
                // test different offsets (zero or one)
                //
                // to avoid unnecessary slowdown we don't test ALL possible
                // combinations of operation types. We just generate one random
                // set of parameters and test it.
                //
                aoffsi = AP.Math.RandomInteger(maxn);
                aoffsj = AP.Math.RandomInteger(maxn);
                xoffs = AP.Math.RandomInteger(maxn);
                yoffs = AP.Math.RandomInteger(maxn);
                opca = AP.Math.RandomInteger(3);
                opra = AP.Math.RandomInteger(2);
                ablas.cmatrixmv(m, n, ref refca, aoffsi, aoffsj, opca, ref cx, xoffs, ref cy, yoffs);
                for(i=0; i<=2*maxn-1; i++)
                {
                    if( i<yoffs | i>=yoffs+m )
                    {
                        result = result | cy[i]!=i;
                    }
                    else
                    {
                        cv1 = cy[i];
                        if( opca==0 )
                        {
                            i1_ = (xoffs)-(aoffsj);
                            cv2 = 0.0;
                            for(i_=aoffsj; i_<=aoffsj+n-1;i_++)
                            {
                                cv2 += refca[aoffsi+i-yoffs,i_]*cx[i_+i1_];
                            }
                        }
                        if( opca==1 )
                        {
                            i1_ = (xoffs)-(aoffsi);
                            cv2 = 0.0;
                            for(i_=aoffsi; i_<=aoffsi+n-1;i_++)
                            {
                                cv2 += refca[i_,aoffsj+i-yoffs]*cx[i_+i1_];
                            }
                        }
                        if( opca==2 )
                        {
                            i1_ = (xoffs)-(aoffsi);
                            cv2 = 0.0;
                            for(i_=aoffsi; i_<=aoffsi+n-1;i_++)
                            {
                                cv2 += AP.Math.Conj(refca[i_,aoffsj+i-yoffs])*cx[i_+i1_];
                            }
                        }
                        result = result | (double)(AP.Math.AbsComplex(cv1-cv2))>(double)(threshold);
                    }
                }
                ablas.rmatrixmv(m, n, ref refra, aoffsi, aoffsj, opra, ref rx, xoffs, ref ry, yoffs);
                for(i=0; i<=2*maxn-1; i++)
                {
                    if( i<yoffs | i>=yoffs+m )
                    {
                        result = result | (double)(ry[i])!=(double)(i);
                    }
                    else
                    {
                        rv1 = ry[i];
                        if( opra==0 )
                        {
                            i1_ = (xoffs)-(aoffsj);
                            rv2 = 0.0;
                            for(i_=aoffsj; i_<=aoffsj+n-1;i_++)
                            {
                                rv2 += refra[aoffsi+i-yoffs,i_]*rx[i_+i1_];
                            }
                        }
                        if( opra==1 )
                        {
                            i1_ = (xoffs)-(aoffsi);
                            rv2 = 0.0;
                            for(i_=aoffsi; i_<=aoffsi+n-1;i_++)
                            {
                                rv2 += refra[i_,aoffsj+i-yoffs]*rx[i_+i1_];
                            }
                        }
                        result = result | (double)(Math.Abs(rv1-rv2))>(double)(threshold);
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testablasunit_test_silent()
        {
            bool result = new bool();

            result = testablas(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testablasunit_test()
        {
            bool result = new bool();

            result = testablas(false);
            return result;
        }
    }
}
