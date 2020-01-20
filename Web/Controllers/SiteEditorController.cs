using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using Cydon.Data.Base;
using Cydon.Data.CySys;
using Cydon.Data.Security;
using Cydon.Data.Security.Cache;
using Cydon.Data.World;
using Web.Base;

namespace Web.Controllers
{
    [CydonAuthorization]
    public class SiteEditorController : BaseController
    {
        private SitePermissionUser permission;
        protected override void PreActionCheck(ActionExecutingContext filterContext, SessionCache.CachedSession cachedSession)
        {
            Context context = new Context();
            User user = context.Users.First(u => u.UserID == cachedSession.UserID);
            permission = context.SitePermissionUsers.FirstOrDefault(spu => spu.UserID == UserID);
            
            if (permission == null || (!permission.CanAddCountries && !permission.CanDeleteCountries && !permission.CanManagePermissions))
            {
                filterContext.Result = new HttpUnauthorizedResult("You don't have permission to use the Site Editor");
            }
        }

        // GET: Editor
        public ActionResult Index()
        {
            Context context = new Context();
            SitePermissionUser sitePermissionUser = context.SitePermissionUsers.Where(spu => spu.UserID == UserID).FirstOrDefault();
            return View(sitePermissionUser ?? new SitePermissionUser());
        }

        public ActionResult CountryList()
        {
            Context context = new Context();
            SitePermissionUser sitePermissionUser = context.SitePermissionUsers.Where(spu => spu.UserID == UserID).FirstOrDefault();
            ViewData["Permission"] = sitePermissionUser ?? new SitePermissionUser();
            return View(new List<Country>(context.Countries));
        }

        public ActionResult CountryDetails(long? id)
        {
            Context context = new Context();
            Country country = context.Countries.FirstOrDefault(c => c.CountryID == id);

            ViewData["SelectableUsers"] = context.Users;

            return View(country);
        }

        public ActionResult CountryAdd(Country country)
        {
            if (!permission.CanAddCountries)
            {
                return new HttpUnauthorizedResult("User does not have permission to Add Countries");
            }

            Context context = new Context();
            context.Countries.Add(country);

            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    transaction.Rollback();
                    return ModalSaveFailedResult(ex);
                }

                // Create default page and navigation
                Page page = new Page();
                page.CountryID = country.CountryID.Value;
                page.Name = "Index";
                context.Pages.Add(page);

                try
                {
                    context.SaveChanges();
                }
                catch(DbEntityValidationException ex)
                {
                    transaction.Rollback();
                    return ModalSaveFailedResult(ex);
                }

                Navigation navigation = new Navigation();
                navigation.Text = country.Name;
                navigation.PageID = page.PageID.Value;
                context.Navigations.Add(navigation);

                try
                {
                    context.SaveChanges();
                }
                catch(DbEntityValidationException ex)
                {
                    transaction.Rollback();
                    return ModalSaveFailedResult(ex);
                }

                transaction.Commit();
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult CountryDetails(Country country)
        {
            if (country == null)
            {
                throw new NotImplementedException("Object creation should go through add action");
            }

            Context context = new Context();
            Country existingCountry = context.Countries.FirstOrDefault(c => c.CountryID == country.CountryID);
            existingCountry.Name = country.Name;

            try
            {
                context.SaveChanges();
            }
            catch(DbEntityValidationException ex)
            {
                ModelState.AddErrors(ex);
            }

            ViewData["SaveSuccessful"] = ModelState.IsValid;
            ViewData["SelectableUsers"] = context.Users;
            
            return View(existingCountry);
        }

        public ActionResult CountryDelete(long? id)
        {
            if (!permission.CanDeleteCountries)
            {
                return new HttpUnauthorizedResult("User does not have permission to Delete Countries");
            }

            Context context = new Context();
            Country country = context.Countries.FirstOrDefault(c => c.CountryID == id);
            if (country != null)
            {
                context.Pages.RemoveRange(country.Pages);
                context.Countries.Remove(country);
                context.SaveChanges();
            }

            return RedirectToAction("CountryList");
        }

        public ActionResult SitePermissionUserList()
        {
            if (!permission.CanManagePermissions)
            {
                return new HttpUnauthorizedResult("User does not have permission to edit permissions");
            }

            Context context = new Context();

            ViewData["AvailableUsers"] = context.Users.Except(context.SitePermissionUsers.Select(spu => spu.User));

            return View(context.SitePermissionUsers.OrderBy(spu => spu.User.Username).ToList());
        }

        public ActionResult SitePermissionUserAdd(SitePermissionUser sitePermissionUser)
        {
            if (!permission.CanManagePermissions)
            {
                return new HttpUnauthorizedResult("User does not have permission to edit permissions");
            }

            Context context = new Context();
            context.SitePermissionUsers.Add(sitePermissionUser);

            try
            {
                context.SaveChanges();
            }
            catch(DbEntityValidationException ex)
            {
                return ModalSaveFailedResult(ex);
            }

            return Json(new { success = true });
        }

        public ActionResult SitePermissionUserDetails(long? id)
        {
            if (!permission.CanManagePermissions)
            {
                return new HttpUnauthorizedResult("User does not have permission to edit permissions");
            }

            SitePermissionUser sitePermissionUser = new SitePermissionUser();
            Context context = new Context();
            if (id != null)
            {
                sitePermissionUser = context.SitePermissionUsers.FirstOrDefault(spu => spu.SitePermissionUserID == id);
                if (sitePermissionUser == null)
                {
                    return new HttpNotFoundResult("Site Permission User could not be found");
                }
            }

            IEnumerable<User> usersNotSelected = context.Users.Except(context.SitePermissionUsers.Where(spu => spu.UserID != sitePermissionUser.UserID).Select(spu => spu.User));
            ViewData["AvailableUsers"] = usersNotSelected;

            return View(sitePermissionUser);
        }

        [HttpPost]
        public ActionResult SitePermissionUserDetails(SitePermissionUser user)
        {
            Context context = new Context();
            SitePermissionUser databaseSPU = context.SitePermissionUsers.FirstOrDefault(u => u.SitePermissionUserID == user.SitePermissionUserID);
            if (databaseSPU == null)
            {
                return new HttpNotFoundResult("Could not find Site Permission User in database");
            }

            databaseSPU.CanAddCountries = user.CanAddCountries;
            databaseSPU.CanDeleteCountries = user.CanDeleteCountries;
            databaseSPU.CanManagePermissions = user.CanManagePermissions;

            try
            {
                context.SaveChanges();
            }
            catch(DbEntityValidationException ex)
            {
                ModelState.AddErrors(ex);
            }

            IEnumerable<User> usersNotSelected = context.Users.Except(context.SitePermissionUsers.Where(spu => spu.UserID != user.UserID).Select(spu => spu.User));
            ViewData["AvailableUsers"] = usersNotSelected;

            ViewData["SaveSuccessful"] = ModelState.IsValid;

            return View(databaseSPU);
        }

        public ActionResult SitePermissionUserDelete(long? id)
        {
            if (!permission.CanManagePermissions)
            {
                return new HttpUnauthorizedResult("User does not have permission to manage Site Permission Users");
            }

            if (id == null)
            {
                return RedirectToAction("SitePermissionUserList");
            }

            Context context = new Context();
            SitePermissionUser sitePermissionUser = context.SitePermissionUsers.FirstOrDefault(spu => spu.SitePermissionUserID == id);
            if (sitePermissionUser != null)
            {
                context.SitePermissionUsers.Remove(sitePermissionUser);
                context.SaveChanges();
            }

            return RedirectToAction("SitePermissionUserList");
        }

        public ActionResult UserList()
        {
            Context context = new Context();

            return View(context.Users.ToList());
        }

        public ActionResult UserDelete(long? id)
        {
            Context context = new Context();
            User user = context.Users.FirstOrDefault(u => u.UserID == id);
            if (user != null)
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }

            return RedirectToAction("UserList");
        }

        public ActionResult UserDetails(long? id)
        {
            Context context = new Context();
            User user = context.Users.FirstOrDefault(u => u.UserID == id);

            return View(user);
        }

        [HttpPost]
        public ActionResult UserDetails(User user)
        {
            if (user.UserID == null)
            {
                throw new NotImplementedException("User creation should go through modal");
            }

            if (user.IsDiscordUser)
            {
                return View(user);
            }

            Context context = new Context();
            User existingUser = context.Users.First(u => u.UserID == user.UserID);
            existingUser.Username = user.Username;
            
            try
            {
                context.SaveChanges();
            }
            catch(DbEntityValidationException ex)
            {
                ModelState.AddErrors(ex);
            }

            ViewData["SaveSuccessful"] = ModelState.IsValid;

            return View(existingUser);
        }

        public ActionResult CountryRoleAdd(CountryRole countryRole)
        {
            if (!permission.CanAddCountries)
            {
                return new HttpUnauthorizedResult("User does not have permission to Add Country Roles");
            }

            Context context = new Context();

            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.CountryRoles.Add(countryRole);

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        return ModalSaveFailedResult(ex);
                    }

                    Dictionary<string, string> errors = new Dictionary<string, string>();
                    Dictionary<long?, string> keyByUserID = new Dictionary<long?, string>();
                    foreach (string key in Request.Form.AllKeys.Where(aKey => aKey.StartsWith("CountryRoleUser-")))
                    {
                        if (!long.TryParse(Request.Form[key], out long userID))
                        {
                            errors.Add(key, "User ID is not valid");
                            continue;
                        }

                        CountryRoleUser countryRoleUser = new CountryRoleUser();
                        countryRoleUser.CountryRoleID = countryRole.CountryRoleID.Value;
                        countryRoleUser.UserID = userID;
                        context.CountryRoleUsers.Add(countryRoleUser);

                        if (keyByUserID.ContainsKey(userID))
                        {
                            errors.Add(key, "User must be unique");
                        }

                        keyByUserID[userID] = key;
                    }

                    if (errors.Any())
                    {
                        return Json(new { success = false, errors });
                    }

                    try
                    {
                        context.SaveChanges();
                    }
                    catch(DbEntityValidationException ex)
                    {
                        foreach(DbEntityValidationResult entityValidationResult in ex.EntityValidationErrors)
                        {
                            long? userID = entityValidationResult.Entry.Property("UserID").CurrentValue as long?;

                            if (keyByUserID.ContainsKey(userID))
                            {
                                errors[keyByUserID[userID]] = entityValidationResult.ValidationErrors.FirstOrDefault().ErrorMessage;
                            }
                        }
                    }

                    if (errors.Any())
                    {
                        return Json(new { success = false, errors });
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    throw ex;
                }
            }

            return Json(new { success = true });
        }

        public ActionResult CountryRoleDelete(long? id)
        {
            if (!permission.CanAddCountries)
            {
                return new HttpUnauthorizedResult("User does not have permission to Add Country Roles");
            }

            Context context = new Context();
            CountryRole role = context.CountryRoles.FirstOrDefault(cr => cr.CountryRoleID == id);

            if (role == null)
            {
                return HttpNotFound("Could not find Country Role");
            }

            context.CountryRoles.Remove(role);
            context.SaveChanges();

            return RedirectToAction("CountryDetails", new { id = role.CountryID });
        }
    }
}