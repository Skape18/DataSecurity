using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Lab5_6_7.Models.DTOs;
using Lab5_6_7.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab5_6_7.Pages
{
    [Authorize()]
    public class SensitiveDataModel : PageModel
    {
        private readonly ISensitiveDataService _service;

        public SensitiveDataModel(ISensitiveDataService service)
        {
            _service = service;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public void OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sensitiveData = _service.RetrieveAndDecrypt(userId);
            Input = new InputModel {SensitiveData = sensitiveData?.SensitiveData};
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var data = new SensitiveDataDto { SensitiveData = Input.SensitiveData };

            _service.EncryptAndSave(userId, data);

            return Page();
        }

        public class InputModel
        {
            [Required]
            public string SensitiveData { get; set; }
        }
    }
}
