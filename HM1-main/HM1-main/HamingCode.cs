using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.IO;

namespace HammingDistanceDecoder
{
    public class Hamming
    {
        // All possible messages vector.
        public List<List<int>> Messages { get; } = new List<List<int>>()
        {
            new List<int>() { 0, 0, 0, 0 },
            new List<int>() { 0, 0, 0, 1 },
            new List<int>() { 0, 0, 1, 0 },
            new List<int>() { 0, 0, 1, 1 },
            new List<int>() { 0, 1, 0, 0 },
            new List<int>() { 0, 1, 0, 1 },
            new List<int>() { 0, 1, 1, 0 },
            new List<int>() { 0, 1, 1, 1 },
            new List<int>() { 1, 0, 0, 0 },
            new List<int>() { 1, 0, 0, 1 },
            new List<int>() { 1, 0, 1, 0 },
            new List<int>() { 1, 0, 1, 1 },
            new List<int>() { 1, 1, 0, 0 },
            new List<int>() { 1, 1, 0, 1 },
            new List<int>() { 1, 1, 1, 0 },
            new List<int>() { 1, 1, 1, 1 }
        };

        // Generator Matrix.
        public List<List<int>> GenMAT { get; } = new List<List<int>>()
        {
            new List<int>() { 1, 0, 0, 0, 1, 1, 0 },
            new List<int>() { 0, 1, 0, 0, 0, 1, 1 },
            new List<int>() { 0, 0, 1, 0, 1, 1, 1 },
            new List<int>() { 0, 0, 0, 1, 1, 0, 1 }
        };

        // Function for generating binary bits at the source side. Each bit is equiprobable.
        // Output is a list that contains the binary bits of length one_million * 1.
        // one_million is defined by the programmer of this code.
        public List<int> SourceVector()
        {
            List<int> sourceBits = new List<int>();

            Random rand = new Random();

            for (int i = 0; i < 1_000_000; i++)
            {
                sourceBits.Add(rand.Next(2));
            }

            return sourceBits;
        }

        // Function for sum of two binary numbers in Galois field 2.
        private int Galois2Sum(int a, int b)
        {
            return (~a & b) | (a & ~b);
        }

        // Function for multiplication of two binary numbers in Galois field 2.
        private int Galois2Mul(int a, int b)
        {
            return a & b;
        }

        // Function for error message in matrices multiplication.
        private static void ErrorMSG()
        {
            Console.WriteLine("\n! Matrices size are not proper for multiplication !!! :-(");
            Console.WriteLine();
        }

        // Function for multiplying two matrices in Galois field 2.
        private List<List<int>> MatrixMultiGF2(List<List<int>> MAT1, List<List<int>> MAT2)
        {
            int rows = MAT1.Count;
            int columns = MAT2[0].Count;

            List<int> finalRow = new List<int>(columns);
            List<List<int>> resultantMAT = new List<List<int>>(rows);

            for (int i = 0; i < rows; i++)
            {
                finalRow.Clear();

                for (int j = 0; j < columns; j++)
                {
                    finalRow.Add(0);

                    for (int k = 0; k < MAT1[i].Count; k++)
                    {
                        finalRow[j] = Galois2Sum(finalRow[j], Galois2Mul(MAT1[i][k], MAT2[k][j]));
                    }
                }

                resultantMAT.Add(new List<int>(finalRow));
            }

            return resultantMAT;
        }

        // Function for encoding the messages.
        public List<List<int>> Encoder(List<List<int>> MAT1, List<List<int>> MAT2)
        {
            if (MAT1[0].Count == MAT2.Count)
            {
                return MatrixMultiGF2(MAT1, MAT2);
            }
            else
            {
                ErrorMSG();
                return new List<List<int>>();
            }
        }

        // Function for encoding the source stream.
        public List<int> EncodedBits(List<int> SourceBits, List<List<int>> GenMAT, int k)
        {
            List<int> encodedBits = new List<int>();
            List<int> inter = new List<int>();
            List<List<int>> MAT1 = new List<List<int>>();
            List<List<int>> MAT2;

            if (SourceBits.Count % k == 0)
            {
                for (int i = 0; i < SourceBits.Count; i += k)
                {
                    inter.Clear();

                    for (int j = i; j < i + k; j++)
                    {
                        inter.Add(SourceBits[j]);
                    }

                    MAT1.Add(new List<int>(inter));
                    MAT2 = Encoder(MAT1, GenMAT);

                    foreach (int value in MAT2[0])
                    {
                        encodedBits.Add(value);
                    }

                    MAT1.Clear();
                }
            }
            else
            {
                int padding = k - SourceBits.Count % k;

                for (int i = 0; i < k - padding; i++)
                {
                    SourceBits.Add(0);
                }

                for (int i = 0; i < SourceBits.Count; i += k)
                {
                    inter.Clear();

                    for (int j = i; j < i + k; j++)
                    {
                        inter.Add(SourceBits[j]);
                    }

                    MAT1.Add(new List<int>(inter));
                    MAT2 = Encoder(MAT1, GenMAT);

                    foreach (int value in MAT2[0])
                    {
                        encodedBits.Add(value);
                    }

                    MAT1.Clear();
                }
            }

            return encodedBits;
        }

        // Function for generating all possible codewords for estimating the Hamming distance.
        public List<List<int>> AllCodewordsList(List<List<int>> MAT1, List<List<int>> MAT2)
        {
            List<List<int>> codewordList = new List<List<int>>();
            List<List<int>> inter = new List<List<int>>();

            for (int i = 0; i < MAT1.Count; i++)
            {
                inter.Add(new List<int>(MAT1[i]));
                inter = MatrixMultiGF2(inter, MAT2);
                codewordList.Add(new List<int>(inter[0]));
                inter.Clear();
            }

            return codewordList;
        }

        // Function for calculating the Hamming distance between two codewords.
        private int HammingDistance(List<int> v1, List<int> v2)
        {
            int hamDist = 0;

            if (v1.Count == v2.Count)
            {
                for (int i = 0; i < v1.Count; i++)
                {
                    hamDist += (~v1[i] & v2[i]) | (v1[i] & ~v2[i]);
                }

                return hamDist;
            }
            else
            {
                Console.WriteLine("Error: Vector length is not the same. No Hamming distance.");
                return -1;
            }
        }

        // Function for finding the index of the minimum entry in an array.
        private int MinEntryIndex(List<int> array)
        {
            int minIndex = 0;

            for (int i = 0; i < array.Count; i++)
            {
                if (array[i] < array[minIndex])
                {
                    minIndex = i;
                }
            }

            return minIndex;
        }

        // Function for calculating the minimum Hamming distance among received codeword and possible codewords.
        private int MinHammingIndex(List<int> vec, List<List<int>> codeList)
        {
            List<int> hammingDistList = new List<int>();
            List<int> interBlock = new List<int>();

            for (int i = 0; i < codeList.Count; i++)
            {
                interBlock.Clear();

                for (int j = 0; j < codeList[i].Count; j++)
                {
                    interBlock.Add(codeList[i][j]);
                }

                hammingDistList.Add(HammingDistance(vec, interBlock));
            }

            return MinEntryIndex(hammingDistList);
        }

        // Function for decoding the received vector using Hamming distance methods.
        public List<int> HammingDecoder(List<double> decodedBits, List<List<int>> msgList, List<List<int>> codeList, int n)
        {
            List<int> finalBits = new List<int>();
            List<int> interBlock = new List<int>();
            int index;

            for (int i = 0; i < decodedBits.Count; i += n)
            {
                interBlock.Clear();

                for (int j = i; j < i + n; j++)
                {
                    interBlock.Add((int)Math.Round(decodedBits[j]));
                }

                index = MinHammingIndex(interBlock, codeList);
                interBlock.Clear();
                interBlock.AddRange(msgList[index]);

                finalBits.AddRange(interBlock);
            }

            return finalBits;
        }

        // Function for removing the padding of zeros.
        private List<int> FinalCut(List<int> array, int sizeofArray)
        {
            int cut = array.Count - sizeofArray;

            for (int i = 0; i < cut; i++)
            {
                array.RemoveAt(array.Count - 1);
            }

            return array;
        }

        // Function for counting the number of errors in the received bits.
        private double ErrorCalculation(List<int> sourceBits, List<int> decodedBits)
        {
            double countError = 0;

            for (int i = 0; i < sourceBits.Count; i++)
            {
                if (sourceBits[i] != decodedBits[i])
                {
                    countError++;
                }
            }

            return countError;
        }

        // Function to store the data in a file (.dat)
        private void DataFile(List<double> xindB, List<double> probError)
        {
            using (StreamWriter outfile = new StreamWriter("Ham1.dat"))
            {
                for (int i = 0; i < xindB.Count; i++)
                {
                    outfile.WriteLine($"{xindB[i]}\t\t{probError[i]}");
                }
            }
        }

        public void RunHammingDistanceDecoder()
        {
            List<double> energyOfBits = new List<double>();
            List<double> probError = new List<double>();
            double N_o = 4;
            double stdNoise, errorCount, normalValue;
            int k = 4;
            int n = 7;

            stdNoise = Math.Sqrt(N_o / 2); // Standard deviation of noise.

            List<double> SNR_dB = new List<double>();

            for (float i = 0; i <= 10; i += 0.125f)
            {
                SNR_dB.Add(i);
            }

            foreach (double snr in SNR_dB)
            {
                normalValue = Math.Pow(10, snr / 10);
                energyOfBits.Add(N_o * normalValue);
            }

            List<List<int>> codewordTable = AllCodewordsList(Messages, GenMAT); // All possible codewords.

            for (int step = 0; step < energyOfBits.Count; step++)
            {
                List<int> sourceBits = SourceVector();

                List<int> encodedBits = EncodedBits(sourceBits, GenMAT, k);

                List<double> trans = BitMapsToSymbolOfEnergyE(encodedBits, energyOfBits[step]);

                List<double> gnoise = GNoiseVector(0.0, stdNoise, encodedBits.Count);

                List<double> receiveBits = ReceiveBits(trans, gnoise);

                List<double> decodedBits = DecisionBlock(receiveBits);

                List<int> finalBits = HammingDecoder(decodedBits, Messages, codewordTable, n);

                if (finalBits.Count != sourceBits.Count)
                {
                    finalBits = FinalCut(finalBits, sourceBits.Count);
                }

                List<int> temp = new List<int>();

                foreach (var item in decodedBits)
                {
                    temp.Add((int)item);
                }

                errorCount = ErrorCalculation(encodedBits, temp);
                Console.WriteLine($"Error without Hamming distance: {errorCount}");

                errorCount = ErrorCalculation(sourceBits, finalBits);
                double pe = errorCount / 1_000_000;

                Console.WriteLine($"Errors: {errorCount}");
                Console.WriteLine($"Pe: {pe}");
                Console.WriteLine();

                probError.Add(pe);
                Console.WriteLine();
            }




            Plot(SNR_dB, probError);
            Console.WriteLine();
        }



        static void Plot(List<double> snr, List<double> pro)
        {
            PlotModel plotModel = new PlotModel { Title = "BER vs Eb/n0" };

            LogarithmicAxis xAxis = new LogarithmicAxis { Position = AxisPosition.Bottom, Title = "Eb/n0(db)", };
            LogarithmicAxis yAxis = new LogarithmicAxis { Position = AxisPosition.Left, Title = "BER", Base = 10, Minimum = 1e-6, Maximum = 1e-1, MajorStep = 1e-1 };

            LineSeries line = new LineSeries { MarkerType = MarkerType.Circle, MarkerSize = 4, MarkerStroke = OxyColors.Blue };

            for (int i = 0; i < snr.Count; i++)
            {
                line.Points.Add(new DataPoint(snr[i], pro[i]));
            }

            plotModel.Series.Add(line);
            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            var pdf = new PdfExporter { Width = 600, Height = 400 };

            using (var stream = System.IO.File.Create(@"C:\Users\ITA\Desktop\BPSKHammingCode.pdf"))
            {
                pdf.Export(plotModel, stream);
            }
        }



        // Modulator block

        // Function for mapping bits to symbol energy.
        public List<double> BitMapsToSymbolOfEnergyE(List<int> sourceBits, double energyOfSymbol)
        {
            List<double> transmittedSymbol = new List<double>();

            foreach (int bit in sourceBits)
            {
                if (bit == 0)
                {
                    transmittedSymbol.Add(-Math.Sqrt(energyOfSymbol));
                }
                else
                {
                    transmittedSymbol.Add(Math.Sqrt(energyOfSymbol));
                }
            }

            return transmittedSymbol;
        }

        // Noise generation (AWG noise)

        // Function for generating random noise based on a Gaussian distribution N(mean, variance).
        public List<double> GNoiseVector(double mean, double stddev, int size)
        {
            List<double> data = new List<double>();

            Random rand = new Random();

            for (int i = 0; i < size; i++)
            {
                double u1 = 1.0 - rand.NextDouble();
                double u2 = 1.0 - rand.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
                double randNormal = mean + stddev * randStdNormal;
                data.Add(randNormal);
            }

            return data;
        }

        // Receiver block

        // Function for adding noise to transmitted bits.
        public List<double> ReceiveBits(List<double> transmittedBits, List<double> noise)
        {
            List<double> receivedBits = new List<double>();

            for (int i = 0; i < transmittedBits.Count; i++)
            {
                receivedBits.Add(transmittedBits[i] + noise[i]);
            }

            return receivedBits;
        }

        // Function for deciding the received bits.
        public List<double> DecisionBlock(List<double> receivedBits)
        {
            List<double> decodedBits = new List<double>();

            foreach (double bit in receivedBits)
            {
                if (bit >= 0)
                {
                    decodedBits.Add(1);
                }
                else
                {
                    decodedBits.Add(0);
                }
            }

            return decodedBits;
        }
    }

    class Program
    {
        static void Main()
        {
            Hamming hamming = new Hamming();
            hamming.RunHammingDistanceDecoder();
        }
    }
}
