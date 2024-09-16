public interface ICorrelationIdGenerator
{
    string GenerateCorrelationId();
}

public class CorrelationIdGenerator : ICorrelationIdGenerator
{
    public string GenerateCorrelationId()
    {
        return Guid.NewGuid().ToString();
    }
}