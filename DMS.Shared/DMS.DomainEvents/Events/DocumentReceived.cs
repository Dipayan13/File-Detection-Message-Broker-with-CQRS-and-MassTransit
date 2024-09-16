namespace DMS.DomainEvents
{
    public class DocumentReceived
    {
        public Context Context { get; set; }
        public DocumentReceivedMessage Message { get; set; }
    }

    public class DocumentReceivedMessage
    {
        public string DocumentId { get; set; }
        public string Source { get; set; }
        public string DocumentSrc { get; set; }
    }

    public class Context
    {
        public string TimeStamp { get; set; }
        public string CorrelationId { get; set; }
        public string SourceServiceName { get; set; }
        public string Domain { get; set; }
        public string CompanyName { get; set; }
    }
}
