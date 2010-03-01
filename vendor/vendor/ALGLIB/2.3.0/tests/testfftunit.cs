
using System;

namespace alglib
{
    public class testfftunit
    {
        /*************************************************************************
        Test
        *************************************************************************/
        public static bool testfft(bool silent)
        {
            bool result = new bool();
            int n = 0;
            int i = 0;
            int k = 0;
            AP.Complex[] a1 = new AP.Complex[0];
            AP.Complex[] a2 = new AP.Complex[0];
            AP.Complex[] a3 = new AP.Complex[0];
            double[] r1 = new double[0];
            double[] r2 = new double[0];
            double[] buf = new double[0];
            ftbase.ftplan plan = new ftbase.ftplan();
            int maxn = 0;
            double bidierr = 0;
            double bidirerr = 0;
            double referr = 0;
            double refrerr = 0;
            double reinterr = 0;
            double errtol = 0;
            bool referrors = new bool();
            bool bidierrors = new bool();
            bool refrerrors = new bool();
            bool bidirerrors = new bool();
            bool reinterrors = new bool();
            bool waserrors = new bool();
            int i_ = 0;

            maxn = 128;
            errtol = 100000*Math.Pow(maxn, (double)(3)/(double)(2))*AP.Math.MachineEpsilon;
            bidierrors = false;
            referrors = false;
            bidirerrors = false;
            refrerrors = false;
            reinterrors = false;
            waserrors = false;
            
            //
            // Test bi-directional error: norm(x-invFFT(FFT(x)))
            //
            bidierr = 0;
            bidirerr = 0;
            for(n=1; n<=maxn; n++)
            {
                
                //
                // Complex FFT/invFFT
                //
                a1 = new AP.Complex[n];
                a2 = new AP.Complex[n];
                a3 = new AP.Complex[n];
                for(i=0; i<=n-1; i++)
                {
                    a1[i].x = 2*AP.Math.RandomReal()-1;
                    a1[i].y = 2*AP.Math.RandomReal()-1;
                    a2[i] = a1[i];
                    a3[i] = a1[i];
                }
                fft.fftc1d(ref a2, n);
                fft.fftc1dinv(ref a2, n);
                fft.fftc1dinv(ref a3, n);
                fft.fftc1d(ref a3, n);
                for(i=0; i<=n-1; i++)
                {
                    bidierr = Math.Max(bidierr, AP.Math.AbsComplex(a1[i]-a2[i]));
                    bidierr = Math.Max(bidierr, AP.Math.AbsComplex(a1[i]-a3[i]));
                }
                
                //
                // Real
                //
                r1 = new double[n];
                r2 = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    r1[i] = 2*AP.Math.RandomReal()-1;
                    r2[i] = r1[i];
                }
                fft.fftr1d(ref r2, n, ref a1);
                for(i_=0; i_<=n-1;i_++)
                {
                    r2[i_] = 0*r2[i_];
                }
                fft.fftr1dinv(ref a1, n, ref r2);
                for(i=0; i<=n-1; i++)
                {
                    bidirerr = Math.Max(bidirerr, AP.Math.AbsComplex(r1[i]-r2[i]));
                }
            }
            bidierrors = bidierrors | (double)(bidierr)>(double)(errtol);
            bidirerrors = bidirerrors | (double)(bidirerr)>(double)(errtol);
            
            //
            // Test against reference O(N^2) implementation
            //
            referr = 0;
            refrerr = 0;
            for(n=1; n<=maxn; n++)
            {
                
                //
                // Complex FFT
                //
                a1 = new AP.Complex[n];
                a2 = new AP.Complex[n];
                for(i=0; i<=n-1; i++)
                {
                    a1[i].x = 2*AP.Math.RandomReal()-1;
                    a1[i].y = 2*AP.Math.RandomReal()-1;
                    a2[i] = a1[i];
                }
                fft.fftc1d(ref a1, n);
                reffftc1d(ref a2, n);
                for(i=0; i<=n-1; i++)
                {
                    referr = Math.Max(referr, AP.Math.AbsComplex(a1[i]-a2[i]));
                }
                
                //
                // Complex inverse FFT
                //
                a1 = new AP.Complex[n];
                a2 = new AP.Complex[n];
                for(i=0; i<=n-1; i++)
                {
                    a1[i].x = 2*AP.Math.RandomReal()-1;
                    a1[i].y = 2*AP.Math.RandomReal()-1;
                    a2[i] = a1[i];
                }
                fft.fftc1dinv(ref a1, n);
                reffftc1dinv(ref a2, n);
                for(i=0; i<=n-1; i++)
                {
                    referr = Math.Max(referr, AP.Math.AbsComplex(a1[i]-a2[i]));
                }
                
                //
                // Real forward/inverse FFT:
                // * calculate and check forward FFT
                // * use precalculated FFT to check backward FFT
                //   fill unused parts of frequencies array with random numbers
                //   to ensure that they are not really used
                //
                r1 = new double[n];
                r2 = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    r1[i] = 2*AP.Math.RandomReal()-1;
                    r2[i] = r1[i];
                }
                fft.fftr1d(ref r1, n, ref a1);
                refinternalrfft(ref r2, n, ref a2);
                for(i=0; i<=n-1; i++)
                {
                    refrerr = Math.Max(refrerr, AP.Math.AbsComplex(a1[i]-a2[i]));
                }
                a3 = new AP.Complex[(int)Math.Floor((double)(n)/(double)(2))+1];
                for(i=0; i<=(int)Math.Floor((double)(n)/(double)(2)); i++)
                {
                    a3[i] = a2[i];
                }
                a3[0].y = 2*AP.Math.RandomReal()-1;
                if( n%2==0 )
                {
                    a3[(int)Math.Floor((double)(n)/(double)(2))].y = 2*AP.Math.RandomReal()-1;
                }
                for(i=0; i<=n-1; i++)
                {
                    r1[i] = 0;
                }
                fft.fftr1dinv(ref a3, n, ref r1);
                for(i=0; i<=n-1; i++)
                {
                    refrerr = Math.Max(refrerr, Math.Abs(r2[i]-r1[i]));
                }
            }
            referrors = referrors | (double)(referr)>(double)(errtol);
            refrerrors = refrerrors | (double)(refrerr)>(double)(errtol);
            
            //
            // test internal real even FFT
            //
            reinterr = 0;
            for(k=1; k<=maxn/2; k++)
            {
                n = 2*k;
                
                //
                // Real forward FFT
                //
                r1 = new double[n];
                r2 = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    r1[i] = 2*AP.Math.RandomReal()-1;
                    r2[i] = r1[i];
                }
                ftbase.ftbasegeneratecomplexfftplan(n/2, ref plan);
                buf = new double[n];
                fft.fftr1dinternaleven(ref r1, n, ref buf, ref plan);
                refinternalrfft(ref r2, n, ref a2);
                reinterr = Math.Max(reinterr, Math.Abs(r1[0]-a2[0].x));
                reinterr = Math.Max(reinterr, Math.Abs(r1[1]-a2[n/2].x));
                for(i=1; i<=n/2-1; i++)
                {
                    reinterr = Math.Max(reinterr, Math.Abs(r1[2*i+0]-a2[i].x));
                    reinterr = Math.Max(reinterr, Math.Abs(r1[2*i+1]-a2[i].y));
                }
                
                //
                // Real backward FFT
                //
                r1 = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    r1[i] = 2*AP.Math.RandomReal()-1;
                }
                a2 = new AP.Complex[(int)Math.Floor((double)(n)/(double)(2))+1];
                a2[0] = r1[0];
                for(i=1; i<=(int)Math.Floor((double)(n)/(double)(2))-1; i++)
                {
                    a2[i].x = r1[2*i+0];
                    a2[i].y = r1[2*i+1];
                }
                a2[(int)Math.Floor((double)(n)/(double)(2))] = r1[1];
                ftbase.ftbasegeneratecomplexfftplan(n/2, ref plan);
                buf = new double[n];
                fft.fftr1dinvinternaleven(ref r1, n, ref buf, ref plan);
                fft.fftr1dinv(ref a2, n, ref r2);
                for(i=0; i<=n-1; i++)
                {
                    reinterr = Math.Max(reinterr, Math.Abs(r1[i]-r2[i]));
                }
            }
            reinterrors = reinterrors | (double)(reinterr)>(double)(errtol);
            
            //
            // end
            //
            waserrors = bidierrors | bidirerrors | referrors | refrerrors | reinterrors;
            if( !silent )
            {
                System.Console.Write("TESTING FFT");
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
                System.Console.Write("* BI-DIRECTIONAL COMPLEX TEST:            ");
                if( bidierrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* AGAINST REFERENCE COMPLEX FFT:          ");
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
                System.Console.Write("* BI-DIRECTIONAL REAL TEST:               ");
                if( bidirerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* AGAINST REFERENCE REAL FFT:             ");
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
                System.Console.Write("* INTERNAL EVEN FFT:                      ");
                if( reinterrors )
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
        Reference FFT
        *************************************************************************/
        private static void reffftc1d(ref AP.Complex[] a,
            int n)
        {
            double[] buf = new double[0];
            int i = 0;

            System.Diagnostics.Debug.Assert(n>0, "FFTC1D: incorrect N!");
            buf = new double[2*n];
            for(i=0; i<=n-1; i++)
            {
                buf[2*i+0] = a[i].x;
                buf[2*i+1] = a[i].y;
            }
            refinternalcfft(ref buf, n, false);
            for(i=0; i<=n-1; i++)
            {
                a[i].x = buf[2*i+0];
                a[i].y = buf[2*i+1];
            }
        }


        /*************************************************************************
        Reference inverse FFT
        *************************************************************************/
        private static void reffftc1dinv(ref AP.Complex[] a,
            int n)
        {
            double[] buf = new double[0];
            int i = 0;

            System.Diagnostics.Debug.Assert(n>0, "FFTC1DInv: incorrect N!");
            buf = new double[2*n];
            for(i=0; i<=n-1; i++)
            {
                buf[2*i+0] = a[i].x;
                buf[2*i+1] = a[i].y;
            }
            refinternalcfft(ref buf, n, true);
            for(i=0; i<=n-1; i++)
            {
                a[i].x = buf[2*i+0];
                a[i].y = buf[2*i+1];
            }
        }


        /*************************************************************************
        Internal complex FFT stub.
        Uses straightforward formula with O(N^2) complexity.
        *************************************************************************/
        private static void refinternalcfft(ref double[] a,
            int nn,
            bool inversefft)
        {
            double[] tmp = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            double hre = 0;
            double him = 0;
            double c = 0;
            double s = 0;
            double re = 0;
            double im = 0;

            tmp = new double[2*nn-1+1];
            if( !inversefft )
            {
                for(i=0; i<=nn-1; i++)
                {
                    hre = 0;
                    him = 0;
                    for(k=0; k<=nn-1; k++)
                    {
                        re = a[2*k];
                        im = a[2*k+1];
                        c = Math.Cos(-(2*Math.PI*k*i/nn));
                        s = Math.Sin(-(2*Math.PI*k*i/nn));
                        hre = hre+c*re-s*im;
                        him = him+c*im+s*re;
                    }
                    tmp[2*i] = hre;
                    tmp[2*i+1] = him;
                }
                for(i=0; i<=2*nn-1; i++)
                {
                    a[i] = tmp[i];
                }
            }
            else
            {
                for(k=0; k<=nn-1; k++)
                {
                    hre = 0;
                    him = 0;
                    for(i=0; i<=nn-1; i++)
                    {
                        re = a[2*i];
                        im = a[2*i+1];
                        c = Math.Cos(2*Math.PI*k*i/nn);
                        s = Math.Sin(2*Math.PI*k*i/nn);
                        hre = hre+c*re-s*im;
                        him = him+c*im+s*re;
                    }
                    tmp[2*k] = hre/nn;
                    tmp[2*k+1] = him/nn;
                }
                for(i=0; i<=2*nn-1; i++)
                {
                    a[i] = tmp[i];
                }
            }
        }


        /*************************************************************************
        Internal real FFT stub.
        Uses straightforward formula with O(N^2) complexity.
        *************************************************************************/
        private static void refinternalrfft(ref double[] a,
            int nn,
            ref AP.Complex[] f)
        {
            double[] tmp = new double[0];
            int i = 0;

            tmp = new double[2*nn-1+1];
            for(i=0; i<=nn-1; i++)
            {
                tmp[2*i] = a[i];
                tmp[2*i+1] = 0;
            }
            refinternalcfft(ref tmp, nn, false);
            f = new AP.Complex[nn];
            for(i=0; i<=nn-1; i++)
            {
                f[i].x = tmp[2*i+0];
                f[i].y = tmp[2*i+1];
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testfftunit_test_silent()
        {
            bool result = new bool();

            result = testfft(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testfftunit_test()
        {
            bool result = new bool();

            result = testfft(false);
            return result;
        }
    }
}
