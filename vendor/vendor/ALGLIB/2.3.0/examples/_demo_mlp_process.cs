using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();
        double[] x = new double[0];
        double[] y = new double[0];

        
        //
        // regression task with 2 inputs (independent variables)
        // and 2 outputs (dependent variables).
        //
        // network weights are initialized with small random values.
        //
        mlpbase.mlpcreate0(2, 2, ref net);
        x = new double[2];
        y = new double[2];
        x[0] = AP.Math.RandomReal()-0.5;
        x[1] = AP.Math.RandomReal()-0.5;
        mlpbase.mlpprocess(ref net, ref x, ref y);
        System.Console.Write("Regression task");
        System.Console.WriteLine();
        System.Console.Write("IN[0]  = ");
        System.Console.Write("{0,5:F2}",x[0]);
        System.Console.WriteLine();
        System.Console.Write("IN[1]  = ");
        System.Console.Write("{0,5:F2}",x[1]);
        System.Console.WriteLine();
        System.Console.Write("OUT[0] = ");
        System.Console.Write("{0,5:F2}",y[0]);
        System.Console.WriteLine();
        System.Console.Write("OUT[1] = ");
        System.Console.Write("{0,5:F2}",y[1]);
        System.Console.WriteLine();
        return 0;
    }
}