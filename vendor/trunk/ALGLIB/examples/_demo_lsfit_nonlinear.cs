using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        int m = 0;
        int n = 0;
        int k = 0;
        double[] y = new double[0];
        double[,] x = new double[0,0];
        double[] c = new double[0];
        lsfit.lsfitreport rep = new lsfit.lsfitreport();
        lsfit.lsfitstate state = new lsfit.lsfitstate();
        int info = 0;
        double epsf = 0;
        double epsx = 0;
        int maxits = 0;
        int i = 0;
        int j = 0;
        double a = 0;
        double b = 0;

        System.Console.Write("Fitting 0.5(1+cos(x)) on [-pi,+pi] with exp(-alpha*x^2)");
        System.Console.WriteLine();
        
        //
        // Fitting 0.5(1+cos(x)) on [-pi,+pi] with Gaussian exp(-alpha*x^2):
        // * without Hessian (gradient only)
        // * using alpha=1 as initial value
        // * using 1000 uniformly distributed points to fit to
        //
        // Notes:
        // * N - number of points
        // * M - dimension of space where points reside
        // * K - number of parameters being fitted
        //
        n = 1000;
        m = 1;
        k = 1;
        a = -Math.PI;
        b = +Math.PI;
        
        //
        // Prepare task matrix
        //
        y = new double[n];
        x = new double[n, m];
        c = new double[k];
        for(i=0; i<=n-1; i++)
        {
            x[i,0] = a+(b-a)*i/(n-1);
            y[i] = 0.5*(1+Math.Cos(x[i,0]));
        }
        c[0] = 1.0;
        epsf = 0.0;
        epsx = 0.0001;
        maxits = 0;
        
        //
        // Solve
        //
        lsfit.lsfitnonlinearfg(ref x, ref y, ref c, n, m, k, epsf, epsx, maxits, true, ref state);
        while( lsfit.lsfitnonlineariteration(ref state) )
        {
            if( state.needf )
            {
                
                //
                // F(x) = Exp(-alpha*x^2)
                //
                state.f = Math.Exp(-(state.c[0]*AP.Math.Sqr(state.x[0])));
            }
            if( state.needfg )
            {
                
                //
                // F(x)      = Exp(-alpha*x^2)
                // dF/dAlpha = (-x^2)*Exp(-alpha*x^2)
                //
                state.f = Math.Exp(-(state.c[0]*AP.Math.Sqr(state.x[0])));
                state.g[0] = -(AP.Math.Sqr(state.x[0])*state.f);
            }
        }
        lsfit.lsfitnonlinearresults(ref state, ref info, ref c, ref rep);
        System.Console.Write("alpha:   ");
        System.Console.Write("{0,0:F3}",c[0]);
        System.Console.WriteLine();
        System.Console.Write("rms.err: ");
        System.Console.Write("{0,0:F3}",rep.rmserror);
        System.Console.WriteLine();
        System.Console.Write("max.err: ");
        System.Console.Write("{0,0:F3}",rep.maxerror);
        System.Console.WriteLine();
        System.Console.Write("Termination type: ");
        System.Console.Write("{0,0:d}",info);
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.WriteLine();
        return 0;
    }
}