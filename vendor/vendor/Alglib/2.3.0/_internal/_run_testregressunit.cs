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
            if(!alglib.testregressunit.testregressunit_test())
                return 1;
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception catched:");
            Console.WriteLine(e.Message);
            return 1;
        }
        return 0;
	}
}