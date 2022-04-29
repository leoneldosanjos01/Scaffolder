using System.IO;
using System.Linq;
using System.Text.Json;
using Scaffolder.Models;
using System.Collections.Generic;
using System;

namespace Scaffolder
{
    public class Scaffolders
    {
        public List<DbModels> Models { get; set; }
        public JsonDocument Configurations { get; }

        public Scaffolders(string content)
        {
            // Building the content
            this.Configurations = JsonDocument.Parse(content);

            // Loading the models
            this.Models = Directory.GetFiles(this.Get("Models")?.FirstOrDefault().DbModels).Select(s =>
            {
                var fileInfo = new FileInfo(s);
                return new DbModels
                {
                    Name = fileInfo.Name.Replace(".cs", ""),
                    Path = fileInfo.FullName
                };
            }).ToList();
        }

        public List<Configuration> Get(string key)
        {
            
            try
            {
                // Creating the object
                return JsonSerializer.Deserialize<List<Configuration>>(
                    // Accessing the data
                    this.Configurations.RootElement.GetProperty(key).GetString()
                );
            }
            catch (System.Exception ex)
            {
                Logger.Error("Error: Configuration not found.");
                Logger.Error($"Error Description: { ex.InnerException?.Message ?? ex.Message }");

                return null;
            }
        }

        public bool ModelExists(string name)
        {
            if (this.Models.Any(m => m.Name == name))
                return true;

            Logger.Error("Model not found.");

            Shared.Pause();
            return false;
        }
    }
}