using System;
using System.Collections.Generic;
using System.Text;

namespace PhotonTransport
{
    public class InputParametersIndependentRun
    {
        public long num_photons;       /* to be traced. */
        public double Wth;                 /* play roulette if photon */
        /* weight < Wth.*/

        public double dz;              /* z grid separation.[cm] */
        public double dr;              /* r grid separation.[cm] */
        public double da;              /* alpha grid separation. */
        /* [radian] */
        public short nz;                   /* array range 0..nz-1. */
        public short nr;                   /* array range 0..nr-1. */
        public short na;                   /* array range 0..na-1. */

        public short num_layers;           /* number of layers. */
    }
}
