using Backend;
using Microsoft.AspNetCore.Mvc;

namespace WebFrontend
{
	public class IndexModel : Models.FileScanner
	{
		public new User User { get; set; }

		public IActionResult OnGet()
		{
			User = GetUserFromCookie();

			if (User == null)
			{
				return RedirectToPage("/Login");
			}

			return null;
		}
	}
}
