using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIDiff.Domain.Entities
{
  public class Diff
  {

    public Diff()
    {
      DiffId = 0;
      LeftData = null;
      RightData = null;
    }

    public int DiffId { get; set; }
    public string LeftData { get; set; }
    public string RightData { get; set; }
  }
}
