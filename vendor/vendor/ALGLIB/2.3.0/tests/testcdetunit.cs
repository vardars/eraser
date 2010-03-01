
using System;

namespace alglib
{
    public class testcdetunit
    {
        public static bool deterrors = new bool();


        /*************************************************************************
        Main unittest subroutine
        *************************************************************************/
        public static bool testcdet(bool silent)
        {
            bool result = new bool();
            int maxn = 0;
            int gpasscount = 0;
            double threshold = 0;
            AP.Complex[,] a = new AP.Complex[0,0];
            int n = 0;
            int gpass = 0;
            int i = 0;
            int j = 0;
            bool waserrors = new bool();

            deterrors = false;
            waserrors = false;
            maxn = 8;
            gpasscount = 5;
            threshold = 5*100*AP.Math.MachineEpsilon;
            a = new AP.Complex[maxn-1+1, maxn-1+1];
            
            //
            // Different problems
            //
            for(gpass=1; gpass<=gpasscount; gpass++)
            {
                
                //
                // zero matrix, several cases
                //
                for(i=0; i<=maxn-1; i++)
                {
                    for(j=0; j<=maxn-1; j++)
                    {
                        a[i,j] = 0;
                    }
                }
                for(i=1; i<=maxn; i++)
                {
                    testproblem(ref a, i);
                }
                
                //
                // Dense matrices
                //
                for(n=1; n<=maxn; n++)
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j].x = 2*AP.Math.RandomReal()-1;
                            a[i,j].y = 2*AP.Math.RandomReal()-1;
                        }
                    }
                    testproblem(ref a, n);
                }
            }
            
            //
            // report
            //
            waserrors = deterrors;
            if( !silent )
            {
                System.Console.Write("TESTING DET");
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
        Sparse matrix
        *************************************************************************/
        private static void fillsparsea(ref AP.Complex[,] a,
            int m,
            int n,
            double sparcity)
        {
            int i = 0;
            int j = 0;

            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( (double)(AP.Math.RandomReal())>=(double)(sparcity) )
                    {
                        a[i,j].x = 2*AP.Math.RandomReal()-1;
                        a[i,j].y = 2*AP.Math.RandomReal()-1;
                    }
                    else
                    {
                        a[i,j] = 0;
                    }
                }
            }
        }


        /*************************************************************************
        Problem testing
        *************************************************************************/
        private static void testproblem(ref AP.Complex[,] a,
            int n)
        {
            int i = 0;
            int j = 0;
            AP.Complex[,] b = new AP.Complex[0,0];
            AP.Complex[,] c = new AP.Complex[0,0];
            int[] pivots = new int[0];
            AP.Complex v1 = 0;
            AP.Complex v2 = 0;
            AP.Complex ve = 0;

            b = new AP.Complex[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    b[i,j] = a[i-1,j-1];
                }
            }
            ve = dettriangle(b, n);
            v1 = cdet.cmatrixdet(a, n);
            makeacopy(ref a, n, n, ref c);
            trfac.cmatrixlu(ref c, n, n, ref pivots);
            v2 = cdet.cmatrixludet(ref c, ref pivots, n);
            if( ve!=0 )
            {
                deterrors = deterrors | (double)(AP.Math.AbsComplex((v1-ve)/Math.Max(AP.Math.AbsComplex(ve), 1)))>(double)(1.0E-9);
                deterrors = deterrors | (double)(AP.Math.AbsComplex((v1-ve)/Math.Max(AP.Math.AbsComplex(ve), 1)))>(double)(1.0E-9);
            }
            else
            {
                deterrors = deterrors | v1!=ve;
                deterrors = deterrors | v2!=ve;
            }
        }


        /*************************************************************************
        Copy
        *************************************************************************/
        private static void makeacopy(ref AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[,] b)
        {
            int i = 0;
            int j = 0;

            b = new AP.Complex[m-1+1, n-1+1];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    b[i,j] = a[i,j];
                }
            }
        }


        /*************************************************************************
        Basic det subroutine
        *************************************************************************/
        private static AP.Complex dettriangle(AP.Complex[,] a,
            int n)
        {
            AP.Complex result = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int l = 0;
            int f = 0;
            int z = 0;
            AP.Complex t = 0;
            AP.Complex m1 = 0;

            a = (AP.Complex[,])a.Clone();

            result = 1;
            k = 1;
            do
            {
                m1 = 0;
                i = k;
                while( i<=n )
                {
                    t = a[i,k];
                    if( (double)(AP.Math.AbsComplex(t))>(double)(AP.Math.AbsComplex(m1)) )
                    {
                        m1 = t;
                        j = i;
                    }
                    i = i+1;
                }
                if( (double)(AP.Math.AbsComplex(m1))==(double)(0) )
                {
                    result = 0;
                    k = n+1;
                }
                else
                {
                    if( j!=k )
                    {
                        result = -result;
                        l = k;
                        while( l<=n )
                        {
                            t = a[j,l];
                            a[j,l] = a[k,l];
                            a[k,l] = t;
                            l = l+1;
                        }
                    }
                    f = k+1;
                    while( f<=n )
                    {
                        t = a[f,k]/m1;
                        z = k+1;
                        while( z<=n )
                        {
                            a[f,z] = a[f,z]-t*a[k,z];
                            z = z+1;
                        }
                        f = f+1;
                    }
                    result = result*a[k,k];
                }
                k = k+1;
            }
            while( k<=n );
            return result;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testcdetunit_test_silent()
        {
            bool result = new bool();

            result = testcdet(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testcdetunit_test()
        {
            bool result = new bool();

            result = testcdet(false);
            return result;
        }
    }
}
