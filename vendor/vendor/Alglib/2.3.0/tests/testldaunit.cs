
using System;

namespace alglib
{
    public class testldaunit
    {
        public static bool testlda(bool silent)
        {
            bool result = new bool();
            int maxnf = 0;
            int maxns = 0;
            int maxnc = 0;
            int passcount = 0;
            bool ldanerrors = new bool();
            bool lda1errors = new bool();
            bool waserrors = new bool();
            int nf = 0;
            int nc = 0;
            int ns = 0;
            int i = 0;
            int info = 0;
            int pass = 0;
            int axis = 0;
            double[,] xy = new double[0,0];
            double[,] wn = new double[0,0];
            double[] w1 = new double[0];

            
            //
            // Primary settings
            //
            maxnf = 10;
            maxns = 1000;
            maxnc = 5;
            passcount = 1;
            waserrors = false;
            ldanerrors = false;
            lda1errors = false;
            
            //
            // General tests
            //
            for(nf=1; nf<=maxnf; nf++)
            {
                for(nc=2; nc<=maxnc; nc++)
                {
                    for(pass=1; pass<=passcount; pass++)
                    {
                        
                        //
                        // Simple test for LDA-N/LDA-1
                        //
                        axis = AP.Math.RandomInteger(nf);
                        ns = maxns/2+AP.Math.RandomInteger(maxns/2);
                        gensimpleset(nf, nc, ns, axis, ref xy);
                        lda.fisherldan(ref xy, ns, nf, nc, ref info, ref wn);
                        if( info!=1 )
                        {
                            ldanerrors = true;
                            continue;
                        }
                        ldanerrors = ldanerrors | !testwn(ref xy, ref wn, ns, nf, nc, 0);
                        ldanerrors = ldanerrors | (double)(Math.Abs(wn[axis,0]))<=(double)(0.75);
                        lda.fisherlda(ref xy, ns, nf, nc, ref info, ref w1);
                        for(i=0; i<=nf-1; i++)
                        {
                            lda1errors = lda1errors | (double)(w1[i])!=(double)(wn[i,0]);
                        }
                        
                        //
                        // Degenerate test for LDA-N
                        //
                        if( nf>=3 )
                        {
                            ns = maxns/2+AP.Math.RandomInteger(maxns/2);
                            
                            //
                            // there are two duplicate features,
                            // axis is oriented along non-duplicate feature
                            //
                            axis = AP.Math.RandomInteger(nf-2);
                            gendeg1set(nf, nc, ns, axis, ref xy);
                            lda.fisherldan(ref xy, ns, nf, nc, ref info, ref wn);
                            if( info!=2 )
                            {
                                ldanerrors = true;
                                continue;
                            }
                            ldanerrors = ldanerrors | (double)(wn[axis,0])<=(double)(0.75);
                            lda.fisherlda(ref xy, ns, nf, nc, ref info, ref w1);
                            for(i=0; i<=nf-1; i++)
                            {
                                lda1errors = lda1errors | (double)(w1[i])!=(double)(wn[i,0]);
                            }
                        }
                    }
                }
            }
            
            //
            // Final report
            //
            waserrors = ldanerrors | lda1errors;
            if( !silent )
            {
                System.Console.Write("LDA TEST");
                System.Console.WriteLine();
                System.Console.Write("FISHER LDA-N:                            ");
                if( !ldanerrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("FISHER LDA-1:                            ");
                if( !lda1errors )
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
        Generates 'simple' set - a sequence of unit 'balls' at (0,0), (1,0), (2,0)
        and so on.
        *************************************************************************/
        private static void gensimpleset(int nfeatures,
            int nclasses,
            int nsamples,
            int axis,
            ref double[,] xy)
        {
            int i = 0;
            int j = 0;
            int c = 0;
            double v = 0;

            System.Diagnostics.Debug.Assert(axis>=0 & axis<nfeatures, "GenSimpleSet: wrong Axis!");
            xy = new double[nsamples-1+1, nfeatures+1];
            for(i=0; i<=nsamples-1; i++)
            {
                for(j=0; j<=nfeatures-1; j++)
                {
                    xy[i,j] = generatenormal(0.0, 1.0);
                }
                c = i%nclasses;
                xy[i,axis] = xy[i,axis]+c;
                xy[i,nfeatures] = c;
            }
        }


        /*************************************************************************
        Generates 'degenerate' set #1.
        NFeatures>=3.
        *************************************************************************/
        private static void gendeg1set(int nfeatures,
            int nclasses,
            int nsamples,
            int axis,
            ref double[,] xy)
        {
            int i = 0;
            int j = 0;
            int c = 0;
            double v = 0;

            System.Diagnostics.Debug.Assert(axis>=0 & axis<nfeatures, "GenDeg1Set: wrong Axis!");
            System.Diagnostics.Debug.Assert(nfeatures>=3, "GenDeg1Set: wrong NFeatures!");
            xy = new double[nsamples-1+1, nfeatures+1];
            if( axis>=nfeatures-2 )
            {
                axis = nfeatures-3;
            }
            for(i=0; i<=nsamples-1; i++)
            {
                for(j=0; j<=nfeatures-2; j++)
                {
                    xy[i,j] = generatenormal(0.0, 1.0);
                }
                xy[i,nfeatures-1] = xy[i,nfeatures-2];
                c = i%nclasses;
                xy[i,axis] = xy[i,axis]+c;
                xy[i,nfeatures] = c;
            }
        }


        /*************************************************************************
        Normal random number
        *************************************************************************/
        private static double generatenormal(double mean,
            double sigma)
        {
            double result = 0;
            double u = 0;
            double v = 0;
            double s = 0;
            double sum = 0;

            result = mean;
            while( true )
            {
                u = (2*AP.Math.RandomInteger(2)-1)*AP.Math.RandomReal();
                v = (2*AP.Math.RandomInteger(2)-1)*AP.Math.RandomReal();
                sum = u*u+v*v;
                if( (double)(sum)<(double)(1) & (double)(sum)>(double)(0) )
                {
                    sum = Math.Sqrt(-(2*Math.Log(sum)/sum));
                    result = sigma*u*sum+mean;
                    return result;
                }
            }
            return result;
        }


        /*************************************************************************
        Tests WN for correctness
        *************************************************************************/
        private static bool testwn(ref double[,] xy,
            ref double[,] wn,
            int ns,
            int nf,
            int nc,
            int ndeg)
        {
            bool result = new bool();
            double[,] st = new double[0,0];
            double[,] sw = new double[0,0];
            double[,] a = new double[0,0];
            double[,] z = new double[0,0];
            double[] tx = new double[0];
            double[] jp = new double[0];
            double[] jq = new double[0];
            double[] work = new double[0];
            int i = 0;
            int j = 0;
            double v = 0;
            double wprev = 0;
            double tol = 0;
            double p = 0;
            double q = 0;
            int i_ = 0;

            tol = 10000;
            result = true;
            fishers(ref xy, ns, nf, nc, ref st, ref sw);
            
            //
            // Test for decreasing of J
            //
            tx = new double[nf-1+1];
            jp = new double[nf-1+1];
            jq = new double[nf-1+1];
            for(j=0; j<=nf-1; j++)
            {
                for(i_=0; i_<=nf-1;i_++)
                {
                    tx[i_] = wn[i_,j];
                }
                v = calcj(nf, ref st, ref sw, ref tx, ref p, ref q);
                jp[j] = p;
                jq[j] = q;
            }
            for(i=1; i<=nf-1-ndeg; i++)
            {
                result = result & (double)(jp[i-1]/jq[i-1])>=(double)((1-tol*AP.Math.MachineEpsilon)*jp[i]/jq[i]);
            }
            for(i=nf-1-ndeg+1; i<=nf-1; i++)
            {
                result = result & (double)(jp[i])<=(double)(tol*AP.Math.MachineEpsilon*jp[0]);
            }
            
            //
            // Test for J optimality
            //
            for(i_=0; i_<=nf-1;i_++)
            {
                tx[i_] = wn[i_,0];
            }
            v = calcj(nf, ref st, ref sw, ref tx, ref p, ref q);
            for(i=0; i<=nf-1; i++)
            {
                wprev = tx[i];
                tx[i] = wprev+0.01;
                result = result & (double)(v)>=(double)((1-tol*AP.Math.MachineEpsilon)*calcj(nf, ref st, ref sw, ref tx, ref p, ref q));
                tx[i] = wprev-0.01;
                result = result & (double)(v)>=(double)((1-tol*AP.Math.MachineEpsilon)*calcj(nf, ref st, ref sw, ref tx, ref p, ref q));
                tx[i] = wprev;
            }
            
            //
            // Test for linear independence of W
            //
            work = new double[nf+1];
            a = new double[nf-1+1, nf-1+1];
            blas.matrixmatrixmultiply(ref wn, 0, nf-1, 0, nf-1, false, ref wn, 0, nf-1, 0, nf-1, true, 1.0, ref a, 0, nf-1, 0, nf-1, 0.0, ref work);
            if( sevd.smatrixevd(a, nf, 1, true, ref tx, ref z) )
            {
                result = result & (double)(tx[0])>(double)(tx[nf-1]*1000*AP.Math.MachineEpsilon);
            }
            
            //
            // Test for other properties
            //
            for(j=0; j<=nf-1; j++)
            {
                v = 0.0;
                for(i_=0; i_<=nf-1;i_++)
                {
                    v += wn[i_,j]*wn[i_,j];
                }
                v = Math.Sqrt(v);
                result = result & (double)(Math.Abs(v-1))<=(double)(1000*AP.Math.MachineEpsilon);
                v = 0;
                for(i=0; i<=nf-1; i++)
                {
                    v = v+wn[i,j];
                }
                result = result & (double)(v)>=(double)(0);
            }
            return result;
        }


        /*************************************************************************
        Calculates J
        *************************************************************************/
        private static double calcj(int nf,
            ref double[,] st,
            ref double[,] sw,
            ref double[] w,
            ref double p,
            ref double q)
        {
            double result = 0;
            double[] tx = new double[0];
            int i = 0;
            int j = 0;
            double v = 0;
            int i_ = 0;

            tx = new double[nf-1+1];
            for(i=0; i<=nf-1; i++)
            {
                v = 0.0;
                for(i_=0; i_<=nf-1;i_++)
                {
                    v += st[i,i_]*w[i_];
                }
                tx[i] = v;
            }
            v = 0.0;
            for(i_=0; i_<=nf-1;i_++)
            {
                v += w[i_]*tx[i_];
            }
            p = v;
            for(i=0; i<=nf-1; i++)
            {
                v = 0.0;
                for(i_=0; i_<=nf-1;i_++)
                {
                    v += sw[i,i_]*w[i_];
                }
                tx[i] = v;
            }
            v = 0.0;
            for(i_=0; i_<=nf-1;i_++)
            {
                v += w[i_]*tx[i_];
            }
            q = v;
            result = p/q;
            return result;
        }


        /*************************************************************************
        Calculates ST/SW
        *************************************************************************/
        private static void fishers(ref double[,] xy,
            int npoints,
            int nfeatures,
            int nclasses,
            ref double[,] st,
            ref double[,] sw)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            int[] c = new int[0];
            double[] mu = new double[0];
            double[,] muc = new double[0,0];
            int[] nc = new int[0];
            double[] tf = new double[0];
            double[] work = new double[0];
            int i_ = 0;

            
            //
            // Prepare temporaries
            //
            tf = new double[nfeatures-1+1];
            work = new double[nfeatures+1];
            
            //
            // Convert class labels from reals to integers (just for convenience)
            //
            c = new int[npoints-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                c[i] = (int)Math.Round(xy[i,nfeatures]);
            }
            
            //
            // Calculate class sizes and means
            //
            mu = new double[nfeatures-1+1];
            muc = new double[nclasses-1+1, nfeatures-1+1];
            nc = new int[nclasses-1+1];
            for(j=0; j<=nfeatures-1; j++)
            {
                mu[j] = 0;
            }
            for(i=0; i<=nclasses-1; i++)
            {
                nc[i] = 0;
                for(j=0; j<=nfeatures-1; j++)
                {
                    muc[i,j] = 0;
                }
            }
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nfeatures-1;i_++)
                {
                    mu[i_] = mu[i_] + xy[i,i_];
                }
                for(i_=0; i_<=nfeatures-1;i_++)
                {
                    muc[c[i],i_] = muc[c[i],i_] + xy[i,i_];
                }
                nc[c[i]] = nc[c[i]]+1;
            }
            for(i=0; i<=nclasses-1; i++)
            {
                v = (double)(1)/(double)(nc[i]);
                for(i_=0; i_<=nfeatures-1;i_++)
                {
                    muc[i,i_] = v*muc[i,i_];
                }
            }
            v = (double)(1)/(double)(npoints);
            for(i_=0; i_<=nfeatures-1;i_++)
            {
                mu[i_] = v*mu[i_];
            }
            
            //
            // Create ST matrix
            //
            st = new double[nfeatures-1+1, nfeatures-1+1];
            for(i=0; i<=nfeatures-1; i++)
            {
                for(j=0; j<=nfeatures-1; j++)
                {
                    st[i,j] = 0;
                }
            }
            for(k=0; k<=npoints-1; k++)
            {
                for(i_=0; i_<=nfeatures-1;i_++)
                {
                    tf[i_] = xy[k,i_];
                }
                for(i_=0; i_<=nfeatures-1;i_++)
                {
                    tf[i_] = tf[i_] - mu[i_];
                }
                for(i=0; i<=nfeatures-1; i++)
                {
                    v = tf[i];
                    for(i_=0; i_<=nfeatures-1;i_++)
                    {
                        st[i,i_] = st[i,i_] + v*tf[i_];
                    }
                }
            }
            
            //
            // Create SW matrix
            //
            sw = new double[nfeatures-1+1, nfeatures-1+1];
            for(i=0; i<=nfeatures-1; i++)
            {
                for(j=0; j<=nfeatures-1; j++)
                {
                    sw[i,j] = 0;
                }
            }
            for(k=0; k<=npoints-1; k++)
            {
                for(i_=0; i_<=nfeatures-1;i_++)
                {
                    tf[i_] = xy[k,i_];
                }
                for(i_=0; i_<=nfeatures-1;i_++)
                {
                    tf[i_] = tf[i_] - muc[c[k],i_];
                }
                for(i=0; i<=nfeatures-1; i++)
                {
                    v = tf[i];
                    for(i_=0; i_<=nfeatures-1;i_++)
                    {
                        sw[i,i_] = sw[i,i_] + v*tf[i_];
                    }
                }
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testldaunit_test_silent()
        {
            bool result = new bool();

            result = testlda(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testldaunit_test()
        {
            bool result = new bool();

            result = testlda(false);
            return result;
        }
    }
}
