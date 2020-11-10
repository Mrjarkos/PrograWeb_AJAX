using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Server.Controllers
{
    public class UserController : Controller
    {

        // GET: User
        [Seguridad.FiltroAut]
        public ActionResult Add()
        {
            return View();
        }
        [Seguridad.FiltroAut]
        public ActionResult Load()
        {
            return View();
        }
        [Seguridad.FiltroAut]
        public ActionResult Delete(string Id)
        {
            try
            {
                ViewBag.error = false;
                int id = int.Parse(Id);
                using (var context = new Datos.DatosEntities())
                {
                    USUARIOS user_ant = (from q in context.USUARIOS
                                        where q.ID == id
                                        select q).First();

                    context.USUARIOS.Remove(user_ant);
                    context.SaveChanges();
                }
                ViewBag.mensaje = "Usuario borrado exitosamente";
            }
            catch (Exception ex)
            {
                ViewBag.error = true;
                ViewBag.mensaje = "Error:" + ex.Message.ToString();
            }
            var r = View();
            return r;
        }

        [HttpPost]
        [Seguridad.FiltroAut]
        public ActionResult Modify(string Id)
        {
            int id = int.Parse(Id);
            Models.Usuario usuario = new Models.Usuario();
            usuario.ID = id;
            try
            {
                using (var context = new Datos.DatosEntities())
                {

                    Datos.USUARIOS user_ant = (from q in context.USUARIOS
                                              where q.ID == id
                                              select q).First();
                    usuario.CORREO = user_ant.CORREO;
                    usuario.PASSWORD = user_ant.PASSWORD;
                    usuario.DOCUMENTO = user_ant.DOCUMENTO;
                    usuario.DOC_TYPE = user_ant.DOC_TYPE;
                    usuario.NOMBRE = user_ant.NOMBRE;
                    usuario.ID = user_ant.ID;
                    usuario.ROL = user_ant.ROL;
                    usuario.DATECREATED = user_ant.DATECREATED;
                    usuario.DATELASTMODIFICATION = user_ant.DATELASTMODIFICATION;
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = true;
                ViewBag.mensaje = "Error:" + ex.Message.ToString();
            }
            return View(usuario);
        }

        [HttpPost]
        [Seguridad.FiltroAut]
        public ActionResult Modificar(Models.Usuario usuario)
        {
            int id = usuario.ID;
            try
            {
                ViewBag.error = false;

                if (ModelState.IsValid)
                {
                    using (var context = new Datos.DatosEntities())
                    {
                        var u = context.USUARIOS.SingleOrDefault(b => b.ID ==id);
                        if (u != null)
                        {
                            u.NOMBRE = usuario.NOMBRE;
                            u.PASSWORD = usuario.PASSWORD;
                            u.ROL = usuario.ROL;
                            u.DOC_TYPE = usuario.DOC_TYPE;
                            u.CORREO = usuario.CORREO;
                            u.DOCUMENTO = usuario.DOCUMENTO;
                            u.DATELASTMODIFICATION = DateTime.Now;
                        }
                        else {
                            throw new Exception("No se pudo almacenar en la base de datos, error del programador");
                        }
                        context.SaveChanges();
                    }

                    ViewBag.mensaje = "Usuario editado exitosamente";
                }
                else {
                    throw new Exception("El modelo no es válido, por favor verifique que se hayan llenado todos los campos");
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = true;
                ViewBag.mensaje = "Error:" + ex.Message.ToString();
            }
            return View("Modify");
        }

        // POST: User
        [HttpPost]
        [Seguridad.FiltroAut]
        public ActionResult Guardar(Models.Usuario usuario)
        {
            try
            {
                DateTime date = DateTime.Now;
                ViewBag.error = false;
                if (ModelState.IsValid)
                {
                    using (var context = new DatosEntities())
                    {
                        var user_ant = (from q in context.USUARIOS
                                    where q.CORREO == usuario.CORREO
                                        select q).FirstOrDefault();
                        if (user_ant != null) {
                            throw new Exception("No se pudo agregar usuario a la base de datos, Correo ya existe");
                        }
                        var admins = (from q in context.USUARIOS
                                        where q.ROL == "ADMINISTRADOR"
                                      select q).ToList();
                        if (admins.Count >= 5 && usuario.ROL== "ADMINISTRADOR") {
                            throw new Exception("No se pudo agregar usuario a la base de datos, Demasiados Administradores");
                        }
                        if (admins.Count >= 50 && usuario.ROL == "MONITOR")
                        {
                            throw new Exception("No se pudo agregar usuario a la base de datos,  Demasiados Monitores");
                        }
                        context.USUARIOS.Add(new USUARIOS
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
            return View("Add");
        }

        [HttpGet]
        [Seguridad.FiltroAut]
        public JsonResult GetRegister(string id_search, string id_user_search, string document_search, 
                                      string number_search, string email_search, string rol_search,
                                      string Creacion_search, string Modificacion_search, int Page, int N_items)
        {
            bool Contain = false;

            var modelo = new Models.RegisterView();
            modelo.Registros = new List<Models.Register>();
            var Responce = new Models.JsonTable();

            ViewBag.error = false;
            try
            {
                using (var context = new Datos.DatosEntities())
                {
                    var us = context.USUARIOS.ToArray();
                    foreach (var u in us)
                    {
                        Contain = u.ID.ToString().Contains(id_search) &&
                                  u.NOMBRE.ToString().Contains(id_user_search) &&
                                  u.DOC_TYPE.Contains(document_search) &&
                                  u.DOCUMENTO.Contains(number_search) &&
                                  u.CORREO.Contains(email_search) &&
                                  u.ROL.Contains(rol_search) &&
                                  u.DATECREATED.ToString().Contains(Creacion_search) &&
                                  u.DATELASTMODIFICATION.ToString().Contains(Modificacion_search);
                        if (Contain)
                        {
                            modelo.Registros.Add(new Models.Usuario
                            {
                                ID = u.ID,
                                NOMBRE = u.NOMBRE,
                                DOC_TYPE = u.DOC_TYPE,
                                DOCUMENTO = u.DOCUMENTO,
                                CORREO = u.CORREO,
                                ROL = u.ROL,
                                DATECREATED = u.DATECREATED,
                                DATELASTMODIFICATION = u.DATELASTMODIFICATION
                            });
                        }
                    }
                    //division

                    Responce = new Models.JsonTable()
                    {
                        TotalPaginas = (int)Math.Ceiling((double)modelo.Registros.Count() / N_items),
                        PaginaActual = Page,
                        Registros = modelo.Registros.Skip((Page - 1) * N_items).Take(N_items).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = true;
                ViewBag.mensaje = "Error:" + ex.Message.ToString();
            }
            return Json(Responce, JsonRequestBehavior.AllowGet);
        }

    }
}