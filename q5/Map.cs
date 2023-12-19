namespace q5;

public class Map
{
    public (string Source, string Target) Link { get; set; }

    public List<(long Source, long Target, long Count)> Ranges { get; set; }
}