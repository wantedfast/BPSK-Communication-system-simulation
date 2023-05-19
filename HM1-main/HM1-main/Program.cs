using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Homework1
{
    internal class Program
    {
        static double e = 2.71;
        static int Ec = 1;
        static int[] ber = new int[10];
        static int errors = 0;

        private static List<double> dbValues = new List<double>();
        private static List<double> berValues = new List<double>();

        static void MockBPSK(int m, double db, Random rand)
        {
            // transmitted m.
            int s = 1 - 2 * m;

            double EbN0 = Math.Pow(10, db / 10);

            double noise = Math.Sqrt(Ec / (2 * EbN0));

            // generate the noise by with Gaussian random
            double n = noise * rand.NextGaussian();

            // add noise
            double result = Math.Sqrt(Ec) * s + n;

            // receive signal
            int detectedBit = result >= 0 ? 0 : 1;

            if (detectedBit != m)
            {
                errors = errors + 1;
            }
        }

        static void BER()
        {
            Random rand = new Random();

            int m = rand.Next(2);

            double[] signal = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            foreach (double db in signal)
            {
                for (int i = 0; i < 10000; i++)
                {
                    MockBPSK(m, db, rand);
                }

                double ber = (double)errors / 10000;
                Console.WriteLine($"Eb/N0 = {db} dB, BER = {ber}");

                dbValues.Add(db);
                berValues.Add(ber);
            }
        }

        static void Plot()
        {
            BER();

            PlotModel plotModel = new PlotModel { Title = "BER vs Eb/n0" };

            LogarithmicAxis xAxis = new LogarithmicAxis { Position = AxisPosition.Bottom, Title = "Eb/n0(db)", };
            LogarithmicAxis yAxis = new LogarithmicAxis { Position = AxisPosition.Left, Title = "BER", Base = 10, Minimum = 1e-6, Maximum = 1e-1, MajorStep = 1e-1 };

            LineSeries line = new LineSeries { MarkerType = MarkerType.Circle, MarkerSize = 4, MarkerStroke = OxyColors.Blue };

            for (int i = 0; i < dbValues.Count; i++)
            {
                line.Points.Add(new DataPoint(dbValues[i], berValues[i]));
            }

            plotModel.Series.Add(line);
            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            var pdf = new PdfExporter { Width = 600, Height = 400 };

            using (var stream = System.IO.File.Create(@"C:\Users\ITA\Desktop\BPSK.pdf"))
            {
                pdf.Export(plotModel, stream);
            }
        }

        static int random()
        {
            var seed = Guid.NewGuid().GetHashCode();
            Random r = new Random(seed);
            int i = r.Next(0, 10000);

            return i % 2;
        }

        static void MockBPSKwithHCode()
        {
            int[] a = new int[4];

            for (int i = 0; i < 4; i++)
            {
                a[i] = random();
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
            double t = (value) * (8 / 7);
            double noise = 1 / t;

            Random ran = new Random();
            double n = ran.NextDouble() * (noise - 0) + 0;

            double[] r = new double[7];

            for (int i = 0; i < 7; i++)
            {
                r[i] = s[i] + n;
            }

            double[] c = new double[7];

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

            for (int i = 0; i < 4; i++)
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

            for (int i = 0; i < 7; i++)
            {
                if (matrixG[poistion, i] != outputBits[i])
                {
                    errorBitNumber++;
                }
            }

            Console.WriteLine($"MockBPSKwithHCode Accuarcy rate is: %{(double)errorBitNumber / 7 * 100}");
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

            Plot();
        }
    }
}

static class RandomExtensions
{
    public static double NextGaussian(this Random random)
    {
        double u1 = 1.0 - random.NextDouble();
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

        return randStdNormal;
    }
}