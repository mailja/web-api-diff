using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Ninject.Infrastructure.Language;
using WebAPIDiff.Controllers;
using WebAPIDiff.Domain.Abstract;
using WebAPIDiff.Domain.Entities;
using WebAPIDiff.Models;

namespace WebAPIDiff.Tests
{
  [TestClass]
  public class GetDiffUnitTests
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

      _ctrl = new DiffController(mock.Object);

      var config = new HttpConfiguration();
      var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/v1/Diff");
      var route = config.Routes.MapHttpRoute("Diffs", "v1/{controller}/{diffId}");
      var routeData = new HttpRouteData(route, new HttpRouteValueDictionary
      {
        { "controller", "Diff" },
        { "diffId", @"\d+" }
      });

      _ctrl.ControllerContext = new HttpControllerContext(config, routeData, request);
      _ctrl.Request = request;
      _ctrl.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

    }

    [TestCleanup()]
    public void TestCleanup() {
      _ctrl.Dispose();
    }

    [TestMethod]
    public void GetDiff_DataFieldsNotValid_StatusNotFound() {

      // Act
      var result = _ctrl.GetDiff(1).ExecuteAsync(CancellationToken.None);

      // Assert
      var statusCode = result.Result.StatusCode;
      Assert.IsTrue(statusCode == HttpStatusCode.NotFound);
    }

    [TestMethod]
    public void GetDiff_EqualContent_DiffsResultTypeEquals() {

      // Act
      var result = _ctrl.GetDiff(4).ExecuteAsync(CancellationToken.None);

      // Assert
      var statusCode = result.Result.StatusCode;
      Assert.IsTrue(statusCode == HttpStatusCode.OK);

      var json = result.Result.Content.ReadAsStringAsync().Result;
      var diffModel = JsonConvert.DeserializeObject<DiffResultModel>(json);
      Assert.IsNotNull(diffModel);
      Assert.IsTrue(diffModel.DiffResultType == "Equals");

    }

    [TestMethod]
    public void GetDiff_ContentDoNotMach_DiffsCollectionVerified() {

      // Act
      var result = _ctrl.GetDiff(6).ExecuteAsync(CancellationToken.None);

      // Assert
      var statusCode = result.Result.StatusCode;
      Assert.IsTrue(statusCode == HttpStatusCode.OK);

      var json = result.Result.Content.ReadAsStringAsync().Result;
      var diffModel = JsonConvert.DeserializeObject<DiffResultWithItemsModel>(json);
      Assert.IsNotNull(diffModel);
      Assert.IsTrue(diffModel.DiffResultType == "ContentDoNotMatch");
      Assert.IsTrue(diffModel.Diffs.Count == 2);
      Assert.IsTrue(diffModel.Diffs.First().Offset == 4);
      Assert.IsTrue(diffModel.Diffs.ElementAt(1).Offset == 22);

    }

  }
}
