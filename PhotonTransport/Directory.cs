using System;
using System.Collections.Generic;
using System.Text;

namespace PhotonTransport
{
    public class Directory
    {

        public double GenerateRandomNumber()
        {
            var rnd = new Random();
            var num = rnd.NextDouble();
            return num;
        }

    }
}
