using Client;
using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Client
{
    public class CsvParser
    {
        public List<EcgSample> ParseEcgCsv(string filePath, string participantId)
        {
            List<EcgSample> samples = new List<EcgSample>();

            // IDisposable and StreamReader (using block)
            using (var reader = new StreamReader(filePath))
            {
                // Skip header
                reader.ReadLine();

                int rowIndex = 1;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(',');
                    try
                    {
                        // Parse 9 channels with InvariantCulture
                        samples.Add(new EcgSample
                        {
                            TimestampMs = long.Parse(parts[0]),
                            EcgMicroV = ParseNullableDouble(parts[1]),
                            HeartRate = ParseNullableDouble(parts[2]),
                            IBI_ms = ParseNullableDouble(parts[3]),
                            AccX = ParseNullableDouble(parts[4]),
                            AccY = ParseNullableDouble(parts[5]),
                            AccZ = ParseNullableDouble(parts[6]),
                            ParticipantId = participantId,
                            RowIndex = rowIndex++
                        });
                    }
                    catch
                    {
                        // Problematic rows are written to rejects_client.csv
                        File.AppendAllText("rejected_client.csv", $"{line}{Environment.NewLine}");
                    }
                }
            }
            return samples;
        }

        // NaN is mapped to null
        private double? ParseNullableDouble(string value)
        {
            if (value.Trim().Equals("NaN", StringComparison.OrdinalIgnoreCase)) return null;
            return double.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}