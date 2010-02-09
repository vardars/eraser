using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ComLib;
using ComLib.Application;
using ComLib.Arguments;
using ComLib.Logging;
using ComLib.Environments;
using ComLib.EmailSupport;
using ComLib.Configuration;
using ComLib.IO;
using ComLib.CodeGeneration;
using ComLib.Models;
using ComLib.LocationSupport;
using CommonLibrary.WebModules.Events;


namespace CommonLibrary.CodeGeneration
{
    /// <summary>
    /// Full sample application using the CommonLibrary.NET framework.
    /// </summary>
    public class CodeGeneratorApplication : App
    {
        /// <summary>
        /// Run the application via it's interface ( Init - Execute - Shutdown )
        /// using the static Run utility method.
        /// </summary>
        /// <param name="args">command line arguments.
        /// e.g. -env:Prod,Dev -date:${today-1} -config:config\prod.config,config\dev.config -source:Reuters 10</param>
        static int Main(string[] args)
        {
            int result = Run(new CodeGeneratorApplication(), args, false, "log,diagnostics").AsExitCode();
            return result;
        }



        /// <summary>
        /// Execute the application.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override BoolMessageItem Execute(object context)
        {
            Log.Info("starting the code generation tests.");

            // Step 1. Generate the code
            ModelContainer models = SampleModels.GetModelContainer();
            ModelContext ctx = new ModelContext() { AllModels = models };
            IList<ICodeBuilder> builders = new List<ICodeBuilder>()
            {
                // This generates the Database tables in SQL - Server.
                // You connection string in ModelBuilderSettings.Connection must be set.
                new CodeBuilderDb(ctx.AllModels.Settings.Connection),
                new CodeBuilderDomain(),              
            };
            BoolMessage message = CodeBuilder.Process(ctx, builders);
            Console.WriteLine("Code generation Sucess : {0} - {1}", message.Success, message.Message);
            Log.Info("finishing the code generation.");

            /*
            // Step 2 : Init the Events active record.
            EventRepository repo = new EventRepository(ctx.AllModels.Settings.Connection);            
            Events.Init(new EventService(repo, new EventValidator(), new EventSettings() { EnableAuthentication = true, EnableValidation = true }));
            
            // Step 3: Run the code to create / update / get Events using activerecord.     
            CreateEvents(20);
            UpdateEvents();
            PagedList<Event> items1 = Events.GetRecent(1, 4).Item;
            PagedList<Event> items2 = Events.Get(2, 4).Item;
            Event ev = Events.Get(4).Item;
            */
            return BoolMessageItem.True;
        }



        /*
        public void CreateEvents(int max)
        {
            for (int count = 0; count < max; count++)
            {
                Event ev = new Event();
                ev.Title = "friends party " + count.ToString();
                ev.Description = "end of month social No. ";
                ev.Address = new Address("105-20 66th road.", "queens", "new york", "ny", "11375");
                ev.Email = "kishore.pr4@gmail.com";
                ev.Phone = "123-456-7890";
                ev.StartDate = DateTime.Today.AddDays(5);
                ev.EndDate = DateTime.Today.AddDays(5);
                ev.Url = "http://www.kprnewyears.com";
                Events.Save(ev);
            }
            
        }


        public void UpdateEvents()
        {
            PagedList<Event> all = Events.Get(1, 30).Item;
            foreach (Event ev in all)
            {
                ev.Title += " updated.";
                Events.Save(ev);
            }
        }
        */
    }
}
