
using System;

namespace alglib
{
    public class testmlpunit
    {
        public static bool testmlp(bool silent)
        {
            bool result = new bool();
            bool waserrors = new bool();
            int passcount = 0;
            int maxn = 0;
            int maxhid = 0;
            int info = 0;
            int nf = 0;
            int nhid = 0;
            int nl = 0;
            int nhid1 = 0;
            int nhid2 = 0;
            int nkind = 0;
            int i = 0;
            int j = 0;
            mlpbase.multilayerperceptron network = new mlpbase.multilayerperceptron();
            mlpbase.multilayerperceptron network2 = new mlpbase.multilayerperceptron();
            mlptrain.mlpreport rep = new mlptrain.mlpreport();
            mlptrain.mlpcvreport cvrep = new mlptrain.mlpcvreport();
            int ncount = 0;
            double[,] xy = new double[0,0];
            double[,] valxy = new double[0,0];
            int ssize = 0;
            int valsize = 0;
            bool allsame = new bool();
            bool inferrors = new bool();
            bool procerrors = new bool();
            bool graderrors = new bool();
            bool hesserrors = new bool();
            bool trnerrors = new bool();

            waserrors = false;
            inferrors = false;
            procerrors = false;
            graderrors = false;
            hesserrors = false;
            trnerrors = false;
            passcount = 10;
            maxn = 4;
            maxhid = 4;
            
            //
            // General multilayer network tests
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
                                testinformational(nkind, nf, nhid1, nhid2, nl, passcount, ref inferrors);
                                testprocessing(nkind, nf, nhid1, nhid2, nl, passcount, ref procerrors);
                                testgradient(nkind, nf, nhid1, nhid2, nl, passcount, ref graderrors);
                                testhessian(nkind, nf, nhid1, nhid2, nl, passcount, ref hesserrors);
                            }
                        }
                    }
                }
            }
            
            //
            // Test network training on simple XOR problem
            //
            xy = new double[3+1, 2+1];
            xy[0,0] = -1;
            xy[0,1] = -1;
            xy[0,2] = -1;
            xy[1,0] = +1;
            xy[1,1] = -1;
            xy[1,2] = +1;
            xy[2,0] = -1;
            xy[2,1] = +1;
            xy[2,2] = +1;
            xy[3,0] = +1;
            xy[3,1] = +1;
            xy[3,2] = -1;
            mlpbase.mlpcreate1(2, 2, 1, ref network);
            mlptrain.mlptrainlm(ref network, ref xy, 4, 0.001, 10, ref info, ref rep);
            trnerrors = trnerrors | (double)(mlpbase.mlprmserror(ref network, ref xy, 4))>(double)(0.1);
            
            //
            // Test CV on random noisy problem
            //
            ncount = 100;
            xy = new double[ncount-1+1, 1+1];
            for(i=0; i<=ncount-1; i++)
            {
                xy[i,0] = 2*AP.Math.RandomReal()-1;
                xy[i,1] = AP.Math.RandomInteger(4);
            }
            mlpbase.mlpcreatec0(1, 4, ref network);
            mlptrain.mlpkfoldcvlm(ref network, ref xy, ncount, 0.001, 5, 10, ref info, ref rep, ref cvrep);
            
            //
            // Final report
            //
            waserrors = inferrors | procerrors | graderrors | hesserrors | trnerrors;
            if( !silent )
            {
                System.Console.Write("MLP TEST");
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
                System.Console.Write("GRADIENT CALCULATION:                    ");
                if( !graderrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("HESSIAN CALCULATION:                     ");
                if( !hesserrors )
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
        private static void createnetwork(ref mlpbase.multilayerperceptron network,
            int nkind,
            double a1,
            double a2,
            int nin,
            int nhid1,
            int nhid2,
            int nout)
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
                    mlpbase.mlpcreate0(nin, nout, ref network);
                }
                else
                {
                    if( nkind==1 )
                    {
                        mlpbase.mlpcreatec0(nin, nout, ref network);
                    }
                    else
                    {
                        if( nkind==2 )
                        {
                            mlpbase.mlpcreateb0(nin, nout, a1, a2, ref network);
                        }
                        else
                        {
                            if( nkind==3 )
                            {
                                mlpbase.mlpcreater0(nin, nout, a1, a2, ref network);
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
                    mlpbase.mlpcreate1(nin, nhid1, nout, ref network);
                }
                else
                {
                    if( nkind==1 )
                    {
                        mlpbase.mlpcreatec1(nin, nhid1, nout, ref network);
                    }
                    else
                    {
                        if( nkind==2 )
                        {
                            mlpbase.mlpcreateb1(nin, nhid1, nout, a1, a2, ref network);
                        }
                        else
                        {
                            if( nkind==3 )
                            {
                                mlpbase.mlpcreater1(nin, nhid1, nout, a1, a2, ref network);
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
                mlpbase.mlpcreate2(nin, nhid1, nhid2, nout, ref network);
            }
            else
            {
                if( nkind==1 )
                {
                    mlpbase.mlpcreatec2(nin, nhid1, nhid2, nout, ref network);
                }
                else
                {
                    if( nkind==2 )
                    {
                        mlpbase.mlpcreateb2(nin, nhid1, nhid2, nout, a1, a2, ref network);
                    }
                    else
                    {
                        if( nkind==3 )
                        {
                            mlpbase.mlpcreater2(nin, nhid1, nhid2, nout, a1, a2, ref network);
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Unsets network (initialize it to smallest network possible
        *************************************************************************/
        private static void unsetnetwork(ref mlpbase.multilayerperceptron network)
        {
            mlpbase.mlpcreate0(1, 1, ref network);
        }


        /*************************************************************************
        Iformational functions test
        *************************************************************************/
        private static void testinformational(int nkind,
            int nin,
            int nhid1,
            int nhid2,
            int nout,
            int passcount,
            ref bool err)
        {
            mlpbase.multilayerperceptron network = new mlpbase.multilayerperceptron();
            int n1 = 0;
            int n2 = 0;
            int wcount = 0;

            createnetwork(ref network, nkind, 0.0, 0.0, nin, nhid1, nhid2, nout);
            mlpbase.mlpproperties(ref network, ref n1, ref n2, ref wcount);
            err = err | n1!=nin | n2!=nout | wcount<=0;
        }


        /*************************************************************************
        Processing functions test
        *************************************************************************/
        private static void testprocessing(int nkind,
            int nin,
            int nhid1,
            int nhid2,
            int nout,
            int passcount,
            ref bool err)
        {
            mlpbase.multilayerperceptron network = new mlpbase.multilayerperceptron();
            mlpbase.multilayerperceptron network2 = new mlpbase.multilayerperceptron();
            int n1 = 0;
            int n2 = 0;
            int wcount = 0;
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
            int i_ = 0;

            System.Diagnostics.Debug.Assert(passcount>=2, "PassCount<2!");
            
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
            createnetwork(ref network, nkind, a1, a2, nin, nhid1, nhid2, nout);
            mlpbase.mlpproperties(ref network, ref n1, ref n2, ref wcount);
            
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
                
                //
                // Last run is made on zero network
                //
                mlpbase.mlprandomizefull(ref network);
                zeronet = false;
                if( pass==passcount )
                {
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        network.weights[i_] = 0*network.weights[i_];
                    }
                    zeronet = true;
                }
                
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
                mlpbase.mlpprocess(ref network, ref x1, ref y1);
                mlpbase.mlpprocess(ref network, ref x2, ref y2);
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
                unsetnetwork(ref network2);
                mlpbase.mlpcopy(ref network, ref network2);
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
                mlpbase.mlpprocess(ref network, ref x1, ref y1);
                mlpbase.mlpprocess(ref network2, ref x2, ref y2);
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
                unsetnetwork(ref network2);
                mlpbase.mlpserialize(ref network, ref ra, ref rlen);
                ra2 = new double[rlen-1+1];
                for(i=0; i<=rlen-1; i++)
                {
                    ra2[i] = ra[i];
                }
                mlpbase.mlpunserialize(ref ra2, ref network2);
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
                mlpbase.mlpprocess(ref network, ref x1, ref y1);
                mlpbase.mlpprocess(ref network2, ref x2, ref y2);
                allsame = true;
                for(i=0; i<=nout-1; i++)
                {
                    allsame = allsame & (double)(y1[i])==(double)(y2[i]);
                }
                err = err | !allsame;
                
                //
                // Different inputs leads to different outputs (non-zero network)
                //
                if( !zeronet )
                {
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
                    mlpbase.mlpprocess(ref network, ref x1, ref y1);
                    mlpbase.mlpprocess(ref network, ref x2, ref y2);
                    allsame = true;
                    for(i=0; i<=nout-1; i++)
                    {
                        allsame = allsame & (double)(y1[i])==(double)(y2[i]);
                    }
                    err = err | allsame;
                }
                
                //
                // Randomization changes outputs (when inputs are unchanged, non-zero network)
                //
                if( !zeronet )
                {
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
                    mlpbase.mlpcopy(ref network, ref network2);
                    mlpbase.mlprandomize(ref network2);
                    mlpbase.mlpprocess(ref network, ref x1, ref y1);
                    mlpbase.mlpprocess(ref network2, ref x1, ref y2);
                    allsame = true;
                    for(i=0; i<=nout-1; i++)
                    {
                        allsame = allsame & (double)(y1[i])==(double)(y2[i]);
                    }
                    err = err | allsame;
                }
                
                //
                // Full randomization changes outputs (when inputs are unchanged, non-zero network)
                //
                if( !zeronet )
                {
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
                    mlpbase.mlpcopy(ref network, ref network2);
                    mlpbase.mlprandomizefull(ref network2);
                    mlpbase.mlpprocess(ref network, ref x1, ref y1);
                    mlpbase.mlpprocess(ref network2, ref x1, ref y2);
                    allsame = true;
                    for(i=0; i<=nout-1; i++)
                    {
                        allsame = allsame & (double)(y1[i])==(double)(y2[i]);
                    }
                    err = err | allsame;
                }
                
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
                    mlpbase.mlpprocess(ref network, ref x1, ref y1);
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
                    mlpbase.mlpprocess(ref network, ref x1, ref y1);
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
                    mlpbase.mlpprocess(ref network, ref x1, ref y1);
                    for(i=0; i<=nout-1; i++)
                    {
                        err = err | (double)(y1[i])<(double)(Math.Min(a1, a2)) | (double)(y1[i])>(double)(Math.Max(a1, a2));
                    }
                }
            }
        }


        /*************************************************************************
        Gradient functions test
        *************************************************************************/
        private static void testgradient(int nkind,
            int nin,
            int nhid1,
            int nhid2,
            int nout,
            int passcount,
            ref bool err)
        {
            mlpbase.multilayerperceptron network = new mlpbase.multilayerperceptron();
            mlpbase.multilayerperceptron network2 = new mlpbase.multilayerperceptron();
            int n1 = 0;
            int n2 = 0;
            int wcount = 0;
            bool zeronet = new bool();
            double h = 0;
            double etol = 0;
            double a1 = 0;
            double a2 = 0;
            int pass = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            bool allsame = new bool();
            int ilen = 0;
            int rlen = 0;
            int ssize = 0;
            double[,] xy = new double[0,0];
            double[] grad1 = new double[0];
            double[] grad2 = new double[0];
            double[] x = new double[0];
            double[] y = new double[0];
            double[] x1 = new double[0];
            double[] x2 = new double[0];
            double[] y1 = new double[0];
            double[] y2 = new double[0];
            int[] ia = new int[0];
            double[] ra = new double[0];
            double v = 0;
            double e = 0;
            double e1 = 0;
            double e2 = 0;
            double v1 = 0;
            double v2 = 0;
            double v3 = 0;
            double v4 = 0;
            double wprev = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(passcount>=2, "PassCount<2!");
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
            createnetwork(ref network, nkind, a1, a2, nin, nhid1, nhid2, nout);
            mlpbase.mlpproperties(ref network, ref n1, ref n2, ref wcount);
            h = 0.0001;
            etol = 0.01;
            
            //
            // Initialize
            //
            x = new double[nin-1+1];
            x1 = new double[nin-1+1];
            x2 = new double[nin-1+1];
            y = new double[nout-1+1];
            y1 = new double[nout-1+1];
            y2 = new double[nout-1+1];
            grad1 = new double[wcount-1+1];
            grad2 = new double[wcount-1+1];
            
            //
            // Process
            //
            for(pass=1; pass<=passcount; pass++)
            {
                mlpbase.mlprandomizefull(ref network);
                
                //
                // Test error/gradient calculation (least squares)
                //
                xy = new double[0+1, nin+nout-1+1];
                for(i=0; i<=nin-1; i++)
                {
                    x[i] = 4*AP.Math.RandomReal()-2;
                }
                for(i_=0; i_<=nin-1;i_++)
                {
                    xy[0,i_] = x[i_];
                }
                if( mlpbase.mlpissoftmax(ref network) )
                {
                    for(i=0; i<=nout-1; i++)
                    {
                        y[i] = 0;
                    }
                    xy[0,nin] = AP.Math.RandomInteger(nout);
                    y[(int)Math.Round(xy[0,nin])] = 1;
                }
                else
                {
                    for(i=0; i<=nout-1; i++)
                    {
                        y[i] = 4*AP.Math.RandomReal()-2;
                    }
                    i1_ = (0) - (nin);
                    for(i_=nin; i_<=nin+nout-1;i_++)
                    {
                        xy[0,i_] = y[i_+i1_];
                    }
                }
                mlpbase.mlpgrad(ref network, ref x, ref y, ref e, ref grad2);
                mlpbase.mlpprocess(ref network, ref x, ref y2);
                for(i_=0; i_<=nout-1;i_++)
                {
                    y2[i_] = y2[i_] - y[i_];
                }
                v = 0.0;
                for(i_=0; i_<=nout-1;i_++)
                {
                    v += y2[i_]*y2[i_];
                }
                v = v/2;
                err = err | (double)(Math.Abs((v-e)/v))>(double)(etol);
                err = err | (double)(Math.Abs((mlpbase.mlperror(ref network, ref xy, 1)-v)/v))>(double)(etol);
                for(i=0; i<=wcount-1; i++)
                {
                    wprev = network.weights[i];
                    network.weights[i] = wprev-2*h;
                    mlpbase.mlpprocess(ref network, ref x, ref y1);
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        y1[i_] = y1[i_] - y[i_];
                    }
                    v1 = 0.0;
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        v1 += y1[i_]*y1[i_];
                    }
                    v1 = v1/2;
                    network.weights[i] = wprev-h;
                    mlpbase.mlpprocess(ref network, ref x, ref y1);
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        y1[i_] = y1[i_] - y[i_];
                    }
                    v2 = 0.0;
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        v2 += y1[i_]*y1[i_];
                    }
                    v2 = v2/2;
                    network.weights[i] = wprev+h;
                    mlpbase.mlpprocess(ref network, ref x, ref y1);
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        y1[i_] = y1[i_] - y[i_];
                    }
                    v3 = 0.0;
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        v3 += y1[i_]*y1[i_];
                    }
                    v3 = v3/2;
                    network.weights[i] = wprev+2*h;
                    mlpbase.mlpprocess(ref network, ref x, ref y1);
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        y1[i_] = y1[i_] - y[i_];
                    }
                    v4 = 0.0;
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        v4 += y1[i_]*y1[i_];
                    }
                    v4 = v4/2;
                    network.weights[i] = wprev;
                    grad1[i] = (v1-8*v2+8*v3-v4)/(12*h);
                    if( (double)(Math.Abs(grad1[i]))>(double)(1.0E-3) )
                    {
                        err = err | (double)(Math.Abs((grad2[i]-grad1[i])/grad1[i]))>(double)(etol);
                    }
                    else
                    {
                        err = err | (double)(Math.Abs(grad2[i]-grad1[i]))>(double)(etol);
                    }
                }
                
                //
                // Test error/gradient calculation (natural).
                // Testing on non-random structure networks
                // (because NKind is representative only in that case).
                //
                xy = new double[0+1, nin+nout-1+1];
                for(i=0; i<=nin-1; i++)
                {
                    x[i] = 4*AP.Math.RandomReal()-2;
                }
                for(i_=0; i_<=nin-1;i_++)
                {
                    xy[0,i_] = x[i_];
                }
                if( mlpbase.mlpissoftmax(ref network) )
                {
                    for(i=0; i<=nout-1; i++)
                    {
                        y[i] = 0;
                    }
                    xy[0,nin] = AP.Math.RandomInteger(nout);
                    y[(int)Math.Round(xy[0,nin])] = 1;
                }
                else
                {
                    for(i=0; i<=nout-1; i++)
                    {
                        y[i] = 4*AP.Math.RandomReal()-2;
                    }
                    i1_ = (0) - (nin);
                    for(i_=nin; i_<=nin+nout-1;i_++)
                    {
                        xy[0,i_] = y[i_+i1_];
                    }
                }
                mlpbase.mlpgradn(ref network, ref x, ref y, ref e, ref grad2);
                mlpbase.mlpprocess(ref network, ref x, ref y2);
                v = 0;
                if( nkind!=1 )
                {
                    for(i=0; i<=nout-1; i++)
                    {
                        v = v+0.5*AP.Math.Sqr(y2[i]-y[i]);
                    }
                }
                else
                {
                    for(i=0; i<=nout-1; i++)
                    {
                        if( (double)(y[i])!=(double)(0) )
                        {
                            if( (double)(y2[i])==(double)(0) )
                            {
                                v = v+y[i]*Math.Log(AP.Math.MaxRealNumber);
                            }
                            else
                            {
                                v = v+y[i]*Math.Log(y[i]/y2[i]);
                            }
                        }
                    }
                }
                err = err | (double)(Math.Abs((v-e)/v))>(double)(etol);
                err = err | (double)(Math.Abs((mlpbase.mlperrorn(ref network, ref xy, 1)-v)/v))>(double)(etol);
                for(i=0; i<=wcount-1; i++)
                {
                    wprev = network.weights[i];
                    network.weights[i] = wprev+h;
                    mlpbase.mlpprocess(ref network, ref x, ref y2);
                    network.weights[i] = wprev-h;
                    mlpbase.mlpprocess(ref network, ref x, ref y1);
                    network.weights[i] = wprev;
                    v = 0;
                    if( nkind!=1 )
                    {
                        for(j=0; j<=nout-1; j++)
                        {
                            v = v+0.5*(AP.Math.Sqr(y2[j]-y[j])-AP.Math.Sqr(y1[j]-y[j]))/(2*h);
                        }
                    }
                    else
                    {
                        for(j=0; j<=nout-1; j++)
                        {
                            if( (double)(y[j])!=(double)(0) )
                            {
                                if( (double)(y2[j])==(double)(0) )
                                {
                                    v = v+y[j]*Math.Log(AP.Math.MaxRealNumber);
                                }
                                else
                                {
                                    v = v+y[j]*Math.Log(y[j]/y2[j]);
                                }
                                if( (double)(y1[j])==(double)(0) )
                                {
                                    v = v-y[j]*Math.Log(AP.Math.MaxRealNumber);
                                }
                                else
                                {
                                    v = v-y[j]*Math.Log(y[j]/y1[j]);
                                }
                            }
                        }
                        v = v/(2*h);
                    }
                    grad1[i] = v;
                    if( (double)(Math.Abs(grad1[i]))>(double)(1.0E-3) )
                    {
                        err = err | (double)(Math.Abs((grad2[i]-grad1[i])/grad1[i]))>(double)(etol);
                    }
                    else
                    {
                        err = err | (double)(Math.Abs(grad2[i]-grad1[i]))>(double)(etol);
                    }
                }
                
                //
                // Test gradient calculation: batch (least squares)
                //
                ssize = 1+AP.Math.RandomInteger(10);
                xy = new double[ssize-1+1, nin+nout-1+1];
                for(i=0; i<=wcount-1; i++)
                {
                    grad1[i] = 0;
                }
                e1 = 0;
                for(i=0; i<=ssize-1; i++)
                {
                    for(j=0; j<=nin-1; j++)
                    {
                        x1[j] = 4*AP.Math.RandomReal()-2;
                    }
                    for(i_=0; i_<=nin-1;i_++)
                    {
                        xy[i,i_] = x1[i_];
                    }
                    if( mlpbase.mlpissoftmax(ref network) )
                    {
                        for(j=0; j<=nout-1; j++)
                        {
                            y1[j] = 0;
                        }
                        xy[i,nin] = AP.Math.RandomInteger(nout);
                        y1[(int)Math.Round(xy[i,nin])] = 1;
                    }
                    else
                    {
                        for(j=0; j<=nout-1; j++)
                        {
                            y1[j] = 4*AP.Math.RandomReal()-2;
                        }
                        i1_ = (0) - (nin);
                        for(i_=nin; i_<=nin+nout-1;i_++)
                        {
                            xy[i,i_] = y1[i_+i1_];
                        }
                    }
                    mlpbase.mlpgrad(ref network, ref x1, ref y1, ref v, ref grad2);
                    e1 = e1+v;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        grad1[i_] = grad1[i_] + grad2[i_];
                    }
                }
                mlpbase.mlpgradbatch(ref network, ref xy, ssize, ref e2, ref grad2);
                err = err | (double)(Math.Abs(e1-e2)/e1)>(double)(0.01);
                for(i=0; i<=wcount-1; i++)
                {
                    if( (double)(grad1[i])!=(double)(0) )
                    {
                        err = err | (double)(Math.Abs((grad2[i]-grad1[i])/grad1[i]))>(double)(etol);
                    }
                    else
                    {
                        err = err | (double)(grad2[i])!=(double)(grad1[i]);
                    }
                }
                
                //
                // Test gradient calculation: batch (natural error func)
                //
                ssize = 1+AP.Math.RandomInteger(10);
                xy = new double[ssize-1+1, nin+nout-1+1];
                for(i=0; i<=wcount-1; i++)
                {
                    grad1[i] = 0;
                }
                e1 = 0;
                for(i=0; i<=ssize-1; i++)
                {
                    for(j=0; j<=nin-1; j++)
                    {
                        x1[j] = 4*AP.Math.RandomReal()-2;
                    }
                    for(i_=0; i_<=nin-1;i_++)
                    {
                        xy[i,i_] = x1[i_];
                    }
                    if( mlpbase.mlpissoftmax(ref network) )
                    {
                        for(j=0; j<=nout-1; j++)
                        {
                            y1[j] = 0;
                        }
                        xy[i,nin] = AP.Math.RandomInteger(nout);
                        y1[(int)Math.Round(xy[i,nin])] = 1;
                    }
                    else
                    {
                        for(j=0; j<=nout-1; j++)
                        {
                            y1[j] = 4*AP.Math.RandomReal()-2;
                        }
                        i1_ = (0) - (nin);
                        for(i_=nin; i_<=nin+nout-1;i_++)
                        {
                            xy[i,i_] = y1[i_+i1_];
                        }
                    }
                    mlpbase.mlpgradn(ref network, ref x1, ref y1, ref v, ref grad2);
                    e1 = e1+v;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        grad1[i_] = grad1[i_] + grad2[i_];
                    }
                }
                mlpbase.mlpgradnbatch(ref network, ref xy, ssize, ref e2, ref grad2);
                err = err | (double)(Math.Abs(e1-e2)/e1)>(double)(etol);
                for(i=0; i<=wcount-1; i++)
                {
                    if( (double)(grad1[i])!=(double)(0) )
                    {
                        err = err | (double)(Math.Abs((grad2[i]-grad1[i])/grad1[i]))>(double)(etol);
                    }
                    else
                    {
                        err = err | (double)(grad2[i])!=(double)(grad1[i]);
                    }
                }
            }
        }


        /*************************************************************************
        Hessian functions test
        *************************************************************************/
        private static void testhessian(int nkind,
            int nin,
            int nhid1,
            int nhid2,
            int nout,
            int passcount,
            ref bool err)
        {
            mlpbase.multilayerperceptron network = new mlpbase.multilayerperceptron();
            mlpbase.multilayerperceptron network2 = new mlpbase.multilayerperceptron();
            int hkind = 0;
            int n1 = 0;
            int n2 = 0;
            int wcount = 0;
            bool zeronet = new bool();
            double h = 0;
            double etol = 0;
            int pass = 0;
            int i = 0;
            int j = 0;
            bool allsame = new bool();
            int ilen = 0;
            int rlen = 0;
            int ssize = 0;
            double a1 = 0;
            double a2 = 0;
            double[,] xy = new double[0,0];
            double[,] h1 = new double[0,0];
            double[,] h2 = new double[0,0];
            double[] grad1 = new double[0];
            double[] grad2 = new double[0];
            double[] grad3 = new double[0];
            double[] x = new double[0];
            double[] y = new double[0];
            double[] x1 = new double[0];
            double[] x2 = new double[0];
            double[] y1 = new double[0];
            double[] y2 = new double[0];
            int[] ia = new int[0];
            double[] ra = new double[0];
            double v = 0;
            double e = 0;
            double e1 = 0;
            double e2 = 0;
            double v1 = 0;
            double v2 = 0;
            double v3 = 0;
            double v4 = 0;
            double wprev = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(passcount>=2, "PassCount<2!");
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
            createnetwork(ref network, nkind, a1, a2, nin, nhid1, nhid2, nout);
            mlpbase.mlpproperties(ref network, ref n1, ref n2, ref wcount);
            h = 0.0001;
            etol = 0.05;
            
            //
            // Initialize
            //
            x = new double[nin-1+1];
            x1 = new double[nin-1+1];
            x2 = new double[nin-1+1];
            y = new double[nout-1+1];
            y1 = new double[nout-1+1];
            y2 = new double[nout-1+1];
            grad1 = new double[wcount-1+1];
            grad2 = new double[wcount-1+1];
            grad3 = new double[wcount-1+1];
            h1 = new double[wcount-1+1, wcount-1+1];
            h2 = new double[wcount-1+1, wcount-1+1];
            
            //
            // Process
            //
            for(pass=1; pass<=passcount; pass++)
            {
                mlpbase.mlprandomizefull(ref network);
                
                //
                // Test hessian calculation .
                // E1 contains total error (calculated using MLPGrad/MLPGradN)
                // Grad1 contains total gradient (calculated using MLPGrad/MLPGradN)
                // H1 contains Hessian calculated using differences of gradients
                //
                // E2, Grad2 and H2 contains corresponing values calculated using MLPHessianBatch/MLPHessianNBatch
                //
                for(hkind=0; hkind<=1; hkind++)
                {
                    ssize = 1+AP.Math.RandomInteger(10);
                    xy = new double[ssize-1+1, nin+nout-1+1];
                    for(i=0; i<=wcount-1; i++)
                    {
                        grad1[i] = 0;
                    }
                    for(i=0; i<=wcount-1; i++)
                    {
                        for(j=0; j<=wcount-1; j++)
                        {
                            h1[i,j] = 0;
                        }
                    }
                    e1 = 0;
                    for(i=0; i<=ssize-1; i++)
                    {
                        
                        //
                        // X, Y
                        //
                        for(j=0; j<=nin-1; j++)
                        {
                            x1[j] = 4*AP.Math.RandomReal()-2;
                        }
                        for(i_=0; i_<=nin-1;i_++)
                        {
                            xy[i,i_] = x1[i_];
                        }
                        if( mlpbase.mlpissoftmax(ref network) )
                        {
                            for(j=0; j<=nout-1; j++)
                            {
                                y1[j] = 0;
                            }
                            xy[i,nin] = AP.Math.RandomInteger(nout);
                            y1[(int)Math.Round(xy[i,nin])] = 1;
                        }
                        else
                        {
                            for(j=0; j<=nout-1; j++)
                            {
                                y1[j] = 4*AP.Math.RandomReal()-2;
                            }
                            i1_ = (0) - (nin);
                            for(i_=nin; i_<=nin+nout-1;i_++)
                            {
                                xy[i,i_] = y1[i_+i1_];
                            }
                        }
                        
                        //
                        // E1, Grad1
                        //
                        if( hkind==0 )
                        {
                            mlpbase.mlpgrad(ref network, ref x1, ref y1, ref v, ref grad2);
                        }
                        else
                        {
                            mlpbase.mlpgradn(ref network, ref x1, ref y1, ref v, ref grad2);
                        }
                        e1 = e1+v;
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            grad1[i_] = grad1[i_] + grad2[i_];
                        }
                        
                        //
                        // H1
                        //
                        for(j=0; j<=wcount-1; j++)
                        {
                            wprev = network.weights[j];
                            network.weights[j] = wprev-2*h;
                            if( hkind==0 )
                            {
                                mlpbase.mlpgrad(ref network, ref x1, ref y1, ref v, ref grad2);
                            }
                            else
                            {
                                mlpbase.mlpgradn(ref network, ref x1, ref y1, ref v, ref grad2);
                            }
                            network.weights[j] = wprev-h;
                            if( hkind==0 )
                            {
                                mlpbase.mlpgrad(ref network, ref x1, ref y1, ref v, ref grad3);
                            }
                            else
                            {
                                mlpbase.mlpgradn(ref network, ref x1, ref y1, ref v, ref grad3);
                            }
                            for(i_=0; i_<=wcount-1;i_++)
                            {
                                grad2[i_] = grad2[i_] - 8*grad3[i_];
                            }
                            network.weights[j] = wprev+h;
                            if( hkind==0 )
                            {
                                mlpbase.mlpgrad(ref network, ref x1, ref y1, ref v, ref grad3);
                            }
                            else
                            {
                                mlpbase.mlpgradn(ref network, ref x1, ref y1, ref v, ref grad3);
                            }
                            for(i_=0; i_<=wcount-1;i_++)
                            {
                                grad2[i_] = grad2[i_] + 8*grad3[i_];
                            }
                            network.weights[j] = wprev+2*h;
                            if( hkind==0 )
                            {
                                mlpbase.mlpgrad(ref network, ref x1, ref y1, ref v, ref grad3);
                            }
                            else
                            {
                                mlpbase.mlpgradn(ref network, ref x1, ref y1, ref v, ref grad3);
                            }
                            for(i_=0; i_<=wcount-1;i_++)
                            {
                                grad2[i_] = grad2[i_] - grad3[i_];
                            }
                            v = 1/(12*h);
                            for(i_=0; i_<=wcount-1;i_++)
                            {
                                h1[j,i_] = h1[j,i_] + v*grad2[i_];
                            }
                            network.weights[j] = wprev;
                        }
                    }
                    if( hkind==0 )
                    {
                        mlpbase.mlphessianbatch(ref network, ref xy, ssize, ref e2, ref grad2, ref h2);
                    }
                    else
                    {
                        mlpbase.mlphessiannbatch(ref network, ref xy, ssize, ref e2, ref grad2, ref h2);
                    }
                    err = err | (double)(Math.Abs(e1-e2)/e1)>(double)(etol);
                    for(i=0; i<=wcount-1; i++)
                    {
                        if( (double)(Math.Abs(grad1[i]))>(double)(1.0E-2) )
                        {
                            err = err | (double)(Math.Abs((grad2[i]-grad1[i])/grad1[i]))>(double)(etol);
                        }
                        else
                        {
                            err = err | (double)(Math.Abs(grad2[i]-grad1[i]))>(double)(etol);
                        }
                    }
                    for(i=0; i<=wcount-1; i++)
                    {
                        for(j=0; j<=wcount-1; j++)
                        {
                            if( (double)(Math.Abs(h1[i,j]))>(double)(5.0E-2) )
                            {
                                err = err | (double)(Math.Abs((h1[i,j]-h2[i,j])/h1[i,j]))>(double)(etol);
                            }
                            else
                            {
                                err = err | (double)(Math.Abs(h2[i,j]-h1[i,j]))>(double)(etol);
                            }
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testmlpunit_test_silent()
        {
            bool result = new bool();

            result = testmlp(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testmlpunit_test()
        {
            bool result = new bool();

            result = testmlp(false);
            return result;
        }
    }
}
