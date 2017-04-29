using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using WebAPIDiff.Controllers;
using WebAPIDiff.DataAccess.Sql.Concrete;
using WebAPIDiff.Domain.Abstract;
using WebAPIDiff.Domain.Entities;
using WebAPIDiff.Models;

namespace WebAPIDiff.Tests
{
  [TestClass]
  public class PutDiffIntegrationTests
  {
    private DiffController _ctrl;

    [TestInitialize]
    public void Init() {

      // Arrange
      var context = new DiffDbContext();
      var repository = new EfDiffRepository(context);
      _ctrl = new DiffController(repository);

      var config = new HttpConfiguration();
      var request = new HttpRequestMessage(HttpMethod.Put, "http://localhost/v1/Diff");
      var route = config.Routes.MapHttpRoute("Diffs", "v1/{controller}/{diffId}/{side}");
      var routeData = new HttpRouteData(route, new HttpRouteValueDictionary
      {
        { "controller", "Diff" },
        { "side", "^left$|^right$|^$" },
        { "diffId", @"\d+" }
      });

      _ctrl.ControllerContext = new HttpControllerContext(config, routeData, request);
      _ctrl.Request = request;
      _ctrl.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

    }

    [TestCleanup()]
    public void TestCleanup()
    {
      _ctrl.Dispose();
    }


    [TestMethod]
    public void PutDiff_DataFieldsNotValid_StatusBadRequest() {
      // Arange
      var model = new DiffPutModel()
      {
        Data = null
      };

      // Act
      var resultLeft = _ctrl.PutDiff(1, "left", model).Result.ExecuteAsync(CancellationToken.None);
      var resultRight = _ctrl.PutDiff(1, "right", model).Result.ExecuteAsync(CancellationToken.None);

      // Assert
      var statusCodeLeft = resultLeft.Result.StatusCode;
      var statusCodeRight = resultRight.Result.StatusCode;
      Assert.IsTrue(statusCodeLeft == HttpStatusCode.BadRequest);
      Assert.IsTrue(statusCodeRight == HttpStatusCode.BadRequest);
    }


    [TestMethod]
    public void PutDiff_DataFieldValid_StatusCreated() {
      // Arange
      var modelPutLeft = new DiffPutModel()
      {
        Data = "TGVwIHBvemRyYXYgaXogZGFsamF2IDIzNA=="
      };
      var modelPutRight = new DiffPutModel()
      {
        Data = "TGVwIHBvemRyYXYgMzU1"
      };
      var modelClean = new DiffPutModel()
      {
        Data = ""
      };

      // Act
      var cleanLeft = _ctrl.PutDiff(999, "left", modelClean).Result.ExecuteAsync(CancellationToken.None);
      var cleanRight = _ctrl.PutDiff(999, "right", modelClean).Result.ExecuteAsync(CancellationToken.None);

      var resultLeft = _ctrl.PutDiff(999, "left", modelPutLeft).Result.ExecuteAsync(CancellationToken.None);
      var resultRight = _ctrl.PutDiff(999, "right", modelPutRight).Result.ExecuteAsync(CancellationToken.None);

      var resultGet = _ctrl.GetDiff(999).ExecuteAsync(CancellationToken.None);

      cleanLeft = _ctrl.PutDiff(999, "left", modelClean).Result.ExecuteAsync(CancellationToken.None);
      cleanRight = _ctrl.PutDiff(999, "right", modelClean).Result.ExecuteAsync(CancellationToken.None);

      // Assert
      var statusCodeGet = resultGet.Result.StatusCode;
      var json = resultGet.Result.Content.ReadAsStringAsync().Result;
      var diffModel = JsonConvert.DeserializeObject<DiffResultModel>(json);
      Assert.IsTrue(statusCodeGet == HttpStatusCode.OK);
      Assert.IsTrue(diffModel.DiffResultType == "SizeDoNotMatch");

      var statusCodeLeft = resultLeft.Result.StatusCode;
      var statusCodeRight = resultRight.Result.StatusCode;
      Assert.IsTrue(statusCodeLeft == HttpStatusCode.Created);
      Assert.IsTrue(statusCodeRight == HttpStatusCode.Created);
    }

  }
}
