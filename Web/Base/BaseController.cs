using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cydon.Data.Base;
using Cydon.Data.Security;
using Cydon.Data.Security.Cache;
using Web.ViewEngine.ViewParts;
using Web.ViewEngine.ViewParts.Base;

namespace Web.Base
{
    public abstract class BaseController : Controller
    {
        protected string SessionID { get; private set; }
        protected long? UserID { get; private set; }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SessionID = filterContext.RequestContext.HttpContext.Request.Cookies["cydonSessionID"]?.Value ?? string.Empty;

            SessionCache sessionCache = Cache.GetCache<SessionCache>();
            if (filterContext.RequestContext.HttpContext.Request.QueryString.AllKeys.Contains("forceSessionRefresh"))
            {
                sessionCache.ForceRefreshSession(SessionID);
            }

            SessionCache.CachedSession cachedSession = sessionCache.GetSessionBySessionID(SessionID);

            object authorizationAttribute = GetType().GetCustomAttributes(typeof(CydonAuthorizationAttribute), true).FirstOrDefault();
            
            if (authorizationAttribute == null)
            {
                authorizationAttribute = filterContext.ActionDescriptor.GetCustomAttributes(typeof(CydonAuthorizationAttribute), true).FirstOrDefault();
            }

            if (authorizationAttribute == null)
            {
                if (cachedSession != null && cachedSession.Expiration >= DateTime.Now)
                {
                    UserID = cachedSession.UserID;
                }
                return;
            }

            if (cachedSession == null || cachedSession.Expiration < DateTime.Now)
            {
                string redirect = Config.INSTANCE.UnauthenticatedRedirect + "?redirectUrl=" + Uri.EscapeDataString(filterContext.RequestContext.HttpContext.Request.Url.ToString());
                filterContext.Result = Redirect(redirect);

                return;
            }

            UserID = cachedSession.UserID;

            cachedSession.ResetSessionExpiration();

            if (filterContext.Result == null)
            {
                PreActionCheck(filterContext, cachedSession);
            }
        }

        protected virtual void PreActionCheck(ActionExecutingContext filterContext, SessionCache.CachedSession cachedSession) { }

        protected ActionResult ModalSaveFailedResult(DbEntityValidationException ex)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            foreach (DbValidationError error in ex.EntityValidationErrors.SelectMany(evr => evr.ValidationErrors))
            {
                errors.Add(error.PropertyName, error.ErrorMessage);
            }

            return Json(new { success = false, errors });
        }

        public virtual List<ViewPart> GetRightAlignViewParts()
        {
            List<ViewPart> retVal = new List<ViewPart>();
            if (string.IsNullOrEmpty(SessionID))
            {
                retVal.Add(new Anchor()
                {
                    Href = Config.INSTANCE.UnauthenticatedRedirect,
                    Parts =
                    {
                        new Span()
                        {
                            Class = { "fas", "fa-sign-in-alt" }
                        }
                    },
                    TextAfterContent = "Sign In"
                });

                return retVal;
            }

            Context context = new Context();
            User user = context.Users.FirstOrDefault(u => u.UserID == UserID);

            if (user == null)
            {
                retVal.Add(new Anchor()
                {
                    Href = Config.INSTANCE.UnauthenticatedRedirect,
                    TextAfterContent = "Sign In"
                });

                return retVal;
            }

            if (user.CountryRoleUsers.Any(cru => cru.CountryRole.CanAddPages || cru.CountryRole.CanDeletePages || cru.CountryRole.CountryRolePageElements.Any(crpe => crpe.CanEdit)))
            {
                retVal.Add(new Anchor()
                {
                    Href = VirtualPathUtility.ToAbsolute("~/PageEditor"),
                    TextAfterContent = "Page Editor"
                });
            }

            if (user.SitePermissionUsers.Any(spu => spu.CanAddCountries || spu.CanDeleteCountries || spu.CanManagePermissions))
            {
                retVal.Add(new Anchor()
                {
                    Href = VirtualPathUtility.ToAbsolute("~/SiteEditor"),
                    TextAfterContent = "Site Editor"
                });
            }

            retVal.Add(new Div()
            {
                Class = { "dropdown-divider" }
            });

            retVal.Add(new Anchor()
            {
                Parts =
                {
                    new Span()
                    {
                        Class = { "fas", "fa-sign-out-alt" }
                    }
                },
                Href = VirtualPathUtility.ToAbsolute("~/CySys/SignOut"),                
                TextAfterContent = "Signout"
            });

            return retVal;
        }
    }
}