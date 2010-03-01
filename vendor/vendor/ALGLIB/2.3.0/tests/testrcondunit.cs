
using System;

namespace alglib
{
    public class testrcondunit
    {
        public const double threshold50 = 0.25;
        public const double threshold90 = 0.10;


        public static bool testrcond(bool silent)
        {
            bool result = new bool();
            int maxn = 0;
            int passcount = 0;
            bool waserrors = new bool();
            bool rerr = new bool();
            bool cerr = new bool();
            bool spderr = new bool();
            bool hpderr = new bool();

            maxn = 10;
            passcount = 100;
            
            //
            // report
            //
            rerr = !testrmatrixrcond(maxn, passcount);
            cerr = !testcmatrixrcond(maxn, passcount);
            spderr = !testspdmatrixrcond(maxn, passcount);
            hpderr = !testhpdmatrixrcond(maxn, passcount);
            waserrors = rerr | cerr | spderr | hpderr;
            if( !silent )
            {
                System.Console.Write("TESTING RCOND");
                System.Console.WriteLine();
                System.Console.Write("REAL:                                    ");
                if( !rerr )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("SPD:                                     ");
                if( !spderr )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("HPD:                                     ");
                if( !hpderr )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("COMPLEX:                                 ");
                if( !cerr )
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
        Copy
        *************************************************************************/
        private static void rmatrixmakeacopy(ref double[,] a,
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
        Drops upper or lower half of the matrix - fills it by special pattern
        which may be used later to ensure that this part wasn't changed
        *************************************************************************/
        private static void rmatrixdrophalf(ref double[,] a,
            int n,
            bool droplower)
        {
            int i = 0;
            int j = 0;

            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( droplower & i>j | !droplower & i<j )
                    {
                        a[i,j] = 1+2*i+3*j;
                    }
                }
            }
        }


        /*************************************************************************
        Drops upper or lower half of the matrix - fills it by special pattern
        which may be used later to ensure that this part wasn't changed
        *************************************************************************/
        private static void cmatrixdrophalf(ref AP.Complex[,] a,
            int n,
            bool droplower)
        {
            int i = 0;
            int j = 0;

            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( droplower & i>j | !droplower & i<j )
                    {
                        a[i,j] = 1+2*i+3*j;
                    }
                }
            }
        }


        /*************************************************************************
        Generate matrix with given condition number C (2-norm)
        *************************************************************************/
        private static void rmatrixgenzero(ref double[,] a0,
            int n)
        {
            int i = 0;
            int j = 0;

            a0 = new double[n, n];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a0[i,j] = 0;
                }
            }
        }


        /*************************************************************************
        triangular inverse
        *************************************************************************/
        private static bool rmatrixinvmattr(ref double[,] a,
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
        private static bool rmatrixinvmatlu(ref double[,] a,
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
            if( !rmatrixinvmattr(ref a, n, true, false) )
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
        private static bool rmatrixinvmat(ref double[,] a,
            int n)
        {
            bool result = new bool();
            int[] pivots = new int[0];

            trfac.rmatrixlu(ref a, n, n, ref pivots);
            result = rmatrixinvmatlu(ref a, ref pivots, n);
            return result;
        }


        /*************************************************************************
        reference RCond
        *************************************************************************/
        private static void rmatrixrefrcond(ref double[,] a,
            int n,
            ref double rc1,
            ref double rcinf)
        {
            double[,] inva = new double[0,0];
            double nrm1a = 0;
            double nrminfa = 0;
            double nrm1inva = 0;
            double nrminfinva = 0;
            double v = 0;
            int k = 0;
            int i = 0;

            
            //
            // inv A
            //
            rmatrixmakeacopy(ref a, n, n, ref inva);
            if( !rmatrixinvmat(ref inva, n) )
            {
                rc1 = 0;
                rcinf = 0;
                return;
            }
            
            //
            // norm A
            //
            nrm1a = 0;
            nrminfa = 0;
            for(k=0; k<=n-1; k++)
            {
                v = 0;
                for(i=0; i<=n-1; i++)
                {
                    v = v+Math.Abs(a[i,k]);
                }
                nrm1a = Math.Max(nrm1a, v);
                v = 0;
                for(i=0; i<=n-1; i++)
                {
                    v = v+Math.Abs(a[k,i]);
                }
                nrminfa = Math.Max(nrminfa, v);
            }
            
            //
            // norm inv A
            //
            nrm1inva = 0;
            nrminfinva = 0;
            for(k=0; k<=n-1; k++)
            {
                v = 0;
                for(i=0; i<=n-1; i++)
                {
                    v = v+Math.Abs(inva[i,k]);
                }
                nrm1inva = Math.Max(nrm1inva, v);
                v = 0;
                for(i=0; i<=n-1; i++)
                {
                    v = v+Math.Abs(inva[k,i]);
                }
                nrminfinva = Math.Max(nrminfinva, v);
            }
            
            //
            // result
            //
            rc1 = nrm1inva*nrm1a;
            rcinf = nrminfinva*nrminfa;
        }


        /*************************************************************************
        Copy
        *************************************************************************/
        private static void cmatrixmakeacopy(ref AP.Complex[,] a,
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
        Generate matrix with given condition number C (2-norm)
        *************************************************************************/
        private static void cmatrixgenzero(ref AP.Complex[,] a0,
            int n)
        {
            int i = 0;
            int j = 0;

            a0 = new AP.Complex[n, n];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a0[i,j] = 0;
                }
            }
        }


        /*************************************************************************
        triangular inverse
        *************************************************************************/
        private static bool cmatrixinvmattr(ref AP.Complex[,] a,
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
        private static bool cmatrixinvmatlu(ref AP.Complex[,] a,
            ref int[] pivots,
            int n)
        {
            bool result = new bool();
            AP.Complex[] work = new AP.Complex[0];
            int i = 0;
            int iws = 0;
            int j = 0;
            int jb = 0;
            int jj = 0;
            int jp = 0;
            AP.Complex v = 0;
            int i_ = 0;

            result = true;
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                return result;
            }
            work = new AP.Complex[n-1+1];
            
            //
            // Form inv(U)
            //
            if( !cmatrixinvmattr(ref a, n, true, false) )
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
        private static bool cmatrixinvmat(ref AP.Complex[,] a,
            int n)
        {
            bool result = new bool();
            int[] pivots = new int[0];

            trfac.cmatrixlu(ref a, n, n, ref pivots);
            result = cmatrixinvmatlu(ref a, ref pivots, n);
            return result;
        }


        /*************************************************************************
        reference RCond
        *************************************************************************/
        private static void cmatrixrefrcond(ref AP.Complex[,] a,
            int n,
            ref double rc1,
            ref double rcinf)
        {
            AP.Complex[,] inva = new AP.Complex[0,0];
            double nrm1a = 0;
            double nrminfa = 0;
            double nrm1inva = 0;
            double nrminfinva = 0;
            double v = 0;
            int k = 0;
            int i = 0;

            
            //
            // inv A
            //
            cmatrixmakeacopy(ref a, n, n, ref inva);
            if( !cmatrixinvmat(ref inva, n) )
            {
                rc1 = 0;
                rcinf = 0;
                return;
            }
            
            //
            // norm A
            //
            nrm1a = 0;
            nrminfa = 0;
            for(k=0; k<=n-1; k++)
            {
                v = 0;
                for(i=0; i<=n-1; i++)
                {
                    v = v+AP.Math.AbsComplex(a[i,k]);
                }
                nrm1a = Math.Max(nrm1a, v);
                v = 0;
                for(i=0; i<=n-1; i++)
                {
                    v = v+AP.Math.AbsComplex(a[k,i]);
                }
                nrminfa = Math.Max(nrminfa, v);
            }
            
            //
            // norm inv A
            //
            nrm1inva = 0;
            nrminfinva = 0;
            for(k=0; k<=n-1; k++)
            {
                v = 0;
                for(i=0; i<=n-1; i++)
                {
                    v = v+AP.Math.AbsComplex(inva[i,k]);
                }
                nrm1inva = Math.Max(nrm1inva, v);
                v = 0;
                for(i=0; i<=n-1; i++)
                {
                    v = v+AP.Math.AbsComplex(inva[k,i]);
                }
                nrminfinva = Math.Max(nrminfinva, v);
            }
            
            //
            // result
            //
            rc1 = nrm1inva*nrm1a;
            rcinf = nrminfinva*nrminfa;
        }


        /*************************************************************************
        Returns True for successful test, False - for failed test
        *************************************************************************/
        private static bool testrmatrixrcond(int maxn,
            int passcount)
        {
            bool result = new bool();
            double[,] a = new double[0,0];
            double[,] lua = new double[0,0];
            int[] p = new int[0];
            int n = 0;
            int i = 0;
            int j = 0;
            int pass = 0;
            bool err50 = new bool();
            bool err90 = new bool();
            bool errspec = new bool();
            bool errless = new bool();
            double erc1 = 0;
            double ercinf = 0;
            double[] q50 = new double[0];
            double[] q90 = new double[0];
            double v = 0;

            err50 = false;
            err90 = false;
            errless = false;
            errspec = false;
            q50 = new double[3+1];
            q90 = new double[3+1];
            for(n=1; n<=maxn; n++)
            {
                
                //
                // special test for zero matrix
                //
                rmatrixgenzero(ref a, n);
                rmatrixmakeacopy(ref a, n, n, ref lua);
                trfac.rmatrixlu(ref lua, n, n, ref p);
                errspec = errspec | (double)(rcond.rmatrixrcond1(a, n))!=(double)(0);
                errspec = errspec | (double)(rcond.rmatrixrcondinf(a, n))!=(double)(0);
                errspec = errspec | (double)(rcond.rmatrixlurcond1(ref lua, n))!=(double)(0);
                errspec = errspec | (double)(rcond.rmatrixlurcondinf(ref lua, n))!=(double)(0);
                
                //
                // general test
                //
                a = new double[n-1+1, n-1+1];
                for(i=0; i<=3; i++)
                {
                    q50[i] = 0;
                    q90[i] = 0;
                }
                for(pass=1; pass<=passcount; pass++)
                {
                    matgen.rmatrixrndcond(n, Math.Exp(AP.Math.RandomReal()*Math.Log(1000)), ref a);
                    rmatrixmakeacopy(ref a, n, n, ref lua);
                    trfac.rmatrixlu(ref lua, n, n, ref p);
                    rmatrixrefrcond(ref a, n, ref erc1, ref ercinf);
                    
                    //
                    // 1-norm, normal
                    //
                    v = 1/rcond.rmatrixrcond1(a, n);
                    if( (double)(v)>=(double)(threshold50*erc1) )
                    {
                        q50[0] = q50[0]+(double)(1)/(double)(passcount);
                    }
                    if( (double)(v)>=(double)(threshold90*erc1) )
                    {
                        q90[0] = q90[0]+(double)(1)/(double)(passcount);
                    }
                    errless = errless | (double)(v)>(double)(erc1*1.001);
                    
                    //
                    // 1-norm, LU
                    //
                    v = 1/rcond.rmatrixlurcond1(ref lua, n);
                    if( (double)(v)>=(double)(threshold50*erc1) )
                    {
                        q50[1] = q50[1]+(double)(1)/(double)(passcount);
                    }
                    if( (double)(v)>=(double)(threshold90*erc1) )
                    {
                        q90[1] = q90[1]+(double)(1)/(double)(passcount);
                    }
                    errless = errless | (double)(v)>(double)(erc1*1.001);
                    
                    //
                    // Inf-norm, normal
                    //
                    v = 1/rcond.rmatrixrcondinf(a, n);
                    if( (double)(v)>=(double)(threshold50*ercinf) )
                    {
                        q50[2] = q50[2]+(double)(1)/(double)(passcount);
                    }
                    if( (double)(v)>=(double)(threshold90*ercinf) )
                    {
                        q90[2] = q90[2]+(double)(1)/(double)(passcount);
                    }
                    errless = errless | (double)(v)>(double)(ercinf*1.001);
                    
                    //
                    // Inf-norm, LU
                    //
                    v = 1/rcond.rmatrixlurcondinf(ref lua, n);
                    if( (double)(v)>=(double)(threshold50*ercinf) )
                    {
                        q50[3] = q50[3]+(double)(1)/(double)(passcount);
                    }
                    if( (double)(v)>=(double)(threshold90*ercinf) )
                    {
                        q90[3] = q90[3]+(double)(1)/(double)(passcount);
                    }
                    errless = errless | (double)(v)>(double)(ercinf*1.001);
                }
                for(i=0; i<=3; i++)
                {
                    err50 = err50 | (double)(q50[i])<(double)(0.50);
                    err90 = err90 | (double)(q90[i])<(double)(0.90);
                }
                
                //
                // degenerate matrix test
                //
                if( n>=3 )
                {
                    a = new double[n, n];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 0.0;
                        }
                    }
                    a[0,0] = 1;
                    a[n-1,n-1] = 1;
                    errspec = errspec | (double)(rcond.rmatrixrcond1(a, n))!=(double)(0);
                    errspec = errspec | (double)(rcond.rmatrixrcondinf(a, n))!=(double)(0);
                    errspec = errspec | (double)(rcond.rmatrixlurcond1(ref a, n))!=(double)(0);
                    errspec = errspec | (double)(rcond.rmatrixlurcondinf(ref a, n))!=(double)(0);
                }
                
                //
                // near-degenerate matrix test
                //
                if( n>=2 )
                {
                    a = new double[n, n];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 0.0;
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        a[i,i] = 1;
                    }
                    i = AP.Math.RandomInteger(n);
                    a[i,i] = 0.1*AP.Math.MaxRealNumber;
                    errspec = errspec | (double)(rcond.rmatrixrcond1(a, n))!=(double)(0);
                    errspec = errspec | (double)(rcond.rmatrixrcondinf(a, n))!=(double)(0);
                    errspec = errspec | (double)(rcond.rmatrixlurcond1(ref a, n))!=(double)(0);
                    errspec = errspec | (double)(rcond.rmatrixlurcondinf(ref a, n))!=(double)(0);
                }
            }
            
            //
            // report
            //
            result = !(err50 | err90 | errless | errspec);
            return result;
        }


        /*************************************************************************
        Returns True for successful test, False - for failed test
        *************************************************************************/
        private static bool testspdmatrixrcond(int maxn,
            int passcount)
        {
            bool result = new bool();
            double[,] a = new double[0,0];
            double[,] cha = new double[0,0];
            int[] p = new int[0];
            int n = 0;
            int i = 0;
            int j = 0;
            int pass = 0;
            bool err50 = new bool();
            bool err90 = new bool();
            bool errspec = new bool();
            bool errless = new bool();
            bool isupper = new bool();
            double erc1 = 0;
            double ercinf = 0;
            double[] q50 = new double[0];
            double[] q90 = new double[0];
            double v = 0;

            err50 = false;
            err90 = false;
            errless = false;
            errspec = false;
            q50 = new double[2];
            q90 = new double[2];
            for(n=1; n<=maxn; n++)
            {
                isupper = (double)(AP.Math.RandomReal())>(double)(0.5);
                
                //
                // general test
                //
                a = new double[n, n];
                for(i=0; i<=1; i++)
                {
                    q50[i] = 0;
                    q90[i] = 0;
                }
                for(pass=1; pass<=passcount; pass++)
                {
                    matgen.spdmatrixrndcond(n, Math.Exp(AP.Math.RandomReal()*Math.Log(1000)), ref a);
                    rmatrixrefrcond(ref a, n, ref erc1, ref ercinf);
                    rmatrixdrophalf(ref a, n, isupper);
                    rmatrixmakeacopy(ref a, n, n, ref cha);
                    trfac.spdmatrixcholesky(ref cha, n, isupper);
                    
                    //
                    // normal
                    //
                    v = 1/rcond.spdmatrixrcond(a, n, isupper);
                    if( (double)(v)>=(double)(threshold50*erc1) )
                    {
                        q50[0] = q50[0]+(double)(1)/(double)(passcount);
                    }
                    if( (double)(v)>=(double)(threshold90*erc1) )
                    {
                        q90[0] = q90[0]+(double)(1)/(double)(passcount);
                    }
                    errless = errless | (double)(v)>(double)(erc1*1.001);
                    
                    //
                    // Cholesky
                    //
                    v = 1/rcond.spdmatrixcholeskyrcond(ref cha, n, isupper);
                    if( (double)(v)>=(double)(threshold50*erc1) )
                    {
                        q50[1] = q50[1]+(double)(1)/(double)(passcount);
                    }
                    if( (double)(v)>=(double)(threshold90*erc1) )
                    {
                        q90[1] = q90[1]+(double)(1)/(double)(passcount);
                    }
                    errless = errless | (double)(v)>(double)(erc1*1.001);
                }
                for(i=0; i<=1; i++)
                {
                    err50 = err50 | (double)(q50[i])<(double)(0.50);
                    err90 = err90 | (double)(q90[i])<(double)(0.90);
                }
                
                //
                // degenerate matrix test
                //
                if( n>=3 )
                {
                    a = new double[n, n];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 0.0;
                        }
                    }
                    a[0,0] = 1;
                    a[n-1,n-1] = 1;
                    errspec = errspec | (double)(rcond.spdmatrixrcond(a, n, isupper))!=(double)(-1);
                    errspec = errspec | (double)(rcond.spdmatrixcholeskyrcond(ref a, n, isupper))!=(double)(0);
                }
                
                //
                // near-degenerate matrix test
                //
                if( n>=2 )
                {
                    a = new double[n, n];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 0.0;
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        a[i,i] = 1;
                    }
                    i = AP.Math.RandomInteger(n);
                    a[i,i] = 0.1*AP.Math.MaxRealNumber;
                    errspec = errspec | (double)(rcond.spdmatrixrcond(a, n, isupper))!=(double)(0);
                    errspec = errspec | (double)(rcond.spdmatrixcholeskyrcond(ref a, n, isupper))!=(double)(0);
                }
            }
            
            //
            // report
            //
            result = !(err50 | err90 | errless | errspec);
            return result;
        }


        /*************************************************************************
        Returns True for successful test, False - for failed test
        *************************************************************************/
        private static bool testcmatrixrcond(int maxn,
            int passcount)
        {
            bool result = new bool();
            AP.Complex[,] a = new AP.Complex[0,0];
            AP.Complex[,] lua = new AP.Complex[0,0];
            int[] p = new int[0];
            int n = 0;
            int i = 0;
            int j = 0;
            int pass = 0;
            bool err50 = new bool();
            bool err90 = new bool();
            bool errless = new bool();
            bool errspec = new bool();
            double erc1 = 0;
            double ercinf = 0;
            double[] q50 = new double[0];
            double[] q90 = new double[0];
            double v = 0;

            q50 = new double[3+1];
            q90 = new double[3+1];
            err50 = false;
            err90 = false;
            errless = false;
            errspec = false;
            
            //
            // process
            //
            for(n=1; n<=maxn; n++)
            {
                
                //
                // special test for zero matrix
                //
                cmatrixgenzero(ref a, n);
                cmatrixmakeacopy(ref a, n, n, ref lua);
                trfac.cmatrixlu(ref lua, n, n, ref p);
                errspec = errspec | (double)(rcond.cmatrixrcond1(a, n))!=(double)(0);
                errspec = errspec | (double)(rcond.cmatrixrcondinf(a, n))!=(double)(0);
                errspec = errspec | (double)(rcond.cmatrixlurcond1(ref lua, n))!=(double)(0);
                errspec = errspec | (double)(rcond.cmatrixlurcondinf(ref lua, n))!=(double)(0);
                
                //
                // general test
                //
                a = new AP.Complex[n-1+1, n-1+1];
                for(i=0; i<=3; i++)
                {
                    q50[i] = 0;
                    q90[i] = 0;
                }
                for(pass=1; pass<=passcount; pass++)
                {
                    matgen.cmatrixrndcond(n, Math.Exp(AP.Math.RandomReal()*Math.Log(1000)), ref a);
                    cmatrixmakeacopy(ref a, n, n, ref lua);
                    trfac.cmatrixlu(ref lua, n, n, ref p);
                    cmatrixrefrcond(ref a, n, ref erc1, ref ercinf);
                    
                    //
                    // 1-norm, normal
                    //
                    v = 1/rcond.cmatrixrcond1(a, n);
                    if( (double)(v)>=(double)(threshold50*erc1) )
                    {
                        q50[0] = q50[0]+(double)(1)/(double)(passcount);
                    }
                    if( (double)(v)>=(double)(threshold90*erc1) )
                    {
                        q90[0] = q90[0]+(double)(1)/(double)(passcount);
                    }
                    errless = errless | (double)(v)>(double)(erc1*1.001);
                    
                    //
                    // 1-norm, LU
                    //
                    v = 1/rcond.cmatrixlurcond1(ref lua, n);
                    if( (double)(v)>=(double)(threshold50*erc1) )
                    {
                        q50[1] = q50[1]+(double)(1)/(double)(passcount);
                    }
                    if( (double)(v)>=(double)(threshold90*erc1) )
                    {
                        q90[1] = q90[1]+(double)(1)/(double)(passcount);
                    }
                    errless = errless | (double)(v)>(double)(erc1*1.001);
                    
                    //
                    // Inf-norm, normal
                    //
                    v = 1/rcond.cmatrixrcondinf(a, n);
                    if( (double)(v)>=(double)(threshold50*ercinf) )
                    {
                        q50[2] = q50[2]+(double)(1)/(double)(passcount);
                    }
                    if( (double)(v)>=(double)(threshold90*ercinf) )
                    {
                        q90[2] = q90[2]+(double)(1)/(double)(passcount);
                    }
                    errless = errless | (double)(v)>(double)(ercinf*1.001);
                    
                    //
                    // Inf-norm, LU
                    //
                    v = 1/rcond.cmatrixlurcondinf(ref lua, n);
                    if( (double)(v)>=(double)(threshold50*ercinf) )
                    {
                        q50[3] = q50[3]+(double)(1)/(double)(passcount);
                    }
                    if( (double)(v)>=(double)(threshold90*ercinf) )
                    {
                        q90[3] = q90[3]+(double)(1)/(double)(passcount);
                    }
                    errless = errless | (double)(v)>(double)(ercinf*1.001);
                }
                for(i=0; i<=3; i++)
                {
                    err50 = err50 | (double)(q50[i])<(double)(0.50);
                    err90 = err90 | (double)(q90[i])<(double)(0.90);
                }
                
                //
                // degenerate matrix test
                //
                if( n>=3 )
                {
                    a = new AP.Complex[n, n];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 0.0;
                        }
                    }
                    a[0,0] = 1;
                    a[n-1,n-1] = 1;
                    errspec = errspec | (double)(rcond.cmatrixrcond1(a, n))!=(double)(0);
                    errspec = errspec | (double)(rcond.cmatrixrcondinf(a, n))!=(double)(0);
                    errspec = errspec | (double)(rcond.cmatrixlurcond1(ref a, n))!=(double)(0);
                    errspec = errspec | (double)(rcond.cmatrixlurcondinf(ref a, n))!=(double)(0);
                }
                
                //
                // near-degenerate matrix test
                //
                if( n>=2 )
                {
                    a = new AP.Complex[n, n];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 0.0;
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        a[i,i] = 1;
                    }
                    i = AP.Math.RandomInteger(n);
                    a[i,i] = 0.1*AP.Math.MaxRealNumber;
                    errspec = errspec | (double)(rcond.cmatrixrcond1(a, n))!=(double)(0);
                    errspec = errspec | (double)(rcond.cmatrixrcondinf(a, n))!=(double)(0);
                    errspec = errspec | (double)(rcond.cmatrixlurcond1(ref a, n))!=(double)(0);
                    errspec = errspec | (double)(rcond.cmatrixlurcondinf(ref a, n))!=(double)(0);
                }
            }
            
            //
            // report
            //
            result = !(err50 | err90 | errless | errspec);
            return result;
        }


        /*************************************************************************
        Returns True for successful test, False - for failed test
        *************************************************************************/
        private static bool testhpdmatrixrcond(int maxn,
            int passcount)
        {
            bool result = new bool();
            AP.Complex[,] a = new AP.Complex[0,0];
            AP.Complex[,] cha = new AP.Complex[0,0];
            int[] p = new int[0];
            int n = 0;
            int i = 0;
            int j = 0;
            int pass = 0;
            bool err50 = new bool();
            bool err90 = new bool();
            bool errspec = new bool();
            bool errless = new bool();
            bool isupper = new bool();
            double erc1 = 0;
            double ercinf = 0;
            double[] q50 = new double[0];
            double[] q90 = new double[0];
            double v = 0;

            err50 = false;
            err90 = false;
            errless = false;
            errspec = false;
            q50 = new double[2];
            q90 = new double[2];
            for(n=1; n<=maxn; n++)
            {
                isupper = (double)(AP.Math.RandomReal())>(double)(0.5);
                
                //
                // general test
                //
                a = new AP.Complex[n, n];
                for(i=0; i<=1; i++)
                {
                    q50[i] = 0;
                    q90[i] = 0;
                }
                for(pass=1; pass<=passcount; pass++)
                {
                    matgen.hpdmatrixrndcond(n, Math.Exp(AP.Math.RandomReal()*Math.Log(1000)), ref a);
                    cmatrixrefrcond(ref a, n, ref erc1, ref ercinf);
                    cmatrixdrophalf(ref a, n, isupper);
                    cmatrixmakeacopy(ref a, n, n, ref cha);
                    trfac.hpdmatrixcholesky(ref cha, n, isupper);
                    
                    //
                    // normal
                    //
                    v = 1/rcond.hpdmatrixrcond(a, n, isupper);
                    if( (double)(v)>=(double)(threshold50*erc1) )
                    {
                        q50[0] = q50[0]+(double)(1)/(double)(passcount);
                    }
                    if( (double)(v)>=(double)(threshold90*erc1) )
                    {
                        q90[0] = q90[0]+(double)(1)/(double)(passcount);
                    }
                    errless = errless | (double)(v)>(double)(erc1*1.001);
                    
                    //
                    // Cholesky
                    //
                    v = 1/rcond.hpdmatrixcholeskyrcond(ref cha, n, isupper);
                    if( (double)(v)>=(double)(threshold50*erc1) )
                    {
                        q50[1] = q50[1]+(double)(1)/(double)(passcount);
                    }
                    if( (double)(v)>=(double)(threshold90*erc1) )
                    {
                        q90[1] = q90[1]+(double)(1)/(double)(passcount);
                    }
                    errless = errless | (double)(v)>(double)(erc1*1.001);
                }
                for(i=0; i<=1; i++)
                {
                    err50 = err50 | (double)(q50[i])<(double)(0.50);
                    err90 = err90 | (double)(q90[i])<(double)(0.90);
                }
                
                //
                // degenerate matrix test
                //
                if( n>=3 )
                {
                    a = new AP.Complex[n, n];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 0.0;
                        }
                    }
                    a[0,0] = 1;
                    a[n-1,n-1] = 1;
                    errspec = errspec | (double)(rcond.hpdmatrixrcond(a, n, isupper))!=(double)(-1);
                    errspec = errspec | (double)(rcond.hpdmatrixcholeskyrcond(ref a, n, isupper))!=(double)(0);
                }
                
                //
                // near-degenerate matrix test
                //
                if( n>=2 )
                {
                    a = new AP.Complex[n, n];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 0.0;
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        a[i,i] = 1;
                    }
                    i = AP.Math.RandomInteger(n);
                    a[i,i] = 0.1*AP.Math.MaxRealNumber;
                    errspec = errspec | (double)(rcond.hpdmatrixrcond(a, n, isupper))!=(double)(0);
                    errspec = errspec | (double)(rcond.hpdmatrixcholeskyrcond(ref a, n, isupper))!=(double)(0);
                }
            }
            
            //
            // report
            //
            result = !(err50 | err90 | errless | errspec);
            return result;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testrcondunit_test_silent()
        {
            bool result = new bool();

            result = testrcond(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testrcondunit_test()
        {
            bool result = new bool();

            result = testrcond(false);
            return result;
        }
    }
}
