﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIDiff.Models
{
  
  public class DiffResultWithItemsModel : IDiffResultModel
  {
    public string DiffResultType { get; set; }
    public List<DiffResultItemModel> Diffs { get; set; }
  }
}