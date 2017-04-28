using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIDiff.Domain.Entities;

namespace WebAPIDiff.Domain.Abstract
{
  public interface IDiffRepository
  {
    IEnumerable<Diff> Diffs { get; }
    IQueryable<Diff> GetDiffs();
    Task<int> SaveDiffAsync(Diff diff);
  }
}
