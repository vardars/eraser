
using System;

namespace alglib
{
    public class testgkq
    {
        /*************************************************************************
        Test
        *************************************************************************/
        public static bool testgkqunit(bool silent)
        {
            bool result = new bool();
            int pkind = 0;
            double errtol = 0;
            double eps = 0;
            double nonstricterrtol = 0;
            int n = 0;
            int i = 0;
            int k = 0;
            int info = 0;
            double err = 0;
            int akind = 0;
            int bkind = 0;
            double alphac = 0;
            double betac = 0;
            double[] x1 = new double[0];
            double[] wg1 = new double[0];
            double[] wk1 = new double[0];
            double[] x2 = new double[0];
            double[] wg2 = new double[0];
            double[] wk2 = new double[0];
            int info1 = 0;
            int info2 = 0;
            bool successatleastonce = new bool();
            bool intblerrors = new bool();
            bool vstblerrors = new bool();
            bool generrors = new bool();
            bool waserrors = new bool();

            intblerrors = false;
            vstblerrors = false;
            generrors = false;
            waserrors = false;
            errtol = 10000*AP.Math.MachineEpsilon;
            nonstricterrtol = 1000*errtol;
            
            //
            // test recurrence-based Legendre nodes against the precalculated table
            //
            for(pkind=0; pkind<=5; pkind++)
            {
                n = 0;
                if( pkind==0 )
                {
                    n = 15;
                }
                if( pkind==1 )
                {
                    n = 21;
                }
                if( pkind==2 )
                {
                    n = 31;
                }
                if( pkind==3 )
                {
                    n = 41;
                }
                if( pkind==4 )
                {
                    n = 51;
                }
                if( pkind==5 )
                {
                    n = 61;
                }
                gkq.gkqlegendrecalc(n, ref info, ref x1, ref wk1, ref wg1);
                gkq.gkqlegendretbl(n, ref x2, ref wk2, ref wg2, ref eps);
                if( info<=0 )
                {
                    generrors = true;
                    break;
                }
                for(i=0; i<=n-1; i++)
                {
                    vstblerrors = vstblerrors | (double)(Math.Abs(x1[i]-x2[i]))>(double)(errtol);
                    vstblerrors = vstblerrors | (double)(Math.Abs(wk1[i]-wk2[i]))>(double)(errtol);
                    vstblerrors = vstblerrors | (double)(Math.Abs(wg1[i]-wg2[i]))>(double)(errtol);
                }
            }
            
            //
            // Test recurrence-baced Gauss-Kronrod nodes against Gauss-only nodes
            // calculated with subroutines from GQ unit.
            //
            for(k=1; k<=30; k++)
            {
                n = 2*k+1;
                
                //
                // Gauss-Legendre
                //
                err = 0;
                gkq.gkqgenerategausslegendre(n, ref info1, ref x1, ref wk1, ref wg1);
                gq.gqgenerategausslegendre(k, ref info2, ref x2, ref wg2);
                if( info1>0 & info2>0 )
                {
                    for(i=0; i<=k-1; i++)
                    {
                        err = Math.Max(err, Math.Abs(x1[2*i+1]-x2[i]));
                        err = Math.Max(err, Math.Abs(wg1[2*i+1]-wg2[i]));
                    }
                }
                else
                {
                    generrors = true;
                }
                generrors = generrors | (double)(err)>(double)(errtol);
            }
            for(k=1; k<=15; k++)
            {
                n = 2*k+1;
                
                //
                // Gauss-Jacobi
                //
                successatleastonce = false;
                err = 0;
                for(akind=0; akind<=9; akind++)
                {
                    for(bkind=0; bkind<=9; bkind++)
                    {
                        alphac = mapkind(akind);
                        betac = mapkind(bkind);
                        gkq.gkqgenerategaussjacobi(n, alphac, betac, ref info1, ref x1, ref wk1, ref wg1);
                        gq.gqgenerategaussjacobi(k, alphac, betac, ref info2, ref x2, ref wg2);
                        if( info1>0 & info2>0 )
                        {
                            successatleastonce = true;
                            for(i=0; i<=k-1; i++)
                            {
                                err = Math.Max(err, Math.Abs(x1[2*i+1]-x2[i]));
                                err = Math.Max(err, Math.Abs(wg1[2*i+1]-wg2[i]));
                            }
                        }
                        else
                        {
                            generrors = generrors | info1!=-5;
                        }
                    }
                }
                generrors = generrors | (double)(err)>(double)(errtol) | !successatleastonce;
            }
            
            //
            // end
            //
            waserrors = intblerrors | vstblerrors | generrors;
            if( !silent )
            {
                System.Console.Write("TESTING GAUSS-KRONROD QUADRATURES");
                System.Console.WriteLine();
                System.Console.Write("FINAL RESULT:                             ");
                if( waserrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* PRE-CALCULATED TABLE:                   ");
                if( intblerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* CALCULATED AGAINST THE TABLE:           ");
                if( vstblerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* GENERAL PROPERTIES:                     ");
                if( generrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                if( waserrors )
                {
                    System.Console.Write("TEST FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("TEST PASSED");
                    System.Console.WriteLine();
                }
                System.Console.WriteLine();
                System.Console.WriteLine();
            }
            result = !waserrors;
            return result;
        }


        /*************************************************************************
        Maps:
            0   =>  -0.9
            1   =>  -0.5
            2   =>  -0.1
            3   =>   0.0
            4   =>  +0.1
            5   =>  +0.5
            6   =>  +0.9
            7   =>  +1.0
            8   =>  +1.5
            9   =>  +2.0
        *************************************************************************/
        private static double mapkind(int k)
        {
            double result = 0;

            result = 0;
            if( k==0 )
            {
                result = -0.9;
            }
            if( k==1 )
            {
                result = -0.5;
            }
            if( k==2 )
            {
                result = -0.1;
            }
            if( k==3 )
            {
                result = 0.0;
            }
            if( k==4 )
            {
                result = +0.1;
            }
            if( k==5 )
            {
                result = +0.5;
            }
            if( k==6 )
            {
                result = +0.9;
            }
            if( k==7 )
            {
                result = +1.0;
            }
            if( k==8 )
            {
                result = +1.5;
            }
            if( k==9 )
            {
                result = +2.0;
            }
            return result;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testgkq_test_silent()
        {
            bool result = new bool();

            result = testgkqunit(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testgkq_test()
        {
            bool result = new bool();

            result = testgkqunit(false);
            return result;
        }
    }
}
