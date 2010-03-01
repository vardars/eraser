
using System;

namespace alglib
{
    public class testhtridiagonalunit
    {
        public static bool testhtridiagonal(bool silent)
        {
            bool result = new bool();
            int pass = 0;
            int passcount = 0;
            int maxn = 0;
            double materr = 0;
            double orterr = 0;
            int n = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            AP.Complex[,] a = new AP.Complex[0,0];
            double threshold = 0;
            bool waserrors = new bool();

            materr = 0;
            orterr = 0;
            waserrors = false;
            threshold = 1000*AP.Math.MachineEpsilon;
            maxn = 15;
            passcount = 20;
            for(n=1; n<=maxn; n++)
            {
                a = new AP.Complex[n-1+1, n-1+1];
                
                //
                // Test zero matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a[i,j] = 0;
                    }
                }
                testhtdproblem(ref a, n, ref materr, ref orterr);
                
                //
                // Test other matrix types
                //
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // Test dense matrix
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        a[i,i] = 2*AP.Math.RandomReal()-1;
                        for(j=i+1; j<=n-1; j++)
                        {
                            a[i,j].x = 2*AP.Math.RandomReal()-1;
                            a[i,j].y = 2*AP.Math.RandomReal()-1;
                            a[j,i] = AP.Math.Conj(a[i,j]);
                        }
                    }
                    testhtdproblem(ref a, n, ref materr, ref orterr);
                    
                    //
                    // Diagonal matrix
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        a[i,i] = 2*AP.Math.RandomReal()-1;
                        for(j=i+1; j<=n-1; j++)
                        {
                            a[i,j] = 0;
                            a[j,i] = 0;
                        }
                    }
                    testhtdproblem(ref a, n, ref materr, ref orterr);
                    
                    //
                    // sparse matrix
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        a[i,i] = 0;
                        for(j=i+1; j<=n-1; j++)
                        {
                            a[i,j] = 0;
                            a[j,i] = 0;
                        }
                    }
                    for(k=1; k<=2; k++)
                    {
                        i = AP.Math.RandomInteger(n);
                        j = AP.Math.RandomInteger(n);
                        if( i==j )
                        {
                            a[i,j] = 2*AP.Math.RandomReal()-1;
                        }
                        else
                        {
                            a[i,j].x = 2*AP.Math.RandomReal()-1;
                            a[i,j].y = 2*AP.Math.RandomReal()-1;
                            a[j,i] = AP.Math.Conj(a[i,j]);
                        }
                    }
                    testhtdproblem(ref a, n, ref materr, ref orterr);
                }
            }
            
            //
            // report
            //
            waserrors = (double)(materr)>(double)(threshold) | (double)(orterr)>(double)(threshold);
            if( !silent )
            {
                System.Console.Write("TESTING HERMITIAN TO TRIDIAGONAL");
                System.Console.WriteLine();
                System.Console.Write("Matrix error:                            ");
                System.Console.Write("{0,5:E3}",materr);
                System.Console.WriteLine();
                System.Console.Write("Q orthogonality error:                   ");
                System.Console.Write("{0,5:E3}",orterr);
                System.Console.WriteLine();
                System.Console.Write("Threshold:                               ");
                System.Console.Write("{0,5:E3}",threshold);
                System.Console.WriteLine();
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


        private static void testhtdproblem(ref AP.Complex[,] a,
            int n,
            ref double materr,
            ref double orterr)
        {
            int i = 0;
            int j = 0;
            AP.Complex[,] ua = new AP.Complex[0,0];
            AP.Complex[,] la = new AP.Complex[0,0];
            AP.Complex[,] t = new AP.Complex[0,0];
            AP.Complex[,] q = new AP.Complex[0,0];
            AP.Complex[,] t2 = new AP.Complex[0,0];
            AP.Complex[,] t3 = new AP.Complex[0,0];
            AP.Complex[] tau = new AP.Complex[0];
            double[] d = new double[0];
            double[] e = new double[0];
            AP.Complex v = 0;
            int i_ = 0;

            ua = new AP.Complex[n-1+1, n-1+1];
            la = new AP.Complex[n-1+1, n-1+1];
            t = new AP.Complex[n-1+1, n-1+1];
            q = new AP.Complex[n-1+1, n-1+1];
            t2 = new AP.Complex[n-1+1, n-1+1];
            t3 = new AP.Complex[n-1+1, n-1+1];
            
            //
            // fill
            //
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    ua[i,j] = 0;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=i; j<=n-1; j++)
                {
                    ua[i,j] = a[i,j];
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    la[i,j] = 0;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=i; j++)
                {
                    la[i,j] = a[i,j];
                }
            }
            
            //
            // Test 2tridiagonal: upper
            //
            htridiagonal.hmatrixtd(ref ua, n, true, ref tau, ref d, ref e);
            htridiagonal.hmatrixtdunpackq(ref ua, n, true, ref tau, ref q);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    t[i,j] = 0;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                t[i,i] = d[i];
            }
            for(i=0; i<=n-2; i++)
            {
                t[i,i+1] = e[i];
                t[i+1,i] = e[i];
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += AP.Math.Conj(q[i_,i])*a[i_,j];
                    }
                    t2[i,j] = v;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += t2[i,i_]*q[i_,j];
                    }
                    t3[i,j] = v;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    materr = Math.Max(materr, AP.Math.AbsComplex(t3[i,j]-t[i,j]));
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += q[i,i_]*AP.Math.Conj(q[j,i_]);
                    }
                    if( i==j )
                    {
                        orterr = Math.Max(orterr, AP.Math.AbsComplex(v-1));
                    }
                    else
                    {
                        orterr = Math.Max(orterr, AP.Math.AbsComplex(v));
                    }
                }
            }
            
            //
            // Test 2tridiagonal: lower
            //
            htridiagonal.hmatrixtd(ref la, n, false, ref tau, ref d, ref e);
            htridiagonal.hmatrixtdunpackq(ref la, n, false, ref tau, ref q);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    t[i,j] = 0;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                t[i,i] = d[i];
            }
            for(i=0; i<=n-2; i++)
            {
                t[i,i+1] = e[i];
                t[i+1,i] = e[i];
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += AP.Math.Conj(q[i_,i])*a[i_,j];
                    }
                    t2[i,j] = v;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += t2[i,i_]*q[i_,j];
                    }
                    t3[i,j] = v;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    materr = Math.Max(materr, AP.Math.AbsComplex(t3[i,j]-t[i,j]));
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += q[i,i_]*AP.Math.Conj(q[j,i_]);
                    }
                    if( i==j )
                    {
                        orterr = Math.Max(orterr, AP.Math.AbsComplex(v-1));
                    }
                    else
                    {
                        orterr = Math.Max(orterr, AP.Math.AbsComplex(v));
                    }
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testhtridiagonalunit_test_silent()
        {
            bool result = new bool();

            result = testhtridiagonal(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testhtridiagonalunit_test()
        {
            bool result = new bool();

            result = testhtridiagonal(false);
            return result;
        }
    }
}
