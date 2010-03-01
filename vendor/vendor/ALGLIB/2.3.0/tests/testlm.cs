
using System;

namespace alglib
{
    public class testlm
    {
        public static bool testminlm(bool silent)
        {
            bool result = new bool();
            bool waserrors = new bool();
            bool referror = new bool();
            bool lin1error = new bool();
            bool lin2error = new bool();
            bool eqerror = new bool();
            bool converror = new bool();
            bool scerror = new bool();
            int rkind = 0;
            int ckind = 0;
            double epsf = 0;
            double epsx = 0;
            int maxits = 0;
            int n = 0;
            int m = 0;
            double[] x = new double[0];
            double[] xe = new double[0];
            double[] b = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            double[,] a = new double[0,0];
            minlm.lmstate state = new minlm.lmstate();
            minlm.lmreport rep = new minlm.lmreport();
            int i_ = 0;

            waserrors = false;
            referror = false;
            lin1error = false;
            lin2error = false;
            eqerror = false;
            converror = false;
            scerror = false;
            
            //
            // Reference problem.
            // RKind is a algorithm selector:
            // * 0 = FJ
            // * 1 = FGJ
            // * 2 = FGH
            //
            x = new double[2+1];
            n = 3;
            m = 3;
            for(rkind=0; rkind<=2; rkind++)
            {
                x[0] = 100*AP.Math.RandomReal()-50;
                x[1] = 100*AP.Math.RandomReal()-50;
                x[2] = 100*AP.Math.RandomReal()-50;
                if( rkind==0 )
                {
                    minlm.minlmfj(n, m, ref x, 0.0, 0.0, 0, ref state);
                }
                if( rkind==1 )
                {
                    minlm.minlmfgj(n, m, ref x, 0.0, 0.0, 0, ref state);
                }
                if( rkind==2 )
                {
                    minlm.minlmfgh(n, ref x, 0.0, 0.0, 0, ref state);
                }
                while( minlm.minlmiteration(ref state) )
                {
                    
                    //
                    // (x-2)^2 + y^2 + (z-x)^2
                    //
                    state.f = AP.Math.Sqr(state.x[0]-2)+AP.Math.Sqr(state.x[1])+AP.Math.Sqr(state.x[2]-state.x[0]);
                    if( state.needfg | state.needfgh )
                    {
                        state.g[0] = 2*(state.x[0]-2)+2*(state.x[0]-state.x[2]);
                        state.g[1] = 2*state.x[1];
                        state.g[2] = 2*(state.x[2]-state.x[0]);
                    }
                    if( state.needfij )
                    {
                        state.fi[0] = state.x[0]-2;
                        state.fi[1] = state.x[1];
                        state.fi[2] = state.x[2]-state.x[0];
                        state.j[0,0] = 1;
                        state.j[0,1] = 0;
                        state.j[0,2] = 0;
                        state.j[1,0] = 0;
                        state.j[1,1] = 1;
                        state.j[1,2] = 0;
                        state.j[2,0] = -1;
                        state.j[2,1] = 0;
                        state.j[2,2] = 1;
                    }
                    if( state.needfgh )
                    {
                        state.h[0,0] = 4;
                        state.h[0,1] = 0;
                        state.h[0,2] = -2;
                        state.h[1,0] = 0;
                        state.h[1,1] = 2;
                        state.h[1,2] = 0;
                        state.h[2,0] = -2;
                        state.h[2,1] = 0;
                        state.h[2,2] = 2;
                    }
                    scerror = scerror | !rkindvsstatecheck(rkind, ref state);
                }
                minlm.minlmresults(ref state, ref x, ref rep);
                referror = referror | rep.terminationtype<=0 | (double)(Math.Abs(x[0]-2))>(double)(0.001) | (double)(Math.Abs(x[1]))>(double)(0.001) | (double)(Math.Abs(x[2]-2))>(double)(0.001);
            }
            
            //
            // 1D problem #1
            //
            for(rkind=0; rkind<=2; rkind++)
            {
                x = new double[1];
                n = 1;
                m = 1;
                x[0] = 100*AP.Math.RandomReal()-50;
                if( rkind==0 )
                {
                    minlm.minlmfj(n, m, ref x, 0.0, 0.0, 0, ref state);
                }
                if( rkind==1 )
                {
                    minlm.minlmfgj(n, m, ref x, 0.0, 0.0, 0, ref state);
                }
                if( rkind==2 )
                {
                    minlm.minlmfgh(n, ref x, 0.0, 0.0, 0, ref state);
                }
                while( minlm.minlmiteration(ref state) )
                {
                    state.f = AP.Math.Sqr(Math.Sin(state.x[0]));
                    if( state.needfg | state.needfgh )
                    {
                        state.g[0] = 2*Math.Sin(state.x[0])*Math.Cos(state.x[0]);
                    }
                    if( state.needfij )
                    {
                        state.fi[0] = Math.Sin(state.x[0]);
                        state.j[0,0] = Math.Cos(state.x[0]);
                    }
                    if( state.needfgh )
                    {
                        state.h[0,0] = 2*(Math.Cos(state.x[0])*Math.Cos(state.x[0])-Math.Sin(state.x[0])*Math.Sin(state.x[0]));
                    }
                    scerror = scerror | !rkindvsstatecheck(rkind, ref state);
                }
                minlm.minlmresults(ref state, ref x, ref rep);
                lin1error = rep.terminationtype<=0 | (double)(Math.Abs(x[0]/Math.PI-(int)Math.Round(x[0]/Math.PI)))>(double)(0.001);
            }
            
            //
            // Linear equations
            //
            for(n=1; n<=10; n++)
            {
                
                //
                // Prepare task
                //
                matgen.rmatrixrndcond(n, 100, ref a);
                x = new double[n];
                xe = new double[n];
                b = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    xe[i] = 2*AP.Math.RandomReal()-1;
                }
                for(i=0; i<=n-1; i++)
                {
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += a[i,i_]*xe[i_];
                    }
                    b[i] = v;
                }
                
                //
                // Test different RKind
                //
                for(rkind=0; rkind<=2; rkind++)
                {
                    
                    //
                    // Solve task
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        x[i] = 2*AP.Math.RandomReal()-1;
                    }
                    if( rkind==0 )
                    {
                        minlm.minlmfj(n, n, ref x, 0.0, 0.0, 0, ref state);
                    }
                    if( rkind==1 )
                    {
                        minlm.minlmfgj(n, n, ref x, 0.0, 0.0, 0, ref state);
                    }
                    if( rkind==2 )
                    {
                        minlm.minlmfgh(n, ref x, 0.0, 0.0, 0, ref state);
                    }
                    while( minlm.minlmiteration(ref state) )
                    {
                        if( state.needf | state.needfg | state.needfgh )
                        {
                            state.f = 0;
                        }
                        if( state.needfg | state.needfgh )
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                state.g[i] = 0;
                            }
                        }
                        if( state.needfgh )
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    state.h[i,j] = 0;
                                }
                            }
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            v = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                v += a[i,i_]*state.x[i_];
                            }
                            if( state.needf | state.needfg | state.needfgh )
                            {
                                state.f = state.f+AP.Math.Sqr(v-b[i]);
                            }
                            if( state.needfg | state.needfgh )
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    state.g[j] = state.g[j]+2*(v-b[i])*a[i,j];
                                }
                            }
                            if( state.needfgh )
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    for(k=0; k<=n-1; k++)
                                    {
                                        state.h[j,k] = state.h[j,k]+2*a[i,j]*a[i,k];
                                    }
                                }
                            }
                            if( state.needfij )
                            {
                                state.fi[i] = v-b[i];
                                for(i_=0; i_<=n-1;i_++)
                                {
                                    state.j[i,i_] = a[i,i_];
                                }
                            }
                        }
                        scerror = scerror | !rkindvsstatecheck(rkind, ref state);
                    }
                    minlm.minlmresults(ref state, ref x, ref rep);
                    eqerror = eqerror | rep.terminationtype<=0;
                    for(i=0; i<=n-1; i++)
                    {
                        eqerror = eqerror | (double)(Math.Abs(x[i]-xe[i]))>(double)(0.001);
                    }
                }
            }
            
            //
            // Testing convergence properties using
            // different optimizer types and different conditions
            //
            for(rkind=0; rkind<=2; rkind++)
            {
                for(ckind=0; ckind<=2; ckind++)
                {
                    epsf = 0;
                    epsx = 0;
                    maxits = 0;
                    if( ckind==0 )
                    {
                        epsf = 0.0001;
                    }
                    if( ckind==1 )
                    {
                        epsx = 0.0001;
                    }
                    if( ckind==2 )
                    {
                        maxits = 2;
                    }
                    x = new double[3];
                    n = 3;
                    m = 3;
                    for(i=0; i<=2; i++)
                    {
                        x[i] = 6;
                    }
                    if( rkind==0 )
                    {
                        minlm.minlmfj(n, m, ref x, epsf, epsx, maxits, ref state);
                    }
                    if( rkind==1 )
                    {
                        minlm.minlmfgj(n, m, ref x, epsf, epsx, maxits, ref state);
                    }
                    if( rkind==2 )
                    {
                        minlm.minlmfgh(n, ref x, epsf, epsx, maxits, ref state);
                    }
                    while( minlm.minlmiteration(ref state) )
                    {
                        if( state.needf | state.needfg | state.needfgh )
                        {
                            state.f = AP.Math.Sqr(Math.Exp(state.x[0])-2)+AP.Math.Sqr(state.x[1])+AP.Math.Sqr(state.x[2]-state.x[0]);
                        }
                        if( state.needfg | state.needfgh )
                        {
                            state.g[0] = 2*(Math.Exp(state.x[0])-2)*Math.Exp(state.x[0])+2*(state.x[0]-state.x[2]);
                            state.g[1] = 2*state.x[1];
                            state.g[2] = 2*(state.x[2]-state.x[0]);
                        }
                        if( state.needfgh )
                        {
                            state.h[0,0] = 4*AP.Math.Sqr(Math.Exp(state.x[0]))-4*Math.Exp(state.x[0])+2;
                            state.h[0,1] = 0;
                            state.h[0,2] = -2;
                            state.h[1,0] = 0;
                            state.h[1,1] = 2;
                            state.h[1,2] = 0;
                            state.h[2,0] = -2;
                            state.h[2,1] = 0;
                            state.h[2,2] = 2;
                        }
                        if( state.needfij )
                        {
                            state.fi[0] = Math.Exp(state.x[0])-2;
                            state.j[0,0] = Math.Exp(state.x[0]);
                            state.j[0,1] = 0;
                            state.j[0,2] = 0;
                            state.fi[1] = state.x[1];
                            state.j[1,0] = 0;
                            state.j[1,1] = 1;
                            state.j[1,2] = 0;
                            state.fi[2] = state.x[2]-state.x[0];
                            state.j[2,0] = -1;
                            state.j[2,1] = 0;
                            state.j[2,2] = 1;
                        }
                        scerror = scerror | !rkindvsstatecheck(rkind, ref state);
                    }
                    minlm.minlmresults(ref state, ref x, ref rep);
                    if( ckind==0 )
                    {
                        converror = converror | (double)(Math.Abs(x[0]-Math.Log(2)))>(double)(0.05);
                        converror = converror | (double)(Math.Abs(x[1]))>(double)(0.05);
                        converror = converror | (double)(Math.Abs(x[2]-Math.Log(2)))>(double)(0.05);
                        converror = converror | rep.terminationtype!=1;
                    }
                    if( ckind==1 )
                    {
                        converror = converror | (double)(Math.Abs(x[0]-Math.Log(2)))>(double)(0.05);
                        converror = converror | (double)(Math.Abs(x[1]))>(double)(0.05);
                        converror = converror | (double)(Math.Abs(x[2]-Math.Log(2)))>(double)(0.05);
                        converror = converror | rep.terminationtype!=2;
                    }
                    if( ckind==2 )
                    {
                        converror = converror | rep.terminationtype!=5 | rep.iterationscount!=maxits;
                    }
                }
            }
            
            //
            // end
            //
            waserrors = referror | lin1error | lin2error | eqerror | converror | scerror;
            if( !silent )
            {
                System.Console.Write("TESTING LEVENBERG-MARQUARDT OPTIMIZATION");
                System.Console.WriteLine();
                System.Console.Write("REFERENCE PROBLEM:                        ");
                if( referror )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("1-D PROBLEM #1:                           ");
                if( lin1error )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("1-D PROBLEM #2:                           ");
                if( lin2error )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("LINEAR EQUATIONS:                         ");
                if( eqerror )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("CONVERGENCE PROPERTIES:                   ");
                if( converror )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("STATE FIELDS CONSISTENCY:                 ");
                if( scerror )
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
        Asserts that State fields are consistent with RKind.
        Returns False otherwise.
        *************************************************************************/
        private static bool rkindvsstatecheck(int rkind,
            ref minlm.lmstate state)
        {
            bool result = new bool();
            int nset = 0;

            nset = 0;
            if( state.needf )
            {
                nset = nset+1;
            }
            if( state.needfg )
            {
                nset = nset+1;
            }
            if( state.needfij )
            {
                nset = nset+1;
            }
            if( state.needfgh )
            {
                nset = nset+1;
            }
            if( nset!=1 )
            {
                result = false;
                return result;
            }
            if( rkind==0 & (state.needfg | state.needfgh) )
            {
                result = false;
                return result;
            }
            if( rkind==1 & state.needfgh )
            {
                result = false;
                return result;
            }
            if( rkind==2 & state.needfij )
            {
                result = false;
                return result;
            }
            result = true;
            return result;
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testlm_test_silent()
        {
            bool result = new bool();

            result = testminlm(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testlm_test()
        {
            bool result = new bool();

            result = testminlm(false);
            return result;
        }
    }
}
