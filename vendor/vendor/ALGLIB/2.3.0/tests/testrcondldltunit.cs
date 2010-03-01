
using System;

namespace alglib
{
    public class testrcondldltunit
    {
        public static bool testrcondldlt(bool silent)
        {
            bool result = new bool();
            double[,] a = new double[0,0];
            double[,] a2 = new double[0,0];
            double[,] a3 = new double[0,0];
            int minij = 0;
            int[] p = new int[0];
            int n = 0;
            int maxn = 0;
            int i = 0;
            int j = 0;
            int pass = 0;
            int passcount = 0;
            bool waserrors = new bool();
            bool err50 = new bool();
            bool err90 = new bool();
            bool errless = new bool();
            double erc1 = 0;
            double ercinf = 0;
            double[] q50 = new double[0];
            double[] q90 = new double[0];
            double v = 0;
            double threshold50 = 0;
            double threshold90 = 0;
            int htask = 0;
            int mtask = 0;
            bool upperin = new bool();

            waserrors = false;
            err50 = false;
            err90 = false;
            errless = false;
            maxn = 10;
            passcount = 100;
            threshold50 = 0.5;
            threshold90 = 0.1;
            q50 = new double[1+1];
            q90 = new double[1+1];
            
            //
            // process
            //
            for(n=1; n<=maxn; n++)
            {
                a = new double[n-1+1, n-1+1];
                a2 = new double[n-1+1, n-1+1];
                a3 = new double[n-1+1, n-1+1];
                for(htask=0; htask<=1; htask++)
                {
                    for(mtask=2; mtask<=2; mtask++)
                    {
                        for(i=0; i<=1; i++)
                        {
                            q50[i] = 0;
                            q90[i] = 0;
                        }
                        for(pass=1; pass<=passcount; pass++)
                        {
                            upperin = htask==0;
                            
                            //
                            // Prepare task:
                            // * A contains symmetric matrix
                            // * A2, A3 contains its upper (or lower) half
                            //
                            generatematrix(ref a, n, mtask);
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    a2[i,j] = a[i,j];
                                    a3[i,j] = a[i,j];
                                }
                            }
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    if( upperin )
                                    {
                                        if( j<i )
                                        {
                                            a2[i,j] = 0;
                                            a3[i,j] = 0;
                                        }
                                    }
                                    else
                                    {
                                        if( i<j )
                                        {
                                            a2[i,j] = 0;
                                            a3[i,j] = 0;
                                        }
                                    }
                                }
                            }
                            refrcond(ref a, n, ref erc1, ref ercinf);
                            
                            //
                            // normal
                            //
                            v = srcond.smatrixrcond(ref a2, n, upperin);
                            if( (double)(v)<=(double)(erc1/threshold50) )
                            {
                                q50[0] = q50[0]+(double)(1)/(double)(passcount);
                            }
                            if( (double)(v)<=(double)(erc1/threshold90) )
                            {
                                q90[0] = q90[0]+(double)(1)/(double)(passcount);
                            }
                            errless = errless | (double)(v)<(double)(erc1/1.001);
                            
                            //
                            // LDLT
                            //
                            ldlt.smatrixldlt(ref a3, n, upperin, ref p);
                            v = srcond.smatrixldltrcond(ref a3, ref p, n, upperin);
                            if( (double)(v)<=(double)(erc1/threshold50) )
                            {
                                q50[1] = q50[1]+(double)(1)/(double)(passcount);
                            }
                            if( (double)(v)<=(double)(erc1/threshold90) )
                            {
                                q90[1] = q90[1]+(double)(1)/(double)(passcount);
                            }
                            errless = errless | (double)(v)<(double)(erc1/1.001);
                        }
                        for(i=0; i<=1; i++)
                        {
                            err50 = err50 | (double)(q50[i])<(double)(0.50);
                            err90 = err90 | (double)(q90[i])<(double)(0.90);
                        }
                    }
                }
            }
            
            //
            // report
            //
            waserrors = err50 | err90 | errless;
            if( !silent )
            {
                System.Console.Write("TESTING RCOND (LDLT)");
                System.Console.WriteLine();
                System.Console.Write("50");
                System.Console.Write("%");
                System.Console.Write(" within [0.5*cond,cond]:              ");
                if( err50 | errless )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("90");
                System.Console.Write("%");
                System.Console.Write(" within [0.1*cond,cond]               ");
                if( err90 | errless )
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


        private static void generatematrix(ref double[,] a,
            int n,
            int task)
        {
            int i = 0;
            int j = 0;

            if( task==0 )
            {
                
                //
                // Zero matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a[i,j] = 0;
                    }
                }
            }
            if( task==1 )
            {
                
                //
                // Sparse matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    for(j=i+1; j<=n-1; j++)
                    {
                        if( (double)(AP.Math.RandomReal())>(double)(0.95) )
                        {
                            a[i,j] = 2*AP.Math.RandomReal()-1;
                        }
                        else
                        {
                            a[i,j] = 0;
                        }
                        a[j,i] = a[i,j];
                    }
                    if( (double)(AP.Math.RandomReal())>(double)(0.95) )
                    {
                        a[i,i] = (2*AP.Math.RandomInteger(2)-1)*(0.8+AP.Math.RandomReal());
                    }
                    else
                    {
                        a[i,i] = 0;
                    }
                }
            }
            if( task==2 )
            {
                
                //
                // Dense matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    for(j=i+1; j<=n-1; j++)
                    {
                        a[i,j] = 2*AP.Math.RandomReal()-1;
                        a[j,i] = a[i,j];
                    }
                    a[i,i] = (2*AP.Math.RandomInteger(2)-1)*(0.7+AP.Math.RandomReal());
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
        reference RCond
        *************************************************************************/
        private static void refrcond(ref double[,] a,
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
            makeacopy(ref a, n, n, ref inva);
            if( !invmat(ref inva, n) )
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
            rc1 = 1.0/(nrm1inva*nrm1a);
            rcinf = 1.0/(nrminfinva*nrminfa);
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testrcondldltunit_test_silent()
        {
            bool result = new bool();

            result = testrcondldlt(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testrcondldltunit_test()
        {
            bool result = new bool();

            result = testrcondldlt(false);
            return result;
        }
    }
}
