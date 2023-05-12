using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Homework1
{
    internal class Program
    {
        static int db = 1;
        static int r = 1;
        static double e = 2.71;

        static int MockBPSK(int m)
        {
            //Random rnd = new Random();

            int s = 1 - 2 * m;

            // 10*log eb/n = 1, set eb/n = value
            double value = Math.Pow(e, 0.1);

            // eb/n = 1/R * Ec/2*q^2 set the variable as n

            // this part is add noise
            double t = (value) * 2;
            //double noise = Math.Sqrt(1 / t);
            double noise = 1 / t;

            Random random = new Random();
            double result = random.NextDouble() * (noise - 0) + 0;

            // receive signal
            double re = Math.Sqrt(1) * s + result;

            if (re >= 0)
            {
                return 0;
            }
            else if (re < 0)
            {
                return 1;
            }

            return 0;
        }


        static void Task()
        {
            





        }


        static void Main(string[] args)
        {
            int count = 0;

            for (int i = 0; i < 1000; i++)
            {
                Random rnd = new Random();

                int m = rnd.Next(0, 1);

                int result = MockBPSK(m);

                if (m == result)
                {
                    count++;
                }
            }

            // why there is no bug, this is impossible for me!!!
            double correctPer = count / 1000 ;
            Console.WriteLine(correctPer);

            //int[,] array2 = { { 1, 0, 0, 0, 1, 0, 1 }, { 0, 1, 0, 0, 1, 1, 1 }, { 0, 0, 1, 0, 1, 1, 0 }, { 0, 0, 0, 1, 0, 1, 1 } };
            //int[] a = new int[4];
            //int[] cArray = new int[7];

            //for (int i = 0; i < 4; i++)
            //{         
            //    Random rnd = new Random();

            //    a[i] = rnd.Next(0, 2);
            //}

            //for (int i = 0; i < 7; i++)
            //{
            //    for (int j = 0; j < 4; j++)
            //    {
            //        cArray[i] = a[j]*array2[j,i];
            //    }
            //}

            //int meanNoting = 0;
        }
    }
}
