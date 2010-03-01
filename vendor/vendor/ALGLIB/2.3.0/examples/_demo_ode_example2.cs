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
        // y'' = -y, y(0) = 0, y'(0)=1.
        //
        // This ODE may be written as first-order system:
        // y' =  z
        // z' = -y
        //
        // Its solution is well known in academic circles :)
        //
        // Three intermediate values are calculated,
        // plus starting and final points.
        //
        y = new double[2];
        y[0] = 0;
        y[1] = 1;
        x = new double[5];
        x[0] = Math.PI*0/4;
        x[1] = Math.PI*1/4;
        x[2] = Math.PI*2/4;
        x[3] = Math.PI*3/4;
        x[4] = Math.PI*4/4;
        eps = 1.0E-8;
        h = 0.01;
        odesolver.odesolverrkck(ref y, 2, ref x, 5, eps, h, ref state);
        while( odesolver.odesolveriteration(ref state) )
        {
            state.dy[0] = state.y[1];
            state.dy[1] = -state.y[0];
        }
        odesolver.odesolverresults(ref state, ref m, ref x, ref ytbl, ref rep);
        System.Console.Write("     X   Y(X)     Error");
        System.Console.WriteLine();
        for(i=0; i<=m-1; i++)
        {
            System.Console.Write("{0,6:F3}",x[i]);
            System.Console.Write(" ");
            System.Console.Write("{0,6:F3}",ytbl[i,0]);
            System.Console.Write("  ");
            System.Console.Write("{0,8:E1}",Math.Abs(ytbl[i,0]-Math.Sin(x[i])));
            System.Console.WriteLine();
        }
        return 0;
    }
}