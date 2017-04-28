namespace WebAPIDiff.Models
{
  public class DiffResultItemModel
  {
    public DiffResultItemModel()
    {
      Offset = 0;
      Length = 0;
    }

    public int Offset { get; set; }
    public int Length { get; set; }
  }
}