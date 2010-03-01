
using System;

namespace alglib
{
    public class testldltunit
    {
        public static bool testldlt(bool silent)
        {
            bool result = new bool();
            double[,] a = new double[0,0];
            double[,] a2 = new double[0,0];
            double[,] l = new double[0,0];
            double[,] d = new double[0,0];
            double[,] u = new double[0,0];
            double[,] t = new double[0,0];
            double[,] t2 = new double[0,0];
            int[] p = new int[0];
            int n = 0;
            int pass = 0;
            int mtask = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int minij = 0;
            bool upperin = new bool();
            bool cr = new bool();
            double v = 0;
            double err = 0;
            bool waserrors = new bool();
            int passcount = 0;
            int maxn = 0;
            int htask = 0;
            double threshold = 0;
            int i_ = 0;

            err = 0;
            passcount = 10;
            maxn = 20;
            threshold = 1000*AP.Math.MachineEpsilon;
            waserrors = false;
            
            //
            // Test
            //
            for(n=1; n<=maxn; n++)
            {
                a = new double[n-1+1, n-1+1];
                a2 = new double[n-1+1, n-1+1];
                l = new double[n-1+1, n-1+1];
                u = new double[n-1+1, n-1+1];
                d = new double[n-1+1, n-1+1];
                t = new double[n-1+1, n-1+1];
                t2 = new double[n-1+1, n-1+1];
                for(mtask=0; mtask<=2; mtask++)
                {
                    for(htask=0; htask<=1; htask++)
                    {
                        for(pass=1; pass<=passcount; pass++)
                        {
                            upperin = htask==0;
                            
                            //
                            // Prepare task:
                            // * A contains symmetric matrix
                            // * A2 contains its upper (or lower) half
                            //
                            generatematrix(ref a, n, mtask);
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    a2[i,j] = a[i,j];
                                }
                            }
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    if( upperin )
                                    {
                                        if( j<i )
                                        {
                                            a2[i,j] = 0;
                                        }
                                    }
                                    else
                                    {
                                        if( i<j )
                                        {
                                            a2[i,j] = 0;
                                        }
                                    }
                                }
                            }
                            
                            //
                            // LDLt
                            //
                            ldlt.smatrixldlt(ref a2, n, upperin, ref p);
                            
                            //
                            // Test (upper or lower)
                            //
                            if( upperin )
                            {
                                
                                //
                                // Unpack D
                                //
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        d[i,j] = 0;
                                    }
                                }
                                k = 0;
                                while( k<n )
                                {
                                    if( p[k]>=0 )
                                    {
                                        d[k,k] = a2[k,k];
                                        k = k+1;
                                    }
                                    else
                                    {
                                        d[k,k] = a2[k,k];
                                        d[k,k+1] = a2[k,k+1];
                                        d[k+1,k] = a2[k,k+1];
                                        d[k+1,k+1] = a2[k+1,k+1];
                                        k = k+2;
                                    }
                                }
                                
                                //
                                // Unpack U
                                //
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        u[i,j] = 0;
                                    }
                                    u[i,i] = 1;
                                }
                                k = 0;
                                while( k<n )
                                {
                                    
                                    //
                                    // unpack Uk
                                    //
                                    for(i=0; i<=n-1; i++)
                                    {
                                        for(j=0; j<=n-1; j++)
                                        {
                                            t[i,j] = 0;
                                        }
                                        t[i,i] = 1;
                                    }
                                    if( p[k]>=0 )
                                    {
                                        for(i=0; i<=k-1; i++)
                                        {
                                            t[i,k] = a2[i,k];
                                        }
                                    }
                                    else
                                    {
                                        for(i=0; i<=k-1; i++)
                                        {
                                            t[i,k] = a2[i,k];
                                            t[i,k+1] = a2[i,k+1];
                                        }
                                    }
                                    
                                    //
                                    // multiply U
                                    //
                                    for(i=0; i<=n-1; i++)
                                    {
                                        for(j=0; j<=n-1; j++)
                                        {
                                            v = 0.0;
                                            for(i_=0; i_<=n-1;i_++)
                                            {
                                                v += t[i,i_]*u[i_,j];
                                            }
                                            t2[i,j] = v;
                                        }
                                    }
                                    for(i=0; i<=n-1; i++)
                                    {
                                        for(j=0; j<=n-1; j++)
                                        {
                                            u[i,j] = t2[i,j];
                                        }
                                    }
                                    
                                    //
                                    // permutations
                                    //
                                    if( p[k]>=0 )
                                    {
                                        for(j=0; j<=n-1; j++)
                                        {
                                            v = u[k,j];
                                            u[k,j] = u[p[k],j];
                                            u[p[k],j] = v;
                                        }
                                        k = k+1;
                                    }
                                    else
                                    {
                                        for(j=0; j<=n-1; j++)
                                        {
                                            v = u[k,j];
                                            u[k,j] = u[n+p[k],j];
                                            u[n+p[k],j] = v;
                                        }
                                        k = k+2;
                                    }
                                }
                                
                                //
                                // Calculate U*D*U', store result in T2
                                //
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        v = 0.0;
                                        for(i_=0; i_<=n-1;i_++)
                                        {
                                            v += u[i,i_]*d[i_,j];
                                        }
                                        t[i,j] = v;
                                    }
                                }
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        v = 0.0;
                                        for(i_=0; i_<=n-1;i_++)
                                        {
                                            v += t[i,i_]*u[j,i_];
                                        }
                                        t2[i,j] = v;
                                    }
                                }
                            }
                            else
                            {
                                
                                //
                                // Unpack D
                                //
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        d[i,j] = 0;
                                    }
                                }
                                k = 0;
                                while( k<n )
                                {
                                    if( p[k]>=0 )
                                    {
                                        d[k,k] = a2[k,k];
                                        k = k+1;
                                    }
                                    else
                                    {
                                        d[k,k] = a2[k,k];
                                        d[k,k+1] = a2[k+1,k];
                                        d[k+1,k] = a2[k+1,k];
                                        d[k+1,k+1] = a2[k+1,k+1];
                                        k = k+2;
                                    }
                                }
                                
                                //
                                // Unpack L
                                //
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        l[i,j] = 0;
                                    }
                                    l[i,i] = 1;
                                }
                                k = 0;
                                while( k<n )
                                {
                                    
                                    //
                                    // permutations
                                    //
                                    if( p[k]>=0 )
                                    {
                                        for(i=0; i<=n-1; i++)
                                        {
                                            v = l[i,k];
                                            l[i,k] = l[i,p[k]];
                                            l[i,p[k]] = v;
                                        }
                                    }
                                    else
                                    {
                                        for(i=0; i<=n-1; i++)
                                        {
                                            v = l[i,k+1];
                                            l[i,k+1] = l[i,n+p[k+1]];
                                            l[i,n+p[k+1]] = v;
                                        }
                                    }
                                    
                                    //
                                    // unpack Lk
                                    //
                                    for(i=0; i<=n-1; i++)
                                    {
                                        for(j=0; j<=n-1; j++)
                                        {
                                            t[i,j] = 0;
                                        }
                                        t[i,i] = 1;
                                    }
                                    if( p[k]>=0 )
                                    {
                                        for(i=k+1; i<=n-1; i++)
                                        {
                                            t[i,k] = a2[i,k];
                                        }
                                    }
                                    else
                                    {
                                        for(i=k+2; i<=n-1; i++)
                                        {
                                            t[i,k] = a2[i,k];
                                            t[i,k+1] = a2[i,k+1];
                                        }
                                    }
                                    
                                    //
                                    // multiply L
                                    //
                                    for(i=0; i<=n-1; i++)
                                    {
                                        for(j=0; j<=n-1; j++)
                                        {
                                            v = 0.0;
                                            for(i_=0; i_<=n-1;i_++)
                                            {
                                                v += l[i,i_]*t[i_,j];
                                            }
                                            t2[i,j] = v;
                                        }
                                    }
                                    for(i=0; i<=n-1; i++)
                                    {
                                        for(j=0; j<=n-1; j++)
                                        {
                                            l[i,j] = t2[i,j];
                                        }
                                    }
                                    
                                    //
                                    // Next K
                                    //
                                    if( p[k]>=0 )
                                    {
                                        k = k+1;
                                    }
                                    else
                                    {
                                        k = k+2;
                                    }
                                }
                                
                                //
                                // Calculate L*D*L', store result in T2
                                //
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        v = 0.0;
                                        for(i_=0; i_<=n-1;i_++)
                                        {
                                            v += l[i,i_]*d[i_,j];
                                        }
                                        t[i,j] = v;
                                    }
                                }
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=n-1; j++)
                                    {
                                        v = 0.0;
                                        for(i_=0; i_<=n-1;i_++)
                                        {
                                            v += t[i,i_]*l[j,i_];
                                        }
                                        t2[i,j] = v;
                                    }
                                }
                            }
                            
                            //
                            // Test
                            //
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    err = Math.Max(err, Math.Abs(a[i,j]-t2[i,j]));
                                }
                            }
                        }
                    }
                }
            }
            
            //
            // report
            //
            waserrors = (double)(err)>(double)(threshold);
            if( !silent )
            {
                System.Console.Write("TESTING LDLT DECOMPOSITION");
                System.Console.WriteLine();
                System.Console.Write("ERROR:                                   ");
                System.Console.Write("{0,5:E3}",err);
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


        private static void generatematrix(ref double[,] a,
            int n,
            int task)
        {
            int i = 0;
            int j = 0;

            if( task==0 )
            {
                
                //
                // Zero matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a[i,j] = 0;
                    }
                }
            }
            if( task==1 )
            {
                
                //
                // Sparse matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    for(j=i+1; j<=n-1; j++)
                    {
                        if( (double)(AP.Math.RandomReal())>(double)(0.95) )
                        {
                            a[i,j] = 2*AP.Math.RandomReal()-1;
                        }
                        else
                        {
                            a[i,j] = 0;
                        }
                        a[j,i] = a[i,j];
                    }
                    if( (double)(AP.Math.RandomReal())>(double)(0.95) )
                    {
                        a[i,i] = (2*AP.Math.RandomInteger(2)-1)*(0.8+AP.Math.RandomReal());
                    }
                    else
                    {
                        a[i,i] = 0;
                    }
                }
            }
            if( task==2 )
            {
                
                //
                // Dense matrix
                //
                for(i=0; i<=n-1; i++)
                {
                    for(j=i+1; j<=n-1; j++)
                    {
                        a[i,j] = 2*AP.Math.RandomReal()-1;
                        a[j,i] = a[i,j];
                    }
                    a[i,i] = (2*AP.Math.RandomInteger(2)-1)*(0.8+AP.Math.RandomReal());
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testldltunit_test_silent()
        {
            bool result = new bool();

            result = testldlt(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testldltunit_test()
        {
            bool result = new bool();

            result = testldlt(false);
            return result;
        }
    }
}
