using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        mlpbase.multilayerperceptron network1 = new mlpbase.multilayerperceptron();
        mlpbase.multilayerperceptron network2 = new mlpbase.multilayerperceptron();
        mlpbase.multilayerperceptron network3 = new mlpbase.multilayerperceptron();
        double[] x = new double[0];
        double[] y = new double[0];
        double[] r = new double[0];
        int rlen = 0;
        double v1 = 0;
        double v2 = 0;

        
        //
        // Generate two networks filled with small random values.
        // Use MLPSerialize/MLPUnserialize to make network copy.
        //
        mlpbase.mlpcreate0(1, 1, ref network1);
        mlpbase.mlpcreate0(1, 1, ref network2);
        mlpbase.mlpserialize(ref network1, ref r, ref rlen);
        mlpbase.mlpunserialize(ref r, ref network2);
        
        //
        // Now Network1 and Network2 should be identical.
        // Let's demonstrate it.
        //
        System.Console.Write("Test serialization/unserialization");
        System.Console.WriteLine();
        x = new double[1];
        y = new double[1];
        x[0] = 2*AP.Math.RandomReal()-1;
        mlpbase.mlpprocess(ref network1, ref x, ref y);
        v1 = y[0];
        System.Console.Write("Network1(X) = ");
        System.Console.Write("{0,0:F2}",y[0]);
        System.Console.WriteLine();
        mlpbase.mlpprocess(ref network2, ref x, ref y);
        v2 = y[0];
        System.Console.Write("Network2(X) = ");
        System.Console.Write("{0,0:F2}",y[0]);
        System.Console.WriteLine();
        if( (double)(v1)==(double)(v2) )
        {
            System.Console.Write("Results are equal, OK.");
            System.Console.WriteLine();
        }
        else
        {
            System.Console.Write("Results are not equal... Strange...");
        }
        return 0;
    }
}