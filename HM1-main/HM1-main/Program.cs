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
            
            // transmitted m.
            int s = 1 - 2 * m;

            // e*log eb/n = 1, set eb/n = value
            double value = Math.Pow(10, 0.1);

            // eb/n = 1/R * Ec/2*q^2 set the variable as n

            // this part is add noise
            //double noise = Math.Sqrt(1 / t);

            double noise = 1 / value;

            // generate the noise by random
            var seed = Guid.NewGuid().GetHashCode();
            Random random = new Random(seed);

            double result = random.NextDouble() * (noise - 0) + 0;

            // receive signal
            double re = s + result;

            // dected m
            if (re >= 0)
            {
                return 0;
            }
            else if(re<0)
            {
                return 1;
            }

            return 0;

            //int decectedM = 0; ;

            //if (re >= 0)
            //{
            //    decectedM = 0;
            //}
            //else if (re < 0)
            //{
            //    decectedM = 1 ;
            //}

            //if (decectedM == s)
            //{
            //    return 1;       
            //}
            //else { return 0; }
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

            // Convert input bits to a 4x1 matrix
            int[,] inputVector = new int[4, 1];
            for (int i = 0; i < 4; i++)
            {
                inputVector[i, 0] = a[i];
            }

            int[,] matrixG = new int[,]
            {
                {1, 0, 0, 0, 1, 0,  1},
                { 0, 1, 0, 0, 1, 1, 1},
                { 0, 0, 1, 0, 1, 1, 0},
                { 0, 0, 0, 1, 0, 1, 1} 
            };

            //double[] cArray = new double[7];

            //int flag = 0;
            //for (int i = 0; i < 7; i++)
            //{
            //    int temp = 0;

            //    for (int j = 0; j < 4; j++)
            //    {
            //        cArray[flag++] = (a[temp] * matrixG[j, i]) + (a[temp+1] * matrixG[j + 1, i]) + (a[temp+2] * matrixG[j + 2, i]) + (a[temp+3] * matrixG[j + 3, i]);
            //        break;
            //    }
            //}

            int[,] outputVector = new int[7, 1];
            for (int i = 0; i < 7; i++)
            {
                int sum = 0;

                for (int j = 0; j < 4; j++)
                {
                    sum += matrixG[j, i] * inputVector[j, 0];
                }

                outputVector[i, 0] = sum % 2;
            }

            // Convert output vector to an array of bits
            int[] outputBits = new int[7];
            for (int i = 0; i < 7; i++)
            {
                outputBits[i] = outputVector[i, 0];
            }

            // s_i = f(c_i)= 1 - 2c_i;
            int[] s = new int[7];
            
            for (int i = 0; i < 7; i++)
            {
                s[i] = 1 - 2 * outputBits[i];
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
            
            int count = 0;

            double[] distance = new double[4];

           
            
            for (int i = 0; i < 4; i++)
            {
                double sum = 0;
                int localCount = 0;

                while (localCount != 7)
                {
                    sum += Math.Pow(r[i] - G[i, localCount++], 2);
                }

                distance[i] = Math.Sqrt(sum);
            }
                
            double[] tempArray = new double[4];
            
            for (int i = 0;i<4;i++)
            {
                tempArray[i] = distance[i];
            }

            BubbleSort(distance);

            double miniDistance = distance[0];

            int poistion = 0;
            for (int i = 0; i < 4; i++)
            {
                if (tempArray[i] == miniDistance)
                { poistion = i; } 
            }

            int errorBitNumber = 0;
            
            for(int i = 0;i<7;i++)
            {
                if (matrixG[poistion, i] != outputBits[i])
                {
                    errorBitNumber++;
                }
            }

            Console.WriteLine($"MockBPSKwithHCode Accuarcy rate is: %{(double)errorBitNumber/7*100}");
        }

        public static void BubbleSort(double[] arr)
        {
            double n = arr.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        double temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }
            }
        }

        private static int[,] G = new int[4, 7] {
            {-1,1,1,1,-1,1,-1},
            {1,-1,1,1,-1,-1,-1},
            {1,1,-1,1,-1,-1,1},
            {1,1,1,-1,1,-1,-1 }
            };

        static void Main(string[] args)
        {

            //MockBPSKwithHCode();

            int count = 0;

            //for (int i = 0; i < 10; i++)
            //{
            //    count += MockBPSK(random());
            //}

            for (int i = 0; i < 1000; i++)
            {
                // generate m
                int n = random();

                // the result is dected m
                int result = MockBPSK(n);

                if (result == n)
                {
                    count++;
                }
            }

            float correctPer = (float)count / 1000; 
            Console.WriteLine($"MockBPSK Accuarcy rate is:{correctPer * 100}%");
        }
    } 
}
