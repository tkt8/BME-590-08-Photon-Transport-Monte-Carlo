using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace PhotonTransport
{
    public class Transport
    {

        public double Rspecular;
        public PhotonPacket newPhoton = new PhotonPacket();
        public LayerProperties newLayer0 = new LayerProperties();
        public LayerProperties newLayer1 = new LayerProperties();
        public LayerProperties newLayer2 = new LayerProperties();
        public Scoring score = new Scoring();
        public InputParametersIndependentRun newInput = new InputParametersIndependentRun();

        public List<LayerProperties> layerList = new List<LayerProperties>();

        Transport()
        {
            layerList.Add(newLayer0);
            layerList.Add(newLayer1);
            layerList.Add(newLayer2);
        }

        public double CalculateRspecular()
        {
            var temp = (layerList[0].n - layerList[1].n) / (layerList[0].n + layerList[1].n);
            var rsp1 = temp * temp;
            Rspecular = rsp1;
            if (layerList[1].mua == 0.0 && layerList[1].mus == 0.0)
            {
                temp = (layerList[1].n - layerList[2].n) / (layerList[1].n + layerList[2].n);
                var rsp2 = temp * temp;

                var rsp = rsp1 + ((Math.Pow(1 - rsp1, 2)) * rsp2) / (1 - rsp1 * rsp2);
                Rspecular = rsp;
            }

            return Rspecular;
        }
        public void LaunchPhoton()
        {
            newPhoton.w = 1.0 - Rspecular;
            newPhoton.dead = false;
            newPhoton.layer = 1;
            newPhoton.s = 0;
            newPhoton.sleft = 0;

            newPhoton.x = 0.0;
            newPhoton.y = 0.0;
            newPhoton.z = 0.0;
            newPhoton.ux = 0.0;
            newPhoton.uy = 0.0;
            newPhoton.uz = 1.0;

            if (layerList[1].mua == 0.0 && layerList[1].mus == 0.0)
            {
                //glass 
                newPhoton.layer = 2;
                newPhoton.z = layerList[2].z0;
            }
        }


        public void StepSizeInGlass()
        {
            double dl_b;
            if (newPhoton.uz > 0.0)
                dl_b = (layerList[newPhoton.layer].z1 - newPhoton.z)/ newPhoton.uz;
            else if (newPhoton.uz < 0.0)
                dl_b = (layerList[newPhoton.layer].z0 - newPhoton.z)/ newPhoton.uz;
            else
                dl_b = 0.0;

            newPhoton.s = dl_b;

        }

        public void StepSizeInTissue()
        {
            var mua = layerList[newPhoton.layer].mua;
            var mus = layerList[newPhoton.layer].mus;

            if(newPhoton.sleft == 0.0)
            {
                //Make new Step
                var rnd = GenerateRandomNumber();
                while (rnd <= 0.0)
                    newPhoton.s = -Math.Log(rnd) / (mua + mus);
            }
            else
            {
                newPhoton.s = newPhoton.sleft / (mua + mus);
                newPhoton.sleft = 0.0;
            }

        }

        public double GenerateRandomNumber()
        {
            var rnd = new Random();
            var num = rnd.NextDouble();
            return num;
        }

        public void CrossUpOrNot()
        {
            var uz = newPhoton.uz;
            var uz1 = 0.0;
            var reflectance1 = 0.0;
            var layer = newPhoton.layer;
            var ni = layerList[layer].n;
            var nt = layerList[layer - 1].n;

            if (-uz <= layerList[layer].cos_crit0)
                reflectance1 = 1.0;     //total internal reflection

            else
                (reflectance1,uz1) = RFresnel(ni, nt, -uz);

            
            if(layer ==1 && reflectance1 < 1.0)
            {
                newPhoton.uz = -uz1;
                RecordR(reflectance1);
                newPhoton.uz = -uz;
            }
            else if (GenerateRandomNumber()> reflectance1)
            {
                newPhoton.layer--;
                newPhoton.ux *= ni / nt;
                newPhoton.uy *= ni / nt;
                newPhoton.uz = -uz1;
            }
            else
            {
                newPhoton.uz = -uz;
            }


            if (GenerateRandomNumber() > reflectance1)
            {
                if (layer == 1)
                {
                    newPhoton.uz = -uz1;
                    RecordR(0.0);
                    newPhoton.dead = true;
                }

                else
                {
                    newPhoton.layer--;
                    newPhoton.ux *= ni / nt;
                    newPhoton.uy *= ni / nt;
                    newPhoton.uz = -uz1;
                }

            }
            else
                newPhoton.uz = -uz;
        }


        public void CrossDownOrNot()
        {
            var uz = newPhoton.uz;
            var uz1 = 0.0;
            var reflectance1 = 0.0;
            var layer = newPhoton.layer;
            var ni = layerList[layer].n;
            var nt = layerList[layer + 1].n;

            if (uz <= layerList[layer].cos_crit1)
                reflectance1 = 1.0;
            else
                (reflectance1,uz1) = RFresnel(ni, nt, uz);

            if (layer == newInput.num_layers && reflectance1 < 1.0)
            {
                newPhoton.uz = uz1;
                RecordT(reflectance1);
                newPhoton.uz = -uz;
            }
            else if (GenerateRandomNumber() > reflectance1)
            {
                newPhoton.layer++;
                newPhoton.ux *= ni / nt;
                newPhoton.uy *= ni / nt;
                newPhoton.uz = uz1;
            }
            else
            {
                newPhoton.uz = -uz;
            }

            if (GenerateRandomNumber() > reflectance1)
            {
                if (layer == newInput.num_layers)
                {
                    newPhoton.uz = uz1;
                    RecordT(0.0);
                    newPhoton.dead = true;
                }
                else
                {
                    newPhoton.layer++;
                    newPhoton.ux *= ni / nt;
                    newPhoton.uy *= ni / nt;
                    newPhoton.uz = uz1;
                }
                
            }
            else
            {
                newPhoton.uz = -uz;
            }
        }

        public void CrossOrNot()
        {
            if(newPhoton.uz <0.0)
                CrossUpOrNot();
            else
                CrossDownOrNot();
        }
        private void RecordT(double reflectance1)
        {
            //var x = newPhoton.x;
            //var y = newPhoton.y;
            //double ir, ia;

            //var ird = Math.Sqrt(x * x + y * y) / newInput.dr;
            //if (ird > newInput.nr - 1)
            //    ir = newInput.nr - 1;
            //else
            //    ir = ird;

            //var iad = Math.Acos(newPhoton.uz) / newInput.da;
            //if (iad > newInput.na - 1)
            //    ia = newInput.na - 1;
            //else
            //    ia = iad;

            //score.Tt_ra[ir][ia] += newPhoton.w*(1.0 -reflectance1);
            //newPhoton.w *= reflectance1;
        }

        private void RecordR(double reflectance1)
        {
            //var x = newPhoton.x;
            //var y = newPhoton.y;
            //double ir, ia;

            //var ird = Math.Sqrt(x * x + y * y) / newInput.dr;
            //if (ird > newInput.nr - 1)
            //    ir = newInput.nr - 1;
            //else
            //    ir = ird;

            //var iad = Math.Acos(newPhoton.uz) / newInput.da;
            //if (iad > newInput.na - 1)
            //    ia = newInput.na - 1;
            //else
            //    ia = iad;

            //score.Rd_ra[ir][ia] += newPhoton.w*(1.0 - reflectance1);
            //newPhoton.w *= reflectance1;
        }

        private (double,double) RFresnel(double n1, double n2, double ca1)
        {
            double r,uz1;

            if (n1 == n2)
            {
                uz1 = ca1;
                r = 0.0;
            }

            else if (ca1 > Math.Cos(0.0))
            {
                uz1 = ca1;
                r = (n2 - n1) / (n2 + n1);
                r *= r;
            }

            else if (ca1 < Math.Cos(Math.PI / 2))
            {
                uz1 = 0.0;
                r = 1.0;
            }

            else
            {
                var sa1 = Math.Sqrt(1 - ca1 * ca1);
                var sa2 = n1 * sa1 / n2;

                if (sa2 >= 1.0)
                {
                    uz1 = 0.0;
                    r = 1.0;
                }
                else
                {
                    uz1 = Math.Sqrt(1 - sa2 * sa2);
                    var ca2 = uz1;

                    var cap = ca1 * ca2 - sa1* sa2;
                    var cam = ca1 * ca2 + sa1 * sa2;
                    var sap = sa1 * ca2 + ca1 * sa2;
                    var sam = sa1 * ca2 - ca1 * sa2;

                    r = 0.5 * sam * sam * (cam * cam + cap * cap) / (sap * sap * cam * cam);
                }
            }

            return (r, uz1);
        }

        public void HopInGlass()
        {
            if (newPhoton.uz == 0.0)
            {
                newPhoton.dead = true;
            }
            else
            {
                StepSizeInGlass();
                Hop();
                CrossOrNot();
            }
        }

        private void Hop()
        {
            var s = newPhoton.s;


            newPhoton.x += s * newPhoton.ux;
            newPhoton.y += s * newPhoton.uy;
            newPhoton.z += s * newPhoton.uz;
        }

        public void HopDropSpinInTissue()
        {
            StepSizeInTissue();
            if (HitBoundary())
            {
                Hop();
                CrossOrNot();
            }
            else
            {
                Hop();
                Drop();
                Spin(layerList[newPhoton.layer].g);

            }
        }

        public void HopDropSpin()
        {
            var layer = newPhoton.layer;
            if(layerList[layer].mua ==0.0 && layerList[layer].mua ==0.0)
                HopInGlass();
            else
                HopDropSpinInTissue();

            if (newPhoton.w < newInput.Wth && !newPhoton.dead)
                Roulette();
        }

        private void Roulette()
        {
            const double Chance = 0.1;

            if (newPhoton.w == 0.0)
                newPhoton.dead = true;
            else if (GenerateRandomNumber() < Chance)
                newPhoton.w /= Chance;
            else
                newPhoton.dead = true;
        }

        private void Spin(double g)
        {
            var ux = newPhoton.ux;
            var uy = newPhoton.uy;
            var uz = newPhoton.uz;

            var cost = SpinTheta(g);
            var sint = Math.Sqrt(1.0 - cost * cost);

            var psi = 2.0 * Math.PI * GenerateRandomNumber();
            var cosp = Math.Cos(psi);
            double sinp; 
            
            if (psi < Math.PI)
                sinp = Math.Sqrt(1.0 - cosp * cosp);
            else
                sinp = - Math.Sqrt(1.0 - cosp * cosp);

            if (Math.Abs(uz) > Math.Cos(0))
            {
                newPhoton.ux = sint * cosp;
                newPhoton.uy = sint * sinp;
                newPhoton.uz = cost * (uz >= 0 ? 1 : -1);
            }
            else
            {
                var temp = Math.Sqrt(1.0 - uz * uz);
                newPhoton.ux = sint * (ux * uz * cosp - uy * sinp) / temp + ux * cost;
                newPhoton.uy = sint * (uy * uz * cosp + ux * sinp) / temp + uy * cost;
                newPhoton.uz = -sint * cosp * temp + uz * cost;
            }


        }

        private double SpinTheta(double d)
        {
            double cost;
            if (d == 0.0)
                cost = 2 * GenerateRandomNumber() - 1;
            else
            {
                var temp = (1 - d * d) / (1 - d + 2 * d * GenerateRandomNumber());
                cost = (1 + d * d - temp * temp) / (2 * d);
                if (cost < -1)
                    cost = -1;
                else if (cost > 1)
                    cost = 1;
                
            }

            return cost;
        }

        private void Drop()
        {
            var x = newPhoton.x;
            var y = newPhoton.y;
            double iz, ir;

            var layer = newPhoton.layer;

            var izd = newPhoton.z / newInput.dz;
            if (izd > newInput.nz - 1)
                iz = newInput.nz - 1;
            else
                iz = izd;

            var ird = Math.Sqrt(x * x + y * y) / newInput.dr;
            if (ird > newInput.nr - 1)
                ir = newInput.nr - 1;
            else
                ir = ird;

            var mua = layerList[layer].mua;
            var mus = layerList[layer].mus;

            var dwa = newPhoton.w * mua / (mua + mus);
            newPhoton.w -= dwa;

            //score.A_rz[ir][iz] += dwa;
        }

        private bool HitBoundary()
        {
            var layer = newPhoton.layer;
            var uz = newPhoton.uz;
            bool hit;
            double dl_b = 0.0;

            if (uz > 0.0)
                dl_b = (layerList[layer].z1 - newPhoton.z) / uz;
            else if (uz < 0.0)
                dl_b = (layerList[layer].z0 - newPhoton.z) / uz;

            if (uz != 0.0 && newPhoton.s > dl_b)
            {
                var mut = layerList[layer].mua + layerList[layer].mus;
                newPhoton.sleft = (newPhoton.s - dl_b) / mut;
                newPhoton.s = dl_b;
                hit = true;

            }
            else
            {
                hit = false;
            }

            return (hit);
        }
    }

}
