
using System;

namespace alglib
{
    public class testregressunit
    {
        public static bool testlinregression(bool silent)
        {
            bool result = new bool();
            double sigmathreshold = 0;
            int maxn = 0;
            int maxm = 0;
            int passcount = 0;
            int estpasscount = 0;
            double threshold = 0;
            int n = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int tmpi = 0;
            int pass = 0;
            int epass = 0;
            int m = 0;
            int tasktype = 0;
            int modeltype = 0;
            int m1 = 0;
            int m2 = 0;
            int n1 = 0;
            int n2 = 0;
            int info = 0;
            int info2 = 0;
            double[,] xy = new double[0,0];
            double[,] xy2 = new double[0,0];
            double[] s = new double[0];
            double[] s2 = new double[0];
            double[] w2 = new double[0];
            double[] x = new double[0];
            double[] ta = new double[0];
            double[] tb = new double[0];
            double[] tc = new double[0];
            double[] xy0 = new double[0];
            double[] tmpweights = new double[0];
            linreg.linearmodel w = new linreg.linearmodel();
            linreg.linearmodel wt = new linreg.linearmodel();
            linreg.linearmodel wt2 = new linreg.linearmodel();
            double[] x1 = new double[0];
            double[] x2 = new double[0];
            double[] ra = new double[0];
            double[] ra2 = new double[0];
            double y1 = 0;
            double y2 = 0;
            int rlen = 0;
            bool allsame = new bool();
            double ea = 0;
            double eb = 0;
            double varatested = 0;
            double varbtested = 0;
            double a = 0;
            double b = 0;
            double vara = 0;
            double varb = 0;
            double a2 = 0;
            double b2 = 0;
            double covab = 0;
            double corrab = 0;
            double p = 0;
            int qcnt = 0;
            double[] qtbl = new double[0];
            double[] qvals = new double[0];
            double[] qsigma = new double[0];
            linreg.lrreport ar = new linreg.lrreport();
            linreg.lrreport ar2 = new linreg.lrreport();
            double f = 0;
            double fp = 0;
            double fm = 0;
            double v = 0;
            double vv = 0;
            double cvrmserror = 0;
            double cvavgerror = 0;
            double cvavgrelerror = 0;
            double rmserror = 0;
            double avgerror = 0;
            double avgrelerror = 0;
            bool nondefect = new bool();
            double sinshift = 0;
            double tasklevel = 0;
            double noiselevel = 0;
            double hstep = 0;
            double sigma = 0;
            double mean = 0;
            double means = 0;
            double stddev = 0;
            double stddevs = 0;
            bool slcerrors = new bool();
            bool slerrors = new bool();
            bool grcoverrors = new bool();
            bool gropterrors = new bool();
            bool gresterrors = new bool();
            bool grothererrors = new bool();
            bool grconverrors = new bool();
            bool waserrors = new bool();
            int i_ = 0;

            
            //
            // Primary settings
            //
            maxn = 40;
            maxm = 5;
            passcount = 3;
            estpasscount = 1000;
            sigmathreshold = 7;
            threshold = 1000000*AP.Math.MachineEpsilon;
            slerrors = false;
            slcerrors = false;
            grcoverrors = false;
            gropterrors = false;
            gresterrors = false;
            grothererrors = false;
            grconverrors = false;
            waserrors = false;
            
            //
            // Quantiles table setup
            //
            qcnt = 5;
            qtbl = new double[qcnt-1+1];
            qvals = new double[qcnt-1+1];
            qsigma = new double[qcnt-1+1];
            qtbl[0] = 0.5;
            qtbl[1] = 0.25;
            qtbl[2] = 0.10;
            qtbl[3] = 0.05;
            qtbl[4] = 0.025;
            for(i=0; i<=qcnt-1; i++)
            {
                qsigma[i] = Math.Sqrt(qtbl[i]*(1-qtbl[i])/estpasscount);
            }
            
            //
            // Other setup
            //
            ta = new double[estpasscount-1+1];
            tb = new double[estpasscount-1+1];
            
            //
            // Test straight line regression
            //
            for(n=2; n<=maxn; n++)
            {
                
                //
                // Fail/pass test
                //
                generaterandomtask(-1, 1, false, -1, 1, 1, 2, n, ref xy, ref s);
                linreg.lrlines(ref xy, ref s, n, ref info, ref a, ref b, ref vara, ref varb, ref covab, ref corrab, ref p);
                slcerrors = slcerrors | info!=1;
                generaterandomtask(+1, 1, false, -1, 1, 1, 2, n, ref xy, ref s);
                linreg.lrlines(ref xy, ref s, n, ref info, ref a, ref b, ref vara, ref varb, ref covab, ref corrab, ref p);
                slcerrors = slcerrors | info!=-3;
                generaterandomtask(-1, 1, false, -1, 1, -1, -1, n, ref xy, ref s);
                linreg.lrlines(ref xy, ref s, n, ref info, ref a, ref b, ref vara, ref varb, ref covab, ref corrab, ref p);
                slcerrors = slcerrors | info!=-2;
                generaterandomtask(-1, 1, false, -1, 1, 2, 1, 2, ref xy, ref s);
                linreg.lrlines(ref xy, ref s, 1, ref info, ref a, ref b, ref vara, ref varb, ref covab, ref corrab, ref p);
                slcerrors = slcerrors | info!=-1;
                
                //
                // Multipass tests
                //
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // Test S variant against non-S variant
                    //
                    ea = 2*AP.Math.RandomReal()-1;
                    eb = 2*AP.Math.RandomReal()-1;
                    generatetask(ea, eb, -(5*AP.Math.RandomReal()), +(5*AP.Math.RandomReal()), (double)(AP.Math.RandomReal())>(double)(0.5), 1, 1, n, ref xy, ref s);
                    linreg.lrlines(ref xy, ref s, n, ref info, ref a, ref b, ref vara, ref varb, ref covab, ref corrab, ref p);
                    linreg.lrline(ref xy, n, ref info2, ref a2, ref b2);
                    if( info!=1 | info2!=1 )
                    {
                        slcerrors = true;
                    }
                    else
                    {
                        slerrors = slerrors | (double)(Math.Abs(a-a2))>(double)(threshold) | (double)(Math.Abs(b-b2))>(double)(threshold);
                    }
                    
                    //
                    // Test for A/B
                    //
                    // Generate task with exact, non-perturbed y[i],
                    // then make non-zero s[i]
                    //
                    ea = 2*AP.Math.RandomReal()-1;
                    eb = 2*AP.Math.RandomReal()-1;
                    generatetask(ea, eb, -(5*AP.Math.RandomReal()), +(5*AP.Math.RandomReal()), n>4, 0.0, 0.0, n, ref xy, ref s);
                    for(i=0; i<=n-1; i++)
                    {
                        s[i] = 1+AP.Math.RandomReal();
                    }
                    linreg.lrlines(ref xy, ref s, n, ref info, ref a, ref b, ref vara, ref varb, ref covab, ref corrab, ref p);
                    if( info!=1 )
                    {
                        slcerrors = true;
                    }
                    else
                    {
                        slerrors = slerrors | (double)(Math.Abs(a-ea))>(double)(0.001) | (double)(Math.Abs(b-eb))>(double)(0.001);
                    }
                    
                    //
                    // Test for VarA, VarB, P (P is being tested only for N>2)
                    //
                    for(i=0; i<=qcnt-1; i++)
                    {
                        qvals[i] = 0;
                    }
                    ea = 2*AP.Math.RandomReal()-1;
                    eb = 2*AP.Math.RandomReal()-1;
                    generatetask(ea, eb, -(5*AP.Math.RandomReal()), +(5*AP.Math.RandomReal()), n>4, 1.0, 2.0, n, ref xy, ref s);
                    linreg.lrlines(ref xy, ref s, n, ref info, ref a, ref b, ref vara, ref varb, ref covab, ref corrab, ref p);
                    if( info!=1 )
                    {
                        slcerrors = true;
                        continue;
                    }
                    varatested = vara;
                    varbtested = varb;
                    for(epass=0; epass<=estpasscount-1; epass++)
                    {
                        
                        //
                        // Generate
                        //
                        filltaskwithy(ea, eb, n, ref xy, ref s);
                        linreg.lrlines(ref xy, ref s, n, ref info, ref a, ref b, ref vara, ref varb, ref covab, ref corrab, ref p);
                        if( info!=1 )
                        {
                            slcerrors = true;
                            continue;
                        }
                        
                        //
                        // A, B, P
                        // (P is being tested for uniformity, additional p-tests are below)
                        //
                        ta[epass] = a;
                        tb[epass] = b;
                        for(i=0; i<=qcnt-1; i++)
                        {
                            if( (double)(p)<=(double)(qtbl[i]) )
                            {
                                qvals[i] = qvals[i]+(double)(1)/(double)(estpasscount);
                            }
                        }
                    }
                    calculatemv(ref ta, estpasscount, ref mean, ref means, ref stddev, ref stddevs);
                    slerrors = slerrors | (double)(Math.Abs(mean-ea)/means)>=(double)(sigmathreshold);
                    slerrors = slerrors | (double)(Math.Abs(stddev-Math.Sqrt(varatested))/stddevs)>=(double)(sigmathreshold);
                    calculatemv(ref tb, estpasscount, ref mean, ref means, ref stddev, ref stddevs);
                    slerrors = slerrors | (double)(Math.Abs(mean-eb)/means)>=(double)(sigmathreshold);
                    slerrors = slerrors | (double)(Math.Abs(stddev-Math.Sqrt(varbtested))/stddevs)>=(double)(sigmathreshold);
                    if( n>2 )
                    {
                        for(i=0; i<=qcnt-1; i++)
                        {
                            if( (double)(Math.Abs(qtbl[i]-qvals[i])/qsigma[i])>(double)(sigmathreshold) )
                            {
                                slerrors = true;
                            }
                        }
                    }
                    
                    //
                    // Additional tests for P: correlation with fit quality
                    //
                    if( n>2 )
                    {
                        generatetask(ea, eb, -(5*AP.Math.RandomReal()), +(5*AP.Math.RandomReal()), false, 0.0, 0.0, n, ref xy, ref s);
                        for(i=0; i<=n-1; i++)
                        {
                            s[i] = 1+AP.Math.RandomReal();
                        }
                        linreg.lrlines(ref xy, ref s, n, ref info, ref a, ref b, ref vara, ref varb, ref covab, ref corrab, ref p);
                        if( info!=1 )
                        {
                            slcerrors = true;
                            continue;
                        }
                        slerrors = slerrors | (double)(p)<(double)(0.999);
                        generatetask(0, 0, -(5*AP.Math.RandomReal()), +(5*AP.Math.RandomReal()), false, 1.0, 1.0, n, ref xy, ref s);
                        for(i=0; i<=n-1; i++)
                        {
                            if( i%2==0 )
                            {
                                xy[i,1] = +5.0;
                            }
                            else
                            {
                                xy[i,1] = -5.0;
                            }
                        }
                        if( n%2!=0 )
                        {
                            xy[n-1,1] = 0;
                        }
                        linreg.lrlines(ref xy, ref s, n, ref info, ref a, ref b, ref vara, ref varb, ref covab, ref corrab, ref p);
                        if( info!=1 )
                        {
                            slcerrors = true;
                            continue;
                        }
                        slerrors = slerrors | (double)(p)>(double)(0.001);
                    }
                }
            }
            
            //
            // General regression tests:
            //
            
            //
            // Simple linear tests (small sample, optimum point, covariance)
            //
            for(n=3; n<=maxn; n++)
            {
                s = new double[n-1+1];
                
                //
                // Linear tests:
                // a. random points, sigmas
                // b. no sigmas
                //
                xy = new double[n-1+1, 1+1];
                for(i=0; i<=n-1; i++)
                {
                    xy[i,0] = 2*AP.Math.RandomReal()-1;
                    xy[i,1] = 2*AP.Math.RandomReal()-1;
                    s[i] = 1+AP.Math.RandomReal();
                }
                linreg.lrbuilds(ref xy, ref s, n, 1, ref info, ref wt, ref ar);
                if( info!=1 )
                {
                    grconverrors = true;
                    continue;
                }
                linreg.lrunpack(ref wt, ref tmpweights, ref tmpi);
                linreg.lrlines(ref xy, ref s, n, ref info2, ref a, ref b, ref vara, ref varb, ref covab, ref corrab, ref p);
                gropterrors = gropterrors | (double)(Math.Abs(a-tmpweights[1]))>(double)(threshold);
                gropterrors = gropterrors | (double)(Math.Abs(b-tmpweights[0]))>(double)(threshold);
                grcoverrors = grcoverrors | (double)(Math.Abs(vara-ar.c[1,1]))>(double)(threshold);
                grcoverrors = grcoverrors | (double)(Math.Abs(varb-ar.c[0,0]))>(double)(threshold);
                grcoverrors = grcoverrors | (double)(Math.Abs(covab-ar.c[1,0]))>(double)(threshold);
                grcoverrors = grcoverrors | (double)(Math.Abs(covab-ar.c[0,1]))>(double)(threshold);
                linreg.lrbuild(ref xy, n, 1, ref info, ref wt, ref ar);
                if( info!=1 )
                {
                    grconverrors = true;
                    continue;
                }
                linreg.lrunpack(ref wt, ref tmpweights, ref tmpi);
                linreg.lrline(ref xy, n, ref info2, ref a, ref b);
                gropterrors = gropterrors | (double)(Math.Abs(a-tmpweights[1]))>(double)(threshold);
                gropterrors = gropterrors | (double)(Math.Abs(b-tmpweights[0]))>(double)(threshold);
            }
            
            //
            // S covariance versus S-less covariance.
            // Slightly skewed task, large sample size.
            // Will S-less subroutine estimate covariance matrix good enough?
            //
            n = 1000+AP.Math.RandomInteger(3000);
            sigma = 0.1+AP.Math.RandomReal()*1.9;
            xy = new double[n-1+1, 1+1];
            s = new double[n-1+1];
            for(i=0; i<=n-1; i++)
            {
                xy[i,0] = 1.5*AP.Math.RandomReal()-0.5;
                xy[i,1] = 1.2*xy[i,0]-0.3+generatenormal(0, sigma);
                s[i] = sigma;
            }
            linreg.lrbuild(ref xy, n, 1, ref info, ref wt, ref ar);
            linreg.lrlines(ref xy, ref s, n, ref info2, ref a, ref b, ref vara, ref varb, ref covab, ref corrab, ref p);
            if( info!=1 | info2!=1 )
            {
                grconverrors = true;
            }
            else
            {
                grcoverrors = grcoverrors | (double)(Math.Abs(Math.Log(ar.c[0,0]/varb)))>(double)(Math.Log(1.2));
                grcoverrors = grcoverrors | (double)(Math.Abs(Math.Log(ar.c[1,1]/vara)))>(double)(Math.Log(1.2));
                grcoverrors = grcoverrors | (double)(Math.Abs(Math.Log(ar.c[0,1]/covab)))>(double)(Math.Log(1.2));
                grcoverrors = grcoverrors | (double)(Math.Abs(Math.Log(ar.c[1,0]/covab)))>(double)(Math.Log(1.2));
            }
            
            //
            // General tests:
            // * basis functions - up to cubic
            // * task types:
            // * data set is noisy sine half-period with random shift
            // * tests:
            //   unpacking/packing
            //   optimality
            //   error estimates
            // * tasks:
            //   0 = noised sine
            //   1 = degenerate task with 1-of-n encoded categorical variables
            //   2 = random task with large variation (for 1-type models)
            //   3 = random task with small variation (for 1-type models)
            //
            //   Additional tasks TODO
            //   specially designed task with defective vectors which leads to
            //   the failure of the fast CV formula.
            //
            //
            for(modeltype=0; modeltype<=1; modeltype++)
            {
                for(tasktype=0; tasktype<=3; tasktype++)
                {
                    if( tasktype==0 )
                    {
                        m1 = 1;
                        m2 = 3;
                    }
                    if( tasktype==1 )
                    {
                        m1 = 9;
                        m2 = 9;
                    }
                    if( tasktype==2 | tasktype==3 )
                    {
                        m1 = 9;
                        m2 = 9;
                    }
                    for(m=m1; m<=m2; m++)
                    {
                        if( tasktype==0 )
                        {
                            n1 = m+3;
                            n2 = m+20;
                        }
                        if( tasktype==1 )
                        {
                            n1 = 70+AP.Math.RandomInteger(70);
                            n2 = n1;
                        }
                        if( tasktype==2 | tasktype==3 )
                        {
                            n1 = 100;
                            n2 = n1;
                        }
                        for(n=n1; n<=n2; n++)
                        {
                            xy = new double[n-1+1, m+1];
                            xy0 = new double[n-1+1];
                            s = new double[n-1+1];
                            hstep = 0.001;
                            noiselevel = 0.2;
                            
                            //
                            // Prepare task
                            //
                            if( tasktype==0 )
                            {
                                for(i=0; i<=n-1; i++)
                                {
                                    xy[i,0] = 2*AP.Math.RandomReal()-1;
                                }
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=1; j<=m-1; j++)
                                    {
                                        xy[i,j] = xy[i,0]*xy[i,j-1];
                                    }
                                }
                                sinshift = AP.Math.RandomReal()*Math.PI;
                                for(i=0; i<=n-1; i++)
                                {
                                    xy0[i] = Math.Sin(sinshift+Math.PI*0.5*(xy[i,0]+1));
                                    xy[i,m] = xy0[i]+noiselevel*generatenormal(0, 1);
                                }
                            }
                            if( tasktype==1 )
                            {
                                System.Diagnostics.Debug.Assert(m==9);
                                ta = new double[8+1];
                                ta[0] = 1;
                                ta[1] = 2;
                                ta[2] = 3;
                                ta[3] = 0.25;
                                ta[4] = 0.5;
                                ta[5] = 0.75;
                                ta[6] = 0.06;
                                ta[7] = 0.12;
                                ta[8] = 0.18;
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=m-1; j++)
                                    {
                                        xy[i,j] = 0;
                                    }
                                    xy[i,0+i%3] = 1;
                                    xy[i,3+i/3%3] = 1;
                                    xy[i,6+i/9%3] = 1;
                                    v = 0.0;
                                    for(i_=0; i_<=8;i_++)
                                    {
                                        v += xy[i,i_]*ta[i_];
                                    }
                                    xy0[i] = v;
                                    xy[i,m] = v+noiselevel*generatenormal(0, 1);
                                }
                            }
                            if( tasktype==2 | tasktype==3 )
                            {
                                System.Diagnostics.Debug.Assert(m==9);
                                ta = new double[8+1];
                                ta[0] = 1;
                                ta[1] = -2;
                                ta[2] = 3;
                                ta[3] = 0.25;
                                ta[4] = -0.5;
                                ta[5] = 0.75;
                                ta[6] = -0.06;
                                ta[7] = 0.12;
                                ta[8] = -0.18;
                                for(i=0; i<=n-1; i++)
                                {
                                    for(j=0; j<=m-1; j++)
                                    {
                                        if( tasktype==2 )
                                        {
                                            xy[i,j] = 1+generatenormal(0, 3);
                                        }
                                        else
                                        {
                                            xy[i,j] = 1+generatenormal(0, 0.05);
                                        }
                                    }
                                    v = 0.0;
                                    for(i_=0; i_<=8;i_++)
                                    {
                                        v += xy[i,i_]*ta[i_];
                                    }
                                    xy0[i] = v;
                                    xy[i,m] = v+noiselevel*generatenormal(0, 1);
                                }
                            }
                            for(i=0; i<=n-1; i++)
                            {
                                s[i] = 1+AP.Math.RandomReal();
                            }
                            
                            //
                            // Solve (using S-variant, non-S-variant is not tested)
                            //
                            if( modeltype==0 )
                            {
                                linreg.lrbuilds(ref xy, ref s, n, m, ref info, ref wt, ref ar);
                            }
                            else
                            {
                                linreg.lrbuildzs(ref xy, ref s, n, m, ref info, ref wt, ref ar);
                            }
                            if( info!=1 )
                            {
                                grconverrors = true;
                                continue;
                            }
                            linreg.lrunpack(ref wt, ref tmpweights, ref tmpi);
                            
                            //
                            // LRProcess test
                            //
                            x = new double[m-1+1];
                            v = tmpweights[m];
                            for(i=0; i<=m-1; i++)
                            {
                                x[i] = 2*AP.Math.RandomReal()-1;
                                v = v+tmpweights[i]*x[i];
                            }
                            grothererrors = grothererrors | (double)(Math.Abs(v-linreg.lrprocess(ref wt, ref x))/Math.Max(Math.Abs(v), 1))>(double)(threshold);
                            
                            //
                            // LRPack test
                            //
                            linreg.lrpack(ref tmpweights, m, ref wt2);
                            x = new double[m-1+1];
                            for(i=0; i<=m-1; i++)
                            {
                                x[i] = 2*AP.Math.RandomReal()-1;
                            }
                            v = linreg.lrprocess(ref wt, ref x);
                            grothererrors = grothererrors | (double)(Math.Abs(v-linreg.lrprocess(ref wt2, ref x))/Math.Abs(v))>(double)(threshold);
                            
                            //
                            // Optimality test
                            //
                            for(k=0; k<=m; k++)
                            {
                                if( modeltype==1 & k==m )
                                {
                                    
                                    //
                                    // 0-type models (with non-zero constant term)
                                    // are tested for optimality of all coefficients.
                                    //
                                    // 1-type models (with zero constant term)
                                    // are tested for optimality of non-constant terms only.
                                    //
                                    continue;
                                }
                                f = 0;
                                fp = 0;
                                fm = 0;
                                for(i=0; i<=n-1; i++)
                                {
                                    v = tmpweights[m];
                                    for(j=0; j<=m-1; j++)
                                    {
                                        v = v+xy[i,j]*tmpweights[j];
                                    }
                                    f = f+AP.Math.Sqr((v-xy[i,m])/s[i]);
                                    if( k<m )
                                    {
                                        vv = xy[i,k];
                                    }
                                    else
                                    {
                                        vv = 1;
                                    }
                                    fp = fp+AP.Math.Sqr((v+vv*hstep-xy[i,m])/s[i]);
                                    fm = fm+AP.Math.Sqr((v-vv*hstep-xy[i,m])/s[i]);
                                }
                                gropterrors = gropterrors | (double)(f)>(double)(fp) | (double)(f)>(double)(fm);
                            }
                            
                            //
                            // Covariance matrix test:
                            // generate random vector, project coefficients on it,
                            // compare variance of projection with estimate provided
                            // by cov.matrix
                            //
                            ta = new double[estpasscount-1+1];
                            tb = new double[m+1];
                            tc = new double[m+1];
                            xy2 = new double[n-1+1, m+1];
                            for(i=0; i<=m; i++)
                            {
                                tb[i] = generatenormal(0, 1);
                            }
                            for(epass=0; epass<=estpasscount-1; epass++)
                            {
                                for(i=0; i<=n-1; i++)
                                {
                                    for(i_=0; i_<=m-1;i_++)
                                    {
                                        xy2[i,i_] = xy[i,i_];
                                    }
                                    xy2[i,m] = xy0[i]+s[i]*generatenormal(0, 1);
                                }
                                if( modeltype==0 )
                                {
                                    linreg.lrbuilds(ref xy2, ref s, n, m, ref info, ref wt, ref ar2);
                                }
                                else
                                {
                                    linreg.lrbuildzs(ref xy2, ref s, n, m, ref info, ref wt, ref ar2);
                                }
                                if( info!=1 )
                                {
                                    ta[epass] = 0;
                                    grconverrors = true;
                                    return result;
                                }
                                linreg.lrunpack(ref wt, ref w2, ref tmpi);
                                v = 0.0;
                                for(i_=0; i_<=m;i_++)
                                {
                                    v += tb[i_]*w2[i_];
                                }
                                ta[epass] = v;
                            }
                            calculatemv(ref ta, estpasscount, ref mean, ref means, ref stddev, ref stddevs);
                            for(i=0; i<=m; i++)
                            {
                                v = 0.0;
                                for(i_=0; i_<=m;i_++)
                                {
                                    v += tb[i_]*ar.c[i_,i];
                                }
                                tc[i] = v;
                            }
                            v = 0.0;
                            for(i_=0; i_<=m;i_++)
                            {
                                v += tc[i_]*tb[i_];
                            }
                            grcoverrors = grcoverrors | (double)(Math.Abs((Math.Sqrt(v)-stddev)/stddevs))>=(double)(sigmathreshold);
                            
                            //
                            // Test for the fast CV error:
                            // calculate CV error by definition (leaving out N
                            // points and recalculating solution).
                            //
                            // Test for the training set error
                            //
                            cvrmserror = 0;
                            cvavgerror = 0;
                            cvavgrelerror = 0;
                            rmserror = 0;
                            avgerror = 0;
                            avgrelerror = 0;
                            xy2 = new double[n-2+1, m+1];
                            s2 = new double[n-2+1];
                            for(i=0; i<=n-2; i++)
                            {
                                for(i_=0; i_<=m;i_++)
                                {
                                    xy2[i,i_] = xy[i+1,i_];
                                }
                                s2[i] = s[i+1];
                            }
                            for(i=0; i<=n-1; i++)
                            {
                                
                                //
                                // Trn
                                //
                                v = 0.0;
                                for(i_=0; i_<=m-1;i_++)
                                {
                                    v += xy[i,i_]*tmpweights[i_];
                                }
                                v = v+tmpweights[m];
                                rmserror = rmserror+AP.Math.Sqr(v-xy[i,m]);
                                avgerror = avgerror+Math.Abs(v-xy[i,m]);
                                avgrelerror = avgrelerror+Math.Abs((v-xy[i,m])/xy[i,m]);
                                
                                //
                                // CV: non-defect vectors only
                                //
                                nondefect = true;
                                for(k=0; k<=ar.ncvdefects-1; k++)
                                {
                                    if( ar.cvdefects[k]==i )
                                    {
                                        nondefect = false;
                                    }
                                }
                                if( nondefect )
                                {
                                    if( modeltype==0 )
                                    {
                                        linreg.lrbuilds(ref xy2, ref s2, n-1, m, ref info2, ref wt, ref ar2);
                                    }
                                    else
                                    {
                                        linreg.lrbuildzs(ref xy2, ref s2, n-1, m, ref info2, ref wt, ref ar2);
                                    }
                                    if( info2!=1 )
                                    {
                                        grconverrors = true;
                                        continue;
                                    }
                                    linreg.lrunpack(ref wt, ref w2, ref tmpi);
                                    v = 0.0;
                                    for(i_=0; i_<=m-1;i_++)
                                    {
                                        v += xy[i,i_]*w2[i_];
                                    }
                                    v = v+w2[m];
                                    cvrmserror = cvrmserror+AP.Math.Sqr(v-xy[i,m]);
                                    cvavgerror = cvavgerror+Math.Abs(v-xy[i,m]);
                                    cvavgrelerror = cvavgrelerror+Math.Abs((v-xy[i,m])/xy[i,m]);
                                }
                                
                                //
                                // Next set
                                //
                                if( i!=n-1 )
                                {
                                    for(i_=0; i_<=m;i_++)
                                    {
                                        xy2[i,i_] = xy[i,i_];
                                    }
                                    s2[i] = s[i];
                                }
                            }
                            cvrmserror = Math.Sqrt(cvrmserror/(n-ar.ncvdefects));
                            cvavgerror = cvavgerror/(n-ar.ncvdefects);
                            cvavgrelerror = cvavgrelerror/(n-ar.ncvdefects);
                            rmserror = Math.Sqrt(rmserror/n);
                            avgerror = avgerror/n;
                            avgrelerror = avgrelerror/n;
                            gresterrors = gresterrors | (double)(Math.Abs(Math.Log(ar.cvrmserror/cvrmserror)))>(double)(Math.Log(1+1.0E-5));
                            gresterrors = gresterrors | (double)(Math.Abs(Math.Log(ar.cvavgerror/cvavgerror)))>(double)(Math.Log(1+1.0E-5));
                            gresterrors = gresterrors | (double)(Math.Abs(Math.Log(ar.cvavgrelerror/cvavgrelerror)))>(double)(Math.Log(1+1.0E-5));
                            gresterrors = gresterrors | (double)(Math.Abs(Math.Log(ar.rmserror/rmserror)))>(double)(Math.Log(1+1.0E-5));
                            gresterrors = gresterrors | (double)(Math.Abs(Math.Log(ar.avgerror/avgerror)))>(double)(Math.Log(1+1.0E-5));
                            gresterrors = gresterrors | (double)(Math.Abs(Math.Log(ar.avgrelerror/avgrelerror)))>(double)(Math.Log(1+1.0E-5));
                        }
                    }
                }
            }
            
            //
            // Additional subroutines
            //
            for(pass=1; pass<=50; pass++)
            {
                n = 2;
                do
                {
                    noiselevel = AP.Math.RandomReal()+0.1;
                    tasklevel = 2*AP.Math.RandomReal()-1;
                }
                while( (double)(Math.Abs(noiselevel-tasklevel))<=(double)(0.05) );
                xy = new double[3*n-1+1, 1+1];
                for(i=0; i<=n-1; i++)
                {
                    xy[3*i+0,0] = i;
                    xy[3*i+1,0] = i;
                    xy[3*i+2,0] = i;
                    xy[3*i+0,1] = tasklevel-noiselevel;
                    xy[3*i+1,1] = tasklevel;
                    xy[3*i+2,1] = tasklevel+noiselevel;
                }
                linreg.lrbuild(ref xy, 3*n, 1, ref info, ref wt, ref ar);
                if( info==1 )
                {
                    linreg.lrunpack(ref wt, ref tmpweights, ref tmpi);
                    v = linreg.lrrmserror(ref wt, ref xy, 3*n);
                    grothererrors = grothererrors | (double)(Math.Abs(v-noiselevel*Math.Sqrt((double)(2)/(double)(3))))>(double)(threshold);
                    v = linreg.lravgerror(ref wt, ref xy, 3*n);
                    grothererrors = grothererrors | (double)(Math.Abs(v-noiselevel*((double)(2)/(double)(3))))>(double)(threshold);
                    v = linreg.lravgrelerror(ref wt, ref xy, 3*n);
                    vv = (Math.Abs(noiselevel/(tasklevel-noiselevel))+Math.Abs(noiselevel/(tasklevel+noiselevel)))/3;
                    grothererrors = grothererrors | (double)(Math.Abs(v-vv))>(double)(threshold*vv);
                }
                else
                {
                    grothererrors = true;
                }
                for(i=0; i<=n-1; i++)
                {
                    xy[3*i+0,0] = i;
                    xy[3*i+1,0] = i;
                    xy[3*i+2,0] = i;
                    xy[3*i+0,1] = -noiselevel;
                    xy[3*i+1,1] = 0;
                    xy[3*i+2,1] = +noiselevel;
                }
                linreg.lrbuild(ref xy, 3*n, 1, ref info, ref wt, ref ar);
                if( info==1 )
                {
                    linreg.lrunpack(ref wt, ref tmpweights, ref tmpi);
                    v = linreg.lravgrelerror(ref wt, ref xy, 3*n);
                    grothererrors = grothererrors | (double)(Math.Abs(v-1))>(double)(threshold);
                }
                else
                {
                    grothererrors = true;
                }
            }
            for(pass=1; pass<=10; pass++)
            {
                m = 1+AP.Math.RandomInteger(5);
                n = 10+AP.Math.RandomInteger(10);
                xy = new double[n-1+1, m+1];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m; j++)
                    {
                        xy[i,j] = 2*AP.Math.RandomReal()-1;
                    }
                }
                linreg.lrbuild(ref xy, n, m, ref info, ref w, ref ar);
                if( info<0 )
                {
                    grothererrors = true;
                    break;
                }
                x1 = new double[m-1+1];
                x2 = new double[m-1+1];
                
                //
                // Same inputs on original leads to same outputs
                // on copy created using LRCopy
                //
                unsetlr(ref wt);
                linreg.lrcopy(ref w, ref wt);
                for(i=0; i<=m-1; i++)
                {
                    x1[i] = 2*AP.Math.RandomReal()-1;
                    x2[i] = x1[i];
                }
                y1 = linreg.lrprocess(ref w, ref x1);
                y2 = linreg.lrprocess(ref wt, ref x2);
                allsame = (double)(y1)==(double)(y2);
                grothererrors = grothererrors | !allsame;
                
                //
                // Same inputs on original leads to same outputs
                // on copy created using LRSerialize
                //
                unsetlr(ref wt);
                ra = new double[0+1];
                ra[0] = 0;
                rlen = 0;
                linreg.lrserialize(ref w, ref ra, ref rlen);
                ra2 = new double[rlen-1+1];
                for(i=0; i<=rlen-1; i++)
                {
                    ra2[i] = ra[i];
                }
                linreg.lrunserialize(ref ra2, ref wt);
                for(i=0; i<=m-1; i++)
                {
                    x1[i] = 2*AP.Math.RandomReal()-1;
                    x2[i] = x1[i];
                }
                y1 = linreg.lrprocess(ref w, ref x1);
                y2 = linreg.lrprocess(ref wt, ref x2);
                allsame = (double)(y1)==(double)(y2);
                grothererrors = grothererrors | !allsame;
            }
            
            //
            // TODO: Degenerate tests (when design matrix and right part are zero)
            //
            
            //
            // Final report
            //
            waserrors = slerrors | slcerrors | gropterrors | grcoverrors | gresterrors | grothererrors | grconverrors;
            if( !silent )
            {
                System.Console.Write("REGRESSION TEST");
                System.Console.WriteLine();
                System.Console.Write("STRAIGHT LINE REGRESSION:                ");
                if( !slerrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("STRAIGHT LINE REGRESSION CONVERGENCE:    ");
                if( !slcerrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("GENERAL LINEAR REGRESSION:               ");
                if( !(gropterrors | grcoverrors | gresterrors | grothererrors | grconverrors) )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("* OPTIMALITY:                            ");
                if( !gropterrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("* COV. MATRIX:                           ");
                if( !grcoverrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("* ERROR ESTIMATES:                       ");
                if( !gresterrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("* CONVERGENCE:                           ");
                if( !grconverrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("* OTHER SUBROUTINES:                     ");
                if( !grothererrors )
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
        Task generation. Meaningless task, just random numbers.
        *************************************************************************/
        private static void generaterandomtask(double xl,
            double xr,
            bool randomx,
            double ymin,
            double ymax,
            double smin,
            double smax,
            int n,
            ref double[,] xy,
            ref double[] s)
        {
            int i = 0;

            xy = new double[n-1+1, 1+1];
            s = new double[n-1+1];
            for(i=0; i<=n-1; i++)
            {
                if( randomx )
                {
                    xy[i,0] = xl+(xr-xl)*AP.Math.RandomReal();
                }
                else
                {
                    xy[i,0] = xl+(xr-xl)*i/(n-1);
                }
                xy[i,1] = ymin+(ymax-ymin)*AP.Math.RandomReal();
                s[i] = smin+(smax-smin)*AP.Math.RandomReal();
            }
        }


        /*************************************************************************
        Task generation.
        *************************************************************************/
        private static void generatetask(double a,
            double b,
            double xl,
            double xr,
            bool randomx,
            double smin,
            double smax,
            int n,
            ref double[,] xy,
            ref double[] s)
        {
            int i = 0;

            xy = new double[n-1+1, 1+1];
            s = new double[n-1+1];
            for(i=0; i<=n-1; i++)
            {
                if( randomx )
                {
                    xy[i,0] = xl+(xr-xl)*AP.Math.RandomReal();
                }
                else
                {
                    xy[i,0] = xl+(xr-xl)*i/(n-1);
                }
                s[i] = smin+(smax-smin)*AP.Math.RandomReal();
                xy[i,1] = a+b*xy[i,0]+generatenormal(0, s[i]);
            }
        }


        /*************************************************************************
        Task generation.
        y[i] are filled based on A, B, X[I], S[I]
        *************************************************************************/
        private static void filltaskwithy(double a,
            double b,
            int n,
            ref double[,] xy,
            ref double[] s)
        {
            int i = 0;

            for(i=0; i<=n-1; i++)
            {
                xy[i,1] = a+b*xy[i,0]+generatenormal(0, s[i]);
            }
        }


        /*************************************************************************
        Normal random numbers
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
        Moments estimates and their errors
        *************************************************************************/
        private static void calculatemv(ref double[] x,
            int n,
            ref double mean,
            ref double means,
            ref double stddev,
            ref double stddevs)
        {
            int i = 0;
            double v = 0;
            double v1 = 0;
            double v2 = 0;
            double variance = 0;

            mean = 0;
            means = 1;
            stddev = 0;
            stddevs = 1;
            variance = 0;
            if( n<=1 )
            {
                return;
            }
            
            //
            // Mean
            //
            for(i=0; i<=n-1; i++)
            {
                mean = mean+x[i];
            }
            mean = mean/n;
            
            //
            // Variance (using corrected two-pass algorithm)
            //
            if( n!=1 )
            {
                v1 = 0;
                for(i=0; i<=n-1; i++)
                {
                    v1 = v1+AP.Math.Sqr(x[i]-mean);
                }
                v2 = 0;
                for(i=0; i<=n-1; i++)
                {
                    v2 = v2+(x[i]-mean);
                }
                v2 = AP.Math.Sqr(v2)/n;
                variance = (v1-v2)/(n-1);
                if( (double)(variance)<(double)(0) )
                {
                    variance = 0;
                }
                stddev = Math.Sqrt(variance);
            }
            
            //
            // Errors
            //
            means = stddev/Math.Sqrt(n);
            stddevs = stddev*Math.Sqrt(2)/Math.Sqrt(n-1);
        }


        /*************************************************************************
        Unsets LR
        *************************************************************************/
        private static void unsetlr(ref linreg.linearmodel lr)
        {
            double[,] xy = new double[0,0];
            int info = 0;
            linreg.lrreport rep = new linreg.lrreport();
            int i = 0;

            xy = new double[5+1, 1+1];
            for(i=0; i<=5; i++)
            {
                xy[i,0] = 0;
                xy[i,1] = 0;
            }
            linreg.lrbuild(ref xy, 6, 1, ref info, ref lr, ref rep);
            System.Diagnostics.Debug.Assert(info>0);
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testregressunit_test_silent()
        {
            bool result = new bool();

            result = testlinregression(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testregressunit_test()
        {
            bool result = new bool();

            result = testlinregression(false);
            return result;
        }
    }
}
