using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Datos;

namespace Server.Controllers
{
    public class SensorController : Controller
    {
        [Seguridad.FiltroAut]
        public ActionResult Load()
        {
            return View();
        }
        [Seguridad.FiltroAut]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Seguridad.FiltroAut]
        public ActionResult Guardar(Models.Sensor sensor)
        {
            try
            {
                DateTime date = DateTime.Now;
                ViewBag.error = false;
                if (ModelState.IsValid)
                {
                    using (var context = new DatosEntities())
                    {
                        var us = context.SENSORES.ToArray();
                        foreach (var u in us)
                        {
                            if (u.SERIAL == sensor.SERIAL)
                            {
                                throw new Exception("No se pudo agregar NUEVO SENSOR a la base de datos, SERIAL ya existe");
                            }
                        }
                        context.SENSORES.Add(new SENSORES
                        {
                            SERIAL = sensor.SERIAL,
                            DATECREATED = date,
                            DATELASTMODIFICATION = date,
                            DESCRIPCION = sensor.DESCRIPCION,
                            TIPO = sensor.TIPO,
                            PASSWORD = sensor.PASSWORD
                        });
                        context.SaveChanges();
                    }
                    ViewBag.mensaje = "Sensor adicionado exitosamente";
                }
                else
                {
                    throw new Exception("El modelo no es válido, por favor verifique que se hayan llenado todos los campos obligatorios");
                }

            }
            catch (Exception ex)
            {
                ViewBag.error = true;
                ViewBag.mensaje = "Error:" + ex.Message.ToString();
            }
            return View("Add");
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
                    SENSORES sensor_ant = (from q in context.SENSORES
                                           where q.ID == id
                                           select q).First();

                    context.SENSORES.Remove(sensor_ant);
                    context.SaveChanges();
                }
                ViewBag.mensaje = "Sensor borrado exitosamente";
            }
            catch (Exception ex)
            {
                ViewBag.error = true;
                ViewBag.mensaje = "Error:" + ex.Message.ToString();
            }
            
            return View();
        }

        [HttpPost]
        [Seguridad.FiltroAut]
        public ActionResult Modify(string Id) //Cargar View
        {
            Models.Sensor sensor = new Models.Sensor();
            try
            {
                int id = int.Parse(Id);
                sensor.ID = id;
                using (var context = new Datos.DatosEntities())
                {

                    Datos.SENSORES sensor_ant = (from q in context.SENSORES
                                                 where q.ID == id
                                                 select q).First();
                    sensor.DATECREATED = sensor_ant.DATECREATED;
                    sensor.DATELASTMODIFICATION = sensor_ant.DATELASTMODIFICATION;
                    sensor.DESCRIPCION = sensor_ant.DESCRIPCION;
                    sensor.ID = sensor_ant.ID;
                    sensor.PASSWORD = sensor_ant.PASSWORD;
                    sensor.SERIAL = sensor_ant.SERIAL;
                    sensor.TIPO = sensor_ant.TIPO;
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = true;
                ViewBag.mensaje = "Error:" + ex.Message.ToString();
            }
            return View(sensor);
        }

        [HttpPost]
        [Seguridad.FiltroAut]
        public ActionResult Modificar(Models.Sensor sensor)
        {
            int id = sensor.ID;
            try
            {
                ViewBag.error = false;

                if (ModelState.IsValid)
                {
                    using (var context = new Datos.DatosEntities())
                    {
                        var u = context.SENSORES.SingleOrDefault(b => b.ID == id);
                        if (u != null)
                        {
                            u.ID = sensor.ID;
                            u.PASSWORD = sensor.PASSWORD;
                            u.SERIAL = sensor.SERIAL;
                            u.TIPO = sensor.TIPO;
                            u.DESCRIPCION = sensor.DESCRIPCION;
                            u.DATELASTMODIFICATION = DateTime.Now;
                        }
                        else
                        {
                            throw new Exception("No se pudo almacenar en la base de datos, no se encontró ID (Probablemente error del programador)");
                        }
                        context.SaveChanges();
                    }

                    ViewBag.mensaje = "Sensor editado exitosamente";
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
            return View("Modify");
        }

        [HttpGet]
        [Seguridad.FiltroAut]
        public JsonResult GetRegister(string id_search, string id_sensor_search,
                                      string tipo_search, string Creacion_search,
                                      string Modificacion_search, int Page, int N_items)
        {
            bool Contain=false;

            var modelo = new Models.RegisterView();
            modelo.Registros = new List<Models.Register>();
            var Responce = new Models.JsonTable();

            ViewBag.error = false;
            try
            {
                using (var context = new Datos.DatosEntities())
                {
                    var us = context.SENSORES.ToArray();
                    foreach (var u in us)
                    {
                        var creacion = u.DATECREATED;
                        var modificacion = u.DATELASTMODIFICATION;
                        Contain = u.ID.ToString().Contains(id_search) &&
                                  u.SERIAL.ToString().Contains(id_sensor_search) &&
                                  u.TIPO.ToString().Contains(tipo_search) &&
                                  creacion.ToString("dd-MM-yy HH:mm:ss").Contains(Creacion_search) &&
                                  modificacion.ToString("dd-MM-yy HH:mm:ss").Contains(Modificacion_search);
                        if (Contain)
                        {
                            modelo.Registros.Add(new Models.Sensor
                            {
                                ID = u.ID,
                                SERIAL = u.SERIAL,
                                TIPO = u.TIPO,
                                DESCRIPCION = u.DESCRIPCION,
                                DATECREATED = u.DATECREATED,
                                DATELASTMODIFICATION = u.DATELASTMODIFICATION
                            });
                        }
                    }

                    //division
                    Responce = new Models.JsonTable() {
                        TotalPaginas = (int)Math.Ceiling((double)modelo.Registros.Count() / N_items),
                        PaginaActual =Page,
                        Registros= modelo.Registros.Skip((Page - 1) * N_items).Take(N_items).ToList()
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