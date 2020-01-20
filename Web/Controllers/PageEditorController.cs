using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using System.Xml.Linq;
using Cydon.Data.Base;
using Cydon.Data.CySys;
using Cydon.Data.Security;
using Cydon.Data.Security.Cache;
using Cydon.Data.World;
using Web.Base;
using Web.ViewEngine.ViewParts;
using Web.ViewEngine.ViewParts.Base;

namespace Web.Controllers
{
    [CydonAuthorization]
    public class PageEditorController : BaseController
    {
        private CountryRole Permission;
        private long? CountryID = null;
        protected override void PreActionCheck(ActionExecutingContext filterContext, SessionCache.CachedSession cachedSession)
        {
            if (!RouteData.Values.Keys.Contains("countryid"))
            {
                if (filterContext.ActionDescriptor.ActionName == "Index")
                {
                    Permission = null;
                    return;
                }
                else
                {
                    filterContext.Result = HttpNotFound("Country ID was not supplied");
                }
            }

            if (!long.TryParse(RouteData.Values["countryid"] as string, out long countryID))
            {
                filterContext.Result = HttpNotFound("CountryID is not valid");
                return;
            }

            CountryID = countryID;

            Context context = new Context();
            IEnumerable<CountryRole> countryRoles = context.Users.First(u => u.UserID == UserID).CountryRoleUsers.Where(cru => cru.CountryRole.CountryID == CountryID).Select(cru => cru.CountryRole);

            if (!countryRoles.Any(cr => cr.CountryID == CountryID))
            {
                filterContext.Result = new HttpUnauthorizedResult("User does not have access to edit this country");
            }

            Permission = new CountryRole();
            Permission.CanAddPages = countryRoles.Any(cr => cr.CountryID == CountryID && cr.CanAddPages);
            Permission.CanDeletePages = countryRoles.Any(cr => cr.CountryID == CountryID && cr.CanDeletePages);
            Permission.CanUpdatePermissions = countryRoles.Any(cr => cr.CountryID == CountryID && cr.CanUpdatePermissions);

            filterContext.Controller.ViewData["Permission"] = Permission;
        }

        // GET: PageEditor
        public ActionResult Index()
        {
            Context context = new Context();
            User user = context.Users.First(u => u.UserID == UserID);

            return View(user.CountryRoleUsers.Where(cru => cru.CountryRole.CanAddPages || cru.CountryRole.CanDeletePages || cru.CountryRole.CanUpdatePermissions).Select(cru => cru.CountryRole.Country));
        }

        public ActionResult CountryIndex(long? countryid)
        {
            if (Permission == null)
            {
                return new HttpUnauthorizedResult("You do not have permission to access this Country");
            }

            Context context = new Context();
            ViewData["Users"] = context.Users;
            return View(context.Countries.Single(country => country.CountryID == countryid));
        }

        public ActionResult NavigationDetails(long? id)
        {
            if (!Permission.CanAddPages)
            {
                return new HttpUnauthorizedResult("You do not have permission to access Navigations for this Country");
            }

            Context context = new Context();
            Navigation navigation = context.Navigations.FirstOrDefault(n => n.NavigationID == id);

            if (navigation == null)
            {
                return HttpNotFound("Could not find Navigation");
            }

            ViewData["Pages"] = context.Pages.Where(p => p.CountryID == CountryID);

            return View(navigation);
        }

        [HttpPost]
        public ActionResult NavigationDetails(Navigation navigation)
        {
            if (navigation == null || navigation.NavigationID == null)
            {
                throw new NotImplementedException("Navigation creation should go through modal");
            }

            Context context = new Context();

            Country currentCountry = context.Countries.Single(c => c.CountryID == CountryID);
            navigation.ParentNavigationID = currentCountry.Pages.Single(p => p.Navigations.Any(n => n.ParentNavigationID == null)).Navigations.Single(n => n.ParentNavigationID == null).NavigationID;

            Navigation databaseNavigation = context.Navigations.FirstOrDefault(n => n.NavigationID == navigation.NavigationID);
            if (databaseNavigation == null)
            {
                return HttpNotFound("Could not find Navigation in database");
            }

            databaseNavigation.ParentNavigationID = navigation.ParentNavigationID;
            databaseNavigation.PageID = navigation.PageID;
            databaseNavigation.Text = navigation.Text;

            try
            {
                context.SaveChanges();
            }
            catch(DbEntityValidationException ex)
            {
                ModelState.AddErrors(ex);
            }

            ViewData["Pages"] = context.Pages.Where(p => p.CountryID == CountryID);
            ViewData["SaveSuccessful"] = ModelState.IsValid;

            return View(databaseNavigation);
        }

        public ActionResult NavigationAdd(Navigation navigation)
        {
            if (!Permission.CanAddPages)
            {
                return new HttpUnauthorizedResult("User does not have permission to Add navigations");
            }

            Context context = new Context();

            Country currentCountry = context.Countries.Single(c => c.CountryID == CountryID);
            Navigation parentNavigation = currentCountry.Pages.Single(p => p.Navigations.Any(n => n.ParentNavigationID == null)).Navigations.Single(n => n.ParentNavigationID == null);
            navigation.ParentNavigationID = parentNavigation.NavigationID;
            parentNavigation.ChildNavigations.Add(navigation);

            context.Navigations.Add(navigation);

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

        public ActionResult CountryRoleDetails(long? id)
        {
            if (!Permission.CanUpdatePermissions)
            {
                return new HttpUnauthorizedResult("User does not have permission to manage permissions");
            }

            Context context = new Context();
            CountryRole role = context.CountryRoles.FirstOrDefault(cr => cr.CountryRoleID == id);

            if (role == null)
            {
                return HttpNotFound("Could not find Role");
            }

            ViewData["Users"] = context.Users.ToList();

            return View(role);
        }

        [HttpPost]
        public ActionResult CountryRoleDetails(CountryRole countryRole)
        {
            if (!Permission.CanUpdatePermissions)
            {
                return new HttpUnauthorizedResult("User does not have permission to manage permissions");
            }

            if (countryRole.CountryRoleID == null)
            {
                throw new NotImplementedException("Country Role creation should go through modal");
            }

            Context context = new Context();
            CountryRole databaseCountryRole = context.CountryRoles.FirstOrDefault(cr => cr.CountryRoleID == countryRole.CountryRoleID);

            if (databaseCountryRole == null)
            {
                return HttpNotFound("Country Role not found");
            }

            databaseCountryRole.Name = countryRole.Name;
            databaseCountryRole.CanAddPages = countryRole.CanAddPages;
            databaseCountryRole.CanDeletePages = countryRole.CanDeletePages;
            databaseCountryRole.CanUpdatePermissions = countryRole.CanUpdatePermissions;

            try
            {
                context.SaveChanges();
            }
            catch(DbEntityValidationException ex)
            {
                ModelState.AddErrors(ex);
            }

            ViewData["SaveSuccessful"] = ModelState.IsValid;
            ViewData["Users"] = context.Users.ToList();

            return View(databaseCountryRole);
        }

        public ActionResult CountryRoleDelete(long? id)
        {
            if (!Permission.CanUpdatePermissions)
            {
                return new HttpUnauthorizedResult("User does not have access to update permissions");
            }

            Context context = new Context();
            CountryRole countryRole = context.CountryRoles.FirstOrDefault(cr => cr.CountryRoleID == id);

            if (countryRole == null)
            {
                return HttpNotFound("Could not find Role");
            }

            context.CountryRoles.Remove(countryRole);

            context.SaveChanges();

            return RedirectToAction("CountryIndex", new { countryid = CountryID });
        }

        public ActionResult CountryRoleUserAdd(FormCollection collection)
        {
            if (!Permission.CanUpdatePermissions)
            {
                return new HttpUnauthorizedResult("User does not have permission to Add Country Role Users");
            }

            int.TryParse(collection["UserID"], out int userID);
            int.TryParse(collection["CountryRoleID"], out int countryRoleID);
            CountryRoleUser countryRoleUser = new CountryRoleUser();
            countryRoleUser.UserID = userID;
            countryRoleUser.CountryRoleID = countryRoleID;

            Context context = new Context();
            context.CountryRoleUsers.Add(countryRoleUser);

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

        public ActionResult CountryRoleUserDelete(long? id)
        {
            if (!Permission.CanUpdatePermissions)
            {
                return new HttpUnauthorizedResult("User does not have access to update permissions");
            }

            Context context = new Context();
            CountryRoleUser countryRoleUser = context.CountryRoleUsers.FirstOrDefault(cru => cru.CountryRoleUserID == id);

            if (countryRoleUser == null)
            {
                return HttpNotFound("Could not find Role User");
            }

            long? countryRoleID = countryRoleUser.CountryRoleID;

            context.CountryRoleUsers.Remove(countryRoleUser);

            context.SaveChanges();

            return RedirectToAction("CountryDetails", new { countryid = CountryID, id = countryRoleID });
        }

        public ActionResult CountryRoleAdd(CountryRole countryRole)
        {
            if (!Permission.CanUpdatePermissions)
            {
                return new HttpUnauthorizedResult("User does not have permission to add Country Roles");
            }

            countryRole.CountryRoleUsers = new List<CountryRoleUser>();

            foreach(string key in Request.Form.AllKeys.Where(key => key.StartsWith("CountryRoleUser-")))
            {
                if (!int.TryParse(Request.Form[key], out int userID))
                {
                    continue;
                }

                CountryRoleUser countryRoleUser = new CountryRoleUser();
                countryRoleUser.UserID = userID;
                countryRole.CountryRoleUsers.Add(countryRoleUser);
            }

            Context context = new Context();
            context.CountryRoles.Add(countryRole);

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

        public ActionResult NavigationDelete(long? id)
        {
            if (!Permission.CanDeletePages)
            {
                return new HttpUnauthorizedResult("User does not have permission to delete Navigations");
            }

            Context context = new Context();
            Navigation databaseNavigation = context.Navigations.FirstOrDefault(n => n.NavigationID == id);
            if (databaseNavigation == null)
            {
                return HttpNotFound("Could not find Navigation");
            }

            context.Navigations.Remove(databaseNavigation);

            context.SaveChanges();

            return RedirectToAction("CountryIndex", new { @countryid = CountryID });
        }

        public ActionResult PageDetails(long? id)
        {
            if (!Permission.CanAddPages)
            {
                return new HttpUnauthorizedResult("User does not have permission to edit Pages");
            }

            Context context = new Context();
            Cydon.Data.CySys.Page page = context.Pages.FirstOrDefault(p => p.PageID == id);
            if (page == null)
            {
                return HttpNotFound("Could not find Page");
            }

            #region Build Editor Parts
            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            HtmlTextWriter htmlWriter = new HtmlTextWriter(writer);
            
            foreach(PageElement content in page.PageElements.OrderBy(pe => pe.DisplayOrder))
            {
                Div elementDiv = new Div()
                {
                    Class = { "p-1", "mb-1", "bg-light", "rounded" },
                    PageElement = content
                };

                if (!content.CountryRolePageElements.Any(crpe => crpe.CanView && crpe.CountryRole.CountryRoleUsers.Any(cru => cru.UserID == UserID)))
                {
                    elementDiv.Parts.Add(new Header() { HeaderType = Header.HeaderTypes.H6, Text = "You don't have permission to view this Page Element" });
                    elementDiv.Render(htmlWriter);
                    continue;
                }

                XElement xElement = XElement.Parse(content.ElementXML);
                string typeName = xElement.Attribute("type")?.Value;

                if (string.IsNullOrEmpty(typeName))
                {
                    continue;
                }

                Type type = Type.GetType(typeName);
                if (!typeof(PageContentViewPart).IsAssignableFrom(type))
                {
                    continue;
                }

                bool editable = content.CountryRolePageElements.Any(crpe => crpe.CanEdit && crpe.CountryRole.CountryRoleUsers.Any(cru => cru.UserID == UserID));

                if (editable)
                {
                    Span rightAligned = new Span()
                    {
                        Class = { "float-right" }
                    };

                    Button moveUp = new Button()
                    {
                        Class = { "btn", "btn-outline-primary" },
                        Type = "button",
                        OnClick = "movePageElementUp($(this));",
                        ViewPartsBeforeText = new List<ViewPart>()
                        {
                            new Span()
                            {
                                Class = { "fas", "fa-chevron-up" }
                            }
                        }
                    };

                    Button moveDown = new Button()
                    {
                        Class = { "btn", "btn-outline-primary" },
                        Type = "button",
                        OnClick = "movePageElementDown($(this));",
                        ViewPartsBeforeText = new List<ViewPart>()
                        {
                            new Span()
                            {
                                Class = { "fas", "fa-chevron-down" }
                            }
                        }
                    };

                    Button remove = new Button()
                    {
                        Class = { "btn", "btn-danger" },
                        Type = "button",
                        OnClick = "deletePageElement($(this));",
                        ViewPartsBeforeText = new List<ViewPart>()
                        {
                            new Span()
                            {
                                Class = { "fas", "fa-trash" }
                            }
                        }
                    };

                    rightAligned.Parts.Add(moveUp);
                    rightAligned.Parts.Add(moveDown);
                    rightAligned.Parts.Add(remove);

                    elementDiv.Parts.Add(rightAligned);
                }

                Form form = new Form()
                {
                    Id = $"PageElement-{content.PageElementID}",
                    Action = "."
                };

                form.Parts.Add(new Div() { Class = { "genericError", "bg-danger" } });

                PageContentViewPart pageContentViewPart = (PageContentViewPart)Activator.CreateInstance(type);
                pageContentViewPart.PageElement = content;
                pageContentViewPart.EditorMode = true;
                pageContentViewPart.ReadOnly = !editable;
                pageContentViewPart.ReadFromXML(xElement);

                form.Parts.Add(pageContentViewPart);

                form.Parts.Add(new Hidden()
                {
                    Id = "PageElementID",
                    Value = content.PageElementID.ToString()
                });

                form.Parts.Add(new Hidden()
                {
                    Id = "CountryID",
                    Value = content.Page.CountryID.ToString()
                });

                form.Parts.Add(new Hidden()
                {
                    Id = "ElementType",
                    Value = type.Name
                });

                if (editable)
                {
                    form.Parts.Add(new Button()
                    {
                        Type = "button",
                        Class = { "btn", "btn-primary" },
                        ViewPartsBeforeText =
                        {
                            new Span()
                            {
                                Class = { "fas", "fa-save" }
                            }
                        },
                        Text = " Save",
                        OnClick = "savePageElement($(this));"
                    });

                    form.Parts.Add(new Button()
                    {
                        Type = "button",
                        Class = { "btn", "btn-primary", "ml-1" },
                        ViewPartsBeforeText =
                        {
                            new Span()
                            {
                                Class = { "fas", "fa-user-edit" }
                            }
                        },
                        Text = " Permissions",
                        OnClick = "$('#ManagePageElementPermissions').find('input[name=\"PageElementID\"]').val('" + content.PageElementID + "');$('#ManagePageElementPermissions').modal('show');"
                    });
                }

                elementDiv.Parts.Add(form);

                elementDiv.Render(htmlWriter);
            }

            htmlWriter.Flush();
            stream.Position = 0;

            ViewData["Content"] = new StreamReader(stream).ReadToEnd();
            #endregion

            #region Get Add-able Parts
            Type addablePartType = typeof(PageContentViewPart);
            List<string> addableParts = new List<string>();
            foreach(Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t != addablePartType && addablePartType.IsAssignableFrom(t)))
            {
                addableParts.Add(type.Name);
            }

            ViewData["AddableParts"] = addableParts;
            #endregion

            htmlWriter.Close();
            stream.Close();

            return View(page);
        }

        public ActionResult PageElementAdd(FormCollection collection)
        {
            if (!Permission.CanAddPages)
            {
                return new HttpUnauthorizedResult("User does not have permission to Add Page Elements");
            }

            Dictionary<string, string> errors = new Dictionary<string, string>();
            if (!collection.AllKeys.Contains("ElementType"))
            {
                errors.Add("ElementType", "Element Type is a required field");
            }

            int pageID = -1;
            if (!collection.AllKeys.Contains("PageID") || !int.TryParse(collection["PageID"], out pageID))
            {
                errors.Add("ElementType", "Page ID is required");
            }

            if (errors.Any())
            {
                return Json(new { success = false, errors });
            }

            string elementType = collection["ElementType"];

            Type pageContentViewPartType = typeof(PageContentViewPart);
            Type selectedType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).FirstOrDefault(t => t != pageContentViewPartType && pageContentViewPartType.IsAssignableFrom(t) && t.Name == elementType);

            if (selectedType == null)
            {
                errors.Add("ElementType", "Element Type is a required field");

                return Json(new { success = false, errors });
            }

            PageContentViewPart pageContentViewPart = (PageContentViewPart)Activator.CreateInstance(selectedType);
            XElement data = new XElement("element", new XAttribute("type", selectedType.FullName));
            pageContentViewPart.WriteToXML(data);

            Context context = new Context();

            PageElement newPageElement = new PageElement();
            newPageElement.PageID = pageID;
            newPageElement.ElementXML = data.ToString();
            newPageElement.DisplayOrder = (byte)(context.PageElements.Count() + 1);
            newPageElement.CountryRolePageElements = new List<CountryRolePageElement>();

            foreach(CountryRole countryRole in context.CountryRoles.Where(cr => cr.CountryID == CountryID))
            {
                CountryRolePageElement countryRolePageElement = new CountryRolePageElement();
                countryRolePageElement.CountryRoleID = countryRole.CountryRoleID.Value;
                countryRolePageElement.CanView = true;
                countryRolePageElement.CanEdit = true;
                newPageElement.CountryRolePageElements.Add(countryRolePageElement);
            }

            context.PageElements.Add(newPageElement);

            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                return ModalSaveFailedResult(ex);
            }

            return Json(new { success = true });
        }

        public ActionResult PageElementSave(FormCollection collection)
        {
            string elementType = collection["ElementType"];
            Type pageContentViewPartType = typeof(PageContentViewPart);

            Type contentType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).FirstOrDefault(t => t != pageContentViewPartType && pageContentViewPartType.IsAssignableFrom(t) && t.Name == elementType);
            if (contentType == null)
            {
                return Json(new { success = false, errors = new Dictionary<string, string>() { { "Generic", "Element Type not found" } } });
            }

            string pageElementIDString = collection["PageElementID"];
            if (!long.TryParse(pageElementIDString, out long pageElementID))
            {
                return Json(new { success = false, errors = new Dictionary<string, string>() { { "Generic", "Page Element ID not found" } } });
            }

            Context context = new Context();
            PageElement pageElement = context.PageElements.FirstOrDefault(pe => pe.PageElementID == pageElementID);
            if (pageElement == null)
            {
                return Json(new { success = false, errors = new Dictionary<string, string>() { { "Generic", "Page Element ID not found" } } });
            }

            PageContentViewPart pageContentViewPart = (PageContentViewPart)Activator.CreateInstance(contentType);
            pageContentViewPart.PageElement = pageElement;
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach(string key in collection.AllKeys)
            {
                values[key] = collection[key];
            }

            pageContentViewPart.UpdateFromFormInput(values);
            XElement currentElementXML = new XElement("element", new XAttribute("type", pageContentViewPart.GetType().FullName));
            pageContentViewPart.WriteToXML(currentElementXML);

            pageElement.ElementXML = currentElementXML.ToString();
            
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

        public ActionResult PageElementDelete(long? id)
        {
            if (!Permission.CanDeletePages)
            {
                return new HttpUnauthorizedResult("User does not have permission to Delete Page Elements");
            }

            Context context = new Context();
            PageElement pageElement = context.PageElements.FirstOrDefault(pe => pe.PageElementID == id);
            if (pageElement == null)
            {
                return HttpNotFound();
            }

            byte currentDisplayOrder = pageElement.DisplayOrder;

            context.PageElements.Remove(pageElement);

            currentDisplayOrder++;
            while (true)
            {
                PageElement nextPageElement = context.PageElements.FirstOrDefault(pe => pe.PageID == pageElement.PageID && pe.DisplayOrder == currentDisplayOrder);

                if (nextPageElement == null)
                {
                    break;
                }

                nextPageElement.DisplayOrder--;
                currentDisplayOrder++;
            }

            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                return ModalSaveFailedResult(ex);
            }

            return Json(new { success = true });
        }

        public ActionResult PageElementMoveUp(long? id)
        {
            if (!Permission.CanAddPages)
            {
                return new HttpUnauthorizedResult("User does not have permission to update Page Elements");
            }

            Context context = new Context();
            PageElement pageElement = context.PageElements.FirstOrDefault(pe => pe.PageElementID == id);

            if (pageElement == null)
            {
                return HttpNotFound();
            }

            if (pageElement.DisplayOrder <= 1)
            {
                return Json(new { success = true });
            }

            PageElement existingPageElementAtPosition = context.PageElements.FirstOrDefault(pe => pe.PageID == pageElement.PageID && pe.DisplayOrder == pageElement.DisplayOrder - 1);
            if (existingPageElementAtPosition == null)
            {
                return HttpNotFound();
            }

            pageElement.DisplayOrder--;
            existingPageElementAtPosition.DisplayOrder++;

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

        public ActionResult PageElementMoveDown(long? id)
        {
            if (!Permission.CanAddPages)
            {
                return new HttpUnauthorizedResult("User does not have permission to update Page Elements");
            }

            Context context = new Context();
            PageElement pageElement = context.PageElements.FirstOrDefault(pe => pe.PageElementID == id);

            if (pageElement == null)
            {
                return HttpNotFound();
            }

            PageElement existingPageElementAtPosition = context.PageElements.FirstOrDefault(pe => pe.PageID == pageElement.PageID && pe.DisplayOrder == pageElement.DisplayOrder + 1);
            if (existingPageElementAtPosition == null)
            {
                return HttpNotFound();
            }

            if (existingPageElementAtPosition.DisplayOrder <= 1)
            {
                return Json(new { success = true });
            }

            pageElement.DisplayOrder++;
            existingPageElementAtPosition.DisplayOrder--;

            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                return ModalSaveFailedResult(ex);
            }

            return Json(new { success = true });
        }

        public ActionResult PageAdd(FormCollection collection)
        {
            if (!Permission.CanAddPages)
            {
                return new HttpUnauthorizedResult("User does not have permission to Add pages");
            }

            Dictionary<string, string> errors = new Dictionary<string, string>();
            if (!collection.AllKeys.Contains("Name"))
            {
                errors.Add("Name", "Name is a required field");
            }

            if (errors.Any())
            {
                return Json(new { success = false, errors });
            }

            Cydon.Data.CySys.Page page = new Cydon.Data.CySys.Page();
            page.Name = collection["Name"];
            page.CountryID = CountryID.Value;

            Context context = new Context();
            context.Pages.Add(page);

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

        public ActionResult PageDelete(long? id)
        {
            if (!Permission.CanDeletePages)
            {
                return new HttpUnauthorizedResult("User does not have permission to delete Pages");
            }

            Context context = new Context();
            Cydon.Data.CySys.Page page = context.Pages.FirstOrDefault(p => p.PageID == id);

            if (page == null)
            {
                return HttpNotFound("Could not find Page");
            }

            context.Pages.Remove(page);

            context.SaveChanges();

            return RedirectToAction("CountryIndex", new { countryid = CountryID });
        }

        public ActionResult PageElementLoadPerms(FormCollection collection)
        {
            if (!collection.AllKeys.Contains("PageElementID"))
            {
                return Json(new { success = false });
            }

            string pageElementIDString = collection["PageElementID"];
            if (!int.TryParse(pageElementIDString, out int pageElementID))
            {
                return Json(new { success = false });
            }

            Context context = new Context();
            PageElement pageElement = context.PageElements.FirstOrDefault(pe => pe.PageElementID == pageElementID);
            if (pageElement == null)
            {
                return Json(new { success = false });
            }

            List<ViewPart> viewPartsToRender = new List<ViewPart>();

            viewPartsToRender.Add(GetPageElementPermAuthenticationViewParts(pageElement));
            viewPartsToRender.AddRange(GetPageElementPermCountryRolesViewParts(pageElement));            

            string renderedContent = "";
            using (MemoryStream stream = new MemoryStream())
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(stream))
            using (HtmlTextWriter htmlWriter = new HtmlTextWriter(writer))
            {
                foreach (ViewPart part in viewPartsToRender)
                {
                    part.Render(htmlWriter);
                }

                htmlWriter.Flush();
                stream.Position = 0;
                renderedContent = reader.ReadToEnd();
            }

            return Json(new
            {
                success = true,
                content = renderedContent
            });
        }

        private Div GetPageElementPermAuthenticationViewParts(PageElement pageElement)
        {
            Div whoCanSeeDiv = new Div()
            {
                Class = { "form-group" }
            };

            Label whoCanSeeLabel = new Label() { Text = "Who can see this Element?", Class = { "d-block" } };
            RadioButton everyone = new RadioButton()
            {
                Inline = true,
                Name = "IsForAuthenticated",
                Value = "false",
                Id = "IsForAuthenticatedFalse",
                Text = "Everyone",
                Checked = !pageElement.ForAuthenticatedOnly
            };

            RadioButton authenticatedOnly = new RadioButton()
            {
                Inline = true,
                Name = "IsForAuthenticated",
                Value = "true",
                Id = "IsForAuthenticatedTrue",
                Text = "Authenticated Users Only",
                Checked = pageElement.ForAuthenticatedOnly
            };

            whoCanSeeDiv.Parts.AddRange(new List<ViewPart>()
            {
                whoCanSeeLabel,
                everyone,
                authenticatedOnly
            });
            return whoCanSeeDiv;
        }

        private IEnumerable<ViewPart> GetPageElementPermCountryRolesViewParts(PageElement pageElement)
        {
            yield return new Header()
            {
                HeaderType = Header.HeaderTypes.H4,
                Text = "Who can make edits to this Element?"
            };

            Table table = new Table();
            Table.Row headerRow = table.Header.CreateRow();
            Table.Column nameHeader = headerRow.CreateColumn();
            nameHeader.Class.Add("col-sm-11");
            nameHeader.Parts.Add(new Literal()
            {
                LiteralString = "Country Role"
            });

            Table.Column buttonHeader = headerRow.CreateColumn();
            buttonHeader.Class.Add("col-sm-1");
            buttonHeader.Parts.Add(new Literal()
            {
                LiteralString = "Delete"
            });

            for(int i = 0; i < pageElement.CountryRolePageElements.Count; i++)
            {
                CountryRolePageElement perm = pageElement.CountryRolePageElements.ElementAt(i);
                string idPrefix = string.Format("perm-{0}-", perm.CountryRolePageElementID.Value);

                Table.Row row = table.Body.CreateRow();
                Table.Column nameCol = row.CreateColumn();
                nameCol.Class.Add("col-sm-11");
                nameCol.Parts.Add(new Literal() { LiteralString = perm.CountryRole.Name });
                nameCol.Parts.Add(new Checkbox()
                {
                    Id = idPrefix + "canview",
                    Text = "Can View?",
                    Checked = perm.CanView
                });
                nameCol.Parts.Add(new Checkbox()
                {
                    Id = idPrefix + "canedit",
                    Text = "Can Edit?",
                    Checked = perm.CanEdit
                });
                nameCol.Parts.Add(new Hidden()
                {
                    Id = idPrefix + "countryrolepagelementid",
                    Value = perm.CountryRolePageElementID.Value.ToString()
                });

                Table.Column removeCol = row.CreateColumn();
                removeCol.Class.Add("col-sm-1");
                Button removeButton = new Button()
                {
                    Type = "button",
                    Class = { "btn", "btn-danger" },
                    ViewPartsBeforeText = new List<ViewPart>()
                    {
                        new Span() { Class = { "fas", "fa-trash" } }
                    },
                    Text = " Delete",
                    OnClick = "managePageElementPermissionsRemoveCountryRole($(this));"
                };
                removeCol.Parts.Add(removeButton);
            }

            Table.Row templateRow = table.Body.CreateRow();
            templateRow.Class.Add("d-none");
            templateRow.Class.Add("templateRow");

            string templateIdPrefix = "template-perm-";
            Table.Column nameColumn = templateRow.CreateColumn();
            ComboBox nameDropDown = new ComboBox();
            foreach(CountryRole role in pageElement.Page.Country.CountryRoles)
            {
                nameDropDown.SelectItems.Add(new ComboBox.Item()
                {
                    Text = role.Name,
                    Value = role.CountryRoleID?.ToString()
                });
            }
            nameDropDown.Class.Add("form-control");
            nameDropDown.Id = templateIdPrefix + "countryroleid";
            nameColumn.Parts.Add(nameDropDown);
            nameColumn.Parts.Add(new Label()
            {
                Class = { "text-danger" },
                Data = { { "validate-message-for", "CountryRoleID" } }
            });
            nameColumn.Parts.Add(new Checkbox()
            {
                Id = templateIdPrefix + "canview",
                Text = "Can View?"
            });
            nameColumn.Parts.Add(new Checkbox()
            {
                Id = templateIdPrefix + "canedit",
                Text = "Can Edit?"
            });

            Table.Column deleteCol = templateRow.CreateColumn();
            deleteCol.Class.Add("col-sm-1");
            Button deleteTemplate = new Button()
            {
                Type = "button",
                Class = { "btn", "btn-danger" },
                ViewPartsBeforeText = new List<ViewPart>()
                {
                    new Span() { Class = { "fas", "fa-trash" } }
                },
                Text = " Delete",
                OnClick = "managePageElementPermissionsRemoveCountryRole($(this));"
            };
            deleteCol.Parts.Add(deleteTemplate);

            yield return table;

            Button add = new Button()
            {
                Type = "button",
                Class = { "btn", "btn-primary" },
                ViewPartsBeforeText = new List<ViewPart>()
                {
                    new Span() { Class = { "fas", "fa-plus-circle" } }
                },
                Text = " Add",
                OnClick = "managePageElementPermissionsAddCountryRole($(this));"
            };

            yield return add;
        }

        public ActionResult CountryRolePageElementSave(FormCollection permissions)
        {
            if (!Permission.CanUpdatePermissions)
            {
                return new HttpUnauthorizedResult("User does not have permission to update Page Element permissions");
            }

            if (!long.TryParse(permissions.Get("PageElementID"), out long pageElementID))
            {
                return HttpNotFound("Could not find Page Element");
            }

            HashSet<int> handledNewLines = new HashSet<int>();
            HashSet<int> handledExistingLines = new HashSet<int>();
            Context context = new Context();
            PageElement pageElement = context.PageElements.FirstOrDefault(pe => pe.PageElementID == pageElementID);
            pageElement.ForAuthenticatedOnly = bool.Parse(permissions["IsForAuthenticated"]);
            foreach(string key in permissions.AllKeys)
            {
                string[] parts = key.Split('-');
                int countryRolePageElementID;
                switch(parts[0])
                {
                    case "new":
                        if (!int.TryParse(parts[2], out countryRolePageElementID))
                        {
                            continue;
                        }

                        if (!handledNewLines.Add(countryRolePageElementID))
                        {
                            continue;
                        }

                        string countryRoleIDString = permissions[string.Format("new-perm-{0}-countryroleid", countryRolePageElementID)];
                        string canViewString = permissions[string.Format("new-perm-{0}-canview", countryRolePageElementID)];
                        string canEditString = permissions[string.Format("new-perm-{0}-canedit", countryRolePageElementID)];

                        if (!int.TryParse(countryRoleIDString, out int countryRoleID) ||
                            !bool.TryParse(canViewString, out bool canView) ||
                            !bool.TryParse(canEditString, out bool canEdit))
                        {
                            continue;
                        }

                        CountryRolePageElement newCountryRolePageElement = new CountryRolePageElement();
                        newCountryRolePageElement.CountryRoleID = countryRoleID;
                        newCountryRolePageElement.PageElementID = pageElementID;
                        newCountryRolePageElement.CanView = canView;
                        newCountryRolePageElement.CanEdit = canEdit;

                        context.CountryRolePageElements.Add(newCountryRolePageElement);
                        break;
                    case "removed":
                        if (!int.TryParse(parts[2], out countryRolePageElementID))
                        {
                            continue;
                        }

                        if (!handledExistingLines.Add(countryRolePageElementID))
                        {
                            continue;
                        }

                        CountryRolePageElement countryRolePageElementToDelete = context.CountryRolePageElements.FirstOrDefault(crpe => crpe.CountryRolePageElementID == countryRolePageElementID);
                        if (countryRolePageElementToDelete != null)
                        {
                            context.CountryRolePageElements.Remove(countryRolePageElementToDelete);
                        }

                        break;
                    case "perm":
                        if (!int.TryParse(parts[1], out countryRolePageElementID))
                        {
                            continue;
                        }

                        if (!handledExistingLines.Add(countryRolePageElementID))
                        {
                            continue;
                        }

                        CountryRolePageElement existingCountryRolePageElement = context.CountryRolePageElements.FirstOrDefault(crpe => crpe.CountryRolePageElementID == countryRolePageElementID);
                        if (existingCountryRolePageElement == null)
                        {
                            continue;
                        }

                        string updateCanViewString = permissions[string.Format("perm-{0}-canview", countryRolePageElementID)];
                        string updateCanEditString = permissions[string.Format("perm-{0}-canedit", countryRolePageElementID)];

                        if (!bool.TryParse(updateCanViewString, out bool updateCanView) ||
                            !bool.TryParse(updateCanEditString, out bool updateCanEdit))
                        {
                            continue;
                        }

                        existingCountryRolePageElement.CanView = updateCanView;
                        existingCountryRolePageElement.CanEdit = updateCanEdit;
                        break;
                }
            }

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

        #region Form
        //public ActionResult FormElementAdd(FormCollection collection)
        //{
        //    string formIDString = null;
        //    string selectionString = null;

        //    foreach(string key in collection.Keys)
        //    {
        //        if (!key.StartsWith("addmodal-"))
        //        {
        //            continue;
        //        }

        //        string substrKey = key.Substring(9);

        //        switch(substrKey)
        //        {
        //            case "formid":
        //                formIDString = collection[key];
        //                break;
        //            case "selection":
        //                selectionString = collection[key];
        //                break;
        //        }
        //    }

        //    if (!int.TryParse(formIDString, out int formID) || !int.TryParse(selectionString, out int selection))
        //    {
        //        return Json(new { success = false });
        //    }

        //    Context context = new Context();
        //    FormElement formElement = new FormElement()
        //    {
        //        FormID = formID,
        //        ElementType = (FormElement.ElementTypes)selection
        //    };
        //    context.FormElements.Add(formElement);
        //    context.SaveChanges();

        //    BaseFormPart baseFormPart = (BaseFormPart)Activator.CreateInstance(BaseFormPart.ElementTypesTypeLookup[(FormElement.ElementTypes)selection]);
        //    baseFormPart.FormElement = formElement;
        //    XElement newElement = new XElement("element");
        //    baseFormPart.WriteToXML(newElement);

        //    formElement.ElementXML = newElement.ToString();
        //    context.SaveChanges();

        //    return Json(new { success = true });
        //}

        //public ActionResult FormElementDelete(FormCollection formCollection)
        //{

        //}
        #endregion
    }
}