
using System;

namespace alglib
{
    public class testnsevdunit
    {
        public static bool testnonsymmetricevd(bool silent)
        {
            bool result = new bool();
            double[,] a = new double[0,0];
            int n = 0;
            int i = 0;
            int j = 0;
            int gpass = 0;
            bool waserrors = new bool();
            bool wfailed = new bool();
            double vecerr = 0;
            double valonlydiff = 0;
            double threshold = 0;

            vecerr = 0;
            valonlydiff = 0;
            wfailed = false;
            waserrors = false;
            threshold = 1000*AP.Math.MachineEpsilon;
            
            //
            // First set: N = 1..10
            //
            for(n=1; n<=10; n++)
            {
                a = new double[n-1+1, n-1+1];
                
                //
                // zero matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a[i,j] = 0;
                    }
                }
                testnsevdproblem(ref a, n, ref vecerr, ref valonlydiff, ref wfailed);
                
                //
                // Dense and sparse matrices
                //
                for(gpass=1; gpass<=1; gpass++)
                {
                    
                    //
                    // Dense matrix
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 2*AP.Math.RandomReal()-1;
                        }
                    }
                    testnsevdproblem(ref a, n, ref vecerr, ref valonlydiff, ref wfailed);
                    
                    //
                    // Very matrix
                    //
                    fillsparsea(ref a, n, 0.98);
                    testnsevdproblem(ref a, n, ref vecerr, ref valonlydiff, ref wfailed);
                    
                    //
                    // Incredible sparse matrix
                    //
                    fillsparsea(ref a, n, 0.995);
                    testnsevdproblem(ref a, n, ref vecerr, ref valonlydiff, ref wfailed);
                }
            }
            
            //
            // Second set: N = 70..72
            //
            for(n=70; n<=72; n++)
            {
                a = new double[n-1+1, n-1+1];
                
                //
                // zero matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a[i,j] = 0;
                    }
                }
                testnsevdproblem(ref a, n, ref vecerr, ref valonlydiff, ref wfailed);
                
                //
                // Dense and sparse matrices
                //
                for(gpass=1; gpass<=1; gpass++)
                {
                    
                    //
                    // Dense matrix
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 2*AP.Math.RandomReal()-1;
                        }
                    }
                    testnsevdproblem(ref a, n, ref vecerr, ref valonlydiff, ref wfailed);
                    
                    //
                    // Very matrix
                    //
                    fillsparsea(ref a, n, 0.98);
                    testnsevdproblem(ref a, n, ref vecerr, ref valonlydiff, ref wfailed);
                    
                    //
                    // Incredible sparse matrix
                    //
                    fillsparsea(ref a, n, 0.995);
                    testnsevdproblem(ref a, n, ref vecerr, ref valonlydiff, ref wfailed);
                }
            }
            
            //
            // report
            //
            waserrors = (double)(valonlydiff)>(double)(1000*threshold) | (double)(vecerr)>(double)(threshold) | wfailed;
            if( !silent )
            {
                System.Console.Write("TESTING NONSYMMETTRIC EVD");
                System.Console.WriteLine();
                System.Console.Write("Av-lambdav error:                        ");
                System.Console.Write("{0,5:E3}",vecerr);
                System.Console.WriteLine();
                System.Console.Write("Values only difference:                  ");
                System.Console.Write("{0,5:E3}",valonlydiff);
                System.Console.WriteLine();
                System.Console.Write("Always converged:                        ");
                if( !wfailed )
                {
                    System.Console.Write("YES");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("NO");
                    System.Console.WriteLine();
                }
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


        private static void fillsparsea(ref double[,] a,
            int n,
            double sparcity)
        {
            int i = 0;
            int j = 0;

            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( (double)(AP.Math.RandomReal())>=(double)(sparcity) )
                    {
                        a[i,j] = 2*AP.Math.RandomReal()-1;
                    }
                    else
                    {
                        a[i,j] = 0;
                    }
                }
            }
        }


        private static void testnsevdproblem(ref double[,] a,
            int n,
            ref double vecerr,
            ref double valonlydiff,
            ref bool wfailed)
        {
            double mx = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int vjob = 0;
            bool needl = new bool();
            bool needr = new bool();
            double[] wr0 = new double[0];
            double[] wi0 = new double[0];
            double[] wr1 = new double[0];
            double[] wi1 = new double[0];
            double[] wr0s = new double[0];
            double[] wi0s = new double[0];
            double[] wr1s = new double[0];
            double[] wi1s = new double[0];
            double[,] vl = new double[0,0];
            double[,] vr = new double[0,0];
            double[] vec1r = new double[0];
            double[] vec1i = new double[0];
            double[] vec2r = new double[0];
            double[] vec2i = new double[0];
            double[] vec3r = new double[0];
            double[] vec3i = new double[0];
            double curwr = 0;
            double curwi = 0;
            double vt = 0;
            double tmp = 0;
            int i_ = 0;

            vec1r = new double[n-1+1];
            vec2r = new double[n-1+1];
            vec3r = new double[n-1+1];
            vec1i = new double[n-1+1];
            vec2i = new double[n-1+1];
            vec3i = new double[n-1+1];
            wr0s = new double[n-1+1];
            wr1s = new double[n-1+1];
            wi0s = new double[n-1+1];
            wi1s = new double[n-1+1];
            mx = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( (double)(Math.Abs(a[i,j]))>(double)(mx) )
                    {
                        mx = Math.Abs(a[i,j]);
                    }
                }
            }
            if( (double)(mx)==(double)(0) )
            {
                mx = 1;
            }
            
            //
            // Load values-only
            //
            if( !nsevd.rmatrixevd(a, n, 0, ref wr0, ref wi0, ref vl, ref vr) )
            {
                wfailed = false;
                return;
            }
            
            //
            // Test different jobs
            //
            for(vjob=1; vjob<=3; vjob++)
            {
                needr = vjob==1 | vjob==3;
                needl = vjob==2 | vjob==3;
                if( !nsevd.rmatrixevd(a, n, vjob, ref wr1, ref wi1, ref vl, ref vr) )
                {
                    wfailed = false;
                    return;
                }
                
                //
                // Test values:
                // 1. sort by real part
                // 2. test
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    wr0s[i_] = wr0[i_];
                }
                for(i_=0; i_<=n-1;i_++)
                {
                    wi0s[i_] = wi0[i_];
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-2-i; j++)
                    {
                        if( (double)(wr0s[j])>(double)(wr0s[j+1]) )
                        {
                            tmp = wr0s[j];
                            wr0s[j] = wr0s[j+1];
                            wr0s[j+1] = tmp;
                            tmp = wi0s[j];
                            wi0s[j] = wi0s[j+1];
                            wi0s[j+1] = tmp;
                        }
                    }
                }
                for(i_=0; i_<=n-1;i_++)
                {
                    wr1s[i_] = wr1[i_];
                }
                for(i_=0; i_<=n-1;i_++)
                {
                    wi1s[i_] = wi1[i_];
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-2-i; j++)
                    {
                        if( (double)(wr1s[j])>(double)(wr1s[j+1]) )
                        {
                            tmp = wr1s[j];
                            wr1s[j] = wr1s[j+1];
                            wr1s[j+1] = tmp;
                            tmp = wi1s[j];
                            wi1s[j] = wi1s[j+1];
                            wi1s[j+1] = tmp;
                        }
                    }
                }
                for(i=0; i<=n-1; i++)
                {
                    valonlydiff = Math.Max(valonlydiff, Math.Abs(wr0s[i]-wr1s[i]));
                    valonlydiff = Math.Max(valonlydiff, Math.Abs(wi0s[i]-wi1s[i]));
                }
                
                //
                // Test right vectors
                //
                if( needr )
                {
                    k = 0;
                    while( k<=n-1 )
                    {
                        if( (double)(wi1[k])==(double)(0) )
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vec1r[i_] = vr[i_,k];
                            }
                            for(i=0; i<=n-1; i++)
                            {
                                vec1i[i] = 0;
                            }
                            curwr = wr1[k];
                            curwi = 0;
                        }
                        if( (double)(wi1[k])>(double)(0) )
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vec1r[i_] = vr[i_,k];
                            }
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vec1i[i_] = vr[i_,k+1];
                            }
                            curwr = wr1[k];
                            curwi = wi1[k];
                        }
                        if( (double)(wi1[k])<(double)(0) )
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vec1r[i_] = vr[i_,k-1];
                            }
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vec1i[i_] = -vr[i_,k];
                            }
                            curwr = wr1[k];
                            curwi = wi1[k];
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            vt = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vt += a[i,i_]*vec1r[i_];
                            }
                            vec2r[i] = vt;
                            vt = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vt += a[i,i_]*vec1i[i_];
                            }
                            vec2i[i] = vt;
                        }
                        for(i_=0; i_<=n-1;i_++)
                        {
                            vec3r[i_] = curwr*vec1r[i_];
                        }
                        for(i_=0; i_<=n-1;i_++)
                        {
                            vec3r[i_] = vec3r[i_] - curwi*vec1i[i_];
                        }
                        for(i_=0; i_<=n-1;i_++)
                        {
                            vec3i[i_] = curwi*vec1r[i_];
                        }
                        for(i_=0; i_<=n-1;i_++)
                        {
                            vec3i[i_] = vec3i[i_] + curwr*vec1i[i_];
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            vecerr = Math.Max(vecerr, Math.Abs(vec2r[i]-vec3r[i]));
                            vecerr = Math.Max(vecerr, Math.Abs(vec2i[i]-vec3i[i]));
                        }
                        k = k+1;
                    }
                }
                
                //
                // Test left vectors
                //
                if( needl )
                {
                    k = 0;
                    while( k<=n-1 )
                    {
                        if( (double)(wi1[k])==(double)(0) )
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vec1r[i_] = vl[i_,k];
                            }
                            for(i=0; i<=n-1; i++)
                            {
                                vec1i[i] = 0;
                            }
                            curwr = wr1[k];
                            curwi = 0;
                        }
                        if( (double)(wi1[k])>(double)(0) )
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vec1r[i_] = vl[i_,k];
                            }
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vec1i[i_] = vl[i_,k+1];
                            }
                            curwr = wr1[k];
                            curwi = wi1[k];
                        }
                        if( (double)(wi1[k])<(double)(0) )
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vec1r[i_] = vl[i_,k-1];
                            }
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vec1i[i_] = -vl[i_,k];
                            }
                            curwr = wr1[k];
                            curwi = wi1[k];
                        }
                        for(j=0; j<=n-1; j++)
                        {
                            vt = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vt += vec1r[i_]*a[i_,j];
                            }
                            vec2r[j] = vt;
                            vt = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vt += vec1i[i_]*a[i_,j];
                            }
                            vec2i[j] = -vt;
                        }
                        for(i_=0; i_<=n-1;i_++)
                        {
                            vec3r[i_] = curwr*vec1r[i_];
                        }
                        for(i_=0; i_<=n-1;i_++)
                        {
                            vec3r[i_] = vec3r[i_] + curwi*vec1i[i_];
                        }
                        for(i_=0; i_<=n-1;i_++)
                        {
                            vec3i[i_] = curwi*vec1r[i_];
                        }
                        for(i_=0; i_<=n-1;i_++)
                        {
                            vec3i[i_] = vec3i[i_] - curwr*vec1i[i_];
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            vecerr = Math.Max(vecerr, Math.Abs(vec2r[i]-vec3r[i]));
                            vecerr = Math.Max(vecerr, Math.Abs(vec2i[i]-vec3i[i]));
                        }
                        k = k+1;
                    }
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testnsevdunit_test_silent()
        {
            bool result = new bool();

            result = testnonsymmetricevd(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testnsevdunit_test()
        {
            bool result = new bool();

            result = testnonsymmetricevd(false);
            return result;
        }
    }
}
