using System;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

class BPSKTransmit
{
    static void Main()
    {
        int numBits = 100000; // Number of bits to transmit
        List<double> EbN0dBValues = new List<double>(); // List to store Eb/N0 values
        List<double> berValues = new List<double>(); // List to store BER values
        Random random = new Random();

        double[] EbN0dBs = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }; // Eb/N0 values in dB
        foreach (double EbN0dB in EbN0dBs)
        {
            double EbN0 = Math.Pow(10, EbN0dB / 10); // Convert Eb/N0 from dB to linear scale

            double Ec = 1; // Energy per bit
            double c = Math.Sqrt(Ec / (2 * EbN0)); // Calculate the noise standard deviation (sigma)

            int numErrors = 0; // Counter for bit errors

            // Transmission loop
            for (int i = 0; i < numBits; i++)
            {
                // Generate random bit with equal probabilities
                int m = random.Next(2);

                // Map the bit to the signal constellation of BPSK
                int s = 1 - 2 * m;

                // Generate a Gaussian random variable with noise standard deviation c
                double n = c * random.NextGaussian();

                // Add noise to the transmitted signal
                double r = Math.Sqrt(Ec) * s + n;

                // Perform binary detection on the received symbol
                int detectedBit = r >= 0 ? 0 : 1;

                // Compare detected bit with the transmitted bit and count the number of errors
                if (detectedBit != m)
                    numErrors++;
            }

            // Calculate bit error rate (BER)
            double ber = (double)numErrors / numBits;

            // Store Eb/N0 and BER values for plotting
            EbN0dBValues.Add(EbN0dB);
            berValues.Add(ber);

            // Print Eb/N0 and BER
            Console.WriteLine($"Eb/N0 = {EbN0dB} dB, BER = {ber}");
        }

        // Plotting the BER versus Eb/N0 curve
        PlotModel plotModel = new PlotModel { Title = "BER vs Eb/N0" };

        LinearAxis xAxis = new LinearAxis { Position = AxisPosition.Bottom, Title = "Eb/N0 (dB)" };
        LogarithmicAxis yAxis = new LogarithmicAxis { Position = AxisPosition.Left, Title = "BER", Base = 10, Minimum = 1e-6, Maximum = 1e-1, MajorStep = 1 };

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