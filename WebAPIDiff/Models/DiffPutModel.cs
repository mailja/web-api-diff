using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIDiff.Models
{
  /// <summary>
  /// JSON formated object containing data to store.
  /// </summary>
  public class DiffPutModel
  {
    /// <summary>
    /// Field containing Base64 encoded binary data. Length is limited to 500 characters.
    /// </summary>
    public string Data { get; set; }
  }
}