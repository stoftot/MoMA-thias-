using MoMA_thias.web.Model;

namespace MoMA_thias.web.Service;

public interface IArtService
{
    Task<Art> CreateArtAsync(string name, double price);
    Task<Art> UpdateArtAsync(Guid id, string name, double price);
    Task<Art> DeleteArtAsync(Guid id);
}
