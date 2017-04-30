using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIDiff.Domain.Abstract;
using WebAPIDiff.Domain.Entities;

namespace WebAPIDiff.Domain.Services
{
  public class DiffService
  {
    private IDiffRepository _diffRepository;

    public DiffService(IDiffRepository diffRepository)
    {
      if (diffRepository == null) throw new ArgumentNullException(nameof(diffRepository));

      _diffRepository = diffRepository;
    }


    public Diff GetDiff(int diffId)
    {
      var diff = _diffRepository.GetDiffs()
        .FirstOrDefault(p => p.DiffId == diffId);

      return diff;
    }

    public IQueryable<Diff> GetDiffs()
    {
      var diffs = _diffRepository.GetDiffs()
        .OrderBy(d => d.DiffId);

      return diffs;
    }

    public async Task<int> SaveDiff(Diff diff)
    {
      var saveResult =  await _diffRepository.SaveDiffAsync(diff);

      return saveResult;
    }
  }
}
