namespace MC.Source.Threading
{
    public interface IThreadFactory
    {
        IThreder CreateObject(string type, string filePath);
    }
}
