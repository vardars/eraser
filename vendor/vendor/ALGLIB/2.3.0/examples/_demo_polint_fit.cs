using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        int m = 0;
        int n = 0;
        double[] x = new double[0];
        double[] y = new double[0];
        double[] w = new double[0];
        double[] xc = new double[0];
        double[] yc = new double[0];
        int[] dc = new int[0];
        polint.polynomialfitreport rep = new polint.polynomialfitreport();
        int info = 0;
        ratint.barycentricinterpolant p = new ratint.barycentricinterpolant();
        int i = 0;
        int j = 0;
        double a = 0;
        double b = 0;
        double v = 0;
        double dv = 0;

        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.Write("Fitting exp(2*x) at [-1,+1] by polinomial");
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.Write("Fit type             rms.err max.err    p(0)   dp(0)");
        System.Console.WriteLine();
        
        //
        // Prepare points
        //
        m = 5;
        a = -1;
        b = +1;
        n = 1000;
        x = new double[n];
        y = new double[n];
        w = new double[n];
        for(i=0; i<=n-1; i++)
        {
            x[i] = a+(b-a)*i/(n-1);
            y[i] = Math.Exp(2*x[i]);
            w[i] = 1.0;
        }
        
        //
        // Fitting:
        // a) f(x)=exp(2*x) at [-1,+1]
        // b) by 5th degree polynomial
        // c) without constraints
        //
        polint.polynomialfit(ref x, ref y, n, m, ref info, ref p, ref rep);
        ratint.barycentricdiff1(ref p, 0.0, ref v, ref dv);
        System.Console.Write("Unconstrained        ");
        System.Console.Write("{0,7:F4}",rep.rmserror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",rep.maxerror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",v);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",dv);
        System.Console.WriteLine();
        
        //
        // Fitting:
        // a) f(x)=exp(2*x) at [-1,+1]
        // b) by 5th degree polynomial
        // c) constrained: p(0)=1
        //
        xc = new double[1];
        yc = new double[1];
        dc = new int[1];
        xc[0] = 0;
        yc[0] = 1;
        dc[0] = 0;
        polint.polynomialfitwc(x, y, ref w, n, xc, yc, ref dc, 1, m, ref info, ref p, ref rep);
        ratint.barycentricdiff1(ref p, 0.0, ref v, ref dv);
        System.Console.Write("Constrained, p(0)=1  ");
        System.Console.Write("{0,7:F4}",rep.rmserror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",rep.maxerror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",v);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",dv);
        System.Console.WriteLine();
        
        //
        // Fitting:
        // a) f(x)=exp(2*x) at [-1,+1]
        // b) by 5th degree polynomial
        // c) constrained: dp(0)=2
        //
        xc = new double[1];
        yc = new double[1];
        dc = new int[1];
        xc[0] = 0;
        yc[0] = 2;
        dc[0] = 1;
        polint.polynomialfitwc(x, y, ref w, n, xc, yc, ref dc, 1, m, ref info, ref p, ref rep);
        ratint.barycentricdiff1(ref p, 0.0, ref v, ref dv);
        System.Console.Write("Constrained, dp(0)=2 ");
        System.Console.Write("{0,7:F4}",rep.rmserror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",rep.maxerror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",v);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",dv);
        System.Console.WriteLine();
        
        //
        // Fitting:
        // a) f(x)=exp(2*x) at [-1,+1]
        // b) by 5th degree polynomial
        // c) constrained: p(0)=1, dp(0)=2
        //
        xc = new double[2];
        yc = new double[2];
        dc = new int[2];
        xc[0] = 0;
        yc[0] = 1;
        dc[0] = 0;
        xc[1] = 0;
        yc[1] = 2;
        dc[1] = 1;
        polint.polynomialfitwc(x, y, ref w, n, xc, yc, ref dc, 2, m, ref info, ref p, ref rep);
        ratint.barycentricdiff1(ref p, 0.0, ref v, ref dv);
        System.Console.Write("Constrained, both    ");
        System.Console.Write("{0,7:F4}",rep.rmserror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",rep.maxerror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",v);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",dv);
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.WriteLine();
        return 0;
    }
}