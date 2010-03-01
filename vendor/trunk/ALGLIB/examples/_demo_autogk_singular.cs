using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        autogk.autogkstate state = new autogk.autogkstate();
        double v = 0;
        autogk.autogkreport rep = new autogk.autogkreport();
        double a = 0;
        double b = 0;
        double alpha = 0;

        
        //
        // f1(x) = (1+x)*(x-a)^alpha, alpha=-0.3
        // Exact answer is (B-A)^(Alpha+2)/(Alpha+2) + (1+A)*(B-A)^(Alpha+1)/(Alpha+1)
        //
        // This code demonstrates use of the State.XMinusA (State.BMinusX) field.
        //
        // If we try to use State.X instead of State.XMinusA,
        // we will end up dividing by zero! (in 64-bit precision)
        //
        a = 1.0;
        b = 5.0;
        alpha = -0.9;
        autogk.autogksingular(a, b, alpha, 0.0, ref state);
        while( autogk.autogkiteration(ref state) )
        {
            state.f = Math.Pow(state.xminusa, alpha)*(1+state.x);
        }
        autogk.autogkresults(ref state, ref v, ref rep);
        System.Console.Write("integral((1+x)*(x-a)^alpha) on [");
        System.Console.Write("{0,0:F1}",a);
        System.Console.Write("; ");
        System.Console.Write("{0,0:F1}",b);
        System.Console.Write("] = ");
        System.Console.Write("{0,0:F2}",v);
        System.Console.WriteLine();
        System.Console.Write("Exact answer is ");
        System.Console.Write("{0,0:F2}",Math.Pow(b-a, alpha+2)/(alpha+2)+(1+a)*Math.Pow(b-a, alpha+1)/(alpha+1));
        System.Console.WriteLine();
        return 0;
    }
}