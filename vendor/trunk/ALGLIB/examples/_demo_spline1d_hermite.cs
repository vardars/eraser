using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        double[] x = new double[0];
        double[] y = new double[0];
        double[] d = new double[0];
        int n = 0;
        int i = 0;
        double t = 0;
        spline1d.spline1dinterpolant s = new spline1d.spline1dinterpolant();
        double err = 0;
        double maxerr = 0;

        
        //
        // Interpolation by natural Cubic spline.
        //
        System.Console.Write("INTERPOLATION BY HERMITE SPLINE");
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.Write("F(x)=sin(x), [0, pi], 3 nodes");
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.Write("     x   F(x)   S(x)  Error");
        System.Console.WriteLine();
        
        //
        // Create spline
        //
        n = 3;
        x = new double[n];
        y = new double[n];
        d = new double[n];
        for(i=0; i<=n-1; i++)
        {
            x[i] = Math.PI*i/(n-1);
            y[i] = Math.Sin(x[i]);
            d[i] = Math.Cos(x[i]);
        }
        spline1d.spline1dbuildhermite(x, y, d, n, ref s);
        
        //
        // Output results
        //
        t = 0;
        maxerr = 0;
        while( (double)(t)<(double)(0.999999*Math.PI) )
        {
            err = Math.Abs(spline1d.spline1dcalc(ref s, t)-Math.Sin(t));
            maxerr = Math.Max(err, maxerr);
            System.Console.Write("{0,6:F3}",t);
            System.Console.Write(" ");
            System.Console.Write("{0,6:F3}",Math.Sin(t));
            System.Console.Write(" ");
            System.Console.Write("{0,6:F3}",spline1d.spline1dcalc(ref s, t));
            System.Console.Write(" ");
            System.Console.Write("{0,6:F3}",err);
            System.Console.WriteLine();
            t = Math.Min(Math.PI, t+0.25);
        }
        err = Math.Abs(spline1d.spline1dcalc(ref s, Math.PI)-Math.Sin(Math.PI));
        maxerr = Math.Max(err, maxerr);
        System.Console.Write("{0,6:F3}",Math.PI);
        System.Console.Write(" ");
        System.Console.Write("{0,6:F3}",Math.Sin(Math.PI));
        System.Console.Write(" ");
        System.Console.Write("{0,6:F3}",spline1d.spline1dcalc(ref s, Math.PI));
        System.Console.Write(" ");
        System.Console.Write("{0,6:F3}",err);
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.Write("max|error| = ");
        System.Console.Write("{0,0:F3}",maxerr);
        System.Console.WriteLine();
        System.Console.Write("Try other demos (spline1d_linear, spline1d_cubic) and compare errors...");
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.WriteLine();
        return 0;
    }
}