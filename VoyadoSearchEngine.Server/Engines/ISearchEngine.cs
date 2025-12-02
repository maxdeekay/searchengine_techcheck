namespace VoyadoSearchEngine.Server.Engines
{
    public interface ISearchEngine
    {
        string Name { get; }

        Task<int> SearchAsync(string word);
    }
}
