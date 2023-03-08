using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Core.Resolving;
using System.Diagnostics;

namespace MiniAPI
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.Register(c => new ValuesService(c.Resolve<ILogger<ValuesService>>()))
            //    .As<IValuesService>()
            //    .InstancePerLifetimeScope();
        }
    }
}
