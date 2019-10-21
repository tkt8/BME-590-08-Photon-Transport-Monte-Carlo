using System;
using System.Collections.Generic;
using System.Text;

namespace PhotonTransport
{
    public static class UpdateScoring
    {
        private static void Sum2DRd()
        {
            var nr = GridParametersClass.nr;
            var na = GridParametersClass.na;

            var sum = 0.0;

            for (var ir = 0; ir < nr; ir++)
            {
                sum = 0.0;
                for (var ia = 0; ia < na; ia++)
                {
                    sum += Scoring.Rd_ra[ir][ia];
                }

                Scoring.Rd_r[ir] = sum;
            }

            for (var ia = 0; ia < na; ia++)
            {
                sum = 0.0;
                for (var ir = 0; ir < nr; ir++)
                {
                    sum += Scoring.Rd_ra[ir][ia];
                }

                Scoring.Rd_a[ia] = sum;
            }

            sum = 0.0;
            for (var ir = 0; ir < nr; ir++)
            {
                sum += Scoring.Rd_r[ir];
            }
            Scoring.Rd = sum;

        }

        private static void Sum2DA()
        {
            var nr = GridParametersClass.nr;
            var nz = GridParametersClass.nz;

            double sum;
            for (var iz = 0; iz < nz; iz++)
            {
                sum = 0.0;
                for (var ir = 0; ir < nr; ir++)
                {
                    sum += Scoring.A_rz[ir][iz];
                }

                Scoring.A_z[iz] = sum;
            }

            sum = 0.0;
            for (var iz = 0; iz < nz; iz++)
            {
                sum += Scoring.A_z[iz];
                Scoring.A_l[IzToLayer(iz)] += Scoring.A_z[iz];
            }
            Scoring.A = sum;
        }

        private static int IzToLayer(int iz)
        {
            var i = 1;
            var numLayers = GridParametersClass.num_layers;
            var dz = GridParametersClass.dz;

            //while((iz + 0.5)*dz >= layerList[i].z1 && i<numLayers) 
            //    i++;

            return i;
        }


        private static void Sum2DTt()
        {
            var nr = GridParametersClass.nr;
            var na = GridParametersClass.na;
            double sum;

            for (var ir = 0; ir < nr; ir++)
            {
                sum = 0.0;
                for (var ia = 0; ia < na; ia++) 
                    sum +=Scoring.Tt_ra[ir][ia];
                Scoring.Tt_r[ir] = sum;
            }

            for (var ia = 0; ia < na; ia++)
            {
                sum = 0.0;
                for (var ir = 0; ir < nr; ir++) 
                    sum += Scoring.Tt_ra[ir][ia];
                Scoring.Tt_a[ia] = sum;
            }

            sum = 0.0;
            for (var ir = 0; ir < nr; ir++) 
                sum += Scoring.Tt_r[ir];
            Scoring.Tt = sum;
        }

        private static void ScaleRdTt()
        {
            var nr = GridParametersClass.nr;
            var na = GridParametersClass.na;
            var dr = GridParametersClass.dr;
            var da = GridParametersClass.da;
            double scale2;

            var scale1 = 4.0 * Math.PI * Math.PI * dr * Math.Sin(da / 2) * dr * GridParametersClass.num_photons;
            /* The factor (ir+0.5)*sin(2a) to be added. */

            for (var ir = 0; ir < nr; ir++)
            for (var ia = 0; ia < na; ia++)
            {
                scale2 = 1.0 / ((ir + 0.5) * Math.Sin(2.0 * (ia + 0.5) * da) * scale1);
                Scoring.Rd_ra[ir][ia] *= scale2;
                Scoring.Tt_ra[ir][ia] *= scale2;
            }

            scale1 = 2.0 * Math.PI * dr * dr * GridParametersClass.num_photons;
            /* area is 2*PI*[(ir+0.5)*dr]*dr.*/
            /* ir+0.5 to be added. */

            for (var ir = 0; ir < nr; ir++)
            {
                scale2 = 1.0 / ((ir + 0.5) * scale1);
                Scoring.Rd_r[ir] *= scale2;
                Scoring.Tt_r[ir] *= scale2;
            }

            scale1 = 2.0 * Math.PI * da * GridParametersClass.num_photons;
            /* solid angle is 2*PI*sin(a)*da. sin(a) to be added. */

            for (var ia = 0; ia < na; ia++)
            {
                scale2 = 1.0 / (Math.Sin((ia + 0.5) * da) * scale1);
                Scoring.Rd_a[ia] *= scale2;
                Scoring.Tt_a[ia] *= scale2;
            }

            scale2 = 1.0 / (double)GridParametersClass.num_photons;
            Scoring.Rd *= scale2;
            Scoring.Tt *= scale2;
        }

        private static void ScaleA()
        {
            var nz = GridParametersClass.nz;
            var nr = GridParametersClass.nr;
            var dz = GridParametersClass.dz;
            var dr = GridParametersClass.dr;
            var nl = GridParametersClass.num_layers;

            /* Scale A_rz. */
            var scale1 = 2.0 * Math.PI * dr * dr * dz * GridParametersClass.num_photons;
            /* volume is 2*pi*(ir+0.5)*dr*dr*dz.*/
            /* ir+0.5 to be added. */
            for (var iz = 0; iz < nz; iz++)
            for (var ir = 0; ir < nr; ir++)
                Scoring.A_rz[ir][iz] /= (ir + 0.5) * scale1;

            /* Scale A_z. */
            scale1 = 1.0 / (dz * GridParametersClass.num_photons);
            for (var iz = 0; iz < nz; iz++)
                Scoring.A_z[iz] *= scale1;

            /* Scale A_l. Avoid int/int. */
            scale1 = 1.0 / (double)GridParametersClass.num_photons;
            for (var il = 0; il <= nl + 1; il++)
                Scoring.A_l[il] *= scale1;

            Scoring.A *= scale1;
        }

        public static void SumScaleResult()
        {
            /* Get 1D & 0D results. */
            Sum2DRd();
            Sum2DA();
            Sum2DTt();

            ScaleRdTt();
            ScaleA();
        }

    }

}
