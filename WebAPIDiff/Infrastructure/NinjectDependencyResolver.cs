using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Moq;
using Ninject;
using WebAPIDiff.Domain.Abstract;
using WebAPIDiff.Domain.Entities;

namespace WebAPIDiff.Infrastructure
{
  public class NinjectDependencyResolver : IDependencyResolver
  {
    private IKernel _kernel;

    public NinjectDependencyResolver(IKernel kernelParam) {
      _kernel = kernelParam;
      AddBindings();
    }

    public object GetService(Type serviceType) {
      return _kernel.TryGet(serviceType);
    }

    public IEnumerable<object> GetServices(Type serviceType) {
      return _kernel.GetAll(serviceType);
    }

    private void AddBindings() {
      var isMockBinding = false;
      if (ConfigurationManager.AppSettings["IsMockBinding"].IsBool())
      {
        isMockBinding = Convert.ToBoolean(ConfigurationManager.AppSettings["IsMockBinding"]);
      }
      ;

      if (isMockBinding)
      {
        Mock<IDiffRepository> mock = new Mock<IDiffRepository>();
        mock.Setup(m => m.Diffs).Returns(new List<Diff>
        {
          new Diff {DiffId = 1, LeftData = null, RightData = null},
          new Diff {DiffId = 2, LeftData = "TGVwIHBvemRyYXYgaXogZGFsamF2IDIzNA==", RightData = null},
          new Diff {DiffId = 3, LeftData = null, RightData = "TGVwIHBvemRyYXYgaXogZGFsamF2IDIzNA=="},
          new Diff {DiffId = 4, LeftData = "TGVwIHBvemRyYXYgaXogZGFsamF2IDIzNA==", RightData = "TGVwIHBvemRyYXYgaXogZGFsamF2IDIzNA=="},
          new Diff {DiffId = 5, LeftData = "TGVwIHBvemRyYXYgaXogZGFsamF2IDIzNA==", RightData = "TGVwIHBvemRyYXYgMzU1"},
          new Diff {DiffId = 6, LeftData = "TGVwIHBvemRyYXYgaXogZGFsamF2IDIzNA==", RightData = "TGVwIG9kemRyYXYgaXogZGFsamF2IDM1NQ=="},
          new Diff {DiffId = 7, LeftData = "", RightData = ""},
        });
        _kernel.Bind<IDiffRepository>().ToConstant(mock.Object);
      } else
      {
        //_kernel.Bind<EfDbContext>().To<EfDbContext>().InRequestScope();
        //_kernel.Bind<IDiffRepository>().To<EfDiffRepository>().InRequestScope();
      }
    }
  }
}