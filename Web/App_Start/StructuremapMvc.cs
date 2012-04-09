using System.Web.Http;
using System.Web.Mvc;
using StructureMap;
using StructureMap.ServiceLocatorAdapter;
using Web.App_Start;
using Web.DependencyResolution;
using WebActivator;

[assembly: PreApplicationStartMethod(typeof (StructuremapMvc), "Start")]

namespace Web.App_Start
{
    public static class StructuremapMvc
    {
        public static void Start()
        {
            IContainer container = IoC.Initialize();
            DependencyResolver.SetResolver(new SmDependencyResolver(container));
            // this override is needed because WebAPI is not using DependencyResolver to build controllers 
            //GlobalConfiguration.Configuration.ServiceResolver.SetResolver(new StructureMapServiceLocator(container));
        }
    }
}