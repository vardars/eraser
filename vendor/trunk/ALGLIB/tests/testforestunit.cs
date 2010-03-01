
using System;

namespace alglib
{
    public class testforestunit
    {
        public static bool testforest(bool silent)
        {
            bool result = new bool();
            int ncmax = 0;
            int nvmax = 0;
            int passcount = 0;
            int nvars = 0;
            int nclasses = 0;
            bool waserrors = new bool();
            bool basicerrors = new bool();
            bool procerrors = new bool();
            int i = 0;
            int j = 0;

            
            //
            // Primary settings
            //
            nvmax = 4;
            ncmax = 3;
            passcount = 10;
            basicerrors = false;
            procerrors = false;
            waserrors = false;
            
            //
            // Tests
            //
            testprocessing(ref procerrors);
            for(nvars=1; nvars<=nvmax; nvars++)
            {
                for(nclasses=1; nclasses<=ncmax; nclasses++)
                {
                    basictest1(nvars, nclasses, passcount, ref basicerrors);
                }
            }
            basictest2(ref basicerrors);
            basictest3(ref basicerrors);
            basictest4(ref basicerrors);
            basictest5(ref basicerrors);
            
            //
            // Final report
            //
            waserrors = basicerrors | procerrors;
            if( !silent )
            {
                System.Console.Write("RANDOM FOREST TEST");
                System.Console.WriteLine();
                System.Console.Write("TOTAL RESULTS:                           ");
                if( !waserrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("* PROCESSING FUNCTIONS:                  ");
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
                System.Console.Write("* BASIC TESTS:                           ");
                if( !basicerrors )
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
        Processing functions test
        *************************************************************************/
        private static void testprocessing(ref bool err)
        {
            int nvars = 0;
            int nclasses = 0;
            int nsample = 0;
            int ntrees = 0;
            int nfeatures = 0;
            int flags = 0;
            dforest.decisionforest df1 = new dforest.decisionforest();
            dforest.decisionforest df2 = new dforest.decisionforest();
            int npoints = 0;
            double[,] xy = new double[0,0];
            int pass = 0;
            int passcount = 0;
            int i = 0;
            int j = 0;
            bool allsame = new bool();
            int rlen = 0;
            int info = 0;
            dforest.dfreport rep = new dforest.dfreport();
            double[] x1 = new double[0];
            double[] x2 = new double[0];
            double[] y1 = new double[0];
            double[] y2 = new double[0];
            double[] ra = new double[0];
            double[] ra2 = new double[0];
            double v = 0;

            passcount = 100;
            
            //
            // Main cycle
            //
            for(pass=1; pass<=passcount; pass++)
            {
                
                //
                // initialize parameters
                //
                nvars = 1+AP.Math.RandomInteger(5);
                nclasses = 1+AP.Math.RandomInteger(3);
                ntrees = 1+AP.Math.RandomInteger(4);
                nfeatures = 1+AP.Math.RandomInteger(nvars);
                flags = 0;
                if( (double)(AP.Math.RandomReal())>(double)(0.5) )
                {
                    flags = flags+2;
                }
                
                //
                // Initialize arrays and data
                //
                npoints = 10+AP.Math.RandomInteger(50);
                nsample = Math.Max(10, AP.Math.RandomInteger(npoints));
                x1 = new double[nvars-1+1];
                x2 = new double[nvars-1+1];
                y1 = new double[nclasses-1+1];
                y2 = new double[nclasses-1+1];
                xy = new double[npoints-1+1, nvars+1];
                for(i=0; i<=npoints-1; i++)
                {
                    for(j=0; j<=nvars-1; j++)
                    {
                        if( j%2==0 )
                        {
                            xy[i,j] = 2*AP.Math.RandomReal()-1;
                        }
                        else
                        {
                            xy[i,j] = AP.Math.RandomInteger(2);
                        }
                    }
                    if( nclasses==1 )
                    {
                        xy[i,nvars] = 2*AP.Math.RandomReal()-1;
                    }
                    else
                    {
                        xy[i,nvars] = AP.Math.RandomInteger(nclasses);
                    }
                }
                
                //
                // create forest
                //
                dforest.dfbuildinternal(ref xy, npoints, nvars, nclasses, ntrees, nsample, nfeatures, flags, ref info, ref df1, ref rep);
                if( info<=0 )
                {
                    err = true;
                    return;
                }
                
                //
                // Same inputs leads to same outputs
                //
                for(i=0; i<=nvars-1; i++)
                {
                    x1[i] = 2*AP.Math.RandomReal()-1;
                    x2[i] = x1[i];
                }
                for(i=0; i<=nclasses-1; i++)
                {
                    y1[i] = 2*AP.Math.RandomReal()-1;
                    y2[i] = 2*AP.Math.RandomReal()-1;
                }
                dforest.dfprocess(ref df1, ref x1, ref y1);
                dforest.dfprocess(ref df1, ref x2, ref y2);
                allsame = true;
                for(i=0; i<=nclasses-1; i++)
                {
                    allsame = allsame & (double)(y1[i])==(double)(y2[i]);
                }
                err = err | !allsame;
                
                //
                // Same inputs on original forest leads to same outputs
                // on copy created using DFCopy
                //
                unsetdf(ref df2);
                dforest.dfcopy(ref df1, ref df2);
                for(i=0; i<=nvars-1; i++)
                {
                    x1[i] = 2*AP.Math.RandomReal()-1;
                    x2[i] = x1[i];
                }
                for(i=0; i<=nclasses-1; i++)
                {
                    y1[i] = 2*AP.Math.RandomReal()-1;
                    y2[i] = 2*AP.Math.RandomReal()-1;
                }
                dforest.dfprocess(ref df1, ref x1, ref y1);
                dforest.dfprocess(ref df2, ref x2, ref y2);
                allsame = true;
                for(i=0; i<=nclasses-1; i++)
                {
                    allsame = allsame & (double)(y1[i])==(double)(y2[i]);
                }
                err = err | !allsame;
                
                //
                // Same inputs on original forest leads to same outputs
                // on copy created using DFSerialize
                //
                unsetdf(ref df2);
                ra = new double[0+1];
                ra[0] = 0;
                rlen = 0;
                dforest.dfserialize(ref df1, ref ra, ref rlen);
                ra2 = new double[rlen-1+1];
                for(i=0; i<=rlen-1; i++)
                {
                    ra2[i] = ra[i];
                }
                dforest.dfunserialize(ref ra2, ref df2);
                for(i=0; i<=nvars-1; i++)
                {
                    x1[i] = 2*AP.Math.RandomReal()-1;
                    x2[i] = x1[i];
                }
                for(i=0; i<=nclasses-1; i++)
                {
                    y1[i] = 2*AP.Math.RandomReal()-1;
                    y2[i] = 2*AP.Math.RandomReal()-1;
                }
                dforest.dfprocess(ref df1, ref x1, ref y1);
                dforest.dfprocess(ref df2, ref x2, ref y2);
                allsame = true;
                for(i=0; i<=nclasses-1; i++)
                {
                    allsame = allsame & (double)(y1[i])==(double)(y2[i]);
                }
                err = err | !allsame;
                
                //
                // Normalization properties
                //
                if( nclasses>1 )
                {
                    for(i=0; i<=nvars-1; i++)
                    {
                        x1[i] = 2*AP.Math.RandomReal()-1;
                    }
                    dforest.dfprocess(ref df1, ref x1, ref y1);
                    v = 0;
                    for(i=0; i<=nclasses-1; i++)
                    {
                        v = v+y1[i];
                        err = err | (double)(y1[i])<(double)(0);
                    }
                    err = err | (double)(Math.Abs(v-1))>(double)(1000*AP.Math.MachineEpsilon);
                }
            }
        }


        /*************************************************************************
        Basic test:  one-tree forest built using full sample must remember all the
        training cases
        *************************************************************************/
        private static void basictest1(int nvars,
            int nclasses,
            int passcount,
            ref bool err)
        {
            int pass = 0;
            double[,] xy = new double[0,0];
            int npoints = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            double s = 0;
            int info = 0;
            dforest.decisionforest df = new dforest.decisionforest();
            double[] x = new double[0];
            double[] y = new double[0];
            dforest.dfreport rep = new dforest.dfreport();
            bool hassame = new bool();
            int i_ = 0;

            if( nclasses==1 )
            {
                
                //
                // only classification tasks
                //
                return;
            }
            for(pass=1; pass<=passcount; pass++)
            {
                
                //
                // select number of points
                //
                if( pass<=3 & passcount>3 )
                {
                    npoints = pass;
                }
                else
                {
                    npoints = 100+AP.Math.RandomInteger(100);
                }
                
                //
                // Prepare task
                //
                xy = new double[npoints-1+1, nvars+1];
                x = new double[nvars-1+1];
                y = new double[nclasses-1+1];
                for(i=0; i<=npoints-1; i++)
                {
                    for(j=0; j<=nvars-1; j++)
                    {
                        xy[i,j] = 2*AP.Math.RandomReal()-1;
                    }
                    xy[i,nvars] = AP.Math.RandomInteger(nclasses);
                }
                
                //
                // Test
                //
                dforest.dfbuildinternal(ref xy, npoints, nvars, nclasses, 1, npoints, 1, 1, ref info, ref df, ref rep);
                if( info<=0 )
                {
                    err = true;
                    return;
                }
                for(i=0; i<=npoints-1; i++)
                {
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        x[i_] = xy[i,i_];
                    }
                    dforest.dfprocess(ref df, ref x, ref y);
                    s = 0;
                    for(j=0; j<=nclasses-1; j++)
                    {
                        if( (double)(y[j])<(double)(0) )
                        {
                            err = true;
                            return;
                        }
                        s = s+y[j];
                    }
                    if( (double)(Math.Abs(s-1))>(double)(1000*AP.Math.MachineEpsilon) )
                    {
                        err = true;
                        return;
                    }
                    if( (double)(Math.Abs(y[(int)Math.Round(xy[i,nvars])]-1))>(double)(1000*AP.Math.MachineEpsilon) )
                    {
                        
                        //
                        // not an error if there exists such K,J that XY[K,J]=XY[I,J]
                        // (may be we just can't distinguish two tied values).
                        //
                        // definitely error otherwise.
                        //
                        hassame = false;
                        for(k=0; k<=npoints-1; k++)
                        {
                            if( k!=i )
                            {
                                for(j=0; j<=nvars-1; j++)
                                {
                                    if( (double)(xy[k,j])==(double)(xy[i,j]) )
                                    {
                                        hassame = true;
                                    }
                                }
                            }
                        }
                        if( !hassame )
                        {
                            err = true;
                            return;
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Basic test:  tests generalization ability on a simple noisy classification
        task:
        * 0<x<1 - P(class=0)=1
        * 1<x<2 - P(class=0)=2-x
        * 2<x<3 - P(class=0)=0
        *************************************************************************/
        private static void basictest2(ref bool err)
        {
            int pass = 0;
            int passcount = 0;
            double[,] xy = new double[0,0];
            int npoints = 0;
            int ntrees = 0;
            int i = 0;
            int j = 0;
            double s = 0;
            int info = 0;
            dforest.decisionforest df = new dforest.decisionforest();
            double[] x = new double[0];
            double[] y = new double[0];
            dforest.dfreport rep = new dforest.dfreport();
            bool hassame = new bool();

            passcount = 1;
            for(pass=1; pass<=passcount; pass++)
            {
                
                //
                // select npoints and ntrees
                //
                npoints = 3000;
                ntrees = 50;
                
                //
                // Prepare task
                //
                xy = new double[npoints-1+1, 1+1];
                x = new double[0+1];
                y = new double[1+1];
                for(i=0; i<=npoints-1; i++)
                {
                    xy[i,0] = 3*AP.Math.RandomReal();
                    if( (double)(xy[i,0])<=(double)(1) )
                    {
                        xy[i,1] = 0;
                    }
                    else
                    {
                        if( (double)(xy[i,0])<=(double)(2) )
                        {
                            if( (double)(AP.Math.RandomReal())<(double)(xy[i,0]-1) )
                            {
                                xy[i,1] = 1;
                            }
                            else
                            {
                                xy[i,1] = 0;
                            }
                        }
                        else
                        {
                            xy[i,1] = 1;
                        }
                    }
                }
                
                //
                // Test
                //
                dforest.dfbuildinternal(ref xy, npoints, 1, 2, ntrees, (int)Math.Round(0.05*npoints), 1, 0, ref info, ref df, ref rep);
                if( info<=0 )
                {
                    err = true;
                    return;
                }
                x[0] = 0.0;
                while( (double)(x[0])<=(double)(3.0) )
                {
                    dforest.dfprocess(ref df, ref x, ref y);
                    
                    //
                    // Test for basic properties
                    //
                    s = 0;
                    for(j=0; j<=1; j++)
                    {
                        if( (double)(y[j])<(double)(0) )
                        {
                            err = true;
                            return;
                        }
                        s = s+y[j];
                    }
                    if( (double)(Math.Abs(s-1))>(double)(1000*AP.Math.MachineEpsilon) )
                    {
                        err = true;
                        return;
                    }
                    
                    //
                    // test for good correlation with results
                    //
                    if( (double)(x[0])<(double)(1) )
                    {
                        err = err | (double)(y[0])<(double)(0.8);
                    }
                    if( (double)(x[0])>=(double)(1) & (double)(x[0])<=(double)(2) )
                    {
                        err = err | (double)(Math.Abs(y[1]-(x[0]-1)))>(double)(0.5);
                    }
                    if( (double)(x[0])>(double)(2) )
                    {
                        err = err | (double)(y[1])<(double)(0.8);
                    }
                    x[0] = x[0]+0.01;
                }
            }
        }


        /*************************************************************************
        Basic test:  tests  generalization ability on a simple classification task
        (no noise):
        * |x|<1, |y|<1
        * x^2+y^2<=0.25 - P(class=0)=1
        * x^2+y^2>0.25  - P(class=0)=0
        *************************************************************************/
        private static void basictest3(ref bool err)
        {
            int pass = 0;
            int passcount = 0;
            double[,] xy = new double[0,0];
            int npoints = 0;
            int ntrees = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            double s = 0;
            int info = 0;
            dforest.decisionforest df = new dforest.decisionforest();
            double[] x = new double[0];
            double[] y = new double[0];
            dforest.dfreport rep = new dforest.dfreport();
            int testgridsize = 0;
            double r = 0;

            passcount = 1;
            testgridsize = 50;
            for(pass=1; pass<=passcount; pass++)
            {
                
                //
                // select npoints and ntrees
                //
                npoints = 2000;
                ntrees = 100;
                
                //
                // Prepare task
                //
                xy = new double[npoints-1+1, 2+1];
                x = new double[1+1];
                y = new double[1+1];
                for(i=0; i<=npoints-1; i++)
                {
                    xy[i,0] = 2*AP.Math.RandomReal()-1;
                    xy[i,1] = 2*AP.Math.RandomReal()-1;
                    if( (double)(AP.Math.Sqr(xy[i,0])+AP.Math.Sqr(xy[i,1]))<=(double)(0.25) )
                    {
                        xy[i,2] = 0;
                    }
                    else
                    {
                        xy[i,2] = 1;
                    }
                }
                
                //
                // Test
                //
                dforest.dfbuildinternal(ref xy, npoints, 2, 2, ntrees, (int)Math.Round(0.1*npoints), 1, 0, ref info, ref df, ref rep);
                if( info<=0 )
                {
                    err = true;
                    return;
                }
                for(i=-(testgridsize/2); i<=testgridsize/2; i++)
                {
                    for(j=-(testgridsize/2); j<=testgridsize/2; j++)
                    {
                        x[0] = (double)(i)/((double)(testgridsize/2));
                        x[1] = (double)(j)/((double)(testgridsize/2));
                        dforest.dfprocess(ref df, ref x, ref y);
                        
                        //
                        // Test for basic properties
                        //
                        s = 0;
                        for(k=0; k<=1; k++)
                        {
                            if( (double)(y[k])<(double)(0) )
                            {
                                err = true;
                                return;
                            }
                            s = s+y[k];
                        }
                        if( (double)(Math.Abs(s-1))>(double)(1000*AP.Math.MachineEpsilon) )
                        {
                            err = true;
                            return;
                        }
                        
                        //
                        // test for good correlation with results
                        //
                        r = Math.Sqrt(AP.Math.Sqr(x[0])+AP.Math.Sqr(x[1]));
                        if( (double)(r)<(double)(0.5*0.5) )
                        {
                            err = err | (double)(y[0])<(double)(0.6);
                        }
                        if( (double)(r)>(double)(0.5*1.5) )
                        {
                            err = err | (double)(y[1])<(double)(0.6);
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Basic test: simple regression task without noise:
        * |x|<1, |y|<1
        * F(x,y) = x^2+y
        *************************************************************************/
        private static void basictest4(ref bool err)
        {
            int pass = 0;
            int passcount = 0;
            double[,] xy = new double[0,0];
            int npoints = 0;
            int ntrees = 0;
            int ns = 0;
            int strongc = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            double s = 0;
            int info = 0;
            dforest.decisionforest df = new dforest.decisionforest();
            dforest.decisionforest df2 = new dforest.decisionforest();
            double[] x = new double[0];
            double[] y = new double[0];
            dforest.dfreport rep = new dforest.dfreport();
            dforest.dfreport rep2 = new dforest.dfreport();
            int testgridsize = 0;
            double maxerr = 0;
            double maxerr2 = 0;
            double avgerr = 0;
            double avgerr2 = 0;
            int cnt = 0;
            double ey = 0;

            passcount = 1;
            testgridsize = 50;
            for(pass=1; pass<=passcount; pass++)
            {
                
                //
                // select npoints and ntrees
                //
                npoints = 5000;
                ntrees = 100;
                ns = (int)Math.Round(0.1*npoints);
                strongc = 1;
                
                //
                // Prepare task
                //
                xy = new double[npoints-1+1, 2+1];
                x = new double[1+1];
                y = new double[0+1];
                for(i=0; i<=npoints-1; i++)
                {
                    xy[i,0] = 2*AP.Math.RandomReal()-1;
                    xy[i,1] = 2*AP.Math.RandomReal()-1;
                    xy[i,2] = AP.Math.Sqr(xy[i,0])+xy[i,1];
                }
                
                //
                // Test
                //
                dforest.dfbuildinternal(ref xy, npoints, 2, 1, ntrees, ns, 1, 0, ref info, ref df, ref rep);
                if( info<=0 )
                {
                    err = true;
                    return;
                }
                dforest.dfbuildinternal(ref xy, npoints, 2, 1, ntrees, ns, 1, strongc, ref info, ref df2, ref rep2);
                if( info<=0 )
                {
                    err = true;
                    return;
                }
                maxerr = 0;
                maxerr2 = 0;
                avgerr = 0;
                avgerr2 = 0;
                cnt = 0;
                for(i=(int)Math.Round(-(0.7*testgridsize/2)); i<=(int)Math.Round(0.7*testgridsize/2); i++)
                {
                    for(j=(int)Math.Round(-(0.7*testgridsize/2)); j<=(int)Math.Round(0.7*testgridsize/2); j++)
                    {
                        x[0] = (double)(i)/((double)(testgridsize/2));
                        x[1] = (double)(j)/((double)(testgridsize/2));
                        ey = AP.Math.Sqr(x[0])+x[1];
                        dforest.dfprocess(ref df, ref x, ref y);
                        maxerr = Math.Max(maxerr, Math.Abs(y[0]-ey));
                        avgerr = avgerr+Math.Abs(y[0]-ey);
                        dforest.dfprocess(ref df2, ref x, ref y);
                        maxerr2 = Math.Max(maxerr2, Math.Abs(y[0]-ey));
                        avgerr2 = avgerr2+Math.Abs(y[0]-ey);
                        cnt = cnt+1;
                    }
                }
                avgerr = avgerr/cnt;
                avgerr2 = avgerr2/cnt;
                err = err | (double)(maxerr)>(double)(0.2);
                err = err | (double)(maxerr2)>(double)(0.2);
                err = err | (double)(avgerr)>(double)(0.1);
                err = err | (double)(avgerr2)>(double)(0.1);
            }
        }


        /*************************************************************************
        Basic test: extended variable selection leads to better results.

        Next task CAN be solved without EVS but it is very unlikely. With EVS
        it can be easily and exactly solved.

        Task matrix:
            1 0 0 0 ... 0   0
            0 1 0 0 ... 0   1
            0 0 1 0 ... 0   2
            0 0 0 1 ... 0   3
            0 0 0 0 ... 1   N-1
        *************************************************************************/
        private static void basictest5(ref bool err)
        {
            double[,] xy = new double[0,0];
            int nvars = 0;
            int npoints = 0;
            int nfeatures = 0;
            int nsample = 0;
            int ntrees = 0;
            int evs = 0;
            int i = 0;
            int j = 0;
            bool eflag = new bool();
            int info = 0;
            dforest.decisionforest df = new dforest.decisionforest();
            double[] x = new double[0];
            double[] y = new double[0];
            dforest.dfreport rep = new dforest.dfreport();
            int i_ = 0;

            
            //
            // select npoints and ntrees
            //
            npoints = 50;
            nvars = npoints;
            ntrees = 1;
            nsample = npoints;
            evs = 2;
            nfeatures = 1;
            
            //
            // Prepare task
            //
            xy = new double[npoints-1+1, nvars+1];
            x = new double[nvars-1+1];
            y = new double[0+1];
            for(i=0; i<=npoints-1; i++)
            {
                for(j=0; j<=nvars-1; j++)
                {
                    xy[i,j] = 0;
                }
                xy[i,i] = 1;
                xy[i,nvars] = i;
            }
            
            //
            // Without EVS
            //
            dforest.dfbuildinternal(ref xy, npoints, nvars, 1, ntrees, nsample, nfeatures, 0, ref info, ref df, ref rep);
            if( info<=0 )
            {
                err = true;
                return;
            }
            eflag = false;
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    x[i_] = xy[i,i_];
                }
                dforest.dfprocess(ref df, ref x, ref y);
                if( (double)(Math.Abs(y[0]-xy[i,nvars]))>(double)(1000*AP.Math.MachineEpsilon) )
                {
                    eflag = true;
                }
            }
            if( !eflag )
            {
                err = true;
                return;
            }
            
            //
            // With EVS
            //
            dforest.dfbuildinternal(ref xy, npoints, nvars, 1, ntrees, nsample, nfeatures, evs, ref info, ref df, ref rep);
            if( info<=0 )
            {
                err = true;
                return;
            }
            eflag = false;
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    x[i_] = xy[i,i_];
                }
                dforest.dfprocess(ref df, ref x, ref y);
                if( (double)(Math.Abs(y[0]-xy[i,nvars]))>(double)(1000*AP.Math.MachineEpsilon) )
                {
                    eflag = true;
                }
            }
            if( eflag )
            {
                err = true;
                return;
            }
        }


        /*************************************************************************
        Random normal number
        *************************************************************************/
        private static double rnormal()
        {
            double result = 0;
            double u = 0;
            double v = 0;
            double s = 0;
            double x1 = 0;
            double x2 = 0;

            while( true )
            {
                u = 2*AP.Math.RandomReal()-1;
                v = 2*AP.Math.RandomReal()-1;
                s = AP.Math.Sqr(u)+AP.Math.Sqr(v);
                if( (double)(s)>(double)(0) & (double)(s)<(double)(1) )
                {
                    s = Math.Sqrt(-(2*Math.Log(s)/s));
                    x1 = u*s;
                    x2 = v*s;
                    break;
                }
            }
            result = x1;
            return result;
        }


        /*************************************************************************
        Random point from sphere
        *************************************************************************/
        private static double rsphere(ref double[,] xy,
            int n,
            int i)
        {
            double result = 0;
            int j = 0;
            double v = 0;
            int i_ = 0;

            for(j=0; j<=n-1; j++)
            {
                xy[i,j] = rnormal();
            }
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += xy[i,i_]*xy[i,i_];
            }
            v = AP.Math.RandomReal()/Math.Sqrt(v);
            for(i_=0; i_<=n-1;i_++)
            {
                xy[i,i_] = v*xy[i,i_];
            }
            return result;
        }


        /*************************************************************************
        Unsets DF
        *************************************************************************/
        private static void unsetdf(ref dforest.decisionforest df)
        {
            double[,] xy = new double[0,0];
            int info = 0;
            dforest.dfreport rep = new dforest.dfreport();

            xy = new double[0+1, 1+1];
            xy[0,0] = 0;
            xy[0,1] = 0;
            dforest.dfbuildinternal(ref xy, 1, 1, 1, 1, 1, 1, 0, ref info, ref df, ref rep);
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testforestunit_test_silent()
        {
            bool result = new bool();

            result = testforest(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testforestunit_test()
        {
            bool result = new bool();

            result = testforest(false);
            return result;
        }
    }
}
