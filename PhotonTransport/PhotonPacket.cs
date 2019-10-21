using System;
using System.Collections.Generic;
using System.Text;

namespace PhotonTransport
{
    public class PhotonPacket
    {
        public double x, y, z; /* Cartesian coordinates.[cm] */
        public double ux, uy, uz;/* directional cosines of a photon. */
        public double w;           /* weight. */
        public bool dead;       /* 1 if photon is terminated. */
        public short layer;        /* index to layer where the photon */
                                   /* packet resides. */
        public double s;           /* current step size. [cm]. */
        public double sleft;
    }
}
