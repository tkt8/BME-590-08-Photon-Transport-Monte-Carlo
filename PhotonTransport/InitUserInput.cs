using System;
using System.Collections.Generic;
using System.Text;

namespace PhotonTransport
{
    public  static class InitUserInput
    {
        public static void InitializeVariables(List<string> list1)
        {
            GridParametersClass.num_photons = Convert.ToInt64(list1[0]);
            GridParametersClass.dr= Convert.ToDouble(list1[1]);
            GridParametersClass.dz= Convert.ToDouble(list1[2]);
            GridParametersClass.nz = Convert.ToInt16(list1[3]);
            GridParametersClass.nr = Convert.ToInt16(list1[4]);
            GridParametersClass.na = Convert.ToInt16(list1[5]);
            GridParametersClass.da = 0.5 * Math.PI / GridParametersClass.na;
            GridParametersClass.num_layers = 3;
            GridParametersClass.Wth = 1E-4;
        }


        public static void SetLayer1(List<string> listLayer)
        {
            LayerProperties.newLayer0.n = Convert.ToDouble(listLayer[0]);
            LayerProperties.newLayer0.mua = Convert.ToDouble(listLayer[1]);
            LayerProperties.newLayer0.mus = Convert.ToDouble(listLayer[2]);
            LayerProperties.newLayer0.g = Convert.ToDouble(listLayer[3]);
            LayerProperties.newLayer0.z0 = 0.0;
            LayerProperties.newLayer0.z1 = LayerProperties.newLayer0.z0 + Convert.ToDouble(listLayer[4]);

        }

        public static void SetLayer2(List<string> listLayer)
        {
            LayerProperties.newLayer1.n = Convert.ToDouble(listLayer[0]);
            LayerProperties.newLayer1.mua = Convert.ToDouble(listLayer[1]);
            LayerProperties.newLayer1.mus = Convert.ToDouble(listLayer[2]);
            LayerProperties.newLayer1.g = Convert.ToDouble(listLayer[3]);
            LayerProperties.newLayer1.z0 = 0.0;
            LayerProperties.newLayer1.z1 = LayerProperties.newLayer0.z0 + Convert.ToDouble(listLayer[4]);
        }

        public static void SetLayer3(List<string> listLayer)
        {
            LayerProperties.newLayer2.n = Convert.ToDouble(listLayer[0]);
            LayerProperties.newLayer2.mua = Convert.ToDouble(listLayer[1]);
            LayerProperties.newLayer2.mus = Convert.ToDouble(listLayer[2]);
            LayerProperties.newLayer2.g = Convert.ToDouble(listLayer[3]);
            LayerProperties.newLayer2.z0 = 0.0;
            LayerProperties.newLayer2.z1 = LayerProperties.newLayer0.z0 + Convert.ToDouble(listLayer[4]);
        }


        public static void CalculateCriticalAngleLayer1()
        {
            var r = Scoring.A;
           var n1 = LayerProperties.newLayer0.n;
           var n2 = 1.0; //ambient

           LayerProperties.newLayer0.cos_crit0 = n1 > n2 ? Math.Sqrt(1.0 - n2 * n2 / (n1 * n1)) : 0.0;

           n2 = LayerProperties.newLayer1.n;
           LayerProperties.newLayer0.cos_crit1 = n1 > n2 ? Math.Sqrt(1.0 - n2 * n2 / (n1 * n1)) : 0.0;
        }

        public static void CalculateCriticalAngleLayer2()
        {
            var n1 = LayerProperties.newLayer1.n;
            var n2 = LayerProperties.newLayer0.n;

            LayerProperties.newLayer1.cos_crit0 = n1 > n2 ? Math.Sqrt(1.0 - n2 * n2 / (n1 * n1)) : 0.0;

            n2 = LayerProperties.newLayer0.n;
            LayerProperties.newLayer1.cos_crit1 = n1 > n2 ? Math.Sqrt(1.0 - n2 * n2 / (n1 * n1)) : 0.0;
        }

        public static void CalculateCriticalAngleLayer3()
        {
            var n1 = LayerProperties.newLayer2.n;
            var n2 = LayerProperties.newLayer1.n;

            LayerProperties.newLayer2.cos_crit0 = n1 > n2 ? Math.Sqrt(1.0 - n2 * n2 / (n1 * n1)) : 0.0;

            n2 = LayerProperties.newLayer1.n;
            LayerProperties.newLayer2.cos_crit1 = n1 > n2 ? Math.Sqrt(1.0 - n2 * n2 / (n1 * n1)) : 0.0;
        }
    }
}

