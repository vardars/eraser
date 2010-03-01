using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        int n = 0;
        int i = 0;
        int j = 0;
        double c1 = 0;
        double x = 0;
        double[,] a = new double[0,0];

        System.Console.Write("                 CONDITION NUMBERS");
        System.Console.WriteLine();
        System.Console.Write("OF VANDERMONDE AND CHEBYSHEV INTERPOLATION MATRICES");
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.Write("    VANDERMONDE   CHEBYSHEV");
        System.Console.WriteLine();
        System.Console.Write("  N      1-norm      1-norm");
        System.Console.WriteLine();
        for(n=2; n<=14; n++)
        {
            a = new double[n, n];
            System.Console.Write("{0,3:d}",n);
            
            //
            // Vandermone matrix
            //
            for(i=0; i<=n-1; i++)
            {
                x = (double)(2*i)/((double)(n-1))-1;
                a[i,0] = 1;
                for(j=1; j<=n-1; j++)
                {
                    a[i,j] = a[i,j-1]*x;
                }
            }
            c1 = 1/rcond.rmatrixrcond1(a, n);
            System.Console.Write(" ");
            System.Console.Write("{0,11:F1}",c1);
            
            //
            // Chebyshev interpolation matrix
            //
            for(i=0; i<=n-1; i++)
            {
                x = (double)(2*i)/((double)(n-1))-1;
                a[i,0] = 1;
                if( n>=2 )
                {
                    a[i,1] = x;
                }
                for(j=2; j<=n-1; j++)
                {
                    a[i,j] = 2*x*a[i,j-1]-a[i,j-2];
                }
            }
            c1 = 1/rcond.rmatrixrcond1(a, n);
            System.Console.Write(" ");
            System.Console.Write("{0,11:F1}",c1);
            System.Console.WriteLine();
        }
        return 0;
    }
}