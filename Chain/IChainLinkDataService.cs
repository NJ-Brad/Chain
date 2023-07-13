namespace Chain
{
    public interface IChainLinkDataService
    {
        Task<IEnumerable<ChainLink>> Select(string orderBy="", bool descending = false);
        Task<ChainLink> GetChainLink(string id);
        Task<int> DeleteChainLink(string id);
        Task<ChainLink> UpdateChainLink(ChainLink document);
        Task<ChainLink> CreateChainLink(ChainLink document);
    }
}
