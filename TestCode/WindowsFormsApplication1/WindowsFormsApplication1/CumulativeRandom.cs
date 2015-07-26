using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WindowsFormsApplication1
{
    class CumulativeRandom
    {
        private RandomNumberGenerator rnd;
        //private Random rnd;
        private double buff;
        private double basis;

        public CumulativeRandom(int maxTimes)
        {
            rnd = RNGCryptoServiceProvider.Create();
            //rnd = new Random();
            basis = 1.0/maxTimes;
            buff = 0.0;
        }


        public bool Try()
        {
            bool result = NextDouble() < basis + buff;
            if (result)
            {
                buff = 0.0;
                counter = 0;
            }
            else
            {
                IncreaseBuff();
            }
            return result;
        }

        private byte[] doubleData = new byte[6];

        private double NextDouble()
        {
            //return rnd.NextDouble();

            rnd.GetBytes(doubleData);
            double result = 0;
            for (int i = 0; i < doubleData.Length; i++)
            {
                result += doubleData[i];
                result /= 256;
            }
            return result;
        }

        private void IncreaseBuff()
        {
            counter++;
            
            //buff += basis;
            
            //double multiplier = (basis + buff)/basis;
            //multiplier *= Math.Pow(1.0 / basis, basis);
            //buff = multiplier*basis - basis;
            
            // 1.0 - basis = (1.0/basis)*(1.0/basis+1)*k/2            
            //double k = (1.0 - basis)*basis*basis/(basis + 1)*2;
            //buff += k*counter;

            buff = 1.0/((1.0/basis) - counter) - basis;
        }

        private int counter = 0;
    }
}
