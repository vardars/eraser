
using System;

namespace alglib
{
    public class testmatgenunit
    {
        public const int maxsvditerations = 60;


        public static bool testmatgen(bool silent)
        {
            bool result = new bool();
            double[,] a = new double[0,0];
            double[,] b = new double[0,0];
            double[,] u = new double[0,0];
            double[,] v = new double[0,0];
            AP.Complex[,] ca = new AP.Complex[0,0];
            AP.Complex[,] cb = new AP.Complex[0,0];
            double[,] r1 = new double[0,0];
            double[,] r2 = new double[0,0];
            AP.Complex[,] c1 = new AP.Complex[0,0];
            AP.Complex[,] c2 = new AP.Complex[0,0];
            double[] w = new double[0];
            int n = 0;
            int maxn = 0;
            int i = 0;
            int j = 0;
            int pass = 0;
            int passcount = 0;
            bool waserrors = new bool();
            double cond = 0;
            double threshold = 0;
            double vt = 0;
            AP.Complex ct = 0;
            double minw = 0;
            double maxw = 0;
            bool serr = new bool();
            bool herr = new bool();
            bool spderr = new bool();
            bool hpderr = new bool();
            bool rerr = new bool();
            bool cerr = new bool();
            int i_ = 0;

            rerr = false;
            cerr = false;
            serr = false;
            herr = false;
            spderr = false;
            hpderr = false;
            waserrors = false;
            maxn = 20;
            passcount = 15;
            threshold = 1000*AP.Math.MachineEpsilon;
            
            //
            // Testing orthogonal
            //
            for(n=1; n<=maxn; n++)
            {
                for(pass=1; pass<=passcount; pass++)
                {
                    r1 = new double[n-1+1, 2*n-1+1];
                    r2 = new double[2*n-1+1, n-1+1];
                    c1 = new AP.Complex[n-1+1, 2*n-1+1];
                    c2 = new AP.Complex[2*n-1+1, n-1+1];
                    
                    //
                    // Random orthogonal, real
                    //
                    unset2d(ref a);
                    unset2d(ref b);
                    matgen.rmatrixrndorthogonal(n, ref a);
                    matgen.rmatrixrndorthogonal(n, ref b);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            
                            //
                            // orthogonality test
                            //
                            vt = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vt += a[i,i_]*a[j,i_];
                            }
                            if( i==j )
                            {
                                rerr = rerr | (double)(Math.Abs(vt-1))>(double)(threshold);
                            }
                            else
                            {
                                rerr = rerr | (double)(Math.Abs(vt))>(double)(threshold);
                            }
                            vt = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vt += b[i,i_]*b[j,i_];
                            }
                            if( i==j )
                            {
                                rerr = rerr | (double)(Math.Abs(vt-1))>(double)(threshold);
                            }
                            else
                            {
                                rerr = rerr | (double)(Math.Abs(vt))>(double)(threshold);
                            }
                            
                            //
                            // test for difference in A and B
                            //
                            if( n>=2 )
                            {
                                rerr = rerr | (double)(a[i,j])==(double)(b[i,j]);
                            }
                        }
                    }
                    
                    //
                    // Random orthogonal, complex
                    //
                    unset2dc(ref ca);
                    unset2dc(ref cb);
                    matgen.cmatrixrndorthogonal(n, ref ca);
                    matgen.cmatrixrndorthogonal(n, ref cb);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            
                            //
                            // orthogonality test
                            //
                            ct = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                ct += ca[i,i_]*AP.Math.Conj(ca[j,i_]);
                            }
                            if( i==j )
                            {
                                cerr = cerr | (double)(AP.Math.AbsComplex(ct-1))>(double)(threshold);
                            }
                            else
                            {
                                cerr = cerr | (double)(AP.Math.AbsComplex(ct))>(double)(threshold);
                            }
                            ct = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                ct += cb[i,i_]*AP.Math.Conj(cb[j,i_]);
                            }
                            if( i==j )
                            {
                                cerr = cerr | (double)(AP.Math.AbsComplex(ct-1))>(double)(threshold);
                            }
                            else
                            {
                                cerr = cerr | (double)(AP.Math.AbsComplex(ct))>(double)(threshold);
                            }
                            
                            //
                            // test for difference in A and B
                            //
                            if( n>=2 )
                            {
                                cerr = cerr | ca[i,j]==cb[i,j];
                            }
                        }
                    }
                    
                    //
                    // From the right real tests:
                    // 1. E*Q is orthogonal
                    // 2. Q1<>Q2 (routine result is changing)
                    // 3. (E E)'*Q = (Q' Q')' (correct handling of non-square matrices)
                    //
                    unset2d(ref a);
                    unset2d(ref b);
                    a = new double[n-1+1, n-1+1];
                    b = new double[n-1+1, n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 0;
                            b[i,j] = 0;
                        }
                        a[i,i] = 1;
                        b[i,i] = 1;
                    }
                    matgen.rmatrixrndorthogonalfromtheright(ref a, n, n);
                    matgen.rmatrixrndorthogonalfromtheright(ref b, n, n);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            
                            //
                            // orthogonality test
                            //
                            vt = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vt += a[i,i_]*a[j,i_];
                            }
                            if( i==j )
                            {
                                rerr = rerr | (double)(Math.Abs(vt-1))>(double)(threshold);
                            }
                            else
                            {
                                rerr = rerr | (double)(Math.Abs(vt))>(double)(threshold);
                            }
                            vt = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vt += b[i,i_]*b[j,i_];
                            }
                            if( i==j )
                            {
                                rerr = rerr | (double)(Math.Abs(vt-1))>(double)(threshold);
                            }
                            else
                            {
                                rerr = rerr | (double)(Math.Abs(vt))>(double)(threshold);
                            }
                            
                            //
                            // test for difference in A and B
                            //
                            if( n>=2 )
                            {
                                rerr = rerr | (double)(a[i,j])==(double)(b[i,j]);
                            }
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            r2[i,j] = 2*AP.Math.RandomReal()-1;
                            r2[i+n,j] = r2[i,j];
                        }
                    }
                    matgen.rmatrixrndorthogonalfromtheright(ref r2, 2*n, n);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            rerr = rerr | (double)(Math.Abs(r2[i+n,j]-r2[i,j]))>(double)(threshold);
                        }
                    }
                    
                    //
                    // From the left real tests:
                    // 1. Q*E is orthogonal
                    // 2. Q1<>Q2 (routine result is changing)
                    // 3. Q*(E E) = (Q Q) (correct handling of non-square matrices)
                    //
                    unset2d(ref a);
                    unset2d(ref b);
                    a = new double[n-1+1, n-1+1];
                    b = new double[n-1+1, n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a[i,j] = 0;
                            b[i,j] = 0;
                        }
                        a[i,i] = 1;
                        b[i,i] = 1;
                    }
                    matgen.rmatrixrndorthogonalfromtheleft(ref a, n, n);
                    matgen.rmatrixrndorthogonalfromtheleft(ref b, n, n);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            
                            //
                            // orthogonality test
                            //
                            vt = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vt += a[i,i_]*a[j,i_];
                            }
                            if( i==j )
                            {
                                rerr = rerr | (double)(Math.Abs(vt-1))>(double)(threshold);
                            }
                            else
                            {
                                rerr = rerr | (double)(Math.Abs(vt))>(double)(threshold);
                            }
                            vt = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                vt += b[i,i_]*b[j,i_];
                            }
                            if( i==j )
                            {
                                rerr = rerr | (double)(Math.Abs(vt-1))>(double)(threshold);
                            }
                            else
                            {
                                rerr = rerr | (double)(Math.Abs(vt))>(double)(threshold);
                            }
                            
                            //
                            // test for difference in A and B
                            //
                            if( n>=2 )
                            {
                                rerr = rerr | (double)(a[i,j])==(double)(b[i,j]);
                            }
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            r1[i,j] = 2*AP.Math.RandomReal()-1;
                            r1[i,j+n] = r1[i,j];
                        }
                    }
                    matgen.rmatrixrndorthogonalfromtheleft(ref r1, n, 2*n);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            rerr = rerr | (double)(Math.Abs(r1[i,j]-r1[i,j+n]))>(double)(threshold);
                        }
                    }
                    
                    //
                    // From the right complex tests:
                    // 1. E*Q is orthogonal
                    // 2. Q1<>Q2 (routine result is changing)
                    // 3. (E E)'*Q = (Q' Q')' (correct handling of non-square matrices)
                    //
                    unset2dc(ref ca);
                    unset2dc(ref cb);
                    ca = new AP.Complex[n-1+1, n-1+1];
                    cb = new AP.Complex[n-1+1, n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ca[i,j] = 0;
                            cb[i,j] = 0;
                        }
                        ca[i,i] = 1;
                        cb[i,i] = 1;
                    }
                    matgen.cmatrixrndorthogonalfromtheright(ref ca, n, n);
                    matgen.cmatrixrndorthogonalfromtheright(ref cb, n, n);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            
                            //
                            // orthogonality test
                            //
                            ct = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                ct += ca[i,i_]*AP.Math.Conj(ca[j,i_]);
                            }
                            if( i==j )
                            {
                                cerr = cerr | (double)(AP.Math.AbsComplex(ct-1))>(double)(threshold);
                            }
                            else
                            {
                                cerr = cerr | (double)(AP.Math.AbsComplex(ct))>(double)(threshold);
                            }
                            ct = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                ct += cb[i,i_]*AP.Math.Conj(cb[j,i_]);
                            }
                            if( i==j )
                            {
                                cerr = cerr | (double)(AP.Math.AbsComplex(ct-1))>(double)(threshold);
                            }
                            else
                            {
                                cerr = cerr | (double)(AP.Math.AbsComplex(ct))>(double)(threshold);
                            }
                            
                            //
                            // test for difference in A and B
                            //
                            cerr = cerr | ca[i,j]==cb[i,j];
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            c2[i,j] = 2*AP.Math.RandomReal()-1;
                            c2[i+n,j] = c2[i,j];
                        }
                    }
                    matgen.cmatrixrndorthogonalfromtheright(ref c2, 2*n, n);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            cerr = cerr | (double)(AP.Math.AbsComplex(c2[i+n,j]-c2[i,j]))>(double)(threshold);
                        }
                    }
                    
                    //
                    // From the left complex tests:
                    // 1. Q*E is orthogonal
                    // 2. Q1<>Q2 (routine result is changing)
                    // 3. Q*(E E) = (Q Q) (correct handling of non-square matrices)
                    //
                    unset2dc(ref ca);
                    unset2dc(ref cb);
                    ca = new AP.Complex[n-1+1, n-1+1];
                    cb = new AP.Complex[n-1+1, n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ca[i,j] = 0;
                            cb[i,j] = 0;
                        }
                        ca[i,i] = 1;
                        cb[i,i] = 1;
                    }
                    matgen.cmatrixrndorthogonalfromtheleft(ref ca, n, n);
                    matgen.cmatrixrndorthogonalfromtheleft(ref cb, n, n);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            
                            //
                            // orthogonality test
                            //
                            ct = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                ct += ca[i,i_]*AP.Math.Conj(ca[j,i_]);
                            }
                            if( i==j )
                            {
                                cerr = cerr | (double)(AP.Math.AbsComplex(ct-1))>(double)(threshold);
                            }
                            else
                            {
                                cerr = cerr | (double)(AP.Math.AbsComplex(ct))>(double)(threshold);
                            }
                            ct = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                ct += cb[i,i_]*AP.Math.Conj(cb[j,i_]);
                            }
                            if( i==j )
                            {
                                cerr = cerr | (double)(AP.Math.AbsComplex(ct-1))>(double)(threshold);
                            }
                            else
                            {
                                cerr = cerr | (double)(AP.Math.AbsComplex(ct))>(double)(threshold);
                            }
                            
                            //
                            // test for difference in A and B
                            //
                            cerr = cerr | ca[i,j]==cb[i,j];
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            c1[i,j] = 2*AP.Math.RandomReal()-1;
                            c1[i,j+n] = c1[i,j];
                        }
                    }
                    matgen.cmatrixrndorthogonalfromtheleft(ref c1, n, 2*n);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            cerr = cerr | (double)(AP.Math.AbsComplex(c1[i,j]-c1[i,j+n]))>(double)(threshold);
                        }
                    }
                }
            }
            
            //
            // Testing GCond
            //
            for(n=2; n<=maxn; n++)
            {
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // real test
                    //
                    unset2d(ref a);
                    cond = Math.Exp(Math.Log(1000)*AP.Math.RandomReal());
                    matgen.rmatrixrndcond(n, cond, ref a);
                    b = new double[n+1, n+1];
                    for(i=1; i<=n; i++)
                    {
                        for(j=1; j<=n; j++)
                        {
                            b[i,j] = a[i-1,j-1];
                        }
                    }
                    if( obsoletesvddecomposition(ref b, n, n, ref w, ref v) )
                    {
                        maxw = w[1];
                        minw = w[1];
                        for(i=2; i<=n; i++)
                        {
                            if( (double)(w[i])>(double)(maxw) )
                            {
                                maxw = w[i];
                            }
                            if( (double)(w[i])<(double)(minw) )
                            {
                                minw = w[i];
                            }
                        }
                        vt = maxw/minw/cond;
                        if( (double)(Math.Abs(Math.Log(vt)))>(double)(Math.Log(1+threshold)) )
                        {
                            rerr = true;
                        }
                    }
                }
            }
            
            //
            // Symmetric/SPD
            // N = 2 .. 30
            //
            for(n=2; n<=maxn; n++)
            {
                
                //
                // SPD matrices
                //
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // Generate A
                    //
                    unset2d(ref a);
                    cond = Math.Exp(Math.Log(1000)*AP.Math.RandomReal());
                    matgen.spdmatrixrndcond(n, cond, ref a);
                    
                    //
                    // test condition number
                    //
                    spderr = spderr | (double)(svdcond(ref a, n)/cond-1)>(double)(threshold);
                    
                    //
                    // test SPD
                    //
                    spderr = spderr | !isspd(a, n, true);
                    
                    //
                    // test that A is symmetic
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            spderr = spderr | (double)(Math.Abs(a[i,j]-a[j,i]))>(double)(threshold);
                        }
                    }
                    
                    //
                    // test for difference between A and B (subsequent matrix)
                    //
                    unset2d(ref b);
                    matgen.spdmatrixrndcond(n, cond, ref b);
                    if( n>=2 )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                spderr = spderr | (double)(a[i,j])==(double)(b[i,j]);
                            }
                        }
                    }
                }
                
                //
                // HPD matrices
                //
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // Generate A
                    //
                    unset2dc(ref ca);
                    cond = Math.Exp(Math.Log(1000)*AP.Math.RandomReal());
                    matgen.hpdmatrixrndcond(n, cond, ref ca);
                    
                    //
                    // test HPD
                    //
                    hpderr = hpderr | !ishpd(ca, n);
                    
                    //
                    // test that A is Hermitian
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            hpderr = hpderr | (double)(AP.Math.AbsComplex(ca[i,j]-AP.Math.Conj(ca[j,i])))>(double)(threshold);
                        }
                    }
                    
                    //
                    // test for difference between A and B (subsequent matrix)
                    //
                    unset2dc(ref cb);
                    matgen.hpdmatrixrndcond(n, cond, ref cb);
                    if( n>=2 )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                hpderr = hpderr | ca[i,j]==cb[i,j];
                            }
                        }
                    }
                }
                
                //
                // Symmetric matrices
                //
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // test condition number
                    //
                    unset2d(ref a);
                    cond = Math.Exp(Math.Log(1000)*AP.Math.RandomReal());
                    matgen.smatrixrndcond(n, cond, ref a);
                    serr = serr | (double)(svdcond(ref a, n)/cond-1)>(double)(threshold);
                    
                    //
                    // test for difference between A and B
                    //
                    unset2d(ref b);
                    matgen.smatrixrndcond(n, cond, ref b);
                    if( n>=2 )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                serr = serr | (double)(a[i,j])==(double)(b[i,j]);
                            }
                        }
                    }
                }
                
                //
                // Hermitian matrices
                //
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // Generate A
                    //
                    unset2dc(ref ca);
                    cond = Math.Exp(Math.Log(1000)*AP.Math.RandomReal());
                    matgen.hmatrixrndcond(n, cond, ref ca);
                    
                    //
                    // test that A is Hermitian
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            herr = herr | (double)(AP.Math.AbsComplex(ca[i,j]-AP.Math.Conj(ca[j,i])))>(double)(threshold);
                        }
                    }
                    
                    //
                    // test for difference between A and B (subsequent matrix)
                    //
                    unset2dc(ref cb);
                    matgen.hmatrixrndcond(n, cond, ref cb);
                    if( n>=2 )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                herr = herr | ca[i,j]==cb[i,j];
                            }
                        }
                    }
                }
            }
            
            //
            // report
            //
            waserrors = rerr | cerr | serr | spderr | herr | hpderr;
            if( !silent )
            {
                System.Console.Write("TESTING MATRIX GENERATOR");
                System.Console.WriteLine();
                System.Console.Write("REAL TEST:                               ");
                if( !rerr )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("COMPLEX TEST:                            ");
                if( !cerr )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("SYMMETRIC TEST:                          ");
                if( !serr )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("HERMITIAN TEST:                          ");
                if( !herr )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("SPD TEST:                                ");
                if( !spderr )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("HPD TEST:                                ");
                if( !hpderr )
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
        Test whether matrix is SPD
        *************************************************************************/
        public static bool isspd(double[,] a,
            int n,
            bool isupper)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            double ajj = 0;
            double v = 0;
            int i_ = 0;

            a = (double[,])a.Clone();

            
            //
            //     Test the input parameters.
            //
            System.Diagnostics.Debug.Assert(n>=0, "Error in SMatrixCholesky: incorrect function arguments");
            
            //
            //     Quick return if possible
            //
            result = true;
            if( n<=0 )
            {
                return result;
            }
            if( isupper )
            {
                
                //
                // Compute the Cholesky factorization A = U'*U.
                //
                for(j=0; j<=n-1; j++)
                {
                    
                    //
                    // Compute U(J,J) and test for non-positive-definiteness.
                    //
                    v = 0.0;
                    for(i_=0; i_<=j-1;i_++)
                    {
                        v += a[i_,j]*a[i_,j];
                    }
                    ajj = a[j,j]-v;
                    if( (double)(ajj)<=(double)(0) )
                    {
                        result = false;
                        return result;
                    }
                    ajj = Math.Sqrt(ajj);
                    a[j,j] = ajj;
                    
                    //
                    // Compute elements J+1:N of row J.
                    //
                    if( j<n-1 )
                    {
                        for(i=j+1; i<=n-1; i++)
                        {
                            v = 0.0;
                            for(i_=0; i_<=j-1;i_++)
                            {
                                v += a[i_,i]*a[i_,j];
                            }
                            a[j,i] = a[j,i]-v;
                        }
                        v = 1/ajj;
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            a[j,i_] = v*a[j,i_];
                        }
                    }
                }
            }
            else
            {
                
                //
                // Compute the Cholesky factorization A = L*L'.
                //
                for(j=0; j<=n-1; j++)
                {
                    
                    //
                    // Compute L(J,J) and test for non-positive-definiteness.
                    //
                    v = 0.0;
                    for(i_=0; i_<=j-1;i_++)
                    {
                        v += a[j,i_]*a[j,i_];
                    }
                    ajj = a[j,j]-v;
                    if( (double)(ajj)<=(double)(0) )
                    {
                        result = false;
                        return result;
                    }
                    ajj = Math.Sqrt(ajj);
                    a[j,j] = ajj;
                    
                    //
                    // Compute elements J+1:N of column J.
                    //
                    if( j<n-1 )
                    {
                        for(i=j+1; i<=n-1; i++)
                        {
                            v = 0.0;
                            for(i_=0; i_<=j-1;i_++)
                            {
                                v += a[i,i_]*a[j,i_];
                            }
                            a[i,j] = a[i,j]-v;
                        }
                        v = 1/ajj;
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            a[i_,j] = v*a[i_,j];
                        }
                    }
                }
            }
            return result;
        }


        public static bool obsoletesvddecomposition(ref double[,] a,
            int m,
            int n,
            ref double[] w,
            ref double[,] v)
        {
            bool result = new bool();
            int nm = 0;
            int minmn = 0;
            int l = 0;
            int k = 0;
            int j = 0;
            int jj = 0;
            int its = 0;
            int i = 0;
            double z = 0;
            double y = 0;
            double x = 0;
            double vscale = 0;
            double s = 0;
            double h = 0;
            double g = 0;
            double f = 0;
            double c = 0;
            double anorm = 0;
            double[] rv1 = new double[0];
            bool flag = new bool();

            rv1 = new double[n+1];
            w = new double[n+1];
            v = new double[n+1, n+1];
            result = true;
            if( m<n )
            {
                minmn = m;
            }
            else
            {
                minmn = n;
            }
            g = 0.0;
            vscale = 0.0;
            anorm = 0.0;
            for(i=1; i<=n; i++)
            {
                l = i+1;
                rv1[i] = vscale*g;
                g = 0;
                s = 0;
                vscale = 0;
                if( i<=m )
                {
                    for(k=i; k<=m; k++)
                    {
                        vscale = vscale+Math.Abs(a[k,i]);
                    }
                    if( (double)(vscale)!=(double)(0.0) )
                    {
                        for(k=i; k<=m; k++)
                        {
                            a[k,i] = a[k,i]/vscale;
                            s = s+a[k,i]*a[k,i];
                        }
                        f = a[i,i];
                        g = -extsign(Math.Sqrt(s), f);
                        h = f*g-s;
                        a[i,i] = f-g;
                        if( i!=n )
                        {
                            for(j=l; j<=n; j++)
                            {
                                s = 0.0;
                                for(k=i; k<=m; k++)
                                {
                                    s = s+a[k,i]*a[k,j];
                                }
                                f = s/h;
                                for(k=i; k<=m; k++)
                                {
                                    a[k,j] = a[k,j]+f*a[k,i];
                                }
                            }
                        }
                        for(k=i; k<=m; k++)
                        {
                            a[k,i] = vscale*a[k,i];
                        }
                    }
                }
                w[i] = vscale*g;
                g = 0.0;
                s = 0.0;
                vscale = 0.0;
                if( i<=m & i!=n )
                {
                    for(k=l; k<=n; k++)
                    {
                        vscale = vscale+Math.Abs(a[i,k]);
                    }
                    if( (double)(vscale)!=(double)(0.0) )
                    {
                        for(k=l; k<=n; k++)
                        {
                            a[i,k] = a[i,k]/vscale;
                            s = s+a[i,k]*a[i,k];
                        }
                        f = a[i,l];
                        g = -extsign(Math.Sqrt(s), f);
                        h = f*g-s;
                        a[i,l] = f-g;
                        for(k=l; k<=n; k++)
                        {
                            rv1[k] = a[i,k]/h;
                        }
                        if( i!=m )
                        {
                            for(j=l; j<=m; j++)
                            {
                                s = 0.0;
                                for(k=l; k<=n; k++)
                                {
                                    s = s+a[j,k]*a[i,k];
                                }
                                for(k=l; k<=n; k++)
                                {
                                    a[j,k] = a[j,k]+s*rv1[k];
                                }
                            }
                        }
                        for(k=l; k<=n; k++)
                        {
                            a[i,k] = vscale*a[i,k];
                        }
                    }
                }
                anorm = mymax(anorm, Math.Abs(w[i])+Math.Abs(rv1[i]));
            }
            for(i=n; i>=1; i--)
            {
                if( i<n )
                {
                    if( (double)(g)!=(double)(0.0) )
                    {
                        for(j=l; j<=n; j++)
                        {
                            v[j,i] = a[i,j]/a[i,l]/g;
                        }
                        for(j=l; j<=n; j++)
                        {
                            s = 0.0;
                            for(k=l; k<=n; k++)
                            {
                                s = s+a[i,k]*v[k,j];
                            }
                            for(k=l; k<=n; k++)
                            {
                                v[k,j] = v[k,j]+s*v[k,i];
                            }
                        }
                    }
                    for(j=l; j<=n; j++)
                    {
                        v[i,j] = 0.0;
                        v[j,i] = 0.0;
                    }
                }
                v[i,i] = 1.0;
                g = rv1[i];
                l = i;
            }
            for(i=minmn; i>=1; i--)
            {
                l = i+1;
                g = w[i];
                if( i<n )
                {
                    for(j=l; j<=n; j++)
                    {
                        a[i,j] = 0.0;
                    }
                }
                if( (double)(g)!=(double)(0.0) )
                {
                    g = 1.0/g;
                    if( i!=n )
                    {
                        for(j=l; j<=n; j++)
                        {
                            s = 0.0;
                            for(k=l; k<=m; k++)
                            {
                                s = s+a[k,i]*a[k,j];
                            }
                            f = s/a[i,i]*g;
                            for(k=i; k<=m; k++)
                            {
                                a[k,j] = a[k,j]+f*a[k,i];
                            }
                        }
                    }
                    for(j=i; j<=m; j++)
                    {
                        a[j,i] = a[j,i]*g;
                    }
                }
                else
                {
                    for(j=i; j<=m; j++)
                    {
                        a[j,i] = 0.0;
                    }
                }
                a[i,i] = a[i,i]+1.0;
            }
            for(k=n; k>=1; k--)
            {
                for(its=1; its<=maxsvditerations; its++)
                {
                    flag = true;
                    for(l=k; l>=1; l--)
                    {
                        nm = l-1;
                        if( (double)(Math.Abs(rv1[l])+anorm)==(double)(anorm) )
                        {
                            flag = false;
                            break;
                        }
                        if( (double)(Math.Abs(w[nm])+anorm)==(double)(anorm) )
                        {
                            break;
                        }
                    }
                    if( flag )
                    {
                        c = 0.0;
                        s = 1.0;
                        for(i=l; i<=k; i++)
                        {
                            f = s*rv1[i];
                            if( (double)(Math.Abs(f)+anorm)!=(double)(anorm) )
                            {
                                g = w[i];
                                h = pythag(f, g);
                                w[i] = h;
                                h = 1.0/h;
                                c = g*h;
                                s = -(f*h);
                                for(j=1; j<=m; j++)
                                {
                                    y = a[j,nm];
                                    z = a[j,i];
                                    a[j,nm] = y*c+z*s;
                                    a[j,i] = -(y*s)+z*c;
                                }
                            }
                        }
                    }
                    z = w[k];
                    if( l==k )
                    {
                        if( (double)(z)<(double)(0.0) )
                        {
                            w[k] = -z;
                            for(j=1; j<=n; j++)
                            {
                                v[j,k] = -v[j,k];
                            }
                        }
                        break;
                    }
                    if( its==maxsvditerations )
                    {
                        result = false;
                        return result;
                    }
                    x = w[l];
                    nm = k-1;
                    y = w[nm];
                    g = rv1[nm];
                    h = rv1[k];
                    f = ((y-z)*(y+z)+(g-h)*(g+h))/(2.0*h*y);
                    g = pythag(f, 1);
                    f = ((x-z)*(x+z)+h*(y/(f+extsign(g, f))-h))/x;
                    c = 1.0;
                    s = 1.0;
                    for(j=l; j<=nm; j++)
                    {
                        i = j+1;
                        g = rv1[i];
                        y = w[i];
                        h = s*g;
                        g = c*g;
                        z = pythag(f, h);
                        rv1[j] = z;
                        c = f/z;
                        s = h/z;
                        f = x*c+g*s;
                        g = -(x*s)+g*c;
                        h = y*s;
                        y = y*c;
                        for(jj=1; jj<=n; jj++)
                        {
                            x = v[jj,j];
                            z = v[jj,i];
                            v[jj,j] = x*c+z*s;
                            v[jj,i] = -(x*s)+z*c;
                        }
                        z = pythag(f, h);
                        w[j] = z;
                        if( (double)(z)!=(double)(0.0) )
                        {
                            z = 1.0/z;
                            c = f*z;
                            s = h*z;
                        }
                        f = c*g+s*y;
                        x = -(s*g)+c*y;
                        for(jj=1; jj<=m; jj++)
                        {
                            y = a[jj,j];
                            z = a[jj,i];
                            a[jj,j] = y*c+z*s;
                            a[jj,i] = -(y*s)+z*c;
                        }
                    }
                    rv1[l] = 0.0;
                    rv1[k] = f;
                    w[k] = x;
                }
            }
            return result;
        }


        /*************************************************************************
        Unsets 2D array.
        *************************************************************************/
        private static void unset2d(ref double[,] a)
        {
            a = new double[0+1, 0+1];
            a[0,0] = 2*AP.Math.RandomReal()-1;
        }


        /*************************************************************************
        Unsets 2D array.
        *************************************************************************/
        private static void unset2dc(ref AP.Complex[,] a)
        {
            a = new AP.Complex[0+1, 0+1];
            a[0,0] = 2*AP.Math.RandomReal()-1;
        }


        /*************************************************************************
        Tests whether A is HPD
        *************************************************************************/
        private static bool ishpd(AP.Complex[,] a,
            int n)
        {
            bool result = new bool();
            int j = 0;
            double ajj = 0;
            AP.Complex v = 0;
            double r = 0;
            AP.Complex[] t = new AP.Complex[0];
            AP.Complex[] t2 = new AP.Complex[0];
            AP.Complex[] t3 = new AP.Complex[0];
            int i = 0;
            AP.Complex[,] a1 = new AP.Complex[0,0];
            int i_ = 0;

            a = (AP.Complex[,])a.Clone();

            t = new AP.Complex[n-1+1];
            t2 = new AP.Complex[n-1+1];
            t3 = new AP.Complex[n-1+1];
            result = true;
            
            //
            // Compute the Cholesky factorization A = U'*U.
            //
            for(j=0; j<=n-1; j++)
            {
                
                //
                // Compute U(J,J) and test for non-positive-definiteness.
                //
                v = 0.0;
                for(i_=0; i_<=j-1;i_++)
                {
                    v += AP.Math.Conj(a[i_,j])*a[i_,j];
                }
                ajj = (a[j,j]-v).x;
                if( (double)(ajj)<=(double)(0) )
                {
                    a[j,j] = ajj;
                    result = false;
                    return result;
                }
                ajj = Math.Sqrt(ajj);
                a[j,j] = ajj;
                
                //
                // Compute elements J+1:N-1 of row J.
                //
                if( j<n-1 )
                {
                    for(i_=0; i_<=j-1;i_++)
                    {
                        t2[i_] = AP.Math.Conj(a[i_,j]);
                    }
                    for(i_=j+1; i_<=n-1;i_++)
                    {
                        t3[i_] = a[j,i_];
                    }
                    for(i=j+1; i<=n-1; i++)
                    {
                        v = 0.0;
                        for(i_=0; i_<=j-1;i_++)
                        {
                            v += a[i_,i]*t2[i_];
                        }
                        t3[i] = t3[i]-v;
                    }
                    for(i_=j+1; i_<=n-1;i_++)
                    {
                        a[j,i_] = t3[i_];
                    }
                    r = 1/ajj;
                    for(i_=j+1; i_<=n-1;i_++)
                    {
                        a[j,i_] = r*a[j,i_];
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        SVD condition number
        *************************************************************************/
        private static double svdcond(ref double[,] a,
            int n)
        {
            double result = 0;
            double[,] a1 = new double[0,0];
            double[,] v = new double[0,0];
            double[] w = new double[0];
            int i = 0;
            int j = 0;
            double minw = 0;
            double maxw = 0;

            a1 = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    a1[i,j] = a[i-1,j-1];
                }
            }
            if( !obsoletesvddecomposition(ref a1, n, n, ref w, ref v) )
            {
                result = 0;
                return result;
            }
            minw = w[1];
            maxw = w[1];
            for(i=2; i<=n; i++)
            {
                if( (double)(w[i])<(double)(minw) )
                {
                    minw = w[i];
                }
                if( (double)(w[i])>(double)(maxw) )
                {
                    maxw = w[i];
                }
            }
            result = maxw/minw;
            return result;
        }


        private static double extsign(double a,
            double b)
        {
            double result = 0;

            if( (double)(b)>=(double)(0) )
            {
                result = Math.Abs(a);
            }
            else
            {
                result = -Math.Abs(a);
            }
            return result;
        }


        private static double mymax(double a,
            double b)
        {
            double result = 0;

            if( (double)(a)>(double)(b) )
            {
                result = a;
            }
            else
            {
                result = b;
            }
            return result;
        }


        private static double pythag(double a,
            double b)
        {
            double result = 0;

            if( (double)(Math.Abs(a))<(double)(Math.Abs(b)) )
            {
                result = Math.Abs(b)*Math.Sqrt(1+AP.Math.Sqr(a/b));
            }
            else
            {
                result = Math.Abs(a)*Math.Sqrt(1+AP.Math.Sqr(b/a));
            }
            return result;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testmatgenunit_test_silent()
        {
            bool result = new bool();

            result = testmatgen(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testmatgenunit_test()
        {
            bool result = new bool();

            result = testmatgen(false);
            return result;
        }
    }
}
