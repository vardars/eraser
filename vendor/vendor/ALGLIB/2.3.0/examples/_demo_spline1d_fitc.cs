using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        double[] x = new double[0];
        double[] y = new double[0];
        double[] w = new double[0];
        double[] xc = new double[0];
        double[] yc = new double[0];
        int[] dc = new int[0];
        int n = 0;
        int i = 0;
        int info = 0;
        spline1d.spline1dinterpolant s = new spline1d.spline1dinterpolant();
        double t = 0;
        spline1d.spline1dfitreport rep = new spline1d.spline1dfitreport();

        
        //
        // Fitting by constrained Hermite spline
        //
        System.Console.Write("FITTING BY CONSTRAINED HERMITE SPLINE");
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.Write("F(x)=sin(x)      function being fitted");
        System.Console.WriteLine();
        System.Console.Write("[0, pi]          interval");
        System.Console.WriteLine();
        System.Console.Write("M=6              number of basis functions to use");
        System.Console.WriteLine();
        System.Console.Write("S(0)=0           first constraint");
        System.Console.WriteLine();
        System.Console.Write("S(pi)=0          second constraint");
        System.Console.WriteLine();
        System.Console.Write("N=100            number of points to fit");
        System.Console.WriteLine();
        
        //
        // Create and fit:
        // * X  contains points
        // * Y  contains values
        // * W  contains weights
        // * XC contains constraints locations
        // * YC contains constraints values
        // * DC contains derivative indexes (0 = constrained function value)
        //
        n = 100;
        x = new double[n];
        y = new double[n];
        w = new double[n];
        for(i=0; i<=n-1; i++)
        {
            x[i] = Math.PI*i/(n-1);
            y[i] = Math.Sin(x[i]);
            w[i] = 1;
        }
        xc = new double[2];
        yc = new double[2];
        dc = new int[2];
        xc[0] = 0;
        yc[0] = 0;
        dc[0] = 0;
        xc[0] = Math.PI;
        yc[0] = 0;
        dc[0] = 0;
        spline1d.spline1dfithermitewc(ref x, ref y, ref w, n, ref xc, ref yc, ref dc, 2, 6, ref info, ref s, ref rep);
        
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
            System.Console.Write("S(0) = S(pi) = 0 (exactly)");
            System.Console.WriteLine();
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