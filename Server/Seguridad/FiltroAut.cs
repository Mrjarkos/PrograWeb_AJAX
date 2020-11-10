using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Datos;
using Server.Models;

namespace Server.Seguridad
{
    public class FiltroAut : FilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            try
            {
                var Key = "SesionValida";
                var REQ = filterContext.HttpContext.Request.Cookies[Key];
                var ROUTE = filterContext.HttpContext.Request.RawUrl;
                if (REQ == null)
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
                else
                {
                    if (!(REQ["UserRol"].Equals("MONITOR") || REQ["UserRol"].Equals("ADMINISTRADOR")))
                    {
                        filterContext.Result = new HttpUnauthorizedResult();
                    }
                    else
                    {
                        if (REQ["UserRol"].Equals("MONITOR") && (ROUTE.Contains("User") || ROUTE.Contains("Sensor")))
                        {filterContext.Result = new HttpUnauthorizedResult();}
                    }
                }

            }
            catch
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }

        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                
                filterContext.Result = new RedirectResult("/Login/Error", false);
            }
        }
    }


    
}