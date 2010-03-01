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

        System.Console.Write("Fitting 1-x^2 on [-1,+1] with cos(alpha*pi*x)+beta");
        System.Console.WriteLine();
        
        //
        // Fitting 1-x^2 on [-1,+1] with cos(alpha*pi*x)+beta:
        // * using Hessian
        // * using alpha=1 and beta=0 as initial values
        // * using 1000 uniformly distributed points to fit to
        //
        // Notes:
        // * N - number of points
        // * M - dimension of space where points reside
        // * K - number of parameters being fitted
        //
        n = 1000;
        m = 1;
        k = 2;
        a = -1;
        b = +1;
        
        //
        // Prepare task matrix
        //
        y = new double[n];
        x = new double[n, m];
        c = new double[k];
        for(i=0; i<=n-1; i++)
        {
            x[i,0] = a+(b-a)*i/(n-1);
            y[i] = 1-AP.Math.Sqr(x[i,0]);
        }
        c[0] = 1.0;
        c[1] = 0.0;
        epsf = 0.0;
        epsx = 0.0001;
        maxits = 0;
        
        //
        // Solve
        //
        lsfit.lsfitnonlinearfgh(ref x, ref y, ref c, n, m, k, epsf, epsx, maxits, ref state);
        while( lsfit.lsfitnonlineariteration(ref state) )
        {
            
            //
            // F(x) = Cos(alpha*pi*x)+beta
            //
            state.f = Math.Cos(state.c[0]*Math.PI*state.x[0])+state.c[1];
            
            //
            // F(x)      = Cos(alpha*pi*x)+beta
            // dF/dAlpha = -pi*x*Sin(alpha*pi*x)
            // dF/dBeta  = 1.0
            //
            if( state.needfg | state.needfgh )
            {
                state.g[0] = -(Math.PI*state.x[0]*Math.Sin(state.c[0]*Math.PI*state.x[0]));
                state.g[1] = 1.0;
            }
            
            //
            // F(x)            = Cos(alpha*pi*x)+beta
            // d2F/dAlpha2     = -(pi*x)^2*Cos(alpha*pi*x)
            // d2F/dAlphadBeta = 0
            // d2F/dBeta2     =  0
            //
            if( state.needfgh )
            {
                state.h[0,0] = -(AP.Math.Sqr(Math.PI*state.x[0])*Math.Cos(state.c[0]*Math.PI*state.x[0]));
                state.h[0,1] = 0.0;
                state.h[1,0] = 0.0;
                state.h[1,1] = 0.0;
            }
        }
        lsfit.lsfitnonlinearresults(ref state, ref info, ref c, ref rep);
        System.Console.Write("alpha:   ");
        System.Console.Write("{0,0:F3}",c[0]);
        System.Console.WriteLine();
        System.Console.Write("beta:    ");
        System.Console.Write("{0,0:F3}",c[1]);
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