using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace MyWebsite.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
        /*
        public async Task<IActionResult> OnPostDeleteAsync(int deviceId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.DeleteAsync($"RemoveDevice?id={deviceId}");
            return RedirectToPage();
        }*/
    }
}
