using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Scaffolder.Models;

namespace Scaffolder.Scaffold
{
    public class RunAllScaffold : GenerationConditions
    {
        public Scaffolders Scaffolders { get; set; }

        private string name;

        public RunAllScaffold()
        {
            this.name = "Run All at Once";
            Console.Clear();

            Logger.Log(
                string.Join(" -> ", new[] {
                    $"Project",
                    $"({ Application.Instance.SelectedProject.AppName }|Yellow)",
                    $"({ this.name }|Yellow)"
                })
            );

            Logger.Log("");

            Logger.Log("\nType the Class Name: ");
            var name = Console.ReadLine();

            name.Split(",").Select(s => s.Trim()).ToList().ForEach(model =>
            {
                var scaffolder = Application.Instance.SelectedProject.GetScaffolders();
                
                // Essential
                if (!scaffolder.ModelExists(model)) return;

                // Getting the actually dictionary of the data 
                var keys = scaffolder.Configurations.RootElement
                            .EnumerateObject().Where(x => x.Name != "Models").Select(x => x.Name).ToList();

                // TODO Code logic here
                keys.ForEach(scaffold =>
                {
                    try
                    {
                        IEnumerable<Type> types = Assembly.GetExecutingAssembly().ExportedTypes;
                        var type = types.FirstOrDefault(x => x.Name == $"{ scaffold }Scaffold");

                        if (type == null) 
                        {
                            Logger.Error($"Scaffolder Class Not Found `{ scaffold }Scaffold`");
                            Shared.Pause();
                        };

                        // Instantiating the scaffolder class chosed
                        var @class = Activator.CreateInstance(type);
        
                        type.GetMethod("Run").Invoke(@class, new[] { model });
                    }
                    catch
                    {
                        Logger.Error("Unavailable option, try again!");
                    }
                });
            });
        }
    }
}