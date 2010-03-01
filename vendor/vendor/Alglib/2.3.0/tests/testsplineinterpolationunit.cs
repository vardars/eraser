
using System;

namespace alglib
{
    public class testsplineinterpolationunit
    {
        public static bool testsplineinterpolation(bool silent)
        {
            bool result = new bool();
            bool waserrors = new bool();
            bool cserrors = new bool();
            bool hserrors = new bool();
            bool aserrors = new bool();
            bool lserrors = new bool();
            bool dserrors = new bool();
            bool uperrors = new bool();
            bool cperrors = new bool();
            bool lterrors = new bool();
            bool ierrors = new bool();
            bool fiterrors = new bool();
            double nonstrictthreshold = 0;
            double threshold = 0;
            int passcount = 0;
            double lstep = 0;
            double h = 0;
            int maxn = 0;
            int n = 0;
            int m = 0;
            int i = 0;
            int k = 0;
            int pass = 0;
            int bltype = 0;
            int brtype = 0;
            int stype = 0;
            double[] x = new double[0];
            double[] y = new double[0];
            double[] w = new double[0];
            double[] w2 = new double[0];
            double[] y2 = new double[0];
            double[] d = new double[0];
            double[] xc = new double[0];
            double[] yc = new double[0];
            int[] dc = new int[0];
            spline1d.spline1dinterpolant c = new spline1d.spline1dinterpolant();
            spline1d.spline1dinterpolant c2 = new spline1d.spline1dinterpolant();
            int info = 0;
            int info1 = 0;
            int info2 = 0;
            double a = 0;
            double b = 0;
            double bl = 0;
            double br = 0;
            double t = 0;
            double sa = 0;
            double sb = 0;
            double v = 0;
            double v1 = 0;
            double v2 = 0;
            double l10 = 0;
            double l11 = 0;
            double l12 = 0;
            double l20 = 0;
            double l21 = 0;
            double l22 = 0;
            double p0 = 0;
            double p1 = 0;
            double p2 = 0;
            double s = 0;
            double ds = 0;
            double d2s = 0;
            double s2 = 0;
            double ds2 = 0;
            double d2s2 = 0;
            double vl = 0;
            double vm = 0;
            double vr = 0;
            double err = 0;
            spline1d.spline1dfitreport rep = new spline1d.spline1dfitreport();
            spline1d.spline1dfitreport rep2 = new spline1d.spline1dfitreport();
            double refrms = 0;
            double refavg = 0;
            double refavgrel = 0;
            double refmax = 0;
            double[] ra = new double[0];
            double[] ra2 = new double[0];
            int ralen = 0;
            int i_ = 0;

            waserrors = false;
            passcount = 20;
            lstep = 0.005;
            h = 0.00001;
            maxn = 10;
            threshold = 10000*AP.Math.MachineEpsilon;
            nonstrictthreshold = 0.00001;
            lserrors = false;
            cserrors = false;
            hserrors = false;
            aserrors = false;
            dserrors = false;
            cperrors = false;
            uperrors = false;
            lterrors = false;
            ierrors = false;
            fiterrors = false;
            
            //
            // General test: linear, cubic, Hermite, Akima
            //
            for(n=2; n<=maxn; n++)
            {
                x = new double[n-1+1];
                y = new double[n-1+1];
                d = new double[n-1+1];
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // Prepare task
                    //
                    a = -1-AP.Math.RandomReal();
                    b = +1+AP.Math.RandomReal();
                    bl = 2*AP.Math.RandomReal()-1;
                    br = 2*AP.Math.RandomReal()-1;
                    for(i=0; i<=n-1; i++)
                    {
                        x[i] = 0.5*(b+a)+0.5*(b-a)*Math.Cos(Math.PI*(2*i+1)/(2*n));
                        if( i==0 )
                        {
                            x[i] = a;
                        }
                        if( i==n-1 )
                        {
                            x[i] = b;
                        }
                        y[i] = Math.Cos(1.3*Math.PI*x[i]+0.4);
                        d[i] = -(1.3*Math.PI*Math.Sin(1.3*Math.PI*x[i]+0.4));
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        k = AP.Math.RandomInteger(n);
                        if( k!=i )
                        {
                            t = x[i];
                            x[i] = x[k];
                            x[k] = t;
                            t = y[i];
                            y[i] = y[k];
                            y[k] = t;
                            t = d[i];
                            d[i] = d[k];
                            d[k] = t;
                        }
                    }
                    
                    //
                    // Build linear spline
                    // Test for general interpolation scheme properties:
                    // * values at nodes
                    // * continuous function
                    // Test for specific properties is implemented below.
                    //
                    spline1d.spline1dbuildlinear(x, y, n, ref c);
                    err = 0;
                    for(i=0; i<=n-1; i++)
                    {
                        err = Math.Max(err, Math.Abs(y[i]-spline1d.spline1dcalc(ref c, x[i])));
                    }
                    lserrors = lserrors | (double)(err)>(double)(threshold);
                    lconst(a, b, ref c, lstep, ref l10, ref l11, ref l12);
                    lconst(a, b, ref c, lstep/3, ref l20, ref l21, ref l22);
                    lserrors = lserrors | (double)(l20/l10)>(double)(1.2);
                    
                    //
                    // Build cubic spline.
                    // Test for interpolation scheme properties:
                    // * values at nodes
                    // * boundary conditions
                    // * continuous function
                    // * continuous first derivative
                    // * continuous second derivative
                    //
                    for(bltype=0; bltype<=2; bltype++)
                    {
                        for(brtype=0; brtype<=2; brtype++)
                        {
                            spline1d.spline1dbuildcubic(x, y, n, bltype, bl, brtype, br, ref c);
                            err = 0;
                            for(i=0; i<=n-1; i++)
                            {
                                err = Math.Max(err, Math.Abs(y[i]-spline1d.spline1dcalc(ref c, x[i])));
                            }
                            cserrors = cserrors | (double)(err)>(double)(threshold);
                            err = 0;
                            if( bltype==0 )
                            {
                                spline1d.spline1ddiff(ref c, a-h, ref s, ref ds, ref d2s);
                                spline1d.spline1ddiff(ref c, a+h, ref s2, ref ds2, ref d2s2);
                                t = (d2s2-d2s)/(2*h);
                                err = Math.Max(err, Math.Abs(t));
                            }
                            if( bltype==1 )
                            {
                                t = (spline1d.spline1dcalc(ref c, a+h)-spline1d.spline1dcalc(ref c, a-h))/(2*h);
                                err = Math.Max(err, Math.Abs(bl-t));
                            }
                            if( bltype==2 )
                            {
                                t = (spline1d.spline1dcalc(ref c, a+h)-2*spline1d.spline1dcalc(ref c, a)+spline1d.spline1dcalc(ref c, a-h))/AP.Math.Sqr(h);
                                err = Math.Max(err, Math.Abs(bl-t));
                            }
                            if( brtype==0 )
                            {
                                spline1d.spline1ddiff(ref c, b-h, ref s, ref ds, ref d2s);
                                spline1d.spline1ddiff(ref c, b+h, ref s2, ref ds2, ref d2s2);
                                t = (d2s2-d2s)/(2*h);
                                err = Math.Max(err, Math.Abs(t));
                            }
                            if( brtype==1 )
                            {
                                t = (spline1d.spline1dcalc(ref c, b+h)-spline1d.spline1dcalc(ref c, b-h))/(2*h);
                                err = Math.Max(err, Math.Abs(br-t));
                            }
                            if( brtype==2 )
                            {
                                t = (spline1d.spline1dcalc(ref c, b+h)-2*spline1d.spline1dcalc(ref c, b)+spline1d.spline1dcalc(ref c, b-h))/AP.Math.Sqr(h);
                                err = Math.Max(err, Math.Abs(br-t));
                            }
                            cserrors = cserrors | (double)(err)>(double)(1.0E-3);
                            lconst(a, b, ref c, lstep, ref l10, ref l11, ref l12);
                            lconst(a, b, ref c, lstep/3, ref l20, ref l21, ref l22);
                            cserrors = cserrors | (double)(l20/l10)>(double)(1.2) & (double)(l10)>(double)(1.0E-6);
                            cserrors = cserrors | (double)(l21/l11)>(double)(1.2) & (double)(l11)>(double)(1.0E-6);
                            cserrors = cserrors | (double)(l22/l12)>(double)(1.2) & (double)(l12)>(double)(1.0E-6);
                        }
                    }
                    
                    //
                    // Build Hermite spline.
                    // Test for interpolation scheme properties:
                    // * values and derivatives at nodes
                    // * continuous function
                    // * continuous first derivative
                    //
                    spline1d.spline1dbuildhermite(x, y, d, n, ref c);
                    err = 0;
                    for(i=0; i<=n-1; i++)
                    {
                        err = Math.Max(err, Math.Abs(y[i]-spline1d.spline1dcalc(ref c, x[i])));
                    }
                    hserrors = hserrors | (double)(err)>(double)(threshold);
                    err = 0;
                    for(i=0; i<=n-1; i++)
                    {
                        t = (spline1d.spline1dcalc(ref c, x[i]+h)-spline1d.spline1dcalc(ref c, x[i]-h))/(2*h);
                        err = Math.Max(err, Math.Abs(d[i]-t));
                    }
                    hserrors = hserrors | (double)(err)>(double)(1.0E-3);
                    lconst(a, b, ref c, lstep, ref l10, ref l11, ref l12);
                    lconst(a, b, ref c, lstep/3, ref l20, ref l21, ref l22);
                    hserrors = hserrors | (double)(l20/l10)>(double)(1.2);
                    hserrors = hserrors | (double)(l21/l11)>(double)(1.2);
                    
                    //
                    // Build Akima spline
                    // Test for general interpolation scheme properties:
                    // * values at nodes
                    // * continuous function
                    // * continuous first derivative
                    // Test for specific properties is implemented below.
                    //
                    if( n>=5 )
                    {
                        spline1d.spline1dbuildakima(x, y, n, ref c);
                        err = 0;
                        for(i=0; i<=n-1; i++)
                        {
                            err = Math.Max(err, Math.Abs(y[i]-spline1d.spline1dcalc(ref c, x[i])));
                        }
                        aserrors = aserrors | (double)(err)>(double)(threshold);
                        lconst(a, b, ref c, lstep, ref l10, ref l11, ref l12);
                        lconst(a, b, ref c, lstep/3, ref l20, ref l21, ref l22);
                        hserrors = hserrors | (double)(l20/l10)>(double)(1.2);
                        hserrors = hserrors | (double)(l21/l11)>(double)(1.2);
                    }
                }
            }
            
            //
            // Special linear spline test:
            // test for linearity between x[i] and x[i+1]
            //
            for(n=2; n<=maxn; n++)
            {
                x = new double[n-1+1];
                y = new double[n-1+1];
                
                //
                // Prepare task
                //
                a = -1;
                b = +1;
                for(i=0; i<=n-1; i++)
                {
                    x[i] = a+(b-a)*i/(n-1);
                    y[i] = 2*AP.Math.RandomReal()-1;
                }
                spline1d.spline1dbuildlinear(x, y, n, ref c);
                
                //
                // Test
                //
                err = 0;
                for(k=0; k<=n-2; k++)
                {
                    a = x[k];
                    b = x[k+1];
                    for(pass=1; pass<=passcount; pass++)
                    {
                        t = a+(b-a)*AP.Math.RandomReal();
                        v = y[k]+(t-a)/(b-a)*(y[k+1]-y[k]);
                        err = Math.Max(err, Math.Abs(spline1d.spline1dcalc(ref c, t)-v));
                    }
                }
                lserrors = lserrors | (double)(err)>(double)(threshold);
            }
            
            //
            // Special Akima test: test outlier sensitivity
            // Spline value at (x[i], x[i+1]) should depend from
            // f[i-2], f[i-1], f[i], f[i+1], f[i+2], f[i+3] only.
            //
            for(n=5; n<=maxn; n++)
            {
                x = new double[n-1+1];
                y = new double[n-1+1];
                y2 = new double[n-1+1];
                
                //
                // Prepare unperturbed Akima spline
                //
                a = -1;
                b = +1;
                for(i=0; i<=n-1; i++)
                {
                    x[i] = a+(b-a)*i/(n-1);
                    y[i] = Math.Cos(1.3*Math.PI*x[i]+0.4);
                }
                spline1d.spline1dbuildakima(x, y, n, ref c);
                
                //
                // Process perturbed tasks
                //
                err = 0;
                for(k=0; k<=n-1; k++)
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        y2[i_] = y[i_];
                    }
                    y2[k] = 5;
                    spline1d.spline1dbuildakima(x, y2, n, ref c2);
                    
                    //
                    // Test left part independence
                    //
                    if( k-3>=1 )
                    {
                        a = -1;
                        b = x[k-3];
                        for(pass=1; pass<=passcount; pass++)
                        {
                            t = a+(b-a)*AP.Math.RandomReal();
                            err = Math.Max(err, Math.Abs(spline1d.spline1dcalc(ref c, t)-spline1d.spline1dcalc(ref c2, t)));
                        }
                    }
                    
                    //
                    // Test right part independence
                    //
                    if( k+3<=n-2 )
                    {
                        a = x[k+3];
                        b = +1;
                        for(pass=1; pass<=passcount; pass++)
                        {
                            t = a+(b-a)*AP.Math.RandomReal();
                            err = Math.Max(err, Math.Abs(spline1d.spline1dcalc(ref c, t)-spline1d.spline1dcalc(ref c2, t)));
                        }
                    }
                }
                aserrors = aserrors | (double)(err)>(double)(threshold);
            }
            
            //
            // Differentiation, copy/serialize/unpack test
            //
            for(n=2; n<=maxn; n++)
            {
                x = new double[n-1+1];
                y = new double[n-1+1];
                
                //
                // Prepare cubic spline
                //
                a = -1-AP.Math.RandomReal();
                b = +1+AP.Math.RandomReal();
                for(i=0; i<=n-1; i++)
                {
                    x[i] = a+(b-a)*i/(n-1);
                    y[i] = Math.Cos(1.3*Math.PI*x[i]+0.4);
                }
                spline1d.spline1dbuildcubic(x, y, n, 2, 0.0, 2, 0.0, ref c);
                
                //
                // Test diff
                //
                err = 0;
                for(pass=1; pass<=passcount; pass++)
                {
                    t = a+(b-a)*AP.Math.RandomReal();
                    spline1d.spline1ddiff(ref c, t, ref s, ref ds, ref d2s);
                    vl = spline1d.spline1dcalc(ref c, t-h);
                    vm = spline1d.spline1dcalc(ref c, t);
                    vr = spline1d.spline1dcalc(ref c, t+h);
                    err = Math.Max(err, Math.Abs(s-vm));
                    err = Math.Max(err, Math.Abs(ds-(vr-vl)/(2*h)));
                    err = Math.Max(err, Math.Abs(d2s-(vr-2*vm+vl)/AP.Math.Sqr(h)));
                }
                dserrors = dserrors | (double)(err)>(double)(0.001);
                
                //
                // Test copy
                //
                unsetspline1d(ref c2);
                spline1d.spline1dcopy(ref c, ref c2);
                err = 0;
                for(pass=1; pass<=passcount; pass++)
                {
                    t = a+(b-a)*AP.Math.RandomReal();
                    err = Math.Max(err, Math.Abs(spline1d.spline1dcalc(ref c, t)-spline1d.spline1dcalc(ref c2, t)));
                }
                cperrors = cperrors | (double)(err)>(double)(threshold);
                
                //
                // Test serialize/deserialize
                //
                unsetspline1d(ref c2);
                unset1d(ref ra2);
                spline1d.spline1dserialize(ref c, ref ra, ref ralen);
                ra2 = new double[ralen];
                for(i_=0; i_<=ralen-1;i_++)
                {
                    ra2[i_] = ra[i_];
                }
                spline1d.spline1dunserialize(ref ra2, ref c2);
                err = 0;
                for(pass=1; pass<=passcount; pass++)
                {
                    t = a+(b-a)*AP.Math.RandomReal();
                    err = Math.Max(err, Math.Abs(spline1d.spline1dcalc(ref c, t)-spline1d.spline1dcalc(ref c2, t)));
                }
                cperrors = cperrors | (double)(err)>(double)(threshold);
                
                //
                // Test unpack
                //
                uperrors = uperrors | !testunpack(ref c, ref x);
                
                //
                // Test lin.trans.
                //
                err = 0;
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // LinTransX, general A
                    //
                    sa = 4*AP.Math.RandomReal()-2;
                    sb = 2*AP.Math.RandomReal()-1;
                    t = a+(b-a)*AP.Math.RandomReal();
                    spline1d.spline1dcopy(ref c, ref c2);
                    spline1d.spline1dlintransx(ref c2, sa, sb);
                    err = Math.Max(err, Math.Abs(spline1d.spline1dcalc(ref c, t)-spline1d.spline1dcalc(ref c2, (t-sb)/sa)));
                    
                    //
                    // LinTransX, special case: A=0
                    //
                    sb = 2*AP.Math.RandomReal()-1;
                    t = a+(b-a)*AP.Math.RandomReal();
                    spline1d.spline1dcopy(ref c, ref c2);
                    spline1d.spline1dlintransx(ref c2, 0, sb);
                    err = Math.Max(err, Math.Abs(spline1d.spline1dcalc(ref c, sb)-spline1d.spline1dcalc(ref c2, t)));
                    
                    //
                    // LinTransY
                    //
                    sa = 2*AP.Math.RandomReal()-1;
                    sb = 2*AP.Math.RandomReal()-1;
                    t = a+(b-a)*AP.Math.RandomReal();
                    spline1d.spline1dcopy(ref c, ref c2);
                    spline1d.spline1dlintransy(ref c2, sa, sb);
                    err = Math.Max(err, Math.Abs(sa*spline1d.spline1dcalc(ref c, t)+sb-spline1d.spline1dcalc(ref c2, t)));
                }
                lterrors = lterrors | (double)(err)>(double)(threshold);
            }
            
            //
            // Testing integration. Two tests are performed:
            // * approximate test (well behaved smooth function, many points,
            //   integration inside [a,b])
            // * exact test (integration of parabola, outside of [a,b]
            //
            err = 0;
            for(n=20; n<=35; n++)
            {
                x = new double[n-1+1];
                y = new double[n-1+1];
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // Prepare cubic spline
                    //
                    a = -1-0.2*AP.Math.RandomReal();
                    b = +1+0.2*AP.Math.RandomReal();
                    for(i=0; i<=n-1; i++)
                    {
                        x[i] = a+(b-a)*i/(n-1);
                        y[i] = Math.Sin(Math.PI*x[i]+0.4)+Math.Exp(x[i]);
                    }
                    bl = Math.PI*Math.Cos(Math.PI*a+0.4)+Math.Exp(a);
                    br = Math.PI*Math.Cos(Math.PI*b+0.4)+Math.Exp(b);
                    spline1d.spline1dbuildcubic(x, y, n, 1, bl, 1, br, ref c);
                    
                    //
                    // Test
                    //
                    t = a+(b-a)*AP.Math.RandomReal();
                    v = -(Math.Cos(Math.PI*a+0.4)/Math.PI)+Math.Exp(a);
                    v = -(Math.Cos(Math.PI*t+0.4)/Math.PI)+Math.Exp(t)-v;
                    v = v-spline1d.spline1dintegrate(ref c, t);
                    err = Math.Max(err, Math.Abs(v));
                }
            }
            ierrors = ierrors | (double)(err)>(double)(0.001);
            p0 = 2*AP.Math.RandomReal()-1;
            p1 = 2*AP.Math.RandomReal()-1;
            p2 = 2*AP.Math.RandomReal()-1;
            a = -AP.Math.RandomReal()-0.5;
            b = +AP.Math.RandomReal()+0.5;
            n = 2;
            x = new double[n];
            y = new double[n];
            d = new double[n];
            x[0] = a;
            y[0] = p0+p1*a+p2*AP.Math.Sqr(a);
            d[0] = p1+2*p2*a;
            x[1] = b;
            y[1] = p0+p1*b+p2*AP.Math.Sqr(b);
            d[1] = p1+2*p2*b;
            spline1d.spline1dbuildhermite(x, y, d, n, ref c);
            bl = Math.Min(a, b)-Math.Abs(b-a);
            br = Math.Min(a, b)+Math.Abs(b-a);
            err = 0;
            for(pass=1; pass<=100; pass++)
            {
                t = bl+(br-bl)*AP.Math.RandomReal();
                v = p0*t+p1*AP.Math.Sqr(t)/2+p2*AP.Math.Sqr(t)*t/3-(p0*a+p1*AP.Math.Sqr(a)/2+p2*AP.Math.Sqr(a)*a/3);
                v = v-spline1d.spline1dintegrate(ref c, t);
                err = Math.Max(err, Math.Abs(v));
            }
            ierrors = ierrors | (double)(err)>(double)(threshold);
            
            //
            // Test fitting.
            //
            for(pass=1; pass<=passcount; pass++)
            {
                
                //
                // Cubic splines
                // Ability to handle boundary constraints (1-4 constraints on F, dF/dx).
                //
                for(m=4; m<=8; m++)
                {
                    for(k=1; k<=4; k++)
                    {
                        if( k>=m )
                        {
                            continue;
                        }
                        n = 100;
                        x = new double[n];
                        y = new double[n];
                        w = new double[n];
                        xc = new double[4];
                        yc = new double[4];
                        dc = new int[4];
                        sa = 1+AP.Math.RandomReal();
                        sb = 2*AP.Math.RandomReal()-1;
                        for(i=0; i<=n-1; i++)
                        {
                            x[i] = sa*AP.Math.RandomReal()+sb;
                            y[i] = 2*AP.Math.RandomReal()-1;
                            w[i] = 1+AP.Math.RandomReal();
                        }
                        xc[0] = sb;
                        yc[0] = 2*AP.Math.RandomReal()-1;
                        dc[0] = 0;
                        xc[1] = sb;
                        yc[1] = 2*AP.Math.RandomReal()-1;
                        dc[1] = 1;
                        xc[2] = sa+sb;
                        yc[2] = 2*AP.Math.RandomReal()-1;
                        dc[2] = 0;
                        xc[3] = sa+sb;
                        yc[3] = 2*AP.Math.RandomReal()-1;
                        dc[3] = 1;
                        spline1d.spline1dfitcubicwc(ref x, ref y, ref w, n, ref xc, ref yc, ref dc, k, m, ref info, ref c, ref rep);
                        if( info<=0 )
                        {
                            fiterrors = true;
                        }
                        else
                        {
                            
                            //
                            // Check that constraints are satisfied
                            //
                            for(i=0; i<=k-1; i++)
                            {
                                spline1d.spline1ddiff(ref c, xc[i], ref s, ref ds, ref d2s);
                                if( dc[i]==0 )
                                {
                                    fiterrors = fiterrors | (double)(Math.Abs(s-yc[i]))>(double)(threshold);
                                }
                                if( dc[i]==1 )
                                {
                                    fiterrors = fiterrors | (double)(Math.Abs(ds-yc[i]))>(double)(threshold);
                                }
                                if( dc[i]==2 )
                                {
                                    fiterrors = fiterrors | (double)(Math.Abs(d2s-yc[i]))>(double)(threshold);
                                }
                            }
                        }
                    }
                }
                
                //
                // Cubic splines
                // Ability to handle one internal constraint
                //
                for(m=4; m<=8; m++)
                {
                    n = 100;
                    x = new double[n];
                    y = new double[n];
                    w = new double[n];
                    xc = new double[1];
                    yc = new double[1];
                    dc = new int[1];
                    sa = 1+AP.Math.RandomReal();
                    sb = 2*AP.Math.RandomReal()-1;
                    for(i=0; i<=n-1; i++)
                    {
                        x[i] = sa*AP.Math.RandomReal()+sb;
                        y[i] = 2*AP.Math.RandomReal()-1;
                        w[i] = 1+AP.Math.RandomReal();
                    }
                    xc[0] = sa*AP.Math.RandomReal()+sb;
                    yc[0] = 2*AP.Math.RandomReal()-1;
                    dc[0] = AP.Math.RandomInteger(2);
                    spline1d.spline1dfitcubicwc(ref x, ref y, ref w, n, ref xc, ref yc, ref dc, 1, m, ref info, ref c, ref rep);
                    if( info<=0 )
                    {
                        fiterrors = true;
                    }
                    else
                    {
                        
                        //
                        // Check that constraints are satisfied
                        //
                        spline1d.spline1ddiff(ref c, xc[0], ref s, ref ds, ref d2s);
                        if( dc[0]==0 )
                        {
                            fiterrors = fiterrors | (double)(Math.Abs(s-yc[0]))>(double)(threshold);
                        }
                        if( dc[0]==1 )
                        {
                            fiterrors = fiterrors | (double)(Math.Abs(ds-yc[0]))>(double)(threshold);
                        }
                        if( dc[0]==2 )
                        {
                            fiterrors = fiterrors | (double)(Math.Abs(d2s-yc[0]))>(double)(threshold);
                        }
                    }
                }
                
                //
                // Hermite splines
                // Ability to handle boundary constraints (1-4 constraints on F, dF/dx).
                //
                for(m=4; m<=8; m++)
                {
                    for(k=1; k<=4; k++)
                    {
                        if( k>=m )
                        {
                            continue;
                        }
                        if( m%2!=0 )
                        {
                            continue;
                        }
                        n = 100;
                        x = new double[n];
                        y = new double[n];
                        w = new double[n];
                        xc = new double[4];
                        yc = new double[4];
                        dc = new int[4];
                        sa = 1+AP.Math.RandomReal();
                        sb = 2*AP.Math.RandomReal()-1;
                        for(i=0; i<=n-1; i++)
                        {
                            x[i] = sa*AP.Math.RandomReal()+sb;
                            y[i] = 2*AP.Math.RandomReal()-1;
                            w[i] = 1+AP.Math.RandomReal();
                        }
                        xc[0] = sb;
                        yc[0] = 2*AP.Math.RandomReal()-1;
                        dc[0] = 0;
                        xc[1] = sb;
                        yc[1] = 2*AP.Math.RandomReal()-1;
                        dc[1] = 1;
                        xc[2] = sa+sb;
                        yc[2] = 2*AP.Math.RandomReal()-1;
                        dc[2] = 0;
                        xc[3] = sa+sb;
                        yc[3] = 2*AP.Math.RandomReal()-1;
                        dc[3] = 1;
                        spline1d.spline1dfithermitewc(ref x, ref y, ref w, n, ref xc, ref yc, ref dc, k, m, ref info, ref c, ref rep);
                        if( info<=0 )
                        {
                            fiterrors = true;
                        }
                        else
                        {
                            
                            //
                            // Check that constraints are satisfied
                            //
                            for(i=0; i<=k-1; i++)
                            {
                                spline1d.spline1ddiff(ref c, xc[i], ref s, ref ds, ref d2s);
                                if( dc[i]==0 )
                                {
                                    fiterrors = fiterrors | (double)(Math.Abs(s-yc[i]))>(double)(threshold);
                                }
                                if( dc[i]==1 )
                                {
                                    fiterrors = fiterrors | (double)(Math.Abs(ds-yc[i]))>(double)(threshold);
                                }
                                if( dc[i]==2 )
                                {
                                    fiterrors = fiterrors | (double)(Math.Abs(d2s-yc[i]))>(double)(threshold);
                                }
                            }
                        }
                    }
                }
                
                //
                // Hermite splines
                // Ability to handle one internal constraint
                //
                for(m=4; m<=8; m++)
                {
                    if( m%2!=0 )
                    {
                        continue;
                    }
                    n = 100;
                    x = new double[n];
                    y = new double[n];
                    w = new double[n];
                    xc = new double[1];
                    yc = new double[1];
                    dc = new int[1];
                    sa = 1+AP.Math.RandomReal();
                    sb = 2*AP.Math.RandomReal()-1;
                    for(i=0; i<=n-1; i++)
                    {
                        x[i] = sa*AP.Math.RandomReal()+sb;
                        y[i] = 2*AP.Math.RandomReal()-1;
                        w[i] = 1+AP.Math.RandomReal();
                    }
                    xc[0] = sa*AP.Math.RandomReal()+sb;
                    yc[0] = 2*AP.Math.RandomReal()-1;
                    dc[0] = AP.Math.RandomInteger(2);
                    spline1d.spline1dfithermitewc(ref x, ref y, ref w, n, ref xc, ref yc, ref dc, 1, m, ref info, ref c, ref rep);
                    if( info<=0 )
                    {
                        fiterrors = true;
                    }
                    else
                    {
                        
                        //
                        // Check that constraints are satisfied
                        //
                        spline1d.spline1ddiff(ref c, xc[0], ref s, ref ds, ref d2s);
                        if( dc[0]==0 )
                        {
                            fiterrors = fiterrors | (double)(Math.Abs(s-yc[0]))>(double)(threshold);
                        }
                        if( dc[0]==1 )
                        {
                            fiterrors = fiterrors | (double)(Math.Abs(ds-yc[0]))>(double)(threshold);
                        }
                        if( dc[0]==2 )
                        {
                            fiterrors = fiterrors | (double)(Math.Abs(d2s-yc[0]))>(double)(threshold);
                        }
                    }
                }
            }
            for(m=4; m<=8; m++)
            {
                for(stype=0; stype<=1; stype++)
                {
                    for(pass=1; pass<=passcount; pass++)
                    {
                        if( stype==1 & m%2!=0 )
                        {
                            continue;
                        }
                        
                        //
                        // cubic/Hermite spline fitting:
                        // * generate "template spline" C2
                        // * generate 2*N points from C2, such that result of
                        //   ideal fit should be equal to C2
                        // * fit, store in C
                        // * compare C and C2
                        //
                        sa = 1+AP.Math.RandomReal();
                        sb = 2*AP.Math.RandomReal()-1;
                        if( stype==0 )
                        {
                            x = new double[m-2];
                            y = new double[m-2];
                            for(i=0; i<=m-2-1; i++)
                            {
                                x[i] = sa*i/(m-2-1)+sb;
                                y[i] = 2*AP.Math.RandomReal()-1;
                            }
                            spline1d.spline1dbuildcubic(x, y, m-2, 1, 2*AP.Math.RandomReal()-1, 1, 2*AP.Math.RandomReal()-1, ref c2);
                        }
                        if( stype==1 )
                        {
                            x = new double[m/2];
                            y = new double[m/2];
                            d = new double[m/2];
                            for(i=0; i<=m/2-1; i++)
                            {
                                x[i] = sa*i/(m/2-1)+sb;
                                y[i] = 2*AP.Math.RandomReal()-1;
                                d[i] = 2*AP.Math.RandomReal()-1;
                            }
                            spline1d.spline1dbuildhermite(x, y, d, m/2, ref c2);
                        }
                        n = 50;
                        x = new double[2*n];
                        y = new double[2*n];
                        w = new double[2*n];
                        for(i=0; i<=n-1; i++)
                        {
                            
                            //
                            // "if i=0" and "if i=1" are needed to
                            // synchronize interval size for C2 and
                            // spline being fitted (i.e. C).
                            //
                            t = AP.Math.RandomReal();
                            x[i] = sa*AP.Math.RandomReal()+sb;
                            if( i==0 )
                            {
                                x[i] = sb;
                            }
                            if( i==1 )
                            {
                                x[i] = sa+sb;
                            }
                            v = spline1d.spline1dcalc(ref c2, x[i]);
                            y[i] = v+t;
                            w[i] = 1+AP.Math.RandomReal();
                            x[n+i] = x[i];
                            y[n+i] = v-t;
                            w[n+i] = w[i];
                        }
                        if( stype==0 )
                        {
                            spline1d.spline1dfitcubicwc(ref x, ref y, ref w, 2*n, ref xc, ref yc, ref dc, 0, m, ref info, ref c, ref rep);
                        }
                        if( stype==1 )
                        {
                            spline1d.spline1dfithermitewc(ref x, ref y, ref w, 2*n, ref xc, ref yc, ref dc, 0, m, ref info, ref c, ref rep);
                        }
                        if( info<=0 )
                        {
                            fiterrors = true;
                        }
                        else
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                v = sa*AP.Math.RandomReal()+sb;
                                fiterrors = fiterrors | (double)(Math.Abs(spline1d.spline1dcalc(ref c, v)-spline1d.spline1dcalc(ref c2, v)))>(double)(threshold);
                            }
                        }
                    }
                }
            }
            for(m=4; m<=8; m++)
            {
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // prepare points/weights
                    //
                    sa = 1+AP.Math.RandomReal();
                    sb = 2*AP.Math.RandomReal()-1;
                    n = 10+AP.Math.RandomInteger(10);
                    x = new double[n];
                    y = new double[n];
                    w = new double[n];
                    for(i=0; i<=n-1; i++)
                    {
                        x[i] = sa*AP.Math.RandomReal()+sb;
                        y[i] = 2*AP.Math.RandomReal()-1;
                        w[i] = 1;
                    }
                    
                    //
                    // Fit cubic with unity weights, without weights, then compare
                    //
                    if( m>=4 )
                    {
                        spline1d.spline1dfitcubicwc(ref x, ref y, ref w, n, ref xc, ref yc, ref dc, 0, m, ref info1, ref c, ref rep);
                        spline1d.spline1dfitcubic(ref x, ref y, n, m, ref info2, ref c2, ref rep2);
                        if( info1<=0 | info2<=0 )
                        {
                            fiterrors = true;
                        }
                        else
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                v = sa*AP.Math.RandomReal()+sb;
                                fiterrors = fiterrors | (double)(spline1d.spline1dcalc(ref c, v))!=(double)(spline1d.spline1dcalc(ref c2, v));
                                fiterrors = fiterrors | (double)(rep.taskrcond)!=(double)(rep2.taskrcond);
                                fiterrors = fiterrors | (double)(rep.rmserror)!=(double)(rep2.rmserror);
                                fiterrors = fiterrors | (double)(rep.avgerror)!=(double)(rep2.avgerror);
                                fiterrors = fiterrors | (double)(rep.avgrelerror)!=(double)(rep2.avgrelerror);
                                fiterrors = fiterrors | (double)(rep.maxerror)!=(double)(rep2.maxerror);
                            }
                        }
                    }
                    
                    //
                    // Fit Hermite with unity weights, without weights, then compare
                    //
                    if( m>=4 & m%2==0 )
                    {
                        spline1d.spline1dfithermitewc(ref x, ref y, ref w, n, ref xc, ref yc, ref dc, 0, m, ref info1, ref c, ref rep);
                        spline1d.spline1dfithermite(ref x, ref y, n, m, ref info2, ref c2, ref rep2);
                        if( info1<=0 | info2<=0 )
                        {
                            fiterrors = true;
                        }
                        else
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                v = sa*AP.Math.RandomReal()+sb;
                                fiterrors = fiterrors | (double)(spline1d.spline1dcalc(ref c, v))!=(double)(spline1d.spline1dcalc(ref c2, v));
                                fiterrors = fiterrors | (double)(rep.taskrcond)!=(double)(rep2.taskrcond);
                                fiterrors = fiterrors | (double)(rep.rmserror)!=(double)(rep2.rmserror);
                                fiterrors = fiterrors | (double)(rep.avgerror)!=(double)(rep2.avgerror);
                                fiterrors = fiterrors | (double)(rep.avgrelerror)!=(double)(rep2.avgrelerror);
                                fiterrors = fiterrors | (double)(rep.maxerror)!=(double)(rep2.maxerror);
                            }
                        }
                    }
                }
            }
            for(pass=1; pass<=passcount; pass++)
            {
                System.Diagnostics.Debug.Assert(passcount>=2, "PassCount should be 2 or greater!");
                
                //
                // solve simple task (all X[] are the same, Y[] are specially
                // calculated to ensure simple form of all types of errors)
                // and check correctness of the errors calculated by subroutines
                //
                // First pass is done with zero Y[], other passes - with random Y[].
                // It should test both ability to correctly calculate errors and
                // ability to not fail while working with zeros :)
                //
                n = 4;
                if( pass==1 )
                {
                    v1 = 0;
                    v2 = 0;
                    v = 0;
                }
                else
                {
                    v1 = AP.Math.RandomReal();
                    v2 = AP.Math.RandomReal();
                    v = 1+AP.Math.RandomReal();
                }
                x = new double[4];
                y = new double[4];
                w = new double[4];
                x[0] = 0;
                y[0] = v-v2;
                w[0] = 1;
                x[1] = 0;
                y[1] = v-v1;
                w[1] = 1;
                x[2] = 0;
                y[2] = v+v1;
                w[2] = 1;
                x[3] = 0;
                y[3] = v+v2;
                w[3] = 1;
                refrms = Math.Sqrt((AP.Math.Sqr(v1)+AP.Math.Sqr(v2))/2);
                refavg = (Math.Abs(v1)+Math.Abs(v2))/2;
                if( pass==1 )
                {
                    refavgrel = 0;
                }
                else
                {
                    refavgrel = 0.25*(Math.Abs(v2)/Math.Abs(v-v2)+Math.Abs(v1)/Math.Abs(v-v1)+Math.Abs(v1)/Math.Abs(v+v1)+Math.Abs(v2)/Math.Abs(v+v2));
                }
                refmax = Math.Max(v1, v2);
                
                //
                // Test cubic fitting
                //
                spline1d.spline1dfitcubic(ref x, ref y, 4, 4, ref info, ref c, ref rep);
                if( info<=0 )
                {
                    fiterrors = true;
                }
                else
                {
                    s = spline1d.spline1dcalc(ref c, 0);
                    fiterrors = fiterrors | (double)(Math.Abs(s-v))>(double)(threshold);
                    fiterrors = fiterrors | (double)(Math.Abs(rep.rmserror-refrms))>(double)(threshold);
                    fiterrors = fiterrors | (double)(Math.Abs(rep.avgerror-refavg))>(double)(threshold);
                    fiterrors = fiterrors | (double)(Math.Abs(rep.avgrelerror-refavgrel))>(double)(threshold);
                    fiterrors = fiterrors | (double)(Math.Abs(rep.maxerror-refmax))>(double)(threshold);
                }
                
                //
                // Test cubic fitting
                //
                spline1d.spline1dfithermite(ref x, ref y, 4, 4, ref info, ref c, ref rep);
                if( info<=0 )
                {
                    fiterrors = true;
                }
                else
                {
                    s = spline1d.spline1dcalc(ref c, 0);
                    fiterrors = fiterrors | (double)(Math.Abs(s-v))>(double)(threshold);
                    fiterrors = fiterrors | (double)(Math.Abs(rep.rmserror-refrms))>(double)(threshold);
                    fiterrors = fiterrors | (double)(Math.Abs(rep.avgerror-refavg))>(double)(threshold);
                    fiterrors = fiterrors | (double)(Math.Abs(rep.avgrelerror-refavgrel))>(double)(threshold);
                    fiterrors = fiterrors | (double)(Math.Abs(rep.maxerror-refmax))>(double)(threshold);
                }
            }
            
            //
            // report
            //
            waserrors = lserrors | cserrors | hserrors | aserrors | dserrors | cperrors | uperrors | lterrors | ierrors | fiterrors;
            if( !silent )
            {
                System.Console.Write("TESTING SPLINE INTERPOLATION");
                System.Console.WriteLine();
                
                //
                // Normal tests
                //
                System.Console.Write("LINEAR SPLINE TEST:                      ");
                if( lserrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("CUBIC SPLINE TEST:                       ");
                if( cserrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("HERMITE SPLINE TEST:                     ");
                if( hserrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("AKIMA SPLINE TEST:                       ");
                if( aserrors )
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
                System.Console.Write("COPY/SERIALIZATION TEST:                 ");
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
                System.Console.Write("INTEGRATION TEST:                        ");
                if( ierrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("FITTING TEST:                            ");
                if( fiterrors )
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
        Lipschitz constants for spline inself, first and second derivatives.
        *************************************************************************/
        private static void lconst(double a,
            double b,
            ref spline1d.spline1dinterpolant c,
            double lstep,
            ref double l0,
            ref double l1,
            ref double l2)
        {
            double t = 0;
            double vl = 0;
            double vm = 0;
            double vr = 0;
            double prevf = 0;
            double prevd = 0;
            double prevd2 = 0;
            double f = 0;
            double d = 0;
            double d2 = 0;

            l0 = 0;
            l1 = 0;
            l2 = 0;
            t = a-0.1;
            vl = spline1d.spline1dcalc(ref c, t-2*lstep);
            vm = spline1d.spline1dcalc(ref c, t-lstep);
            vr = spline1d.spline1dcalc(ref c, t);
            f = vm;
            d = (vr-vl)/(2*lstep);
            d2 = (vr-2*vm+vl)/AP.Math.Sqr(lstep);
            while( (double)(t)<=(double)(b+0.1) )
            {
                prevf = f;
                prevd = d;
                prevd2 = d2;
                vl = vm;
                vm = vr;
                vr = spline1d.spline1dcalc(ref c, t+lstep);
                f = vm;
                d = (vr-vl)/(2*lstep);
                d2 = (vr-2*vm+vl)/AP.Math.Sqr(lstep);
                l0 = Math.Max(l0, Math.Abs((f-prevf)/lstep));
                l1 = Math.Max(l1, Math.Abs((d-prevd)/lstep));
                l2 = Math.Max(l2, Math.Abs((d2-prevd2)/lstep));
                t = t+lstep;
            }
        }


        /*************************************************************************
        Unpack testing
        *************************************************************************/
        private static bool testunpack(ref spline1d.spline1dinterpolant c,
            ref double[] x)
        {
            bool result = new bool();
            int i = 0;
            int n = 0;
            double err = 0;
            double t = 0;
            double v1 = 0;
            double v2 = 0;
            int pass = 0;
            int passcount = 0;
            double[,] tbl = new double[0,0];

            passcount = 20;
            err = 0;
            spline1d.spline1dunpack(ref c, ref n, ref tbl);
            for(i=0; i<=n-2; i++)
            {
                for(pass=1; pass<=passcount; pass++)
                {
                    t = AP.Math.RandomReal()*(tbl[i,1]-tbl[i,0]);
                    v1 = tbl[i,2]+t*tbl[i,3]+AP.Math.Sqr(t)*tbl[i,4]+t*AP.Math.Sqr(t)*tbl[i,5];
                    v2 = spline1d.spline1dcalc(ref c, tbl[i,0]+t);
                    err = Math.Max(err, Math.Abs(v1-v2));
                }
            }
            for(i=0; i<=n-2; i++)
            {
                err = Math.Max(err, Math.Abs(x[i]-tbl[i,0]));
            }
            for(i=0; i<=n-2; i++)
            {
                err = Math.Max(err, Math.Abs(x[i+1]-tbl[i,1]));
            }
            result = (double)(err)<(double)(100*AP.Math.MachineEpsilon);
            return result;
        }


        /*************************************************************************
        Unset spline, i.e. initialize it with random garbage
        *************************************************************************/
        private static void unsetspline1d(ref spline1d.spline1dinterpolant c)
        {
            double[] x = new double[0];
            double[] y = new double[0];
            double[] d = new double[0];

            x = new double[2];
            y = new double[2];
            d = new double[2];
            x[0] = -1;
            y[0] = AP.Math.RandomReal();
            d[0] = AP.Math.RandomReal();
            x[1] = 1;
            y[1] = AP.Math.RandomReal();
            d[1] = AP.Math.RandomReal();
            spline1d.spline1dbuildhermite(x, y, d, 2, ref c);
        }


        /*************************************************************************
        Unsets real vector
        *************************************************************************/
        private static void unset1d(ref double[] x)
        {
            x = new double[1];
            x[0] = 2*AP.Math.RandomReal()-1;
        }


        /*************************************************************************
        Tests whether constant C is solution of 1D LLS problem
        *************************************************************************/
        private static bool is1dsolution(int n,
            ref double[] y,
            ref double[] w,
            double c)
        {
            bool result = new bool();
            int i = 0;
            double v = 0;
            double s1 = 0;
            double s2 = 0;
            double s3 = 0;
            double delta = 0;

            delta = 0.001;
            
            //
            // Test result
            //
            s1 = 0;
            for(i=0; i<=n-1; i++)
            {
                s1 = s1+AP.Math.Sqr(w[i]*(c-y[i]));
            }
            s2 = 0;
            s3 = 0;
            for(i=0; i<=n-1; i++)
            {
                s2 = s2+AP.Math.Sqr(w[i]*(c+delta-y[i]));
                s3 = s3+AP.Math.Sqr(w[i]*(c-delta-y[i]));
            }
            result = (double)(s2)>=(double)(s1) & (double)(s3)>=(double)(s1);
            return result;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testsplineinterpolationunit_test_silent()
        {
            bool result = new bool();

            result = testsplineinterpolation(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testsplineinterpolationunit_test()
        {
            bool result = new bool();

            result = testsplineinterpolation(false);
            return result;
        }
    }
}
