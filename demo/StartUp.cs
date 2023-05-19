using Autofac;
using Harmonic.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Harmonic.Controllers;
using Harmonic.Controllers.Living;
using Harmonic.Networking.Rtmp;
using Harmonic.Service;
using Microsoft.Extensions.Logging.Console;

namespace demo
{
    class Startup : IStartup
    {
        public void ConfigureServices(ContainerBuilder builder)
        {

            //builder.Register(c => new MyRecordConfiguration()).As<RecordServiceConfiguration>();

            // builder.Register(x => new MyLivingController()).As<RtmpController>();
           // builder.RegisterType<MyLivingStream>().As<NetStream>();
            //builder.Register(c => new MyLivingController())
            //    .AsSelf();
            //builder.Register(c => new RecordService(c.Resolve<RecordServiceConfiguration>()))
            //    .AsSelf()
            //    .InstancePerLifetimeScope();
            //builder.Register(c => new PublisherSessionService())
            //    .AsSelf()
            //    .InstancePerLifetimeScope();

        }



    }
    class MyRecordConfiguration : RecordServiceConfiguration
    {
        public MyRecordConfiguration()
        {
            this.RecordPath = @"D:\\recored";
            this.FilenameFormat = @"recorded-{streamName}";
        }

    };
}
