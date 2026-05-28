namespace HubBancario.Infrastructure.BankAdapters.Itau
{
    public class ItauOptions
    {
        public const string SectionName = "BankAdapters:Itau";
        
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePassword { get; set; }
    }
}