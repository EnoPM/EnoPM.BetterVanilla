namespace BetterVanilla.Cosmetics.Api.NamePlates;

public interface INamePlateResources<TResource>
{
    public TResource MainResource { get; set; }
}