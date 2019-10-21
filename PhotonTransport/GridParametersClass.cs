namespace PhotonTransport
{
    public static class GridParametersClass
    {
        public static long num_photons;       /* to be traced. */
        public static double Wth;                 /* play roulette if photon */
        /* weight < Wth.*/

        public static double dz;              /* z grid separation.[cm] */
        public static double dr;              /* r grid separation.[cm] */
        public static double da;              /* alpha grid separation. */
        /* [radian] */
        public static short nz;                   /* array range 0..nz-1. */
        public static short nr;                   /* array range 0..nr-1. */
        public static short na;                   /* array range 0..na-1. */

        public static short num_layers;           /* number of layers. */
        
    }
}
