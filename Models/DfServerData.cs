namespace WindowsHealth_ServerCheck.Models
{
    public class DfServerData
    {
        public bool HasCertifiedDigitization { get; set; }
        public bool HasConfigureDigitization { get; set; }

        public bool HasDfSignature { get; set; }
        public int DfSignatureCount { get; set; }   
        public bool ClientNotificateSignature { get; set; }

        public bool HasCertificates { get; set; }
        public List<CertificateInfo> Certificate { get; set; } = new List<CertificateInfo>();
    }

    public class CertificateInfo
    {
        public string Name { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool ClientNotificade { get; set; }

        public CertificateStatus Status { get; set; }
    }

    public enum CertificateStatus
    {
        Valid,
        ExpiringSoon,
        Expired
    }
}
