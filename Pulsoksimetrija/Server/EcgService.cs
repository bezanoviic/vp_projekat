using Common;
using Common1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class EcgService : IEcgService, IDisposable
    {
        private FileWriter fileWriter;
        private long _lastTimestamp = -1;

        public void StartSession(SessionMeta meta)
        {
            Console.WriteLine($"[SESSION] Participant beginning: {meta.ParticipantId}");
            string fileName = $"{meta.ParticipantId}_ECG_Received.csv";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            fileWriter = new FileWriter(path);
        }

        public void PushSample(EcgSample sample)
        {
            // Validation: Growth of timestamp 
            if (sample.TimestampMs <= _lastTimestamp)
            {
                throw new FaultException<ValidationFault>(new ValidationFault
                {
                    Message = "Timestamp must be greater than last!",
                    Parametar = "TimestampMs"
                });
            }
            _lastTimestamp = sample.TimestampMs;

            // Validation: ECG range [-5000, 5000]
            if (sample.EcgMicroV.HasValue && (sample.EcgMicroV < -5000 || sample.EcgMicroV > 5000))
            {
                throw new FaultException<DataFormatFault>(new DataFormatFault
                {
                    Message = "ECG value out of range!",
                    Details = "EcgMicroV"
                });
            }

            // Validation: Pulse range [30, 220]
            if (sample.HeartRate.HasValue && (sample.HeartRate < 30 || sample.HeartRate > 220))
            {
                throw new FaultException<DataFormatFault>(new DataFormatFault
                {
                    Message = "Pulse out of realistic range!",
                    Details = "HeartRate"
                });
            }

            // Write to file: Timestamp, ECG, Pulse, RowIndex
            string line = $"{sample.TimestampMs},{sample.EcgMicroV},{sample.HeartRate},{sample.RowIndex}";
            fileWriter.WriteLine(line);
        }

        public void EndSession()
        {
            Console.WriteLine("[SESSION] Participant ending.");
            Dispose();
        }

        public void Dispose()
        {
            if (fileWriter != null)
            {
                fileWriter.Dispose();
                fileWriter = null;
                Console.WriteLine("[DEBUG] FileWriter resources have been successfully released.");
            }
        }
    }
}