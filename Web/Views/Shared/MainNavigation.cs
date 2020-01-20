using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cydon.Data.Base;
using Cydon.Data.CySys;
using Cydon.Data.World;
using Web.Base;
using Web.ViewEngine;
using Web.ViewEngine.ViewParts;
using Web.ViewEngine.ViewParts.Base;

namespace Web.Views.Shared
{
    public class MainNavigation : CydonPartialViewSystem
    {
        protected override void FillPartsList(ViewContext context)
        {
            Nav nav = new Nav();
            nav.Parts.Add(new Anchor()
            {
                Class = { "navbar-brand" },
                Href = "#",
                TextAfterContent = "Cydon"
            });
            nav.Parts.Add(new Button()
            {
                Class = { "navbar-toggler" },
                Type = "button",
                Data =
                {
                    { "toggle", "collapse" },
                    { "target", "#navbarContent" }
                },
                ViewPartsBeforeText =
                {
                    new Span()
                    {
                        Class = { "navbar-toggler-icon" }
                    }
                }
            });

            Div navbarContent = new Div()
            {
                Class = { "collapse", "navbar-collapse" },
                Id = "navbarContent"
            };

            List navbarList = new List()
            {
                ListType = List.ListTypes.Unordered,
                Class = { "navbar-nav" }
            };

            // Load navigation stuff here
            // Normal content
            Context dataContext = new Context();
            foreach(Navigation navigation in dataContext.Navigations.Include("ChildNavigations.Page.Country").Where(n => n.ParentNavigationID == null))
            {
                List.ListItem outterNavigationItem = new List.ListItem()
                {
                    Class = { "nav-item", "dropdown" }
                };

                Anchor button = new Anchor()
                {
                    Class = { "nav-link", "dropdown-toggle" },
                    Href = "#",
                    Id = "nav" + navigation.NavigationID,
                    InnerAttributes =
                    {
                        { "role", "button" }
                    },
                    Data =
                    {
                        { "toggle", "dropdown" }
                    },
                    TextAfterContent = navigation.Text
                };

                outterNavigationItem.Parts.Add(button);

                Div dropdownItems = new Div()
                {
                    Class = { "dropdown-menu" }
                };

                foreach(Navigation childNavigation in navigation.ChildNavigations)
                {
                    Anchor childNavLink = new Anchor()
                    {
                        Class = { "dropdown-item" },
                        Href = VirtualPathUtility.ToAbsolute("~/Country/" + childNavigation.Page.Country.Name + "/" + childNavigation.Page.Name),
                        TextAfterContent = childNavigation.Text
                    };

                    dropdownItems.Parts.Add(childNavLink);
                }

                outterNavigationItem.Parts.Add(dropdownItems);
                navbarList.ListItems.Add(outterNavigationItem);
            }

            // Right align content
            Span rightAlignContent = new Span()
            {
                Class = { "ml-auto" }
            };

            if (context.Controller is BaseController baseController)
            {
                List<ViewPart> viewPartsForRightAlign = baseController.GetRightAlignViewParts();
                if (viewPartsForRightAlign.Any())
                {
                    if (viewPartsForRightAlign.Count == 1)
                    {
                        Div rightAlignSingleItemDiv = new Div()
                        {
                            Class = { "nav-item" },
                            Parts =
                            {
                                viewPartsForRightAlign.Single()
                            }
                        };

                        rightAlignSingleItemDiv.Class.Add("nav-link");
                        rightAlignContent.Parts.Add(rightAlignSingleItemDiv);
                    }
                    else
                    {
                        Div rightAlignDropdown = new Div()
                        {
                            Class = { "nav-item", "dropdown" }
                        };
                        rightAlignDropdown.Parts.Add(new Anchor()
                        {
                            Class = { "nav-link", "dropdown-toggle" },
                            Href = "#",
                            Id = "rightAlignDropdown",
                            InnerAttributes = { { "role", "button" } },
                            Data = { { "toggle", "dropdown" } },
                            TextAfterContent = "Options"
                        });
                        Div dropDownContent = new Div()
                        {
                            Class = { "dropdown-menu", "dropdown-menu-right" }
                        };

                        foreach(ViewPart part in viewPartsForRightAlign)
                        {
                            if (!part.Class.Contains("dropdown-divider"))
                            {
                                part.Class.Add("dropdown-item");
                            }
                            dropDownContent.Parts.Add(part);
                        }

                        rightAlignDropdown.Parts.Add(dropDownContent);

                        rightAlignContent.Parts.Add(rightAlignDropdown);
                    }
                }
            }


            navbarContent.Parts.Add(navbarList);
            navbarContent.Parts.Add(rightAlignContent);
            nav.Parts.Add(navbarContent);

            Parts.Add(nav);
        }
    }
}