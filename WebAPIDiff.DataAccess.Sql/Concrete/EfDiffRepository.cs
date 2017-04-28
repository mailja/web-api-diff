using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIDiff.Domain.Abstract;
using WebAPIDiff.Domain.Entities;

namespace WebAPIDiff.DataAccess.Sql.Concrete
{
  public class EfDiffRepository : IDiffRepository
  {
    private DiffDbContext _context;

    public EfDiffRepository(DiffDbContext context) {
      _context = context;
    }

    public async Task<int> SaveDiffAsync(Diff diff)
    {
      if (diff.DiffId > 0)
      {
        var dbEntry = GetDiffs()
          .SingleOrDefault(d => d.DiffId == diff.DiffId);

        if (dbEntry != null)
        {
          if (diff.LeftData != null)
          {
            dbEntry.LeftData = diff.LeftData;
          }
          if (diff.RightData != null)
          {
            dbEntry.RightData = diff.RightData;
          }
        }
        else
        {
          _context.Diffs.Add(diff);
        }
      }

      return await _context.SaveChangesAsync();
    }

    public IQueryable<Diff> GetDiffs() => _context.Diffs;

    public IEnumerable<Diff> Diffs => _context.Diffs;
  }
}
