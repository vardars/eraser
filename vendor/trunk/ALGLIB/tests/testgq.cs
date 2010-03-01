
using System;

namespace alglib
{
    public class testgq
    {
        /*************************************************************************
        Test
        *************************************************************************/
        public static bool testgqunit(bool silent)
        {
            bool result = new bool();
            double[] alpha = new double[0];
            double[] beta = new double[0];
            double[] x = new double[0];
            double[] w = new double[0];
            double[] x2 = new double[0];
            double[] w2 = new double[0];
            double err = 0;
            int n = 0;
            int i = 0;
            int info = 0;
            int akind = 0;
            int bkind = 0;
            double alphac = 0;
            double betac = 0;
            double errtol = 0;
            double nonstricterrtol = 0;
            double stricterrtol = 0;
            bool recerrors = new bool();
            bool specerrors = new bool();
            bool waserrors = new bool();

            recerrors = false;
            specerrors = false;
            waserrors = false;
            errtol = 1.0E-12;
            nonstricterrtol = 1.0E-6;
            stricterrtol = 1000*AP.Math.MachineEpsilon;
            
            //
            // Three tests for rec-based Gauss quadratures with known weights/nodes:
            // 1. Gauss-Legendre with N=2
            // 2. Gauss-Legendre with N=5
            // 3. Gauss-Chebyshev with N=1, 2, 4, 8, ..., 512
            //
            err = 0;
            alpha = new double[2];
            beta = new double[2];
            alpha[0] = 0;
            alpha[1] = 0;
            beta[1] = (double)(1)/((double)(4*1*1-1));
            gq.gqgeneraterec(ref alpha, ref beta, 2.0, 2, ref info, ref x, ref w);
            if( info>0 )
            {
                err = Math.Max(err, Math.Abs(x[0]+Math.Sqrt(3)/3));
                err = Math.Max(err, Math.Abs(x[1]-Math.Sqrt(3)/3));
                err = Math.Max(err, Math.Abs(w[0]-1));
                err = Math.Max(err, Math.Abs(w[1]-1));
                for(i=0; i<=0; i++)
                {
                    recerrors = recerrors | (double)(x[i])>=(double)(x[i+1]);
                }
            }
            else
            {
                recerrors = true;
            }
            alpha = new double[5];
            beta = new double[5];
            alpha[0] = 0;
            for(i=1; i<=4; i++)
            {
                alpha[i] = 0;
                beta[i] = AP.Math.Sqr(i)/(4*AP.Math.Sqr(i)-1);
            }
            gq.gqgeneraterec(ref alpha, ref beta, 2.0, 5, ref info, ref x, ref w);
            if( info>0 )
            {
                err = Math.Max(err, Math.Abs(x[0]+Math.Sqrt(245+14*Math.Sqrt(70))/21));
                err = Math.Max(err, Math.Abs(x[0]+x[4]));
                err = Math.Max(err, Math.Abs(x[1]+Math.Sqrt(245-14*Math.Sqrt(70))/21));
                err = Math.Max(err, Math.Abs(x[1]+x[3]));
                err = Math.Max(err, Math.Abs(x[2]));
                err = Math.Max(err, Math.Abs(w[0]-(322-13*Math.Sqrt(70))/900));
                err = Math.Max(err, Math.Abs(w[0]-w[4]));
                err = Math.Max(err, Math.Abs(w[1]-(322+13*Math.Sqrt(70))/900));
                err = Math.Max(err, Math.Abs(w[1]-w[3]));
                err = Math.Max(err, Math.Abs(w[2]-(double)(128)/(double)(225)));
                for(i=0; i<=3; i++)
                {
                    recerrors = recerrors | (double)(x[i])>=(double)(x[i+1]);
                }
            }
            else
            {
                recerrors = true;
            }
            n = 1;
            while( n<=512 )
            {
                alpha = new double[n];
                beta = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    alpha[i] = 0;
                    if( i==0 )
                    {
                        beta[i] = 0;
                    }
                    if( i==1 )
                    {
                        beta[i] = (double)(1)/(double)(2);
                    }
                    if( i>1 )
                    {
                        beta[i] = (double)(1)/(double)(4);
                    }
                }
                gq.gqgeneraterec(ref alpha, ref beta, Math.PI, n, ref info, ref x, ref w);
                if( info>0 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        err = Math.Max(err, Math.Abs(x[i]-Math.Cos(Math.PI*(n-i-0.5)/n)));
                        err = Math.Max(err, Math.Abs(w[i]-Math.PI/n));
                    }
                    for(i=0; i<=n-2; i++)
                    {
                        recerrors = recerrors | (double)(x[i])>=(double)(x[i+1]);
                    }
                }
                else
                {
                    recerrors = true;
                }
                n = n*2;
            }
            recerrors = recerrors | (double)(err)>(double)(errtol);
            
            //
            // Three tests for rec-based Gauss-Lobatto quadratures with known weights/nodes:
            // 1. Gauss-Lobatto with N=3
            // 2. Gauss-Lobatto with N=4
            // 3. Gauss-Lobatto with N=6
            //
            err = 0;
            alpha = new double[2];
            beta = new double[2];
            alpha[0] = 0;
            alpha[1] = 0;
            beta[0] = 0;
            beta[1] = (double)(1*1)/((double)(4*1*1-1));
            gq.gqgenerategausslobattorec(alpha, beta, 2.0, -1, +1, 3, ref info, ref x, ref w);
            if( info>0 )
            {
                err = Math.Max(err, Math.Abs(x[0]+1));
                err = Math.Max(err, Math.Abs(x[1]));
                err = Math.Max(err, Math.Abs(x[2]-1));
                err = Math.Max(err, Math.Abs(w[0]-(double)(1)/(double)(3)));
                err = Math.Max(err, Math.Abs(w[1]-(double)(4)/(double)(3)));
                err = Math.Max(err, Math.Abs(w[2]-(double)(1)/(double)(3)));
                for(i=0; i<=1; i++)
                {
                    recerrors = recerrors | (double)(x[i])>=(double)(x[i+1]);
                }
            }
            else
            {
                recerrors = true;
            }
            alpha = new double[3];
            beta = new double[3];
            alpha[0] = 0;
            alpha[1] = 0;
            alpha[2] = 0;
            beta[0] = 0;
            beta[1] = (double)(1*1)/((double)(4*1*1-1));
            beta[2] = (double)(2*2)/((double)(4*2*2-1));
            gq.gqgenerategausslobattorec(alpha, beta, 2.0, -1, +1, 4, ref info, ref x, ref w);
            if( info>0 )
            {
                err = Math.Max(err, Math.Abs(x[0]+1));
                err = Math.Max(err, Math.Abs(x[1]+Math.Sqrt(5)/5));
                err = Math.Max(err, Math.Abs(x[2]-Math.Sqrt(5)/5));
                err = Math.Max(err, Math.Abs(x[3]-1));
                err = Math.Max(err, Math.Abs(w[0]-(double)(1)/(double)(6)));
                err = Math.Max(err, Math.Abs(w[1]-(double)(5)/(double)(6)));
                err = Math.Max(err, Math.Abs(w[2]-(double)(5)/(double)(6)));
                err = Math.Max(err, Math.Abs(w[3]-(double)(1)/(double)(6)));
                for(i=0; i<=2; i++)
                {
                    recerrors = recerrors | (double)(x[i])>=(double)(x[i+1]);
                }
            }
            else
            {
                recerrors = true;
            }
            alpha = new double[5];
            beta = new double[5];
            alpha[0] = 0;
            alpha[1] = 0;
            alpha[2] = 0;
            alpha[3] = 0;
            alpha[4] = 0;
            beta[0] = 0;
            beta[1] = (double)(1*1)/((double)(4*1*1-1));
            beta[2] = (double)(2*2)/((double)(4*2*2-1));
            beta[3] = (double)(3*3)/((double)(4*3*3-1));
            beta[4] = (double)(4*4)/((double)(4*4*4-1));
            gq.gqgenerategausslobattorec(alpha, beta, 2.0, -1, +1, 6, ref info, ref x, ref w);
            if( info>0 )
            {
                err = Math.Max(err, Math.Abs(x[0]+1));
                err = Math.Max(err, Math.Abs(x[1]+Math.Sqrt((7+2*Math.Sqrt(7))/21)));
                err = Math.Max(err, Math.Abs(x[2]+Math.Sqrt((7-2*Math.Sqrt(7))/21)));
                err = Math.Max(err, Math.Abs(x[3]-Math.Sqrt((7-2*Math.Sqrt(7))/21)));
                err = Math.Max(err, Math.Abs(x[4]-Math.Sqrt((7+2*Math.Sqrt(7))/21)));
                err = Math.Max(err, Math.Abs(x[5]-1));
                err = Math.Max(err, Math.Abs(w[0]-(double)(1)/(double)(15)));
                err = Math.Max(err, Math.Abs(w[1]-(14-Math.Sqrt(7))/30));
                err = Math.Max(err, Math.Abs(w[2]-(14+Math.Sqrt(7))/30));
                err = Math.Max(err, Math.Abs(w[3]-(14+Math.Sqrt(7))/30));
                err = Math.Max(err, Math.Abs(w[4]-(14-Math.Sqrt(7))/30));
                err = Math.Max(err, Math.Abs(w[5]-(double)(1)/(double)(15)));
                for(i=0; i<=4; i++)
                {
                    recerrors = recerrors | (double)(x[i])>=(double)(x[i+1]);
                }
            }
            else
            {
                recerrors = true;
            }
            recerrors = recerrors | (double)(err)>(double)(errtol);
            
            //
            // Three tests for rec-based Gauss-Radau quadratures with known weights/nodes:
            // 1. Gauss-Radau with N=2
            // 2. Gauss-Radau with N=3
            // 3. Gauss-Radau with N=3 (another case)
            //
            err = 0;
            alpha = new double[1];
            beta = new double[2];
            alpha[0] = 0;
            beta[0] = 0;
            beta[1] = (double)(1*1)/((double)(4*1*1-1));
            gq.gqgenerategaussradaurec(alpha, beta, 2.0, -1, 2, ref info, ref x, ref w);
            if( info>0 )
            {
                err = Math.Max(err, Math.Abs(x[0]+1));
                err = Math.Max(err, Math.Abs(x[1]-(double)(1)/(double)(3)));
                err = Math.Max(err, Math.Abs(w[0]-0.5));
                err = Math.Max(err, Math.Abs(w[1]-1.5));
                for(i=0; i<=0; i++)
                {
                    recerrors = recerrors | (double)(x[i])>=(double)(x[i+1]);
                }
            }
            else
            {
                recerrors = true;
            }
            alpha = new double[2];
            beta = new double[3];
            alpha[0] = 0;
            alpha[1] = 0;
            for(i=0; i<=2; i++)
            {
                beta[i] = AP.Math.Sqr(i)/(4*AP.Math.Sqr(i)-1);
            }
            gq.gqgenerategaussradaurec(alpha, beta, 2.0, -1, 3, ref info, ref x, ref w);
            if( info>0 )
            {
                err = Math.Max(err, Math.Abs(x[0]+1));
                err = Math.Max(err, Math.Abs(x[1]-(1-Math.Sqrt(6))/5));
                err = Math.Max(err, Math.Abs(x[2]-(1+Math.Sqrt(6))/5));
                err = Math.Max(err, Math.Abs(w[0]-(double)(2)/(double)(9)));
                err = Math.Max(err, Math.Abs(w[1]-(16+Math.Sqrt(6))/18));
                err = Math.Max(err, Math.Abs(w[2]-(16-Math.Sqrt(6))/18));
                for(i=0; i<=1; i++)
                {
                    recerrors = recerrors | (double)(x[i])>=(double)(x[i+1]);
                }
            }
            else
            {
                recerrors = true;
            }
            alpha = new double[2];
            beta = new double[3];
            alpha[0] = 0;
            alpha[1] = 0;
            for(i=0; i<=2; i++)
            {
                beta[i] = AP.Math.Sqr(i)/(4*AP.Math.Sqr(i)-1);
            }
            gq.gqgenerategaussradaurec(alpha, beta, 2.0, +1, 3, ref info, ref x, ref w);
            if( info>0 )
            {
                err = Math.Max(err, Math.Abs(x[2]-1));
                err = Math.Max(err, Math.Abs(x[1]+(1-Math.Sqrt(6))/5));
                err = Math.Max(err, Math.Abs(x[0]+(1+Math.Sqrt(6))/5));
                err = Math.Max(err, Math.Abs(w[2]-(double)(2)/(double)(9)));
                err = Math.Max(err, Math.Abs(w[1]-(16+Math.Sqrt(6))/18));
                err = Math.Max(err, Math.Abs(w[0]-(16-Math.Sqrt(6))/18));
                for(i=0; i<=1; i++)
                {
                    recerrors = recerrors | (double)(x[i])>=(double)(x[i+1]);
                }
            }
            else
            {
                recerrors = true;
            }
            recerrors = recerrors | (double)(err)>(double)(errtol);
            
            //
            // test recurrence-based special cases (Legendre, Jacobi, Hermite, ...)
            // against another implementation (polynomial root-finder)
            //
            for(n=1; n<=20; n++)
            {
                
                //
                // test gauss-legendre
                //
                err = 0;
                gq.gqgenerategausslegendre(n, ref info, ref x, ref w);
                if( info>0 )
                {
                    buildgausslegendrequadrature(n, ref x2, ref w2);
                    for(i=0; i<=n-1; i++)
                    {
                        err = Math.Max(err, Math.Abs(x[i]-x2[i]));
                        err = Math.Max(err, Math.Abs(w[i]-w2[i]));
                    }
                }
                else
                {
                    specerrors = true;
                }
                specerrors = specerrors | (double)(err)>(double)(errtol);
                
                //
                // Test Gauss-Jacobi.
                // Since task is much more difficult we will use less strict
                // threshold.
                //
                err = 0;
                for(akind=0; akind<=9; akind++)
                {
                    for(bkind=0; bkind<=9; bkind++)
                    {
                        alphac = mapkind(akind);
                        betac = mapkind(bkind);
                        gq.gqgenerategaussjacobi(n, alphac, betac, ref info, ref x, ref w);
                        if( info>0 )
                        {
                            buildgaussjacobiquadrature(n, alphac, betac, ref x2, ref w2);
                            for(i=0; i<=n-1; i++)
                            {
                                err = Math.Max(err, Math.Abs(x[i]-x2[i]));
                                err = Math.Max(err, Math.Abs(w[i]-w2[i]));
                            }
                        }
                        else
                        {
                            specerrors = true;
                        }
                    }
                }
                specerrors = specerrors | (double)(err)>(double)(nonstricterrtol);
                
                //
                // special test for Gauss-Jacobi (Chebyshev weight
                // function with analytically known nodes/weights)
                //
                err = 0;
                gq.gqgenerategaussjacobi(n, -0.5, -0.5, ref info, ref x, ref w);
                if( info>0 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        err = Math.Max(err, Math.Abs(x[i]+Math.Cos(Math.PI*(i+0.5)/n)));
                        err = Math.Max(err, Math.Abs(w[i]-Math.PI/n));
                    }
                }
                else
                {
                    specerrors = true;
                }
                specerrors = specerrors | (double)(err)>(double)(stricterrtol);
                
                //
                // Test Gauss-Laguerre
                //
                err = 0;
                for(akind=0; akind<=9; akind++)
                {
                    alphac = mapkind(akind);
                    gq.gqgenerategausslaguerre(n, alphac, ref info, ref x, ref w);
                    if( info>0 )
                    {
                        buildgausslaguerrequadrature(n, alphac, ref x2, ref w2);
                        for(i=0; i<=n-1; i++)
                        {
                            err = Math.Max(err, Math.Abs(x[i]-x2[i]));
                            err = Math.Max(err, Math.Abs(w[i]-w2[i]));
                        }
                    }
                    else
                    {
                        specerrors = true;
                    }
                }
                specerrors = specerrors | (double)(err)>(double)(nonstricterrtol);
                
                //
                // Test Gauss-Hermite
                //
                err = 0;
                gq.gqgenerategausshermite(n, ref info, ref x, ref w);
                if( info>0 )
                {
                    buildgausshermitequadrature(n, ref x2, ref w2);
                    for(i=0; i<=n-1; i++)
                    {
                        err = Math.Max(err, Math.Abs(x[i]-x2[i]));
                        err = Math.Max(err, Math.Abs(w[i]-w2[i]));
                    }
                }
                else
                {
                    specerrors = true;
                }
                specerrors = specerrors | (double)(err)>(double)(nonstricterrtol);
            }
            
            //
            // end
            //
            waserrors = recerrors | specerrors;
            if( !silent )
            {
                System.Console.Write("TESTING GAUSS QUADRATURES");
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
                System.Console.Write("* SPECIAL CASES (LEGENDRE/JACOBI/..)      ");
                if( specerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* RECURRENCE-BASED:                       ");
                if( recerrors )
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
        Gauss-Hermite, another variant
        *************************************************************************/
        public static void buildgausshermitequadrature(int n,
            ref double[] x,
            ref double[] w)
        {
            int i = 0;
            int j = 0;
            double r = 0;
            double r1 = 0;
            double p1 = 0;
            double p2 = 0;
            double p3 = 0;
            double dp3 = 0;
            double pipm4 = 0;
            double tmp = 0;

            x = new double[n-1+1];
            w = new double[n-1+1];
            pipm4 = Math.Pow(Math.PI, -0.25);
            for(i=0; i<=(n+1)/2-1; i++)
            {
                if( i==0 )
                {
                    r = Math.Sqrt(2*n+1)-1.85575*Math.Pow(2*n+1, -((double)(1)/(double)(6)));
                }
                else
                {
                    if( i==1 )
                    {
                        r = r-1.14*Math.Pow(n, 0.426)/r;
                    }
                    else
                    {
                        if( i==2 )
                        {
                            r = 1.86*r-0.86*x[0];
                        }
                        else
                        {
                            if( i==3 )
                            {
                                r = 1.91*r-0.91*x[1];
                            }
                            else
                            {
                                r = 2*r-x[i-2];
                            }
                        }
                    }
                }
                do
                {
                    p2 = 0;
                    p3 = pipm4;
                    for(j=0; j<=n-1; j++)
                    {
                        p1 = p2;
                        p2 = p3;
                        p3 = p2*r*Math.Sqrt((double)(2)/((double)(j+1)))-p1*Math.Sqrt((double)(j)/((double)(j+1)));
                    }
                    dp3 = Math.Sqrt(2*j)*p2;
                    r1 = r;
                    r = r-p3/dp3;
                }
                while( (double)(Math.Abs(r-r1))>=(double)(AP.Math.MachineEpsilon*(1+Math.Abs(r))*100) );
                x[i] = r;
                w[i] = 2/(dp3*dp3);
                x[n-1-i] = -x[i];
                w[n-1-i] = w[i];
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-2-i; j++)
                {
                    if( (double)(x[j])>=(double)(x[j+1]) )
                    {
                        tmp = x[j];
                        x[j] = x[j+1];
                        x[j+1] = tmp;
                        tmp = w[j];
                        w[j] = w[j+1];
                        w[j+1] = tmp;
                    }
                }
            }
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
        Gauss-Legendre, another variant
        *************************************************************************/
        private static void buildgausslegendrequadrature(int n,
            ref double[] x,
            ref double[] w)
        {
            int i = 0;
            int j = 0;
            double r = 0;
            double r1 = 0;
            double p1 = 0;
            double p2 = 0;
            double p3 = 0;
            double dp3 = 0;
            double tmp = 0;

            x = new double[n-1+1];
            w = new double[n-1+1];
            for(i=0; i<=(n+1)/2-1; i++)
            {
                r = Math.Cos(Math.PI*(4*i+3)/(4*n+2));
                do
                {
                    p2 = 0;
                    p3 = 1;
                    for(j=0; j<=n-1; j++)
                    {
                        p1 = p2;
                        p2 = p3;
                        p3 = ((2*j+1)*r*p2-j*p1)/(j+1);
                    }
                    dp3 = n*(r*p3-p2)/(r*r-1);
                    r1 = r;
                    r = r-p3/dp3;
                }
                while( (double)(Math.Abs(r-r1))>=(double)(AP.Math.MachineEpsilon*(1+Math.Abs(r))*100) );
                x[i] = r;
                x[n-1-i] = -r;
                w[i] = 2/((1-r*r)*dp3*dp3);
                w[n-1-i] = 2/((1-r*r)*dp3*dp3);
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-2-i; j++)
                {
                    if( (double)(x[j])>=(double)(x[j+1]) )
                    {
                        tmp = x[j];
                        x[j] = x[j+1];
                        x[j+1] = tmp;
                        tmp = w[j];
                        w[j] = w[j+1];
                        w[j+1] = tmp;
                    }
                }
            }
        }


        /*************************************************************************
        Gauss-Jacobi, another variant
        *************************************************************************/
        private static void buildgaussjacobiquadrature(int n,
            double alpha,
            double beta,
            ref double[] x,
            ref double[] w)
        {
            int i = 0;
            int j = 0;
            double r = 0;
            double r1 = 0;
            double t1 = 0;
            double t2 = 0;
            double t3 = 0;
            double p1 = 0;
            double p2 = 0;
            double p3 = 0;
            double pp = 0;
            double an = 0;
            double bn = 0;
            double a = 0;
            double b = 0;
            double c = 0;
            double tmpsgn = 0;
            double tmp = 0;
            double alfbet = 0;
            double temp = 0;
            int its = 0;

            x = new double[n-1+1];
            w = new double[n-1+1];
            for(i=0; i<=n-1; i++)
            {
                if( i==0 )
                {
                    an = alpha/n;
                    bn = beta/n;
                    t1 = (1+alpha)*(2.78/(4+n*n)+0.768*an/n);
                    t2 = 1+1.48*an+0.96*bn+0.452*an*an+0.83*an*bn;
                    r = (t2-t1)/t2;
                }
                else
                {
                    if( i==1 )
                    {
                        t1 = (4.1+alpha)/((1+alpha)*(1+0.156*alpha));
                        t2 = 1+0.06*(n-8)*(1+0.12*alpha)/n;
                        t3 = 1+0.012*beta*(1+0.25*Math.Abs(alpha))/n;
                        r = r-t1*t2*t3*(1-r);
                    }
                    else
                    {
                        if( i==2 )
                        {
                            t1 = (1.67+0.28*alpha)/(1+0.37*alpha);
                            t2 = 1+0.22*(n-8)/n;
                            t3 = 1+8*beta/((6.28+beta)*n*n);
                            r = r-t1*t2*t3*(x[0]-r);
                        }
                        else
                        {
                            if( i<n-2 )
                            {
                                r = 3*x[i-1]-3*x[i-2]+x[i-3];
                            }
                            else
                            {
                                if( i==n-2 )
                                {
                                    t1 = (1+0.235*beta)/(0.766+0.119*beta);
                                    t2 = 1/(1+0.639*(n-4)/(1+0.71*(n-4)));
                                    t3 = 1/(1+20*alpha/((7.5+alpha)*n*n));
                                    r = r+t1*t2*t3*(r-x[i-2]);
                                }
                                else
                                {
                                    if( i==n-1 )
                                    {
                                        t1 = (1+0.37*beta)/(1.67+0.28*beta);
                                        t2 = 1/(1+0.22*(n-8)/n);
                                        t3 = 1/(1+8*alpha/((6.28+alpha)*n*n));
                                        r = r+t1*t2*t3*(r-x[i-2]);
                                    }
                                }
                            }
                        }
                    }
                }
                alfbet = alpha+beta;
                do
                {
                    temp = 2+alfbet;
                    p1 = (alpha-beta+temp*r)*0.5;
                    p2 = 1;
                    for(j=2; j<=n; j++)
                    {
                        p3 = p2;
                        p2 = p1;
                        temp = 2*j+alfbet;
                        a = 2*j*(j+alfbet)*(temp-2);
                        b = (temp-1)*(alpha*alpha-beta*beta+temp*(temp-2)*r);
                        c = 2*(j-1+alpha)*(j-1+beta)*temp;
                        p1 = (b*p2-c*p3)/a;
                    }
                    pp = (n*(alpha-beta-temp*r)*p1+2*(n+alpha)*(n+beta)*p2)/(temp*(1-r*r));
                    r1 = r;
                    r = r1-p1/pp;
                }
                while( (double)(Math.Abs(r-r1))>=(double)(AP.Math.MachineEpsilon*(1+Math.Abs(r))*100) );
                x[i] = r;
                w[i] = Math.Exp(gammafunc.lngamma(alpha+n, ref tmpsgn)+gammafunc.lngamma(beta+n, ref tmpsgn)-gammafunc.lngamma(n+1, ref tmpsgn)-gammafunc.lngamma(n+alfbet+1, ref tmpsgn))*temp*Math.Pow(2, alfbet)/(pp*p2);
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-2-i; j++)
                {
                    if( (double)(x[j])>=(double)(x[j+1]) )
                    {
                        tmp = x[j];
                        x[j] = x[j+1];
                        x[j+1] = tmp;
                        tmp = w[j];
                        w[j] = w[j+1];
                        w[j+1] = tmp;
                    }
                }
            }
        }


        /*************************************************************************
        Gauss-Laguerre, another variant
        *************************************************************************/
        private static void buildgausslaguerrequadrature(int n,
            double alpha,
            ref double[] x,
            ref double[] w)
        {
            int i = 0;
            int j = 0;
            double r = 0;
            double r1 = 0;
            double p1 = 0;
            double p2 = 0;
            double p3 = 0;
            double dp3 = 0;
            double tsg = 0;
            double tmp = 0;

            x = new double[n-1+1];
            w = new double[n-1+1];
            for(i=0; i<=n-1; i++)
            {
                if( i==0 )
                {
                    r = (1+alpha)*(3+0.92*alpha)/(1+2.4*n+1.8*alpha);
                }
                else
                {
                    if( i==1 )
                    {
                        r = r+(15+6.25*alpha)/(1+0.9*alpha+2.5*n);
                    }
                    else
                    {
                        r = r+((1+2.55*(i-1))/(1.9*(i-1))+1.26*(i-1)*alpha/(1+3.5*(i-1)))/(1+0.3*alpha)*(r-x[i-2]);
                    }
                }
                do
                {
                    p2 = 0;
                    p3 = 1;
                    for(j=0; j<=n-1; j++)
                    {
                        p1 = p2;
                        p2 = p3;
                        p3 = ((-r+2*j+alpha+1)*p2-(j+alpha)*p1)/(j+1);
                    }
                    dp3 = (n*p3-(n+alpha)*p2)/r;
                    r1 = r;
                    r = r-p3/dp3;
                }
                while( (double)(Math.Abs(r-r1))>=(double)(AP.Math.MachineEpsilon*(1+Math.Abs(r))*100) );
                x[i] = r;
                w[i] = -(Math.Exp(gammafunc.lngamma(alpha+n, ref tsg)-gammafunc.lngamma(n, ref tsg))/(dp3*n*p2));
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-2-i; j++)
                {
                    if( (double)(x[j])>=(double)(x[j+1]) )
                    {
                        tmp = x[j];
                        x[j] = x[j+1];
                        x[j+1] = tmp;
                        tmp = w[j];
                        w[j] = w[j+1];
                        w[j+1] = tmp;
                    }
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testgq_test_silent()
        {
            bool result = new bool();

            result = testgqunit(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testgq_test()
        {
            bool result = new bool();

            result = testgqunit(false);
            return result;
        }
    }
}
