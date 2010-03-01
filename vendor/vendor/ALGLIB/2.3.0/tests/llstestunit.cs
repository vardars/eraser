
using System;

namespace alglib
{
    public class llstestunit
    {
        public static bool testlls(bool silent)
        {
            bool result = new bool();
            bool waserrors = new bool();
            bool llserrors = new bool();
            bool nlserrors = new bool();
            double threshold = 0;
            double nlthreshold = 0;
            int maxn = 0;
            int maxm = 0;
            int passcount = 0;
            int n = 0;
            int m = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int pass = 0;
            double xscale = 0;
            double[] x = new double[0];
            double[] y = new double[0];
            double[] w = new double[0];
            double[] w2 = new double[0];
            double[] c = new double[0];
            double[] c2 = new double[0];
            double[,] a = new double[0,0];
            double[,] a2 = new double[0,0];
            double[,] cm = new double[0,0];
            double v = 0;
            double v1 = 0;
            double v2 = 0;
            lsfit.lsfitreport rep = new lsfit.lsfitreport();
            lsfit.lsfitreport rep2 = new lsfit.lsfitreport();
            int info = 0;
            int info2 = 0;
            double refrms = 0;
            double refavg = 0;
            double refavgrel = 0;
            double refmax = 0;
            lsfit.lsfitstate state = new lsfit.lsfitstate();

            waserrors = false;
            llserrors = false;
            nlserrors = false;
            threshold = 10000*AP.Math.MachineEpsilon;
            nlthreshold = 0.00001;
            maxn = 6;
            maxm = 6;
            passcount = 4;
            
            //
            // Testing unconstrained least squares (linear/nonlinear)
            //
            for(n=1; n<=maxn; n++)
            {
                for(m=1; m<=maxm; m++)
                {
                    for(pass=1; pass<=passcount; pass++)
                    {
                        
                        //
                        // Solve non-degenerate linear least squares task
                        // Use Chebyshev basis. Its condition number is very good.
                        //
                        a = new double[n, m];
                        x = new double[n];
                        y = new double[n];
                        w = new double[n];
                        xscale = 0.9+0.1*AP.Math.RandomReal();
                        for(i=0; i<=n-1; i++)
                        {
                            if( n==1 )
                            {
                                x[i] = 2*AP.Math.RandomReal()-1;
                            }
                            else
                            {
                                x[i] = xscale*((double)(2*i)/((double)(n-1))-1);
                            }
                            y[i] = 3*x[i]+Math.Exp(x[i]);
                            w[i] = 1+AP.Math.RandomReal();
                            a[i,0] = 1;
                            if( m>1 )
                            {
                                a[i,1] = x[i];
                            }
                            for(j=2; j<=m-1; j++)
                            {
                                a[i,j] = 2*x[i]*a[i,j-1]-a[i,j-2];
                            }
                        }
                        
                        //
                        // 1. test weighted fitting (optimality)
                        // 2. Solve degenerate least squares task built on the basis
                        //    of previous task
                        //
                        lsfit.lsfitlinearw(ref y, ref w, ref a, n, m, ref info, ref c, ref rep);
                        if( info<=0 )
                        {
                            llserrors = true;
                        }
                        else
                        {
                            llserrors = llserrors | !isglssolution(n, m, 0, ref y, ref w, ref a, ref cm, c);
                        }
                        a2 = new double[n, 2*m];
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                a2[i,2*j+0] = a[i,j];
                                a2[i,2*j+1] = a[i,j];
                            }
                        }
                        lsfit.lsfitlinearw(ref y, ref w, ref a2, n, 2*m, ref info, ref c2, ref rep);
                        if( info<=0 )
                        {
                            llserrors = true;
                        }
                        else
                        {
                            
                            //
                            // test answer correctness using design matrix properties
                            // and previous task solution
                            //
                            for(j=0; j<=m-1; j++)
                            {
                                llserrors = llserrors | (double)(Math.Abs(c2[2*j+0]+c2[2*j+1]-c[j]))>(double)(threshold);
                            }
                        }
                        
                        //
                        // test non-weighted fitting
                        //
                        w2 = new double[n];
                        for(i=0; i<=n-1; i++)
                        {
                            w2[i] = 1;
                        }
                        lsfit.lsfitlinearw(ref y, ref w2, ref a, n, m, ref info, ref c, ref rep);
                        lsfit.lsfitlinear(ref y, ref a, n, m, ref info2, ref c2, ref rep2);
                        if( info<=0 | info2<=0 )
                        {
                            llserrors = true;
                        }
                        else
                        {
                            
                            //
                            // test answer correctness
                            //
                            for(j=0; j<=m-1; j++)
                            {
                                llserrors = llserrors | (double)(Math.Abs(c[j]-c2[j]))>(double)(threshold);
                            }
                            llserrors = llserrors | (double)(Math.Abs(rep.taskrcond-rep2.taskrcond))>(double)(threshold);
                        }
                        
                        //
                        // test nonlinear fitting on the linear task
                        // (only non-degenerate task are tested)
                        // and compare with answer from linear fitting subroutine
                        //
                        if( n>=m )
                        {
                            c2 = new double[m];
                            
                            //
                            // test gradient-only or Hessian-based weighted fitting
                            //
                            lsfit.lsfitlinearw(ref y, ref w, ref a, n, m, ref info, ref c, ref rep);
                            for(i=0; i<=m-1; i++)
                            {
                                c2[i] = 2*AP.Math.RandomReal()-1;
                            }
                            lsfit.lsfitnonlinearwfg(ref a, ref y, ref w, ref c2, n, m, m, 0.0, nlthreshold, 0, (double)(AP.Math.RandomReal())>(double)(0.5), ref state);
                            fitlinearnonlinear(m, true, ref a, ref state, ref nlserrors);
                            lsfit.lsfitnonlinearresults(ref state, ref info, ref c2, ref rep2);
                            if( info<=0 )
                            {
                                nlserrors = true;
                            }
                            else
                            {
                                for(i=0; i<=m-1; i++)
                                {
                                    nlserrors = nlserrors | (double)(Math.Abs(c[i]-c2[i]))>(double)(100*nlthreshold);
                                }
                            }
                            for(i=0; i<=m-1; i++)
                            {
                                c2[i] = 2*AP.Math.RandomReal()-1;
                            }
                            lsfit.lsfitnonlinearwfgh(ref a, ref y, ref w, ref c2, n, m, m, 0.0, nlthreshold, 0, ref state);
                            fitlinearnonlinear(m, false, ref a, ref state, ref nlserrors);
                            lsfit.lsfitnonlinearresults(ref state, ref info, ref c2, ref rep2);
                            if( info<=0 )
                            {
                                nlserrors = true;
                            }
                            else
                            {
                                for(i=0; i<=m-1; i++)
                                {
                                    nlserrors = nlserrors | (double)(Math.Abs(c[i]-c2[i]))>(double)(100*nlthreshold);
                                }
                            }
                            
                            //
                            // test gradient-only or Hessian-based fitting without weights
                            //
                            lsfit.lsfitlinear(ref y, ref a, n, m, ref info, ref c, ref rep);
                            for(i=0; i<=m-1; i++)
                            {
                                c2[i] = 2*AP.Math.RandomReal()-1;
                            }
                            lsfit.lsfitnonlinearfg(ref a, ref y, ref c2, n, m, m, 0.0, nlthreshold, 0, (double)(AP.Math.RandomReal())>(double)(0.5), ref state);
                            fitlinearnonlinear(m, true, ref a, ref state, ref nlserrors);
                            lsfit.lsfitnonlinearresults(ref state, ref info, ref c2, ref rep2);
                            if( info<=0 )
                            {
                                nlserrors = true;
                            }
                            else
                            {
                                for(i=0; i<=m-1; i++)
                                {
                                    nlserrors = nlserrors | (double)(Math.Abs(c[i]-c2[i]))>(double)(100*nlthreshold);
                                }
                            }
                            for(i=0; i<=m-1; i++)
                            {
                                c2[i] = 2*AP.Math.RandomReal()-1;
                            }
                            lsfit.lsfitnonlinearfgh(ref a, ref y, ref c2, n, m, m, 0.0, nlthreshold, 0, ref state);
                            fitlinearnonlinear(m, false, ref a, ref state, ref nlserrors);
                            lsfit.lsfitnonlinearresults(ref state, ref info, ref c2, ref rep2);
                            if( info<=0 )
                            {
                                nlserrors = true;
                            }
                            else
                            {
                                for(i=0; i<=m-1; i++)
                                {
                                    nlserrors = nlserrors | (double)(Math.Abs(c[i]-c2[i]))>(double)(100*nlthreshold);
                                }
                            }
                        }
                    }
                }
                
                //
                // test correctness of the RCond field
                //
                a = new double[n-1+1, n-1+1];
                x = new double[n-1+1];
                y = new double[n-1+1];
                w = new double[n-1+1];
                v1 = AP.Math.MaxRealNumber;
                v2 = AP.Math.MinRealNumber;
                for(i=0; i<=n-1; i++)
                {
                    x[i] = 0.1+0.9*AP.Math.RandomReal();
                    y[i] = 0.1+0.9*AP.Math.RandomReal();
                    w[i] = 1;
                    for(j=0; j<=n-1; j++)
                    {
                        if( i==j )
                        {
                            a[i,i] = 0.1+0.9*AP.Math.RandomReal();
                            v1 = Math.Min(v1, a[i,i]);
                            v2 = Math.Max(v2, a[i,i]);
                        }
                        else
                        {
                            a[i,j] = 0;
                        }
                    }
                }
                lsfit.lsfitlinearw(ref y, ref w, ref a, n, n, ref info, ref c, ref rep);
                if( info<=0 )
                {
                    llserrors = true;
                }
                else
                {
                    llserrors = llserrors | (double)(Math.Abs(rep.taskrcond-v1/v2))>(double)(threshold);
                }
            }
            
            //
            // Test constrained least squares
            //
            for(pass=1; pass<=passcount; pass++)
            {
                for(n=1; n<=maxn; n++)
                {
                    for(m=1; m<=maxm; m++)
                    {
                        
                        //
                        // test for K<>0
                        //
                        for(k=1; k<=m-1; k++)
                        {
                            
                            //
                            // Prepare Chebyshev basis. Its condition number is very good.
                            // Prepare constraints (random numbers)
                            //
                            a = new double[n, m];
                            x = new double[n];
                            y = new double[n];
                            w = new double[n];
                            xscale = 0.9+0.1*AP.Math.RandomReal();
                            for(i=0; i<=n-1; i++)
                            {
                                if( n==1 )
                                {
                                    x[i] = 2*AP.Math.RandomReal()-1;
                                }
                                else
                                {
                                    x[i] = xscale*((double)(2*i)/((double)(n-1))-1);
                                }
                                y[i] = 3*x[i]+Math.Exp(x[i]);
                                w[i] = 1+AP.Math.RandomReal();
                                a[i,0] = 1;
                                if( m>1 )
                                {
                                    a[i,1] = x[i];
                                }
                                for(j=2; j<=m-1; j++)
                                {
                                    a[i,j] = 2*x[i]*a[i,j-1]-a[i,j-2];
                                }
                            }
                            cm = new double[k, m+1];
                            for(i=0; i<=k-1; i++)
                            {
                                for(j=0; j<=m; j++)
                                {
                                    cm[i,j] = 2*AP.Math.RandomReal()-1;
                                }
                            }
                            
                            //
                            // Solve constrained task
                            //
                            lsfit.lsfitlinearwc(y, ref w, ref a, cm, n, m, k, ref info, ref c, ref rep);
                            if( info<=0 )
                            {
                                llserrors = true;
                            }
                            else
                            {
                                llserrors = llserrors | !isglssolution(n, m, k, ref y, ref w, ref a, ref cm, c);
                            }
                            
                            //
                            // test non-weighted fitting
                            //
                            w2 = new double[n];
                            for(i=0; i<=n-1; i++)
                            {
                                w2[i] = 1;
                            }
                            lsfit.lsfitlinearwc(y, ref w2, ref a, cm, n, m, k, ref info, ref c, ref rep);
                            lsfit.lsfitlinearc(y, ref a, ref cm, n, m, k, ref info2, ref c2, ref rep2);
                            if( info<=0 | info2<=0 )
                            {
                                llserrors = true;
                            }
                            else
                            {
                                
                                //
                                // test answer correctness
                                //
                                for(j=0; j<=m-1; j++)
                                {
                                    llserrors = llserrors | (double)(Math.Abs(c[j]-c2[j]))>(double)(threshold);
                                }
                                llserrors = llserrors | (double)(Math.Abs(rep.taskrcond-rep2.taskrcond))>(double)(threshold);
                            }
                        }
                    }
                }
            }
            
            //
            // nonlinear task for nonlinear fitting:
            //
            //     f(X,C) = 1/(1+C*X^2),
            //     C(true) = 2.
            //
            n = 100;
            c = new double[1];
            c[0] = 1+2*AP.Math.RandomReal();
            a = new double[n, 1];
            y = new double[n];
            for(i=0; i<=n-1; i++)
            {
                a[i,0] = 4*AP.Math.RandomReal()-2;
                y[i] = 1/(1+2*AP.Math.Sqr(a[i,0]));
            }
            lsfit.lsfitnonlinearfg(ref a, ref y, ref c, n, 1, 1, 0.0, nlthreshold, 0, true, ref state);
            while( lsfit.lsfitnonlineariteration(ref state) )
            {
                if( state.needf )
                {
                    state.f = 1/(1+state.c[0]*AP.Math.Sqr(state.x[0]));
                }
                if( state.needfg )
                {
                    state.f = 1/(1+state.c[0]*AP.Math.Sqr(state.x[0]));
                    state.g[0] = -(AP.Math.Sqr(state.x[0])/AP.Math.Sqr(1+state.c[0]*AP.Math.Sqr(state.x[0])));
                }
            }
            lsfit.lsfitnonlinearresults(ref state, ref info, ref c, ref rep);
            if( info<=0 )
            {
                nlserrors = true;
            }
            else
            {
                nlserrors = nlserrors | (double)(Math.Abs(c[0]-2))>(double)(100*nlthreshold);
            }
            
            //
            // solve simple task (fitting by constant function) and check
            // correctness of the errors calculated by subroutines
            //
            for(pass=1; pass<=passcount; pass++)
            {
                
                //
                // test on task with non-zero Yi
                //
                n = 4;
                v1 = AP.Math.RandomReal();
                v2 = AP.Math.RandomReal();
                v = 1+AP.Math.RandomReal();
                c = new double[1];
                c[0] = 1+2*AP.Math.RandomReal();
                a = new double[4, 1];
                y = new double[4];
                a[0,0] = 1;
                y[0] = v-v2;
                a[1,0] = 1;
                y[1] = v-v1;
                a[2,0] = 1;
                y[2] = v+v1;
                a[3,0] = 1;
                y[3] = v+v2;
                refrms = Math.Sqrt((AP.Math.Sqr(v1)+AP.Math.Sqr(v2))/2);
                refavg = (Math.Abs(v1)+Math.Abs(v2))/2;
                refavgrel = 0.25*(Math.Abs(v2)/Math.Abs(v-v2)+Math.Abs(v1)/Math.Abs(v-v1)+Math.Abs(v1)/Math.Abs(v+v1)+Math.Abs(v2)/Math.Abs(v+v2));
                refmax = Math.Max(v1, v2);
                
                //
                // Test LLS
                //
                lsfit.lsfitlinear(ref y, ref a, 4, 1, ref info, ref c, ref rep);
                if( info<=0 )
                {
                    llserrors = true;
                }
                else
                {
                    llserrors = llserrors | (double)(Math.Abs(c[0]-v))>(double)(threshold);
                    llserrors = llserrors | (double)(Math.Abs(rep.rmserror-refrms))>(double)(threshold);
                    llserrors = llserrors | (double)(Math.Abs(rep.avgerror-refavg))>(double)(threshold);
                    llserrors = llserrors | (double)(Math.Abs(rep.avgrelerror-refavgrel))>(double)(threshold);
                    llserrors = llserrors | (double)(Math.Abs(rep.maxerror-refmax))>(double)(threshold);
                }
                
                //
                // Test NLS
                //
                lsfit.lsfitnonlinearfg(ref a, ref y, ref c, 4, 1, 1, 0.0, nlthreshold, 0, true, ref state);
                while( lsfit.lsfitnonlineariteration(ref state) )
                {
                    if( state.needf )
                    {
                        state.f = state.c[0];
                    }
                    if( state.needfg )
                    {
                        state.f = state.c[0];
                        state.g[0] = 1;
                    }
                }
                lsfit.lsfitnonlinearresults(ref state, ref info, ref c, ref rep);
                if( info<=0 )
                {
                    nlserrors = true;
                }
                else
                {
                    nlserrors = nlserrors | (double)(Math.Abs(c[0]-v))>(double)(threshold);
                    nlserrors = nlserrors | (double)(Math.Abs(rep.rmserror-refrms))>(double)(threshold);
                    nlserrors = nlserrors | (double)(Math.Abs(rep.avgerror-refavg))>(double)(threshold);
                    nlserrors = nlserrors | (double)(Math.Abs(rep.avgrelerror-refavgrel))>(double)(threshold);
                    nlserrors = nlserrors | (double)(Math.Abs(rep.maxerror-refmax))>(double)(threshold);
                }
            }
            
            //
            // report
            //
            waserrors = llserrors | nlserrors;
            if( !silent )
            {
                System.Console.Write("TESTING LEAST SQUARES");
                System.Console.WriteLine();
                System.Console.Write("LINEAR LEAST SQUARES:                    ");
                if( llserrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("NON-LINEAR LEAST SQUARES:                ");
                if( nlserrors )
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
            
            //
            // end
            //
            result = !waserrors;
            return result;
        }


        /*************************************************************************
        Tests whether C is solution of (possibly) constrained LLS problem
        *************************************************************************/
        private static bool isglssolution(int n,
            int m,
            int k,
            ref double[] y,
            ref double[] w,
            ref double[,] fmatrix,
            ref double[,] cmatrix,
            double[] c)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            double[] c2 = new double[0];
            double[] sv = new double[0];
            double[] deltac = new double[0];
            double[] deltaproj = new double[0];
            double[,] u = new double[0,0];
            double[,] vt = new double[0,0];
            double v = 0;
            double s1 = 0;
            double s2 = 0;
            double s3 = 0;
            double delta = 0;
            double threshold = 0;
            int i_ = 0;

            c = (double[])c.Clone();

            
            //
            // Setup.
            // Threshold is small because CMatrix may be ill-conditioned
            //
            delta = 0.001;
            threshold = Math.Sqrt(AP.Math.MachineEpsilon);
            c2 = new double[m];
            deltac = new double[m];
            deltaproj = new double[m];
            
            //
            // test whether C is feasible point or not (projC must be close to C)
            //
            for(i=0; i<=k-1; i++)
            {
                v = 0.0;
                for(i_=0; i_<=m-1;i_++)
                {
                    v += cmatrix[i,i_]*c[i_];
                }
                if( (double)(Math.Abs(v-cmatrix[i,m]))>(double)(threshold) )
                {
                    result = false;
                    return result;
                }
            }
            
            //
            // find orthogonal basis of Null(CMatrix) (stored in rows from K to M-1)
            //
            if( k>0 )
            {
                svd.rmatrixsvd(cmatrix, k, m, 0, 2, 2, ref sv, ref u, ref vt);
            }
            
            //
            // Test result
            //
            result = true;
            s1 = getglserror(n, m, ref y, ref w, ref fmatrix, ref c);
            for(j=0; j<=m-1; j++)
            {
                
                //
                // prepare modification of C which leave us in the feasible set.
                //
                // let deltaC be increment on Jth coordinate, then project
                // deltaC in the Null(CMatrix) and store result in DeltaProj
                //
                for(i_=0; i_<=m-1;i_++)
                {
                    c2[i_] = c[i_];
                }
                for(i=0; i<=m-1; i++)
                {
                    if( i==j )
                    {
                        deltac[i] = delta;
                    }
                    else
                    {
                        deltac[i] = 0;
                    }
                }
                if( k==0 )
                {
                    for(i_=0; i_<=m-1;i_++)
                    {
                        deltaproj[i_] = deltac[i_];
                    }
                }
                else
                {
                    for(i=0; i<=m-1; i++)
                    {
                        deltaproj[i] = 0;
                    }
                    for(i=k; i<=m-1; i++)
                    {
                        v = 0.0;
                        for(i_=0; i_<=m-1;i_++)
                        {
                            v += vt[i,i_]*deltac[i_];
                        }
                        for(i_=0; i_<=m-1;i_++)
                        {
                            deltaproj[i_] = deltaproj[i_] + v*vt[i,i_];
                        }
                    }
                }
                
                //
                // now we have DeltaProj such that if C is feasible,
                // then C+DeltaProj is feasible too
                //
                for(i_=0; i_<=m-1;i_++)
                {
                    c2[i_] = c[i_];
                }
                for(i_=0; i_<=m-1;i_++)
                {
                    c2[i_] = c2[i_] + deltaproj[i_];
                }
                s2 = getglserror(n, m, ref y, ref w, ref fmatrix, ref c2);
                for(i_=0; i_<=m-1;i_++)
                {
                    c2[i_] = c[i_];
                }
                for(i_=0; i_<=m-1;i_++)
                {
                    c2[i_] = c2[i_] - deltaproj[i_];
                }
                s3 = getglserror(n, m, ref y, ref w, ref fmatrix, ref c2);
                result = result & (double)(s2)>=(double)(s1/(1+threshold)) & (double)(s3)>=(double)(s1/(1+threshold));
            }
            return result;
        }


        /*************************************************************************
        Tests whether C is solution of LLS problem
        *************************************************************************/
        private static double getglserror(int n,
            int m,
            ref double[] y,
            ref double[] w,
            ref double[,] fmatrix,
            ref double[] c)
        {
            double result = 0;
            int i = 0;
            double v = 0;
            int i_ = 0;

            result = 0;
            for(i=0; i<=n-1; i++)
            {
                v = 0.0;
                for(i_=0; i_<=m-1;i_++)
                {
                    v += fmatrix[i,i_]*c[i_];
                }
                result = result+AP.Math.Sqr(w[i]*(v-y[i]));
            }
            return result;
        }


        /*************************************************************************
        Subroutine for nonlinear fitting of linear problem
        *************************************************************************/
        private static void fitlinearnonlinear(int m,
            bool gradonly,
            ref double[,] xy,
            ref lsfit.lsfitstate state,
            ref bool nlserrors)
        {
            int i = 0;
            int j = 0;
            double v = 0;
            int i_ = 0;

            while( lsfit.lsfitnonlineariteration(ref state) )
            {
                
                //
                // assume that one and only one of flags is set
                // test that we didn't request hessian in hessian-free setting
                //
                if( gradonly & state.needfgh )
                {
                    nlserrors = true;
                }
                i = 0;
                if( state.needf )
                {
                    i = i+1;
                }
                if( state.needfg )
                {
                    i = i+1;
                }
                if( state.needfgh )
                {
                    i = i+1;
                }
                if( i!=1 )
                {
                    nlserrors = true;
                }
                
                //
                // test that PointIndex is consistent with actual point passed
                //
                for(i=0; i<=m-1; i++)
                {
                    nlserrors = nlserrors | (double)(xy[state.pointindex,i])!=(double)(state.x[i]);
                }
                
                //
                // calculate
                //
                if( state.needf )
                {
                    v = 0.0;
                    for(i_=0; i_<=m-1;i_++)
                    {
                        v += state.x[i_]*state.c[i_];
                    }
                    state.f = v;
                    continue;
                }
                if( state.needfg )
                {
                    v = 0.0;
                    for(i_=0; i_<=m-1;i_++)
                    {
                        v += state.x[i_]*state.c[i_];
                    }
                    state.f = v;
                    for(i_=0; i_<=m-1;i_++)
                    {
                        state.g[i_] = state.x[i_];
                    }
                    continue;
                }
                if( state.needfgh )
                {
                    v = 0.0;
                    for(i_=0; i_<=m-1;i_++)
                    {
                        v += state.x[i_]*state.c[i_];
                    }
                    state.f = v;
                    for(i_=0; i_<=m-1;i_++)
                    {
                        state.g[i_] = state.x[i_];
                    }
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            state.h[i,j] = 0;
                        }
                    }
                    continue;
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool llstestunit_test_silent()
        {
            bool result = new bool();

            result = testlls(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool llstestunit_test()
        {
            bool result = new bool();

            result = testlls(false);
            return result;
        }
    }
}
