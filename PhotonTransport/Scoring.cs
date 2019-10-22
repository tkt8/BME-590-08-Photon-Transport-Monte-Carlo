using System.Collections.Specialized;
using System.Runtime.InteropServices.ComTypes;

namespace PhotonTransport
{
    public static class Scoring
    {
        public static int Nz = GridParametersClass.nz;
        public static int Nr = GridParametersClass.nr;
        public static int Na = GridParametersClass.na;


        public static double Rsp = 0.0;
        public static double[,] Rd_ra = new double[Nr,Na] ;
        public static double[] Rd_r=new double[Nr];
        public static double[] Rd_a=new double[Na];
        public static double Rd = 0.0;

        public static double[,] A_rz = new double[Nr,Nz];
        public static double[] A_z=new double[Nz];
        public static double[] A_l = new double[GridParametersClass.num_layers];
        public static double A = 0.0;

        public static double[,] Tt_ra= new double[Nr,Na];
        public static double[] Tt_r=new double[Nr];
        public static double[] Tt_a=new double[Na];
        public static double Tt = 0.0;

        
    }
}
