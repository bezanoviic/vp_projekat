using System;
using System.IO;
using Common;
using Common1;
using System.ServiceModel;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Starting... ===");

            // User input for participant number
            Console.Write("Enter the number of participants (e.g. 01, 02..): ");
            string input = Console.ReadLine().Trim().ToUpper();

            string participant = input;
            if (!participant.StartsWith("U"))
            {
                // If user enters only "1", format it to "U01"
                participant = "U" + input.PadLeft(2, '0');
            }

            // Client starting from ...\Client\bin\Debug
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            // Going 3 levels up to the main solution folder
            string rootDir = Directory.GetParent(baseDir).Parent.Parent.FullName;
            // Constructing the full path
            string filePath = Path.Combine(rootDir, "Dataset", participant, "UserH10", "ECG.csv");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"[ERROR] File not found! Requested path: {filePath}");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"\n✓ Starting process for participant: {participant}");
            Console.WriteLine($"✓ File found: {filePath}");

            // 3Starting WCF communication (IDisposable client)
            ChannelFactory<IEcgService> factory = new ChannelFactory<IEcgService>("EcgEndpoint");
            IEcgService proxy = factory.CreateChannel();

            try
            {
                // StartSession with Meta data
                var meta = new SessionMeta
                {
                    ParticipantId = participant,
                    DeviceId = "H10",
                    SampleRateHz = 130
                };

                proxy.StartSession(meta);
                Console.WriteLine("\n [SESSION OPEN SUCCESSFULLY]");

                CsvParser parser = new CsvParser();
                var samples = parser.ParseEcgCsv(filePath, participant);

                Console.WriteLine($"[CSV] Loaded {samples.Count} valid records into memory. Sending...");

                foreach (var s in samples)
                {
                    try
                    {
                        proxy.PushSample(s);
                    }
                    catch (FaultException<ValidationFault> vf)
                    {
                        Console.WriteLine($"Validation error; Row {s.RowIndex}: {vf.Detail.Message} [Field: {vf.Detail.Parametar}]");
                    }
                    catch (FaultException<DataFormatFault> df)
                    {
                        Console.WriteLine($"Validation error; Row {s.RowIndex}: {df.Detail.Message} [Field: {df.Detail.Details}]");
                    }
                }

                proxy.EndSession();
                Console.WriteLine("\n Transfer completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                ((IClientChannel)proxy).Abort(); // Properly close on error
            }
            finally
            {
                if (factory.State == CommunicationState.Opened)
                {
                    factory.Close();
                }
                else
                {
                    factory.Abort();
                }
            }
            Console.WriteLine("Press [ENTER] to exit...");
            Console.ReadLine();
        }
    }
}
