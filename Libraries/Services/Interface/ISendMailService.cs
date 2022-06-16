namespace Services.Interface
{
    public interface ISendMailService
    {
        public delegate ISendMailService MailServiceResolver(MailServiceType type);

        public enum MailServiceType { Normal, Aws };

        Task SendMail(string subject, string body, List<string>? carbonCopies = null, List<string>? blindCarbonCopies = null);
    }
}
