using System;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

class CodedBPSKCommunication
{
    static void Main()
    {
        int numBits = 100000; // Number of information bits to transmit
        int numCodewordBits = 7; // Number of codeword bits (including parity)
        int numCodewords = numBits / 4; // Number of codewords to transmit
        List<double> EbN0dBValues = new List<double>(); // List to store Eb/N0 values
        List<double> berValues = new List<double>(); // List to store BER values
        Random random = new Random();

        double[] EbN0dBs = { 0, 2, 4, 6, 8, 10 }; // Eb/N0 values in dB
        foreach (double EbN0dB in EbN0dBs)
        {
            double EbN0 = Math.Pow(10, EbN0dB / 10); // Convert Eb/N0 from dB to linear scale

            int numErrors = 0; // Counter for bit errors

            // Transmission loop
            for (int i = 0; i < numCodewords; i++)
            {
                // Generate random information bits
                int[] informationBits = GenerateRandomBits(4, random);

                // Encode the information bits using (7,4) Hamming code
                int[] codeword = EncodeHammingCode(informationBits);

                // Map the codeword bits to the signal constellation of BPSK
                int[] symbols = MapToBPSK(codeword);

                // Generate AWGN noise
                double[] noise = GenerateAWGN(symbols.Length, EbN0, random);

                // Add noise to the transmitted signal
                double[] receivedSymbols = AddNoise(symbols, noise);

                // Perform binary detection on the received symbols
                int[] detectedCodeword = DetectCodeword(receivedSymbols);

                // Decode the detected codeword using (7,4) Hamming code
                int[] decodedBits = DecodeHammingCode(detectedCodeword);

                // Compare decoded bits with the original information bits and count the number of errors
                for (int j = 0; j < informationBits.Length; j++)
                {
                    if (informationBits[j] != decodedBits[j])
                        numErrors++;
                }
            }

            // Calculate bit error rate (BER)
            double ber = (double)numErrors / (numBits);

            // Store Eb/N0 and BER values for plotting
            EbN0dBValues.Add(EbN0dB);
            berValues.Add(ber);

            // Print Eb/N0 and BER
            Console.WriteLine($"Eb/N0 = {EbN0dB} dB, BER = {ber}");
        }

        // Plotting the BER versus Eb/N0 curve
        PlotModel plotModel = new PlotModel { Title = "BER vs Eb/N0" };

        LinearAxis xAxis = new LinearAxis { Position = AxisPosition.Bottom, Title = "Eb/N0 (dB)" };
        LogarithmicAxis yAxis = new LogarithmicAxis { Position = AxisPosition.Left, Title = "BER", Base = 10, Minimum = 1e-6, Maximum = 1e-1, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, TitleFontWeight = FontWeights.Bold };

        LineSeries series = new LineSeries { MarkerType = MarkerType.Circle, MarkerSize = 4, MarkerStroke = OxyColors.Black };

        for (int i = 0; i < EbN0dBValues.Count; i++)
        {
            series.Points.Add(new DataPoint(EbN0dBValues[i], berValues[i]));
        }

        plotModel.Series.Add(series);
        plotModel.Axes.Add(xAxis);
        plotModel.Axes.Add(yAxis);

        // Create a PDF file to save the plot
        var pdfExporter = new PdfExporter { Width = 600, Height = 400 };
        using (var stream = System.IO.File.Create("BER_vs_EbN0.pdf"))
        {
            pdfExporter.Export(plotModel, stream);
        }
    }

    // Generate random binary bits
    static int[] GenerateRandomBits(int numBits, Random random)
    {
        int[] bits = new int[numBits];
        for (int i = 0; i < numBits; i++)
        {
            bits[i] = random.Next(2);
        }
        return bits;
    }

    // Encode the information bits using (7,4) Hamming code
    static int[] EncodeHammingCode(int[] informationBits)
    {
        int[] codeword = new int[7];
        codeword[2] = informationBits[0];
        codeword[4] = informationBits[1];
        codeword[5] = informationBits[2];
        codeword[6] = informationBits[3];

        codeword[0] = (codeword[2] + codeword[4] + codeword[6]) % 2;
        codeword[1] = (codeword[2] + codeword[5] + codeword[6]) % 2;
        codeword[3] = (codeword[4] + codeword[5] + codeword[6]) % 2;

        return codeword;
    }

    // Map the codeword bits to the signal constellation of BPSK
    static int[] MapToBPSK(int[] codeword)
    {
        int[] symbols = new int[codeword.Length];
        for (int i = 0; i < codeword.Length; i++)
        {
            symbols[i] = 1 - 2 * codeword[i];
        }
        return symbols;
    }

    // Generate AWGN noise
    static double[] GenerateAWGN(int numSymbols, double EbN0, Random random)
    {
        double noisePower = 1 / (2 * EbN0); // Calculate the noise power based on Eb/N0
        double[] noise = new double[numSymbols];
        for (int i = 0; i < numSymbols; i++)
        {
            noise[i] = Math.Sqrt(noisePower) * random.NextGaussian();
        }
        return noise;
    }

    // Add noise to the transmitted signal
    static double[] AddNoise(int[] symbols, double[] noise)
    {
        double[] receivedSymbols = new double[symbols.Length];
        for (int i = 0; i < symbols.Length; i++)
        {
            receivedSymbols[i] = symbols[i] + noise[i];
        }
        return receivedSymbols;
    }

    // Perform binary detection on the received symbols
    static int[] DetectCodeword(double[] receivedSymbols)
    {
        int[] detectedCodeword = new int[receivedSymbols.Length];
        for (int i = 0; i < receivedSymbols.Length; i++)
        {
            detectedCodeword[i] = receivedSymbols[i] >= 0 ? 0 : 1;
        }
        return detectedCodeword;
    }

    // Decode the detected codeword using (7,4) Hamming code
    static int[] DecodeHammingCode(int[] detectedCodeword)
    {
        int[] decodedBits = new int[4];
        decodedBits[0] = detectedCodeword[2];
        decodedBits[1] = detectedCodeword[4];
        decodedBits[2] = detectedCodeword[5];
        decodedBits[3] = detectedCodeword[6];
        return decodedBits;
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
