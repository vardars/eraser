using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        double[] x = new double[0];
        double[] y = new double[0];
        double[,] ytbl = new double[0,0];
        double eps = 0;
        double h = 0;
        int m = 0;
        int i = 0;
        odesolver.odesolverstate state = new odesolver.odesolverstate();
        odesolver.odesolverreport rep = new odesolver.odesolverreport();

        
        //
        // ODESolver unit is used to solve simple ODE:
        // y' = y, y(0) = 1.
        //
        // Its solution is well known in academic circles :)
        //
        // No intermediate values are calculated,
        // just starting and final points.
        //
        y = new double[1];
        y[0] = 1;
        x = new double[2];
        x[0] = 0;
        x[1] = 1;
        eps = 1.0E-4;
        h = 0.01;
        odesolver.odesolverrkck(ref y, 1, ref x, 2, eps, h, ref state);
        while( odesolver.odesolveriteration(ref state) )
        {
            state.dy[0] = state.y[0];
        }
        odesolver.odesolverresults(ref state, ref m, ref x, ref ytbl, ref rep);
        System.Console.Write("    X  Y(X)");
        System.Console.WriteLine();
        for(i=0; i<=m-1; i++)
        {
            System.Console.Write("{0,5:F3}",x[i]);
            System.Console.Write(" ");
            System.Console.Write("{0,5:F3}",ytbl[i,0]);
            System.Console.WriteLine();
        }
        return 0;
    }
}