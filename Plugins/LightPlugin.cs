using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using sktodo.Models;

namespace sktodo.Plugins
{
    public class LightPlugin
    {
        // In-memory storage for lights
        private readonly List<Light> lights = new()
        {
            new Light { Id = 1, Name = "Living Room Light", IsOn = false },
            new Light { Id = 2, Name = "Kitchen Light", IsOn = true },
            new Light { Id = 3, Name = "Bedroom Light", IsOn = false }
        };

        [KernelFunction("get_lights")]
        [Description("Gets a list of lights and their current state")]
        [return: Description("An array of lights")]
        public async Task<List<Light>> GetLightsAsync()
        {
            return lights;
        }

        [KernelFunction("add_light")]
        [Description("Adds a new light")]
        [return: Description("The added light")]
        public async Task<Light> AddLightAsync(string name, bool isOn)
        {
            var newLight = new Light
            {
                Id = lights.Count > 0 ? lights.Max(l => l.Id) + 1 : 1,
                Name = name,
                IsOn = isOn
            };

            lights.Add(newLight);
            return newLight;
        }

        [KernelFunction("change_light_state")]
        [Description("Changes the state of a light")]
        [return: Description("The updated light state; null if the light does not exist")]
        public async Task<Light?> ChangeLightStateAsync(int id, bool isOn)
        {
            var light = lights.FirstOrDefault(l => l.Id == id);

            if (light == null) return null;

            light.IsOn = isOn;
            return light;
        }

        [KernelFunction("update_light")]
        [Description("Updates an existing light")]
        [return: Description("The updated light; null if the light does not exist")]
        public async Task<Light?> UpdateLightAsync(int id, string name, bool isOn)
        {
            var light = lights.FirstOrDefault(l => l.Id == id);

            if (light == null) return null;

            light.Name = name;
            light.IsOn = isOn;
            return light;
        }

        [KernelFunction("delete_light")]
        [Description("Deletes a light")]
        [return: Description("True if the light was deleted; false if the light does not exist")]
        public async Task<bool> DeleteLightAsync(int id)
        {
            var light = lights.FirstOrDefault(l => l.Id == id);

            if (light == null) return false;

            lights.Remove(light);
            return true;
        }
    }

        public class Light
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("isOn")]
        public bool IsOn { get; set; }
    }
}
