
using System;

namespace alglib
{
    public class testconvunit
    {
        /*************************************************************************
        Test
        *************************************************************************/
        public static bool testconv(bool silent)
        {
            bool result = new bool();
            int m = 0;
            int n = 0;
            int i = 0;
            int rkind = 0;
            int circkind = 0;
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
            // Automatic ConvC1D() and different algorithms of ConvC1DX() are tested.
            //
            referr = 0;
            refrerr = 0;
            for(m=1; m<=maxn; m++)
            {
                for(n=1; n<=maxn; n++)
                {
                    for(circkind=0; circkind<=1; circkind++)
                    {
                        for(rkind=-3; rkind<=1; rkind++)
                        {
                            
                            //
                            // skip impossible combinations of parameters:
                            // * circular convolution, M<N, RKind<>-3 - internal subroutine does not support M<N.
                            //
                            if( circkind!=0 & m<n & rkind!=-3 )
                            {
                                continue;
                            }
                            
                            //
                            // Complex convolution
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
                            if( rkind==-3 )
                            {
                                
                                //
                                // test wrapper subroutine:
                                // * circular/non-circular
                                //
                                if( circkind==0 )
                                {
                                    conv.convc1d(ref ca, m, ref cb, n, ref cr1);
                                }
                                else
                                {
                                    conv.convc1dcircular(ref ca, m, ref cb, n, ref cr1);
                                }
                            }
                            else
                            {
                                
                                //
                                // test internal subroutine
                                //
                                if( m>=n )
                                {
                                    
                                    //
                                    // test internal subroutine:
                                    // * circular/non-circular mode
                                    //
                                    conv.convc1dx(ref ca, m, ref cb, n, circkind!=0, rkind, 0, ref cr1);
                                }
                                else
                                {
                                    
                                    //
                                    // test internal subroutine - circular mode only
                                    //
                                    System.Diagnostics.Debug.Assert(circkind==0, "Convolution test: internal error!");
                                    conv.convc1dx(ref cb, n, ref ca, m, false, rkind, 0, ref cr1);
                                }
                            }
                            if( circkind==0 )
                            {
                                refconvc1d(ref ca, m, ref cb, n, ref cr2);
                            }
                            else
                            {
                                refconvc1dcircular(ref ca, m, ref cb, n, ref cr2);
                            }
                            if( circkind==0 )
                            {
                                for(i=0; i<=m+n-2; i++)
                                {
                                    referr = Math.Max(referr, AP.Math.AbsComplex(cr1[i]-cr2[i]));
                                }
                            }
                            else
                            {
                                for(i=0; i<=m-1; i++)
                                {
                                    referr = Math.Max(referr, AP.Math.AbsComplex(cr1[i]-cr2[i]));
                                }
                            }
                            
                            //
                            // Real convolution
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
                            if( rkind==-3 )
                            {
                                
                                //
                                // test wrapper subroutine:
                                // * circular/non-circular
                                //
                                if( circkind==0 )
                                {
                                    conv.convr1d(ref ra, m, ref rb, n, ref rr1);
                                }
                                else
                                {
                                    conv.convr1dcircular(ref ra, m, ref rb, n, ref rr1);
                                }
                            }
                            else
                            {
                                if( m>=n )
                                {
                                    
                                    //
                                    // test internal subroutine:
                                    // * circular/non-circular mode
                                    //
                                    conv.convr1dx(ref ra, m, ref rb, n, circkind!=0, rkind, 0, ref rr1);
                                }
                                else
                                {
                                    
                                    //
                                    // test internal subroutine - non-circular mode only
                                    //
                                    conv.convr1dx(ref rb, n, ref ra, m, circkind!=0, rkind, 0, ref rr1);
                                }
                            }
                            if( circkind==0 )
                            {
                                refconvr1d(ref ra, m, ref rb, n, ref rr2);
                            }
                            else
                            {
                                refconvr1dcircular(ref ra, m, ref rb, n, ref rr2);
                            }
                            if( circkind==0 )
                            {
                                for(i=0; i<=m+n-2; i++)
                                {
                                    refrerr = Math.Max(refrerr, Math.Abs(rr1[i]-rr2[i]));
                                }
                            }
                            else
                            {
                                for(i=0; i<=m-1; i++)
                                {
                                    refrerr = Math.Max(refrerr, Math.Abs(rr1[i]-rr2[i]));
                                }
                            }
                        }
                    }
                }
            }
            referrors = referrors | (double)(referr)>(double)(errtol);
            refrerrors = refrerrors | (double)(refrerr)>(double)(errtol);
            
            //
            // Test inverse convolution
            //
            inverr = 0;
            invrerr = 0;
            for(m=1; m<=maxn; m++)
            {
                for(n=1; n<=maxn; n++)
                {
                    
                    //
                    // Complex circilar and non-circular
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
                    cr2 = new AP.Complex[1];
                    conv.convc1d(ref ca, m, ref cb, n, ref cr2);
                    conv.convc1dinv(ref cr2, m+n-1, ref cb, n, ref cr1);
                    for(i=0; i<=m-1; i++)
                    {
                        inverr = Math.Max(inverr, AP.Math.AbsComplex(cr1[i]-ca[i]));
                    }
                    cr1 = new AP.Complex[1];
                    cr2 = new AP.Complex[1];
                    conv.convc1dcircular(ref ca, m, ref cb, n, ref cr2);
                    conv.convc1dcircularinv(ref cr2, m, ref cb, n, ref cr1);
                    for(i=0; i<=m-1; i++)
                    {
                        inverr = Math.Max(inverr, AP.Math.AbsComplex(cr1[i]-ca[i]));
                    }
                    
                    //
                    // Real circilar and non-circular
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
                    rr2 = new double[1];
                    conv.convr1d(ref ra, m, ref rb, n, ref rr2);
                    conv.convr1dinv(ref rr2, m+n-1, ref rb, n, ref rr1);
                    for(i=0; i<=m-1; i++)
                    {
                        invrerr = Math.Max(invrerr, Math.Abs(rr1[i]-ra[i]));
                    }
                    rr1 = new double[1];
                    rr2 = new double[1];
                    conv.convr1dcircular(ref ra, m, ref rb, n, ref rr2);
                    conv.convr1dcircularinv(ref rr2, m, ref rb, n, ref rr1);
                    for(i=0; i<=m-1; i++)
                    {
                        invrerr = Math.Max(invrerr, Math.Abs(rr1[i]-ra[i]));
                    }
                }
            }
            inverrors = inverrors | (double)(inverr)>(double)(errtol);
            invrerrors = invrerrors | (double)(invrerr)>(double)(errtol);
            
            //
            // end
            //
            waserrors = referrors | refrerrors | inverrors | invrerrors;
            if( !silent )
            {
                System.Console.Write("TESTING CONVOLUTION");
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
                System.Console.Write("* AGAINST REFERENCE COMPLEX CONV:         ");
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
                System.Console.Write("* AGAINST REFERENCE REAL CONV:            ");
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
                System.Console.Write("* COMPLEX INVERSE:                        ");
                if( inverrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* REAL INVERSE:                           ");
                if( invrerrors )
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
        public static bool testconvunit_test_silent()
        {
            bool result = new bool();

            result = testconv(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testconvunit_test()
        {
            bool result = new bool();

            result = testconv(false);
            return result;
        }
    }
}
