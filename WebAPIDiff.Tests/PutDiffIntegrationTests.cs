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
      var mock = new Mock<IDiffRepository>();
      mock.Setup(d => d.GetDiffs()).Returns(new List<Diff>()
      {
        new Diff {DiffId = 1, LeftData = null, RightData = null},
        new Diff {DiffId = 2, LeftData = "TGVwIHBvemRyYXYgaXogZGFsamF2IDIzNA==", RightData = null},
        new Diff {DiffId = 3, LeftData = null, RightData = "TGVwIHBvemRyYXYgaXogZGFsamF2IDIzNA=="},
        new Diff {DiffId = 4, LeftData = "TGVwIHBvemRyYXYgaXogZGFsamF2IDIzNA==", RightData = "TGVwIHBvemRyYXYgaXogZGFsamF2IDIzNA=="},
        new Diff {DiffId = 5, LeftData = "TGVwIHBvemRyYXYgaXogZGFsamF2IDIzNA==", RightData = "TGVwIHBvemRyYXYgMzU1"},
        new Diff {DiffId = 6, LeftData = "TGVwIHBvemRyYXYgaXogZGFsamF2IDIzNA==", RightData = "TGVwIG9kemRyYXYgaXogZGFsamF2IDM1NQ=="},
        new Diff {DiffId = 7, LeftData = "", RightData = ""},

      }
      .AsQueryable());

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
      var modelPut = new DiffPutModel()
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

      var resultLeft = _ctrl.PutDiff(999, "left", modelPut).Result.ExecuteAsync(CancellationToken.None);
      var resultRight = _ctrl.PutDiff(999, "right", modelPut).Result.ExecuteAsync(CancellationToken.None);

      var resultGet = _ctrl.GetDiff(4).ExecuteAsync(CancellationToken.None);

      cleanLeft = _ctrl.PutDiff(999, "left", modelClean).Result.ExecuteAsync(CancellationToken.None);
      cleanRight = _ctrl.PutDiff(999, "right", modelClean).Result.ExecuteAsync(CancellationToken.None);

      // Assert
      var statusCodeGet = resultGet.Result.StatusCode;
      var statusCodeLeft = resultLeft.Result.StatusCode;
      var statusCodeRight = resultRight.Result.StatusCode;
      Assert.IsTrue(statusCodeGet == HttpStatusCode.OK);
      Assert.IsTrue(statusCodeLeft == HttpStatusCode.Created);
      Assert.IsTrue(statusCodeRight == HttpStatusCode.Created);
    }

  }
}
