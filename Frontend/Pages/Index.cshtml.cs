using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Net.Http.Json;

namespace MyWebsite.Pages
{
    using FeatureHubSDK;
    public class IndexModel : PageModel
    {
        public bool AddDeviceEnabled { get; set; } = true;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            var featureConfig = new EdgeFeatureHubConfig("http://featurehub:8085", "your-api-key");

            // Blocking call (can be improved with caching if performance matters)
            var context = featureConfig.NewContext().Build().GetAwaiter().GetResult();
            AddDeviceEnabled = context["AddDevice"].IsEnabled;
        }

        public List<Device> Devices { get; set; } = new();

        [BindProperty]
        public Device NewDevice { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.GetAsync("GetAllDevices");
            if (response.IsSuccessStatusCode)
            {
                Devices = await response.Content.ReadFromJsonAsync<List<Device>>() ?? new();
            }

        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.PostAsJsonAsync("AddDevice", NewDevice);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage();
            }

            ModelState.AddModelError(string.Empty, "Failed to add device.");
            return Page();
        }
        
        public async Task<IActionResult> OnPostDeleteAsync(int deviceId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.DeleteAsync($"RemoveDevice?id={deviceId}");
            return RedirectToPage();
        }
    }
}
