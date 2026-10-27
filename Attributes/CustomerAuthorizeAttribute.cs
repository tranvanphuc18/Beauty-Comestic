using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace nv3.Attributes
{
    /// <summary>Đặt trên Controller/Action: chưa đăng nhập thì chuyển sang trang Login.</summary>
    public class CustomerAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Session != null && httpContext.Session["CustomerID"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new
                {
                    controller = "Account",
                    action = "Login",
                    returnUrl = filterContext.HttpContext.Request.RawUrl
                }));
        }
    }
}
