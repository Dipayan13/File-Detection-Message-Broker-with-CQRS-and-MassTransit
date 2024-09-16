public interface IFileSourceIdentifier
{
    string IdentifySource();
}

public class FileSourceIdentifier : IFileSourceIdentifier
{
    public string IdentifySource()
    {
        return "File System";
    }
}
