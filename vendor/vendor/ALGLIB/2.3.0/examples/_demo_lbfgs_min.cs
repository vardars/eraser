using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        int n = 0;
        int m = 0;
        lbfgs.lbfgsstate state = new lbfgs.lbfgsstate();
        lbfgs.lbfgsreport rep = new lbfgs.lbfgsreport();
        double[] s = new double[0];
        double x = 0;
        double y = 0;

        
        //
        // Function minimized:
        //     F = exp(x-1) + exp(1-x) + (y-x)^2
        // N = 2 - task dimension
        // M = 1 - build tank-1 model
        //
        n = 2;
        m = 1;
        s = new double[2];
        s[0] = AP.Math.RandomReal()-0.5;
        s[1] = AP.Math.RandomReal()-0.5;
        lbfgs.minlbfgs(n, m, ref s, 0.0, 0.0, 0.0001, 0, 0, ref state);
        while( lbfgs.minlbfgsiteration(ref state) )
        {
            x = state.x[0];
            y = state.x[1];
            state.f = Math.Exp(x-1)+Math.Exp(1-x)+AP.Math.Sqr(y-x);
            state.g[0] = Math.Exp(x-1)-Math.Exp(1-x)+2*(x-y);
            state.g[1] = 2*(y-x);
        }
        lbfgs.minlbfgsresults(ref state, ref s, ref rep);
        
        //
        // output results
        //
        System.Console.Write("F = exp(x-1) + exp(1-x) + (y-x)^2");
        System.Console.WriteLine();
        System.Console.Write("X = ");
        System.Console.Write("{0,4:F2}",s[0]);
        System.Console.Write(" (should be 1.00)");
        System.Console.WriteLine();
        System.Console.Write("Y = ");
        System.Console.Write("{0,4:F2}",s[1]);
        System.Console.Write(" (should be 1.00)");
        System.Console.WriteLine();
        return 0;
    }
}