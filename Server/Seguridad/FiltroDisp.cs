﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Datos;
using Server.Models;

namespace Server.Seguridad
{
    public class FiltroDisp : FilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            try
            {
                var Key = "DispositivoValido";
                var REQ = filterContext.HttpContext.Request.Cookies[Key];
                var ROUTE = filterContext.HttpContext.Request.RawUrl;
                if (REQ == null)
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }

            }
            catch
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }

        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {

        }
    }



}