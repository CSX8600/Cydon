using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cydon.Data.Base;
using Cydon.Data.Security;
using Cydon.Data.Security.Cache;
using Web.Base;

namespace Web.Controllers
{
    public class CySysController : BaseController
    {
        // GET: CySys
        public ActionResult SignOut()
        {
            if (!string.IsNullOrEmpty(SessionID))
            {
                Context context = new Context();

                Session session = context.Sessions.FirstOrDefault(sesh => sesh.SessionStateID == SessionID);
                if (session != null)
                {
                    context.Sessions.Remove(session);

                    try
                    {
                        context.SaveChanges();
                    }
                    catch(DbEntityValidationException ex)
                    {
                        // Add logging
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred logging out");
                    }
                }

                if (Request.Cookies["cydonSessionID"] != null)
                {
                    HttpCookie cookie = Request.Cookies["cydonSessionID"];
                    cookie.Expires = DateTime.Now.AddDays(-10);
                    cookie.Domain = "." + Config.INSTANCE.CookieDomain;
                    Response.Cookies.Set(cookie);
                }
            }

            return new RedirectResult(VirtualPathUtility.ToAbsolute("~") + "?signoutReason=Manual");
        }

        public ActionResult RefreshSession()
        {
            var failed = new { success = false };
            var success = new { success = true };

            if (SessionID == null)
            {
                return Json(failed);
            }

            SessionCache sessionCache = Cache.GetCache<SessionCache>();
            SessionCache.CachedSession cachedSession = sessionCache.GetSessionBySessionID(SessionID);

            if (cachedSession == null || cachedSession.Expiration < DateTime.Now)
            {
                return Json(failed);
            }

            cachedSession.ResetSessionExpiration();
            return Json(success);
        }
    }
}