using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPIDiff.Domain.Entities;

namespace WebAPIDiff.Models
{
  /// <summary>
  /// 
  /// </summary>
  public class ModelFactory
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="diffId"></param>
    /// <param name="side"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public Diff CreateDiff(int diffId, string side, DiffPutModel model) {

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

      return diff;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="diffPair"></param>
    /// <returns></returns>
    public IDiffResultModel CompareDiffPair(Diff diffPair)
    {
      // Initialize result elements
      var diffResultType = "Equals";
      var diffs = new List<DiffResultItemModel>();

      // Decode content
      var leftContent = Convert.FromBase64String(diffPair.LeftData.Trim());
      var rightContent = Convert.FromBase64String(diffPair.RightData.Trim());

      // Comparing decoded content and not Base64 encoded data
      if (leftContent.Length != rightContent.Length)
      {
        diffResultType = "SizeDoNotMatch";
      } else
      {
        var size = leftContent.Length;
        // Identifying the diffs
        for (var i = 0; i < size; i++)
        {
          if (leftContent[i] == rightContent[i]) continue;

          diffResultType = "ContentDoNotMatch";
          var diff = new DiffResultItemModel()
          {
            Offset = i,
            Length = 0
          };
          // How long is the Diff
          for (var j = i; ((j < size) && (leftContent[j] != rightContent[j])); j++)
          {
            diff.Length++;
            i++;
          }

          diffs.Add(diff);
        }
      }

      var diffResultModel = CreateDiffResultModel(diffResultType, diffs);

      return diffResultModel;
    }


    private IDiffResultModel CreateDiffResultModel(string diffResultType, List<DiffResultItemModel> diffs)
    {
      IDiffResultModel diffResultModel;

      // Two different concrete result models required (as it seems) in specification
      if (diffs.Count > 0)
      {
        diffResultModel = new DiffResultWithItemsModel()
        {
          Diffs = diffs
        };
      } else
      {
        diffResultModel = new DiffResultModel();
      }
      diffResultModel.DiffResultType = diffResultType;

      return diffResultModel;
    }

  }
}