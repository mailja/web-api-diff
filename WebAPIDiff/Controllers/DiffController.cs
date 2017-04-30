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
using WebAPIDiff.Domain.Services;
using WebAPIDiff.Models;

namespace WebAPIDiff.Controllers
{
  /// <summary>
  /// Web API providing appropriate http endpoins.
  /// </summary>
  public class DiffController : ApiController
  {
    private IDiffRepository _diffRepository;
    private DiffService _diffService;
    private ModelFactory _modelFactory;

    /// <summary>
    /// Dependency Injection Container used for decoupling (testability etc.)
    /// </summary>
    /// <param name="diffRepository">Constructor injected repository.</param>
    public DiffController(IDiffRepository diffRepository)
    {
      if (diffRepository == null) throw new ArgumentNullException(nameof(diffRepository));

      _diffRepository = diffRepository;
      _diffService = new DiffService(_diffRepository);
      _modelFactory = new ModelFactory();
    }

    /// <summary>
    /// Result of decoded (from Base64 encoded) data pair comparison is returned.
    /// </summary>
    /// <param name="diffId">Id field used to retrieve data from storage.</param>
    /// <returns></returns>
    [HttpGet]
    public IHttpActionResult GetDiff(int diffId)
    {
      var diffPair = _diffService.GetDiff(diffId);

      if (diffPair?.LeftData == null || diffPair?.RightData == null)
      {
        return NotFound();
      }

      // Compare pair and create result
      var diffResultModel = _modelFactory.CompareDiffPair(diffPair);

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
        var diff = _modelFactory.CreateDiff(diffId, side, model);

        // Save object 
        var saveResult = await _diffService.SaveDiff(diff);

        if (saveResult > 0)
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
