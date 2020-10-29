using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ejemplo1.Controllers
{
    public class UserController : Controller
    {
        
        // GET: User
        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Load()
        {
            var modelo = new Models.UsuarioView();
            modelo.usuarios = new List<Models.Usuario>();
            try
            {
                using (var context = new Datos.DatosEntities())
                {
                    var us = context.USUARIOS.ToArray();
                    foreach (var u in us)
                    {
                        modelo.usuarios.Add(new Models.Usuario
                        {
                            NOMBRE = u.NOMBRE,
                            CORREO = u.CORREO,
                            DOCUMENTO = u.DOCUMENTO,
                            DOC_TYPE = u.DOC_TYPE,
                            ID = u.ID,
                            ROL = u.ROL,
                            PASSWORD = u.PASSWORD
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = true;
                ViewBag.mensaje = "Error:" + ex.Message.ToString();
            }
            var r = View(modelo);
            return r;
        }

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
        public ActionResult Modify(string Id)
        {
            int id = int.Parse(Id);
            Ejemplo1.Models.Usuario usuario = new Models.Usuario();
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
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = true;
                ViewBag.mensaje = "Error:" + ex.Message.ToString();
            }
            var r = View(usuario);
            return r;
        }

        [HttpPost]
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
        public ActionResult Guardar(Models.Usuario usuario)
        {
            try
            {
                ViewBag.error = false;
                if (ModelState.IsValid)
                {
                    using (var context = new DatosEntities())
                    {
                        var us = context.USUARIOS.ToArray();
                        foreach (var u in us)
                        {
                            if (u.CORREO==usuario.CORREO)
                            {
                                throw new Exception("No se pudo agregar usuario a la base de datos, Correo ya existe");
                            }
                        }
                        context.USUARIOS.Add(new USUARIOS
                        {
                            NOMBRE = usuario.NOMBRE,
                            PASSWORD = usuario.PASSWORD,
                            CORREO = usuario.CORREO,
                            DOCUMENTO = usuario.DOCUMENTO,
                            DOC_TYPE = usuario.DOC_TYPE,
                            ROL = usuario.ROL,
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
    }
}