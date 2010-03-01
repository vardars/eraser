
using System;

namespace alglib
{
    public class testsblasunit
    {
        public static bool testsblas(bool silent)
        {
            bool result = new bool();
            double[,] a = new double[0,0];
            double[,] ua = new double[0,0];
            double[,] la = new double[0,0];
            double[] x = new double[0];
            double[] y1 = new double[0];
            double[] y2 = new double[0];
            double[] y3 = new double[0];
            int n = 0;
            int maxn = 0;
            int i = 0;
            int j = 0;
            int i1 = 0;
            int i2 = 0;
            int gpass = 0;
            bool waserrors = new bool();
            double mverr = 0;
            double threshold = 0;
            double alpha = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            mverr = 0;
            waserrors = false;
            maxn = 10;
            threshold = 1000*AP.Math.MachineEpsilon;
            
            //
            // Test MV
            //
            for(n=2; n<=maxn; n++)
            {
                a = new double[n+1, n+1];
                ua = new double[n+1, n+1];
                la = new double[n+1, n+1];
                x = new double[n+1];
                y1 = new double[n+1];
                y2 = new double[n+1];
                y3 = new double[n+1];
                
                //
                // fill A, UA, LA
                //
                for(i=1; i<=n; i++)
                {
                    a[i,i] = 2*AP.Math.RandomReal()-1;
                    for(j=i+1; j<=n; j++)
                    {
                        a[i,j] = 2*AP.Math.RandomReal()-1;
                        a[j,i] = a[i,j];
                    }
                }
                for(i=1; i<=n; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        ua[i,j] = 0;
                    }
                }
                for(i=1; i<=n; i++)
                {
                    for(j=i; j<=n; j++)
                    {
                        ua[i,j] = a[i,j];
                    }
                }
                for(i=1; i<=n; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        la[i,j] = 0;
                    }
                }
                for(i=1; i<=n; i++)
                {
                    for(j=1; j<=i; j++)
                    {
                        la[i,j] = a[i,j];
                    }
                }
                
                //
                // test on different I1, I2
                //
                for(i1=1; i1<=n; i1++)
                {
                    for(i2=i1; i2<=n; i2++)
                    {
                        
                        //
                        // Fill X, choose Alpha
                        //
                        for(i=1; i<=i2-i1+1; i++)
                        {
                            x[i] = 2*AP.Math.RandomReal()-1;
                        }
                        alpha = 2*AP.Math.RandomReal()-1;
                        
                        //
                        // calculate A*x, UA*x, LA*x
                        //
                        for(i=i1; i<=i2; i++)
                        {
                            i1_ = (1)-(i1);
                            v = 0.0;
                            for(i_=i1; i_<=i2;i_++)
                            {
                                v += a[i,i_]*x[i_+i1_];
                            }
                            y1[i-i1+1] = alpha*v;
                        }
                        sblas.symmetricmatrixvectormultiply(ref ua, true, i1, i2, ref x, alpha, ref y2);
                        sblas.symmetricmatrixvectormultiply(ref la, false, i1, i2, ref x, alpha, ref y3);
                        
                        //
                        // Calculate error
                        //
                        for(i_=1; i_<=i2-i1+1;i_++)
                        {
                            y2[i_] = y2[i_] - y1[i_];
                        }
                        v = 0.0;
                        for(i_=1; i_<=i2-i1+1;i_++)
                        {
                            v += y2[i_]*y2[i_];
                        }
                        mverr = Math.Max(mverr, Math.Sqrt(v));
                        for(i_=1; i_<=i2-i1+1;i_++)
                        {
                            y3[i_] = y3[i_] - y1[i_];
                        }
                        v = 0.0;
                        for(i_=1; i_<=i2-i1+1;i_++)
                        {
                            v += y3[i_]*y3[i_];
                        }
                        mverr = Math.Max(mverr, Math.Sqrt(v));
                    }
                }
            }
            
            //
            // report
            //
            waserrors = (double)(mverr)>(double)(threshold);
            if( !silent )
            {
                System.Console.Write("TESTING SYMMETRIC BLAS");
                System.Console.WriteLine();
                System.Console.Write("MV error:                                ");
                System.Console.Write("{0,5:E3}",mverr);
                System.Console.WriteLine();
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
        Silent unit test
        *************************************************************************/
        public static bool testsblasunit_test_silent()
        {
            bool result = new bool();

            result = testsblas(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testsblasunit_test()
        {
            bool result = new bool();

            result = testsblas(false);
            return result;
        }
    }
}
