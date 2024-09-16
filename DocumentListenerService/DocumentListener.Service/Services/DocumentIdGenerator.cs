    public interface IDocumentIdGenerator

    {
        string GenerateUniqueDocumentId();
    }

    public class DocumentIdGenerator : IDocumentIdGenerator
    {
        public string GenerateUniqueDocumentId()
        {
            return Guid.NewGuid().ToString();
        }
    }
