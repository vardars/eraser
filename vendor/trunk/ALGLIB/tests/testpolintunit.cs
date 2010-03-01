
using System;

namespace alglib
{
    public class testpolintunit
    {
        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testpolint(bool silent)
        {
            bool result = new bool();
            bool waserrors = new bool();
            bool interrors = new bool();
            bool fiterrors = new bool();
            double threshold = 0;
            double[] x = new double[0];
            double[] y = new double[0];
            double[] w = new double[0];
            double[] x2 = new double[0];
            double[] y2 = new double[0];
            double[] w2 = new double[0];
            double[] xfull = new double[0];
            double[] yfull = new double[0];
            double a = 0;
            double b = 0;
            double t = 0;
            int i = 0;
            int k = 0;
            double[] xc = new double[0];
            double[] yc = new double[0];
            int[] dc = new int[0];
            int info = 0;
            int info2 = 0;
            double v = 0;
            double v0 = 0;
            double v1 = 0;
            double v2 = 0;
            double s = 0;
            double xmin = 0;
            double xmax = 0;
            double refrms = 0;
            double refavg = 0;
            double refavgrel = 0;
            double refmax = 0;
            ratint.barycentricinterpolant p = new ratint.barycentricinterpolant();
            ratint.barycentricinterpolant p1 = new ratint.barycentricinterpolant();
            ratint.barycentricinterpolant p2 = new ratint.barycentricinterpolant();
            polint.polynomialfitreport rep = new polint.polynomialfitreport();
            polint.polynomialfitreport rep2 = new polint.polynomialfitreport();
            int n = 0;
            int m = 0;
            int maxn = 0;
            int pass = 0;
            int passcount = 0;

            waserrors = false;
            interrors = false;
            fiterrors = false;
            maxn = 5;
            passcount = 20;
            threshold = 1.0E8*AP.Math.MachineEpsilon;
            
            //
            // Test equidistant interpolation
            //
            for(pass=1; pass<=passcount; pass++)
            {
                for(n=1; n<=maxn; n++)
                {
                    
                    //
                    // prepare task:
                    // * equidistant points
                    // * random Y
                    // * T in [A,B] or near (within 10% of its width)
                    //
                    do
                    {
                        a = 2*AP.Math.RandomReal()-1;
                        b = 2*AP.Math.RandomReal()-1;
                    }
                    while( (double)(Math.Abs(a-b))<=(double)(0.2) );
                    t = a+(1.2*AP.Math.RandomReal()-0.1)*(b-a);
                    taskgen.taskgenint1dequidist(a, b, n, ref x, ref y);
                    
                    //
                    // test "fast" equidistant interpolation (no barycentric model)
                    //
                    interrors = interrors | (double)(Math.Abs(polint.polynomialcalceqdist(a, b, ref y, n, t)-internalpolint(ref x, y, n, t)))>(double)(threshold);
                    
                    //
                    // test "slow" equidistant interpolation (create barycentric model)
                    //
                    brcunset(ref p);
                    polint.polynomialbuild(ref x, ref y, n, ref p);
                    interrors = interrors | (double)(Math.Abs(ratint.barycentriccalc(ref p, t)-internalpolint(ref x, y, n, t)))>(double)(threshold);
                    
                    //
                    // test "fast" interpolation (create "fast" barycentric model)
                    //
                    brcunset(ref p);
                    polint.polynomialbuildeqdist(a, b, ref y, n, ref p);
                    interrors = interrors | (double)(Math.Abs(ratint.barycentriccalc(ref p, t)-internalpolint(ref x, y, n, t)))>(double)(threshold);
                }
            }
            
            //
            // Test Chebyshev-1 interpolation
            //
            for(pass=1; pass<=passcount; pass++)
            {
                for(n=1; n<=maxn; n++)
                {
                    
                    //
                    // prepare task:
                    // * equidistant points
                    // * random Y
                    // * T in [A,B] or near (within 10% of its width)
                    //
                    do
                    {
                        a = 2*AP.Math.RandomReal()-1;
                        b = 2*AP.Math.RandomReal()-1;
                    }
                    while( (double)(Math.Abs(a-b))<=(double)(0.2) );
                    t = a+(1.2*AP.Math.RandomReal()-0.1)*(b-a);
                    taskgen.taskgenint1dcheb1(a, b, n, ref x, ref y);
                    
                    //
                    // test "fast" interpolation (no barycentric model)
                    //
                    interrors = interrors | (double)(Math.Abs(polint.polynomialcalccheb1(a, b, ref y, n, t)-internalpolint(ref x, y, n, t)))>(double)(threshold);
                    
                    //
                    // test "slow" interpolation (create barycentric model)
                    //
                    brcunset(ref p);
                    polint.polynomialbuild(ref x, ref y, n, ref p);
                    interrors = interrors | (double)(Math.Abs(ratint.barycentriccalc(ref p, t)-internalpolint(ref x, y, n, t)))>(double)(threshold);
                    
                    //
                    // test "fast" interpolation (create "fast" barycentric model)
                    //
                    brcunset(ref p);
                    polint.polynomialbuildcheb1(a, b, ref y, n, ref p);
                    interrors = interrors | (double)(Math.Abs(ratint.barycentriccalc(ref p, t)-internalpolint(ref x, y, n, t)))>(double)(threshold);
                }
            }
            
            //
            // Test Chebyshev-2 interpolation
            //
            for(pass=1; pass<=passcount; pass++)
            {
                for(n=1; n<=maxn; n++)
                {
                    
                    //
                    // prepare task:
                    // * equidistant points
                    // * random Y
                    // * T in [A,B] or near (within 10% of its width)
                    //
                    do
                    {
                        a = 2*AP.Math.RandomReal()-1;
                        b = 2*AP.Math.RandomReal()-1;
                    }
                    while( (double)(Math.Abs(a-b))<=(double)(0.2) );
                    t = a+(1.2*AP.Math.RandomReal()-0.1)*(b-a);
                    taskgen.taskgenint1dcheb2(a, b, n, ref x, ref y);
                    
                    //
                    // test "fast" interpolation (no barycentric model)
                    //
                    interrors = interrors | (double)(Math.Abs(polint.polynomialcalccheb2(a, b, ref y, n, t)-internalpolint(ref x, y, n, t)))>(double)(threshold);
                    
                    //
                    // test "slow" interpolation (create barycentric model)
                    //
                    brcunset(ref p);
                    polint.polynomialbuild(ref x, ref y, n, ref p);
                    interrors = interrors | (double)(Math.Abs(ratint.barycentriccalc(ref p, t)-internalpolint(ref x, y, n, t)))>(double)(threshold);
                    
                    //
                    // test "fast" interpolation (create "fast" barycentric model)
                    //
                    brcunset(ref p);
                    polint.polynomialbuildcheb2(a, b, ref y, n, ref p);
                    interrors = interrors | (double)(Math.Abs(ratint.barycentriccalc(ref p, t)-internalpolint(ref x, y, n, t)))>(double)(threshold);
                }
            }
            
            //
            // crash-test: ability to solve tasks which will overflow/underflow
            // weights with straightforward implementation
            //
            for(n=1; n<=20; n++)
            {
                a = -(0.1*AP.Math.MaxRealNumber);
                b = +(0.1*AP.Math.MaxRealNumber);
                taskgen.taskgenint1dequidist(a, b, n, ref x, ref y);
                polint.polynomialbuild(ref x, ref y, n, ref p);
                for(i=0; i<=n-1; i++)
                {
                    interrors = interrors | (double)(p.w[i])==(double)(0);
                }
            }
            
            //
            // Test rational fitting:
            //
            for(pass=1; pass<=passcount; pass++)
            {
                for(n=1; n<=maxn; n++)
                {
                    
                    //
                    // N=M+K fitting (i.e. interpolation)
                    //
                    for(k=0; k<=n-1; k++)
                    {
                        taskgen.taskgenint1d(-1, 1, n, ref xfull, ref yfull);
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
                            x[i] = xfull[i];
                            y[i] = yfull[i];
                            w[i] = 1+AP.Math.RandomReal();
                        }
                        for(i=0; i<=k-1; i++)
                        {
                            xc[i] = xfull[n-k+i];
                            yc[i] = yfull[n-k+i];
                            dc[i] = 0;
                        }
                        polint.polynomialfitwc(x, y, ref w, n-k, xc, yc, ref dc, k, n, ref info, ref p1, ref rep);
                        if( info<=0 )
                        {
                            fiterrors = true;
                        }
                        else
                        {
                            for(i=0; i<=n-k-1; i++)
                            {
                                fiterrors = fiterrors | (double)(Math.Abs(ratint.barycentriccalc(ref p1, x[i])-y[i]))>(double)(threshold);
                            }
                            for(i=0; i<=k-1; i++)
                            {
                                fiterrors = fiterrors | (double)(Math.Abs(ratint.barycentriccalc(ref p1, xc[i])-yc[i]))>(double)(threshold);
                            }
                        }
                    }
                    
                    //
                    // Testing constraints on derivatives.
                    // Special tasks which will always have solution:
                    // 1. P(0)=YC[0]
                    // 2. P(0)=YC[0], P'(0)=YC[1]
                    //
                    if( n>1 )
                    {
                        for(m=3; m<=5; m++)
                        {
                            for(k=1; k<=2; k++)
                            {
                                taskgen.taskgenint1d(-1, 1, n, ref x, ref y);
                                w = new double[n];
                                xc = new double[2];
                                yc = new double[2];
                                dc = new int[2];
                                for(i=0; i<=n-1; i++)
                                {
                                    w[i] = 1+AP.Math.RandomReal();
                                }
                                xc[0] = 0;
                                yc[0] = 2*AP.Math.RandomReal()-1;
                                dc[0] = 0;
                                xc[1] = 0;
                                yc[1] = 2*AP.Math.RandomReal()-1;
                                dc[1] = 1;
                                polint.polynomialfitwc(x, y, ref w, n, xc, yc, ref dc, k, m, ref info, ref p1, ref rep);
                                if( info<=0 )
                                {
                                    fiterrors = true;
                                }
                                else
                                {
                                    ratint.barycentricdiff1(ref p1, 0.0, ref v0, ref v1);
                                    fiterrors = fiterrors | (double)(Math.Abs(v0-yc[0]))>(double)(threshold);
                                    if( k==2 )
                                    {
                                        fiterrors = fiterrors | (double)(Math.Abs(v1-yc[1]))>(double)(threshold);
                                    }
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
                    xmin = 0;
                    xmax = 2*Math.PI;
                    for(i=0; i<=n-1; i++)
                    {
                        x2[i] = 2*Math.PI*AP.Math.RandomReal();
                        y2[i] = Math.Sin(x2[i]);
                        w2[i] = 1;
                    }
                    x = new double[m];
                    y = new double[m];
                    for(i=0; i<=m-1; i++)
                    {
                        x[i] = xmin+(xmax-xmin)*i/(m-1);
                        y[i] = Math.Sin(x[i]);
                    }
                    polint.polynomialbuild(ref x, ref y, m, ref p1);
                    polint.polynomialfitwc(x2, y2, ref w2, n, xc, yc, ref dc, 0, m, ref info, ref p2, ref rep);
                    if( info<=0 )
                    {
                        fiterrors = true;
                    }
                    else
                    {
                        
                        //
                        // calculate P1 (interpolant) RMS error, compare with P2 error
                        //
                        v1 = 0;
                        v2 = 0;
                        for(i=0; i<=n-1; i++)
                        {
                            v1 = v1+AP.Math.Sqr(ratint.barycentriccalc(ref p1, x2[i])-y2[i]);
                            v2 = v2+AP.Math.Sqr(ratint.barycentriccalc(ref p2, x2[i])-y2[i]);
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
                    polint.polynomialfitwc(x, y, ref w, n, xc, yc, ref dc, 0, m, ref info, ref p1, ref rep);
                    polint.polynomialfit(ref x, ref y, n, m, ref info2, ref p2, ref rep2);
                    if( info<=0 | info2<=0 )
                    {
                        fiterrors = true;
                    }
                    else
                    {
                        
                        //
                        // calculate P1 (interpolant), compare with P2 error
                        // compare RMS errors
                        //
                        t = 2*AP.Math.RandomReal()-1;
                        v1 = ratint.barycentriccalc(ref p1, t);
                        v2 = ratint.barycentriccalc(ref p2, t);
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
                polint.polynomialfit(ref x, ref y, 4, 1, ref info, ref p, ref rep);
                if( info<=0 )
                {
                    fiterrors = true;
                }
                else
                {
                    s = ratint.barycentriccalc(ref p, 0);
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
            waserrors = interrors | fiterrors;
            if( !silent )
            {
                System.Console.Write("TESTING POLYNOMIAL INTERPOLATION AND FITTING");
                System.Console.WriteLine();
                
                //
                // Normal tests
                //
                System.Console.Write("INTERPOLATION TEST:                      ");
                if( interrors )
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


        private static double internalpolint(ref double[] x,
            double[] f,
            int n,
            double t)
        {
            double result = 0;
            int i = 0;
            int j = 0;

            f = (double[])f.Clone();

            n = n-1;
            for(j=0; j<=n-1; j++)
            {
                for(i=j+1; i<=n; i++)
                {
                    f[i] = ((t-x[j])*f[i]-(t-x[i])*f[j])/(x[i]-x[j]);
                }
            }
            result = f[n];
            return result;
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
        Silent unit test
        *************************************************************************/
        public static bool testpolintunit_test_silent()
        {
            bool result = new bool();

            result = testpolint(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testpolintunit_test()
        {
            bool result = new bool();

            result = testpolint(false);
            return result;
        }
    }
}
