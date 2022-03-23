using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IWCFService
    {
        #region Admin
        [OperationContract]
        string KreiranjeBaze(string bazaPodataka);
        [OperationContract]
        string BrisanjeBaze(string bazaPodataka); 
        //[OperationContract]
       // string ArhiviranjeBaze(string bazaPodataka);
        #endregion

        #region Writer
        [OperationContract]
        string UpisUBazu(string poruka, byte[] promena,byte[] signature);
        [OperationContract]
        string ModifikacijaBaze(string poruka, byte[] promena,byte[] signature);
        #endregion

        #region Reader
        [OperationContract]
        byte[] CitanjeBaze(string bazaPodataka);
        #endregion

        #region All
        [OperationContract]
        string SrednjaVrednostZaGradStarost(string bazaPodataka, string grad, string starostOd, string starostDo);
        [OperationContract]
        string SrednjaVrednostZaDrzavuGodinu(string bazaPodataka, string drzava, string godina);
        [OperationContract]
        string NajvecaPrimanjaPoDrzavi(string bazaPodataka);
        #endregion
    }
}
