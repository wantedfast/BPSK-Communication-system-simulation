using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Homework1
{
    internal class Program
    {
        static double e = 2.71;

        static int MockBPSK(int m)
        {
            //Random rnd = new Random();

            int s = 1 - 2 * m;

            // e*log eb/n = 1, set eb/n = value
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

        static int random()
        {
            var seed = Guid.NewGuid().GetHashCode();
            Random r = new Random(seed);
            int i = r.Next(0,10000);
            return i % 2;
        }

        static void MockBPSKwithHCode()
        {
            int[] a = new int[4];

            for (int i = 0; i < 4; i++) 
            {
                a[i]=random(); 
            }


            int[,] matrixG = new int[,]
            {
                {1, 0, 0, 0, 1, 0,  1},
                { 0, 1, 0, 0, 1, 1, 1},
                { 0, 0, 1, 0, 1, 1, 0},
                { 0, 0, 0, 1, 0, 1, 1} 
            };

            double[] cArray = new double[7];

            int flag = 0;
            for (int i = 0; i < 7; i++)
            {
                int temp = 0;

                for (int j = 0; j < 4; j++)
                {
                    cArray[flag++] = (a[temp] * matrixG[j, i]) + (a[temp+1] * matrixG[j + 1, i]) + (a[temp+2] * matrixG[j + 2, i]) + (a[temp+3] * matrixG[j + 3, i]);
                    break;
                }
            }

            // s_i = f(c_i)= 1 - 2c_i;
            double[] s = new double[7];
            
            for (int i = 0; i < 7; i++)
            {
                s[i] = 1 - 2 * cArray[i];
            }


            // e*log eb/n = 1, set eb/n = value
            double value = Math.Pow(e, 0.1);

            // eb/n = 1/R * Ec/2*q^2 set the variable as n

            // this part is add noise
            double t = (value) * (8/7);
            double noise = 1 / t;

            Random ran = new Random();
            double n = ran.NextDouble() * (noise - 0) + 0;

            double[] r = new double[7];

            for (int i = 0; i < 7; i++)
            {
                r[i] = s[i] + n;
            }

            double[] c = new double[7];
            
            
            for (int i = 0; i < 7; i++)
            {
                c[i] = r[i]<s[i]?r[i]:s[i];
            }

            int count = 0;

            for (int i = 0; i < 7; i++)
            {
                double value = c[i]-cArray[i];
                value = Math.Pow(value, 2);

            }

            Console.WriteLine($"Accuarcy rate is: %{(double)count/7*100}");
        }

        static void Main(string[] args)
        {
            
            MockBPSKwithHCode();

            int count = 0;

            for (int i = 0; i < 10; i++)
            {
                int n = random();

                int result = MockBPSK(random());

                if (n == result)
                {
                    count++;
                }
            }

            float correctPer = count / (float)10;
            Console.WriteLine($"Accuarcy rate is: %{correctPer * 100}");
        }
    } 
}
