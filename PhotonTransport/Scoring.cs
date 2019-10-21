using System;
using System.Collections.Generic;
using System.Text;

namespace PhotonTransport
{
    public class Scoring
    {
        public  double Rsp; /* specular reflectance. [-] */
        public double Rd_ra; /* 2D distribution of diffuse */
        /* reflectance. [1/(cm2 sr)] */
        public double Rd_r;   /* 1D radial distribution of diffuse */
        /* reflectance. [1/cm2] */
        public double Rd_a;   /* 1D angular distribution of diffuse */
        /* reflectance. [1/sr] */
        public double Rd;      /* total diffuse reflectance. [-] */

        public double A_rz;  /* 2D probability density in turbid */
        /* media over r & z. [1/cm3] */
        public double A_z;    /* 1D probability density over z. */
        /* [1/cm] */
        public double A_l;    /* each layer's absorption */
        /* probability. [-] */
        public double A;       /* total absorption probability. [-] */

        public double Tt_ra; /* 2D distribution of total */
        /* transmittance. [1/(cm2 sr)] */
        public double Tt_r;   /* 1D radial distribution of */
        /* transmittance. [1/cm2] */
        public double Tt_a;   /* 1D angular distribution of */
        /* transmittance. [1/sr] */
        public double Tt;
    }
}
