using System.Collections.Generic;
using System.Xml.Serialization;

namespace PhotonTransport
{
    public static class LayerProperties
    {
        public static List<Layers> layerList = new List<Layers>();
        public struct Layers
        {
            public double z0, z1; /* z coordinates of a layer. [cm] */
            public double n; /* refractive index of a layer. */
            public double mua; /* absorption coefficient. [1/cm] */
            public double mus; /* scattering coefficient. [1/cm] */
            public double g; /* anisotropy. */

            public double cos_crit0, cos_crit1;
        }

        public static Layers newLayer0 = new Layers();
        public static Layers newLayer1 = new Layers();
        public static Layers newLayer2 = new Layers();
            
        

        public static void InitList()
        {
            layerList.Add(newLayer0);
            layerList.Add(newLayer1);
            layerList.Add(newLayer2);
        }
    }

}

