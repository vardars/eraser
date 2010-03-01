using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        autogk.autogkstate state = new autogk.autogkstate();
        double v = 0;
        autogk.autogkreport rep = new autogk.autogkreport();

        
        //
        // f(x) = x*sin(x), integrated at [-pi, pi].
        // Exact answer is 2*pi
        //
        autogk.autogksmooth(-Math.PI, +Math.PI, ref state);
        while( autogk.autogkiteration(ref state) )
        {
            state.f = state.x*Math.Sin(state.x);
        }
        autogk.autogkresults(ref state, ref v, ref rep);
        System.Console.Write("integral(x*sin(x),-pi,+pi) = ");
        System.Console.Write("{0,0:F2}",v);
        System.Console.WriteLine();
        System.Console.Write("Exact answer is ");
        System.Console.Write("{0,0:F2}",2*Math.PI);
        System.Console.WriteLine();
        return 0;
    }
}