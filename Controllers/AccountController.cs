using System.Web.Mvc;

namespace BeautyCosmetic.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public new ActionResult Profile()
        {
            return View();
        }
    }
}