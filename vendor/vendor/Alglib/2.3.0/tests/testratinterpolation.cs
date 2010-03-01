
using System;

namespace alglib
{
    public class testratinterpolation
    {
        public static bool testri(bool silent)
        {
            bool result = new bool();
            bool waserrors = new bool();
            bool bcerrors = new bool();
            bool nperrors = new bool();
            bool fiterrors = new bool();
            double threshold = 0;
            double lipschitztol = 0;
            int maxn = 0;
            int passcount = 0;
            ratint.barycentricinterpolant b1 = new ratint.barycentricinterpolant();
            ratint.barycentricinterpolant b2 = new ratint.barycentricinterpolant();
            double[] x = new double[0];
            double[] x2 = new double[0];
            double[] y = new double[0];
            double[] y2 = new double[0];
            double[] w = new double[0];
            double[] w2 = new double[0];
            double[] xc = new double[0];
            double[] yc = new double[0];
            int[] dc = new int[0];
            double h = 0;
            double s1 = 0;
            double s2 = 0;
            bool bsame = new bool();
            int n = 0;
            int m = 0;
            int n2 = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int d = 0;
            int pass = 0;
            double err = 0;
            double maxerr = 0;
            double t = 0;
            double a = 0;
            double b = 0;
            double s = 0;
            double v = 0;
            double v0 = 0;
            double v1 = 0;
            double v2 = 0;
            double v3 = 0;
            double d0 = 0;
            double d1 = 0;
            double d2 = 0;
            int info = 0;
            int info2 = 0;
            double xmin = 0;
            double xmax = 0;
            double refrms = 0;
            double refavg = 0;
            double refavgrel = 0;
            double refmax = 0;
            double[] ra = new double[0];
            double[] ra2 = new double[0];
            int ralen = 0;
            ratint.barycentricfitreport rep = new ratint.barycentricfitreport();
            ratint.barycentricfitreport rep2 = new ratint.barycentricfitreport();
            ratint.barycentricinterpolant b3 = new ratint.barycentricinterpolant();
            ratint.barycentricinterpolant b4 = new ratint.barycentricinterpolant();
            int i_ = 0;

            nperrors = false;
            bcerrors = false;
            fiterrors = false;
            waserrors = false;
            
            //
            // PassCount        number of repeated passes
            // Threshold        error tolerance
            // LipschitzTol     Lipschitz constant increase allowed
            //                  when calculating constant on a twice denser grid
            //
            passcount = 5;
            maxn = 15;
            threshold = 1000000*AP.Math.MachineEpsilon;
            lipschitztol = 1.3;
            
            //
            // Basic barycentric functions
            //
            for(n=1; n<=10; n++)
            {
                
                //
                // randomized tests
                //
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // generate weights from polynomial interpolation
                    //
                    v0 = 1+0.4*AP.Math.RandomReal()-0.2;
                    v1 = 2*AP.Math.RandomReal()-1;
                    v2 = 2*AP.Math.RandomReal()-1;
                    v3 = 2*AP.Math.RandomReal()-1;
                    x = new double[n];
                    y = new double[n];
                    w = new double[n];
                    for(i=0; i<=n-1; i++)
                    {
                        if( n==1 )
                        {
                            x[i] = 0;
                        }
                        else
                        {
                            x[i] = v0*Math.Cos(i*Math.PI/(n-1));
                        }
                        y[i] = Math.Sin(v1*x[i])+Math.Cos(v2*x[i])+Math.Exp(v3*x[i]);
                    }
                    for(j=0; j<=n-1; j++)
                    {
                        w[j] = 1;
                        for(k=0; k<=n-1; k++)
                        {
                            if( k!=j )
                            {
                                w[j] = w[j]/(x[j]-x[k]);
                            }
                        }
                    }
                    ratint.barycentricbuildxyw(ref x, ref y, ref w, n, ref b1);
                    
                    //
                    // unpack, then pack again and compare
                    //
                    brcunset(ref b2);
                    ratint.barycentricunpack(ref b1, ref n2, ref x2, ref y2, ref w2);
                    bcerrors = bcerrors | n2!=n;
                    ratint.barycentricbuildxyw(ref x2, ref y2, ref w2, n2, ref b2);
                    t = 2*AP.Math.RandomReal()-1;
                    bcerrors = bcerrors | (double)(Math.Abs(ratint.barycentriccalc(ref b1, t)-ratint.barycentriccalc(ref b2, t)))>(double)(threshold);
                    
                    //
                    // serialize, unserialize, compare
                    //
                    brcunset(ref b2);
                    ratint.barycentricserialize(ref b1, ref ra, ref ralen);
                    ra2 = new double[ralen];
                    for(i_=0; i_<=ralen-1;i_++)
                    {
                        ra2[i_] = ra[i_];
                    }
                    ratint.barycentricunserialize(ref ra2, ref b2);
                    t = 2*AP.Math.RandomReal()-1;
                    bcerrors = bcerrors | (double)(Math.Abs(ratint.barycentriccalc(ref b1, t)-ratint.barycentriccalc(ref b2, t)))>(double)(threshold);
                    
                    //
                    // copy, compare
                    //
                    brcunset(ref b2);
                    ratint.barycentriccopy(ref b1, ref b2);
                    t = 2*AP.Math.RandomReal()-1;
                    bcerrors = bcerrors | (double)(Math.Abs(ratint.barycentriccalc(ref b1, t)-ratint.barycentriccalc(ref b2, t)))>(double)(threshold);
                    
                    //
                    // test interpolation properties
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        
                        //
                        // test interpolation at nodes
                        //
                        bcerrors = bcerrors | (double)(Math.Abs(ratint.barycentriccalc(ref b1, x[i])-y[i]))>(double)(threshold*Math.Abs(y[i]));
                        
                        //
                        // compare with polynomial interpolation
                        //
                        t = 2*AP.Math.RandomReal()-1;
                        poldiff2(ref x, y, n, t, ref v0, ref v1, ref v2);
                        bcerrors = bcerrors | (double)(Math.Abs(ratint.barycentriccalc(ref b1, t)-v0))>(double)(threshold*Math.Max(Math.Abs(v0), 1));
                        
                        //
                        // test continuity between nodes
                        // calculate Lipschitz constant on two grids -
                        // dense and even more dense. If Lipschitz constant
                        // on a denser grid is significantly increased,
                        // continuity test is failed
                        //
                        t = 3.0;
                        k = 100;
                        s1 = 0;
                        for(j=0; j<=k-1; j++)
                        {
                            v1 = x[i]+(t-x[i])*j/k;
                            v2 = x[i]+(t-x[i])*(j+1)/k;
                            s1 = Math.Max(s1, Math.Abs(ratint.barycentriccalc(ref b1, v2)-ratint.barycentriccalc(ref b1, v1))/Math.Abs(v2-v1));
                        }
                        k = 2*k;
                        s2 = 0;
                        for(j=0; j<=k-1; j++)
                        {
                            v1 = x[i]+(t-x[i])*j/k;
                            v2 = x[i]+(t-x[i])*(j+1)/k;
                            s2 = Math.Max(s2, Math.Abs(ratint.barycentriccalc(ref b1, v2)-ratint.barycentriccalc(ref b1, v1))/Math.Abs(v2-v1));
                        }
                        bcerrors = bcerrors | (double)(s2)>(double)(lipschitztol*s1) & (double)(s1)>(double)(threshold*k);
                    }
                    
                    //
                    // test differentiation properties
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        t = 2*AP.Math.RandomReal()-1;
                        poldiff2(ref x, y, n, t, ref v0, ref v1, ref v2);
                        d0 = 0;
                        d1 = 0;
                        d2 = 0;
                        ratint.barycentricdiff1(ref b1, t, ref d0, ref d1);
                        bcerrors = bcerrors | (double)(Math.Abs(v0-d0))>(double)(threshold*Math.Max(Math.Abs(v0), 1));
                        bcerrors = bcerrors | (double)(Math.Abs(v1-d1))>(double)(threshold*Math.Max(Math.Abs(v1), 1));
                        d0 = 0;
                        d1 = 0;
                        d2 = 0;
                        ratint.barycentricdiff2(ref b1, t, ref d0, ref d1, ref d2);
                        bcerrors = bcerrors | (double)(Math.Abs(v0-d0))>(double)(threshold*Math.Max(Math.Abs(v0), 1));
                        bcerrors = bcerrors | (double)(Math.Abs(v1-d1))>(double)(threshold*Math.Max(Math.Abs(v1), 1));
                        bcerrors = bcerrors | (double)(Math.Abs(v2-d2))>(double)(Math.Sqrt(threshold)*Math.Max(Math.Abs(v2), 1));
                    }
                    
                    //
                    // test linear translation
                    //
                    t = 2*AP.Math.RandomReal()-1;
                    a = 2*AP.Math.RandomReal()-1;
                    b = 2*AP.Math.RandomReal()-1;
                    brcunset(ref b2);
                    ratint.barycentriccopy(ref b1, ref b2);
                    ratint.barycentriclintransx(ref b2, a, b);
                    bcerrors = bcerrors | (double)(Math.Abs(ratint.barycentriccalc(ref b1, a*t+b)-ratint.barycentriccalc(ref b2, t)))>(double)(threshold);
                    a = 0;
                    b = 2*AP.Math.RandomReal()-1;
                    brcunset(ref b2);
                    ratint.barycentriccopy(ref b1, ref b2);
                    ratint.barycentriclintransx(ref b2, a, b);
                    bcerrors = bcerrors | (double)(Math.Abs(ratint.barycentriccalc(ref b1, a*t+b)-ratint.barycentriccalc(ref b2, t)))>(double)(threshold);
                    a = 2*AP.Math.RandomReal()-1;
                    b = 2*AP.Math.RandomReal()-1;
                    brcunset(ref b2);
                    ratint.barycentriccopy(ref b1, ref b2);
                    ratint.barycentriclintransy(ref b2, a, b);
                    bcerrors = bcerrors | (double)(Math.Abs(a*ratint.barycentriccalc(ref b1, t)+b-ratint.barycentriccalc(ref b2, t)))>(double)(threshold);
                }
            }
            for(pass=0; pass<=3; pass++)
            {
                
                //
                // Crash-test: small numbers, large numbers
                //
                x = new double[4];
                y = new double[4];
                w = new double[4];
                h = 1;
                if( pass%2==0 )
                {
                    h = 100*AP.Math.MinRealNumber;
                }
                if( pass%2==1 )
                {
                    h = 0.01*AP.Math.MaxRealNumber;
                }
                x[0] = 0*h;
                x[1] = 1*h;
                x[2] = 2*h;
                x[3] = 3*h;
                y[0] = 0*h;
                y[1] = 1*h;
                y[2] = 2*h;
                y[3] = 3*h;
                w[0] = -(1/(x[1]-x[0]));
                w[1] = +(1*(1/(x[1]-x[0])+1/(x[2]-x[1])));
                w[2] = -(1*(1/(x[2]-x[1])+1/(x[3]-x[2])));
                w[3] = +(1/(x[3]-x[2]));
                if( pass/2==0 )
                {
                    v0 = 0;
                }
                if( pass/2==1 )
                {
                    v0 = 0.6*h;
                }
                ratint.barycentricbuildxyw(ref x, ref y, ref w, 4, ref b1);
                t = ratint.barycentriccalc(ref b1, v0);
                d0 = 0;
                d1 = 0;
                d2 = 0;
                ratint.barycentricdiff1(ref b1, v0, ref d0, ref d1);
                bcerrors = bcerrors | (double)(Math.Abs(t-v0))>(double)(threshold*v0);
                bcerrors = bcerrors | (double)(Math.Abs(d0-v0))>(double)(threshold*v0);
                bcerrors = bcerrors | (double)(Math.Abs(d1-1))>(double)(1000*threshold);
            }
            
            //
            // crash test: large abscissas, small argument
            //
            // test for errors in D0 is not very strict
            // because renormalization used in Diff1()
            // destroys part of precision.
            //
            x = new double[4];
            y = new double[4];
            w = new double[4];
            h = 0.01*AP.Math.MaxRealNumber;
            x[0] = 0*h;
            x[1] = 1*h;
            x[2] = 2*h;
            x[3] = 3*h;
            y[0] = 0*h;
            y[1] = 1*h;
            y[2] = 2*h;
            y[3] = 3*h;
            w[0] = -(1/(x[1]-x[0]));
            w[1] = +(1*(1/(x[1]-x[0])+1/(x[2]-x[1])));
            w[2] = -(1*(1/(x[2]-x[1])+1/(x[3]-x[2])));
            w[3] = +(1/(x[3]-x[2]));
            v0 = 100*AP.Math.MinRealNumber;
            ratint.barycentricbuildxyw(ref x, ref y, ref w, 4, ref b1);
            t = ratint.barycentriccalc(ref b1, v0);
            d0 = 0;
            d1 = 0;
            d2 = 0;
            ratint.barycentricdiff1(ref b1, v0, ref d0, ref d1);
            bcerrors = bcerrors | (double)(Math.Abs(t))>(double)(v0*(1+threshold));
            bcerrors = bcerrors | (double)(Math.Abs(d0))>(double)(v0*(1+threshold));
            bcerrors = bcerrors | (double)(Math.Abs(d1-1))>(double)(1000*threshold);
            
            //
            // crash test: test safe barycentric formula
            //
            x = new double[4];
            y = new double[4];
            w = new double[4];
            h = 2*AP.Math.MinRealNumber;
            x[0] = 0*h;
            x[1] = 1*h;
            x[2] = 2*h;
            x[3] = 3*h;
            y[0] = 0*h;
            y[1] = 1*h;
            y[2] = 2*h;
            y[3] = 3*h;
            w[0] = -(1/(x[1]-x[0]));
            w[1] = +(1*(1/(x[1]-x[0])+1/(x[2]-x[1])));
            w[2] = -(1*(1/(x[2]-x[1])+1/(x[3]-x[2])));
            w[3] = +(1/(x[3]-x[2]));
            v0 = AP.Math.MinRealNumber;
            ratint.barycentricbuildxyw(ref x, ref y, ref w, 4, ref b1);
            t = ratint.barycentriccalc(ref b1, v0);
            bcerrors = bcerrors | (double)(Math.Abs(t-v0)/v0)>(double)(threshold);
            
            //
            // Testing "No Poles" interpolation
            //
            maxerr = 0;
            for(pass=1; pass<=passcount-1; pass++)
            {
                x = new double[1];
                y = new double[1];
                x[0] = 2*AP.Math.RandomReal()-1;
                y[0] = 2*AP.Math.RandomReal()-1;
                ratint.barycentricbuildfloaterhormann(ref x, ref y, 1, 1, ref b1);
                maxerr = Math.Max(maxerr, Math.Abs(ratint.barycentriccalc(ref b1, 2*AP.Math.RandomReal()-1)-y[0]));
            }
            for(n=2; n<=10; n++)
            {
                
                //
                // compare interpolant built by subroutine
                // with interpolant built by hands
                //
                x = new double[n];
                y = new double[n];
                w = new double[n];
                w2 = new double[n];
                
                //
                // D=1, non-equidistant nodes
                //
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // Initialize X, Y, W
                    //
                    a = -1-1*AP.Math.RandomReal();
                    b = +1+1*AP.Math.RandomReal();
                    for(i=0; i<=n-1; i++)
                    {
                        x[i] = Math.Atan((b-a)*i/(n-1)+a);
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        y[i] = 2*AP.Math.RandomReal()-1;
                    }
                    w[0] = -(1/(x[1]-x[0]));
                    s = 1;
                    for(i=1; i<=n-2; i++)
                    {
                        w[i] = s*(1/(x[i]-x[i-1])+1/(x[i+1]-x[i]));
                        s = -s;
                    }
                    w[n-1] = s/(x[n-1]-x[n-2]);
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
                            t = w[i];
                            w[i] = w[k];
                            w[k] = t;
                        }
                    }
                    
                    //
                    // Build and test
                    //
                    ratint.barycentricbuildfloaterhormann(ref x, ref y, n, 1, ref b1);
                    ratint.barycentricbuildxyw(ref x, ref y, ref w, n, ref b2);
                    for(i=1; i<=2*n; i++)
                    {
                        t = a+(b-a)*AP.Math.RandomReal();
                        maxerr = Math.Max(maxerr, Math.Abs(ratint.barycentriccalc(ref b1, t)-ratint.barycentriccalc(ref b2, t)));
                    }
                }
                
                //
                // D = 0, 1, 2. Equidistant nodes.
                //
                for(d=0; d<=2; d++)
                {
                    for(pass=1; pass<=passcount; pass++)
                    {
                        
                        //
                        // Skip incorrect (N,D) pairs
                        //
                        if( n<2*d )
                        {
                            continue;
                        }
                        
                        //
                        // Initialize X, Y, W
                        //
                        a = -1-1*AP.Math.RandomReal();
                        b = +1+1*AP.Math.RandomReal();
                        for(i=0; i<=n-1; i++)
                        {
                            x[i] = (b-a)*i/(n-1)+a;
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            y[i] = 2*AP.Math.RandomReal()-1;
                        }
                        s = 1;
                        if( d==0 )
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                w[i] = s;
                                s = -s;
                            }
                        }
                        if( d==1 )
                        {
                            w[0] = -s;
                            for(i=1; i<=n-2; i++)
                            {
                                w[i] = 2*s;
                                s = -s;
                            }
                            w[n-1] = s;
                        }
                        if( d==2 )
                        {
                            w[0] = s;
                            w[1] = -(3*s);
                            for(i=2; i<=n-3; i++)
                            {
                                w[i] = 4*s;
                                s = -s;
                            }
                            w[n-2] = 3*s;
                            w[n-1] = -s;
                        }
                        
                        //
                        // Mix
                        //
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
                                t = w[i];
                                w[i] = w[k];
                                w[k] = t;
                            }
                        }
                        
                        //
                        // Build and test
                        //
                        ratint.barycentricbuildfloaterhormann(ref x, ref y, n, d, ref b1);
                        ratint.barycentricbuildxyw(ref x, ref y, ref w, n, ref b2);
                        for(i=1; i<=2*n; i++)
                        {
                            t = a+(b-a)*AP.Math.RandomReal();
                            maxerr = Math.Max(maxerr, Math.Abs(ratint.barycentriccalc(ref b1, t)-ratint.barycentriccalc(ref b2, t)));
                        }
                    }
                }
            }
            if( (double)(maxerr)>(double)(threshold) )
            {
                nperrors = true;
            }
            
            //
            // Test rational fitting:
            //
            for(pass=1; pass<=passcount; pass++)
            {
                for(n=2; n<=maxn; n++)
                {
                    
                    //
                    // N=M+K fitting (i.e. interpolation)
                    //
                    for(k=0; k<=n-1; k++)
                    {
                        x = new double[n-k];
                        y = new double[n-k];
                        w = new double[n-k];
                        if( k>0 )
                        {
                            xc = new double[k];
                            yc = new double[k];
                            dc = new int[k];
                        }
                        for(i=0; i<=n-k-1; i++)
                        {
                            x[i] = (double)(i)/((double)(n-1));
                            y[i] = 2*AP.Math.RandomReal()-1;
                            w[i] = 1+AP.Math.RandomReal();
                        }
                        for(i=0; i<=k-1; i++)
                        {
                            xc[i] = ((double)(n-k+i))/((double)(n-1));
                            yc[i] = 2*AP.Math.RandomReal()-1;
                            dc[i] = 0;
                        }
                        ratint.barycentricfitfloaterhormannwc(ref x, ref y, ref w, n-k, ref xc, ref yc, ref dc, k, n, ref info, ref b1, ref rep);
                        if( info<=0 )
                        {
                            fiterrors = true;
                        }
                        else
                        {
                            for(i=0; i<=n-k-1; i++)
                            {
                                fiterrors = fiterrors | (double)(Math.Abs(ratint.barycentriccalc(ref b1, x[i])-y[i]))>(double)(threshold);
                            }
                            for(i=0; i<=k-1; i++)
                            {
                                fiterrors = fiterrors | (double)(Math.Abs(ratint.barycentriccalc(ref b1, xc[i])-yc[i]))>(double)(threshold);
                            }
                        }
                    }
                    
                    //
                    // Testing constraints on derivatives:
                    // * several M's are tried
                    // * several K's are tried - 1, 2.
                    // * constraints at the ends of the interval
                    //
                    for(m=3; m<=5; m++)
                    {
                        for(k=1; k<=2; k++)
                        {
                            x = new double[n];
                            y = new double[n];
                            w = new double[n];
                            xc = new double[2];
                            yc = new double[2];
                            dc = new int[2];
                            for(i=0; i<=n-1; i++)
                            {
                                x[i] = 2*AP.Math.RandomReal()-1;
                                y[i] = 2*AP.Math.RandomReal()-1;
                                w[i] = 1+AP.Math.RandomReal();
                            }
                            xc[0] = -1;
                            yc[0] = 2*AP.Math.RandomReal()-1;
                            dc[0] = 0;
                            xc[1] = +1;
                            yc[1] = 2*AP.Math.RandomReal()-1;
                            dc[1] = 0;
                            ratint.barycentricfitfloaterhormannwc(ref x, ref y, ref w, n, ref xc, ref yc, ref dc, k, m, ref info, ref b1, ref rep);
                            if( info<=0 )
                            {
                                fiterrors = true;
                            }
                            else
                            {
                                for(i=0; i<=k-1; i++)
                                {
                                    ratint.barycentricdiff1(ref b1, xc[i], ref v0, ref v1);
                                    fiterrors = fiterrors | (double)(Math.Abs(v0-yc[i]))>(double)(threshold);
                                }
                            }
                        }
                    }
                }
            }
            for(m=2; m<=8; m++)
            {
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // General fitting
                    //
                    // interpolating function through M nodes should have
                    // greater RMS error than fitting it through the same M nodes
                    //
                    n = 100;
                    x2 = new double[n];
                    y2 = new double[n];
                    w2 = new double[n];
                    xmin = AP.Math.MaxRealNumber;
                    xmax = -AP.Math.MaxRealNumber;
                    for(i=0; i<=n-1; i++)
                    {
                        x2[i] = 2*Math.PI*AP.Math.RandomReal();
                        y2[i] = Math.Sin(x2[i]);
                        w2[i] = 1;
                        xmin = Math.Min(xmin, x2[i]);
                        xmax = Math.Max(xmax, x2[i]);
                    }
                    x = new double[m];
                    y = new double[m];
                    for(i=0; i<=m-1; i++)
                    {
                        x[i] = xmin+(xmax-xmin)*i/(m-1);
                        y[i] = Math.Sin(x[i]);
                    }
                    ratint.barycentricbuildfloaterhormann(ref x, ref y, m, 3, ref b1);
                    ratint.barycentricfitfloaterhormannwc(ref x2, ref y2, ref w2, n, ref xc, ref yc, ref dc, 0, m, ref info, ref b2, ref rep);
                    if( info<=0 )
                    {
                        fiterrors = true;
                    }
                    else
                    {
                        
                        //
                        // calculate B1 (interpolant) RMS error, compare with B2 error
                        //
                        v1 = 0;
                        v2 = 0;
                        for(i=0; i<=n-1; i++)
                        {
                            v1 = v1+AP.Math.Sqr(ratint.barycentriccalc(ref b1, x2[i])-y2[i]);
                            v2 = v2+AP.Math.Sqr(ratint.barycentriccalc(ref b2, x2[i])-y2[i]);
                        }
                        v1 = Math.Sqrt(v1/n);
                        v2 = Math.Sqrt(v2/n);
                        fiterrors = fiterrors | (double)(v2)>(double)(v1);
                        fiterrors = fiterrors | (double)(Math.Abs(v2-rep.rmserror))>(double)(threshold);
                    }
                    
                    //
                    // compare weighted and non-weighted
                    //
                    n = 20;
                    x = new double[n];
                    y = new double[n];
                    w = new double[n];
                    for(i=0; i<=n-1; i++)
                    {
                        x[i] = 2*AP.Math.RandomReal()-1;
                        y[i] = 2*AP.Math.RandomReal()-1;
                        w[i] = 1;
                    }
                    ratint.barycentricfitfloaterhormannwc(ref x, ref y, ref w, n, ref xc, ref yc, ref dc, 0, m, ref info, ref b1, ref rep);
                    ratint.barycentricfitfloaterhormann(ref x, ref y, n, m, ref info2, ref b2, ref rep2);
                    if( info<=0 | info2<=0 )
                    {
                        fiterrors = true;
                    }
                    else
                    {
                        
                        //
                        // calculate B1 (interpolant), compare with B2
                        // compare RMS errors
                        //
                        t = 2*AP.Math.RandomReal()-1;
                        v1 = ratint.barycentriccalc(ref b1, t);
                        v2 = ratint.barycentriccalc(ref b2, t);
                        fiterrors = fiterrors | (double)(v2)!=(double)(v1);
                        fiterrors = fiterrors | (double)(rep.rmserror)!=(double)(rep2.rmserror);
                        fiterrors = fiterrors | (double)(rep.avgerror)!=(double)(rep2.avgerror);
                        fiterrors = fiterrors | (double)(rep.avgrelerror)!=(double)(rep2.avgrelerror);
                        fiterrors = fiterrors | (double)(rep.maxerror)!=(double)(rep2.maxerror);
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
                // Test errors correctness
                //
                ratint.barycentricfitfloaterhormann(ref x, ref y, 4, 2, ref info, ref b1, ref rep);
                if( info<=0 )
                {
                    fiterrors = true;
                }
                else
                {
                    s = ratint.barycentriccalc(ref b1, 0);
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
            waserrors = bcerrors | nperrors | fiterrors;
            if( !silent )
            {
                System.Console.Write("TESTING RATIONAL INTERPOLATION");
                System.Console.WriteLine();
                System.Console.Write("BASIC BARYCENTRIC FUNCTIONS:             ");
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
                System.Console.Write("FLOATER-HORMANN:                         ");
                if( nperrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("RATIONAL FITTING:                        ");
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


        private static void poldiff2(ref double[] x,
            double[] f,
            int n,
            double t,
            ref double p,
            ref double dp,
            ref double d2p)
        {
            int m = 0;
            int i = 0;
            double[] df = new double[0];
            double[] d2f = new double[0];

            f = (double[])f.Clone();

            n = n-1;
            df = new double[n+1];
            d2f = new double[n+1];
            for(i=0; i<=n; i++)
            {
                d2f[i] = 0;
                df[i] = 0;
            }
            for(m=1; m<=n; m++)
            {
                for(i=0; i<=n-m; i++)
                {
                    d2f[i] = ((t-x[i+m])*d2f[i]+(x[i]-t)*d2f[i+1]+2*df[i]-2*df[i+1])/(x[i]-x[i+m]);
                    df[i] = ((t-x[i+m])*df[i]+f[i]+(x[i]-t)*df[i+1]-f[i+1])/(x[i]-x[i+m]);
                    f[i] = ((t-x[i+m])*f[i]+(x[i]-t)*f[i+1])/(x[i]-x[i+m]);
                }
            }
            p = f[0];
            dp = df[0];
            d2p = d2f[0];
        }


        private static void brcunset(ref ratint.barycentricinterpolant b)
        {
            double[] x = new double[0];
            double[] y = new double[0];
            double[] w = new double[0];

            x = new double[1];
            y = new double[1];
            w = new double[1];
            x[0] = 0;
            y[0] = 0;
            w[0] = 1;
            ratint.barycentricbuildxyw(ref x, ref y, ref w, 1, ref b);
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
        public static bool testratinterpolation_test_silent()
        {
            bool result = new bool();

            result = testri(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testratinterpolation_test()
        {
            bool result = new bool();

            result = testri(false);
            return result;
        }
    }
}
