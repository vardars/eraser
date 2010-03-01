
using System;

namespace alglib
{
    public class testpcaunit
    {
        public static bool testpca(bool silent)
        {
            bool result = new bool();
            int passcount = 0;
            int maxn = 0;
            int maxm = 0;
            double threshold = 0;
            int m = 0;
            int n = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int info = 0;
            double[] means = new double[0];
            double[] s = new double[0];
            double[] t2 = new double[0];
            double[] t3 = new double[0];
            double[,] v = new double[0,0];
            double[,] x = new double[0,0];
            double t = 0;
            double h = 0;
            double tmean = 0;
            double tmeans = 0;
            double tstddev = 0;
            double tstddevs = 0;
            double tmean2 = 0;
            double tmeans2 = 0;
            double tstddev2 = 0;
            double tstddevs2 = 0;
            bool pcaconverrors = new bool();
            bool pcaorterrors = new bool();
            bool pcavarerrors = new bool();
            bool pcaopterrors = new bool();
            bool waserrors = new bool();
            int i_ = 0;

            
            //
            // Primary settings
            //
            maxm = 10;
            maxn = 100;
            passcount = 1;
            threshold = 1000*AP.Math.MachineEpsilon;
            waserrors = false;
            pcaconverrors = false;
            pcaorterrors = false;
            pcavarerrors = false;
            pcaopterrors = false;
            
            //
            // Test 1: N random points in M-dimensional space
            //
            for(m=1; m<=maxm; m++)
            {
                for(n=1; n<=maxn; n++)
                {
                    
                    //
                    // Generate task
                    //
                    x = new double[n-1+1, m-1+1];
                    means = new double[m-1+1];
                    for(j=0; j<=m-1; j++)
                    {
                        means[j] = 1.5*AP.Math.RandomReal()-0.75;
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            x[i,j] = means[j]+(2*AP.Math.RandomReal()-1);
                        }
                    }
                    
                    //
                    // Solve
                    //
                    pca.pcabuildbasis(ref x, n, m, ref info, ref s, ref v);
                    if( info!=1 )
                    {
                        pcaconverrors = true;
                        continue;
                    }
                    
                    //
                    // Orthogonality test
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=m-1; j++)
                        {
                            t = 0.0;
                            for(i_=0; i_<=m-1;i_++)
                            {
                                t += v[i_,i]*v[i_,j];
                            }
                            if( i==j )
                            {
                                t = t-1;
                            }
                            pcaorterrors = pcaorterrors | (double)(Math.Abs(t))>(double)(threshold);
                        }
                    }
                    
                    //
                    // Variance test
                    //
                    t2 = new double[n-1+1];
                    for(k=0; k<=m-1; k++)
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            t = 0.0;
                            for(i_=0; i_<=m-1;i_++)
                            {
                                t += x[i,i_]*v[i_,k];
                            }
                            t2[i] = t;
                        }
                        calculatemv(ref t2, n, ref tmean, ref tmeans, ref tstddev, ref tstddevs);
                        if( n!=1 )
                        {
                            t = AP.Math.Sqr(tstddev)*n/(n-1);
                        }
                        else
                        {
                            t = 0;
                        }
                        pcavarerrors = pcavarerrors | (double)(Math.Abs(t-s[k]))>(double)(threshold);
                    }
                    for(k=0; k<=m-2; k++)
                    {
                        pcavarerrors = pcavarerrors | (double)(s[k])<(double)(s[k+1]);
                    }
                    
                    //
                    // Optimality: different perturbations in V[..,0] can't
                    // increase variance of projection - can only decrease.
                    //
                    t2 = new double[n-1+1];
                    t3 = new double[n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        t = 0.0;
                        for(i_=0; i_<=m-1;i_++)
                        {
                            t += x[i,i_]*v[i_,0];
                        }
                        t2[i] = t;
                    }
                    calculatemv(ref t2, n, ref tmean, ref tmeans, ref tstddev, ref tstddevs);
                    for(k=0; k<=2*m-1; k++)
                    {
                        h = 0.001;
                        if( k%2!=0 )
                        {
                            h = -h;
                        }
                        for(i_=0; i_<=n-1;i_++)
                        {
                            t3[i_] = t2[i_];
                        }
                        for(i_=0; i_<=n-1;i_++)
                        {
                            t3[i_] = t3[i_] + h*x[i_,k/2];
                        }
                        t = 0;
                        for(j=0; j<=m-1; j++)
                        {
                            if( j!=k/2 )
                            {
                                t = t+AP.Math.Sqr(v[j,0]);
                            }
                            else
                            {
                                t = t+AP.Math.Sqr(v[j,0]+h);
                            }
                        }
                        t = 1/Math.Sqrt(t);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            t3[i_] = t*t3[i_];
                        }
                        calculatemv(ref t3, n, ref tmean2, ref tmeans2, ref tstddev2, ref tstddevs2);
                        pcaopterrors = pcaopterrors | (double)(tstddev2)>(double)(tstddev+threshold);
                    }
                }
            }
            
            //
            // Special test for N=0
            //
            for(m=1; m<=maxm; m++)
            {
                
                //
                // Solve
                //
                pca.pcabuildbasis(ref x, 0, m, ref info, ref s, ref v);
                if( info!=1 )
                {
                    pcaconverrors = true;
                    continue;
                }
                
                //
                // Orthogonality test
                //
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        t = 0.0;
                        for(i_=0; i_<=m-1;i_++)
                        {
                            t += v[i_,i]*v[i_,j];
                        }
                        if( i==j )
                        {
                            t = t-1;
                        }
                        pcaorterrors = pcaorterrors | (double)(Math.Abs(t))>(double)(threshold);
                    }
                }
            }
            
            //
            // Final report
            //
            waserrors = pcaconverrors | pcaorterrors | pcavarerrors | pcaopterrors;
            if( !silent )
            {
                System.Console.Write("PCA TEST");
                System.Console.WriteLine();
                System.Console.Write("TOTAL RESULTS:                           ");
                if( !waserrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("* CONVERGENCE                            ");
                if( !pcaconverrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("* ORTOGONALITY                           ");
                if( !pcaorterrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("* VARIANCE REPORT                        ");
                if( !pcavarerrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("* OPTIMALITY                             ");
                if( !pcaopterrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                if( waserrors )
                {
                    System.Console.Write("TEST SUMMARY: FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("TEST SUMMARY: PASSED");
                    System.Console.WriteLine();
                }
                System.Console.WriteLine();
                System.Console.WriteLine();
            }
            result = !waserrors;
            return result;
        }


        /*************************************************************************
        Moments estimates and their errors
        *************************************************************************/
        private static void calculatemv(ref double[] x,
            int n,
            ref double mean,
            ref double means,
            ref double stddev,
            ref double stddevs)
        {
            int i = 0;
            double v = 0;
            double v1 = 0;
            double v2 = 0;
            double variance = 0;

            mean = 0;
            means = 1;
            stddev = 0;
            stddevs = 1;
            variance = 0;
            if( n<=1 )
            {
                return;
            }
            
            //
            // Mean
            //
            for(i=0; i<=n-1; i++)
            {
                mean = mean+x[i];
            }
            mean = mean/n;
            
            //
            // Variance (using corrected two-pass algorithm)
            //
            if( n!=1 )
            {
                v1 = 0;
                for(i=0; i<=n-1; i++)
                {
                    v1 = v1+AP.Math.Sqr(x[i]-mean);
                }
                v2 = 0;
                for(i=0; i<=n-1; i++)
                {
                    v2 = v2+(x[i]-mean);
                }
                v2 = AP.Math.Sqr(v2)/n;
                variance = (v1-v2)/n;
                if( (double)(variance)<(double)(0) )
                {
                    variance = 0;
                }
                stddev = Math.Sqrt(variance);
            }
            
            //
            // Errors
            //
            means = stddev/Math.Sqrt(n);
            stddevs = stddev*Math.Sqrt(2)/Math.Sqrt(n-1);
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testpcaunit_test_silent()
        {
            bool result = new bool();

            result = testpca(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testpcaunit_test()
        {
            bool result = new bool();

            result = testpca(false);
            return result;
        }
    }
}
