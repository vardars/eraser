
using System;

namespace alglib
{
    public class testtdevdbiunit
    {
        /*************************************************************************
        Testing EVD, BI
        *************************************************************************/
        public static bool testtdevdbi(bool silent)
        {
            bool result = new bool();
            double[] d = new double[0];
            double[] e = new double[0];
            int pass = 0;
            int n = 0;
            int i = 0;
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
            bool wspecialf = new bool();
            int m = 0;
            double[,] z = new double[0,0];

            failthreshold = 0.005;
            threshold = 1.0E7*AP.Math.MachineEpsilon;
            valerr = 0;
            vecerr = 0;
            wnsorted = false;
            wfailed = false;
            wspecialf = false;
            failc = 0;
            runs = 0;
            maxn = 15;
            passcount = 5;
            
            //
            // Main cycle
            //
            for(n=1; n<=maxn; n++)
            {
                
                //
                // Different tasks
                //
                for(mkind=1; mkind<=4; mkind++)
                {
                    fillde(ref d, ref e, n, mkind);
                    testevdproblem(ref d, ref e, n, ref valerr, ref vecerr, ref wnsorted, ref failc);
                    runs = runs+1;
                }
                
                //
                // Special tests
                //
                fillde(ref d, ref e, n, 0);
                if( !tdbisinv.smatrixtdevdr(ref d, ref e, n, 0, -1.0, 0.0, ref m, ref z) )
                {
                    wspecialf = true;
                    continue;
                }
                wspecialf = wspecialf | m!=n;
                fillde(ref d, ref e, n, 0);
                if( !tdbisinv.smatrixtdevdr(ref d, ref e, n, 0, 0.0, 1.0, ref m, ref z) )
                {
                    wspecialf = true;
                    continue;
                }
                wspecialf = wspecialf | m!=0;
                for(i=0; i<=n-1; i++)
                {
                    fillde(ref d, ref e, n, 0);
                    if( !tdbisinv.smatrixtdevdi(ref d, ref e, n, 0, i, i, ref z) )
                    {
                        wspecialf = true;
                        continue;
                    }
                    wspecialf = wspecialf | (double)(d[0])!=(double)(0);
                }
            }
            
            //
            // report
            //
            failr = (double)(failc)/(double)(runs);
            wfailed = (double)(failr)>(double)(failthreshold);
            waserrors = (double)(valerr)>(double)(threshold) | (double)(vecerr)>(double)(threshold) | wnsorted | wfailed | wspecialf;
            if( !silent )
            {
                System.Console.Write("TESTING TRIDIAGONAL BISECTION AND INVERSE ITERATION EVD");
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
                System.Console.Write("Special tests:                           ");
                if( !wspecialf )
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
        Reference EVD
        *************************************************************************/
        private static bool refevd(ref double[] d,
            ref double[] e,
            int n,
            ref double[] lambda,
            ref double[,] z)
        {
            bool result = new bool();
            double[,] z1 = new double[0,0];
            double[] d1 = new double[0];
            double[] e1 = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;

            z1 = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    z1[i,j] = 0;
                }
            }
            for(i=1; i<=n; i++)
            {
                z1[i,i] = 1;
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
            result = tdbitridiagonalqlieigenvaluesandvectors(ref d1, e1, n, ref z1);
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
                z = new double[n-1+1, n-1+1];
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
                        v = lambda[i];
                        lambda[i] = lambda[k];
                        lambda[k] = v;
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


        private static bool tdbitridiagonalqlieigenvaluesandvectors(ref double[] d,
            double[] e,
            int n,
            ref double[,] z)
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
                                f = z[k,i+1];
                                z[k,i+1] = s*z[k,i]+c*f;
                                z[k,i] = c*z[k,i]-s*f;
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
        Fills D and E
        *************************************************************************/
        private static void fillde(ref double[] d,
            ref double[] e,
            int n,
            int filltype)
        {
            int i = 0;
            int j = 0;

            d = new double[n-1+1];
            if( n>1 )
            {
                e = new double[n-2+1];
            }
            if( filltype==0 )
            {
                
                //
                // Zero matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    d[i] = 0;
                }
                for(i=0; i<=n-2; i++)
                {
                    e[i] = 0;
                }
                return;
            }
            if( filltype==1 )
            {
                
                //
                // Diagonal matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    d[i] = 2*AP.Math.RandomReal()-1;
                }
                for(i=0; i<=n-2; i++)
                {
                    e[i] = 0;
                }
                return;
            }
            if( filltype==2 )
            {
                
                //
                // Off-diagonal matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    d[i] = 0;
                }
                for(i=0; i<=n-2; i++)
                {
                    e[i] = 2*AP.Math.RandomReal()-1;
                }
                return;
            }
            if( filltype==3 )
            {
                
                //
                // Dense matrix with blocks
                //
                for(i=0; i<=n-1; i++)
                {
                    d[i] = 2*AP.Math.RandomReal()-1;
                }
                for(i=0; i<=n-2; i++)
                {
                    e[i] = 2*AP.Math.RandomReal()-1;
                }
                j = 1;
                i = 2;
                while( j<=n-2 )
                {
                    e[j] = 0;
                    j = j+i;
                    i = i+1;
                }
                return;
            }
            
            //
            // dense matrix
            //
            for(i=0; i<=n-1; i++)
            {
                d[i] = 2*AP.Math.RandomReal()-1;
            }
            for(i=0; i<=n-2; i++)
            {
                e[i] = 2*AP.Math.RandomReal()-1;
            }
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
        private static void testevdproblem(ref double[] d,
            ref double[] e,
            int n,
            ref double valerr,
            ref double vecerr,
            ref bool wnsorted,
            ref int failc)
        {
            double[] lambda = new double[0];
            double[] lambdaref = new double[0];
            double[,] z = new double[0,0];
            double[,] zref = new double[0,0];
            double[,] a1 = new double[0,0];
            double[,] a2 = new double[0,0];
            double[,] ar = new double[0,0];
            bool wsucc = new bool();
            int i = 0;
            int j = 0;
            int k = 0;
            int m = 0;
            int i1 = 0;
            int i2 = 0;
            double v = 0;
            double a = 0;
            double b = 0;
            int i_ = 0;

            lambdaref = new double[n-1+1];
            zref = new double[n-1+1, n-1+1];
            a1 = new double[n-1+1, n-1+1];
            a2 = new double[n-1+1, n-1+1];
            
            //
            // Reference EVD
            //
            if( !refevd(ref d, ref e, n, ref lambdaref, ref zref) )
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
                    // Test interval, no vectors
                    //
                    lambda = new double[n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        lambda[i] = d[i];
                    }
                    if( !tdbisinv.smatrixtdevdr(ref lambda, ref e, n, 0, a, b, ref m, ref z) )
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
                    // Test indexes, no vectors
                    //
                    lambda = new double[n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        lambda[i] = d[i];
                    }
                    if( !tdbisinv.smatrixtdevdi(ref lambda, ref e, n, 0, i1, i2, ref z) )
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
                    // Test interval, transform vectors
                    //
                    lambda = new double[n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        lambda[i] = d[i];
                    }
                    a1 = new double[n-1+1, n-1+1];
                    a2 = new double[n-1+1, n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a1[i,j] = 2*AP.Math.RandomReal()-1;
                            a2[i,j] = a1[i,j];
                        }
                    }
                    if( !tdbisinv.smatrixtdevdr(ref lambda, ref e, n, 1, a, b, ref m, ref a1) )
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
                    ar = new double[n-1+1, m-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            v = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                v += a2[i,i_]*zref[i_,i1+j];
                            }
                            ar[i,j] = v;
                        }
                    }
                    for(j=0; j<=m-1; j++)
                    {
                        v = 0.0;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            v += a1[i_,j]*ar[i_,j];
                        }
                        if( (double)(v)<(double)(0) )
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                ar[i_,j] = -1*ar[i_,j];
                            }
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            vecerr = Math.Max(vecerr, Math.Abs(a1[i,j]-ar[i,j]));
                        }
                    }
                    
                    //
                    // Test indexes, transform vectors
                    //
                    lambda = new double[n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        lambda[i] = d[i];
                    }
                    a1 = new double[n-1+1, n-1+1];
                    a2 = new double[n-1+1, n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a1[i,j] = 2*AP.Math.RandomReal()-1;
                            a2[i,j] = a1[i,j];
                        }
                    }
                    if( !tdbisinv.smatrixtdevdi(ref lambda, ref e, n, 1, i1, i2, ref a1) )
                    {
                        failc = failc+1;
                        return;
                    }
                    m = i2-i1+1;
                    for(k=0; k<=m-1; k++)
                    {
                        valerr = Math.Max(valerr, Math.Abs(lambda[k]-lambdaref[i1+k]));
                    }
                    ar = new double[n-1+1, m-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            v = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                v += a2[i,i_]*zref[i_,i1+j];
                            }
                            ar[i,j] = v;
                        }
                    }
                    for(j=0; j<=m-1; j++)
                    {
                        v = 0.0;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            v += a1[i_,j]*ar[i_,j];
                        }
                        if( (double)(v)<(double)(0) )
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                ar[i_,j] = -1*ar[i_,j];
                            }
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            vecerr = Math.Max(vecerr, Math.Abs(a1[i,j]-ar[i,j]));
                        }
                    }
                    
                    //
                    // Test interval, do not transform vectors
                    //
                    lambda = new double[n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        lambda[i] = d[i];
                    }
                    z = new double[0+1, 0+1];
                    if( !tdbisinv.smatrixtdevdr(ref lambda, ref e, n, 2, a, b, ref m, ref z) )
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
                            v += z[i_,j]*zref[i_,i1+j];
                        }
                        if( (double)(v)<(double)(0) )
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                z[i_,j] = -1*z[i_,j];
                            }
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            vecerr = Math.Max(vecerr, Math.Abs(z[i,j]-zref[i,i1+j]));
                        }
                    }
                    
                    //
                    // Test interval, do not transform vectors
                    //
                    lambda = new double[n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        lambda[i] = d[i];
                    }
                    z = new double[0+1, 0+1];
                    if( !tdbisinv.smatrixtdevdi(ref lambda, ref e, n, 2, i1, i2, ref z) )
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
                            v += z[i_,j]*zref[i_,i1+j];
                        }
                        if( (double)(v)<(double)(0) )
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                z[i_,j] = -1*z[i_,j];
                            }
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            vecerr = Math.Max(vecerr, Math.Abs(z[i,j]-zref[i,i1+j]));
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testtdevdbiunit_test_silent()
        {
            bool result = new bool();

            result = testtdevdbi(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testtdevdbiunit_test()
        {
            bool result = new bool();

            result = testtdevdbi(false);
            return result;
        }
    }
}
