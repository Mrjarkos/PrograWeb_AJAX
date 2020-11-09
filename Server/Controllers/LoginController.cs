using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Server.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Cuenta_nueva()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Guardar(Models.Usuario usuario)
        {
                try
                {
                    ViewBag.error = false;
                    DateTime date = DateTime.Now;
                    if (ModelState.IsValid)
                    {
                        using (var context = new Datos.DatosEntities())
                        {
                            var us = context.USUARIOS.ToArray();
                            foreach (var u in us)
                            {
                                if (u.CORREO == usuario.CORREO)
                                {
                                    throw new Exception("No se pudo agregar usuario a la base de datos, Correo ya existe");
                                }
                            }
                            context.USUARIOS.Add(new Datos.USUARIOS
                            {
                                NOMBRE = usuario.NOMBRE,
                                PASSWORD = usuario.PASSWORD,
                                CORREO = usuario.CORREO,
                                DOCUMENTO = usuario.DOCUMENTO,
                                DOC_TYPE = usuario.DOC_TYPE,
                                ROL = usuario.ROL,
                                DATECREATED = date,
                                DATELASTMODIFICATION = date
                            });
                            context.SaveChanges();
                        }
                        ViewBag.mensaje = "Usuario adicionado exitosamente";
                        var user = new Models.Usuario {
                                                        CORREO=usuario.CORREO,
                                                        NOMBRE = usuario.NOMBRE,
                                                        PASSWORD = usuario.PASSWORD,
                                                        ID = usuario.ID,
                                                        DATECREATED = usuario.DATECREATED,
                                                        DATELASTMODIFICATION = usuario.DATELASTMODIFICATION,
                                                        DOCUMENTO = usuario.DOCUMENTO,
                                                        DOC_TYPE = usuario.DOC_TYPE,
                                                        ROL = usuario.ROL
                        };
                        ViewBag.User = user.NOMBRE;
                    }
                    else
                    {
                        throw new Exception("El modelo no es válido, por favor verifique que se hayan llenado todos los campos");
                    }

                }
                catch (Exception ex)
                {
                    ViewBag.error = true;
                    ViewBag.mensaje = "Error:" + ex.Message.ToString();
                }
            if (ViewBag.error)
            {
                return View("Cuenta_nueva");
            }
            else {
                return View("Login");
            }
                
        }

        [HttpPost]
        public ActionResult Ingresar(Models.Usuario user)
        {
            try
            {
                ViewBag.error = false;
                var modelo = new Models.RegisterView();

                using (var context = new Datos.DatosEntities())
                {

                    var us = context.USUARIOS.ToArray();
                    foreach (var u in us)
                    {
                        if (u.CORREO == user.CORREO && u.PASSWORD == user.PASSWORD)
                        {
                            user.NOMBRE = u.NOMBRE;

                            //Cookie
                            string infoCookie = "SesionValida";
                            var Expire = DateTime.Now.AddMinutes(10);
                            HttpCookie cookie = new HttpCookie(infoCookie);
                            cookie["UserName"] = u.NOMBRE.ToString();
                            cookie["UserRol"] = u.ROL.ToString();
                            cookie.Expires = Expire;
                            HttpContext.Response.Cookies.Add(cookie);

                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

                throw new Exception("El correo y la contraseña digitada no coinciden");
            }
            catch (Exception ex)
            {
                ViewBag.error = true;
                ViewBag.mensaje = "Error: " + ex.Message.ToString();
            }
            return View("Login");
        }
    }
}