
using System;

namespace alglib
{
    public class testinverseupdateunit
    {
        public static bool testinverseupdate(bool silent)
        {
            bool result = new bool();
            double[,] a = new double[0,0];
            double[,] inva = new double[0,0];
            double[,] b1 = new double[0,0];
            double[,] b2 = new double[0,0];
            double[] u = new double[0];
            double[] v = new double[0];
            int n = 0;
            int maxn = 0;
            int i = 0;
            int j = 0;
            int updrow = 0;
            int updcol = 0;
            double val = 0;
            int pass = 0;
            int passcount = 0;
            bool waserrors = new bool();
            double threshold = 0;
            double c = 0;

            waserrors = false;
            maxn = 10;
            passcount = 100;
            threshold = 1.0E-6;
            
            //
            // process
            //
            for(n=1; n<=maxn; n++)
            {
                a = new double[n-1+1, n-1+1];
                b1 = new double[n-1+1, n-1+1];
                b2 = new double[n-1+1, n-1+1];
                u = new double[n-1+1];
                v = new double[n-1+1];
                for(pass=1; pass<=passcount; pass++)
                {
                    c = Math.Exp(AP.Math.RandomReal()*Math.Log(10));
                    generaterandommatrixcond(ref a, n, c);
                    makeacopy(ref a, n, n, ref inva);
                    if( !invmat(ref inva, n) )
                    {
                        waserrors = true;
                        break;
                    }
                    
                    //
                    // Test simple update
                    //
                    updrow = AP.Math.RandomInteger(n);
                    updcol = AP.Math.RandomInteger(n);
                    val = 0.1*(2*AP.Math.RandomReal()-1);
                    for(i=0; i<=n-1; i++)
                    {
                        if( i==updrow )
                        {
                            u[i] = val;
                        }
                        else
                        {
                            u[i] = 0;
                        }
                        if( i==updcol )
                        {
                            v[i] = 1;
                        }
                        else
                        {
                            v[i] = 0;
                        }
                    }
                    makeacopy(ref a, n, n, ref b1);
                    if( !updandinv(ref b1, ref u, ref v, n) )
                    {
                        waserrors = true;
                        break;
                    }
                    makeacopy(ref inva, n, n, ref b2);
                    inverseupdate.rmatrixinvupdatesimple(ref b2, n, updrow, updcol, val);
                    waserrors = waserrors | (double)(matrixdiff(ref b1, ref b2, n, n))>(double)(threshold);
                    
                    //
                    // Test row update
                    //
                    updrow = AP.Math.RandomInteger(n);
                    for(i=0; i<=n-1; i++)
                    {
                        if( i==updrow )
                        {
                            u[i] = 1;
                        }
                        else
                        {
                            u[i] = 0;
                        }
                        v[i] = 0.1*(2*AP.Math.RandomReal()-1);
                    }
                    makeacopy(ref a, n, n, ref b1);
                    if( !updandinv(ref b1, ref u, ref v, n) )
                    {
                        waserrors = true;
                        break;
                    }
                    makeacopy(ref inva, n, n, ref b2);
                    inverseupdate.rmatrixinvupdaterow(ref b2, n, updrow, ref v);
                    waserrors = waserrors | (double)(matrixdiff(ref b1, ref b2, n, n))>(double)(threshold);
                    
                    //
                    // Test column update
                    //
                    updcol = AP.Math.RandomInteger(n);
                    for(i=0; i<=n-1; i++)
                    {
                        if( i==updcol )
                        {
                            v[i] = 1;
                        }
                        else
                        {
                            v[i] = 0;
                        }
                        u[i] = 0.1*(2*AP.Math.RandomReal()-1);
                    }
                    makeacopy(ref a, n, n, ref b1);
                    if( !updandinv(ref b1, ref u, ref v, n) )
                    {
                        waserrors = true;
                        break;
                    }
                    makeacopy(ref inva, n, n, ref b2);
                    inverseupdate.rmatrixinvupdatecolumn(ref b2, n, updcol, ref u);
                    waserrors = waserrors | (double)(matrixdiff(ref b1, ref b2, n, n))>(double)(threshold);
                    
                    //
                    // Test full update
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        v[i] = 0.1*(2*AP.Math.RandomReal()-1);
                        u[i] = 0.1*(2*AP.Math.RandomReal()-1);
                    }
                    makeacopy(ref a, n, n, ref b1);
                    if( !updandinv(ref b1, ref u, ref v, n) )
                    {
                        waserrors = true;
                        break;
                    }
                    makeacopy(ref inva, n, n, ref b2);
                    inverseupdate.rmatrixinvupdateuv(ref b2, n, ref u, ref v);
                    waserrors = waserrors | (double)(matrixdiff(ref b1, ref b2, n, n))>(double)(threshold);
                }
            }
            
            //
            // report
            //
            if( !silent )
            {
                System.Console.Write("TESTING INVERSE UPDATE (REAL)");
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
        LU decomposition
        *************************************************************************/
        private static void matlu(ref double[,] a,
            int m,
            int n,
            ref int[] pivots)
        {
            int i = 0;
            int j = 0;
            int jp = 0;
            double[] t1 = new double[0];
            double s = 0;
            int i_ = 0;

            pivots = new int[Math.Min(m-1, n-1)+1];
            t1 = new double[Math.Max(m-1, n-1)+1];
            System.Diagnostics.Debug.Assert(m>=0 & n>=0, "Error in LUDecomposition: incorrect function arguments");
            
            //
            // Quick return if possible
            //
            if( m==0 | n==0 )
            {
                return;
            }
            for(j=0; j<=Math.Min(m-1, n-1); j++)
            {
                
                //
                // Find pivot and test for singularity.
                //
                jp = j;
                for(i=j+1; i<=m-1; i++)
                {
                    if( (double)(Math.Abs(a[i,j]))>(double)(Math.Abs(a[jp,j])) )
                    {
                        jp = i;
                    }
                }
                pivots[j] = jp;
                if( (double)(a[jp,j])!=(double)(0) )
                {
                    
                    //
                    //Apply the interchange to rows
                    //
                    if( jp!=j )
                    {
                        for(i_=0; i_<=n-1;i_++)
                        {
                            t1[i_] = a[j,i_];
                        }
                        for(i_=0; i_<=n-1;i_++)
                        {
                            a[j,i_] = a[jp,i_];
                        }
                        for(i_=0; i_<=n-1;i_++)
                        {
                            a[jp,i_] = t1[i_];
                        }
                    }
                    
                    //
                    //Compute elements J+1:M of J-th column.
                    //
                    if( j<m )
                    {
                        jp = j+1;
                        s = 1/a[j,j];
                        for(i_=jp; i_<=m-1;i_++)
                        {
                            a[i_,j] = s*a[i_,j];
                        }
                    }
                }
                if( j<Math.Min(m, n)-1 )
                {
                    
                    //
                    //Update trailing submatrix.
                    //
                    jp = j+1;
                    for(i=j+1; i<=m-1; i++)
                    {
                        s = a[i,j];
                        for(i_=jp; i_<=n-1;i_++)
                        {
                            a[i,i_] = a[i,i_] - s*a[j,i_];
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Generate matrix with given condition number C (2-norm)
        *************************************************************************/
        private static void generaterandomorthogonalmatrix(ref double[,] a0,
            int n)
        {
            double t = 0;
            double lambda = 0;
            int s = 0;
            int i = 0;
            int j = 0;
            double u1 = 0;
            double u2 = 0;
            double[] w = new double[0];
            double[] v = new double[0];
            double[,] a = new double[0,0];
            double sm = 0;
            int i_ = 0;

            if( n<=0 )
            {
                return;
            }
            w = new double[n+1];
            v = new double[n+1];
            a = new double[n+1, n+1];
            a0 = new double[n-1+1, n-1+1];
            
            //
            // Prepare A
            //
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    if( i==j )
                    {
                        a[i,j] = 1;
                    }
                    else
                    {
                        a[i,j] = 0;
                    }
                }
            }
            
            //
            // Calculate A using Stewart algorithm
            //
            for(s=2; s<=n; s++)
            {
                
                //
                // Prepare v and Lambda = v'*v
                //
                do
                {
                    i = 1;
                    while( i<=s )
                    {
                        u1 = 2*AP.Math.RandomReal()-1;
                        u2 = 2*AP.Math.RandomReal()-1;
                        sm = u1*u1+u2*u2;
                        if( (double)(sm)==(double)(0) | (double)(sm)>(double)(1) )
                        {
                            continue;
                        }
                        sm = Math.Sqrt(-(2*Math.Log(sm)/sm));
                        v[i] = u1*sm;
                        if( i+1<=s )
                        {
                            v[i+1] = u2*sm;
                        }
                        i = i+2;
                    }
                    lambda = 0.0;
                    for(i_=1; i_<=s;i_++)
                    {
                        lambda += v[i_]*v[i_];
                    }
                }
                while( (double)(lambda)==(double)(0) );
                lambda = 2/lambda;
                
                //
                // A * (I - 2 vv'/v'v ) =
                //   = A - (2/v'v) * A * v * v' =
                //   = A - (2/v'v) * w * v'
                //  where w = Av
                //
                for(i=1; i<=s; i++)
                {
                    t = 0.0;
                    for(i_=1; i_<=s;i_++)
                    {
                        t += a[i,i_]*v[i_];
                    }
                    w[i] = t;
                }
                for(i=1; i<=s; i++)
                {
                    t = w[i]*lambda;
                    for(i_=1; i_<=s;i_++)
                    {
                        a[i,i_] = a[i,i_] - t*v[i_];
                    }
                }
            }
            
            //
            //
            //
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    a0[i-1,j-1] = a[i,j];
                }
            }
        }


        private static void generaterandommatrixcond(ref double[,] a0,
            int n,
            double c)
        {
            double l1 = 0;
            double l2 = 0;
            double[,] q1 = new double[0,0];
            double[,] q2 = new double[0,0];
            double[] cc = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;

            generaterandomorthogonalmatrix(ref q1, n);
            generaterandomorthogonalmatrix(ref q2, n);
            cc = new double[n-1+1];
            l1 = 0;
            l2 = Math.Log(1/c);
            cc[0] = Math.Exp(l1);
            for(i=1; i<=n-2; i++)
            {
                cc[i] = Math.Exp(AP.Math.RandomReal()*(l2-l1)+l1);
            }
            cc[n-1] = Math.Exp(l2);
            a0 = new double[n-1+1, n-1+1];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a0[i,j] = 0;
                    for(k=0; k<=n-1; k++)
                    {
                        a0[i,j] = a0[i,j]+q1[i,k]*cc[k]*q2[j,k];
                    }
                }
            }
        }


        /*************************************************************************
        triangular inverse
        *************************************************************************/
        private static bool invmattr(ref double[,] a,
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
        LU inverse
        *************************************************************************/
        private static bool invmatlu(ref double[,] a,
            ref int[] pivots,
            int n)
        {
            bool result = new bool();
            double[] work = new double[0];
            int i = 0;
            int iws = 0;
            int j = 0;
            int jb = 0;
            int jj = 0;
            int jp = 0;
            double v = 0;
            int i_ = 0;

            result = true;
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                return result;
            }
            work = new double[n-1+1];
            
            //
            // Form inv(U)
            //
            if( !invmattr(ref a, n, true, false) )
            {
                result = false;
                return result;
            }
            
            //
            // Solve the equation inv(A)*L = inv(U) for inv(A).
            //
            for(j=n-1; j>=0; j--)
            {
                
                //
                // Copy current column of L to WORK and replace with zeros.
                //
                for(i=j+1; i<=n-1; i++)
                {
                    work[i] = a[i,j];
                    a[i,j] = 0;
                }
                
                //
                // Compute current column of inv(A).
                //
                if( j<n-1 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        v = 0.0;
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            v += a[i,i_]*work[i_];
                        }
                        a[i,j] = a[i,j]-v;
                    }
                }
            }
            
            //
            // Apply column interchanges.
            //
            for(j=n-2; j>=0; j--)
            {
                jp = pivots[j];
                if( jp!=j )
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        work[i_] = a[i_,j];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        a[i_,j] = a[i_,jp];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        a[i_,jp] = work[i_];
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Matrix inverse
        *************************************************************************/
        private static bool invmat(ref double[,] a,
            int n)
        {
            bool result = new bool();
            int[] pivots = new int[0];

            matlu(ref a, n, n, ref pivots);
            result = invmatlu(ref a, ref pivots, n);
            return result;
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
        Update and inverse
        *************************************************************************/
        private static bool updandinv(ref double[,] a,
            ref double[] u,
            ref double[] v,
            int n)
        {
            bool result = new bool();
            int[] pivots = new int[0];
            int i = 0;
            double r = 0;
            int i_ = 0;

            for(i=0; i<=n-1; i++)
            {
                r = u[i];
                for(i_=0; i_<=n-1;i_++)
                {
                    a[i,i_] = a[i,i_] + r*v[i_];
                }
            }
            matlu(ref a, n, n, ref pivots);
            result = invmatlu(ref a, ref pivots, n);
            return result;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testinverseupdateunit_test_silent()
        {
            bool result = new bool();

            result = testinverseupdate(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testinverseupdateunit_test()
        {
            bool result = new bool();

            result = testinverseupdate(false);
            return result;
        }
    }
}
