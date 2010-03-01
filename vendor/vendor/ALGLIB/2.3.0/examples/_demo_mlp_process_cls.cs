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
        // classification task with 2 inputs and 3 classes.
        //
        // network weights are initialized with small random values.
        //
        mlpbase.mlpcreatec0(2, 3, ref net);
        x = new double[2];
        y = new double[3];
        x[0] = AP.Math.RandomReal()-0.5;
        x[1] = AP.Math.RandomReal()-0.5;
        mlpbase.mlpprocess(ref net, ref x, ref y);
        
        //
        // output results
        //
        System.Console.Write("Classification task");
        System.Console.WriteLine();
        System.Console.Write("IN[0]  = ");
        System.Console.Write("{0,5:F2}",x[0]);
        System.Console.WriteLine();
        System.Console.Write("IN[1]  = ");
        System.Console.Write("{0,5:F2}",x[1]);
        System.Console.WriteLine();
        System.Console.Write("Prob(Class=0|IN) = ");
        System.Console.Write("{0,5:F2}",y[0]);
        System.Console.WriteLine();
        System.Console.Write("Prob(Class=1|IN) = ");
        System.Console.Write("{0,5:F2}",y[1]);
        System.Console.WriteLine();
        System.Console.Write("Prob(Class=2|IN) = ");
        System.Console.Write("{0,5:F2}",y[2]);
        System.Console.WriteLine();
        return 0;
    }
}