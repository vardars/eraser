
using System;

namespace alglib
{
    public class testodesolverunit
    {
        /*************************************************************************
        Test
        *************************************************************************/
        public static bool testodesolver(bool silent)
        {
            bool result = new bool();
            int passcount = 0;
            bool curerrors = new bool();
            bool rkckerrors = new bool();
            bool waserrors = new bool();
            double[] xtbl = new double[0];
            double[,] ytbl = new double[0,0];
            odesolver.odesolverreport rep = new odesolver.odesolverreport();
            double[] xg = new double[0];
            double[] y = new double[0];
            double h = 0;
            double eps = 0;
            int solver = 0;
            int pass = 0;
            int mynfev = 0;
            double v = 0;
            int n = 0;
            int m = 0;
            int m2 = 0;
            int i = 0;
            double err = 0;
            odesolver.odesolverstate state = new odesolver.odesolverstate();
            int i_ = 0;

            rkckerrors = false;
            waserrors = false;
            passcount = 10;
            
            //
            // simple test: just A*sin(x)+B*cos(x)
            //
            System.Diagnostics.Debug.Assert(passcount>=2);
            for(pass=0; pass<=passcount-1; pass++)
            {
                for(solver=0; solver<=0; solver++)
                {
                    
                    //
                    // prepare
                    //
                    h = 1.0E-2;
                    eps = 1.0E-5;
                    if( pass%2==0 )
                    {
                        eps = -eps;
                    }
                    y = new double[2];
                    for(i=0; i<=1; i++)
                    {
                        y[i] = 2*AP.Math.RandomReal()-1;
                    }
                    m = 2+AP.Math.RandomInteger(10);
                    xg = new double[m];
                    xg[0] = (m-1)*AP.Math.RandomReal();
                    for(i=1; i<=m-1; i++)
                    {
                        xg[i] = xg[i-1]+AP.Math.RandomReal();
                    }
                    v = 2*Math.PI/(xg[m-1]-xg[0]);
                    for(i_=0; i_<=m-1;i_++)
                    {
                        xg[i_] = v*xg[i_];
                    }
                    if( (double)(AP.Math.RandomReal())>(double)(0.5) )
                    {
                        for(i_=0; i_<=m-1;i_++)
                        {
                            xg[i_] = -1*xg[i_];
                        }
                    }
                    mynfev = 0;
                    
                    //
                    // choose solver
                    //
                    if( solver==0 )
                    {
                        odesolver.odesolverrkck(ref y, 2, ref xg, m, eps, h, ref state);
                    }
                    
                    //
                    // solve
                    //
                    while( odesolver.odesolveriteration(ref state) )
                    {
                        state.dy[0] = state.y[1];
                        state.dy[1] = -state.y[0];
                        mynfev = mynfev+1;
                    }
                    odesolver.odesolverresults(ref state, ref m2, ref xtbl, ref ytbl, ref rep);
                    
                    //
                    // check results
                    //
                    curerrors = false;
                    if( rep.terminationtype<=0 )
                    {
                        curerrors = true;
                    }
                    else
                    {
                        curerrors = curerrors | m2!=m;
                        err = 0;
                        for(i=0; i<=m-1; i++)
                        {
                            err = Math.Max(err, Math.Abs(ytbl[i,0]-(+(y[0]*Math.Cos(xtbl[i]-xtbl[0]))+y[1]*Math.Sin(xtbl[i]-xtbl[0]))));
                            err = Math.Max(err, Math.Abs(ytbl[i,1]-(-(y[0]*Math.Sin(xtbl[i]-xtbl[0]))+y[1]*Math.Cos(xtbl[i]-xtbl[0]))));
                        }
                        curerrors = curerrors | (double)(err)>(double)(10*Math.Abs(eps));
                        curerrors = curerrors | mynfev!=rep.nfev;
                    }
                    if( solver==0 )
                    {
                        rkckerrors = rkckerrors | curerrors;
                    }
                }
            }
            
            //
            // another test:
            //
            //     y(0)   = 0
            //     dy/dx  = f(x,y)
            //     f(x,y) = 0,   x<1
            //              x-1, x>=1
            //
            // with BOTH absolute and fractional tolerances.
            // Starting from zero will be real challenge for
            // fractional tolerance.
            //
            System.Diagnostics.Debug.Assert(passcount>=2);
            for(pass=0; pass<=passcount-1; pass++)
            {
                h = 1.0E-4;
                eps = 1.0E-4;
                if( pass%2==0 )
                {
                    eps = -eps;
                }
                y = new double[1];
                y[0] = 0;
                m = 21;
                xg = new double[m];
                for(i=0; i<=m-1; i++)
                {
                    xg[i] = (double)(2*i)/((double)(m-1));
                }
                mynfev = 0;
                odesolver.odesolverrkck(ref y, 1, ref xg, m, eps, h, ref state);
                while( odesolver.odesolveriteration(ref state) )
                {
                    state.dy[0] = Math.Max(state.x-1, 0);
                    mynfev = mynfev+1;
                }
                odesolver.odesolverresults(ref state, ref m2, ref xtbl, ref ytbl, ref rep);
                if( rep.terminationtype<=0 )
                {
                    rkckerrors = true;
                }
                else
                {
                    rkckerrors = rkckerrors | m2!=m;
                    err = 0;
                    for(i=0; i<=m-1; i++)
                    {
                        err = Math.Max(err, Math.Abs(ytbl[i,0]-AP.Math.Sqr(Math.Max(xg[i]-1, 0))/2));
                    }
                    rkckerrors = rkckerrors | (double)(err)>(double)(Math.Abs(eps));
                    rkckerrors = rkckerrors | mynfev!=rep.nfev;
                }
            }
            
            //
            // end
            //
            waserrors = rkckerrors;
            if( !silent )
            {
                System.Console.Write("TESTING ODE SOLVER");
                System.Console.WriteLine();
                System.Console.Write("* RK CASH-KARP:                           ");
                if( rkckerrors )
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
        Unsets real matrix
        *************************************************************************/
        private static void unset2d(ref double[,] x)
        {
            x = new double[1, 1];
            x[0,0] = 2*AP.Math.RandomReal()-1;
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
        Unsets report
        *************************************************************************/
        private static void unsetrep(ref odesolver.odesolverreport rep)
        {
            rep.nfev = 0;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testodesolverunit_test_silent()
        {
            bool result = new bool();

            result = testodesolver(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testodesolverunit_test()
        {
            bool result = new bool();

            result = testodesolver(false);
            return result;
        }
    }
}
