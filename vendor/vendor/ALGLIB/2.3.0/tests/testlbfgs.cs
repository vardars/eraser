
using System;

namespace alglib
{
    public class testlbfgs
    {
        public static bool testminlbfgs(bool silent)
        {
            bool result = new bool();
            bool waserrors = new bool();
            bool referror = new bool();
            bool lin1error = new bool();
            bool lin2error = new bool();
            bool eqerror = new bool();
            bool converror = new bool();
            int n = 0;
            int m = 0;
            double[] x = new double[0];
            double[] xe = new double[0];
            double[] b = new double[0];
            int i = 0;
            int j = 0;
            double v = 0;
            double[,] a = new double[0,0];
            lbfgs.lbfgsstate state = new lbfgs.lbfgsstate();
            lbfgs.lbfgsreport rep = new lbfgs.lbfgsreport();
            int i_ = 0;

            waserrors = false;
            
            //
            // Reference problem
            //
            x = new double[2+1];
            n = 3;
            m = 2;
            x[0] = 100*AP.Math.RandomReal()-50;
            x[1] = 100*AP.Math.RandomReal()-50;
            x[2] = 100*AP.Math.RandomReal()-50;
            lbfgs.minlbfgs(n, m, ref x, 0.0, 0.0, 0.0, 0, 0, ref state);
            while( lbfgs.minlbfgsiteration(ref state) )
            {
                state.f = AP.Math.Sqr(state.x[0]-2)+AP.Math.Sqr(state.x[1])+AP.Math.Sqr(state.x[2]-state.x[0]);
                state.g[0] = 2*(state.x[0]-2)+2*(state.x[0]-state.x[2]);
                state.g[1] = 2*state.x[1];
                state.g[2] = 2*(state.x[2]-state.x[0]);
            }
            lbfgs.minlbfgsresults(ref state, ref x, ref rep);
            referror = rep.terminationtype<=0 | (double)(Math.Abs(x[0]-2))>(double)(0.001) | (double)(Math.Abs(x[1]))>(double)(0.001) | (double)(Math.Abs(x[2]-2))>(double)(0.001);
            
            //
            // Linear equations
            //
            eqerror = false;
            for(n=1; n<=10; n++)
            {
                
                //
                // Prepare task
                //
                a = new double[n-1+1, n-1+1];
                x = new double[n-1+1];
                xe = new double[n-1+1];
                b = new double[n-1+1];
                for(i=0; i<=n-1; i++)
                {
                    xe[i] = 2*AP.Math.RandomReal()-1;
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a[i,j] = 2*AP.Math.RandomReal()-1;
                    }
                    a[i,i] = a[i,i]+3*Math.Sign(a[i,i]);
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
                // Test different M
                //
                for(m=1; m<=n; m++)
                {
                    
                    //
                    // Solve task
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        x[i] = 2*AP.Math.RandomReal()-1;
                    }
                    lbfgs.minlbfgs(n, m, ref x, 0.0, 0.0, 0.0, 0, 0, ref state);
                    while( lbfgs.minlbfgsiteration(ref state) )
                    {
                        state.f = 0;
                        for(i=0; i<=n-1; i++)
                        {
                            state.g[i] = 0;
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            v = 0.0;
                            for(i_=0; i_<=n-1;i_++)
                            {
                                v += a[i,i_]*state.x[i_];
                            }
                            state.f = state.f+AP.Math.Sqr(v-b[i]);
                            for(j=0; j<=n-1; j++)
                            {
                                state.g[j] = state.g[j]+2*(v-b[i])*a[i,j];
                            }
                        }
                    }
                    lbfgs.minlbfgsresults(ref state, ref x, ref rep);
                    eqerror = eqerror | rep.terminationtype<=0;
                    for(i=0; i<=n-1; i++)
                    {
                        eqerror = eqerror | (double)(Math.Abs(x[i]-xe[i]))>(double)(0.001);
                    }
                }
            }
            
            //
            // Testing convergence properties
            //
            converror = false;
            x = new double[2+1];
            n = 3;
            m = 2;
            for(i=0; i<=2; i++)
            {
                x[i] = 6*AP.Math.RandomReal()-3;
            }
            lbfgs.minlbfgs(n, m, ref x, 0.0001, 0.0, 0.0, 0, 0, ref state);
            while( lbfgs.minlbfgsiteration(ref state) )
            {
                state.f = AP.Math.Sqr(Math.Exp(state.x[0])-2)+AP.Math.Sqr(state.x[1])+AP.Math.Sqr(state.x[2]-state.x[0]);
                state.g[0] = 2*(Math.Exp(state.x[0])-2)*Math.Exp(state.x[0])+2*(state.x[0]-state.x[2]);
                state.g[1] = 2*state.x[1];
                state.g[2] = 2*(state.x[2]-state.x[0]);
            }
            lbfgs.minlbfgsresults(ref state, ref x, ref rep);
            converror = converror | (double)(Math.Abs(x[0]-Math.Log(2)))>(double)(0.05);
            converror = converror | (double)(Math.Abs(x[1]))>(double)(0.05);
            converror = converror | (double)(Math.Abs(x[2]-Math.Log(2)))>(double)(0.05);
            converror = converror | rep.terminationtype!=4;
            for(i=0; i<=2; i++)
            {
                x[i] = 6*AP.Math.RandomReal()-3;
            }
            lbfgs.minlbfgs(n, m, ref x, 0.0, 0.0001, 0.0, 0, 0, ref state);
            while( lbfgs.minlbfgsiteration(ref state) )
            {
                state.f = AP.Math.Sqr(Math.Exp(state.x[0])-2)+AP.Math.Sqr(state.x[1])+AP.Math.Sqr(state.x[2]-state.x[0]);
                state.g[0] = 2*(Math.Exp(state.x[0])-2)*Math.Exp(state.x[0])+2*(state.x[0]-state.x[2]);
                state.g[1] = 2*state.x[1];
                state.g[2] = 2*(state.x[2]-state.x[0]);
            }
            lbfgs.minlbfgsresults(ref state, ref x, ref rep);
            converror = converror | (double)(Math.Abs(x[0]-Math.Log(2)))>(double)(0.05);
            converror = converror | (double)(Math.Abs(x[1]))>(double)(0.05);
            converror = converror | (double)(Math.Abs(x[2]-Math.Log(2)))>(double)(0.05);
            converror = converror | rep.terminationtype!=1;
            for(i=0; i<=2; i++)
            {
                x[i] = 6*AP.Math.RandomReal()-3;
            }
            lbfgs.minlbfgs(n, m, ref x, 0.0, 0.0, 0.0001, 0, 0, ref state);
            while( lbfgs.minlbfgsiteration(ref state) )
            {
                state.f = AP.Math.Sqr(Math.Exp(state.x[0])-2)+AP.Math.Sqr(state.x[1])+AP.Math.Sqr(state.x[2]-state.x[0]);
                state.g[0] = 2*(Math.Exp(state.x[0])-2)*Math.Exp(state.x[0])+2*(state.x[0]-state.x[2]);
                state.g[1] = 2*state.x[1];
                state.g[2] = 2*(state.x[2]-state.x[0]);
            }
            lbfgs.minlbfgsresults(ref state, ref x, ref rep);
            converror = converror | (double)(Math.Abs(x[0]-Math.Log(2)))>(double)(0.05);
            converror = converror | (double)(Math.Abs(x[1]))>(double)(0.05);
            converror = converror | (double)(Math.Abs(x[2]-Math.Log(2)))>(double)(0.05);
            converror = converror | rep.terminationtype!=2;
            for(i=0; i<=2; i++)
            {
                x[i] = 2*AP.Math.RandomReal()-1;
            }
            lbfgs.minlbfgs(n, m, ref x, 0.0, 0.0, 0.0, 10, 0, ref state);
            while( lbfgs.minlbfgsiteration(ref state) )
            {
                state.f = AP.Math.Sqr(Math.Exp(state.x[0])-2)+AP.Math.Sqr(state.x[1])+AP.Math.Sqr(state.x[2]-state.x[0]);
                state.g[0] = 2*(Math.Exp(state.x[0])-2)*Math.Exp(state.x[0])+2*(state.x[0]-state.x[2]);
                state.g[1] = 2*state.x[1];
                state.g[2] = 2*(state.x[2]-state.x[0]);
            }
            lbfgs.minlbfgsresults(ref state, ref x, ref rep);
            converror = converror | rep.terminationtype!=5 | rep.iterationscount!=10;
            
            //
            // end
            //
            waserrors = referror | lin1error | lin2error | eqerror | converror;
            if( !silent )
            {
                System.Console.Write("TESTING L-BFGS OPTIMIZATION");
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
        public static bool testlbfgs_test_silent()
        {
            bool result = new bool();

            result = testminlbfgs(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testlbfgs_test()
        {
            bool result = new bool();

            result = testminlbfgs(false);
            return result;
        }
    }
}
