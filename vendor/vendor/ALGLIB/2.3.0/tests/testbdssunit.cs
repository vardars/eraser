
using System;

namespace alglib
{
    public class testbdssunit
    {
        /*************************************************************************
        Testing BDSS operations
        *************************************************************************/
        public static bool testbdss(bool silent)
        {
            bool result = new bool();
            int n = 0;
            int i = 0;
            int j = 0;
            int pass = 0;
            int passcount = 0;
            int maxn = 0;
            int maxnq = 0;
            double[] a = new double[0];
            double[] a0 = new double[0];
            double[] at = new double[0];
            double[,] p = new double[0,0];
            double[] thresholds = new double[0];
            int ni = 0;
            int[] c = new int[0];
            int[] p1 = new int[0];
            int[] p2 = new int[0];
            int[] ties = new int[0];
            int[] pt1 = new int[0];
            int[] pt2 = new int[0];
            int tiecount = 0;
            int c1 = 0;
            int c0 = 0;
            int nc = 0;
            double[] tmp = new double[0];
            double pal = 0;
            double pbl = 0;
            double par = 0;
            double pbr = 0;
            double cve = 0;
            double cvr = 0;
            int info = 0;
            double threshold = 0;
            int[] tiebuf = new int[0];
            int[] cntbuf = new int[0];
            double rms = 0;
            double cvrms = 0;
            bool waserrors = new bool();
            bool tieserrors = new bool();
            bool split2errors = new bool();
            bool optimalsplitkerrors = new bool();
            bool splitkerrors = new bool();

            waserrors = false;
            tieserrors = false;
            split2errors = false;
            splitkerrors = false;
            optimalsplitkerrors = false;
            maxn = 100;
            maxnq = 49;
            passcount = 10;
            
            //
            // Test ties
            //
            for(n=1; n<=maxn; n++)
            {
                for(pass=1; pass<=passcount; pass++)
                {
                    
                    //
                    // untied data, test DSTie
                    //
                    unset1di(ref p1);
                    unset1di(ref p2);
                    unset1di(ref pt1);
                    unset1di(ref pt2);
                    a = new double[n-1+1];
                    a0 = new double[n-1+1];
                    at = new double[n-1+1];
                    tmp = new double[n-1+1];
                    a[0] = 2*AP.Math.RandomReal()-1;
                    tmp[0] = AP.Math.RandomReal();
                    for(i=1; i<=n-1; i++)
                    {
                        
                        //
                        // A is randomly permuted
                        //
                        a[i] = a[i-1]+0.1*AP.Math.RandomReal()+0.1;
                        tmp[i] = AP.Math.RandomReal();
                    }
                    tsort.tagsortfastr(ref tmp, ref a, n);
                    for(i=0; i<=n-1; i++)
                    {
                        a0[i] = a[i];
                        at[i] = a[i];
                    }
                    bdss.dstie(ref a0, n, ref ties, ref tiecount, ref p1, ref p2);
                    tsort.tagsort(ref at, n, ref pt1, ref pt2);
                    for(i=0; i<=n-1; i++)
                    {
                        tieserrors = tieserrors | p1[i]!=pt1[i];
                        tieserrors = tieserrors | p2[i]!=pt2[i];
                    }
                    tieserrors = tieserrors | tiecount!=n;
                    if( tiecount==n )
                    {
                        for(i=0; i<=n; i++)
                        {
                            tieserrors = tieserrors | ties[i]!=i;
                        }
                    }
                    
                    //
                    // tied data, test DSTie
                    //
                    unset1di(ref p1);
                    unset1di(ref p2);
                    unset1di(ref pt1);
                    unset1di(ref pt2);
                    a = new double[n-1+1];
                    a0 = new double[n-1+1];
                    at = new double[n-1+1];
                    c1 = 0;
                    c0 = 0;
                    for(i=0; i<=n-1; i++)
                    {
                        a[i] = AP.Math.RandomInteger(2);
                        if( (double)(a[i])==(double)(0) )
                        {
                            c0 = c0+1;
                        }
                        else
                        {
                            c1 = c1+1;
                        }
                        a0[i] = a[i];
                        at[i] = a[i];
                    }
                    bdss.dstie(ref a0, n, ref ties, ref tiecount, ref p1, ref p2);
                    tsort.tagsort(ref at, n, ref pt1, ref pt2);
                    for(i=0; i<=n-1; i++)
                    {
                        tieserrors = tieserrors | p1[i]!=pt1[i];
                        tieserrors = tieserrors | p2[i]!=pt2[i];
                    }
                    if( c0==0 | c1==0 )
                    {
                        tieserrors = tieserrors | tiecount!=1;
                        if( tiecount==1 )
                        {
                            tieserrors = tieserrors | ties[0]!=0;
                            tieserrors = tieserrors | ties[1]!=n;
                        }
                    }
                    else
                    {
                        tieserrors = tieserrors | tiecount!=2;
                        if( tiecount==2 )
                        {
                            tieserrors = tieserrors | ties[0]!=0;
                            tieserrors = tieserrors | ties[1]!=c0;
                            tieserrors = tieserrors | ties[2]!=n;
                        }
                    }
                }
            }
            
            //
            // split-2
            //
            
            //
            // General tests for different N's
            //
            for(n=1; n<=maxn; n++)
            {
                a = new double[n-1+1];
                c = new int[n-1+1];
                
                //
                // one-tie test
                //
                if( n%2==0 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        a[i] = n;
                        c[i] = i%2;
                    }
                    bdss.dsoptimalsplit2(a, c, n, ref info, ref threshold, ref pal, ref pbl, ref par, ref pbr, ref cve);
                    if( info!=-3 )
                    {
                        split2errors = true;
                        continue;
                    }
                }
                
                //
                // two-tie test
                //
                
                //
                // test #1
                //
                if( n>1 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        a[i] = i/((n+1)/2);
                        c[i] = i/((n+1)/2);
                    }
                    bdss.dsoptimalsplit2(a, c, n, ref info, ref threshold, ref pal, ref pbl, ref par, ref pbr, ref cve);
                    if( info!=1 )
                    {
                        split2errors = true;
                        continue;
                    }
                    split2errors = split2errors | (double)(Math.Abs(threshold-0.5))>(double)(100*AP.Math.MachineEpsilon);
                    split2errors = split2errors | (double)(Math.Abs(pal-1))>(double)(100*AP.Math.MachineEpsilon);
                    split2errors = split2errors | (double)(Math.Abs(pbl-0))>(double)(100*AP.Math.MachineEpsilon);
                    split2errors = split2errors | (double)(Math.Abs(par-0))>(double)(100*AP.Math.MachineEpsilon);
                    split2errors = split2errors | (double)(Math.Abs(pbr-1))>(double)(100*AP.Math.MachineEpsilon);
                }
            }
            
            //
            // Special "CREDIT"-test (transparency coefficient)
            //
            n = 110;
            a = new double[n-1+1];
            c = new int[n-1+1];
            a[0] = 0.000;
            c[0] = 0;
            a[1] = 0.000;
            c[1] = 0;
            a[2] = 0.000;
            c[2] = 0;
            a[3] = 0.000;
            c[3] = 0;
            a[4] = 0.000;
            c[4] = 0;
            a[5] = 0.000;
            c[5] = 0;
            a[6] = 0.000;
            c[6] = 0;
            a[7] = 0.000;
            c[7] = 1;
            a[8] = 0.000;
            c[8] = 0;
            a[9] = 0.000;
            c[9] = 1;
            a[10] = 0.000;
            c[10] = 0;
            a[11] = 0.000;
            c[11] = 0;
            a[12] = 0.000;
            c[12] = 0;
            a[13] = 0.000;
            c[13] = 0;
            a[14] = 0.000;
            c[14] = 0;
            a[15] = 0.000;
            c[15] = 0;
            a[16] = 0.000;
            c[16] = 0;
            a[17] = 0.000;
            c[17] = 0;
            a[18] = 0.000;
            c[18] = 0;
            a[19] = 0.000;
            c[19] = 0;
            a[20] = 0.000;
            c[20] = 0;
            a[21] = 0.000;
            c[21] = 0;
            a[22] = 0.000;
            c[22] = 1;
            a[23] = 0.000;
            c[23] = 0;
            a[24] = 0.000;
            c[24] = 0;
            a[25] = 0.000;
            c[25] = 0;
            a[26] = 0.000;
            c[26] = 0;
            a[27] = 0.000;
            c[27] = 1;
            a[28] = 0.000;
            c[28] = 0;
            a[29] = 0.000;
            c[29] = 1;
            a[30] = 0.000;
            c[30] = 0;
            a[31] = 0.000;
            c[31] = 1;
            a[32] = 0.000;
            c[32] = 0;
            a[33] = 0.000;
            c[33] = 1;
            a[34] = 0.000;
            c[34] = 0;
            a[35] = 0.030;
            c[35] = 0;
            a[36] = 0.030;
            c[36] = 0;
            a[37] = 0.050;
            c[37] = 0;
            a[38] = 0.070;
            c[38] = 1;
            a[39] = 0.110;
            c[39] = 0;
            a[40] = 0.110;
            c[40] = 1;
            a[41] = 0.120;
            c[41] = 0;
            a[42] = 0.130;
            c[42] = 0;
            a[43] = 0.140;
            c[43] = 0;
            a[44] = 0.140;
            c[44] = 0;
            a[45] = 0.140;
            c[45] = 0;
            a[46] = 0.150;
            c[46] = 0;
            a[47] = 0.150;
            c[47] = 0;
            a[48] = 0.170;
            c[48] = 0;
            a[49] = 0.190;
            c[49] = 1;
            a[50] = 0.200;
            c[50] = 0;
            a[51] = 0.200;
            c[51] = 0;
            a[52] = 0.250;
            c[52] = 0;
            a[53] = 0.250;
            c[53] = 0;
            a[54] = 0.260;
            c[54] = 0;
            a[55] = 0.270;
            c[55] = 0;
            a[56] = 0.280;
            c[56] = 0;
            a[57] = 0.310;
            c[57] = 0;
            a[58] = 0.310;
            c[58] = 0;
            a[59] = 0.330;
            c[59] = 0;
            a[60] = 0.330;
            c[60] = 0;
            a[61] = 0.340;
            c[61] = 0;
            a[62] = 0.340;
            c[62] = 0;
            a[63] = 0.370;
            c[63] = 0;
            a[64] = 0.380;
            c[64] = 1;
            a[65] = 0.380;
            c[65] = 0;
            a[66] = 0.410;
            c[66] = 0;
            a[67] = 0.460;
            c[67] = 0;
            a[68] = 0.520;
            c[68] = 0;
            a[69] = 0.530;
            c[69] = 0;
            a[70] = 0.540;
            c[70] = 0;
            a[71] = 0.560;
            c[71] = 0;
            a[72] = 0.560;
            c[72] = 0;
            a[73] = 0.570;
            c[73] = 0;
            a[74] = 0.600;
            c[74] = 0;
            a[75] = 0.600;
            c[75] = 0;
            a[76] = 0.620;
            c[76] = 0;
            a[77] = 0.650;
            c[77] = 0;
            a[78] = 0.660;
            c[78] = 0;
            a[79] = 0.680;
            c[79] = 0;
            a[80] = 0.700;
            c[80] = 0;
            a[81] = 0.750;
            c[81] = 0;
            a[82] = 0.770;
            c[82] = 0;
            a[83] = 0.770;
            c[83] = 0;
            a[84] = 0.770;
            c[84] = 0;
            a[85] = 0.790;
            c[85] = 0;
            a[86] = 0.810;
            c[86] = 0;
            a[87] = 0.840;
            c[87] = 0;
            a[88] = 0.860;
            c[88] = 0;
            a[89] = 0.870;
            c[89] = 0;
            a[90] = 0.890;
            c[90] = 0;
            a[91] = 0.900;
            c[91] = 1;
            a[92] = 0.900;
            c[92] = 0;
            a[93] = 0.910;
            c[93] = 0;
            a[94] = 0.940;
            c[94] = 0;
            a[95] = 0.950;
            c[95] = 0;
            a[96] = 0.952;
            c[96] = 0;
            a[97] = 0.970;
            c[97] = 0;
            a[98] = 0.970;
            c[98] = 0;
            a[99] = 0.980;
            c[99] = 0;
            a[100] = 1.000;
            c[100] = 0;
            a[101] = 1.000;
            c[101] = 0;
            a[102] = 1.000;
            c[102] = 0;
            a[103] = 1.000;
            c[103] = 0;
            a[104] = 1.000;
            c[104] = 0;
            a[105] = 1.020;
            c[105] = 0;
            a[106] = 1.090;
            c[106] = 0;
            a[107] = 1.130;
            c[107] = 0;
            a[108] = 1.840;
            c[108] = 0;
            a[109] = 2.470;
            c[109] = 0;
            bdss.dsoptimalsplit2(a, c, n, ref info, ref threshold, ref pal, ref pbl, ref par, ref pbr, ref cve);
            if( info!=1 )
            {
                split2errors = true;
            }
            else
            {
                split2errors = split2errors | (double)(Math.Abs(threshold-0.195))>(double)(100*AP.Math.MachineEpsilon);
                split2errors = split2errors | (double)(Math.Abs(pal-0.80))>(double)(0.02);
                split2errors = split2errors | (double)(Math.Abs(pbl-0.20))>(double)(0.02);
                split2errors = split2errors | (double)(Math.Abs(par-0.97))>(double)(0.02);
                split2errors = split2errors | (double)(Math.Abs(pbr-0.03))>(double)(0.02);
            }
            
            //
            // split-2 fast
            //
            
            //
            // General tests for different N's
            //
            for(n=1; n<=maxn; n++)
            {
                a = new double[n-1+1];
                c = new int[n-1+1];
                tiebuf = new int[n+1];
                cntbuf = new int[3+1];
                
                //
                // one-tie test
                //
                if( n%2==0 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        a[i] = n;
                        c[i] = i%2;
                    }
                    bdss.dsoptimalsplit2fast(ref a, ref c, ref tiebuf, ref cntbuf, n, 2, 0.00, ref info, ref threshold, ref rms, ref cvrms);
                    if( info!=-3 )
                    {
                        split2errors = true;
                        continue;
                    }
                }
                
                //
                // two-tie test
                //
                
                //
                // test #1
                //
                if( n>1 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        a[i] = i/((n+1)/2);
                        c[i] = i/((n+1)/2);
                    }
                    bdss.dsoptimalsplit2fast(ref a, ref c, ref tiebuf, ref cntbuf, n, 2, 0.00, ref info, ref threshold, ref rms, ref cvrms);
                    if( info!=1 )
                    {
                        split2errors = true;
                        continue;
                    }
                    split2errors = split2errors | (double)(Math.Abs(threshold-0.5))>(double)(100*AP.Math.MachineEpsilon);
                    split2errors = split2errors | (double)(Math.Abs(rms-0))>(double)(100*AP.Math.MachineEpsilon);
                    if( n==2 )
                    {
                        split2errors = split2errors | (double)(Math.Abs(cvrms-0.5))>(double)(100*AP.Math.MachineEpsilon);
                    }
                    else
                    {
                        if( n==3 )
                        {
                            split2errors = split2errors | (double)(Math.Abs(cvrms-Math.Sqrt((2*0+2*0+2*0.25)/6)))>(double)(100*AP.Math.MachineEpsilon);
                        }
                        else
                        {
                            split2errors = split2errors | (double)(Math.Abs(cvrms))>(double)(100*AP.Math.MachineEpsilon);
                        }
                    }
                }
            }
            
            //
            // special tests
            //
            n = 10;
            a = new double[n-1+1];
            c = new int[n-1+1];
            tiebuf = new int[n+1];
            cntbuf = new int[2*3-1+1];
            for(i=0; i<=n-1; i++)
            {
                a[i] = i;
                if( i<=n-3 )
                {
                    c[i] = 0;
                }
                else
                {
                    c[i] = i-(n-3);
                }
            }
            bdss.dsoptimalsplit2fast(ref a, ref c, ref tiebuf, ref cntbuf, n, 3, 0.00, ref info, ref threshold, ref rms, ref cvrms);
            if( info!=1 )
            {
                split2errors = true;
            }
            else
            {
                split2errors = split2errors | (double)(Math.Abs(threshold-(n-2.5)))>(double)(100*AP.Math.MachineEpsilon);
                split2errors = split2errors | (double)(Math.Abs(rms-Math.Sqrt((0.25+0.25+0.25+0.25)/(3*n))))>(double)(100*AP.Math.MachineEpsilon);
                split2errors = split2errors | (double)(Math.Abs(cvrms-Math.Sqrt(((double)(1+1+1+1))/((double)(3*n)))))>(double)(100*AP.Math.MachineEpsilon);
            }
            
            //
            // Optimal split-K
            //
            
            //
            // General tests for different N's
            //
            for(n=1; n<=maxnq; n++)
            {
                a = new double[n-1+1];
                c = new int[n-1+1];
                
                //
                // one-tie test
                //
                if( n%2==0 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        a[i] = pass;
                        c[i] = i%2;
                    }
                    bdss.dsoptimalsplitk(a, c, n, 2, 2+AP.Math.RandomInteger(5), ref info, ref thresholds, ref ni, ref cve);
                    if( info!=-3 )
                    {
                        optimalsplitkerrors = true;
                        continue;
                    }
                }
                
                //
                // two-tie test
                //
                
                //
                // test #1
                //
                if( n>1 )
                {
                    c0 = 0;
                    c1 = 0;
                    for(i=0; i<=n-1; i++)
                    {
                        a[i] = i/((n+1)/2);
                        c[i] = i/((n+1)/2);
                        if( c[i]==0 )
                        {
                            c0 = c0+1;
                        }
                        if( c[i]==1 )
                        {
                            c1 = c1+1;
                        }
                    }
                    bdss.dsoptimalsplitk(a, c, n, 2, 2+AP.Math.RandomInteger(5), ref info, ref thresholds, ref ni, ref cve);
                    if( info!=1 )
                    {
                        optimalsplitkerrors = true;
                        continue;
                    }
                    optimalsplitkerrors = optimalsplitkerrors | ni!=2;
                    optimalsplitkerrors = optimalsplitkerrors | (double)(Math.Abs(thresholds[0]-0.5))>(double)(100*AP.Math.MachineEpsilon);
                    optimalsplitkerrors = optimalsplitkerrors | (double)(Math.Abs(cve-(-(c0*Math.Log((double)(c0)/((double)(c0+1))))-c1*Math.Log((double)(c1)/((double)(c1+1))))))>(double)(100*AP.Math.MachineEpsilon);
                }
                
                //
                // test #2
                //
                if( n>2 )
                {
                    c0 = 1+AP.Math.RandomInteger(n-1);
                    c1 = n-c0;
                    for(i=0; i<=n-1; i++)
                    {
                        if( i<c0 )
                        {
                            a[i] = 0;
                            c[i] = 0;
                        }
                        else
                        {
                            a[i] = 1;
                            c[i] = 1;
                        }
                    }
                    bdss.dsoptimalsplitk(a, c, n, 2, 2+AP.Math.RandomInteger(5), ref info, ref thresholds, ref ni, ref cve);
                    if( info!=1 )
                    {
                        optimalsplitkerrors = true;
                        continue;
                    }
                    optimalsplitkerrors = optimalsplitkerrors | ni!=2;
                    optimalsplitkerrors = optimalsplitkerrors | (double)(Math.Abs(thresholds[0]-0.5))>(double)(100*AP.Math.MachineEpsilon);
                    optimalsplitkerrors = optimalsplitkerrors | (double)(Math.Abs(cve-(-(c0*Math.Log((double)(c0)/((double)(c0+1))))-c1*Math.Log((double)(c1)/((double)(c1+1))))))>(double)(100*AP.Math.MachineEpsilon);
                }
                
                //
                // multi-tie test
                //
                if( n>=16 )
                {
                    
                    //
                    // Multi-tie test.
                    //
                    // First NC-1 ties have C0 entries, remaining NC-th tie
                    // have C1 entries.
                    //
                    nc = (int)Math.Round(Math.Sqrt(n));
                    c0 = n/nc;
                    c1 = n-c0*(nc-1);
                    for(i=0; i<=nc-2; i++)
                    {
                        for(j=c0*i; j<=c0*(i+1)-1; j++)
                        {
                            a[j] = j;
                            c[j] = i;
                        }
                    }
                    for(j=c0*(nc-1); j<=n-1; j++)
                    {
                        a[j] = j;
                        c[j] = nc-1;
                    }
                    bdss.dsoptimalsplitk(a, c, n, nc, nc+AP.Math.RandomInteger(nc), ref info, ref thresholds, ref ni, ref cve);
                    if( info!=1 )
                    {
                        optimalsplitkerrors = true;
                        continue;
                    }
                    optimalsplitkerrors = optimalsplitkerrors | ni!=nc;
                    if( ni==nc )
                    {
                        for(i=0; i<=nc-2; i++)
                        {
                            optimalsplitkerrors = optimalsplitkerrors | (double)(Math.Abs(thresholds[i]-(c0*(i+1)-1+0.5)))>(double)(100*AP.Math.MachineEpsilon);
                        }
                        cvr = -((nc-1)*c0*Math.Log((double)(c0)/((double)(c0+nc-1)))+c1*Math.Log((double)(c1)/((double)(c1+nc-1))));
                        optimalsplitkerrors = optimalsplitkerrors | (double)(Math.Abs(cve-cvr))>(double)(100*AP.Math.MachineEpsilon);
                    }
                }
            }
            
            //
            // Non-optimal split-K
            //
            
            //
            // General tests for different N's
            //
            for(n=1; n<=maxnq; n++)
            {
                a = new double[n-1+1];
                c = new int[n-1+1];
                
                //
                // one-tie test
                //
                if( n%2==0 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        a[i] = pass;
                        c[i] = i%2;
                    }
                    bdss.dssplitk(a, c, n, 2, 2+AP.Math.RandomInteger(5), ref info, ref thresholds, ref ni, ref cve);
                    if( info!=-3 )
                    {
                        splitkerrors = true;
                        continue;
                    }
                }
                
                //
                // two-tie test
                //
                
                //
                // test #1
                //
                if( n>1 )
                {
                    c0 = 0;
                    c1 = 0;
                    for(i=0; i<=n-1; i++)
                    {
                        a[i] = i/((n+1)/2);
                        c[i] = i/((n+1)/2);
                        if( c[i]==0 )
                        {
                            c0 = c0+1;
                        }
                        if( c[i]==1 )
                        {
                            c1 = c1+1;
                        }
                    }
                    bdss.dssplitk(a, c, n, 2, 2+AP.Math.RandomInteger(5), ref info, ref thresholds, ref ni, ref cve);
                    if( info!=1 )
                    {
                        splitkerrors = true;
                        continue;
                    }
                    splitkerrors = splitkerrors | ni!=2;
                    if( ni==2 )
                    {
                        splitkerrors = splitkerrors | (double)(Math.Abs(thresholds[0]-0.5))>(double)(100*AP.Math.MachineEpsilon);
                        splitkerrors = splitkerrors | (double)(Math.Abs(cve-(-(c0*Math.Log((double)(c0)/((double)(c0+1))))-c1*Math.Log((double)(c1)/((double)(c1+1))))))>(double)(100*AP.Math.MachineEpsilon);
                    }
                }
                
                //
                // test #2
                //
                if( n>2 )
                {
                    c0 = 1+AP.Math.RandomInteger(n-1);
                    c1 = n-c0;
                    for(i=0; i<=n-1; i++)
                    {
                        if( i<c0 )
                        {
                            a[i] = 0;
                            c[i] = 0;
                        }
                        else
                        {
                            a[i] = 1;
                            c[i] = 1;
                        }
                    }
                    bdss.dssplitk(a, c, n, 2, 2+AP.Math.RandomInteger(5), ref info, ref thresholds, ref ni, ref cve);
                    if( info!=1 )
                    {
                        splitkerrors = true;
                        continue;
                    }
                    splitkerrors = splitkerrors | ni!=2;
                    if( ni==2 )
                    {
                        splitkerrors = splitkerrors | (double)(Math.Abs(thresholds[0]-0.5))>(double)(100*AP.Math.MachineEpsilon);
                        splitkerrors = splitkerrors | (double)(Math.Abs(cve-(-(c0*Math.Log((double)(c0)/((double)(c0+1))))-c1*Math.Log((double)(c1)/((double)(c1+1))))))>(double)(100*AP.Math.MachineEpsilon);
                    }
                }
                
                //
                // multi-tie test
                //
                for(c0=4; c0<=n; c0++)
                {
                    if( n%c0==0 & n/c0<=c0 & n/c0>1 )
                    {
                        nc = n/c0;
                        for(i=0; i<=nc-1; i++)
                        {
                            for(j=c0*i; j<=c0*(i+1)-1; j++)
                            {
                                a[j] = j;
                                c[j] = i;
                            }
                        }
                        bdss.dssplitk(a, c, n, nc, nc+AP.Math.RandomInteger(nc), ref info, ref thresholds, ref ni, ref cve);
                        if( info!=1 )
                        {
                            splitkerrors = true;
                            continue;
                        }
                        splitkerrors = splitkerrors | ni!=nc;
                        if( ni==nc )
                        {
                            for(i=0; i<=nc-2; i++)
                            {
                                splitkerrors = splitkerrors | (double)(Math.Abs(thresholds[i]-(c0*(i+1)-1+0.5)))>(double)(100*AP.Math.MachineEpsilon);
                            }
                            cvr = -(nc*c0*Math.Log((double)(c0)/((double)(c0+nc-1))));
                            splitkerrors = splitkerrors | (double)(Math.Abs(cve-cvr))>(double)(100*AP.Math.MachineEpsilon);
                        }
                    }
                }
            }
            
            //
            // report
            //
            waserrors = tieserrors | split2errors | optimalsplitkerrors | splitkerrors;
            if( !silent )
            {
                System.Console.Write("TESTING BASIC DATASET SUBROUTINES");
                System.Console.WriteLine();
                System.Console.Write("TIES:                               ");
                if( !tieserrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("SPLIT-2:                            ");
                if( !split2errors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("OPTIMAL SPLIT-K:                    ");
                if( !optimalsplitkerrors )
                {
                    System.Console.Write("OK");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("FAILED");
                    System.Console.WriteLine();
                }
                System.Console.Write("SPLIT-K:                            ");
                if( !splitkerrors )
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
                    System.Console.Write("TEST FAILED");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.Write("TEST PASSED");
                    System.Console.WriteLine();
                }
                System.Console.WriteLine();
                System.Console.WriteLine();
            }
            result = !waserrors;
            return result;
        }


        /*************************************************************************
        Unsets 2D array.
        *************************************************************************/
        private static void unset2d(ref AP.Complex[,] a)
        {
            a = new AP.Complex[0+1, 0+1];
            a[0,0] = 2*AP.Math.RandomReal()-1;
        }


        /*************************************************************************
        Unsets 1D array.
        *************************************************************************/
        private static void unset1d(ref double[] a)
        {
            a = new double[0+1];
            a[0] = 2*AP.Math.RandomReal()-1;
        }


        /*************************************************************************
        Unsets 1D array.
        *************************************************************************/
        private static void unset1di(ref int[] a)
        {
            a = new int[0+1];
            a[0] = AP.Math.RandomInteger(3)-1;
        }


        private static void testsortresults(ref double[] asorted,
            ref int[] p1,
            ref int[] p2,
            ref double[] aoriginal,
            int n,
            ref bool waserrors)
        {
            int i = 0;
            double[] a2 = new double[0];
            double t = 0;
            int[] f = new int[0];

            a2 = new double[n-1+1];
            f = new int[n-1+1];
            
            //
            // is set ordered?
            //
            for(i=0; i<=n-2; i++)
            {
                waserrors = waserrors | (double)(asorted[i])>(double)(asorted[i+1]);
            }
            
            //
            // P1 correctness
            //
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors | (double)(asorted[i])!=(double)(aoriginal[p1[i]]);
            }
            for(i=0; i<=n-1; i++)
            {
                f[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                f[p1[i]] = f[p1[i]]+1;
            }
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors | f[i]!=1;
            }
            
            //
            // P2 correctness
            //
            for(i=0; i<=n-1; i++)
            {
                a2[i] = aoriginal[i];
            }
            for(i=0; i<=n-1; i++)
            {
                if( p2[i]!=i )
                {
                    t = a2[i];
                    a2[i] = a2[p2[i]];
                    a2[p2[i]] = t;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors | (double)(asorted[i])!=(double)(a2[i]);
            }
        }


        /*************************************************************************
        Silent unit test
        *************************************************************************/
        public static bool testbdssunit_test_silent()
        {
            bool result = new bool();

            result = testbdss(true);
            return result;
        }


        /*************************************************************************
        Unit test
        *************************************************************************/
        public static bool testbdssunit_test()
        {
            bool result = new bool();

            result = testbdss(false);
            return result;
        }
    }
}
