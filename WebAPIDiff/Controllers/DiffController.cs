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

    /// <summary>
    /// Result of decoded (from 64bit encoded) data pair comparison is returned.
    /// </summary>
    /// <param name="diffId">Id field used to retrieve data from storage.</param>
    /// <returns></returns>
    [HttpGet]
    public IHttpActionResult GetDiff(int diffId)
    {

      var result = _diffRepository.GetDiffs()
        .FirstOrDefault(p => p.DiffId == diffId);

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

    /// <summary>
    /// Create new entity (if new diffId is provided) or update one side (left/right) for existing.
    /// </summary>
    /// <param name="diffId">Id field serving as primary key in data storage.</param>
    /// <param name="side">Denotation of storage field (left or right). Just one at a time.</param>
    /// <param name="model">JSON formated object containing data to store.</param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IHttpActionResult> PutDiff(int diffId, string side, [FromBody] DiffPutModel model)
    {

      try
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

        // Save object 
        var result = await _diffRepository.SaveDiffAsync(diff);

        if (result > 0)
        {
          return StatusCode(HttpStatusCode.Created);
        }
        else
        {
          return BadRequest("Could not save to the database.");
        }
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }

    }
  }
}
