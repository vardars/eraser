using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        minlm.lmstate state = new minlm.lmstate();
        minlm.lmreport rep = new minlm.lmreport();
        double[] s = new double[0];
        double x = 0;
        double y = 0;

        
        //
        // Example of solving simple task using FGJ scheme.
        //
        // Function minimized:
        //     F = (x-2*y)^2 + (x-2)^2 + (y-1)^2
        // exact solution is (2,1).
        //
        s = new double[2];
        s[0] = AP.Math.RandomReal()-0.5;
        s[1] = AP.Math.RandomReal()-0.5;
        minlm.minlmfgj(2, 3, ref s, 0.0, 0.001, 0, ref state);
        while( minlm.minlmiteration(ref state) )
        {
            x = state.x[0];
            y = state.x[1];
            if( state.needf )
            {
                state.f = AP.Math.Sqr(x-2*y)+AP.Math.Sqr(x-2)+AP.Math.Sqr(y-1);
            }
            if( state.needfg )
            {
                state.f = AP.Math.Sqr(x-2*y)+AP.Math.Sqr(x-2)+AP.Math.Sqr(y-1);
                state.g[0] = 2*(x-2*y)+2*(x-2)+0;
                state.g[1] = -(4*(x-2*y))+0+2*(y-1);
            }
            if( state.needfij )
            {
                state.fi[0] = x-2*y;
                state.fi[1] = x-2;
                state.fi[2] = y-1;
                state.j[0,0] = 1;
                state.j[0,1] = -2;
                state.j[1,0] = 1;
                state.j[1,1] = 0;
                state.j[2,0] = 0;
                state.j[2,1] = 1;
            }
        }
        minlm.minlmresults(ref state, ref s, ref rep);
        
        //
        // output results
        //
        System.Console.Write("X = ");
        System.Console.Write("{0,4:F2}",s[0]);
        System.Console.Write(" (correct value - 2.00)");
        System.Console.WriteLine();
        System.Console.Write("Y = ");
        System.Console.Write("{0,4:F2}",s[1]);
        System.Console.Write(" (correct value - 1.00)");
        System.Console.WriteLine();
        System.Console.Write("TerminationType = ");
        System.Console.Write("{0,0:d}",rep.terminationtype);
        System.Console.Write(" (should be 2 - stopping when step is small enough)");
        System.Console.WriteLine();
        System.Console.Write("NFunc = ");
        System.Console.Write("{0,0:d}",rep.nfunc);
        System.Console.WriteLine();
        System.Console.Write("NJac  = ");
        System.Console.Write("{0,0:d}",rep.njac);
        System.Console.WriteLine();
        System.Console.Write("NGrad = ");
        System.Console.Write("{0,0:d}",rep.ngrad);
        System.Console.WriteLine();
        System.Console.Write("NHess = ");
        System.Console.Write("{0,0:d}",rep.nhess);
        System.Console.WriteLine();
        return 0;
    }
}