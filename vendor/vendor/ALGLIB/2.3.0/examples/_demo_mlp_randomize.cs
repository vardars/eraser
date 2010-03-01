using System;
using alglib;
public class _Demo
{
    public static int Main(string[] args)
    {
        mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

        mlpbase.mlpcreate0(2, 1, ref net);
        mlpbase.mlprandomize(ref net);
        return 0;
    }
}