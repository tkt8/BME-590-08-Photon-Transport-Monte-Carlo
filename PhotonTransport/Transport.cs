using System;

namespace PhotonTransport
{
    public class Transport
    {

        public double Rspecular;
        
        public double CalculateRspecular()
        {
            var temp = (LayerProperties.layerList[0].n - LayerProperties.layerList[1].n) / (LayerProperties.layerList[0].n + LayerProperties.layerList[1].n);
            var rsp1 = temp * temp;
            Rspecular = rsp1;
            if (LayerProperties.layerList[1].mua == 0.0 && LayerProperties.layerList[1].mus == 0.0)
            {
                temp = (LayerProperties.layerList[1].n - LayerProperties.layerList[2].n) / (LayerProperties.layerList[1].n + LayerProperties.layerList[2].n);
                var rsp2 = temp * temp;

                var rsp = rsp1 + ((Math.Pow(1 - rsp1, 2)) * rsp2) / (1 - rsp1 * rsp2);
                Rspecular = rsp;
            }

            return Rspecular;
        }
        public void LaunchPhoton()
        {
            PhotonPacket.w = 1.0 - Rspecular;
            PhotonPacket.dead = false;
            PhotonPacket.layer = 1;
            PhotonPacket.s = 0;
            PhotonPacket.sleft = 0;

            PhotonPacket.x = 0.0;
            PhotonPacket.y = 0.0;
            PhotonPacket.z = 0.0;
            PhotonPacket.ux = 0.0;
            PhotonPacket.uy = 0.0;
            PhotonPacket.uz = 1.0;

            if (LayerProperties.layerList[1].mua == 0.0 && LayerProperties.layerList[1].mus == 0.0)
            {
                //glass 
                PhotonPacket.layer = 2;
                PhotonPacket.z = LayerProperties.layerList[2].z0;
            }
        }


        public void StepSizeInGlass()
        {
            double dl_b;
            if (PhotonPacket.uz > 0.0)
                dl_b = (LayerProperties.layerList[PhotonPacket.layer-1].z1 - PhotonPacket.z)/ PhotonPacket.uz;
            else if (PhotonPacket.uz < 0.0)
                dl_b = (LayerProperties.layerList[PhotonPacket.layer].z0 - PhotonPacket.z)/ PhotonPacket.uz;
            else
                dl_b = 0.0;

            PhotonPacket.s = dl_b;

        }

        public void StepSizeInTissue()
        {
            var mua = LayerProperties.layerList[PhotonPacket.layer].mua;
            var mus = LayerProperties.layerList[PhotonPacket.layer].mus;

            if(PhotonPacket.sleft == 0.0)
            {
                //Make new Step
                var rnd = GenerateRandomNumber();
                PhotonPacket.s = -Math.Log(rnd) / (mua + mus);
            }
            else
            {
                PhotonPacket.s = PhotonPacket.sleft / (mua + mus);
                PhotonPacket.sleft = 0.0;
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
            var uz = PhotonPacket.uz;
            var uz1 = 0.0;
            var reflectance1 = 0.0;
            var layer = PhotonPacket.layer;
            var ni = LayerProperties.layerList[layer].n;
            var nt = layer>1 ?LayerProperties.layerList[layer - 1].n:1.0;

            if (-uz <= LayerProperties.layerList[layer].cos_crit0)
                reflectance1 = 1.0;     //total internal reflection

            else
                (reflectance1,uz1) = RFresnel(ni, nt, -uz);


            //if (layer == 1 && reflectance1 < 1.0)
            //{
            //    PhotonPacket.uz = -uz1;
            //    RecordR(reflectance1);
            //    PhotonPacket.uz = -uz;
            //}
            //else if (GenerateRandomNumber() > reflectance1)
            //{
            //    PhotonPacket.layer--;
            //    PhotonPacket.ux *= ni / nt;
            //    PhotonPacket.uy *= ni / nt;
            //    PhotonPacket.uz = -uz1;
            //}
            //else
            //{
            //    PhotonPacket.uz = -uz;
            //}


            if (GenerateRandomNumber() > reflectance1)
            {
                if (layer <= 0)
                {
                    PhotonPacket.uz = -uz1;
                    RecordR(0.0);
                    PhotonPacket.dead = true;
                }

                else
                {
                    PhotonPacket.layer--;
                    PhotonPacket.ux *= ni / nt;
                    PhotonPacket.uy *= ni / nt;
                    PhotonPacket.uz = -uz1;
                }

            }
            else
                PhotonPacket.uz = -uz;
        }


        public void CrossDownOrNot()
        {
            var uz = PhotonPacket.uz;
            var uz1 = 0.0;
            var reflectance1 = 0.0;
            var layer = PhotonPacket.layer;
            var ni = LayerProperties.layerList[layer].n;
            var nt = layer<2 ?LayerProperties.layerList[layer+1].n:1.0;

            if (uz <= LayerProperties.layerList[layer].cos_crit1)
                reflectance1 = 1.0;
            else
                (reflectance1,uz1) = RFresnel(ni, nt, uz);

            //if (layer == GridParametersClass.num_layers-1 && reflectance1 < 1.0)
            //{
            //    PhotonPacket.uz = uz1;
            //    RecordT(reflectance1);
            //    PhotonPacket.uz = -uz;
            //}
            //else if (GenerateRandomNumber() > reflectance1)
            //{
            //    PhotonPacket.layer++;
            //    PhotonPacket.ux *= ni / nt;
            //    PhotonPacket.uy *= ni / nt;
            //    PhotonPacket.uz = uz1;
            //}
            //else
            //{
            //    PhotonPacket.uz = -uz;
            //}

            if (GenerateRandomNumber() > reflectance1)
            {
                if (layer == GridParametersClass.num_layers-1)
                {
                    PhotonPacket.uz = uz1;
                    RecordT(0.0);
                    PhotonPacket.dead = true;
                }
                else
                {
                    PhotonPacket.layer++;
                    PhotonPacket.ux *= ni / nt;
                    PhotonPacket.uy *= ni / nt;
                    PhotonPacket.uz = uz1;
                }
                
            }
            else
            {
                PhotonPacket.uz = -uz;
            }
        }

        public void CrossOrNot()
        {
            if(PhotonPacket.uz <0.0)
                CrossUpOrNot();
            else
                CrossDownOrNot();
        }
        private void RecordT(double reflectance1)
        {
            var x = PhotonPacket.x;
            var y = PhotonPacket.y;
            int ir, ia;

            var ird = Math.Sqrt(x * x + y * y) / GridParametersClass.dr;
            if (ird > GridParametersClass.nr - 1)
                ir = GridParametersClass.nr - 1;
            else
                ir =(int) ird;

            var iad = Math.Acos(PhotonPacket.uz) / GridParametersClass.da;
            if (iad > GridParametersClass.na - 1)
                ia = GridParametersClass.na - 1;
            else
                ia = (int)iad;

            Scoring.Tt_ra[ir,ia] += PhotonPacket.w * (1.0 - reflectance1);
            PhotonPacket.w *= reflectance1;
        }

        private void RecordR(double reflectance1)
        {
            var x = PhotonPacket.x;
            var y = PhotonPacket.y;
            int ir, ia;

            var ird = Math.Sqrt(x * x + y * y) / GridParametersClass.dr;
            if (ird > GridParametersClass.nr - 1)
                ir = GridParametersClass.nr - 1;
            else
                ir = (int)ird;

            var iad = Math.Acos(PhotonPacket.uz) / GridParametersClass.da;
            if (iad > GridParametersClass.na - 1)
                ia = GridParametersClass.na - 1;
            else
                ia = (int)iad;

            Scoring.Rd_ra[ir,ia] += PhotonPacket.w * (1.0 - reflectance1);
            PhotonPacket.w *= reflectance1;
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
            if (PhotonPacket.uz == 0.0)
            {
                PhotonPacket.dead = true;
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
            var s = PhotonPacket.s;


            PhotonPacket.x += s * PhotonPacket.ux;
            PhotonPacket.y += s * PhotonPacket.uy;
            PhotonPacket.z += s * PhotonPacket.uz;
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
                Spin(LayerProperties.layerList[PhotonPacket.layer].g);

            }
        }

        public void HopDropSpin()
        {
            var layer = PhotonPacket.layer;
            //if(LayerProperties.layerList[layer].mua ==0.0 && LayerProperties.layerList[layer].mua ==0.0)
            //    HopInGlass();
            //else
                HopDropSpinInTissue();

            if (PhotonPacket.w < GridParametersClass.Wth && !PhotonPacket.dead)
                Roulette();
        }

        private void Roulette()
        {
            const double Chance = 0.1;

            if (PhotonPacket.w == 0.0)
                PhotonPacket.dead = true;
            else if (GenerateRandomNumber() < Chance)
                PhotonPacket.w /= Chance;
            else
                PhotonPacket.dead = true;
        }

        private void Spin(double g)
        {
            var ux = PhotonPacket.ux;
            var uy = PhotonPacket.uy;
            var uz = PhotonPacket.uz;

            var cost = SpinTheta(g);
            var sint = Math.Sqrt(1.0 - cost * cost);

            var psi = 2.0 * Math.PI * GenerateRandomNumber();
            var cosp = Math.Cos(psi);
            double sinp; 
            
            if (psi < Math.PI)
                sinp = Math.Sqrt(1.0 - cosp * cosp);
            else
                sinp = - Math.Sqrt(1.0 - cosp * cosp);

            if (Math.Abs(uz) > 0.9999)
            {
                PhotonPacket.ux = sint * cosp;
                PhotonPacket.uy = sint * sinp;
                PhotonPacket.uz = cost * (uz >= 0 ? 1 : -1);
            }
            else
            {
                var temp = Math.Sqrt(1.0 - uz * uz);
                PhotonPacket.ux = sint * (ux * uz * cosp - uy * sinp) / temp + ux * cost;
                PhotonPacket.uy = sint * (uy * uz * cosp + ux * sinp) / temp + uy * cost;
                PhotonPacket.uz = -sint * cosp * temp + uz * cost;
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
                    cost = -1.0;
                else if (cost > 1)
                    cost = 1.0;
                
            }

            return cost;
        }

        private void Drop()
        {
            var x = PhotonPacket.x;
            var y = PhotonPacket.y;
            int iz, ir;

            var layer = PhotonPacket.layer;

            var izd = PhotonPacket.z / GridParametersClass.dz;
            if (izd > GridParametersClass.nz - 1)
                iz = GridParametersClass.nz - 1;
            else
                iz = (int)izd;

            var ird = Math.Sqrt(x * x + y * y) / GridParametersClass.dr;
            if (ird > GridParametersClass.nr - 1)
                ir = GridParametersClass.nr - 1;
            else
                ir = (int)ird;

            var mua = LayerProperties.layerList[layer].mua;
            var mus = LayerProperties.layerList[layer].mus;

            var dwa = PhotonPacket.w * mua / (mua + mus);
            PhotonPacket.w -= dwa;

            Scoring.A_rz[ir,iz] += dwa;
        }

        private bool HitBoundary()
        {
            var layer = PhotonPacket.layer;
            var uz = PhotonPacket.uz;
            bool hit;
            double dl_b = 0.0;

            if (uz > 0.0)
                dl_b = (LayerProperties.layerList[layer].z1 - PhotonPacket.z) / uz;
            else if (uz < 0.0)
                dl_b = (LayerProperties.layerList[layer].z0 - PhotonPacket.z) / uz;

            if (uz != 0.0 && PhotonPacket.s > dl_b)
            {
                var mut = LayerProperties.layerList[layer].mua + LayerProperties.layerList[layer].mus;
                PhotonPacket.sleft = (PhotonPacket.s - dl_b) / mut;
                PhotonPacket.s = dl_b;
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
