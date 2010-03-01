
using System;

namespace alglib
{
    public class testinterpolation2dunit
    {
        public static bool test2dinterpolation(bool silent)
        {
            bool result = new bool();
            bool waserrors = new bool();
            bool blerrors = new bool();
            bool bcerrors = new bool();
            bool dserrors = new bool();
            bool cperrors = new bool();
            bool uperrors = new bool();
            bool lterrors = new bool();
            bool syerrors = new bool();
            bool rlerrors = new bool();
            bool rcerrors = new bool();
            int pass = 0;
            int passcount = 0;
            int jobtype = 0;
            double lstep = 0;
            double h = 0;
            double[] x = new double[0];
            double[] y = new double[0];
            spline2d.spline2dinterpolant c = new spline2d.spline2dinterpolant();
            spline2d.spline2dinterpolant c2 = new spline2d.spline2dinterpolant();
            double[] lx = new double[0];
            double[] ly = new double[0];
            double[,] f = new double[0,0];
            double[,] fr = new double[0,0];
            double[,] ft = new double[0,0];
            double ax = 0;
            double ay = 0;
            double bx = 0;
            double by = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int n = 0;
            int m = 0;
            int n2 = 0;
            int m2 = 0;
            double err = 0;
            double t = 0;
            double t1 = 0;
            double t2 = 0;
            double l1 = 0;
            double l1x = 0;
            double l1y = 0;
            double l1xy = 0;
            double l2 = 0;
            double l2x = 0;
            double l2y = 0;
            double l2xy = 0;
            double fm = 0;
            double f1 = 0;
            double f2 = 0;
            double f3 = 0;
            double f4 = 0;
            double v1 = 0;
            double v1x = 0;
            double v1y = 0;
            double v1xy = 0;
            double v2 = 0;
            double v2x = 0;
            double v2y = 0;
            double v2xy = 0;
            double mf = 0;
            double[] ra = new double[0];
            double[] ra2 = new double[0];
            int ralen = 0;
            int i_ = 0;

            waserrors = false;
            passcount = 10;
            h = 0.00001;
            lstep = 0.001;
            blerrors = false;
            bcerrors = false;
            dserrors = false;
            cperrors = false;
            uperrors = false;
            lterrors = false;
            syerrors = false;
            rlerrors = false;
            rcerrors = false;
            
            //
            // Test: bilinear, bicubic
            //
            for(n=2; n<=7; n++)
            {
                for(m=2; m<=7; m++)
                {
                    x = new double[n-1+1];
                    y = new double[m-1+1];
                    lx = new double[2*n-2+1];
                    ly = new double[2*m-2+1];
                    f = new double[m-1+1, n-1+1];
                    ft = new double[n-1+1, m-1+1];
                    for(pass=1; pass<=passcount; pass++)
                    {
                        
                        //
                        // Prepare task:
                        // * X and Y stores grid
                        // * F stores function values
                        // * LX and LY stores twice dense grid (for Lipschitz testing)
                        //
                        ax = -1-AP.Math.RandomReal();
                        bx = +1+AP.Math.RandomReal();
                        ay = -1-AP.Math.RandomReal();
                        by = +1+AP.Math.RandomReal();
                        for(j=0; j<=n-1; j++)
                        {
                            x[j] = 0.5*(bx+ax)-0.5*(bx-ax)*Math.Cos(Math.PI*(2*j+1)/(2*n));
                            if( j==0 )
                            {
                                x[j] = ax;
                            }
                            if( j==n-1 )
                            {
                                x[j] = bx;
                            }
                            lx[2*j] = x[j];
                            if( j>0 )
                            {
                                lx[2*j-1] = 0.5*(x[j]+x[j-1]);
                            }
                        }
                        for(j=0; j<=n-1; j++)
                        {
                            k = AP.Math.RandomInteger(n);
                            if( k!=j )
                            {
                                t = x[j];
                                x[j] = x[k];
                                x[k] = t;
                            }
                        }
                        for(i=0; i<=m-1; i++)
                        {
                            y[i] = 0.5*(by+ay)-0.5*(by-ay)*Math.Cos(Math.PI*(2*i+1)/(2*m));
                            if( i==0 )
                            {
                                y[i] = ay;
                            }
                            if( i==m-1 )
                            {
                                y[i] = by;
                            }
                            ly[2*i] = y[i];
                            if( i>0 )
                            {
                                ly[2*i-1] = 0.5*(y[i]+y[i-1]);
                            }
                        }
                        for(i=0; i<=m-1; i++)
                        {
                            k = AP.Math.RandomInteger(m);
                            if( k!=i )
                            {
                                t = y[i];
                                y[i] = y[k];
                                y[k] = t;
                            }
                        }
                        for(i=0; i<=m-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                f[i,j] = Math.Exp(0.6*x[j])-Math.Exp(-(0.3*y[i])+0.08*x[j])+2*Math.Cos(Math.PI*(x[j]+1.2*y[i]))+0.1*Math.Cos(20*x[j]+15*y[i]);
                            }
                        }
                        
                        //
                        // Test bilinear interpolation:
                        // * interpolation at the nodes
                        // * linearity
                        // * continuity
                        // * differentiation in the inner points
                        //
                        spline2d.spline2dbuildbilinear(x, y, f, m, n, ref c);
                        err = 0;
                        for(i=0; i<=m-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                err = Math.Max(err, Math.Abs(f[i,j]-spline2d.spline2dcalc(ref c, x[j], y[i])));
                            }
                        }
                        blerrors = blerrors | (double)(err)>(double)(10000*AP.Math.MachineEpsilon);
                        err = 0;
                        for(i=0; i<=m-2; i++)
                        {
                            for(j=0; j<=n-2; j++)
                            {
                                
                                //
                                // Test for linearity between grid points
                                // (test point - geometric center of the cell)
                                //
                                fm = spline2d.spline2dcalc(ref c, lx[2*j+1], ly[2*i+1]);
                                f1 = spline2d.spline2dcalc(ref c, lx[2*j], ly[2*i]);
                                f2 = spline2d.spline2dcalc(ref c, lx[2*j+2], ly[2*i]);
                                f3 = spline2d.spline2dcalc(ref c, lx[2*j+2], ly[2*i+2]);
                                f4 = spline2d.spline2dcalc(ref c, lx[2*j], ly[2*i+2]);
                                err = Math.Max(err, Math.Abs(0.25*(f1+f2+f3+f4)-fm));
                            }
                        }
                        blerrors = blerrors | (double)(err)>(double)(10000*AP.Math.MachineEpsilon);
                        lconst(ref c, ref lx, ref ly, m, n, lstep, ref l1, ref l1x, ref l1y, ref l1xy);
                        lconst(ref c, ref lx, ref ly, m, n, lstep/3, ref l2, ref l2x, ref l2y, ref l2xy);
                        blerrors = blerrors | (double)(l2/l1)>(double)(1.2);
                        err = 0;
                        for(i=0; i<=m-2; i++)
                        {
                            for(j=0; j<=n-2; j++)
                            {
                                spline2d.spline2ddiff(ref c, lx[2*j+1], ly[2*i+1], ref v1, ref v1x, ref v1y, ref v1xy);
                                twodnumder(ref c, lx[2*j+1], ly[2*i+1], h, ref v2, ref v2x, ref v2y, ref v2xy);
                                err = Math.Max(err, Math.Abs(v1-v2));
                                err = Math.Max(err, Math.Abs(v1x-v2x));
                                err = Math.Max(err, Math.Abs(v1y-v2y));
                                err = Math.Max(err, Math.Abs(v1xy-v2xy));
                            }
                        }
                        dserrors = dserrors | (double)(err)>(double)(1.0E-3);
                        uperrors = uperrors | !testunpack(ref c, ref lx, ref ly);
                        lterrors = lterrors | !testlintrans(ref c, ax, bx, ay, by);
                        
                        //
                        // Test bicubic interpolation.
                        // * interpolation at the nodes
                        // * smoothness
                        // * differentiation
                        //
                        spline2d.spline2dbuildbicubic(x, y, f, m, n, ref c);
                        err = 0;
                        for(i=0; i<=m-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                err = Math.Max(err, Math.Abs(f[i,j]-spline2d.spline2dcalc(ref c, x[j], y[i])));
                            }
                        }
                        bcerrors = bcerrors | (double)(err)>(double)(10000*AP.Math.MachineEpsilon);
                        lconst(ref c, ref lx, ref ly, m, n, lstep, ref l1, ref l1x, ref l1y, ref l1xy);
                        lconst(ref c, ref lx, ref ly, m, n, lstep/3, ref l2, ref l2x, ref l2y, ref l2xy);
                        bcerrors = bcerrors | (double)(l2/l1)>(double)(1.2);
                        bcerrors = bcerrors | (double)(l2x/l1x)>(double)(1.2);
                        bcerrors = bcerrors | (double)(l2y/l1y)>(double)(1.2);
                        if( (double)(l2xy)>(double)(0.01) & (double)(l1xy)>(double)(0.01) )
                        {
                            
                            //
                            // Cross-derivative continuity is tested only when
                            // bigger than 0.01. When the task size is too
                            // small, the d2F/dXdY is nearly zero and Lipschitz
                            // constant ratio is ill-conditioned.
                            //
                            bcerrors = bcerrors | (double)(l2xy/l1xy)>(double)(1.2);
                        }
                        err = 0;
                        for(i=0; i<=2*m-2; i++)
                        {
                            for(j=0; j<=2*n-2; j++)
                            {
                                spline2d.spline2ddiff(ref c, lx[j], ly[i], ref v1, ref v1x, ref v1y, ref v1xy);
                                twodnumder(ref c, lx[j], ly[i], h, ref v2, ref v2x, ref v2y, ref v2xy);
                                err = Math.Max(err, Math.Abs(v1-v2));
                                err = Math.Max(err, Math.Abs(v1x-v2x));
                                err = Math.Max(err, Math.Abs(v1y-v2y));
                                err = Math.Max(err, Math.Abs(v1xy-v2xy));
                            }
                        }
                        dserrors = dserrors | (double)(err)>(double)(1.0E-3);
                        uperrors = uperrors | !testunpack(ref c, ref lx, ref ly);
                        lterrors = lterrors | !testlintrans(ref c, ax, bx, ay, by);
                        
                        //
                        // Copy/Serialise test
                        //
                        if( (double)(AP.Math.RandomReal())>(double)(0.5) )
                        {
                            spline2d.spline2dbuildbicubic(x, y, f, m, n, ref c);
                        }
                        else
                        {
                            spline2d.spline2dbuildbilinear(x, y, f, m, n, ref c);
                        }
                        unsetspline2d(ref c2);
                        spline2d.spline2dcopy(ref c, ref c2);
                        err = 0;
                        for(i=1; i<=5; i++)
                        {
                            t1 = ax+(bx-ax)*AP.Math.RandomReal();
                            t2 = ay+(by-ay)*AP.Math.RandomReal();
                            err = Math.Max(err, Math.Abs(spline2d.spline2dcalc(ref c, t1, t2)-spline2d.spline2dcalc(ref c2, t1, t2)));
                        }
                        cperrors = cperrors | (double)(err)>(double)(10000*AP.Math.MachineEpsilon);
                        unsetspline2d(ref c2);
                        spline2d.spline2dserialize(ref c, ref ra, ref ralen);
                        ra2 = new double[ralen];
                        for(i_=0; i_<=ralen-1;i_++)
                        {
                            ra2[i_] = ra[i_];
                        }
                        spline2d.spline2dunserialize(ref ra2, ref c2);
                        err = 0;
                        for(i=1; i<=5; i++)
                        {
                            t1 = ax+(bx-ax)*AP.Math.RandomReal();
                            t2 = ay+(by-ay)*AP.Math.RandomReal();
                            err = Math.Max(err, Math.Abs(spline2d.spline2dcalc(ref c, t1, t2)-spline2d.spline2dcalc(ref c2, t1, t2)));
                        }
                        cperrors = cperrors | (double)(err)>(double)(10000*AP.Math.MachineEpsilon);
                        
                        //
                        // Special symmetry test
                        //
                        err = 0;
                        for(jobtype=0; jobtype<=1; jobtype++)
                        {
                            
                            //
                            // Prepare
                            //
                            for(i=0; i<=m-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    ft[j,i] = f[i,j];
                                }
                            }
                            if( jobtype==0 )
                            {
                                spline2d.spline2dbuildbilinear(x, y, f, m, n, ref c);
                                spline2d.spline2dbuildbilinear(y, x, ft, n, m, ref c2);
                            }
                            else
                            {
                                spline2d.spline2dbuildbicubic(x, y, f, m, n, ref c);
                                spline2d.spline2dbuildbicubic(y, x, ft, n, m, ref c2);
                            }
                            
                            //
                            // Test
                            //
                            for(i=1; i<=10; i++)
                            {
                                t1 = ax+(bx-ax)*AP.Math.RandomReal();
                                t2 = ay+(by-ay)*AP.Math.RandomReal();
                                err = Math.Max(err, Math.Abs(spline2d.spline2dcalc(ref c, t1, t2)-spline2d.spline2dcalc(ref c2, t2, t1)));
                            }
                        }
                        syerrors = syerrors | (double)(err)>(double)(10000*AP.Math.MachineEpsilon);
                    }
                }
            }
            
            //
            // Test resample
            //
            for(m=2; m<=6; m++)
            {
                for(n=2; n<=6; n++)
                {
                    f = new double[m-1+1, n-1+1];
                    x = new double[n-1+1];
                    y = new double[m-1+1];
                    for(j=0; j<=n-1; j++)
                    {
                        x[j] = (double)(j)/((double)(n-1));
                    }
                    for(i=0; i<=m-1; i++)
                    {
                        y[i] = (double)(i)/((double)(m-1));
                    }
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            f[i,j] = Math.Exp(0.6*x[j])-Math.Exp(-(0.3*y[i])+0.08*x[j])+2*Math.Cos(Math.PI*(x[j]+1.2*y[i]))+0.1*Math.Cos(20*x[j]+15*y[i]);
                        }
                    }
                    for(m2=2; m2<=6; m2++)
                    {
                        for(n2=2; n2<=6; n2++)
                        {
                            for(pass=1; pass<=passcount; pass++)
                            {
                                for(jobtype=0; jobtype<=1; jobtype++)
                                {
                                    if( jobtype==0 )
                                    {
                                        spline2d.spline2dresamplebilinear(ref f, m, n, ref fr, m2, n2);
                                        spline2d.spline2dbuildbilinear(x, y, f, m, n, ref c);
                                    }
                                    if( jobtype==1 )
                                    {
                                        spline2d.spline2dresamplebicubic(ref f, m, n, ref fr, m2, n2);
                                        spline2d.spline2dbuildbicubic(x, y, f, m, n, ref c);
                                    }
                                    err = 0;
                                    mf = 0;
                                    for(i=0; i<=m2-1; i++)
                                    {
                                        for(j=0; j<=n2-1; j++)
                                        {
                                            v1 = spline2d.spline2dcalc(ref c, (double)(j)/((double)(n2-1)), (double)(i)/((double)(m2-1)));
                                            v2 = fr[i,j];
                                            err = Math.Max(err, Math.Abs(v1-v2));
                                            mf = Math.Max(mf, Math.Abs(v1));
                                        }
                                    }
                                    if( jobtype==0 )
                                    {
                                        rlerrors = rlerrors | (double)(err/mf)>(double)(10000*AP.Math.MachineEpsilon);
                                    }
                                    if( jobtype==1 )
                                    {
                                        rcerrors = rcerrors | (double)(err/mf)>(double)(10000*AP.Math.MachineEpsilon);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            //
            // report
            //
            waserrors = blerrors | bcerrors | dserrors | cperrors | uperrors | lterrors | syerrors | rlerrors | rcerrors;
            if( !silent )
            {
                System.Console.Write("TESTING 2D INTERPOLATION");
                System.Console.WriteLine();
                
                //
                // Normal tests
                //
                System.Console.Write("BILINEAR TEST:                           ");
                if( blerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("BICUBIC TEST:                            ");
                if( bcerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("DIFFERENTIATION TEST:                    ");
                if( dserrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("COPY/SERIALIZE TEST:                     ");
                if( cperrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("UNPACK TEST:                             ");
                if( uperrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("LIN.TRANS. TEST:                         ");
                if( lterrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("SPECIAL SYMMETRY TEST:                   ");
                if( syerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("BILINEAR RESAMPLING TEST:                ");
                if( rlerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("BICUBIC RESAMPLING TEST:                 ");
                if( rcerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                
                //
                // Summary
                //
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
        Lipschitz constants for spline inself, first and second derivatives.
        *************************************************************************/
        private static void lconst(ref spline2d.spline2dinterpolant c,
            ref double[] lx,
            ref double[] ly,
            int m,
            int n,
            double lstep,
            ref double lc,
            ref double lcx,
            ref double lcy,
            ref double lcxy)
        {
            int i = 0;
            int j = 0;
            double f1 = 0;
            double f2 = 0;
            double f3 = 0;
            double f4 = 0;
            double fx1 = 0;
            double fx2 = 0;
            double fx3 = 0;
            double fx4 = 0;
            double fy1 = 0;
            double fy2 = 0;
            double fy3 = 0;
            double fy4 = 0;
            double fxy1 = 0;
            double fxy2 = 0;
            double fxy3 = 0;
            double fxy4 = 0;
            double s2lstep = 0;

            lc = 0;
            lcx = 0;
            lcy = 0;
            lcxy = 0;
            s2lstep = Math.Sqrt(2)*lstep;
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    
                    //
                    // Calculate
                    //
                    twodnumder(ref c, lx[j]-lstep/2, ly[i]-lstep/2, lstep/4, ref f1, ref fx1, ref fy1, ref fxy1);
                    twodnumder(ref c, lx[j]+lstep/2, ly[i]-lstep/2, lstep/4, ref f2, ref fx2, ref fy2, ref fxy2);
                    twodnumder(ref c, lx[j]+lstep/2, ly[i]+lstep/2, lstep/4, ref f3, ref fx3, ref fy3, ref fxy3);
                    twodnumder(ref c, lx[j]-lstep/2, ly[i]+lstep/2, lstep/4, ref f4, ref fx4, ref fy4, ref fxy4);
                    
                    //
                    // Lipschitz constant for the function itself
                    //
                    lc = Math.Max(lc, Math.Abs((f1-f2)/lstep));
                    lc = Math.Max(lc, Math.Abs((f2-f3)/lstep));
                    lc = Math.Max(lc, Math.Abs((f3-f4)/lstep));
                    lc = Math.Max(lc, Math.Abs((f4-f1)/lstep));
                    lc = Math.Max(lc, Math.Abs((f1-f3)/s2lstep));
                    lc = Math.Max(lc, Math.Abs((f2-f4)/s2lstep));
                    
                    //
                    // Lipschitz constant for the first derivative
                    //
                    lcx = Math.Max(lcx, Math.Abs((fx1-fx2)/lstep));
                    lcx = Math.Max(lcx, Math.Abs((fx2-fx3)/lstep));
                    lcx = Math.Max(lcx, Math.Abs((fx3-fx4)/lstep));
                    lcx = Math.Max(lcx, Math.Abs((fx4-fx1)/lstep));
                    lcx = Math.Max(lcx, Math.Abs((fx1-fx3)/s2lstep));
                    lcx = Math.Max(lcx, Math.Abs((fx2-fx4)/s2lstep));
                    
                    //
                    // Lipschitz constant for the first derivative
                    //
                    lcy = Math.Max(lcy, Math.Abs((fy1-fy2)/lstep));
                    lcy = Math.Max(lcy, Math.Abs((fy2-fy3)/lstep));
                    lcy = Math.Max(lcy, Math.Abs((fy3-fy4)/lstep));
                    lcy = Math.Max(lcy, Math.Abs((fy4-fy1)/lstep));
                    lcy = Math.Max(lcy, Math.Abs((fy1-fy3)/s2lstep));
                    lcy = Math.Max(lcy, Math.Abs((fy2-fy4)/s2lstep));
                    
                    //
                    // Lipschitz constant for the cross-derivative
                    //
                    lcxy = Math.Max(lcxy, Math.Abs((fxy1-fxy2)/lstep));
                    lcxy = Math.Max(lcxy, Math.Abs((fxy2-fxy3)/lstep));
                    lcxy = Math.Max(lcxy, Math.Abs((fxy3-fxy4)/lstep));
                    lcxy = Math.Max(lcxy, Math.Abs((fxy4-fxy1)/lstep));
                    lcxy = Math.Max(lcxy, Math.Abs((fxy1-fxy3)/s2lstep));
                    lcxy = Math.Max(lcxy, Math.Abs((fxy2-fxy4)/s2lstep));
                }
            }
        }


        /*************************************************************************
        Numerical differentiation.
        *************************************************************************/
        private static void twodnumder(ref spline2d.spline2dinterpolant c,
            double x,
            double y,
            double h,
            ref double f,
            ref double fx,
            ref double fy,
            ref double fxy)
        {
            f = spline2d.spline2dcalc(ref c, x, y);
            fx = (spline2d.spline2dcalc(ref c, x+h, y)-spline2d.spline2dcalc(ref c, x-h, y))/(2*h);
            fy = (spline2d.spline2dcalc(ref c, x, y+h)-spline2d.spline2dcalc(ref c, x, y-h))/(2*h);
            fxy = (spline2d.spline2dcalc(ref c, x+h, y+h)-spline2d.spline2dcalc(ref c, x-h, y+h)-spline2d.spline2dcalc(ref c, x+h, y-h)+spline2d.spline2dcalc(ref c, x-h, y-h))/AP.Math.Sqr(2*h);
        }


        /*************************************************************************
        Unpack test
        *************************************************************************/
        private static bool testunpack(ref spline2d.spline2dinterpolant c,
            ref double[] lx,
            ref double[] ly)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            int n = 0;
            int m = 0;
            int ci = 0;
            int cj = 0;
            int p = 0;
            double err = 0;
            double tx = 0;
            double ty = 0;
            double v1 = 0;
            double v2 = 0;
            int pass = 0;
            int passcount = 0;
            double[,] tbl = new double[0,0];

            passcount = 20;
            err = 0;
            spline2d.spline2dunpack(ref c, ref m, ref n, ref tbl);
            for(i=0; i<=m-2; i++)
            {
                for(j=0; j<=n-2; j++)
                {
                    for(pass=1; pass<=passcount; pass++)
                    {
                        p = (n-1)*i+j;
                        tx = (0.001+0.999*AP.Math.RandomReal())*(tbl[p,1]-tbl[p,0]);
                        ty = (0.001+0.999*AP.Math.RandomReal())*(tbl[p,3]-tbl[p,2]);
                        
                        //
                        // Interpolation properties
                        //
                        v1 = 0;
                        for(ci=0; ci<=3; ci++)
                        {
                            for(cj=0; cj<=3; cj++)
                            {
                                v1 = v1+tbl[p,4+ci*4+cj]*Math.Pow(tx, ci)*Math.Pow(ty, cj);
                            }
                        }
                        v2 = spline2d.spline2dcalc(ref c, tbl[p,0]+tx, tbl[p,2]+ty);
                        err = Math.Max(err, Math.Abs(v1-v2));
                        
                        //
                        // Grid correctness
                        //
                        err = Math.Max(err, Math.Abs(lx[2*j]-tbl[p,0]));
                        err = Math.Max(err, Math.Abs(lx[2*(j+1)]-tbl[p,1]));
                        err = Math.Max(err, Math.Abs(ly[2*i]-tbl[p,2]));
                        err = Math.Max(err, Math.Abs(ly[2*(i+1)]-tbl[p,3]));
                    }
                }
            }
            result = (double)(err)<(double)(10000*AP.Math.MachineEpsilon);
            return result;
        }


        /*************************************************************************
        LinTrans test
        *************************************************************************/
        private static bool testlintrans(ref spline2d.spline2dinterpolant c,
            double ax,
            double bx,
            double ay,
            double by)
        {
            bool result = new bool();
            double err = 0;
            double a1 = 0;
            double a2 = 0;
            double b1 = 0;
            double b2 = 0;
            double tx = 0;
            double ty = 0;
            double vx = 0;
            double vy = 0;
            double v1 = 0;
            double v2 = 0;
            int pass = 0;
            int passcount = 0;
            int xjob = 0;
            int yjob = 0;
            spline2d.spline2dinterpolant c2 = new spline2d.spline2dinterpolant();

            passcount = 5;
            err = 0;
            for(xjob=0; xjob<=1; xjob++)
            {
                for(yjob=0; yjob<=1; yjob++)
                {
                    for(pass=1; pass<=passcount; pass++)
                    {
                        
                        //
                        // Prepare
                        //
                        do
                        {
                            a1 = 2*AP.Math.RandomReal()-1;
                        }
                        while( (double)(a1)==(double)(0) );
                        a1 = a1*xjob;
                        b1 = 2*AP.Math.RandomReal()-1;
                        do
                        {
                            a2 = 2*AP.Math.RandomReal()-1;
                        }
                        while( (double)(a2)==(double)(0) );
                        a2 = a2*yjob;
                        b2 = 2*AP.Math.RandomReal()-1;
                        
                        //
                        // Test XY
                        //
                        spline2d.spline2dcopy(ref c, ref c2);
                        spline2d.spline2dlintransxy(ref c2, a1, b1, a2, b2);
                        tx = ax+AP.Math.RandomReal()*(bx-ax);
                        ty = ay+AP.Math.RandomReal()*(by-ay);
                        if( xjob==0 )
                        {
                            tx = b1;
                            vx = ax+AP.Math.RandomReal()*(bx-ax);
                        }
                        else
                        {
                            vx = (tx-b1)/a1;
                        }
                        if( yjob==0 )
                        {
                            ty = b2;
                            vy = ay+AP.Math.RandomReal()*(by-ay);
                        }
                        else
                        {
                            vy = (ty-b2)/a2;
                        }
                        v1 = spline2d.spline2dcalc(ref c, tx, ty);
                        v2 = spline2d.spline2dcalc(ref c2, vx, vy);
                        err = Math.Max(err, Math.Abs(v1-v2));
                        
                        //
                        // Test F
                        //
                        spline2d.spline2dcopy(ref c, ref c2);
                        spline2d.spline2dlintransf(ref c2, a1, b1);
                        tx = ax+AP.Math.RandomReal()*(bx-ax);
                        ty = ay+AP.Math.RandomReal()*(by-ay);
                        v1 = spline2d.spline2dcalc(ref c, tx, ty);
                        v2 = spline2d.spline2dcalc(ref c2, tx, ty);
                        err = Math.Max(err, Math.Abs(a1*v1+b1-v2));
                    }
                }
            }
            result = (double)(err)<(double)(10000*AP.Math.MachineEpsilon);
            return result;
        }


        /*************************************************************************
        Unset spline, i.e. initialize it with random garbage
        *************************************************************************/
        private static void unsetspline2d(ref spline2d.spline2dinterpolant c)
        {
            double[] x = new double[0];
            double[] y = new double[0];
            double[,] f = new double[0,0];

            x = new double[2];
            y = new double[2];
            f = new double[2, 2];
            x[0] = -1;
            x[1] = +1;
            y[0] = -1;
            y[1] = +1;
            f[0,0] = 0;
            f[0,1] = 0;
            f[1,0] = 0;
            f[1,1] = 0;
            spline2d.spline2dbuildbilinear(x, y, f, 2, 2, ref c);
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testinterpolation2dunit_test_silent()
        {
            bool result = new bool();

            result = test2dinterpolation(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testinterpolation2dunit_test()
        {
            bool result = new bool();

            result = test2dinterpolation(false);
            return result;
        }
    }
}
