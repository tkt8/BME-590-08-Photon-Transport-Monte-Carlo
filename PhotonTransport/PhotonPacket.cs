namespace PhotonTransport
{
    public static class PhotonPacket
    {
        public static double x, y, z; /* Cartesian coordinates.[cm] */
        public static double ux, uy, uz;/* directional cosines of a photon. */
        public static double w;           /* weight. */
        public static bool dead;       /* 1 if photon is terminated. */
        public static short layer;        /* index to layer where the photon */
                                          /* packet resides. */
        public static double s;           /* current step size. [cm]. */
        public static double sleft;
    }
}
