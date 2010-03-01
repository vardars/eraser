
using System;

namespace alglib
{
    public class testreflectionsunit
    {
        public static bool testreflections(bool silent)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            int n = 0;
            int m = 0;
            int maxmn = 0;
            double[] x = new double[0];
            double[] v = new double[0];
            double[] work = new double[0];
            double[,] h = new double[0,0];
            double[,] a = new double[0,0];
            double[,] b = new double[0,0];
            double[,] c = new double[0,0];
            double tmp = 0;
            double beta = 0;
            double tau = 0;
            double err = 0;
            double mer = 0;
            double mel = 0;
            double meg = 0;
            int pass = 0;
            int passcount = 0;
            double threshold = 0;
            int tasktype = 0;
            double xscale = 0;
            int i_ = 0;

            passcount = 10;
            threshold = 100*AP.Math.MachineEpsilon;
            mer = 0;
            mel = 0;
            meg = 0;
            for(pass=1; pass<=passcount; pass++)
            {
                for(n=1; n<=10; n++)
                {
                    for(m=1; m<=10; m++)
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
                        x = new double[maxmn+1];
                        v = new double[maxmn+1];
                        work = new double[maxmn+1];
                        h = new double[maxmn+1, maxmn+1];
                        a = new double[maxmn+1, maxmn+1];
                        b = new double[maxmn+1, maxmn+1];
                        c = new double[maxmn+1, maxmn+1];
                        
                        //
                        // GenerateReflection, three tasks are possible:
                        // * random X
                        // * zero X
                        // * non-zero X[1], all other are zeros
                        // * random X, near underflow scale
                        // * random X, near overflow scale
                        //
                        for(tasktype=0; tasktype<=4; tasktype++)
                        {
                            xscale = 1;
                            if( tasktype==0 )
                            {
                                for(i=1; i<=n; i++)
                                {
                                    x[i] = 2*AP.Math.RandomReal()-1;
                                }
                            }
                            if( tasktype==1 )
                            {
                                for(i=1; i<=n; i++)
                                {
                                    x[i] = 0;
                                }
                            }
                            if( tasktype==2 )
                            {
                                x[1] = 2*AP.Math.RandomReal()-1;
                                for(i=2; i<=n; i++)
                                {
                                    x[i] = 0;
                                }
                            }
                            if( tasktype==3 )
                            {
                                for(i=1; i<=n; i++)
                                {
                                    x[i] = (AP.Math.RandomInteger(21)-10)*AP.Math.MinRealNumber;
                                }
                                xscale = 10*AP.Math.MinRealNumber;
                            }
                            if( tasktype==4 )
                            {
                                for(i=1; i<=n; i++)
                                {
                                    x[i] = (2*AP.Math.RandomReal()-1)*AP.Math.MaxRealNumber;
                                }
                                xscale = AP.Math.MaxRealNumber;
                            }
                            for(i_=1; i_<=n;i_++)
                            {
                                v[i_] = x[i_];
                            }
                            reflections.generatereflection(ref v, n, ref tau);
                            beta = v[1];
                            v[1] = 1;
                            for(i=1; i<=n; i++)
                            {
                                for(j=1; j<=n; j++)
                                {
                                    if( i==j )
                                    {
                                        h[i,j] = 1-tau*v[i]*v[j];
                                    }
                                    else
                                    {
                                        h[i,j] = -(tau*v[i]*v[j]);
                                    }
                                }
                            }
                            err = 0;
                            for(i=1; i<=n; i++)
                            {
                                tmp = 0.0;
                                for(i_=1; i_<=n;i_++)
                                {
                                    tmp += h[i,i_]*x[i_];
                                }
                                if( i==1 )
                                {
                                    err = Math.Max(err, Math.Abs(tmp-beta));
                                }
                                else
                                {
                                    err = Math.Max(err, Math.Abs(tmp));
                                }
                            }
                            meg = Math.Max(meg, err/xscale);
                        }
                        
                        //
                        // ApplyReflectionFromTheLeft
                        //
                        for(i=1; i<=m; i++)
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
                        reflections.generatereflection(ref v, m, ref tau);
                        beta = v[1];
                        v[1] = 1;
                        reflections.applyreflectionfromtheleft(ref b, tau, ref v, 1, m, 1, n, ref work);
                        for(i=1; i<=m; i++)
                        {
                            for(j=1; j<=m; j++)
                            {
                                if( i==j )
                                {
                                    h[i,j] = 1-tau*v[i]*v[j];
                                }
                                else
                                {
                                    h[i,j] = -(tau*v[i]*v[j]);
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
                                err = Math.Max(err, Math.Abs(b[i,j]-c[i,j]));
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
                        reflections.generatereflection(ref v, n, ref tau);
                        beta = v[1];
                        v[1] = 1;
                        reflections.applyreflectionfromtheright(ref b, tau, ref v, 1, m, 1, n, ref work);
                        for(i=1; i<=n; i++)
                        {
                            for(j=1; j<=n; j++)
                            {
                                if( i==j )
                                {
                                    h[i,j] = 1-tau*v[i]*v[j];
                                }
                                else
                                {
                                    h[i,j] = -(tau*v[i]*v[j]);
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
                                err = Math.Max(err, Math.Abs(b[i,j]-c[i,j]));
                            }
                        }
                        mer = Math.Max(mer, err);
                    }
                }
            }
            
            //
            // Overflow crash test
            //
            x = new double[10+1];
            v = new double[10+1];
            for(i=1; i<=10; i++)
            {
                v[i] = AP.Math.MaxRealNumber*0.01*(2*AP.Math.RandomReal()-1);
            }
            reflections.generatereflection(ref v, 10, ref tau);
            result = (double)(meg)<=(double)(threshold) & (double)(mel)<=(double)(threshold) & (double)(mer)<=(double)(threshold);
            if( !silent )
            {
                System.Console.Write("TESTING REFLECTIONS");
                System.Console.WriteLine();
                System.Console.Write("Pass count is ");
                System.Console.Write("{0,0:d}",passcount);
                System.Console.WriteLine();
                System.Console.Write("Generate     absolute error is       ");
                System.Console.Write("{0,5:E3}",meg);
                System.Console.WriteLine();
                System.Console.Write("Apply(Left)  absolute error is       ");
                System.Console.Write("{0,5:E3}",mel);
                System.Console.WriteLine();
                System.Console.Write("Apply(Right) absolute error is       ");
                System.Console.Write("{0,5:E3}",mer);
                System.Console.WriteLine();
                System.Console.Write("Overflow crash test passed");
                System.Console.WriteLine();
                if( result )
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
            return result;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testreflectionsunit_test_silent()
        {
            bool result = new bool();

            result = testreflections(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testreflectionsunit_test()
        {
            bool result = new bool();

            result = testreflections(false);
            return result;
        }
    }
}
