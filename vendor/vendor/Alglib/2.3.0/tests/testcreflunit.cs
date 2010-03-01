
using System;

namespace alglib
{
    public class testcreflunit
    {
        public static bool testcrefl(bool silent)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            int k = 0;
            int n = 0;
            int m = 0;
            int maxmn = 0;
            AP.Complex[] x = new AP.Complex[0];
            AP.Complex[] v = new AP.Complex[0];
            AP.Complex[] work = new AP.Complex[0];
            AP.Complex[,] h = new AP.Complex[0,0];
            AP.Complex[,] a = new AP.Complex[0,0];
            AP.Complex[,] b = new AP.Complex[0,0];
            AP.Complex[,] c = new AP.Complex[0,0];
            AP.Complex tmp = 0;
            AP.Complex beta = 0;
            AP.Complex tau = 0;
            double err = 0;
            double mer = 0;
            double mel = 0;
            double meg = 0;
            int pass = 0;
            int passcount = 0;
            bool waserrors = new bool();
            double threshold = 0;
            int i_ = 0;

            threshold = 1000*AP.Math.MachineEpsilon;
            passcount = 1000;
            mer = 0;
            mel = 0;
            meg = 0;
            for(pass=1; pass<=passcount; pass++)
            {
                
                //
                // Task
                //
                n = 1+AP.Math.RandomInteger(10);
                m = 1+AP.Math.RandomInteger(10);
                maxmn = Math.Max(m, n);
                
                //
                // Initialize
                //
                x = new AP.Complex[maxmn+1];
                v = new AP.Complex[maxmn+1];
                work = new AP.Complex[maxmn+1];
                h = new AP.Complex[maxmn+1, maxmn+1];
                a = new AP.Complex[maxmn+1, maxmn+1];
                b = new AP.Complex[maxmn+1, maxmn+1];
                c = new AP.Complex[maxmn+1, maxmn+1];
                
                //
                // GenerateReflection
                //
                for(i=1; i<=n; i++)
                {
                    x[i].x = 2*AP.Math.RandomReal()-1;
                    x[i].y = 2*AP.Math.RandomReal()-1;
                    v[i] = x[i];
                }
                creflections.complexgeneratereflection(ref v, n, ref tau);
                beta = v[1];
                v[1] = 1;
                for(i=1; i<=n; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        if( i==j )
                        {
                            h[i,j] = 1-tau*v[i]*AP.Math.Conj(v[j]);
                        }
                        else
                        {
                            h[i,j] = -(tau*v[i]*AP.Math.Conj(v[j]));
                        }
                    }
                }
                err = 0;
                for(i=1; i<=n; i++)
                {
                    tmp = 0.0;
                    for(i_=1; i_<=n;i_++)
                    {
                        tmp += AP.Math.Conj(h[i_,i])*x[i_];
                    }
                    if( i==1 )
                    {
                        err = Math.Max(err, AP.Math.AbsComplex(tmp-beta));
                    }
                    else
                    {
                        err = Math.Max(err, AP.Math.AbsComplex(tmp));
                    }
                }
                err = Math.Max(err, Math.Abs(beta.y));
                meg = Math.Max(meg, err);
                
                //
                // ApplyReflectionFromTheLeft
                //
                for(i=1; i<=m; i++)
                {
                    x[i].x = 2*AP.Math.RandomReal()-1;
                    x[i].y = 2*AP.Math.RandomReal()-1;
                    v[i] = x[i];
                }
                for(i=1; i<=m; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        a[i,j].x = 2*AP.Math.RandomReal()-1;
                        a[i,j].y = 2*AP.Math.RandomReal()-1;
                        b[i,j] = a[i,j];
                    }
                }
                creflections.complexgeneratereflection(ref v, m, ref tau);
                beta = v[1];
                v[1] = 1;
                creflections.complexapplyreflectionfromtheleft(ref b, tau, ref v, 1, m, 1, n, ref work);
                for(i=1; i<=m; i++)
                {
                    for(j=1; j<=m; j++)
                    {
                        if( i==j )
                        {
                            h[i,j] = 1-tau*v[i]*AP.Math.Conj(v[j]);
                        }
                        else
                        {
                            h[i,j] = -(tau*v[i]*AP.Math.Conj(v[j]));
                        }
                    }
                }
                for(i=1; i<=m; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        tmp = 0.0;
                        for(i_=1; i_<=m;i_++)
                        {
                            tmp += h[i,i_]*a[i_,j];
                        }
                        c[i,j] = tmp;
                    }
                }
                err = 0;
                for(i=1; i<=m; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        err = Math.Max(err, AP.Math.AbsComplex(b[i,j]-c[i,j]));
                    }
                }
                mel = Math.Max(mel, err);
                
                //
                // ApplyReflectionFromTheRight
                //
                for(i=1; i<=n; i++)
                {
                    x[i] = 2*AP.Math.RandomReal()-1;
                    v[i] = x[i];
                }
                for(i=1; i<=m; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        a[i,j] = 2*AP.Math.RandomReal()-1;
                        b[i,j] = a[i,j];
                    }
                }
                creflections.complexgeneratereflection(ref v, n, ref tau);
                beta = v[1];
                v[1] = 1;
                creflections.complexapplyreflectionfromtheright(ref b, tau, ref v, 1, m, 1, n, ref work);
                for(i=1; i<=n; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        if( i==j )
                        {
                            h[i,j] = 1-tau*v[i]*AP.Math.Conj(v[j]);
                        }
                        else
                        {
                            h[i,j] = -(tau*v[i]*AP.Math.Conj(v[j]));
                        }
                    }
                }
                for(i=1; i<=m; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        tmp = 0.0;
                        for(i_=1; i_<=n;i_++)
                        {
                            tmp += a[i,i_]*h[i_,j];
                        }
                        c[i,j] = tmp;
                    }
                }
                err = 0;
                for(i=1; i<=m; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        err = Math.Max(err, AP.Math.AbsComplex(b[i,j]-c[i,j]));
                    }
                }
                mer = Math.Max(mer, err);
            }
            
            //
            // Overflow crash test
            //
            x = new AP.Complex[10+1];
            v = new AP.Complex[10+1];
            for(i=1; i<=10; i++)
            {
                v[i] = AP.Math.MaxRealNumber*0.01*(2*AP.Math.RandomReal()-1);
            }
            creflections.complexgeneratereflection(ref v, 10, ref tau);
            
            //
            // report
            //
            waserrors = (double)(meg)>(double)(threshold) | (double)(mel)>(double)(threshold) | (double)(mer)>(double)(threshold);
            if( !silent )
            {
                System.Console.Write("TESTING COMPLEX REFLECTIONS");
                System.Console.WriteLine();
                System.Console.Write("Generate error:                          ");
                System.Console.Write("{0,5:E3}",meg);
                System.Console.WriteLine();
                System.Console.Write("Apply(L) error:                          ");
                System.Console.Write("{0,5:E3}",mel);
                System.Console.WriteLine();
                System.Console.Write("Apply(R) error:                          ");
                System.Console.Write("{0,5:E3}",mer);
                System.Console.WriteLine();
                System.Console.Write("Threshold:                               ");
                System.Console.Write("{0,5:E3}",threshold);
                System.Console.WriteLine();
                System.Console.Write("Overflow crash test:                     PASSED");
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


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testcreflunit_test_silent()
        {
            bool result = new bool();

            result = testcrefl(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testcreflunit_test()
        {
            bool result = new bool();

            result = testcrefl(false);
            return result;
        }
    }
}
