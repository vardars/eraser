
using System;

namespace alglib
{
    public class testhevdbiunit
    {
        /*************************************************************************
        Testing hermitian EVD, BI
        *************************************************************************/
        public static bool testhevdbi(bool silent)
        {
            bool result = new bool();
            AP.Complex[,] a = new AP.Complex[0,0];
            AP.Complex[,] al = new AP.Complex[0,0];
            AP.Complex[,] au = new AP.Complex[0,0];
            int pass = 0;
            int n = 0;
            int i = 0;
            int j = 0;
            int mkind = 0;
            int passcount = 0;
            int maxn = 0;
            double valerr = 0;
            double vecerr = 0;
            bool wnsorted = new bool();
            int failc = 0;
            int runs = 0;
            double failr = 0;
            double failthreshold = 0;
            double threshold = 0;
            bool waserrors = new bool();
            bool wfailed = new bool();
            int m = 0;
            double[,] z = new double[0,0];

            failthreshold = 0.005;
            threshold = 1000*AP.Math.MachineEpsilon;
            valerr = 0;
            vecerr = 0;
            wnsorted = false;
            wfailed = false;
            failc = 0;
            runs = 0;
            maxn = 10;
            passcount = 3;
            
            //
            // Main cycle
            //
            for(n=1; n<=maxn; n++)
            {
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // Prepare
                    //
                    a = new AP.Complex[n-1+1, n-1+1];
                    al = new AP.Complex[n-1+1, n-1+1];
                    au = new AP.Complex[n-1+1, n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=i+1; j<=n-1; j++)
                        {
                            
                            //
                            // A
                            //
                            a[i,j].x = 2*AP.Math.RandomReal()-1;
                            a[i,j].y = 2*AP.Math.RandomReal()-1;
                            a[j,i] = AP.Math.Conj(a[i,j]);
                            
                            //
                            // A lower
                            //
                            al[i,j].x = 2*AP.Math.RandomReal()-1;
                            al[i,j].y = 2*AP.Math.RandomReal()-1;
                            al[j,i] = a[j,i];
                            
                            //
                            // A upper
                            //
                            au[i,j] = a[i,j];
                            au[j,i].x = 2*AP.Math.RandomReal()-1;
                            au[j,i].y = 2*AP.Math.RandomReal()-1;
                        }
                        a[i,i] = 2*AP.Math.RandomReal()-1;
                        al[i,i] = a[i,i];
                        au[i,i] = a[i,i];
                    }
                    testevdproblem(ref a, ref al, ref au, n, ref valerr, ref vecerr, ref wnsorted, ref failc);
                    runs = runs+1;
                }
            }
            
            //
            // report
            //
            failr = (double)(failc)/(double)(runs);
            wfailed = (double)(failr)>(double)(failthreshold);
            waserrors = (double)(valerr)>(double)(threshold) | (double)(vecerr)>(double)(threshold) | wnsorted | wfailed;
            if( !silent )
            {
                System.Console.Write("TESTING HERMITIAN BISECTION AND INVERSE ITERATION EVD");
                System.Console.WriteLine();
                System.Console.Write("EVD values error (different variants):   ");
                System.Console.Write("{0,5:E3}",valerr);
                System.Console.WriteLine();
                System.Console.Write("EVD vectors error:                       ");
                System.Console.Write("{0,5:E3}",vecerr);
                System.Console.WriteLine();
                System.Console.Write("Eigen values order:                      ");
                if( !wnsorted )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("Always successfully converged:           ");
                if( !wfailed )
                {
                    System.Console.Write("YES");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("NO");
                    System.Console.WriteLine();
                    System.Console.Write("Fail ratio:                              ");
                    System.Console.Write("{0,5:F3}",failr);
                    System.Console.WriteLine();
                }
                System.Console.Write("Threshold:                               ");
                System.Console.Write("{0,5:E3}",threshold);
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
        Unsets 2D array.
        *************************************************************************/
        private static void unset2d(ref AP.Complex[,] a)
        {
            a = new AP.Complex[0+1, 0+1];
            a[0,0] = 2*AP.Math.RandomReal()-1;
        }


        /*************************************************************************
        Unsets 1D array.
        *************************************************************************/
        private static void unset1d(ref double[] a)
        {
            a = new double[0+1];
            a[0] = 2*AP.Math.RandomReal()-1;
        }


        /*************************************************************************
        Reference EVD
        *************************************************************************/
        private static bool refevd(AP.Complex[,] a,
            int n,
            ref double[] lambda,
            ref AP.Complex[,] z)
        {
            bool result = new bool();
            AP.Complex[,] z2 = new AP.Complex[0,0];
            AP.Complex[,] z1 = new AP.Complex[0,0];
            AP.Complex[] tau = new AP.Complex[0];
            double[] d = new double[0];
            double[] e = new double[0];
            double[] d1 = new double[0];
            double[] e1 = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            double vr = 0;
            AP.Complex v = 0;

            a = (AP.Complex[,])a.Clone();

            
            //
            // to tridiagonal
            //
            htridiagonal.hmatrixtd(ref a, n, true, ref tau, ref d, ref e);
            htridiagonal.hmatrixtdunpackq(ref a, n, true, ref tau, ref z2);
            
            //
            // TDEVD
            //
            z1 = new AP.Complex[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    z1[i,j] = z2[i-1,j-1];
                }
            }
            d1 = new double[n+1];
            for(i=1; i<=n; i++)
            {
                d1[i] = d[i-1];
            }
            e1 = new double[n+1];
            for(i=2; i<=n; i++)
            {
                e1[i] = e[i-2];
            }
            result = hbitridiagonalqlieigenvaluesandvectors(ref d1, e1, n, ref z1);
            if( result )
            {
                
                //
                // copy
                //
                lambda = new double[n-1+1];
                for(i=0; i<=n-1; i++)
                {
                    lambda[i] = d1[i+1];
                }
                z = new AP.Complex[n-1+1, n-1+1];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        z[i,j] = z1[i+1,j+1];
                    }
                }
                
                //
                // Use Selection Sort to minimize swaps of eigenvectors
                //
                for(i=0; i<=n-2; i++)
                {
                    k = i;
                    for(j=i+1; j<=n-1; j++)
                    {
                        if( (double)(lambda[j])<(double)(lambda[k]) )
                        {
                            k = j;
                        }
                    }
                    if( k!=i )
                    {
                        vr = lambda[i];
                        lambda[i] = lambda[k];
                        lambda[k] = vr;
                        for(j=0; j<=n-1; j++)
                        {
                            v = z[j,i];
                            z[j,i] = z[j,k];
                            z[j,k] = v;
                        }
                    }
                }
            }
            return result;
        }


        private static bool hbitridiagonalqlieigenvaluesandvectors(ref double[] d,
            double[] e,
            int n,
            ref AP.Complex[,] z)
        {
            bool result = new bool();
            int m = 0;
            int l = 0;
            int iter = 0;
            int i = 0;
            int k = 0;
            double s = 0;
            double r = 0;
            double p = 0;
            double g = 0;
            double f = 0;
            AP.Complex fc = 0;
            double dd = 0;
            double c = 0;
            double b = 0;

            e = (double[])e.Clone();

            result = true;
            if( n==1 )
            {
                return result;
            }
            for(i=2; i<=n; i++)
            {
                e[i-1] = e[i];
            }
            e[n] = 0.0;
            for(l=1; l<=n; l++)
            {
                iter = 0;
                do
                {
                    for(m=l; m<=n-1; m++)
                    {
                        dd = Math.Abs(d[m])+Math.Abs(d[m+1]);
                        if( (double)(Math.Abs(e[m])+dd)==(double)(dd) )
                        {
                            break;
                        }
                    }
                    if( m!=l )
                    {
                        if( iter==30 )
                        {
                            result = false;
                            return result;
                        }
                        iter = iter+1;
                        g = (d[l+1]-d[l])/(2*e[l]);
                        if( (double)(Math.Abs(g))<(double)(1) )
                        {
                            r = Math.Sqrt(1+AP.Math.Sqr(g));
                        }
                        else
                        {
                            r = Math.Abs(g)*Math.Sqrt(1+AP.Math.Sqr(1/g));
                        }
                        if( (double)(g)<(double)(0) )
                        {
                            g = d[m]-d[l]+e[l]/(g-r);
                        }
                        else
                        {
                            g = d[m]-d[l]+e[l]/(g+r);
                        }
                        s = 1;
                        c = 1;
                        p = 0;
                        for(i=m-1; i>=l; i--)
                        {
                            f = s*e[i];
                            b = c*e[i];
                            if( (double)(Math.Abs(f))<(double)(Math.Abs(g)) )
                            {
                                r = Math.Abs(g)*Math.Sqrt(1+AP.Math.Sqr(f/g));
                            }
                            else
                            {
                                r = Math.Abs(f)*Math.Sqrt(1+AP.Math.Sqr(g/f));
                            }
                            e[i+1] = r;
                            if( (double)(r)==(double)(0) )
                            {
                                d[i+1] = d[i+1]-p;
                                e[m] = 0;
                                break;
                            }
                            s = f/r;
                            c = g/r;
                            g = d[i+1]-p;
                            r = (d[i]-g)*s+2.0*c*b;
                            p = s*r;
                            d[i+1] = g+p;
                            g = c*r-b;
                            for(k=1; k<=n; k++)
                            {
                                fc = z[k,i+1];
                                z[k,i+1] = s*z[k,i]+c*fc;
                                z[k,i] = c*z[k,i]-s*fc;
                            }
                        }
                        if( (double)(r)==(double)(0) & i>=1 )
                        {
                            continue;
                        }
                        d[l] = d[l]-p;
                        e[l] = g;
                        e[m] = 0.0;
                    }
                }
                while( m!=l );
            }
            return result;
        }


        /*************************************************************************
        Tests Z*Lambda*Z' against tridiag(D,E).
        Returns relative error.
        *************************************************************************/
        private static double testproduct(ref double[] d,
            ref double[] e,
            int n,
            ref double[,] z,
            ref double[] lambda)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            double mx = 0;

            result = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    
                    //
                    // Calculate V = A[i,j], A = Z*Lambda*Z'
                    //
                    v = 0;
                    for(k=0; k<=n-1; k++)
                    {
                        v = v+z[i,k]*lambda[k]*z[j,k];
                    }
                    
                    //
                    // Compare
                    //
                    if( Math.Abs(i-j)==0 )
                    {
                        result = Math.Max(result, Math.Abs(v-d[i]));
                    }
                    if( Math.Abs(i-j)==1 )
                    {
                        result = Math.Max(result, Math.Abs(v-e[Math.Min(i, j)]));
                    }
                    if( Math.Abs(i-j)>1 )
                    {
                        result = Math.Max(result, Math.Abs(v));
                    }
                }
            }
            mx = 0;
            for(i=0; i<=n-1; i++)
            {
                mx = Math.Max(mx, Math.Abs(d[i]));
            }
            for(i=0; i<=n-2; i++)
            {
                mx = Math.Max(mx, Math.Abs(e[i]));
            }
            if( (double)(mx)==(double)(0) )
            {
                mx = 1;
            }
            result = result/mx;
            return result;
        }


        /*************************************************************************
        Tests Z*Z' against diag(1...1)
        Returns absolute error.
        *************************************************************************/
        private static double testort(ref double[,] z,
            int n)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            int i_ = 0;

            result = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += z[i_,i]*z[i_,j];
                    }
                    if( i==j )
                    {
                        v = v-1;
                    }
                    result = Math.Max(result, Math.Abs(v));
                }
            }
            return result;
        }


        /*************************************************************************
        Tests EVD problem
        *************************************************************************/
        private static void testevdproblem(ref AP.Complex[,] afull,
            ref AP.Complex[,] al,
            ref AP.Complex[,] au,
            int n,
            ref double valerr,
            ref double vecerr,
            ref bool wnsorted,
            ref int failc)
        {
            double[] lambda = new double[0];
            double[] lambdaref = new double[0];
            AP.Complex[,] z = new AP.Complex[0,0];
            AP.Complex[,] zref = new AP.Complex[0,0];
            bool wsucc = new bool();
            int i = 0;
            int j = 0;
            int k = 0;
            int m = 0;
            int i1 = 0;
            int i2 = 0;
            AP.Complex v = 0;
            double a = 0;
            double b = 0;
            int i_ = 0;

            lambdaref = new double[n-1+1];
            zref = new AP.Complex[n-1+1, n-1+1];
            
            //
            // Reference EVD
            //
            if( !refevd(afull, n, ref lambdaref, ref zref) )
            {
                failc = failc+1;
                return;
            }
            
            //
            // Test different combinations
            //
            for(i1=0; i1<=n-1; i1++)
            {
                for(i2=i1; i2<=n-1; i2++)
                {
                    
                    //
                    // Select A, B
                    //
                    if( i1>0 )
                    {
                        a = 0.5*(lambdaref[i1]+lambdaref[i1-1]);
                    }
                    else
                    {
                        a = lambdaref[0]-1;
                    }
                    if( i2<n-1 )
                    {
                        b = 0.5*(lambdaref[i2]+lambdaref[i2+1]);
                    }
                    else
                    {
                        b = lambdaref[n-1]+1;
                    }
                    
                    //
                    // Test interval, no vectors, lower A
                    //
                    unset1d(ref lambda);
                    unset2d(ref z);
                    if( !hbisinv.hmatrixevdr(al, n, 0, false, a, b, ref m, ref lambda, ref z) )
                    {
                        failc = failc+1;
                        return;
                    }
                    if( m!=i2-i1+1 )
                    {
                        failc = failc+1;
                        return;
                    }
                    for(k=0; k<=m-1; k++)
                    {
                        valerr = Math.Max(valerr, Math.Abs(lambda[k]-lambdaref[i1+k]));
                    }
                    
                    //
                    // Test interval, no vectors, upper A
                    //
                    unset1d(ref lambda);
                    unset2d(ref z);
                    if( !hbisinv.hmatrixevdr(au, n, 0, true, a, b, ref m, ref lambda, ref z) )
                    {
                        failc = failc+1;
                        return;
                    }
                    if( m!=i2-i1+1 )
                    {
                        failc = failc+1;
                        return;
                    }
                    for(k=0; k<=m-1; k++)
                    {
                        valerr = Math.Max(valerr, Math.Abs(lambda[k]-lambdaref[i1+k]));
                    }
                    
                    //
                    // Test indexes, no vectors, lower A
                    //
                    unset1d(ref lambda);
                    unset2d(ref z);
                    if( !hbisinv.hmatrixevdi(al, n, 0, false, i1, i2, ref lambda, ref z) )
                    {
                        failc = failc+1;
                        return;
                    }
                    m = i2-i1+1;
                    for(k=0; k<=m-1; k++)
                    {
                        valerr = Math.Max(valerr, Math.Abs(lambda[k]-lambdaref[i1+k]));
                    }
                    
                    //
                    // Test indexes, no vectors, upper A
                    //
                    unset1d(ref lambda);
                    unset2d(ref z);
                    if( !hbisinv.hmatrixevdi(au, n, 0, true, i1, i2, ref lambda, ref z) )
                    {
                        failc = failc+1;
                        return;
                    }
                    m = i2-i1+1;
                    for(k=0; k<=m-1; k++)
                    {
                        valerr = Math.Max(valerr, Math.Abs(lambda[k]-lambdaref[i1+k]));
                    }
                    
                    //
                    // Test interval, do not transform vectors, lower A
                    //
                    unset1d(ref lambda);
                    unset2d(ref z);
                    if( !hbisinv.hmatrixevdr(al, n, 1, false, a, b, ref m, ref lambda, ref z) )
                    {
                        failc = failc+1;
                        return;
                    }
                    if( m!=i2-i1+1 )
                    {
                        failc = failc+1;
                        return;
                    }
                    for(k=0; k<=m-1; k++)
                    {
                        valerr = Math.Max(valerr, Math.Abs(lambda[k]-lambdaref[i1+k]));
                    }
                    for(j=0; j<=m-1; j++)
                    {
                        v = 0.0;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            v += z[i_,j]*AP.Math.Conj(zref[i_,i1+j]);
                        }
                        v = 1/v;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            z[i_,j] = v*z[i_,j];
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            vecerr = Math.Max(vecerr, AP.Math.AbsComplex(z[i,j]-zref[i,i1+j]));
                        }
                    }
                    
                    //
                    // Test interval, do not transform vectors, upper A
                    //
                    unset1d(ref lambda);
                    unset2d(ref z);
                    if( !hbisinv.hmatrixevdr(au, n, 1, true, a, b, ref m, ref lambda, ref z) )
                    {
                        failc = failc+1;
                        return;
                    }
                    if( m!=i2-i1+1 )
                    {
                        failc = failc+1;
                        return;
                    }
                    for(k=0; k<=m-1; k++)
                    {
                        valerr = Math.Max(valerr, Math.Abs(lambda[k]-lambdaref[i1+k]));
                    }
                    for(j=0; j<=m-1; j++)
                    {
                        v = 0.0;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            v += z[i_,j]*AP.Math.Conj(zref[i_,i1+j]);
                        }
                        v = 1/v;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            z[i_,j] = v*z[i_,j];
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            vecerr = Math.Max(vecerr, AP.Math.AbsComplex(z[i,j]-zref[i,i1+j]));
                        }
                    }
                    
                    //
                    // Test indexes, do not transform vectors, lower A
                    //
                    unset1d(ref lambda);
                    unset2d(ref z);
                    if( !hbisinv.hmatrixevdi(al, n, 1, false, i1, i2, ref lambda, ref z) )
                    {
                        failc = failc+1;
                        return;
                    }
                    m = i2-i1+1;
                    for(k=0; k<=m-1; k++)
                    {
                        valerr = Math.Max(valerr, Math.Abs(lambda[k]-lambdaref[i1+k]));
                    }
                    for(j=0; j<=m-1; j++)
                    {
                        v = 0.0;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            v += z[i_,j]*AP.Math.Conj(zref[i_,i1+j]);
                        }
                        v = 1/v;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            z[i_,j] = v*z[i_,j];
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            vecerr = Math.Max(vecerr, AP.Math.AbsComplex(z[i,j]-zref[i,i1+j]));
                        }
                    }
                    
                    //
                    // Test indexes, do not transform vectors, upper A
                    //
                    unset1d(ref lambda);
                    unset2d(ref z);
                    if( !hbisinv.hmatrixevdi(au, n, 1, true, i1, i2, ref lambda, ref z) )
                    {
                        failc = failc+1;
                        return;
                    }
                    m = i2-i1+1;
                    for(k=0; k<=m-1; k++)
                    {
                        valerr = Math.Max(valerr, Math.Abs(lambda[k]-lambdaref[i1+k]));
                    }
                    for(j=0; j<=m-1; j++)
                    {
                        v = 0.0;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            v += z[i_,j]*AP.Math.Conj(zref[i_,i1+j]);
                        }
                        v = 1/v;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            z[i_,j] = v*z[i_,j];
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            vecerr = Math.Max(vecerr, AP.Math.AbsComplex(z[i,j]-zref[i,i1+j]));
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testhevdbiunit_test_silent()
        {
            bool result = new bool();

            result = testhevdbi(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testhevdbiunit_test()
        {
            bool result = new bool();

            result = testhevdbi(false);
            return result;
        }
    }
}
