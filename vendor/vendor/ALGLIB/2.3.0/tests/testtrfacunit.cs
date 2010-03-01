
using System;

namespace alglib
{
    public class testtrfacunit
    {
        public static bool testtrfac(bool silent)
        {
            bool result = new bool();
            double[,] ra = new double[0,0];
            double[,] ral = new double[0,0];
            double[,] rau = new double[0,0];
            AP.Complex[,] ca = new AP.Complex[0,0];
            AP.Complex[,] cal = new AP.Complex[0,0];
            AP.Complex[,] cau = new AP.Complex[0,0];
            int m = 0;
            int n = 0;
            int mx = 0;
            int maxmn = 0;
            int i = 0;
            int j = 0;
            int minij = 0;
            int pass = 0;
            AP.Complex vc = 0;
            double vr = 0;
            bool waserrors = new bool();
            bool spderr = new bool();
            bool hpderr = new bool();
            bool rerr = new bool();
            bool cerr = new bool();
            bool properr = new bool();
            double threshold = 0;
            int i_ = 0;

            rerr = false;
            spderr = false;
            cerr = false;
            hpderr = false;
            properr = false;
            waserrors = false;
            maxmn = 4*ablas.ablasblocksize(ref ra)+1;
            threshold = 1000*AP.Math.MachineEpsilon*maxmn;
            
            //
            // test LU
            //
            for(mx=1; mx<=maxmn; mx++)
            {
                
                //
                // Initialize N/M, both are <=MX,
                // at least one of them is exactly equal to MX
                //
                n = 1+AP.Math.RandomInteger(mx);
                m = 1+AP.Math.RandomInteger(mx);
                if( (double)(AP.Math.RandomReal())>(double)(0.5) )
                {
                    n = mx;
                }
                else
                {
                    m = mx;
                }
                
                //
                // First, test on zero matrix
                //
                ra = new double[m, n];
                ca = new AP.Complex[m, n];
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        ra[i,j] = 0;
                        ca[i,j] = 0;
                    }
                }
                testcluproblem(ref ca, m, n, threshold, ref cerr, ref properr);
                testrluproblem(ref ra, m, n, threshold, ref rerr, ref properr);
                
                //
                // Second, random matrix with moderate condition number
                //
                ra = new double[m, n];
                ca = new AP.Complex[m, n];
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        ra[i,j] = 0;
                        ca[i,j] = 0;
                    }
                }
                for(i=0; i<=Math.Min(m, n)-1; i++)
                {
                    ra[i,i] = 1+10*AP.Math.RandomReal();
                    ca[i,i] = 1+10*AP.Math.RandomReal();
                }
                matgen.cmatrixrndorthogonalfromtheleft(ref ca, m, n);
                matgen.cmatrixrndorthogonalfromtheright(ref ca, m, n);
                matgen.rmatrixrndorthogonalfromtheleft(ref ra, m, n);
                matgen.rmatrixrndorthogonalfromtheright(ref ra, m, n);
                testcluproblem(ref ca, m, n, threshold, ref cerr, ref properr);
                testrluproblem(ref ra, m, n, threshold, ref rerr, ref properr);
            }
            
            //
            // Test Cholesky
            //
            for(n=1; n<=maxmn; n++)
            {
                
                //
                // Load CA (HPD matrix with low condition number),
                //      CAL and CAU - its lower and upper triangles
                //
                matgen.hpdmatrixrndcond(n, 1+50*AP.Math.RandomReal(), ref ca);
                cal = new AP.Complex[n, n];
                cau = new AP.Complex[n, n];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        cal[i,j] = i;
                        cau[i,j] = j;
                    }
                }
                for(i=0; i<=n-1; i++)
                {
                    for(i_=0; i_<=i;i_++)
                    {
                        cal[i,i_] = ca[i,i_];
                    }
                    for(i_=i; i_<=n-1;i_++)
                    {
                        cau[i,i_] = ca[i,i_];
                    }
                }
                
                //
                // Test HPDMatrixCholesky:
                // 1. it must leave upper (lower) part unchanged
                // 2. max(A-L*L^H) must be small
                //
                if( trfac.hpdmatrixcholesky(ref cal, n, false) )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            if( j>i )
                            {
                                hpderr = hpderr | cal[i,j]!=i;
                            }
                            else
                            {
                                vc = 0.0;
                                for(i_=0; i_<=j;i_++)
                                {
                                    vc += cal[i,i_]*AP.Math.Conj(cal[j,i_]);
                                }
                                hpderr = hpderr | (double)(AP.Math.AbsComplex(ca[i,j]-vc))>(double)(threshold);
                            }
                        }
                    }
                }
                else
                {
                    hpderr = true;
                }
                if( trfac.hpdmatrixcholesky(ref cau, n, true) )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            if( j<i )
                            {
                                hpderr = hpderr | cau[i,j]!=j;
                            }
                            else
                            {
                                vc = 0.0;
                                for(i_=0; i_<=i;i_++)
                                {
                                    vc += AP.Math.Conj(cau[i_,i])*cau[i_,j];
                                }
                                hpderr = hpderr | (double)(AP.Math.AbsComplex(ca[i,j]-vc))>(double)(threshold);
                            }
                        }
                    }
                }
                else
                {
                    hpderr = true;
                }
                
                //
                // Load RA (SPD matrix with low condition number),
                //      RAL and RAU - its lower and upper triangles
                //
                matgen.spdmatrixrndcond(n, 1+50*AP.Math.RandomReal(), ref ra);
                ral = new double[n, n];
                rau = new double[n, n];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        ral[i,j] = i;
                        rau[i,j] = j;
                    }
                }
                for(i=0; i<=n-1; i++)
                {
                    for(i_=0; i_<=i;i_++)
                    {
                        ral[i,i_] = ra[i,i_];
                    }
                    for(i_=i; i_<=n-1;i_++)
                    {
                        rau[i,i_] = ra[i,i_];
                    }
                }
                
                //
                // Test SPDMatrixCholesky:
                // 1. it must leave upper (lower) part unchanged
                // 2. max(A-L*L^H) must be small
                //
                if( trfac.spdmatrixcholesky(ref ral, n, false) )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            if( j>i )
                            {
                                spderr = spderr | (double)(ral[i,j])!=(double)(i);
                            }
                            else
                            {
                                vr = 0.0;
                                for(i_=0; i_<=j;i_++)
                                {
                                    vr += ral[i,i_]*ral[j,i_];
                                }
                                spderr = spderr | (double)(Math.Abs(ra[i,j]-vr))>(double)(threshold);
                            }
                        }
                    }
                }
                else
                {
                    spderr = true;
                }
                if( trfac.spdmatrixcholesky(ref rau, n, true) )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            if( j<i )
                            {
                                spderr = spderr | (double)(rau[i,j])!=(double)(j);
                            }
                            else
                            {
                                vr = 0.0;
                                for(i_=0; i_<=i;i_++)
                                {
                                    vr += rau[i_,i]*rau[i_,j];
                                }
                                spderr = spderr | (double)(Math.Abs(ra[i,j]-vr))>(double)(threshold);
                            }
                        }
                    }
                }
                else
                {
                    spderr = true;
                }
            }
            
            //
            // report
            //
            waserrors = rerr | spderr | cerr | hpderr | properr;
            if( !silent )
            {
                System.Console.Write("TESTING TRIANGULAR FACTORIZATIONS");
                System.Console.WriteLine();
                System.Console.Write("* REAL:                                  ");
                if( rerr )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* SPD:                                   ");
                if( spderr )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* COMPLEX:                               ");
                if( cerr )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* HPD:                                   ");
                if( hpderr )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* OTHER PROPERTIES:                      ");
                if( properr )
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


        private static void testcluproblem(ref AP.Complex[,] a,
            int m,
            int n,
            double threshold,
            ref bool err,
            ref bool properr)
        {
            AP.Complex[,] ca = new AP.Complex[0,0];
            AP.Complex[,] cl = new AP.Complex[0,0];
            AP.Complex[,] cu = new AP.Complex[0,0];
            AP.Complex[,] ca2 = new AP.Complex[0,0];
            AP.Complex[] ct = new AP.Complex[0];
            int i = 0;
            int j = 0;
            int minmn = 0;
            AP.Complex v = 0;
            int[] p = new int[0];
            int i_ = 0;

            minmn = Math.Min(m, n);
            
            //
            // PLU test
            //
            ca = new AP.Complex[m, n];
            for(i=0; i<=m-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    ca[i,i_] = a[i,i_];
                }
            }
            trfac.cmatrixplu(ref ca, m, n, ref p);
            for(i=0; i<=minmn-1; i++)
            {
                if( p[i]<i | p[i]>=m )
                {
                    properr = false;
                    return;
                }
            }
            cl = new AP.Complex[m, minmn];
            for(j=0; j<=minmn-1; j++)
            {
                for(i=0; i<=j-1; i++)
                {
                    cl[i,j] = 0.0;
                }
                cl[j,j] = 1.0;
                for(i=j+1; i<=m-1; i++)
                {
                    cl[i,j] = ca[i,j];
                }
            }
            cu = new AP.Complex[minmn, n];
            for(i=0; i<=minmn-1; i++)
            {
                for(j=0; j<=i-1; j++)
                {
                    cu[i,j] = 0.0;
                }
                for(j=i; j<=n-1; j++)
                {
                    cu[i,j] = ca[i,j];
                }
            }
            ca2 = new AP.Complex[m, n];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=minmn-1;i_++)
                    {
                        v += cl[i,i_]*cu[i_,j];
                    }
                    ca2[i,j] = v;
                }
            }
            ct = new AP.Complex[n];
            for(i=minmn-1; i>=0; i--)
            {
                if( i!=p[i] )
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        ct[i_] = ca2[i,i_];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        ca2[i,i_] = ca2[p[i],i_];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        ca2[p[i],i_] = ct[i_];
                    }
                }
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    err = err | (double)(AP.Math.AbsComplex(a[i,j]-ca2[i,j]))>(double)(threshold);
                }
            }
            
            //
            // LUP test
            //
            ca = new AP.Complex[m, n];
            for(i=0; i<=m-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    ca[i,i_] = a[i,i_];
                }
            }
            trfac.cmatrixlup(ref ca, m, n, ref p);
            for(i=0; i<=minmn-1; i++)
            {
                if( p[i]<i | p[i]>=n )
                {
                    properr = false;
                    return;
                }
            }
            cl = new AP.Complex[m, minmn];
            for(j=0; j<=minmn-1; j++)
            {
                for(i=0; i<=j-1; i++)
                {
                    cl[i,j] = 0.0;
                }
                for(i=j; i<=m-1; i++)
                {
                    cl[i,j] = ca[i,j];
                }
            }
            cu = new AP.Complex[minmn, n];
            for(i=0; i<=minmn-1; i++)
            {
                for(j=0; j<=i-1; j++)
                {
                    cu[i,j] = 0.0;
                }
                cu[i,i] = 1.0;
                for(j=i+1; j<=n-1; j++)
                {
                    cu[i,j] = ca[i,j];
                }
            }
            ca2 = new AP.Complex[m, n];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=minmn-1;i_++)
                    {
                        v += cl[i,i_]*cu[i_,j];
                    }
                    ca2[i,j] = v;
                }
            }
            ct = new AP.Complex[m];
            for(i=minmn-1; i>=0; i--)
            {
                if( i!=p[i] )
                {
                    for(i_=0; i_<=m-1;i_++)
                    {
                        ct[i_] = ca2[i_,i];
                    }
                    for(i_=0; i_<=m-1;i_++)
                    {
                        ca2[i_,i] = ca2[i_,p[i]];
                    }
                    for(i_=0; i_<=m-1;i_++)
                    {
                        ca2[i_,p[i]] = ct[i_];
                    }
                }
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    err = err | (double)(AP.Math.AbsComplex(a[i,j]-ca2[i,j]))>(double)(threshold);
                }
            }
        }


        private static void testrluproblem(ref double[,] a,
            int m,
            int n,
            double threshold,
            ref bool err,
            ref bool properr)
        {
            double[,] ca = new double[0,0];
            double[,] cl = new double[0,0];
            double[,] cu = new double[0,0];
            double[,] ca2 = new double[0,0];
            double[] ct = new double[0];
            int i = 0;
            int j = 0;
            int minmn = 0;
            double v = 0;
            int[] p = new int[0];
            int i_ = 0;

            minmn = Math.Min(m, n);
            
            //
            // PLU test
            //
            ca = new double[m, n];
            for(i=0; i<=m-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    ca[i,i_] = a[i,i_];
                }
            }
            trfac.rmatrixplu(ref ca, m, n, ref p);
            for(i=0; i<=minmn-1; i++)
            {
                if( p[i]<i | p[i]>=m )
                {
                    properr = false;
                    return;
                }
            }
            cl = new double[m, minmn];
            for(j=0; j<=minmn-1; j++)
            {
                for(i=0; i<=j-1; i++)
                {
                    cl[i,j] = 0.0;
                }
                cl[j,j] = 1.0;
                for(i=j+1; i<=m-1; i++)
                {
                    cl[i,j] = ca[i,j];
                }
            }
            cu = new double[minmn, n];
            for(i=0; i<=minmn-1; i++)
            {
                for(j=0; j<=i-1; j++)
                {
                    cu[i,j] = 0.0;
                }
                for(j=i; j<=n-1; j++)
                {
                    cu[i,j] = ca[i,j];
                }
            }
            ca2 = new double[m, n];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=minmn-1;i_++)
                    {
                        v += cl[i,i_]*cu[i_,j];
                    }
                    ca2[i,j] = v;
                }
            }
            ct = new double[n];
            for(i=minmn-1; i>=0; i--)
            {
                if( i!=p[i] )
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        ct[i_] = ca2[i,i_];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        ca2[i,i_] = ca2[p[i],i_];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        ca2[p[i],i_] = ct[i_];
                    }
                }
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    err = err | (double)(Math.Abs(a[i,j]-ca2[i,j]))>(double)(threshold);
                }
            }
            
            //
            // LUP test
            //
            ca = new double[m, n];
            for(i=0; i<=m-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    ca[i,i_] = a[i,i_];
                }
            }
            trfac.rmatrixlup(ref ca, m, n, ref p);
            for(i=0; i<=minmn-1; i++)
            {
                if( p[i]<i | p[i]>=n )
                {
                    properr = false;
                    return;
                }
            }
            cl = new double[m, minmn];
            for(j=0; j<=minmn-1; j++)
            {
                for(i=0; i<=j-1; i++)
                {
                    cl[i,j] = 0.0;
                }
                for(i=j; i<=m-1; i++)
                {
                    cl[i,j] = ca[i,j];
                }
            }
            cu = new double[minmn, n];
            for(i=0; i<=minmn-1; i++)
            {
                for(j=0; j<=i-1; j++)
                {
                    cu[i,j] = 0.0;
                }
                cu[i,i] = 1.0;
                for(j=i+1; j<=n-1; j++)
                {
                    cu[i,j] = ca[i,j];
                }
            }
            ca2 = new double[m, n];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=minmn-1;i_++)
                    {
                        v += cl[i,i_]*cu[i_,j];
                    }
                    ca2[i,j] = v;
                }
            }
            ct = new double[m];
            for(i=minmn-1; i>=0; i--)
            {
                if( i!=p[i] )
                {
                    for(i_=0; i_<=m-1;i_++)
                    {
                        ct[i_] = ca2[i_,i];
                    }
                    for(i_=0; i_<=m-1;i_++)
                    {
                        ca2[i_,i] = ca2[i_,p[i]];
                    }
                    for(i_=0; i_<=m-1;i_++)
                    {
                        ca2[i_,p[i]] = ct[i_];
                    }
                }
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    err = err | (double)(Math.Abs(a[i,j]-ca2[i,j]))>(double)(threshold);
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testtrfacunit_test_silent()
        {
            bool result = new bool();

            result = testtrfac(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testtrfacunit_test()
        {
            bool result = new bool();

            result = testtrfac(false);
            return result;
        }
    }
}
