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
        ratint.barycentricinterpolant p = new ratint.barycentricinterpolant();
        double v = 0;
        double dv = 0;
        double d2v = 0;
        double err = 0;
        double maxerr = 0;

        
        //
        // Demonstration
        //
        System.Console.Write("POLYNOMIAL INTERPOLATION");
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.Write("F(x)=sin(x), [0, pi]");
        System.Console.WriteLine();
        System.Console.Write("Second degree polynomial is used");
        System.Console.WriteLine();
        System.Console.WriteLine();
        
        //
        // Create polynomial interpolant
        //
        n = 3;
        x = new double[n];
        y = new double[n];
        for(i=0; i<=n-1; i++)
        {
            x[i] = Math.PI*i/(n-1);
            y[i] = Math.Sin(x[i]);
        }
        polint.polynomialbuild(ref x, ref y, n, ref p);
        
        //
        // Output results
        //
        ratint.barycentricdiff2(ref p, 0, ref v, ref dv, ref d2v);
        System.Console.Write("                 P(x)    F(x) ");
        System.Console.WriteLine();
        System.Console.Write("function       ");
        System.Console.Write("{0,6:F3}",ratint.barycentriccalc(ref p, 0));
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
        System.Console.WriteLine();
        System.Console.WriteLine();
        return 0;
    }
}