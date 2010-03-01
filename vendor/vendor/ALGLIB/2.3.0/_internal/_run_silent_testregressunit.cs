using System;
public class _Test
{
	public static int Main(string[] args)
	{
        int seed;
        if( args.Length==1 )
            seed = Convert.ToInt32(args[0]);
        else
            seed = System.DateTime.Now.Millisecond + 1000*System.DateTime.Now.Second + 60*1000*System.DateTime.Now.Minute;
        AP.Math.RndObject = new System.Random(seed);
        try
        {
            if(!alglib.testregressunit.testregressunit_test_silent())
                throw new Exception("");
        }
        catch(Exception e)
        {
            System.Console.Write("SEED ");
            System.Console.Write("{0,9:d}",seed);
            System.Console.Write("    UNIT ");
            System.Console.WriteLine("linreg");
            return 1;
        }
        return 0;
	}
}