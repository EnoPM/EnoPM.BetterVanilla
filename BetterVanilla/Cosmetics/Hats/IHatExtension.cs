namespace BetterVanilla.Cosmetics.Hats;

public interface IHatExtension
{
    public int CurrentFrontFrame { get; set; }
    public int CurrentBackFrame { get; set; }
    public int FrontDelay { get; set; }
    public int BackDelay { get; set; }
    public float FrontTime { get; set; }
    public float BackTime { get; set; }
    public bool Behind { get; set; }
}