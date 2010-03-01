using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        double[] x = new double[0];
        double[] y = new double[0];
        int n = 0;
        int i = 0;
        double t = 0;
        spline1d.spline1dinterpolant s = new spline1d.spline1dinterpolant();
        double v = 0;
        double dv = 0;
        double d2v = 0;
        double err = 0;
        double maxerr = 0;

        
        //
        // Demonstration of Spline1DCalc(), Spline1DDiff(), Spline1DIntegrate()
        //
        System.Console.Write("DEMONSTRATION OF Spline1DCalc(), Spline1DDiff(), Spline1DIntegrate()");
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.Write("F(x)=sin(x), [0, pi]");
        System.Console.WriteLine();
        System.Console.Write("Natural cubic spline with 3 nodes is used");
        System.Console.WriteLine();
        System.Console.WriteLine();
        
        //
        // Create spline
        //
        n = 3;
        x = new double[n];
        y = new double[n];
        for(i=0; i<=n-1; i++)
        {
            x[i] = Math.PI*i/(n-1);
            y[i] = Math.Sin(x[i]);
        }
        spline1d.spline1dbuildcubic(x, y, n, 2, 0.0, 2, 0.0, ref s);
        
        //
        // Output results
        //
        spline1d.spline1ddiff(ref s, 0, ref v, ref dv, ref d2v);
        System.Console.Write("                 S(x)    F(x) ");
        System.Console.WriteLine();
        System.Console.Write("function       ");
        System.Console.Write("{0,6:F3}",spline1d.spline1dcalc(ref s, 0));
        System.Console.Write("  ");
        System.Console.Write("{0,6:F3}",0);
        System.Console.Write(" ");
        System.Console.WriteLine();
        System.Console.Write("d/dx(0)        ");
        System.Console.Write("{0,6:F3}",dv);
        System.Console.Write("  ");
        System.Console.Write("{0,6:F3}",1);
        System.Console.Write(" ");
        System.Console.WriteLine();
        System.Console.Write("d2/dx2(0)      ");
        System.Console.Write("{0,6:F3}",d2v);
        System.Console.Write("  ");
        System.Console.Write("{0,6:F3}",0);
        System.Console.Write(" ");
        System.Console.WriteLine();
        System.Console.Write("integral(0,pi) ");
        System.Console.Write("{0,6:F3}",spline1d.spline1dintegrate(ref s, Math.PI));
        System.Console.Write("  ");
        System.Console.Write("{0,6:F3}",2);
        System.Console.Write(" ");
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.WriteLine();
        return 0;
    }
}