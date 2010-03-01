
using System;

namespace alglib
{
    public class testmlpeunit
    {
        public static bool testmlpe(bool silent)
        {
            bool result = new bool();
            bool waserrors = new bool();
            int passcount = 0;
            int maxn = 0;
            int maxhid = 0;
            int nf = 0;
            int nhid = 0;
            int nl = 0;
            int nhid1 = 0;
            int nhid2 = 0;
            int ec = 0;
            int nkind = 0;
            int algtype = 0;
            int tasktype = 0;
            int pass = 0;
            mlpe.mlpensemble ensemble = new mlpe.mlpensemble();
            mlptrain.mlpreport rep = new mlptrain.mlpreport();
            mlptrain.mlpcvreport oobrep = new mlptrain.mlpcvreport();
            double[,] xy = new double[0,0];
            int i = 0;
            int j = 0;
            int nin = 0;
            int nout = 0;
            int npoints = 0;
            double e = 0;
            int info = 0;
            int nless = 0;
            int nall = 0;
            int nclasses = 0;
            bool allsame = new bool();
            bool inferrors = new bool();
            bool procerrors = new bool();
            bool trnerrors = new bool();

            waserrors = false;
            inferrors = false;
            procerrors = false;
            trnerrors = false;
            passcount = 10;
            maxn = 4;
            maxhid = 4;
            
            //
            // General MLP ensembles tests
            //
            for(nf=1; nf<=maxn; nf++)
            {
                for(nl=1; nl<=maxn; nl++)
                {
                    for(nhid1=0; nhid1<=maxhid; nhid1++)
                    {
                        for(nhid2=0; nhid2<=0; nhid2++)
                        {
                            for(nkind=0; nkind<=3; nkind++)
                            {
                                for(ec=1; ec<=3; ec++)
                                {
                                    
                                    //
                                    //  Skip meaningless parameters combinations
                                    //
                                    if( nkind==1 & nl<2 )
                                    {
                                        continue;
                                    }
                                    if( nhid1==0 & nhid2!=0 )
                                    {
                                        continue;
                                    }
                                    
                                    //
                                    // Tests
                                    //
                                    testinformational(nkind, nf, nhid1, nhid2, nl, ec, passcount, ref inferrors);
                                    testprocessing(nkind, nf, nhid1, nhid2, nl, ec, passcount, ref procerrors);
                                }
                            }
                        }
                    }
                }
            }
            
            //
            // network training must reduce error
            // test on random regression task
            //
            nin = 3;
            nout = 2;
            nhid = 5;
            npoints = 100;
            nless = 0;
            nall = 0;
            for(pass=1; pass<=10; pass++)
            {
                for(algtype=0; algtype<=1; algtype++)
                {
                    for(tasktype=0; tasktype<=1; tasktype++)
                    {
                        if( tasktype==0 )
                        {
                            xy = new double[npoints-1+1, nin+nout-1+1];
                            for(i=0; i<=npoints-1; i++)
                            {
                                for(j=0; j<=nin+nout-1; j++)
                                {
                                    xy[i,j] = 2*AP.Math.RandomReal()-1;
                                }
                            }
                            mlpe.mlpecreate1(nin, nhid, nout, 1+AP.Math.RandomInteger(3), ref ensemble);
                        }
                        else
                        {
                            xy = new double[npoints-1+1, nin+1];
                            nclasses = 2+AP.Math.RandomInteger(2);
                            for(i=0; i<=npoints-1; i++)
                            {
                                for(j=0; j<=nin-1; j++)
                                {
                                    xy[i,j] = 2*AP.Math.RandomReal()-1;
                                }
                                xy[i,nin] = AP.Math.RandomInteger(nclasses);
                            }
                            mlpe.mlpecreatec1(nin, nhid, nclasses, 1+AP.Math.RandomInteger(3), ref ensemble);
                        }
                        e = mlpe.mlpermserror(ref ensemble, ref xy, npoints);
                        if( algtype==0 )
                        {
                            mlpe.mlpebagginglm(ref ensemble, ref xy, npoints, 0.001, 1, ref info, ref rep, ref oobrep);
                        }
                        else
                        {
                            mlpe.mlpebagginglbfgs(ref ensemble, ref xy, npoints, 0.001, 1, 0.01, 0, ref info, ref rep, ref oobrep);
                        }
                        if( info<0 )
                        {
                            trnerrors = true;
                        }
                        else
                        {
                            if( (double)(mlpe.mlpermserror(ref ensemble, ref xy, npoints))<(double)(e) )
                            {
                                nless = nless+1;
                            }
                        }
                        nall = nall+1;
                    }
                }
            }
            trnerrors = trnerrors | (double)(nall-nless)>(double)(0.3*nall);
            
            //
            // Final report
            //
            waserrors = inferrors | procerrors | trnerrors;
            if( !silent )
            {
                System.Console.Write("MLP ENSEMBLE TEST");
                System.Console.WriteLine();
                System.Console.Write("INFORMATIONAL FUNCTIONS:                 ");
                if( !inferrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("BASIC PROCESSING:                        ");
                if( !procerrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("TRAINING:                                ");
                if( !trnerrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                if( waserrors )
                {
                    System.Console.Write("TEST SUMMARY: FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("TEST SUMMARY: PASSED");
                    System.Console.WriteLine();
                }
                System.Console.WriteLine();
                System.Console.WriteLine();
            }
            result = !waserrors;
            return result;
        }


        /*************************************************************************
        Network creation
        *************************************************************************/
        private static void createensemble(ref mlpe.mlpensemble ensemble,
            int nkind,
            double a1,
            double a2,
            int nin,
            int nhid1,
            int nhid2,
            int nout,
            int ec)
        {
            System.Diagnostics.Debug.Assert(nin>0 & nhid1>=0 & nhid2>=0 & nout>0, "CreateNetwork error");
            System.Diagnostics.Debug.Assert(nhid1!=0 | nhid2==0, "CreateNetwork error");
            System.Diagnostics.Debug.Assert(nkind!=1 | nout>=2, "CreateNetwork error");
            if( nhid1==0 )
            {
                
                //
                // No hidden layers
                //
                if( nkind==0 )
                {
                    mlpe.mlpecreate0(nin, nout, ec, ref ensemble);
                }
                else
                {
                    if( nkind==1 )
                    {
                        mlpe.mlpecreatec0(nin, nout, ec, ref ensemble);
                    }
                    else
                    {
                        if( nkind==2 )
                        {
                            mlpe.mlpecreateb0(nin, nout, a1, a2, ec, ref ensemble);
                        }
                        else
                        {
                            if( nkind==3 )
                            {
                                mlpe.mlpecreater0(nin, nout, a1, a2, ec, ref ensemble);
                            }
                        }
                    }
                }
                return;
            }
            if( nhid2==0 )
            {
                
                //
                // One hidden layer
                //
                if( nkind==0 )
                {
                    mlpe.mlpecreate1(nin, nhid1, nout, ec, ref ensemble);
                }
                else
                {
                    if( nkind==1 )
                    {
                        mlpe.mlpecreatec1(nin, nhid1, nout, ec, ref ensemble);
                    }
                    else
                    {
                        if( nkind==2 )
                        {
                            mlpe.mlpecreateb1(nin, nhid1, nout, a1, a2, ec, ref ensemble);
                        }
                        else
                        {
                            if( nkind==3 )
                            {
                                mlpe.mlpecreater1(nin, nhid1, nout, a1, a2, ec, ref ensemble);
                            }
                        }
                    }
                }
                return;
            }
            
            //
            // Two hidden layers
            //
            if( nkind==0 )
            {
                mlpe.mlpecreate2(nin, nhid1, nhid2, nout, ec, ref ensemble);
            }
            else
            {
                if( nkind==1 )
                {
                    mlpe.mlpecreatec2(nin, nhid1, nhid2, nout, ec, ref ensemble);
                }
                else
                {
                    if( nkind==2 )
                    {
                        mlpe.mlpecreateb2(nin, nhid1, nhid2, nout, a1, a2, ec, ref ensemble);
                    }
                    else
                    {
                        if( nkind==3 )
                        {
                            mlpe.mlpecreater2(nin, nhid1, nhid2, nout, a1, a2, ec, ref ensemble);
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Unsets network (initialize it to smallest network possible
        *************************************************************************/
        private static void unsetensemble(ref mlpe.mlpensemble ensemble)
        {
            mlpe.mlpecreate0(1, 1, 1, ref ensemble);
        }


        /*************************************************************************
        Iformational functions test
        *************************************************************************/
        private static void testinformational(int nkind,
            int nin,
            int nhid1,
            int nhid2,
            int nout,
            int ec,
            int passcount,
            ref bool err)
        {
            mlpe.mlpensemble ensemble = new mlpe.mlpensemble();
            int n1 = 0;
            int n2 = 0;

            createensemble(ref ensemble, nkind, -1.0, 1.0, nin, nhid1, nhid2, nout, ec);
            mlpe.mlpeproperties(ref ensemble, ref n1, ref n2);
            err = err | n1!=nin | n2!=nout;
        }


        /*************************************************************************
        Processing functions test
        *************************************************************************/
        private static void testprocessing(int nkind,
            int nin,
            int nhid1,
            int nhid2,
            int nout,
            int ec,
            int passcount,
            ref bool err)
        {
            mlpe.mlpensemble ensemble = new mlpe.mlpensemble();
            mlpe.mlpensemble ensemble2 = new mlpe.mlpensemble();
            bool zeronet = new bool();
            double a1 = 0;
            double a2 = 0;
            int pass = 0;
            int i = 0;
            bool allsame = new bool();
            int rlen = 0;
            double[] x1 = new double[0];
            double[] x2 = new double[0];
            double[] y1 = new double[0];
            double[] y2 = new double[0];
            double[] ra = new double[0];
            double[] ra2 = new double[0];
            double v = 0;

            
            //
            // Prepare network
            //
            a1 = 0;
            a2 = 0;
            if( nkind==2 )
            {
                a1 = 1000*AP.Math.RandomReal()-500;
                a2 = 2*AP.Math.RandomReal()-1;
            }
            if( nkind==3 )
            {
                a1 = 1000*AP.Math.RandomReal()-500;
                a2 = a1+(2*AP.Math.RandomInteger(2)-1)*(0.1+0.9*AP.Math.RandomReal());
            }
            
            //
            // Initialize arrays
            //
            x1 = new double[nin-1+1];
            x2 = new double[nin-1+1];
            y1 = new double[nout-1+1];
            y2 = new double[nout-1+1];
            
            //
            // Main cycle
            //
            for(pass=1; pass<=passcount; pass++)
            {
                createensemble(ref ensemble, nkind, a1, a2, nin, nhid1, nhid2, nout, ec);
                
                //
                // Same inputs leads to same outputs
                //
                for(i=0; i<=nin-1; i++)
                {
                    x1[i] = 2*AP.Math.RandomReal()-1;
                    x2[i] = x1[i];
                }
                for(i=0; i<=nout-1; i++)
                {
                    y1[i] = 2*AP.Math.RandomReal()-1;
                    y2[i] = 2*AP.Math.RandomReal()-1;
                }
                mlpe.mlpeprocess(ref ensemble, ref x1, ref y1);
                mlpe.mlpeprocess(ref ensemble, ref x2, ref y2);
                allsame = true;
                for(i=0; i<=nout-1; i++)
                {
                    allsame = allsame & (double)(y1[i])==(double)(y2[i]);
                }
                err = err | !allsame;
                
                //
                // Same inputs on original network leads to same outputs
                // on copy created using MLPCopy
                //
                unsetensemble(ref ensemble2);
                mlpe.mlpecopy(ref ensemble, ref ensemble2);
                for(i=0; i<=nin-1; i++)
                {
                    x1[i] = 2*AP.Math.RandomReal()-1;
                    x2[i] = x1[i];
                }
                for(i=0; i<=nout-1; i++)
                {
                    y1[i] = 2*AP.Math.RandomReal()-1;
                    y2[i] = 2*AP.Math.RandomReal()-1;
                }
                mlpe.mlpeprocess(ref ensemble, ref x1, ref y1);
                mlpe.mlpeprocess(ref ensemble2, ref x2, ref y2);
                allsame = true;
                for(i=0; i<=nout-1; i++)
                {
                    allsame = allsame & (double)(y1[i])==(double)(y2[i]);
                }
                err = err | !allsame;
                
                //
                // Same inputs on original network leads to same outputs
                // on copy created using MLPSerialize
                //
                unsetensemble(ref ensemble2);
                mlpe.mlpeserialize(ref ensemble, ref ra, ref rlen);
                ra2 = new double[rlen-1+1];
                for(i=0; i<=rlen-1; i++)
                {
                    ra2[i] = ra[i];
                }
                mlpe.mlpeunserialize(ref ra2, ref ensemble2);
                for(i=0; i<=nin-1; i++)
                {
                    x1[i] = 2*AP.Math.RandomReal()-1;
                    x2[i] = x1[i];
                }
                for(i=0; i<=nout-1; i++)
                {
                    y1[i] = 2*AP.Math.RandomReal()-1;
                    y2[i] = 2*AP.Math.RandomReal()-1;
                }
                mlpe.mlpeprocess(ref ensemble, ref x1, ref y1);
                mlpe.mlpeprocess(ref ensemble2, ref x2, ref y2);
                allsame = true;
                for(i=0; i<=nout-1; i++)
                {
                    allsame = allsame & (double)(y1[i])==(double)(y2[i]);
                }
                err = err | !allsame;
                
                //
                // Different inputs leads to different outputs (non-zero network)
                //
                for(i=0; i<=nin-1; i++)
                {
                    x1[i] = 2*AP.Math.RandomReal()-1;
                    x2[i] = 2*AP.Math.RandomReal()-1;
                }
                for(i=0; i<=nout-1; i++)
                {
                    y1[i] = 2*AP.Math.RandomReal()-1;
                    y2[i] = y1[i];
                }
                mlpe.mlpeprocess(ref ensemble, ref x1, ref y1);
                mlpe.mlpeprocess(ref ensemble, ref x2, ref y2);
                allsame = true;
                for(i=0; i<=nout-1; i++)
                {
                    allsame = allsame & (double)(y1[i])==(double)(y2[i]);
                }
                err = err | allsame;
                
                //
                // Randomization changes outputs (when inputs are unchanged, non-zero network)
                //
                for(i=0; i<=nin-1; i++)
                {
                    x1[i] = 2*AP.Math.RandomReal()-1;
                    x2[i] = 2*AP.Math.RandomReal()-1;
                }
                for(i=0; i<=nout-1; i++)
                {
                    y1[i] = 2*AP.Math.RandomReal()-1;
                    y2[i] = y1[i];
                }
                mlpe.mlpecopy(ref ensemble, ref ensemble2);
                mlpe.mlperandomize(ref ensemble2);
                mlpe.mlpeprocess(ref ensemble, ref x1, ref y1);
                mlpe.mlpeprocess(ref ensemble2, ref x1, ref y2);
                allsame = true;
                for(i=0; i<=nout-1; i++)
                {
                    allsame = allsame & (double)(y1[i])==(double)(y2[i]);
                }
                err = err | allsame;
                
                //
                // Normalization properties
                //
                if( nkind==1 )
                {
                    
                    //
                    // Classifier network outputs are normalized
                    //
                    for(i=0; i<=nin-1; i++)
                    {
                        x1[i] = 2*AP.Math.RandomReal()-1;
                    }
                    mlpe.mlpeprocess(ref ensemble, ref x1, ref y1);
                    v = 0;
                    for(i=0; i<=nout-1; i++)
                    {
                        v = v+y1[i];
                        err = err | (double)(y1[i])<(double)(0);
                    }
                    err = err | (double)(Math.Abs(v-1))>(double)(1000*AP.Math.MachineEpsilon);
                }
                if( nkind==2 )
                {
                    
                    //
                    // B-type network outputs are bounded from above/below
                    //
                    for(i=0; i<=nin-1; i++)
                    {
                        x1[i] = 2*AP.Math.RandomReal()-1;
                    }
                    mlpe.mlpeprocess(ref ensemble, ref x1, ref y1);
                    for(i=0; i<=nout-1; i++)
                    {
                        if( (double)(a2)>=(double)(0) )
                        {
                            err = err | (double)(y1[i])<(double)(a1);
                        }
                        else
                        {
                            err = err | (double)(y1[i])>(double)(a1);
                        }
                    }
                }
                if( nkind==3 )
                {
                    
                    //
                    // R-type network outputs are within [A1,A2] (or [A2,A1])
                    //
                    for(i=0; i<=nin-1; i++)
                    {
                        x1[i] = 2*AP.Math.RandomReal()-1;
                    }
                    mlpe.mlpeprocess(ref ensemble, ref x1, ref y1);
                    for(i=0; i<=nout-1; i++)
                    {
                        err = err | (double)(y1[i])<(double)(Math.Min(a1, a2)) | (double)(y1[i])>(double)(Math.Max(a1, a2));
                    }
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testmlpeunit_test_silent()
        {
            bool result = new bool();

            result = testmlpe(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testmlpeunit_test()
        {
            bool result = new bool();

            result = testmlpe(false);
            return result;
        }
    }
}
