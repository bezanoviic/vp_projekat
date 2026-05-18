using Common1;
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

        [OperationContract(IsInitiating = true)]
        [FaultContract(typeof(ValidationFault))]
        void StartSession(SessionMeta meta);

        [OperationContract(IsInitiating = false)]
        [FaultContract(typeof(DataFormatFault))]
        [FaultContract(typeof(ValidationFault))]
        void PushSample(EcgSample sample);

        [OperationContract(IsInitiating = false, IsTerminating = true)]
        void EndSession();
    }
}