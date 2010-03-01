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
        int info = 0;
        spline1d.spline1dinterpolant s = new spline1d.spline1dinterpolant();
        double t = 0;
        spline1d.spline1dfitreport rep = new spline1d.spline1dfitreport();

        
        //
        // Fitting by unconstrained natural cubic spline
        //
        System.Console.Write("FITTING BY UNCONSTRAINED NATURAL CUBIC SPLINE");
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.Write("F(x)=sin(x)      function being fitted");
        System.Console.WriteLine();
        System.Console.Write("[0, pi]          interval");
        System.Console.WriteLine();
        System.Console.Write("M=4              number of basis functions to use");
        System.Console.WriteLine();
        System.Console.Write("N=100            number of points to fit");
        System.Console.WriteLine();
        
        //
        // Create and fit
        //
        n = 100;
        x = new double[n];
        y = new double[n];
        for(i=0; i<=n-1; i++)
        {
            x[i] = Math.PI*i/(n-1);
            y[i] = Math.Sin(x[i]);
        }
        spline1d.spline1dfitcubic(ref x, ref y, n, 4, ref info, ref s, ref rep);
        
        //
        // Output results
        //
        if( info>0 )
        {
            System.Console.WriteLine();
            System.Console.Write("OK, we have finished");
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.Write("     x   F(x)   S(x)  Error");
            System.Console.WriteLine();
            t = 0;
            while( (double)(t)<(double)(0.999999*Math.PI) )
            {
                System.Console.Write("{0,6:F3}",t);
                System.Console.Write(" ");
                System.Console.Write("{0,6:F3}",Math.Sin(t));
                System.Console.Write(" ");
                System.Console.Write("{0,6:F3}",spline1d.spline1dcalc(ref s, t));
                System.Console.Write(" ");
                System.Console.Write("{0,6:F3}",Math.Abs(spline1d.spline1dcalc(ref s, t)-Math.Sin(t)));
                System.Console.WriteLine();
                t = Math.Min(Math.PI, t+0.25);
            }
            System.Console.Write("{0,6:F3}",t);
            System.Console.Write(" ");
            System.Console.Write("{0,6:F3}",Math.Sin(t));
            System.Console.Write(" ");
            System.Console.Write("{0,6:F3}",spline1d.spline1dcalc(ref s, t));
            System.Console.Write(" ");
            System.Console.Write("{0,6:F3}",Math.Abs(spline1d.spline1dcalc(ref s, t)-Math.Sin(t)));
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.Write("rms error is ");
            System.Console.Write("{0,6:F3}",rep.rmserror);
            System.Console.WriteLine();
            System.Console.Write("max error is ");
            System.Console.Write("{0,6:F3}",rep.maxerror);
            System.Console.WriteLine();
        }
        else
        {
            System.Console.WriteLine();
            System.Console.Write("Something wrong, Info=");
            System.Console.Write("{0,0:d}",info);
        }
        return 0;
    }
}