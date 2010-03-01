
using System;

namespace alglib
{
    public class testhermiteunit
    {
        public static bool testhermitecalculate(bool silent)
        {
            bool result = new bool();
            double err = 0;
            double sumerr = 0;
            double cerr = 0;
            double threshold = 0;
            int n = 0;
            int maxn = 0;
            int i = 0;
            int j = 0;
            int pass = 0;
            double[] c = new double[0];
            double x = 0;
            double v = 0;
            bool waserrors = new bool();

            err = 0;
            sumerr = 0;
            cerr = 0;
            threshold = 1.0E-9;
            waserrors = false;
            
            //
            // Testing Hermite polynomials
            //
            n = 0;
            err = Math.Max(err, Math.Abs(hermite.hermitecalculate(n, 1)-1));
            n = 1;
            err = Math.Max(err, Math.Abs(hermite.hermitecalculate(n, 1)-2));
            n = 2;
            err = Math.Max(err, Math.Abs(hermite.hermitecalculate(n, 1)-2));
            n = 3;
            err = Math.Max(err, Math.Abs(hermite.hermitecalculate(n, 1)+4));
            n = 4;
            err = Math.Max(err, Math.Abs(hermite.hermitecalculate(n, 1)+20));
            n = 5;
            err = Math.Max(err, Math.Abs(hermite.hermitecalculate(n, 1)+8));
            n = 6;
            err = Math.Max(err, Math.Abs(hermite.hermitecalculate(n, 1)-184));
            n = 7;
            err = Math.Max(err, Math.Abs(hermite.hermitecalculate(n, 1)-464));
            n = 11;
            err = Math.Max(err, Math.Abs(hermite.hermitecalculate(n, 1)-230848));
            n = 12;
            err = Math.Max(err, Math.Abs(hermite.hermitecalculate(n, 1)-280768));
            
            //
            // Testing Clenshaw summation
            //
            maxn = 10;
            c = new double[maxn+1];
            for(pass=1; pass<=10; pass++)
            {
                x = 2*AP.Math.RandomReal()-1;
                v = 0;
                for(n=0; n<=maxn; n++)
                {
                    c[n] = 2*AP.Math.RandomReal()-1;
                    v = v+hermite.hermitecalculate(n, x)*c[n];
                    sumerr = Math.Max(sumerr, Math.Abs(v-hermite.hermitesum(ref c, n, x)));
                }
            }
            
            //
            // Testing coefficients
            //
            hermite.hermitecoefficients(0, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-1));
            hermite.hermitecoefficients(1, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-0));
            cerr = Math.Max(cerr, Math.Abs(c[1]-2));
            hermite.hermitecoefficients(2, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]+2));
            cerr = Math.Max(cerr, Math.Abs(c[1]-0));
            cerr = Math.Max(cerr, Math.Abs(c[2]-4));
            hermite.hermitecoefficients(3, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-0));
            cerr = Math.Max(cerr, Math.Abs(c[1]+12));
            cerr = Math.Max(cerr, Math.Abs(c[2]-0));
            cerr = Math.Max(cerr, Math.Abs(c[3]-8));
            hermite.hermitecoefficients(4, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-12));
            cerr = Math.Max(cerr, Math.Abs(c[1]-0));
            cerr = Math.Max(cerr, Math.Abs(c[2]+48));
            cerr = Math.Max(cerr, Math.Abs(c[3]-0));
            cerr = Math.Max(cerr, Math.Abs(c[4]-16));
            hermite.hermitecoefficients(5, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]-0));
            cerr = Math.Max(cerr, Math.Abs(c[1]-120));
            cerr = Math.Max(cerr, Math.Abs(c[2]-0));
            cerr = Math.Max(cerr, Math.Abs(c[3]+160));
            cerr = Math.Max(cerr, Math.Abs(c[4]-0));
            cerr = Math.Max(cerr, Math.Abs(c[5]-32));
            hermite.hermitecoefficients(6, ref c);
            cerr = Math.Max(cerr, Math.Abs(c[0]+120));
            cerr = Math.Max(cerr, Math.Abs(c[1]-0));
            cerr = Math.Max(cerr, Math.Abs(c[2]-720));
            cerr = Math.Max(cerr, Math.Abs(c[3]-0));
            cerr = Math.Max(cerr, Math.Abs(c[4]+480));
            cerr = Math.Max(cerr, Math.Abs(c[5]-0));
            cerr = Math.Max(cerr, Math.Abs(c[6]-64));
            
            //
            // Reporting
            //
            waserrors = (double)(err)>(double)(threshold) | (double)(sumerr)>(double)(threshold) | (double)(cerr)>(double)(threshold);
            if( !silent )
            {
                System.Console.Write("TESTING CALCULATION OF THE HERMITE POLYNOMIALS");
                System.Console.WriteLine();
                System.Console.Write("Max error                                 ");
                System.Console.Write("{0,5:E2}",err);
                System.Console.WriteLine();
                System.Console.Write("Summation error                           ");
                System.Console.Write("{0,5:E2}",sumerr);
                System.Console.WriteLine();
                System.Console.Write("Coefficients error                        ");
                System.Console.Write("{0,5:E2}",cerr);
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
        public static bool testhermiteunit_test_silent()
        {
            bool result = new bool();

            result = testhermitecalculate(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testhermiteunit_test()
        {
            bool result = new bool();

            result = testhermitecalculate(false);
            return result;
        }
    }
}
