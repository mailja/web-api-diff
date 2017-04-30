using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAPIDiff.Domain.Abstract;
using WebAPIDiff.Domain.Services;

namespace WebAPIDiff.Controllers
{
  public class DiffMvcController : Controller
  {
    private IDiffRepository _diffRepository;

    public DiffMvcController(IDiffRepository diffRepository)
    {
      if (diffRepository == null) throw new ArgumentNullException(nameof(diffRepository));

      _diffRepository = diffRepository;
    }

    #region Queries and Lists

    [HttpGet]
    // GET: DiffMvc
    public ActionResult List() {
      var diffService = new DiffService(_diffRepository);

      var diffList = diffService.GetDiffs().ToList();

      //_diffRepository.GetDiffs().ToList()
      return View(diffList);
    }

    #endregion

  }
}