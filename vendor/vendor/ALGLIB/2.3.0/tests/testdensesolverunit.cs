
using System;

namespace alglib
{
    public class testdensesolverunit
    {
        /*************************************************************************
        Test
        *************************************************************************/
        public static bool testdensesolver(bool silent)
        {
            bool result = new bool();
            double[,] a = new double[0,0];
            double[,] lua = new double[0,0];
            double[,] atmp = new double[0,0];
            int[] p = new int[0];
            double[,] xe = new double[0,0];
            double[,] b = new double[0,0];
            double[] bv = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            int n = 0;
            int m = 0;
            int pass = 0;
            int taskkind = 0;
            double mx = 0;
            double v = 0;
            double verr = 0;
            int info = 0;
            densesolver.densesolverreport rep = new densesolver.densesolverreport();
            densesolver.densesolverlsreport repls = new densesolver.densesolverlsreport();
            double[,] x = new double[0,0];
            double[] xv = new double[0];
            double[] y = new double[0];
            double[] tx = new double[0];
            int maxn = 0;
            int maxm = 0;
            int passcount = 0;
            double threshold = 0;
            bool rerrors = new bool();
            bool cerrors = new bool();
            bool spderrors = new bool();
            bool hpderrors = new bool();
            bool rfserrors = new bool();
            bool waserrors = new bool();

            maxn = 10;
            maxm = 5;
            passcount = 5;
            threshold = 10000*AP.Math.MachineEpsilon;
            rfserrors = false;
            rerrors = false;
            cerrors = false;
            spderrors = false;
            hpderrors = false;
            testrsolver(maxn, maxm, passcount, threshold, ref rerrors, ref rfserrors);
            testspdsolver(maxn, maxm, passcount, threshold, ref spderrors, ref rfserrors);
            testcsolver(maxn, maxm, passcount, threshold, ref cerrors, ref rfserrors);
            testhpdsolver(maxn, maxm, passcount, threshold, ref hpderrors, ref rfserrors);
            waserrors = rerrors | cerrors | spderrors | hpderrors | rfserrors;
            if( !silent )
            {
                System.Console.Write("TESTING DENSE SOLVER");
                System.Console.WriteLine();
                System.Console.Write("* REAL:                                   ");
                if( rerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* COMPLEX:                                ");
                if( cerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* SPD:                                    ");
                if( spderrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* HPD:                                    ");
                if( hpderrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* ITERATIVE IMPROVEMENT:                  ");
                if( rfserrors )
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
            }
            result = !waserrors;
            return result;
        }


        /*************************************************************************
        Checks whether solver results are correct solution.
        Returns True on success.
        *************************************************************************/
        private static bool rmatrixchecksolutionm(ref double[,] xe,
            int n,
            int m,
            double threshold,
            int info,
            ref densesolver.densesolverreport rep,
            ref double[,] xs)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;

            result = true;
            if( info<=0 )
            {
                result = false;
            }
            else
            {
                result = result & !((double)(rep.r1)<(double)(100*AP.Math.MachineEpsilon) | (double)(rep.r1)>(double)(1+1000*AP.Math.MachineEpsilon));
                result = result & !((double)(rep.rinf)<(double)(100*AP.Math.MachineEpsilon) | (double)(rep.rinf)>(double)(1+1000*AP.Math.MachineEpsilon));
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        result = result & (double)(Math.Abs(xe[i,j]-xs[i,j]))<=(double)(threshold);
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Checks whether solver results are correct solution.
        Returns True on success.
        *************************************************************************/
        private static bool rmatrixchecksolution(ref double[,] xe,
            int n,
            double threshold,
            int info,
            ref densesolver.densesolverreport rep,
            ref double[] xs)
        {
            bool result = new bool();
            double[,] xsm = new double[0,0];
            int i_ = 0;

            xsm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                xsm[i_,0] = xs[i_];
            }
            result = rmatrixchecksolutionm(ref xe, n, 1, threshold, info, ref rep, ref xsm);
            return result;
        }


        /*************************************************************************
        Checks whether solver results indicate singular matrix.
        Returns True on success.
        *************************************************************************/
        private static bool rmatrixchecksingularm(int n,
            int m,
            int info,
            ref densesolver.densesolverreport rep,
            ref double[,] xs)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;

            result = true;
            if( info!=-3 & info!=1 )
            {
                result = false;
            }
            else
            {
                result = result & !((double)(rep.r1)<(double)(0) | (double)(rep.r1)>(double)(1000*AP.Math.MachineEpsilon));
                result = result & !((double)(rep.rinf)<(double)(0) | (double)(rep.rinf)>(double)(1000*AP.Math.MachineEpsilon));
                if( info==-3 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            result = result & (double)(xs[i,j])==(double)(0);
                        }
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Checks whether solver results indicate singular matrix.
        Returns True on success.
        *************************************************************************/
        private static bool rmatrixchecksingular(int n,
            int info,
            ref densesolver.densesolverreport rep,
            ref double[] xs)
        {
            bool result = new bool();
            double[,] xsm = new double[0,0];
            int i_ = 0;

            xsm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                xsm[i_,0] = xs[i_];
            }
            result = rmatrixchecksingularm(n, 1, info, ref rep, ref xsm);
            return result;
        }


        /*************************************************************************
        Checks whether solver results are correct solution.
        Returns True on success.
        *************************************************************************/
        private static bool cmatrixchecksolutionm(ref AP.Complex[,] xe,
            int n,
            int m,
            double threshold,
            int info,
            ref densesolver.densesolverreport rep,
            ref AP.Complex[,] xs)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;

            result = true;
            if( info<=0 )
            {
                result = false;
            }
            else
            {
                result = result & !((double)(rep.r1)<(double)(100*AP.Math.MachineEpsilon) | (double)(rep.r1)>(double)(1+1000*AP.Math.MachineEpsilon));
                result = result & !((double)(rep.rinf)<(double)(100*AP.Math.MachineEpsilon) | (double)(rep.rinf)>(double)(1+1000*AP.Math.MachineEpsilon));
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        result = result & (double)(AP.Math.AbsComplex(xe[i,j]-xs[i,j]))<=(double)(threshold);
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Checks whether solver results are correct solution.
        Returns True on success.
        *************************************************************************/
        private static bool cmatrixchecksolution(ref AP.Complex[,] xe,
            int n,
            double threshold,
            int info,
            ref densesolver.densesolverreport rep,
            ref AP.Complex[] xs)
        {
            bool result = new bool();
            AP.Complex[,] xsm = new AP.Complex[0,0];
            int i_ = 0;

            xsm = new AP.Complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                xsm[i_,0] = xs[i_];
            }
            result = cmatrixchecksolutionm(ref xe, n, 1, threshold, info, ref rep, ref xsm);
            return result;
        }


        /*************************************************************************
        Checks whether solver results indicate singular matrix.
        Returns True on success.
        *************************************************************************/
        private static bool cmatrixchecksingularm(int n,
            int m,
            int info,
            ref densesolver.densesolverreport rep,
            ref AP.Complex[,] xs)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;

            result = true;
            if( info!=-3 & info!=1 )
            {
                result = false;
            }
            else
            {
                result = result & !((double)(rep.r1)<(double)(0) | (double)(rep.r1)>(double)(1000*AP.Math.MachineEpsilon));
                result = result & !((double)(rep.rinf)<(double)(0) | (double)(rep.rinf)>(double)(1000*AP.Math.MachineEpsilon));
                if( info==-3 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            result = result & xs[i,j]==0;
                        }
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Checks whether solver results indicate singular matrix.
        Returns True on success.
        *************************************************************************/
        private static bool cmatrixchecksingular(int n,
            int info,
            ref densesolver.densesolverreport rep,
            ref AP.Complex[] xs)
        {
            bool result = new bool();
            AP.Complex[,] xsm = new AP.Complex[0,0];
            int i_ = 0;

            xsm = new AP.Complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                xsm[i_,0] = xs[i_];
            }
            result = cmatrixchecksingularm(n, 1, info, ref rep, ref xsm);
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
        Real test
        *************************************************************************/
        private static void testrsolver(int maxn,
            int maxm,
            int passcount,
            double threshold,
            ref bool rerrors,
            ref bool rfserrors)
        {
            double[,] a = new double[0,0];
            double[,] lua = new double[0,0];
            double[,] atmp = new double[0,0];
            int[] p = new int[0];
            double[,] xe = new double[0,0];
            double[,] b = new double[0,0];
            double[] bv = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            int n = 0;
            int m = 0;
            int pass = 0;
            int taskkind = 0;
            double mx = 0;
            double v = 0;
            double verr = 0;
            int info = 0;
            densesolver.densesolverreport rep = new densesolver.densesolverreport();
            densesolver.densesolverlsreport repls = new densesolver.densesolverlsreport();
            double[,] x = new double[0,0];
            double[] xv = new double[0];
            double[] y = new double[0];
            double[] tx = new double[0];
            int i_ = 0;
            int i1_ = 0;

            
            //
            // General square matrices:
            // * test general solvers
            // * test least squares solver
            //
            for(pass=1; pass<=passcount; pass++)
            {
                for(n=1; n<=maxn; n++)
                {
                    for(m=1; m<=maxm; m++)
                    {
                        
                        //
                        // ********************************************************
                        // WELL CONDITIONED TASKS
                        // ability to find correct solution is tested
                        // ********************************************************
                        //
                        // 1. generate random well conditioned matrix A.
                        // 2. generate random solution vector xe
                        // 3. generate right part b=A*xe
                        // 4. test different methods on original A
                        //
                        matgen.rmatrixrndcond(n, 1000, ref a);
                        rmatrixmakeacopy(ref a, n, n, ref lua);
                        trfac.rmatrixlu(ref lua, n, n, ref p);
                        xe = new double[n, m];
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                xe[i,j] = 2*AP.Math.RandomReal()-1;
                            }
                        }
                        b = new double[n, m];
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                v = 0.0;
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    v += a[i,i_]*xe[i_,j];
                                }
                                b[i,j] = v;
                            }
                        }
                        
                        //
                        // Test solvers
                        //
                        info = 0;
                        unsetrep(ref rep);
                        unset2d(ref x);
                        densesolver.rmatrixsolvem(ref a, n, ref b, m, (double)(AP.Math.RandomReal())>(double)(0.5), ref info, ref rep, ref x);
                        rerrors = rerrors | !rmatrixchecksolutionm(ref xe, n, m, threshold, info, ref rep, ref x);
                        info = 0;
                        unsetrep(ref rep);
                        unset1d(ref xv);
                        bv = new double[n];
                        for(i_=0; i_<=n-1;i_++)
                        {
                            bv[i_] = b[i_,0];
                        }
                        densesolver.rmatrixsolve(ref a, n, ref bv, ref info, ref rep, ref xv);
                        rerrors = rerrors | !rmatrixchecksolution(ref xe, n, threshold, info, ref rep, ref xv);
                        info = 0;
                        unsetrep(ref rep);
                        unset2d(ref x);
                        densesolver.rmatrixlusolvem(ref lua, ref p, n, ref b, m, ref info, ref rep, ref x);
                        rerrors = rerrors | !rmatrixchecksolutionm(ref xe, n, m, threshold, info, ref rep, ref x);
                        info = 0;
                        unsetrep(ref rep);
                        unset1d(ref xv);
                        bv = new double[n];
                        for(i_=0; i_<=n-1;i_++)
                        {
                            bv[i_] = b[i_,0];
                        }
                        densesolver.rmatrixlusolve(ref lua, ref p, n, ref bv, ref info, ref rep, ref xv);
                        rerrors = rerrors | !rmatrixchecksolution(ref xe, n, threshold, info, ref rep, ref xv);
                        info = 0;
                        unsetrep(ref rep);
                        unset2d(ref x);
                        densesolver.rmatrixmixedsolvem(ref a, ref lua, ref p, n, ref b, m, ref info, ref rep, ref x);
                        rerrors = rerrors | !rmatrixchecksolutionm(ref xe, n, m, threshold, info, ref rep, ref x);
                        info = 0;
                        unsetrep(ref rep);
                        unset1d(ref xv);
                        bv = new double[n];
                        for(i_=0; i_<=n-1;i_++)
                        {
                            bv[i_] = b[i_,0];
                        }
                        densesolver.rmatrixmixedsolve(ref a, ref lua, ref p, n, ref bv, ref info, ref rep, ref xv);
                        rerrors = rerrors | !rmatrixchecksolution(ref xe, n, threshold, info, ref rep, ref xv);
                        
                        //
                        // Test DenseSolverRLS():
                        // * test on original system A*x = b
                        // * test on overdetermined system with the same solution: (A' A')'*x = (b' b')'
                        // * test on underdetermined system with the same solution: (A 0 0 0 ) * z = b
                        //
                        info = 0;
                        unsetlsrep(ref repls);
                        unset1d(ref xv);
                        bv = new double[n];
                        for(i_=0; i_<=n-1;i_++)
                        {
                            bv[i_] = b[i_,0];
                        }
                        densesolver.rmatrixsolvels(ref a, n, n, ref bv, 0.0, ref info, ref repls, ref xv);
                        if( info<=0 )
                        {
                            rerrors = true;
                        }
                        else
                        {
                            rerrors = rerrors | (double)(repls.r2)<(double)(100*AP.Math.MachineEpsilon) | (double)(repls.r2)>(double)(1+1000*AP.Math.MachineEpsilon);
                            rerrors = rerrors | repls.n!=n | repls.k!=0;
                            for(i=0; i<=n-1; i++)
                            {
                                rerrors = rerrors | (double)(Math.Abs(xe[i,0]-xv[i]))>(double)(threshold);
                            }
                        }
                        info = 0;
                        unsetlsrep(ref repls);
                        unset1d(ref xv);
                        bv = new double[2*n];
                        for(i_=0; i_<=n-1;i_++)
                        {
                            bv[i_] = b[i_,0];
                        }
                        i1_ = (0) - (n);
                        for(i_=n; i_<=2*n-1;i_++)
                        {
                            bv[i_] = b[i_+i1_,0];
                        }
                        atmp = new double[2*n, n];
                        blas.copymatrix(ref a, 0, n-1, 0, n-1, ref atmp, 0, n-1, 0, n-1);
                        blas.copymatrix(ref a, 0, n-1, 0, n-1, ref atmp, n, 2*n-1, 0, n-1);
                        densesolver.rmatrixsolvels(ref atmp, 2*n, n, ref bv, 0.0, ref info, ref repls, ref xv);
                        if( info<=0 )
                        {
                            rerrors = true;
                        }
                        else
                        {
                            rerrors = rerrors | (double)(repls.r2)<(double)(100*AP.Math.MachineEpsilon) | (double)(repls.r2)>(double)(1+1000*AP.Math.MachineEpsilon);
                            rerrors = rerrors | repls.n!=n | repls.k!=0;
                            for(i=0; i<=n-1; i++)
                            {
                                rerrors = rerrors | (double)(Math.Abs(xe[i,0]-xv[i]))>(double)(threshold);
                            }
                        }
                        info = 0;
                        unsetlsrep(ref repls);
                        unset1d(ref xv);
                        bv = new double[n];
                        for(i_=0; i_<=n-1;i_++)
                        {
                            bv[i_] = b[i_,0];
                        }
                        atmp = new double[n, 2*n];
                        blas.copymatrix(ref a, 0, n-1, 0, n-1, ref atmp, 0, n-1, 0, n-1);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=n; j<=2*n-1; j++)
                            {
                                atmp[i,j] = 0;
                            }
                        }
                        densesolver.rmatrixsolvels(ref atmp, n, 2*n, ref bv, 0.0, ref info, ref repls, ref xv);
                        if( info<=0 )
                        {
                            rerrors = true;
                        }
                        else
                        {
                            rerrors = rerrors | (double)(repls.r2)!=(double)(0);
                            rerrors = rerrors | repls.n!=2*n | repls.k!=n;
                            for(i=0; i<=n-1; i++)
                            {
                                rerrors = rerrors | (double)(Math.Abs(xe[i,0]-xv[i]))>(double)(threshold);
                            }
                            for(i=n; i<=2*n-1; i++)
                            {
                                rerrors = rerrors | (double)(Math.Abs(xv[i]))>(double)(threshold);
                            }
                        }
                        
                        //
                        // ********************************************************
                        // EXACTLY SINGULAR MATRICES
                        // ability to detect singularity is tested
                        // ********************************************************
                        //
                        // 1. generate different types of singular matrices:
                        //    * zero
                        //    * with zero columns
                        //    * with zero rows
                        //    * with equal rows/columns
                        // 2. generate random solution vector xe
                        // 3. generate right part b=A*xe
                        // 4. test different methods
                        //
                        for(taskkind=0; taskkind<=4; taskkind++)
                        {
                            unset2d(ref a);
                            if( taskkind==0 )
                            {
                                
                                //
                                // all zeros
                                //
                                a = new double[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        a[i,j] = 0;
                                    }
                                }
                            }
                            if( taskkind==1 )
                            {
                                
                                //
                                // there is zero column
                                //
                                a = new double[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        a[i,j] = 2*AP.Math.RandomReal()-1;
                                    }
                                }
                                k = AP.Math.RandomInteger(n);
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[i_,k] = 0*a[i_,k];
                                }
                            }
                            if( taskkind==2 )
                            {
                                
                                //
                                // there is zero row
                                //
                                a = new double[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        a[i,j] = 2*AP.Math.RandomReal()-1;
                                    }
                                }
                                k = AP.Math.RandomInteger(n);
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[k,i_] = 0*a[k,i_];
                                }
                            }
                            if( taskkind==3 )
                            {
                                
                                //
                                // equal columns
                                //
                                if( n<2 )
                                {
                                    continue;
                                }
                                a = new double[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        a[i,j] = 2*AP.Math.RandomReal()-1;
                                    }
                                }
                                k = 1+AP.Math.RandomInteger(n-1);
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[i_,0] = a[i_,k];
                                }
                            }
                            if( taskkind==4 )
                            {
                                
                                //
                                // equal rows
                                //
                                if( n<2 )
                                {
                                    continue;
                                }
                                a = new double[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        a[i,j] = 2*AP.Math.RandomReal()-1;
                                    }
                                }
                                k = 1+AP.Math.RandomInteger(n-1);
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[0,i_] = a[k,i_];
                                }
                            }
                            xe = new double[n, m];
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=m-1; j++)
                                {
                                    xe[i,j] = 2*AP.Math.RandomReal()-1;
                                }
                            }
                            b = new double[n, m];
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=m-1; j++)
                                {
                                    v = 0.0;
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        v += a[i,i_]*xe[i_,j];
                                    }
                                    b[i,j] = v;
                                }
                            }
                            rmatrixmakeacopy(ref a, n, n, ref lua);
                            trfac.rmatrixlu(ref lua, n, n, ref p);
                            
                            //
                            // Test RMatrixSolveM()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            unset2d(ref x);
                            densesolver.rmatrixsolvem(ref a, n, ref b, m, (double)(AP.Math.RandomReal())>(double)(0.5), ref info, ref rep, ref x);
                            rerrors = rerrors | !rmatrixchecksingularm(n, m, info, ref rep, ref x);
                            
                            //
                            // Test RMatrixSolve()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            unset2d(ref x);
                            bv = new double[n];
                            for(i_=0; i_<=n-1;i_++)
                            {
                                bv[i_] = b[i_,0];
                            }
                            densesolver.rmatrixsolve(ref a, n, ref bv, ref info, ref rep, ref xv);
                            rerrors = rerrors | !rmatrixchecksingular(n, info, ref rep, ref xv);
                            
                            //
                            // Test RMatrixLUSolveM()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            unset2d(ref x);
                            densesolver.rmatrixlusolvem(ref lua, ref p, n, ref b, m, ref info, ref rep, ref x);
                            rerrors = rerrors | !rmatrixchecksingularm(n, m, info, ref rep, ref x);
                            
                            //
                            // Test RMatrixLUSolve()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            unset2d(ref x);
                            bv = new double[n];
                            for(i_=0; i_<=n-1;i_++)
                            {
                                bv[i_] = b[i_,0];
                            }
                            densesolver.rmatrixlusolve(ref lua, ref p, n, ref bv, ref info, ref rep, ref xv);
                            rerrors = rerrors | !rmatrixchecksingular(n, info, ref rep, ref xv);
                            
                            //
                            // Test RMatrixMixedSolveM()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            unset2d(ref x);
                            densesolver.rmatrixmixedsolvem(ref a, ref lua, ref p, n, ref b, m, ref info, ref rep, ref x);
                            rerrors = rerrors | !rmatrixchecksingularm(n, m, info, ref rep, ref x);
                            
                            //
                            // Test RMatrixMixedSolve()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            unset2d(ref x);
                            bv = new double[n];
                            for(i_=0; i_<=n-1;i_++)
                            {
                                bv[i_] = b[i_,0];
                            }
                            densesolver.rmatrixmixedsolve(ref a, ref lua, ref p, n, ref bv, ref info, ref rep, ref xv);
                            rerrors = rerrors | !rmatrixchecksingular(n, info, ref rep, ref xv);
                        }
                    }
                }
            }
            
            //
            // test iterative improvement
            //
            for(pass=1; pass<=passcount; pass++)
            {
                
                //
                // Test iterative improvement matrices
                //
                // A matrix/right part are constructed such that both matrix
                // and solution components are within (-1,+1). Such matrix/right part
                // have nice properties - system can be solved using iterative
                // improvement with |A*x-b| about several ulps of max(1,|b|).
                //
                n = 100;
                a = new double[n, n];
                b = new double[n, 1];
                bv = new double[n];
                tx = new double[n];
                xv = new double[n];
                y = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    xv[i] = 2*AP.Math.RandomReal()-1;
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a[i,j] = 2*AP.Math.RandomReal()-1;
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        y[i_] = a[i,i_];
                    }
                    xblas.xdot(ref y, ref xv, n, ref tx, ref v, ref verr);
                    bv[i] = v;
                }
                for(i_=0; i_<=n-1;i_++)
                {
                    b[i_,0] = bv[i_];
                }
                
                //
                // Test RMatrixSolveM()
                //
                unset2d(ref x);
                densesolver.rmatrixsolvem(ref a, n, ref b, 1, true, ref info, ref rep, ref x);
                if( info<=0 )
                {
                    rfserrors = true;
                }
                else
                {
                    xv = new double[n];
                    for(i_=0; i_<=n-1;i_++)
                    {
                        xv[i_] = x[i_,0];
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(i_=0; i_<=n-1;i_++)
                        {
                            y[i_] = a[i,i_];
                        }
                        xblas.xdot(ref y, ref xv, n, ref tx, ref v, ref verr);
                        rfserrors = rfserrors | (double)(Math.Abs(v-b[i,0]))>(double)(8*AP.Math.MachineEpsilon*Math.Max(1, Math.Abs(b[i,0])));
                    }
                }
                
                //
                // Test RMatrixSolve()
                //
                unset1d(ref xv);
                densesolver.rmatrixsolve(ref a, n, ref bv, ref info, ref rep, ref xv);
                if( info<=0 )
                {
                    rfserrors = true;
                }
                else
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(i_=0; i_<=n-1;i_++)
                        {
                            y[i_] = a[i,i_];
                        }
                        xblas.xdot(ref y, ref xv, n, ref tx, ref v, ref verr);
                        rfserrors = rfserrors | (double)(Math.Abs(v-bv[i]))>(double)(8*AP.Math.MachineEpsilon*Math.Max(1, Math.Abs(bv[i])));
                    }
                }
                
                //
                // Test LS-solver on the same matrix
                //
                densesolver.rmatrixsolvels(ref a, n, n, ref bv, 0.0, ref info, ref repls, ref xv);
                if( info<=0 )
                {
                    rfserrors = true;
                }
                else
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(i_=0; i_<=n-1;i_++)
                        {
                            y[i_] = a[i,i_];
                        }
                        xblas.xdot(ref y, ref xv, n, ref tx, ref v, ref verr);
                        rfserrors = rfserrors | (double)(Math.Abs(v-bv[i]))>(double)(8*AP.Math.MachineEpsilon*Math.Max(1, Math.Abs(bv[i])));
                    }
                }
            }
        }


        /*************************************************************************
        SPD test
        *************************************************************************/
        private static void testspdsolver(int maxn,
            int maxm,
            int passcount,
            double threshold,
            ref bool spderrors,
            ref bool rfserrors)
        {
            double[,] a = new double[0,0];
            double[,] cha = new double[0,0];
            double[,] atmp = new double[0,0];
            int[] p = new int[0];
            double[,] xe = new double[0,0];
            double[,] b = new double[0,0];
            double[] bv = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            int n = 0;
            int m = 0;
            int pass = 0;
            int taskkind = 0;
            double mx = 0;
            double v = 0;
            double verr = 0;
            bool isupper = new bool();
            int info = 0;
            densesolver.densesolverreport rep = new densesolver.densesolverreport();
            densesolver.densesolverlsreport repls = new densesolver.densesolverlsreport();
            double[,] x = new double[0,0];
            double[] xv = new double[0];
            double[] y = new double[0];
            double[] tx = new double[0];
            int i_ = 0;

            
            //
            // General square matrices:
            // * test general solvers
            // * test least squares solver
            //
            for(pass=1; pass<=passcount; pass++)
            {
                for(n=1; n<=maxn; n++)
                {
                    for(m=1; m<=maxm; m++)
                    {
                        
                        //
                        // ********************************************************
                        // WELL CONDITIONED TASKS
                        // ability to find correct solution is tested
                        // ********************************************************
                        //
                        // 1. generate random well conditioned matrix A.
                        // 2. generate random solution vector xe
                        // 3. generate right part b=A*xe
                        // 4. test different methods on original A
                        //
                        isupper = (double)(AP.Math.RandomReal())>(double)(0.5);
                        matgen.spdmatrixrndcond(n, 1000, ref a);
                        rmatrixmakeacopy(ref a, n, n, ref cha);
                        if( !trfac.spdmatrixcholesky(ref cha, n, isupper) )
                        {
                            spderrors = true;
                            return;
                        }
                        xe = new double[n, m];
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                xe[i,j] = 2*AP.Math.RandomReal()-1;
                            }
                        }
                        b = new double[n, m];
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                v = 0.0;
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    v += a[i,i_]*xe[i_,j];
                                }
                                b[i,j] = v;
                            }
                        }
                        rmatrixdrophalf(ref a, n, isupper);
                        rmatrixdrophalf(ref cha, n, isupper);
                        
                        //
                        // Test solvers
                        //
                        info = 0;
                        unsetrep(ref rep);
                        unset2d(ref x);
                        densesolver.spdmatrixsolvem(ref a, n, isupper, ref b, m, ref info, ref rep, ref x);
                        spderrors = spderrors | !rmatrixchecksolutionm(ref xe, n, m, threshold, info, ref rep, ref x);
                        info = 0;
                        unsetrep(ref rep);
                        unset1d(ref xv);
                        bv = new double[n];
                        for(i_=0; i_<=n-1;i_++)
                        {
                            bv[i_] = b[i_,0];
                        }
                        densesolver.spdmatrixsolve(ref a, n, isupper, ref bv, ref info, ref rep, ref xv);
                        spderrors = spderrors | !rmatrixchecksolution(ref xe, n, threshold, info, ref rep, ref xv);
                        info = 0;
                        unsetrep(ref rep);
                        unset2d(ref x);
                        densesolver.spdmatrixcholeskysolvem(ref cha, n, isupper, ref b, m, ref info, ref rep, ref x);
                        spderrors = spderrors | !rmatrixchecksolutionm(ref xe, n, m, threshold, info, ref rep, ref x);
                        info = 0;
                        unsetrep(ref rep);
                        unset1d(ref xv);
                        bv = new double[n];
                        for(i_=0; i_<=n-1;i_++)
                        {
                            bv[i_] = b[i_,0];
                        }
                        densesolver.spdmatrixcholeskysolve(ref cha, n, isupper, ref bv, ref info, ref rep, ref xv);
                        spderrors = spderrors | !rmatrixchecksolution(ref xe, n, threshold, info, ref rep, ref xv);
                        
                        //
                        // ********************************************************
                        // EXACTLY SINGULAR MATRICES
                        // ability to detect singularity is tested
                        // ********************************************************
                        //
                        // 1. generate different types of singular matrices:
                        //    * zero
                        //    * with zero columns
                        //    * with zero rows
                        //    * with equal rows/columns
                        // 2. generate random solution vector xe
                        // 3. generate right part b=A*xe
                        // 4. test different methods
                        //
                        for(taskkind=0; taskkind<=3; taskkind++)
                        {
                            unset2d(ref a);
                            if( taskkind==0 )
                            {
                                
                                //
                                // all zeros
                                //
                                a = new double[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        a[i,j] = 0;
                                    }
                                }
                            }
                            if( taskkind==1 )
                            {
                                
                                //
                                // there is zero column
                                //
                                a = new double[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=i; j<=n-1; j++)
                                    {
                                        a[i,j] = 2*AP.Math.RandomReal()-1;
                                        a[j,i] = a[i,j];
                                    }
                                }
                                k = AP.Math.RandomInteger(n);
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[i_,k] = 0*a[i_,k];
                                }
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[k,i_] = 0*a[k,i_];
                                }
                            }
                            if( taskkind==2 )
                            {
                                
                                //
                                // there is zero row
                                //
                                a = new double[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=i; j<=n-1; j++)
                                    {
                                        a[i,j] = 2*AP.Math.RandomReal()-1;
                                        a[j,i] = a[i,j];
                                    }
                                }
                                k = AP.Math.RandomInteger(n);
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[k,i_] = 0*a[k,i_];
                                }
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[i_,k] = 0*a[i_,k];
                                }
                            }
                            if( taskkind==3 )
                            {
                                
                                //
                                // equal columns/rows
                                //
                                if( n<2 )
                                {
                                    continue;
                                }
                                a = new double[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=i; j<=n-1; j++)
                                    {
                                        a[i,j] = 2*AP.Math.RandomReal()-1;
                                        a[j,i] = a[i,j];
                                    }
                                }
                                k = 1+AP.Math.RandomInteger(n-1);
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[i_,0] = a[i_,k];
                                }
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[0,i_] = a[k,i_];
                                }
                            }
                            xe = new double[n, m];
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=m-1; j++)
                                {
                                    xe[i,j] = 2*AP.Math.RandomReal()-1;
                                }
                            }
                            b = new double[n, m];
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=m-1; j++)
                                {
                                    v = 0.0;
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        v += a[i,i_]*xe[i_,j];
                                    }
                                    b[i,j] = v;
                                }
                            }
                            rmatrixmakeacopy(ref a, n, n, ref cha);
                            rmatrixdrophalf(ref a, n, isupper);
                            rmatrixdrophalf(ref cha, n, isupper);
                            
                            //
                            // Test SPDMatrixSolveM()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            unset2d(ref x);
                            densesolver.spdmatrixsolvem(ref a, n, isupper, ref b, m, ref info, ref rep, ref x);
                            spderrors = spderrors | !rmatrixchecksingularm(n, m, info, ref rep, ref x);
                            
                            //
                            // Test SPDMatrixSolve()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            unset2d(ref x);
                            bv = new double[n];
                            for(i_=0; i_<=n-1;i_++)
                            {
                                bv[i_] = b[i_,0];
                            }
                            densesolver.spdmatrixsolve(ref a, n, isupper, ref bv, ref info, ref rep, ref xv);
                            spderrors = spderrors | !rmatrixchecksingular(n, info, ref rep, ref xv);
                            
                            //
                            // 'equal columns/rows' are degenerate, but
                            // Cholesky matrix with equal columns/rows IS NOT degenerate,
                            // so it is not used for testing purposes.
                            //
                            if( taskkind!=3 )
                            {
                                
                                //
                                // Test SPDMatrixLUSolveM()
                                //
                                info = 0;
                                unsetrep(ref rep);
                                unset2d(ref x);
                                densesolver.spdmatrixcholeskysolvem(ref cha, n, isupper, ref b, m, ref info, ref rep, ref x);
                                spderrors = spderrors | !rmatrixchecksingularm(n, m, info, ref rep, ref x);
                                
                                //
                                // Test SPDMatrixLUSolve()
                                //
                                info = 0;
                                unsetrep(ref rep);
                                unset2d(ref x);
                                bv = new double[n];
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    bv[i_] = b[i_,0];
                                }
                                densesolver.spdmatrixcholeskysolve(ref cha, n, isupper, ref bv, ref info, ref rep, ref xv);
                                spderrors = spderrors | !rmatrixchecksingular(n, info, ref rep, ref xv);
                            }
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Real test
        *************************************************************************/
        private static void testcsolver(int maxn,
            int maxm,
            int passcount,
            double threshold,
            ref bool cerrors,
            ref bool rfserrors)
        {
            AP.Complex[,] a = new AP.Complex[0,0];
            AP.Complex[,] lua = new AP.Complex[0,0];
            AP.Complex[,] atmp = new AP.Complex[0,0];
            int[] p = new int[0];
            AP.Complex[,] xe = new AP.Complex[0,0];
            AP.Complex[,] b = new AP.Complex[0,0];
            AP.Complex[] bv = new AP.Complex[0];
            int i = 0;
            int j = 0;
            int k = 0;
            int n = 0;
            int m = 0;
            int pass = 0;
            int taskkind = 0;
            double mx = 0;
            double verr = 0;
            AP.Complex v = 0;
            int info = 0;
            densesolver.densesolverreport rep = new densesolver.densesolverreport();
            densesolver.densesolverlsreport repls = new densesolver.densesolverlsreport();
            AP.Complex[,] x = new AP.Complex[0,0];
            AP.Complex[] xv = new AP.Complex[0];
            AP.Complex[] y = new AP.Complex[0];
            double[] tx = new double[0];
            int i_ = 0;

            
            //
            // General square matrices:
            // * test general solvers
            // * test least squares solver
            //
            for(pass=1; pass<=passcount; pass++)
            {
                for(n=1; n<=maxn; n++)
                {
                    for(m=1; m<=maxm; m++)
                    {
                        
                        //
                        // ********************************************************
                        // WELL CONDITIONED TASKS
                        // ability to find correct solution is tested
                        // ********************************************************
                        //
                        // 1. generate random well conditioned matrix A.
                        // 2. generate random solution vector xe
                        // 3. generate right part b=A*xe
                        // 4. test different methods on original A
                        //
                        matgen.cmatrixrndcond(n, 1000, ref a);
                        cmatrixmakeacopy(ref a, n, n, ref lua);
                        trfac.cmatrixlu(ref lua, n, n, ref p);
                        xe = new AP.Complex[n, m];
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                xe[i,j].x = 2*AP.Math.RandomReal()-1;
                                xe[i,j].y = 2*AP.Math.RandomReal()-1;
                            }
                        }
                        b = new AP.Complex[n, m];
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                v = 0.0;
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    v += a[i,i_]*xe[i_,j];
                                }
                                b[i,j] = v;
                            }
                        }
                        
                        //
                        // Test solvers
                        //
                        info = 0;
                        unsetrep(ref rep);
                        cunset2d(ref x);
                        densesolver.cmatrixsolvem(ref a, n, ref b, m, (double)(AP.Math.RandomReal())>(double)(0.5), ref info, ref rep, ref x);
                        cerrors = cerrors | !cmatrixchecksolutionm(ref xe, n, m, threshold, info, ref rep, ref x);
                        info = 0;
                        unsetrep(ref rep);
                        cunset1d(ref xv);
                        bv = new AP.Complex[n];
                        for(i_=0; i_<=n-1;i_++)
                        {
                            bv[i_] = b[i_,0];
                        }
                        densesolver.cmatrixsolve(ref a, n, ref bv, ref info, ref rep, ref xv);
                        cerrors = cerrors | !cmatrixchecksolution(ref xe, n, threshold, info, ref rep, ref xv);
                        info = 0;
                        unsetrep(ref rep);
                        cunset2d(ref x);
                        densesolver.cmatrixlusolvem(ref lua, ref p, n, ref b, m, ref info, ref rep, ref x);
                        cerrors = cerrors | !cmatrixchecksolutionm(ref xe, n, m, threshold, info, ref rep, ref x);
                        info = 0;
                        unsetrep(ref rep);
                        cunset1d(ref xv);
                        bv = new AP.Complex[n];
                        for(i_=0; i_<=n-1;i_++)
                        {
                            bv[i_] = b[i_,0];
                        }
                        densesolver.cmatrixlusolve(ref lua, ref p, n, ref bv, ref info, ref rep, ref xv);
                        cerrors = cerrors | !cmatrixchecksolution(ref xe, n, threshold, info, ref rep, ref xv);
                        info = 0;
                        unsetrep(ref rep);
                        cunset2d(ref x);
                        densesolver.cmatrixmixedsolvem(ref a, ref lua, ref p, n, ref b, m, ref info, ref rep, ref x);
                        cerrors = cerrors | !cmatrixchecksolutionm(ref xe, n, m, threshold, info, ref rep, ref x);
                        info = 0;
                        unsetrep(ref rep);
                        cunset1d(ref xv);
                        bv = new AP.Complex[n];
                        for(i_=0; i_<=n-1;i_++)
                        {
                            bv[i_] = b[i_,0];
                        }
                        densesolver.cmatrixmixedsolve(ref a, ref lua, ref p, n, ref bv, ref info, ref rep, ref xv);
                        cerrors = cerrors | !cmatrixchecksolution(ref xe, n, threshold, info, ref rep, ref xv);
                        
                        //
                        // ********************************************************
                        // EXACTLY SINGULAR MATRICES
                        // ability to detect singularity is tested
                        // ********************************************************
                        //
                        // 1. generate different types of singular matrices:
                        //    * zero
                        //    * with zero columns
                        //    * with zero rows
                        //    * with equal rows/columns
                        // 2. generate random solution vector xe
                        // 3. generate right part b=A*xe
                        // 4. test different methods
                        //
                        for(taskkind=0; taskkind<=4; taskkind++)
                        {
                            cunset2d(ref a);
                            if( taskkind==0 )
                            {
                                
                                //
                                // all zeros
                                //
                                a = new AP.Complex[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        a[i,j] = 0;
                                    }
                                }
                            }
                            if( taskkind==1 )
                            {
                                
                                //
                                // there is zero column
                                //
                                a = new AP.Complex[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        a[i,j].x = 2*AP.Math.RandomReal()-1;
                                        a[i,j].y = 2*AP.Math.RandomReal()-1;
                                    }
                                }
                                k = AP.Math.RandomInteger(n);
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[i_,k] = 0*a[i_,k];
                                }
                            }
                            if( taskkind==2 )
                            {
                                
                                //
                                // there is zero row
                                //
                                a = new AP.Complex[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        a[i,j].x = 2*AP.Math.RandomReal()-1;
                                        a[i,j].y = 2*AP.Math.RandomReal()-1;
                                    }
                                }
                                k = AP.Math.RandomInteger(n);
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[k,i_] = 0*a[k,i_];
                                }
                            }
                            if( taskkind==3 )
                            {
                                
                                //
                                // equal columns
                                //
                                if( n<2 )
                                {
                                    continue;
                                }
                                a = new AP.Complex[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        a[i,j].x = 2*AP.Math.RandomReal()-1;
                                        a[i,j].y = 2*AP.Math.RandomReal()-1;
                                    }
                                }
                                k = 1+AP.Math.RandomInteger(n-1);
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[i_,0] = a[i_,k];
                                }
                            }
                            if( taskkind==4 )
                            {
                                
                                //
                                // equal rows
                                //
                                if( n<2 )
                                {
                                    continue;
                                }
                                a = new AP.Complex[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        a[i,j].x = 2*AP.Math.RandomReal()-1;
                                        a[i,j].y = 2*AP.Math.RandomReal()-1;
                                    }
                                }
                                k = 1+AP.Math.RandomInteger(n-1);
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[0,i_] = a[k,i_];
                                }
                            }
                            xe = new AP.Complex[n, m];
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=m-1; j++)
                                {
                                    xe[i,j] = 2*AP.Math.RandomReal()-1;
                                }
                            }
                            b = new AP.Complex[n, m];
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=m-1; j++)
                                {
                                    v = 0.0;
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        v += a[i,i_]*xe[i_,j];
                                    }
                                    b[i,j] = v;
                                }
                            }
                            cmatrixmakeacopy(ref a, n, n, ref lua);
                            trfac.cmatrixlu(ref lua, n, n, ref p);
                            
                            //
                            // Test CMatrixSolveM()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            cunset2d(ref x);
                            densesolver.cmatrixsolvem(ref a, n, ref b, m, (double)(AP.Math.RandomReal())>(double)(0.5), ref info, ref rep, ref x);
                            cerrors = cerrors | !cmatrixchecksingularm(n, m, info, ref rep, ref x);
                            
                            //
                            // Test CMatrixSolve()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            cunset2d(ref x);
                            bv = new AP.Complex[n];
                            for(i_=0; i_<=n-1;i_++)
                            {
                                bv[i_] = b[i_,0];
                            }
                            densesolver.cmatrixsolve(ref a, n, ref bv, ref info, ref rep, ref xv);
                            cerrors = cerrors | !cmatrixchecksingular(n, info, ref rep, ref xv);
                            
                            //
                            // Test CMatrixLUSolveM()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            cunset2d(ref x);
                            densesolver.cmatrixlusolvem(ref lua, ref p, n, ref b, m, ref info, ref rep, ref x);
                            cerrors = cerrors | !cmatrixchecksingularm(n, m, info, ref rep, ref x);
                            
                            //
                            // Test CMatrixLUSolve()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            cunset2d(ref x);
                            bv = new AP.Complex[n];
                            for(i_=0; i_<=n-1;i_++)
                            {
                                bv[i_] = b[i_,0];
                            }
                            densesolver.cmatrixlusolve(ref lua, ref p, n, ref bv, ref info, ref rep, ref xv);
                            cerrors = cerrors | !cmatrixchecksingular(n, info, ref rep, ref xv);
                            
                            //
                            // Test CMatrixMixedSolveM()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            cunset2d(ref x);
                            densesolver.cmatrixmixedsolvem(ref a, ref lua, ref p, n, ref b, m, ref info, ref rep, ref x);
                            cerrors = cerrors | !cmatrixchecksingularm(n, m, info, ref rep, ref x);
                            
                            //
                            // Test CMatrixMixedSolve()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            cunset2d(ref x);
                            bv = new AP.Complex[n];
                            for(i_=0; i_<=n-1;i_++)
                            {
                                bv[i_] = b[i_,0];
                            }
                            densesolver.cmatrixmixedsolve(ref a, ref lua, ref p, n, ref bv, ref info, ref rep, ref xv);
                            cerrors = cerrors | !cmatrixchecksingular(n, info, ref rep, ref xv);
                        }
                    }
                }
            }
            
            //
            // test iterative improvement
            //
            for(pass=1; pass<=passcount; pass++)
            {
                
                //
                // Test iterative improvement matrices
                //
                // A matrix/right part are constructed such that both matrix
                // and solution components magnitudes are within (-1,+1).
                // Such matrix/right part have nice properties - system can
                // be solved using iterative improvement with |A*x-b| about
                // several ulps of max(1,|b|).
                //
                n = 100;
                a = new AP.Complex[n, n];
                b = new AP.Complex[n, 1];
                bv = new AP.Complex[n];
                tx = new double[2*n];
                xv = new AP.Complex[n];
                y = new AP.Complex[n];
                for(i=0; i<=n-1; i++)
                {
                    xv[i].x = 2*AP.Math.RandomReal()-1;
                    xv[i].y = 2*AP.Math.RandomReal()-1;
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a[i,j].x = 2*AP.Math.RandomReal()-1;
                        a[i,j].y = 2*AP.Math.RandomReal()-1;
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        y[i_] = a[i,i_];
                    }
                    xblas.xcdot(ref y, ref xv, n, ref tx, ref v, ref verr);
                    bv[i] = v;
                }
                for(i_=0; i_<=n-1;i_++)
                {
                    b[i_,0] = bv[i_];
                }
                
                //
                // Test CMatrixSolveM()
                //
                cunset2d(ref x);
                densesolver.cmatrixsolvem(ref a, n, ref b, 1, true, ref info, ref rep, ref x);
                if( info<=0 )
                {
                    rfserrors = true;
                }
                else
                {
                    xv = new AP.Complex[n];
                    for(i_=0; i_<=n-1;i_++)
                    {
                        xv[i_] = x[i_,0];
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(i_=0; i_<=n-1;i_++)
                        {
                            y[i_] = a[i,i_];
                        }
                        xblas.xcdot(ref y, ref xv, n, ref tx, ref v, ref verr);
                        rfserrors = rfserrors | (double)(AP.Math.AbsComplex(v-b[i,0]))>(double)(8*AP.Math.MachineEpsilon*Math.Max(1, AP.Math.AbsComplex(b[i,0])));
                    }
                }
                
                //
                // Test CMatrixSolve()
                //
                cunset1d(ref xv);
                densesolver.cmatrixsolve(ref a, n, ref bv, ref info, ref rep, ref xv);
                if( info<=0 )
                {
                    rfserrors = true;
                }
                else
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(i_=0; i_<=n-1;i_++)
                        {
                            y[i_] = a[i,i_];
                        }
                        xblas.xcdot(ref y, ref xv, n, ref tx, ref v, ref verr);
                        rfserrors = rfserrors | (double)(AP.Math.AbsComplex(v-bv[i]))>(double)(8*AP.Math.MachineEpsilon*Math.Max(1, AP.Math.AbsComplex(bv[i])));
                    }
                }
                
                //
                // TODO: Test LS-solver on the same matrix
                //
            }
        }


        /*************************************************************************
        HPD test
        *************************************************************************/
        private static void testhpdsolver(int maxn,
            int maxm,
            int passcount,
            double threshold,
            ref bool hpderrors,
            ref bool rfserrors)
        {
            AP.Complex[,] a = new AP.Complex[0,0];
            AP.Complex[,] cha = new AP.Complex[0,0];
            AP.Complex[,] atmp = new AP.Complex[0,0];
            int[] p = new int[0];
            AP.Complex[,] xe = new AP.Complex[0,0];
            AP.Complex[,] b = new AP.Complex[0,0];
            AP.Complex[] bv = new AP.Complex[0];
            int i = 0;
            int j = 0;
            int k = 0;
            int n = 0;
            int m = 0;
            int pass = 0;
            int taskkind = 0;
            double mx = 0;
            AP.Complex v = 0;
            bool isupper = new bool();
            int info = 0;
            densesolver.densesolverreport rep = new densesolver.densesolverreport();
            densesolver.densesolverlsreport repls = new densesolver.densesolverlsreport();
            AP.Complex[,] x = new AP.Complex[0,0];
            AP.Complex[] xv = new AP.Complex[0];
            AP.Complex[] y = new AP.Complex[0];
            AP.Complex[] tx = new AP.Complex[0];
            int i_ = 0;

            
            //
            // General square matrices:
            // * test general solvers
            // * test least squares solver
            //
            for(pass=1; pass<=passcount; pass++)
            {
                for(n=1; n<=maxn; n++)
                {
                    for(m=1; m<=maxm; m++)
                    {
                        
                        //
                        // ********************************************************
                        // WELL CONDITIONED TASKS
                        // ability to find correct solution is tested
                        // ********************************************************
                        //
                        // 1. generate random well conditioned matrix A.
                        // 2. generate random solution vector xe
                        // 3. generate right part b=A*xe
                        // 4. test different methods on original A
                        //
                        isupper = (double)(AP.Math.RandomReal())>(double)(0.5);
                        matgen.hpdmatrixrndcond(n, 1000, ref a);
                        cmatrixmakeacopy(ref a, n, n, ref cha);
                        if( !trfac.hpdmatrixcholesky(ref cha, n, isupper) )
                        {
                            hpderrors = true;
                            return;
                        }
                        xe = new AP.Complex[n, m];
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                xe[i,j].x = 2*AP.Math.RandomReal()-1;
                                xe[i,j].y = 2*AP.Math.RandomReal()-1;
                            }
                        }
                        b = new AP.Complex[n, m];
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                v = 0.0;
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    v += a[i,i_]*xe[i_,j];
                                }
                                b[i,j] = v;
                            }
                        }
                        cmatrixdrophalf(ref a, n, isupper);
                        cmatrixdrophalf(ref cha, n, isupper);
                        
                        //
                        // Test solvers
                        //
                        info = 0;
                        unsetrep(ref rep);
                        cunset2d(ref x);
                        densesolver.hpdmatrixsolvem(ref a, n, isupper, ref b, m, ref info, ref rep, ref x);
                        hpderrors = hpderrors | !cmatrixchecksolutionm(ref xe, n, m, threshold, info, ref rep, ref x);
                        info = 0;
                        unsetrep(ref rep);
                        cunset1d(ref xv);
                        bv = new AP.Complex[n];
                        for(i_=0; i_<=n-1;i_++)
                        {
                            bv[i_] = b[i_,0];
                        }
                        densesolver.hpdmatrixsolve(ref a, n, isupper, ref bv, ref info, ref rep, ref xv);
                        hpderrors = hpderrors | !cmatrixchecksolution(ref xe, n, threshold, info, ref rep, ref xv);
                        info = 0;
                        unsetrep(ref rep);
                        cunset2d(ref x);
                        densesolver.hpdmatrixcholeskysolvem(ref cha, n, isupper, ref b, m, ref info, ref rep, ref x);
                        hpderrors = hpderrors | !cmatrixchecksolutionm(ref xe, n, m, threshold, info, ref rep, ref x);
                        info = 0;
                        unsetrep(ref rep);
                        cunset1d(ref xv);
                        bv = new AP.Complex[n];
                        for(i_=0; i_<=n-1;i_++)
                        {
                            bv[i_] = b[i_,0];
                        }
                        densesolver.hpdmatrixcholeskysolve(ref cha, n, isupper, ref bv, ref info, ref rep, ref xv);
                        hpderrors = hpderrors | !cmatrixchecksolution(ref xe, n, threshold, info, ref rep, ref xv);
                        
                        //
                        // ********************************************************
                        // EXACTLY SINGULAR MATRICES
                        // ability to detect singularity is tested
                        // ********************************************************
                        //
                        // 1. generate different types of singular matrices:
                        //    * zero
                        //    * with zero columns
                        //    * with zero rows
                        //    * with equal rows/columns
                        // 2. generate random solution vector xe
                        // 3. generate right part b=A*xe
                        // 4. test different methods
                        //
                        for(taskkind=0; taskkind<=3; taskkind++)
                        {
                            cunset2d(ref a);
                            if( taskkind==0 )
                            {
                                
                                //
                                // all zeros
                                //
                                a = new AP.Complex[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        a[i,j] = 0;
                                    }
                                }
                            }
                            if( taskkind==1 )
                            {
                                
                                //
                                // there is zero column
                                //
                                a = new AP.Complex[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=i; j<=n-1; j++)
                                    {
                                        a[i,j].x = 2*AP.Math.RandomReal()-1;
                                        a[i,j].y = 2*AP.Math.RandomReal()-1;
                                        if( i==j )
                                        {
                                            a[i,j].y = 0;
                                        }
                                        a[j,i] = a[i,j];
                                    }
                                }
                                k = AP.Math.RandomInteger(n);
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[i_,k] = 0*a[i_,k];
                                }
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[k,i_] = 0*a[k,i_];
                                }
                            }
                            if( taskkind==2 )
                            {
                                
                                //
                                // there is zero row
                                //
                                a = new AP.Complex[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=i; j<=n-1; j++)
                                    {
                                        a[i,j].x = 2*AP.Math.RandomReal()-1;
                                        a[i,j].y = 2*AP.Math.RandomReal()-1;
                                        if( i==j )
                                        {
                                            a[i,j].y = 0;
                                        }
                                        a[j,i] = a[i,j];
                                    }
                                }
                                k = AP.Math.RandomInteger(n);
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[k,i_] = 0*a[k,i_];
                                }
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[i_,k] = 0*a[i_,k];
                                }
                            }
                            if( taskkind==3 )
                            {
                                
                                //
                                // equal columns/rows
                                //
                                if( n<2 )
                                {
                                    continue;
                                }
                                a = new AP.Complex[n, n];
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=i; j<=n-1; j++)
                                    {
                                        a[i,j].x = 2*AP.Math.RandomReal()-1;
                                        a[i,j].y = 2*AP.Math.RandomReal()-1;
                                        if( i==j )
                                        {
                                            a[i,j].y = 0;
                                        }
                                        a[j,i] = a[i,j];
                                    }
                                }
                                k = 1+AP.Math.RandomInteger(n-1);
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[i_,0] = a[i_,k];
                                }
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    a[0,i_] = a[k,i_];
                                }
                            }
                            xe = new AP.Complex[n, m];
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=m-1; j++)
                                {
                                    xe[i,j] = 2*AP.Math.RandomReal()-1;
                                }
                            }
                            b = new AP.Complex[n, m];
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=m-1; j++)
                                {
                                    v = 0.0;
                                    for(i_=0; i_<=n-1;i_++)
                                    {
                                        v += a[i,i_]*xe[i_,j];
                                    }
                                    b[i,j] = v;
                                }
                            }
                            cmatrixmakeacopy(ref a, n, n, ref cha);
                            cmatrixdrophalf(ref a, n, isupper);
                            cmatrixdrophalf(ref cha, n, isupper);
                            
                            //
                            // Test SPDMatrixSolveM()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            cunset2d(ref x);
                            densesolver.hpdmatrixsolvem(ref a, n, isupper, ref b, m, ref info, ref rep, ref x);
                            hpderrors = hpderrors | !cmatrixchecksingularm(n, m, info, ref rep, ref x);
                            
                            //
                            // Test SPDMatrixSolve()
                            //
                            info = 0;
                            unsetrep(ref rep);
                            cunset2d(ref x);
                            bv = new AP.Complex[n];
                            for(i_=0; i_<=n-1;i_++)
                            {
                                bv[i_] = b[i_,0];
                            }
                            densesolver.hpdmatrixsolve(ref a, n, isupper, ref bv, ref info, ref rep, ref xv);
                            hpderrors = hpderrors | !cmatrixchecksingular(n, info, ref rep, ref xv);
                            
                            //
                            // 'equal columns/rows' are degenerate, but
                            // Cholesky matrix with equal columns/rows IS NOT degenerate,
                            // so it is not used for testing purposes.
                            //
                            if( taskkind!=3 )
                            {
                                
                                //
                                // Test SPDMatrixLUSolveM()
                                //
                                info = 0;
                                unsetrep(ref rep);
                                cunset2d(ref x);
                                densesolver.hpdmatrixcholeskysolvem(ref cha, n, isupper, ref b, m, ref info, ref rep, ref x);
                                hpderrors = hpderrors | !cmatrixchecksingularm(n, m, info, ref rep, ref x);
                                
                                //
                                // Test SPDMatrixLUSolve()
                                //
                                info = 0;
                                unsetrep(ref rep);
                                cunset2d(ref x);
                                bv = new AP.Complex[n];
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    bv[i_] = b[i_,0];
                                }
                                densesolver.hpdmatrixcholeskysolve(ref cha, n, isupper, ref bv, ref info, ref rep, ref xv);
                                hpderrors = hpderrors | !cmatrixchecksingular(n, info, ref rep, ref xv);
                            }
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Unsets real matrix
        *************************************************************************/
        private static void unset2d(ref double[,] x)
        {
            x = new double[1, 1];
            x[0,0] = 2*AP.Math.RandomReal()-1;
        }


        /*************************************************************************
        Unsets real vector
        *************************************************************************/
        private static void unset1d(ref double[] x)
        {
            x = new double[1];
            x[0] = 2*AP.Math.RandomReal()-1;
        }


        /*************************************************************************
        Unsets real matrix
        *************************************************************************/
        private static void cunset2d(ref AP.Complex[,] x)
        {
            x = new AP.Complex[1, 1];
            x[0,0] = 2*AP.Math.RandomReal()-1;
        }


        /*************************************************************************
        Unsets real vector
        *************************************************************************/
        private static void cunset1d(ref AP.Complex[] x)
        {
            x = new AP.Complex[1];
            x[0] = 2*AP.Math.RandomReal()-1;
        }


        /*************************************************************************
        Unsets report
        *************************************************************************/
        private static void unsetrep(ref densesolver.densesolverreport r)
        {
            r.r1 = -1;
            r.rinf = -1;
        }


        /*************************************************************************
        Unsets report
        *************************************************************************/
        private static void unsetlsrep(ref densesolver.densesolverlsreport r)
        {
            r.r2 = -1;
            r.n = -1;
            r.k = -1;
            unset2d(ref r.cx);
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testdensesolverunit_test_silent()
        {
            bool result = new bool();

            result = testdensesolver(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testdensesolverunit_test()
        {
            bool result = new bool();

            result = testdensesolver(false);
            return result;
        }
    }
}
