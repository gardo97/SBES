using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;

namespace Manager
{
    public class ClientCertValidator : X509CertificateValidator
    {
        /// <summary>
        /// Implementation of a custom certificate validation on the client side.
        /// Client should consider certificate valid if the given certifiate is not self-signed.
        /// If validation fails, throw an exception with an adequate message.
        /// </summary>
        /// <param name="certificate"> certificate to be validate </param>
        public override void Validate(X509Certificate2 certificate)
        {
            string issuer = "CN=SbesCA";

            if (!certificate.Issuer.Equals(issuer))
            {
                throw new Exception("Certificate is not from the valid issuer.");
            }
        }
    }
}
