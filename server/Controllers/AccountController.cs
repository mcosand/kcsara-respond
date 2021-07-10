using System.Threading.Tasks;
using Kcsara.Respond.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kcsara.Respond.Controllers
{
  public class AccountController : Controller
  {
    public async Task Login(string returnUrl = "/")
    {
      await HttpContext.ChallengeAsync("kcsara", new AuthenticationProperties() { RedirectUri = returnUrl });
    }

    [Authorize]
    public async Task Logout()
    {
      await HttpContext.SignOutAsync("kcsara", new AuthenticationProperties
      {
        // Indicate here where Auth0 should redirect the user after a logout.
        // Note that the resulting absolute Uri must be added to the
        // **Allowed Logout URLs** settings for the app.
        RedirectUri = "/" //Url.Action("Index", "Home")
      }) ;
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [Route("/api/me")]
    public ApiResultViewModel<UserViewModel> Me()
    {
      return new ApiResultViewModel<UserViewModel>
      {
        Data = User.Identity.IsAuthenticated ? new UserViewModel { Email = User.FindFirst("email").Value, Name = User.Identity.Name } : null
      };
    }
  }
}
