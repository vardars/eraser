
using System;

namespace alglib
{
    public class testsafesolveunit
    {
        /*************************************************************************
        Main unittest subroutine
        *************************************************************************/
        public static bool testsafesolve(bool silent)
        {
            bool result = new bool();
            int maxmn = 0;
            double threshold = 0;
            bool rerrors = new bool();
            bool cerrors = new bool();
            bool waserrors = new bool();
            bool isupper = new bool();
            int trans = 0;
            bool isunit = new bool();
            double scalea = 0;
            double growth = 0;
            int i = 0;
            int j = 0;
            int n = 0;
            int j1 = 0;
            int j2 = 0;
            AP.Complex cv = 0;
            AP.Complex[,] ca = new AP.Complex[0,0];
            AP.Complex[,] cea = new AP.Complex[0,0];
            AP.Complex[,] ctmpa = new AP.Complex[0,0];
            AP.Complex[] cxs = new AP.Complex[0];
            AP.Complex[] cxe = new AP.Complex[0];
            double rv = 0;
            double[,] ra = new double[0,0];
            double[,] rea = new double[0,0];
            double[,] rtmpa = new double[0,0];
            double[] rxs = new double[0];
            double[] rxe = new double[0];
            int i_ = 0;

            maxmn = 30;
            threshold = 100000*AP.Math.MachineEpsilon;
            rerrors = false;
            cerrors = false;
            waserrors = false;
            
            //
            // Different problems: general tests
            //
            for(n=1; n<=maxmn; n++)
            {
                
                //
                // test complex solver with well-conditioned matrix:
                // 1. generate A: fill off-diagonal elements with small values,
                //    diagonal elements are filled with larger values
                // 2. generate 'effective' A
                // 3. prepare task (exact X is stored in CXE, right part - in CXS),
                //    solve and compare CXS and CXE
                //
                isupper = (double)(AP.Math.RandomReal())>(double)(0.5);
                trans = AP.Math.RandomInteger(3);
                isunit = (double)(AP.Math.RandomReal())>(double)(0.5);
                scalea = AP.Math.RandomReal()+0.5;
                ca = new AP.Complex[n, n];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( i==j )
                        {
                            ca[i,j].x = (2*AP.Math.RandomInteger(2)-1)*(5+AP.Math.RandomReal());
                            ca[i,j].y = (2*AP.Math.RandomInteger(2)-1)*(5+AP.Math.RandomReal());
                        }
                        else
                        {
                            ca[i,j].x = 0.2*AP.Math.RandomReal()-0.1;
                            ca[i,j].y = 0.2*AP.Math.RandomReal()-0.1;
                        }
                    }
                }
                cmatrixmakeacopy(ref ca, n, n, ref ctmpa);
                for(i=0; i<=n-1; i++)
                {
                    if( isupper )
                    {
                        j1 = 0;
                        j2 = i-1;
                    }
                    else
                    {
                        j1 = i+1;
                        j2 = n-1;
                    }
                    for(j=j1; j<=j2; j++)
                    {
                        ctmpa[i,j] = 0;
                    }
                    if( isunit )
                    {
                        ctmpa[i,i] = 1;
                    }
                }
                cea = new AP.Complex[n, n];
                for(i=0; i<=n-1; i++)
                {
                    if( trans==0 )
                    {
                        for(i_=0; i_<=n-1;i_++)
                        {
                            cea[i,i_] = scalea*ctmpa[i,i_];
                        }
                    }
                    if( trans==1 )
                    {
                        for(i_=0; i_<=n-1;i_++)
                        {
                            cea[i_,i] = scalea*ctmpa[i,i_];
                        }
                    }
                    if( trans==2 )
                    {
                        for(i_=0; i_<=n-1;i_++)
                        {
                            cea[i_,i] = scalea*AP.Math.Conj(ctmpa[i,i_]);
                        }
                    }
                }
                cxe = new AP.Complex[n];
                for(i=0; i<=n-1; i++)
                {
                    cxe[i].x = 2*AP.Math.RandomReal()-1;
                    cxe[i].y = 2*AP.Math.RandomReal()-1;
                }
                cxs = new AP.Complex[n];
                for(i=0; i<=n-1; i++)
                {
                    cv = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        cv += cea[i,i_]*cxe[i_];
                    }
                    cxs[i] = cv;
                }
                if( safesolve.cmatrixscaledtrsafesolve(ref ca, scalea, n, ref cxs, isupper, trans, isunit, Math.Sqrt(AP.Math.MaxRealNumber)) )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        cerrors = cerrors | (double)(AP.Math.AbsComplex(cxs[i]-cxe[i]))>(double)(threshold);
                    }
                }
                else
                {
                    cerrors = true;
                }
                
                //
                // same with real
                //
                isupper = (double)(AP.Math.RandomReal())>(double)(0.5);
                trans = AP.Math.RandomInteger(2);
                isunit = (double)(AP.Math.RandomReal())>(double)(0.5);
                scalea = AP.Math.RandomReal()+0.5;
                ra = new double[n, n];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( i==j )
                        {
                            ra[i,j] = (2*AP.Math.RandomInteger(2)-1)*(5+AP.Math.RandomReal());
                        }
                        else
                        {
                            ra[i,j] = 0.2*AP.Math.RandomReal()-0.1;
                        }
                    }
                }
                rmatrixmakeacopy(ref ra, n, n, ref rtmpa);
                for(i=0; i<=n-1; i++)
                {
                    if( isupper )
                    {
                        j1 = 0;
                        j2 = i-1;
                    }
                    else
                    {
                        j1 = i+1;
                        j2 = n-1;
                    }
                    for(j=j1; j<=j2; j++)
                    {
                        rtmpa[i,j] = 0;
                    }
                    if( isunit )
                    {
                        rtmpa[i,i] = 1;
                    }
                }
                rea = new double[n, n];
                for(i=0; i<=n-1; i++)
                {
                    if( trans==0 )
                    {
                        for(i_=0; i_<=n-1;i_++)
                        {
                            rea[i,i_] = scalea*rtmpa[i,i_];
                        }
                    }
                    if( trans==1 )
                    {
                        for(i_=0; i_<=n-1;i_++)
                        {
                            rea[i_,i] = scalea*rtmpa[i,i_];
                        }
                    }
                }
                rxe = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    rxe[i] = 2*AP.Math.RandomReal()-1;
                }
                rxs = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    rv = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        rv += rea[i,i_]*rxe[i_];
                    }
                    rxs[i] = rv;
                }
                if( safesolve.rmatrixscaledtrsafesolve(ref ra, scalea, n, ref rxs, isupper, trans, isunit, Math.Sqrt(AP.Math.MaxRealNumber)) )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        rerrors = rerrors | (double)(Math.Abs(rxs[i]-rxe[i]))>(double)(threshold);
                    }
                }
                else
                {
                    rerrors = true;
                }
            }
            
            //
            // Special test with diagonal ill-conditioned matrix:
            // * ability to solve it when resulting growth is less than threshold
            // * ability to stop solve when resulting growth is greater than threshold
            //
            // A = diag(1, 1/growth)
            // b = (1, 0.5)
            //
            n = 2;
            growth = 10;
            ca = new AP.Complex[n, n];
            ca[0,0] = 1;
            ca[0,1] = 0;
            ca[1,0] = 0;
            ca[1,1] = 1/growth;
            cxs = new AP.Complex[n];
            cxs[0] = 1.0;
            cxs[1] = 0.5;
            cerrors = cerrors | !safesolve.cmatrixscaledtrsafesolve(ref ca, 1.0, n, ref cxs, (double)(AP.Math.RandomReal())>(double)(0.5), AP.Math.RandomInteger(3), false, 1.05*Math.Max(AP.Math.AbsComplex(cxs[1])*growth, 1.0));
            cerrors = cerrors | !safesolve.cmatrixscaledtrsafesolve(ref ca, 1.0, n, ref cxs, (double)(AP.Math.RandomReal())>(double)(0.5), AP.Math.RandomInteger(3), false, 0.95*Math.Max(AP.Math.AbsComplex(cxs[1])*growth, 1.0));
            ra = new double[n, n];
            ra[0,0] = 1;
            ra[0,1] = 0;
            ra[1,0] = 0;
            ra[1,1] = 1/growth;
            rxs = new double[n];
            rxs[0] = 1.0;
            rxs[1] = 0.5;
            rerrors = rerrors | !safesolve.rmatrixscaledtrsafesolve(ref ra, 1.0, n, ref rxs, (double)(AP.Math.RandomReal())>(double)(0.5), AP.Math.RandomInteger(2), false, 1.05*Math.Max(Math.Abs(rxs[1])*growth, 1.0));
            rerrors = rerrors | !safesolve.rmatrixscaledtrsafesolve(ref ra, 1.0, n, ref rxs, (double)(AP.Math.RandomReal())>(double)(0.5), AP.Math.RandomInteger(2), false, 0.95*Math.Max(Math.Abs(rxs[1])*growth, 1.0));
            
            //
            // Special test with diagonal degenerate matrix:
            // * ability to solve it when resulting growth is less than threshold
            // * ability to stop solve when resulting growth is greater than threshold
            //
            // A = diag(1, 0)
            // b = (1, 0.5)
            //
            n = 2;
            ca = new AP.Complex[n, n];
            ca[0,0] = 1;
            ca[0,1] = 0;
            ca[1,0] = 0;
            ca[1,1] = 0;
            cxs = new AP.Complex[n];
            cxs[0] = 1.0;
            cxs[1] = 0.5;
            cerrors = cerrors | safesolve.cmatrixscaledtrsafesolve(ref ca, 1.0, n, ref cxs, (double)(AP.Math.RandomReal())>(double)(0.5), AP.Math.RandomInteger(3), false, Math.Sqrt(AP.Math.MaxRealNumber));
            ra = new double[n, n];
            ra[0,0] = 1;
            ra[0,1] = 0;
            ra[1,0] = 0;
            ra[1,1] = 0;
            rxs = new double[n];
            rxs[0] = 1.0;
            rxs[1] = 0.5;
            rerrors = rerrors | safesolve.rmatrixscaledtrsafesolve(ref ra, 1.0, n, ref rxs, (double)(AP.Math.RandomReal())>(double)(0.5), AP.Math.RandomInteger(2), false, Math.Sqrt(AP.Math.MaxRealNumber));
            
            //
            // report
            //
            waserrors = rerrors | cerrors;
            if( !silent )
            {
                System.Console.Write("TESTING SAFE TR SOLVER");
                System.Console.WriteLine();
                System.Console.Write("REAL:                                    ");
                if( !rerrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("COMPLEX:                                 ");
                if( !cerrors )
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
        Copy
        *************************************************************************/
        private static void rmatrixmakeacopy(ref double[,] a,
            int m,
            int n,
            ref double[,] b)
        {
            int i = 0;
            int j = 0;

            b = new double[m-1+1, n-1+1];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    b[i,j] = a[i,j];
                }
            }
        }


        /*************************************************************************
        Copy
        *************************************************************************/
        private static void cmatrixmakeacopy(ref AP.Complex[,] a,
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
        Silent unit test
        *************************************************************************/
        public static bool testsafesolveunit_test_silent()
        {
            bool result = new bool();

            result = testsafesolve(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testsafesolveunit_test()
        {
            bool result = new bool();

            result = testsafesolve(false);
            return result;
        }
    }
}
