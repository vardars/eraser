using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        int m = 0;
        int n = 0;
        double[] y = new double[0];
        double[,] fmatrix = new double[0,0];
        double[,] cmatrix = new double[0,0];
        lsfit.lsfitreport rep = new lsfit.lsfitreport();
        int info = 0;
        double[] c = new double[0];
        int i = 0;
        int j = 0;
        double x = 0;
        double a = 0;
        double b = 0;

        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.Write("Fitting tan(x) by third degree polynomial");
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.Write("Fit type             rms.err max.err    p(0)   dp(0)");
        System.Console.WriteLine();
        
        //
        // Fitting tan(x) at [0, 0.4*pi] by third degree polynomial:
        // a) without constraints
        // b) constrained at x=0: p(0)=0
        // c) constrained at x=0: p'(0)=1
        // c) constrained at x=0: p(0)=0, p'(0)=1
        //
        m = 4;
        n = 100;
        a = 0;
        b = 0.4*Math.PI;
        
        //
        // Prepare task matrix
        //
        y = new double[n];
        fmatrix = new double[n, m];
        for(i=0; i<=n-1; i++)
        {
            x = a+(b-a)*i/(n-1);
            y[i] = Math.Tan(x);
            fmatrix[i,0] = 1.0;
            for(j=1; j<=m-1; j++)
            {
                fmatrix[i,j] = x*fmatrix[i,j-1];
            }
        }
        
        //
        // Solve unconstrained task
        //
        lsfit.lsfitlinear(ref y, ref fmatrix, n, m, ref info, ref c, ref rep);
        System.Console.Write("Unconstrained        ");
        System.Console.Write("{0,7:F4}",rep.rmserror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",rep.maxerror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",c[0]);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",c[1]);
        System.Console.WriteLine();
        
        //
        // Solve constrained task, p(0)=0
        // Prepare constraints matrix:
        // * first M columns store values of basis functions at X=0
        // * last column stores zero (desired value at X=0)
        //
        cmatrix = new double[1, m+1];
        cmatrix[0,0] = 1;
        for(i=1; i<=m-1; i++)
        {
            cmatrix[0,i] = 0;
        }
        cmatrix[0,m] = 0;
        lsfit.lsfitlinearc(y, ref fmatrix, ref cmatrix, n, m, 1, ref info, ref c, ref rep);
        System.Console.Write("Constrained, p(0)=0  ");
        System.Console.Write("{0,7:F4}",rep.rmserror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",rep.maxerror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",c[0]);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",c[1]);
        System.Console.WriteLine();
        
        //
        // Solve constrained task, p'(0)=0
        // Prepare constraints matrix:
        // * first M columns store derivatives of basis functions at X=0
        // * last column stores 1.0 (desired derivative at X=0)
        //
        cmatrix = new double[1, m+1];
        for(i=0; i<=m-1; i++)
        {
            cmatrix[0,i] = 0;
        }
        cmatrix[0,1] = 1;
        cmatrix[0,m] = 1;
        lsfit.lsfitlinearc(y, ref fmatrix, ref cmatrix, n, m, 1, ref info, ref c, ref rep);
        System.Console.Write("Constrained, dp(0)=1 ");
        System.Console.Write("{0,7:F4}",rep.rmserror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",rep.maxerror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",c[0]);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",c[1]);
        System.Console.WriteLine();
        
        //
        // Solve constrained task, p(0)=0, p'(0)=0
        // Prepare constraints matrix:
        // * first M columns store values/derivatives of basis functions at X=0
        // * last column stores desired values/derivative at X=0
        //
        cmatrix = new double[2, m+1];
        cmatrix[0,0] = 1;
        for(i=1; i<=m-1; i++)
        {
            cmatrix[0,i] = 0;
        }
        cmatrix[0,m] = 0;
        for(i=0; i<=m-1; i++)
        {
            cmatrix[1,i] = 0;
        }
        cmatrix[1,1] = 1;
        cmatrix[1,m] = 1;
        lsfit.lsfitlinearc(y, ref fmatrix, ref cmatrix, n, m, 2, ref info, ref c, ref rep);
        System.Console.Write("Constrained, both    ");
        System.Console.Write("{0,7:F4}",rep.rmserror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",rep.maxerror);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",c[0]);
        System.Console.Write(" ");
        System.Console.Write("{0,7:F4}",c[1]);
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.WriteLine();
        return 0;
    }
}