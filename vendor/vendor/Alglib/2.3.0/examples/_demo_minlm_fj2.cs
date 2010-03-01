using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        minlm.lmstate state = new minlm.lmstate();
        minlm.lmreport rep = new minlm.lmreport();
        int i = 0;
        double[] s = new double[0];
        double[] x = new double[0];
        double[] y = new double[0];
        double fi = 0;
        int n = 0;
        int m = 0;

        
        //
        // Example of solving polynomial approximation task using FJ scheme.
        //
        // Data points:
        //     xi are random numbers from [-1,+1],
        //
        // Function being fitted:
        //     yi = exp(xi) - sin(xi) - x^3/3
        //
        // Function being minimized:
        //     F(a,b,c) =
        //         (a + b*x0 + c*x0^2 - y0)^2 +
        //         (a + b*x1 + c*x1^2 - y1)^2 + ...
        //
        n = 3;
        s = new double[n];
        for(i=0; i<=n-1; i++)
        {
            s[i] = AP.Math.RandomReal()-0.5;
        }
        m = 100;
        x = new double[m];
        y = new double[m];
        for(i=0; i<=m-1; i++)
        {
            x[i] = (double)(2*i)/((double)(m-1))-1;
            y[i] = Math.Exp(x[i])-Math.Sin(x[i])-x[i]*x[i]*x[i]/3;
        }
        
        //
        // Now S stores starting point, X and Y store points being fitted.
        //
        minlm.minlmfj(n, m, ref s, 0.0, 0.001, 0, ref state);
        while( minlm.minlmiteration(ref state) )
        {
            if( state.needf )
            {
                state.f = 0;
            }
            for(i=0; i<=m-1; i++)
            {
                
                //
                // "a" is stored in State.X[0]
                // "b" - State.X[1]
                // "c" - State.X[2]
                //
                fi = state.x[0]+state.x[1]*x[i]+state.x[2]*AP.Math.Sqr(x[i])-y[i];
                if( state.needf )
                {
                    
                    //
                    // F is equal to sum of fi squared.
                    //
                    state.f = state.f+AP.Math.Sqr(fi);
                }
                if( state.needfij )
                {
                    
                    //
                    // Fi
                    //
                    state.fi[i] = fi;
                    
                    //
                    // dFi/da
                    //
                    state.j[i,0] = 1;
                    
                    //
                    // dFi/db
                    //
                    state.j[i,1] = x[i];
                    
                    //
                    // dFi/dc
                    //
                    state.j[i,2] = AP.Math.Sqr(x[i]);
                }
            }
        }
        minlm.minlmresults(ref state, ref s, ref rep);
        
        //
        // output results
        //
        System.Console.Write("A = ");
        System.Console.Write("{0,4:F2}",s[0]);
        System.Console.WriteLine();
        System.Console.Write("B = ");
        System.Console.Write("{0,4:F2}",s[1]);
        System.Console.WriteLine();
        System.Console.Write("C = ");
        System.Console.Write("{0,4:F2}",s[2]);
        System.Console.WriteLine();
        System.Console.Write("TerminationType = ");
        System.Console.Write("{0,0:d}",rep.terminationtype);
        System.Console.Write(" (should be 2 - stopping when step is small enough)");
        System.Console.WriteLine();
        return 0;
    }
}