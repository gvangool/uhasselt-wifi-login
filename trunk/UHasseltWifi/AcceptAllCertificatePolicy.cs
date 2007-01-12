using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace UHasseltWifi
{
	class AcceptAllCertificatePolicy : ICertificatePolicy
	{
		#region ICertificatePolicy Members

		public bool CheckValidationResult(ServicePoint srvPoint, System.Security.Cryptography.X509Certificates.X509Certificate certificate, WebRequest request, int certificateProblem)
		{
			return true;
		}

		#endregion
	}
}
