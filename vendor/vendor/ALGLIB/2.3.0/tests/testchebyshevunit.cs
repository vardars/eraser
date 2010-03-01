
using System;

namespace alglib
{
    public class testchebyshevunit
    {
        public static bool testchebyshev(bool silent)
        {
            bool result = new bool();
            double err = 0;
            double sumerr = 0;
            double cerr = 0;
            double ferr = 0;
            double threshold = 0;
            double x = 0;
            double v = 0;
            double t = 0;
            int pass = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int n = 0;
            int maxn = 0;
            double[] c = new double[0];
            double[] p1 = new double[0];
            double[] p2 = new double[0];
            double[,] a = new double[0,0];
            bool waserrors = new bool();
            int i_ = 0;

            err = 0;
            sumerr = 0;
            cerr = 0;
            ferr = 0;
            threshold = 1.0E-9;
            waserrors = false;
            
            //
            // Testing Chebyshev polynomials of the first kind
            //
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, 0, 0.00)-1));
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, 0, 0.33)-1));
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, 0, -0.42)-1));
            x = 0.2;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, 1, x)-0.2));
            x = 0.4;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, 1, x)-0.4));
            x = 0.6;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, 1, x)-0.6));
            x = 0.8;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, 1, x)-0.8));
            x = 1.0;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, 1, x)-1.0));
            x = 0.2;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, 2, x)+0.92));
            x = 0.4;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, 2, x)+0.68));
            x = 0.6;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, 2, x)+0.28));
            x = 0.8;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, 2, x)-0.28));
            x = 1.0;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, 2, x)-1.00));
            n = 10;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, n, 0.2)-0.4284556288));
            n = 11;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, n, 0.2)+0.7996160205));
            n = 12;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(1, n, 0.2)+0.7483020370));
            
            //
            // Testing Chebyshev polynomials of the second kind
            //
            n = 0;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(2, n, 0.2)-1.0000000000));
            n = 1;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(2, n, 0.2)-0.4000000000));
            n = 2;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(2, n, 0.2)+0.8400000000));
            n = 3;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(2, n, 0.2)+0.7360000000));
            n = 4;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(2, n, 0.2)-0.5456000000));
            n = 10;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(2, n, 0.2)-0.6128946176));
            n = 11;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(2, n, 0.2)+0.6770370970));
            n = 12;
            err = Math.Max(err, Math.Abs(chebyshev.chebyshevcalculate(2, n, 0.2)+0.8837094564));
            
            //
            // Testing Clenshaw summation
            //
            maxn = 20;
            c = new double[maxn+1];
            for(k=1; k<=2; k++)
            {
                for(pass=1; pass<=10; pass++)
                {
                    x = 2*AP.Math.RandomReal()-1;
                    v = 0;
                    for(n=0; n<=maxn; n++)
                    {
                        c[n] = 2*AP.Math.RandomReal()-1;
                        v = v+chebyshev.chebyshevcalculate(k, n, x)*c[n];
                        sumerr = Math.Max(sumerr, Math.Abs(v-chebyshev.chebyshevsum(ref c, k, n, x)));
                    }
                }
            }
            
            //
            // Testing coefficients
            //
            chebyshev.chebyshevcoefficients(0, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-1));
            chebyshev.chebyshevcoefficients(1, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-0));
            cerr = Math.Max(cerr, Math.Abs(c[1]-1));
            chebyshev.chebyshevcoefficients(2, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]+1));
            cerr = Math.Max(cerr, Math.Abs(c[1]-0));
            cerr = Math.Max(cerr, Math.Abs(c[2]-2));
            chebyshev.chebyshevcoefficients(3, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-0));
            cerr = Math.Max(cerr, Math.Abs(c[1]+3));
            cerr = Math.Max(cerr, Math.Abs(c[2]-0));
            cerr = Math.Max(cerr, Math.Abs(c[3]-4));
            chebyshev.chebyshevcoefficients(4, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-1));
            cerr = Math.Max(cerr, Math.Abs(c[1]-0));
            cerr = Math.Max(cerr, Math.Abs(c[2]+8));
            cerr = Math.Max(cerr, Math.Abs(c[3]-0));
            cerr = Math.Max(cerr, Math.Abs(c[4]-8));
            chebyshev.chebyshevcoefficients(9, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-0));
            cerr = Math.Max(cerr, Math.Abs(c[1]-9));
            cerr = Math.Max(cerr, Math.Abs(c[2]-0));
            cerr = Math.Max(cerr, Math.Abs(c[3]+120));
            cerr = Math.Max(cerr, Math.Abs(c[4]-0));
            cerr = Math.Max(cerr, Math.Abs(c[5]-432));
            cerr = Math.Max(cerr, Math.Abs(c[6]-0));
            cerr = Math.Max(cerr, Math.Abs(c[7]+576));
            cerr = Math.Max(cerr, Math.Abs(c[8]-0));
            cerr = Math.Max(cerr, Math.Abs(c[9]-256));
            
            //
            // Testing FromChebyshev
            //
            maxn = 10;
            a = new double[maxn+1, maxn+1];
            for(i=0; i<=maxn; i++)
            {
                for(j=0; j<=maxn; j++)
                {
                    a[i,j] = 0;
                }
                chebyshev.chebyshevcoefficients(i, ref c);
                for(i_=0; i_<=i;i_++)
                {
                    a[i,i_] = c[i_];
                }
            }
            c = new double[maxn+1];
            p1 = new double[maxn+1];
            for(n=0; n<=maxn; n++)
            {
                for(pass=1; pass<=10; pass++)
                {
                    for(i=0; i<=n; i++)
                    {
                        p1[i] = 0;
                    }
                    for(i=0; i<=n; i++)
                    {
                        c[i] = 2*AP.Math.RandomReal()-1;
                        v = c[i];
                        for(i_=0; i_<=i;i_++)
                        {
                            p1[i_] = p1[i_] + v*a[i,i_];
                        }
                    }
                    chebyshev.fromchebyshev(ref c, n, ref p2);
                    for(i=0; i<=n; i++)
                    {
                        ferr = Math.Max(ferr, Math.Abs(p1[i]-p2[i]));
                    }
                }
            }
            
            //
            // Reporting
            //
            waserrors = (double)(err)>(double)(threshold) | (double)(sumerr)>(double)(threshold) | (double)(cerr)>(double)(threshold) | (double)(ferr)>(double)(threshold);
            if( !silent )
            {
                System.Console.Write("TESTING CALCULATION OF THE CHEBYSHEV POLYNOMIALS");
                System.Console.WriteLine();
                System.Console.Write("Max error against table                   ");
                System.Console.Write("{0,5:E2}",err);
                System.Console.WriteLine();
                System.Console.Write("Summation error                           ");
                System.Console.Write("{0,5:E2}",sumerr);
                System.Console.WriteLine();
                System.Console.Write("Coefficients error                        ");
                System.Console.Write("{0,5:E2}",cerr);
                System.Console.WriteLine();
                System.Console.Write("FrobChebyshev error                       ");
                System.Console.Write("{0,5:E2}",ferr);
                System.Console.WriteLine();
                System.Console.Write("Threshold                                 ");
                System.Console.Write("{0,5:E2}",threshold);
                System.Console.WriteLine();
                if( !waserrors )
                {
                    System.Console.Write("TEST PASSED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("TEST FAILED");
                    System.Console.WriteLine();
                }
            }
            result = !waserrors;
            return result;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testchebyshevunit_test_silent()
        {
            bool result = new bool();

            result = testchebyshev(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testchebyshevunit_test()
        {
            bool result = new bool();

            result = testchebyshev(false);
            return result;
        }
    }
}
