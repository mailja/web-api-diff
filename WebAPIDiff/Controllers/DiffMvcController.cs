using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAPIDiff.Domain.Abstract;

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

    // GET: DiffMvc
    public ActionResult List() {
      return View(_diffRepository.Diffs);
    }

    #endregion

  }
}