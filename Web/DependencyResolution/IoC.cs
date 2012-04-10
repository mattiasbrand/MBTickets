using System;
using System.IO;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.ModelBinding;
using Microsoft.Practices.ServiceLocation;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database.Server;
using StructureMap;
using StructureMap.ServiceLocatorAdapter;

namespace Web.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            ObjectFactory.Initialize(x =>
                                         {
                                             x.Scan(scan =>
                                                        {
                                                            scan.TheCallingAssembly();
                                                            scan.WithDefaultConventions();
                                                        });

                                             x.For<IActionValueBinder>().Use<DefaultActionValueBinder>();
                                             x.For<IHttpControllerFactory>().Use<DefaultHttpControllerFactory>();
                                             x.For<IFormatterSelector>().Use<FormatterSelector>();

                                             // Common Service Locator
                                             ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(ObjectFactory.Container));

                                             // RavenDB embedded
                                             x.For<IDocumentStore>().Singleton().Use(GetDocumentStore());
                                         });

            return ObjectFactory.Container;
        }

        private static IDocumentStore GetDocumentStore()
        {
            var documentStore = new EmbeddableDocumentStore
                                    {
                                        DataDirectory =
                                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\RavenDB"),
                                        UseEmbeddedHttpServer = true
                                    };
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8081);
            documentStore.Initialize();
            return documentStore;
        }
    }
}