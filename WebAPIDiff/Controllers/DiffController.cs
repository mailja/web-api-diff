using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Castle.Core.Internal;
using WebAPIDiff.Domain.Abstract;
using WebAPIDiff.Domain.Entities;
using WebAPIDiff.Models;

namespace WebAPIDiff.Controllers
{
  public class DiffController : ApiController
  {
    private IDiffRepository _diffRepository;

    public DiffController(IDiffRepository diffRepository)
    {
      if (diffRepository == null) throw new ArgumentNullException(nameof(diffRepository));

      _diffRepository = diffRepository;
    }

    [HttpGet]
    public IHttpActionResult GetDiff(int diffId)
    {

      var result = _diffRepository.Diffs.FirstOrDefault(p => p.DiffId == diffId);

      if (result?.LeftData == null || result?.RightData == null)
      {
        return NotFound();
      }

      // Initialize result elements
      var diffResultType = "Equals";
      var diffs = new List<DiffResultItemModel>();

      // Decoded content
      var leftContent = Convert.FromBase64String(result.LeftData.Trim());
      var rightContent = Convert.FromBase64String(result.RightData.Trim());

      // Comparing decoded content and not Base64 encoded data
      if (leftContent.Length != rightContent.Length)
      {
        diffResultType = "SizeDoNotMatch";
      }
      else
      {
        // Identifying the diffs
        for (int i = 0; i < leftContent.Length; i++)
        {
          if (leftContent[i] != rightContent[i])
          {
            diffResultType = "ContentDoNotMatch";
            var diff = new DiffResultItemModel()
            {
              Offset = i,
              Length = 0
            };
            // How long is the Diff
            while (leftContent[i] != rightContent[i])
            {
              diff.Length ++;
              i++;
              // Since the size is eqal any field will do
              if (i >= leftContent.Length)
              {
                break;
              }
            }
            diffs.Add(diff);
          }
        }
      }

      // Two different result models required in specification
      if (diffs.Count > 0)
      {
        var diffResultWithItemsModel = new DiffResultWithItemsModel
        {
          DiffResultType = diffResultType,
          Diffs = diffs
        };
        return Ok(diffResultWithItemsModel);
      }

      var diffResultModel = new DiffResultModel
      {
        DiffResultType = diffResultType
      };

      return Ok(diffResultModel);
    }

    [HttpPut]
    public async Task<IHttpActionResult> PutDiff(int diffId, string side, [FromBody] DiffPutModel model)
    {

      if (model?.Data == null || diffId <= 0)
      {
        return BadRequest();
      }

      // Instantiate object to create or update
      Diff diff = new Diff()
      {
        DiffId = diffId
      };

      if (side.ToLower() == "left")
      {
        diff.LeftData = model?.Data.Trim() ?? null;
      }
      if (side.ToLower() == "right")
      {
        diff.RightData = model?.Data.Trim() ?? null;
      }

      //await _diffRepository.SaveProductAsync(diff);

      return StatusCode(HttpStatusCode.Created);
      //return BadRequest(ModelState);

    }
  }
}
