
using System;

namespace alglib
{
    public class testxblasunit
    {
        public const double xchunk = 1048576;
        public const int xchunkcount = 4;


        public static bool testxblas(bool silent)
        {
            bool result = new bool();
            bool approxerrors = new bool();
            bool exactnesserrors = new bool();
            bool waserrors = new bool();
            double approxthreshold = 0;
            int maxn = 0;
            int passcount = 0;
            int n = 0;
            int i = 0;
            int pass = 0;
            double rv1 = 0;
            double rv2 = 0;
            double rv2err = 0;
            AP.Complex cv1 = 0;
            AP.Complex cv2 = 0;
            double cv2err = 0;
            double cv2errx = 0;
            double cv2erry = 0;
            double[] rx = new double[0];
            double[] ry = new double[0];
            AP.Complex[] cx = new AP.Complex[0];
            AP.Complex[] cy = new AP.Complex[0];
            double[] temp = new double[0];
            double b = 0;
            double s = 0;
            int i_ = 0;

            approxerrors = false;
            exactnesserrors = false;
            waserrors = false;
            approxthreshold = 1000*AP.Math.MachineEpsilon;
            maxn = 1000;
            passcount = 10;
            
            //
            // tests:
            // 1. ability to calculate dot product
            // 2. higher precision
            //
            for(n=1; n<=maxn; n++)
            {
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    //  ability to approximately calculate real dot product
                    //
                    rx = new double[n];
                    ry = new double[n];
                    temp = new double[n];
                    for(i=0; i<=n-1; i++)
                    {
                        if( (double)(AP.Math.RandomReal())>(double)(0.2) )
                        {
                            rx[i] = 2*AP.Math.RandomReal()-1;
                        }
                        else
                        {
                            rx[i] = 0;
                        }
                        if( (double)(AP.Math.RandomReal())>(double)(0.2) )
                        {
                            ry[i] = 2*AP.Math.RandomReal()-1;
                        }
                        else
                        {
                            ry[i] = 0;
                        }
                    }
                    rv1 = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        rv1 += rx[i_]*ry[i_];
                    }
                    xblas.xdot(ref rx, ref ry, n, ref temp, ref rv2, ref rv2err);
                    approxerrors = approxerrors | (double)(Math.Abs(rv1-rv2))>(double)(approxthreshold);
                    
                    //
                    //  ability to approximately calculate complex dot product
                    //
                    cx = new AP.Complex[n];
                    cy = new AP.Complex[n];
                    temp = new double[2*n];
                    for(i=0; i<=n-1; i++)
                    {
                        if( (double)(AP.Math.RandomReal())>(double)(0.2) )
                        {
                            cx[i].x = 2*AP.Math.RandomReal()-1;
                            cx[i].y = 2*AP.Math.RandomReal()-1;
                        }
                        else
                        {
                            cx[i] = 0;
                        }
                        if( (double)(AP.Math.RandomReal())>(double)(0.2) )
                        {
                            cy[i].x = 2*AP.Math.RandomReal()-1;
                            cy[i].y = 2*AP.Math.RandomReal()-1;
                        }
                        else
                        {
                            cy[i] = 0;
                        }
                    }
                    cv1 = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        cv1 += cx[i_]*cy[i_];
                    }
                    xblas.xcdot(ref cx, ref cy, n, ref temp, ref cv2, ref cv2err);
                    approxerrors = approxerrors | (double)(AP.Math.AbsComplex(cv1-cv2))>(double)(approxthreshold);
                }
            }
            
            //
            // test of precision: real
            //
            n = 50000;
            rx = new double[n];
            ry = new double[n];
            temp = new double[n];
            for(pass=0; pass<=passcount-1; pass++)
            {
                System.Diagnostics.Debug.Assert(n%2==0);
                
                //
                // First test: X + X + ... + X - X - X - ... - X = 1*X
                //
                s = Math.Exp(Math.Max(pass, 50));
                if( pass==passcount-1 & pass>1 )
                {
                    s = AP.Math.MaxRealNumber;
                }
                ry[0] = (2*AP.Math.RandomReal()-1)*s*Math.Sqrt(2*AP.Math.RandomReal());
                for(i=1; i<=n-1; i++)
                {
                    ry[i] = ry[0];
                }
                for(i=0; i<=n/2-1; i++)
                {
                    rx[i] = 1;
                }
                for(i=n/2; i<=n-2; i++)
                {
                    rx[i] = -1;
                }
                rx[n-1] = 0;
                xblas.xdot(ref rx, ref ry, n, ref temp, ref rv2, ref rv2err);
                exactnesserrors = exactnesserrors | (double)(rv2err)<(double)(0);
                exactnesserrors = exactnesserrors | (double)(rv2err)>(double)(4*AP.Math.MachineEpsilon*Math.Abs(ry[0]));
                exactnesserrors = exactnesserrors | (double)(Math.Abs(rv2-ry[0]))>(double)(rv2err);
                
                //
                // First test: X + X + ... + X = N*X
                //
                s = Math.Exp(Math.Max(pass, 50));
                if( pass==passcount-1 & pass>1 )
                {
                    s = AP.Math.MaxRealNumber;
                }
                ry[0] = (2*AP.Math.RandomReal()-1)*s*Math.Sqrt(2*AP.Math.RandomReal());
                for(i=1; i<=n-1; i++)
                {
                    ry[i] = ry[0];
                }
                for(i=0; i<=n-1; i++)
                {
                    rx[i] = 1;
                }
                xblas.xdot(ref rx, ref ry, n, ref temp, ref rv2, ref rv2err);
                exactnesserrors = exactnesserrors | (double)(rv2err)<(double)(0);
                exactnesserrors = exactnesserrors | (double)(rv2err)>(double)(4*AP.Math.MachineEpsilon*Math.Abs(ry[0])*n);
                exactnesserrors = exactnesserrors | (double)(Math.Abs(rv2-n*ry[0]))>(double)(rv2err);
            }
            
            //
            // test of precision: complex
            //
            n = 50000;
            cx = new AP.Complex[n];
            cy = new AP.Complex[n];
            temp = new double[2*n];
            for(pass=0; pass<=passcount-1; pass++)
            {
                System.Diagnostics.Debug.Assert(n%2==0);
                
                //
                // First test: X + X + ... + X - X - X - ... - X = 1*X
                //
                s = Math.Exp(Math.Max(pass, 50));
                if( pass==passcount-1 & pass>1 )
                {
                    s = AP.Math.MaxRealNumber;
                }
                cy[0].x = (2*AP.Math.RandomReal()-1)*s*Math.Sqrt(2*AP.Math.RandomReal());
                cy[0].y = (2*AP.Math.RandomReal()-1)*s*Math.Sqrt(2*AP.Math.RandomReal());
                for(i=1; i<=n-1; i++)
                {
                    cy[i] = cy[0];
                }
                for(i=0; i<=n/2-1; i++)
                {
                    cx[i] = 1;
                }
                for(i=n/2; i<=n-2; i++)
                {
                    cx[i] = -1;
                }
                cx[n-1] = 0;
                xblas.xcdot(ref cx, ref cy, n, ref temp, ref cv2, ref cv2err);
                exactnesserrors = exactnesserrors | (double)(cv2err)<(double)(0);
                exactnesserrors = exactnesserrors | (double)(cv2err)>(double)(4*AP.Math.MachineEpsilon*AP.Math.AbsComplex(cy[0]));
                exactnesserrors = exactnesserrors | (double)(AP.Math.AbsComplex(cv2-cy[0]))>(double)(cv2err);
                
                //
                // First test: X + X + ... + X = N*X
                //
                s = Math.Exp(Math.Max(pass, 50));
                if( pass==passcount-1 & pass>1 )
                {
                    s = AP.Math.MaxRealNumber;
                }
                cy[0] = (2*AP.Math.RandomReal()-1)*s*Math.Sqrt(2*AP.Math.RandomReal());
                for(i=1; i<=n-1; i++)
                {
                    cy[i] = cy[0];
                }
                for(i=0; i<=n-1; i++)
                {
                    cx[i] = 1;
                }
                xblas.xcdot(ref cx, ref cy, n, ref temp, ref cv2, ref cv2err);
                exactnesserrors = exactnesserrors | (double)(cv2err)<(double)(0);
                exactnesserrors = exactnesserrors | (double)(cv2err)>(double)(4*AP.Math.MachineEpsilon*AP.Math.AbsComplex(cy[0])*n);
                exactnesserrors = exactnesserrors | (double)(AP.Math.AbsComplex(cv2-n*cy[0]))>(double)(cv2err);
            }
            
            //
            // report
            //
            waserrors = approxerrors | exactnesserrors;
            if( !silent )
            {
                System.Console.Write("TESTING XBLAS");
                System.Console.WriteLine();
                System.Console.Write("APPROX.TESTS:                            ");
                if( approxerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("EXACT TESTS:                             ");
                if( exactnesserrors )
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
        Silent unit test
        *************************************************************************/
        public static bool testxblasunit_test_silent()
        {
            bool result = new bool();

            result = testxblas(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testxblasunit_test()
        {
            bool result = new bool();

            result = testxblas(false);
            return result;
        }
    }
}
