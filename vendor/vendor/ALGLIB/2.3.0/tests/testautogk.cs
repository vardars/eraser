
using System;

namespace alglib
{
    public class testautogk
    {
        /*************************************************************************
        Test
        *************************************************************************/
        public static bool testautogkunit(bool silent)
        {
            bool result = new bool();
            double a = 0;
            double b = 0;
            autogk.autogkstate state = new autogk.autogkstate();
            autogk.autogkreport rep = new autogk.autogkreport();
            double v = 0;
            double exact = 0;
            double eabs = 0;
            double alpha = 0;
            int pkind = 0;
            double errtol = 0;
            bool simpleerrors = new bool();
            bool sngenderrors = new bool();
            bool waserrors = new bool();

            simpleerrors = false;
            sngenderrors = false;
            waserrors = false;
            errtol = 10000*AP.Math.MachineEpsilon;
            
            //
            // Simple test: integral(exp(x),+-1,+-2), no maximum width requirements
            //
            a = (2*AP.Math.RandomInteger(2)-1)*1.0;
            b = (2*AP.Math.RandomInteger(2)-1)*2.0;
            autogk.autogksmooth(a, b, ref state);
            while( autogk.autogkiteration(ref state) )
            {
                state.f = Math.Exp(state.x);
            }
            autogk.autogkresults(ref state, ref v, ref rep);
            exact = Math.Exp(b)-Math.Exp(a);
            eabs = Math.Abs(Math.Exp(b)-Math.Exp(a));
            if( rep.terminationtype<=0 )
            {
                simpleerrors = true;
            }
            else
            {
                simpleerrors = simpleerrors | (double)(Math.Abs(exact-v))>(double)(errtol*eabs);
            }
            
            //
            // Simple test: integral(exp(x),+-1,+-2), XWidth=0.1
            //
            a = (2*AP.Math.RandomInteger(2)-1)*1.0;
            b = (2*AP.Math.RandomInteger(2)-1)*2.0;
            autogk.autogksmoothw(a, b, 0.1, ref state);
            while( autogk.autogkiteration(ref state) )
            {
                state.f = Math.Exp(state.x);
            }
            autogk.autogkresults(ref state, ref v, ref rep);
            exact = Math.Exp(b)-Math.Exp(a);
            eabs = Math.Abs(Math.Exp(b)-Math.Exp(a));
            if( rep.terminationtype<=0 )
            {
                simpleerrors = true;
            }
            else
            {
                simpleerrors = simpleerrors | (double)(Math.Abs(exact-v))>(double)(errtol*eabs);
            }
            
            //
            // Simple test: integral(cos(100*x),0,2*pi), no maximum width requirements
            //
            a = 0;
            b = 2*Math.PI;
            autogk.autogksmooth(a, b, ref state);
            while( autogk.autogkiteration(ref state) )
            {
                state.f = Math.Cos(100*state.x);
            }
            autogk.autogkresults(ref state, ref v, ref rep);
            exact = 0;
            eabs = 4;
            if( rep.terminationtype<=0 )
            {
                simpleerrors = true;
            }
            else
            {
                simpleerrors = simpleerrors | (double)(Math.Abs(exact-v))>(double)(errtol*eabs);
            }
            
            //
            // Simple test: integral(cos(100*x),0,2*pi), XWidth=0.3
            //
            a = 0;
            b = 2*Math.PI;
            autogk.autogksmoothw(a, b, 0.3, ref state);
            while( autogk.autogkiteration(ref state) )
            {
                state.f = Math.Cos(100*state.x);
            }
            autogk.autogkresults(ref state, ref v, ref rep);
            exact = 0;
            eabs = 4;
            if( rep.terminationtype<=0 )
            {
                simpleerrors = true;
            }
            else
            {
                simpleerrors = simpleerrors | (double)(Math.Abs(exact-v))>(double)(errtol*eabs);
            }
            
            //
            // singular problem on [a,b] = [0.1, 0.5]
            //     f2(x) = (1+x)*(b-x)^alpha, -1 < alpha < 1
            //
            for(pkind=0; pkind<=6; pkind++)
            {
                a = 0.1;
                b = 0.5;
                if( pkind==0 )
                {
                    alpha = -0.9;
                }
                if( pkind==1 )
                {
                    alpha = -0.5;
                }
                if( pkind==2 )
                {
                    alpha = -0.1;
                }
                if( pkind==3 )
                {
                    alpha = 0.0;
                }
                if( pkind==4 )
                {
                    alpha = 0.1;
                }
                if( pkind==5 )
                {
                    alpha = 0.5;
                }
                if( pkind==6 )
                {
                    alpha = 0.9;
                }
                
                //
                // f1(x) = (1+x)*(x-a)^alpha, -1 < alpha < 1
                // 1. use singular integrator for [a,b]
                // 2. use singular integrator for [b,a]
                //
                exact = Math.Pow(b-a, alpha+2)/(alpha+2)+(1+a)*Math.Pow(b-a, alpha+1)/(alpha+1);
                eabs = Math.Abs(exact);
                autogk.autogksingular(a, b, alpha, 0.0, ref state);
                while( autogk.autogkiteration(ref state) )
                {
                    if( (double)(state.xminusa)<(double)(0.01) )
                    {
                        state.f = Math.Pow(state.xminusa, alpha)*(1+state.x);
                    }
                    else
                    {
                        state.f = Math.Pow(state.x-a, alpha)*(1+state.x);
                    }
                }
                autogk.autogkresults(ref state, ref v, ref rep);
                if( rep.terminationtype<=0 )
                {
                    sngenderrors = true;
                }
                else
                {
                    sngenderrors = sngenderrors | (double)(Math.Abs(v-exact))>(double)(errtol*eabs);
                }
                autogk.autogksingular(b, a, 0.0, alpha, ref state);
                while( autogk.autogkiteration(ref state) )
                {
                    if( (double)(state.bminusx)>(double)(-0.01) )
                    {
                        state.f = Math.Pow(-state.bminusx, alpha)*(1+state.x);
                    }
                    else
                    {
                        state.f = Math.Pow(state.x-a, alpha)*(1+state.x);
                    }
                }
                autogk.autogkresults(ref state, ref v, ref rep);
                if( rep.terminationtype<=0 )
                {
                    sngenderrors = true;
                }
                else
                {
                    sngenderrors = sngenderrors | (double)(Math.Abs(-v-exact))>(double)(errtol*eabs);
                }
                
                //
                // f1(x) = (1+x)*(b-x)^alpha, -1 < alpha < 1
                // 1. use singular integrator for [a,b]
                // 2. use singular integrator for [b,a]
                //
                exact = (1+b)*Math.Pow(b-a, alpha+1)/(alpha+1)-Math.Pow(b-a, alpha+2)/(alpha+2);
                eabs = Math.Abs(exact);
                autogk.autogksingular(a, b, 0.0, alpha, ref state);
                while( autogk.autogkiteration(ref state) )
                {
                    if( (double)(state.bminusx)<(double)(0.01) )
                    {
                        state.f = Math.Pow(state.bminusx, alpha)*(1+state.x);
                    }
                    else
                    {
                        state.f = Math.Pow(b-state.x, alpha)*(1+state.x);
                    }
                }
                autogk.autogkresults(ref state, ref v, ref rep);
                if( rep.terminationtype<=0 )
                {
                    sngenderrors = true;
                }
                else
                {
                    sngenderrors = sngenderrors | (double)(Math.Abs(v-exact))>(double)(errtol*eabs);
                }
                autogk.autogksingular(b, a, alpha, 0.0, ref state);
                while( autogk.autogkiteration(ref state) )
                {
                    if( (double)(state.xminusa)>(double)(-0.01) )
                    {
                        state.f = Math.Pow(-state.xminusa, alpha)*(1+state.x);
                    }
                    else
                    {
                        state.f = Math.Pow(b-state.x, alpha)*(1+state.x);
                    }
                }
                autogk.autogkresults(ref state, ref v, ref rep);
                if( rep.terminationtype<=0 )
                {
                    sngenderrors = true;
                }
                else
                {
                    sngenderrors = sngenderrors | (double)(Math.Abs(-v-exact))>(double)(errtol*eabs);
                }
            }
            
            //
            // end
            //
            waserrors = simpleerrors | sngenderrors;
            if( !silent )
            {
                System.Console.Write("TESTING AUTOGK");
                System.Console.WriteLine();
                System.Console.Write("INTEGRATION WITH GIVEN ACCURACY:          ");
                if( simpleerrors | sngenderrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* SIMPLE PROBLEMS:                        ");
                if( simpleerrors )
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                System.Console.Write("* SINGULAR PROBLEMS (ENDS OF INTERVAL):   ");
                if( sngenderrors )
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
        public static bool testautogk_test_silent()
        {
            bool result = new bool();

            result = testautogkunit(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testautogk_test()
        {
            bool result = new bool();

            result = testautogkunit(false);
            return result;
        }
    }
}
