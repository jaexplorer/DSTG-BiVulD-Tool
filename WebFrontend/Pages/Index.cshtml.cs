using Backend;
using Microsoft.AspNetCore.Mvc;

namespace WebFrontend
{
    public class IndexModel : Models.FileScanner
    {
        public User User { get; set; }
        public IActionResult OnGet()
        {
            User = getUserFromCookie();
            if (User == null)
            {
                return RedirectToPage("/Login");
            }
            else
            {
                return null;
            }
        }
    }
}
