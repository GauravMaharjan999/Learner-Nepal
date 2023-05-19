using Hangfire;
using Harmonic.Hosting;
using Kachuwa.Data;
using Kachuwa.Job;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace demo
{
    class RTMPServerService : ServiceBase
    {
        private string _logFileLocation = @"C:\temp\servicelog.txt";
        public static RtmpServer server = null;
        public static CancellationTokenSource cancellationTokenSource = null;
        private void Log(string logMessage)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_logFileLocation));
            File.AppendAllText(_logFileLocation, DateTime.UtcNow.ToString() + " : " + logMessage + Environment.NewLine);
        }

        protected override void OnStart(string[] args)
        {
            try
            {


                Log("Starting");
                base.OnStart(args);
                server = new RtmpServerBuilder()
                .UseStartup<Startup>()
                .UseWebSocket(c =>
                {
                    c.BindEndPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8080);
                    //ws://127.0.0.1/websocketplay/streamName
                })

                .Build();
                cancellationTokenSource = new CancellationTokenSource();

                var listenerTask = server.StartAsync(cancellationTokenSource.Token);
               // listenerTask.Wait();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log("Error" + e.Message);
            }
        }

        protected override void OnStop()
        {
            Log("Stopping");
            cancellationTokenSource.Cancel();
            try
            {
                foreach (Process proc in Process.GetProcessesByName("RTMPServer"))
                {
                    proc.Kill();
                }
            }
            catch (Exception e)
            {
                Log("Error" + e.Message);
            }
            base.OnStop();
        }

        protected override void OnPause()
        {
            Log("Pausing");
            base.OnPause();
        }
    }
    class Program
    {
        private static IServiceProvider _serviceProvider;
        static async Task Main(string[] args)
        {

            var builder1 = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            IServiceCollection services1 = new ServiceCollection();
            _serviceProvider = services1.BuildServiceProvider();
            var dbfactory = new MsSQLFactory(config, _serviceProvider);
            DbFactoryProvider.SetCurrentDbFactory(dbfactory);
            var jobConnectionString = config["ConnectionStrings:DefaultConnection"];
            string jCon = config[$"ConnectionStrings:JobConnection"];
            services1.AddHangfire(x => x.UseSqlServerStorage(jCon));

            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage(jCon);
            // GlobalConfiguration.Configuration.UseSqlServerStorage("connection_string");


            services1.TryAddSingleton<IKachuwaJobEngineStarter, HangFireEngineStarter>();
            services1.TryAddSingleton<IKachuwaJobRunner, HangFireJobRunner>();
            //using (var server = new BackgroundJobServer())
            //{
            //    Console.WriteLine("Hangfire Server started. Press any key to exit...");
            //    //Console.ReadKey();
            //}

            // Run with console or service


            //#endif
            // DI.Init();
             ServiceBase.Run(new RTMPServerService());

            //var server = new RtmpServerBuilder()
            //    .UseStartup<Startup>()
            //    .UseWebSocket(c =>
            //    {
            //        c.BindEndPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8080);
            //        //ws://127.0.0.1/websocketplay/streamName
            //    })

            //    .Build();
            //var tsk = server.StartAsync();
            //tsk.Wait();
        }
    }



}
