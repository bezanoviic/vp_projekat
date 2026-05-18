using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.IO;

namespace Common
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IEcgService
    {
        // Client sends meta data and opens session
        [OperationContract(IsInitiating = true)]
        void StartSession(SessionMeta meta);

        // Client sends n rows in MemoryStream
        [OperationContract(IsInitiating = false)]
        void PushBatch(Stream batchStream);

        // Client signals end
        [OperationContract(IsInitiating = false, IsTerminating = true)]
        void EndSession();
    }
}