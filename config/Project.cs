using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Scaffolder
{
    public class Project : OptionSelector
    {
        public string AppName { get; set; }
        private string mAppPath;
        public string AppPath { 
            get => mAppPath; 
            set => mAppPath = Path.Combine((value ?? "").Split("/").ToArray());
        }

        public dynamic Scaffolders { get; set; }

        public Scaffolders GetScaffolders()
        {
            if (this.Scaffolders == null)
                throw new System.Exception(
                    Logger.Error("Invalid value in Applications:[AppName]:Scaffolders, it must contain the Scaffolder structure!")
                );

            return new Scaffolders(this.Scaffolders.ToString());
        }

        public string UserSelectedOption(string header)
        {
            // Getting the actually dictionary of the data 
            var keys = this.GetScaffolders().Configurations.RootElement
                            .EnumerateObject().Where(x => x.Name != "Models").Select(x => x.Name).ToList();
            
            // Extra Options
            keys.Add("RunAll");
            keys.Add("Exit");

        Beginning:

            int selectedOptionIndex = -1;

            Console.Clear();

            Logger.Log(header);

            // Reading the selected option
            var typedKey = base.Select(keys);

            // If invalid
            if (!int.TryParse(typedKey.ToString(), out selectedOptionIndex) ||
                (keys.Count() <= (selectedOptionIndex - 1)))
                goto Beginning;

            if (selectedOptionIndex == 0)
                return null;

            var fixedIndex = selectedOptionIndex - 1;

            return keys.ElementAt(fixedIndex);
        }
    }
}