
using System;

namespace alglib
{
    public class testcorrunit
    {
        /*************************************************************************
        Test
        *************************************************************************/
        public static bool testcorr(bool silent)
        {
            bool result = new bool();
            int m = 0;
            int n = 0;
            int i = 0;
            double[] ra = new double[0];
            double[] rb = new double[0];
            double[] rr1 = new double[0];
            double[] rr2 = new double[0];
            AP.Complex[] ca = new AP.Complex[0];
            AP.Complex[] cb = new AP.Complex[0];
            AP.Complex[] cr1 = new AP.Complex[0];
            AP.Complex[] cr2 = new AP.Complex[0];
            int maxn = 0;
            double referr = 0;
            double refrerr = 0;
            double inverr = 0;
            double invrerr = 0;
            double errtol = 0;
            bool referrors = new bool();
            bool refrerrors = new bool();
            bool inverrors = new bool();
            bool invrerrors = new bool();
            bool waserrors = new bool();

            maxn = 32;
            errtol = 100000*Math.Pow(maxn, (double)(3)/(double)(2))*AP.Math.MachineEpsilon;
            referrors = false;
            refrerrors = false;
            inverrors = false;
            invrerrors = false;
            waserrors = false;
            
            //
            // Test against reference O(N^2) implementation.
            //
            referr = 0;
            refrerr = 0;
            for(m=1; m<=maxn; m++)
            {
                for(n=1; n<=maxn; n++)
                {
                    
                    //
                    // Complex correlation
                    //
                    ca = new AP.Complex[m];
                    for(i=0; i<=m-1; i++)
                    {
                        ca[i].x = 2*AP.Math.RandomReal()-1;
                        ca[i].y = 2*AP.Math.RandomReal()-1;
                    }
                    cb = new AP.Complex[n];
                    for(i=0; i<=n-1; i++)
                    {
                        cb[i].x = 2*AP.Math.RandomReal()-1;
                        cb[i].y = 2*AP.Math.RandomReal()-1;
                    }
                    cr1 = new AP.Complex[1];
                    corr.corrc1d(ref ca, m, ref cb, n, ref cr1);
                    refcorrc1d(ref ca, m, ref cb, n, ref cr2);
                    for(i=0; i<=m+n-2; i++)
                    {
                        referr = Math.Max(referr, AP.Math.AbsComplex(cr1[i]-cr2[i]));
                    }
                    cr1 = new AP.Complex[1];
                    corr.corrc1dcircular(ref ca, m, ref cb, n, ref cr1);
                    refcorrc1dcircular(ref ca, m, ref cb, n, ref cr2);
                    for(i=0; i<=m-1; i++)
                    {
                        referr = Math.Max(referr, AP.Math.AbsComplex(cr1[i]-cr2[i]));
                    }
                    
                    //
                    // Real correlation
                    //
                    ra = new double[m];
                    for(i=0; i<=m-1; i++)
                    {
                        ra[i] = 2*AP.Math.RandomReal()-1;
                    }
                    rb = new double[n];
                    for(i=0; i<=n-1; i++)
                    {
                        rb[i] = 2*AP.Math.RandomReal()-1;
                    }
                    rr1 = new double[1];
                    corr.corrr1d(ref ra, m, ref rb, n, ref rr1);
                    refcorrr1d(ref ra, m, ref rb, n, ref rr2);
                    for(i=0; i<=m+n-2; i++)
                    {
                        refrerr = Math.Max(refrerr, Math.Abs(rr1[i]-rr2[i]));
                    }
                    rr1 = new double[1];
                    corr.corrr1dcircular(ref ra, m, ref rb, n, ref rr1);
                    refcorrr1dcircular(ref ra, m, ref rb, n, ref rr2);
                    for(i=0; i<=m-1; i++)
                    {
                        refrerr = Math.Max(refrerr, Math.Abs(rr1[i]-rr2[i]));
                    }
                }
            }
            referrors = referrors | (double)(referr)>(double)(errtol);
            refrerrors = refrerrors | (double)(refrerr)>(double)(errtol);
            
            //
            // end
            //
            waserrors = referrors | refrerrors;
            if( !silent )
            {
                System.Console.Write("TESTING CORRELATION");
                System.Console.WriteLine();
                System.Console.Write("FINAL RESULT:                             ");
                if( waserrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* AGAINST REFERENCE COMPLEX CORR:         ");
                if( referrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* AGAINST REFERENCE REAL CORR:            ");
                if( refrerrors )
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
        Reference implementation
        *************************************************************************/
        private static void refcorrc1d(ref AP.Complex[] signal,
            int n,
            ref AP.Complex[] pattern,
            int m,
            ref AP.Complex[] r)
        {
            int i = 0;
            int j = 0;
            AP.Complex v = 0;
            AP.Complex[] s = new AP.Complex[0];
            int i_ = 0;

            s = new AP.Complex[m+n-1];
            for(i_=0; i_<=n-1;i_++)
            {
                s[i_] = signal[i_];
            }
            for(i=n; i<=m+n-2; i++)
            {
                s[i] = 0;
            }
            r = new AP.Complex[m+n-1];
            for(i=0; i<=n-1; i++)
            {
                v = 0;
                for(j=0; j<=m-1; j++)
                {
                    if( i+j>=n )
                    {
                        break;
                    }
                    v = v+AP.Math.Conj(pattern[j])*s[i+j];
                }
                r[i] = v;
            }
            for(i=1; i<=m-1; i++)
            {
                v = 0;
                for(j=i; j<=m-1; j++)
                {
                    v = v+AP.Math.Conj(pattern[j])*s[j-i];
                }
                r[m+n-1-i] = v;
            }
        }


        /*************************************************************************
        Reference implementation
        *************************************************************************/
        private static void refcorrc1dcircular(ref AP.Complex[] signal,
            int n,
            ref AP.Complex[] pattern,
            int m,
            ref AP.Complex[] r)
        {
            int i = 0;
            int j = 0;
            AP.Complex v = 0;

            r = new AP.Complex[n];
            for(i=0; i<=n-1; i++)
            {
                v = 0;
                for(j=0; j<=m-1; j++)
                {
                    v = v+AP.Math.Conj(pattern[j])*signal[(i+j)%n];
                }
                r[i] = v;
            }
        }


        /*************************************************************************
        Reference implementation
        *************************************************************************/
        private static void refcorrr1d(ref double[] signal,
            int n,
            ref double[] pattern,
            int m,
            ref double[] r)
        {
            int i = 0;
            int j = 0;
            double v = 0;
            double[] s = new double[0];
            int i_ = 0;

            s = new double[m+n-1];
            for(i_=0; i_<=n-1;i_++)
            {
                s[i_] = signal[i_];
            }
            for(i=n; i<=m+n-2; i++)
            {
                s[i] = 0;
            }
            r = new double[m+n-1];
            for(i=0; i<=n-1; i++)
            {
                v = 0;
                for(j=0; j<=m-1; j++)
                {
                    if( i+j>=n )
                    {
                        break;
                    }
                    v = v+pattern[j]*s[i+j];
                }
                r[i] = v;
            }
            for(i=1; i<=m-1; i++)
            {
                v = 0;
                for(j=i; j<=m-1; j++)
                {
                    v = v+pattern[j]*s[-i+j];
                }
                r[m+n-1-i] = v;
            }
        }


        /*************************************************************************
        Reference implementation
        *************************************************************************/
        private static void refcorrr1dcircular(ref double[] signal,
            int n,
            ref double[] pattern,
            int m,
            ref double[] r)
        {
            int i = 0;
            int j = 0;
            double v = 0;

            r = new double[n];
            for(i=0; i<=n-1; i++)
            {
                v = 0;
                for(j=0; j<=m-1; j++)
                {
                    v = v+pattern[j]*signal[(i+j)%n];
                }
                r[i] = v;
            }
        }


        /*************************************************************************
        Reference implementation
        *************************************************************************/
        private static void refconvc1d(ref AP.Complex[] a,
            int m,
            ref AP.Complex[] b,
            int n,
            ref AP.Complex[] r)
        {
            int i = 0;
            AP.Complex v = 0;
            int i_ = 0;
            int i1_ = 0;

            r = new AP.Complex[m+n-1];
            for(i=0; i<=m+n-2; i++)
            {
                r[i] = 0;
            }
            for(i=0; i<=m-1; i++)
            {
                v = a[i];
                i1_ = (0) - (i);
                for(i_=i; i_<=i+n-1;i_++)
                {
                    r[i_] = r[i_] + v*b[i_+i1_];
                }
            }
        }


        /*************************************************************************
        Reference implementation
        *************************************************************************/
        private static void refconvc1dcircular(ref AP.Complex[] a,
            int m,
            ref AP.Complex[] b,
            int n,
            ref AP.Complex[] r)
        {
            int i1 = 0;
            int i2 = 0;
            int j2 = 0;
            AP.Complex[] buf = new AP.Complex[0];
            int i_ = 0;
            int i1_ = 0;

            refconvc1d(ref a, m, ref b, n, ref buf);
            r = new AP.Complex[m];
            for(i_=0; i_<=m-1;i_++)
            {
                r[i_] = buf[i_];
            }
            i1 = m;
            while( i1<=m+n-2 )
            {
                i2 = Math.Min(i1+m-1, m+n-2);
                j2 = i2-i1;
                i1_ = (i1) - (0);
                for(i_=0; i_<=j2;i_++)
                {
                    r[i_] = r[i_] + buf[i_+i1_];
                }
                i1 = i1+m;
            }
        }


        /*************************************************************************
        Reference FFT
        *************************************************************************/
        private static void refconvr1d(ref double[] a,
            int m,
            ref double[] b,
            int n,
            ref double[] r)
        {
            int i = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            r = new double[m+n-1];
            for(i=0; i<=m+n-2; i++)
            {
                r[i] = 0;
            }
            for(i=0; i<=m-1; i++)
            {
                v = a[i];
                i1_ = (0) - (i);
                for(i_=i; i_<=i+n-1;i_++)
                {
                    r[i_] = r[i_] + v*b[i_+i1_];
                }
            }
        }


        /*************************************************************************
        Reference implementation
        *************************************************************************/
        private static void refconvr1dcircular(ref double[] a,
            int m,
            ref double[] b,
            int n,
            ref double[] r)
        {
            int i1 = 0;
            int i2 = 0;
            int j2 = 0;
            double[] buf = new double[0];
            int i_ = 0;
            int i1_ = 0;

            refconvr1d(ref a, m, ref b, n, ref buf);
            r = new double[m];
            for(i_=0; i_<=m-1;i_++)
            {
                r[i_] = buf[i_];
            }
            i1 = m;
            while( i1<=m+n-2 )
            {
                i2 = Math.Min(i1+m-1, m+n-2);
                j2 = i2-i1;
                i1_ = (i1) - (0);
                for(i_=0; i_<=j2;i_++)
                {
                    r[i_] = r[i_] + buf[i_+i1_];
                }
                i1 = i1+m;
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testcorrunit_test_silent()
        {
            bool result = new bool();

            result = testcorr(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testcorrunit_test()
        {
            bool result = new bool();

            result = testcorr(false);
            return result;
        }
    }
}
