using System;
using System.Collections.Generic;
using System.Text;

namespace PhotonTransport
{
    public struct LayerProperties
    {
        public double z0, z1;  /* z coordinates of a layer. [cm] */
        public double n;           /* refractive index of a layer. */
        public double mua;     /* absorption coefficient. [1/cm] */
        public double mus;     /* scattering coefficient. [1/cm] */
        public double g;           /* anisotropy. */

        public double cos_crit0, cos_crit1;
    }
}
