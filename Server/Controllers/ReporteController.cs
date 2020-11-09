using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Datos;
using Server.Models;
using System.Data.Entity.Spatial;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server.Controllers
{
    public class ReporteController : Controller
    {
        // GET: Registro
        [Seguridad.FiltroAut]
        public ActionResult Load()
        {
            return View();
        }

        [Seguridad.FiltroAut]
        public ActionResult Graficar()
        {
            return View();
        }

        [Seguridad.FiltroAut]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Guardar(Models.Reporte reporte)
        {
            try
            {
                DateTime date = DateTime.Now;
                ViewBag.error = false;
                if (ModelState.IsValid)
                {
                    using (var context = new DatosEntities())
                    {
                        var u = context.SENSORES.SingleOrDefault(b => b.SERIAL == reporte.SERIALSENSOR);   
                        if (u==null)
                        {
                            context.REPORTES.Add(new REPORTES
                            {
                                SERIALSENSOR = reporte.SERIALSENSOR,
                                MEDICION = reporte.MEDICION,
                                LATITUD = reporte.LATITUD,
                                LONGITUD = reporte.LONGITUD,
                                DATECREATED = date,
                                DATELASTMODIFICATION = date,
                                DATEREPORTED = date
                            });
                            context.SaveChanges();
                        }
                        else {
                            throw new Exception("Serial de Dispositivo reportado NO existe");
                        }
                        
                    }
                    ViewBag.mensaje = "Reporte de Sensor adicionado exitosamente";
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
                    REPORTES reporte = (from q in context.REPORTES
                                       where q.ID == id
                                       select q).First();

                    context.REPORTES.Remove(reporte);
                    context.SaveChanges();
                }
                ViewBag.mensaje = "Reporte borrado exitosamente";
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
            Models.Reporte reporte = new Models.Reporte();
            try
            {
                int id = int.Parse(Id);
                reporte.ID = id;
                using (var context = new Datos.DatosEntities())
                {

                    Datos.REPORTES reporte_ant = (from q in context.REPORTES
                                                 where q.ID == id
                                                 select q).First();
                    reporte.SERIALSENSOR = reporte_ant.SERIALSENSOR;
                    reporte.MEDICION = reporte_ant.MEDICION;
                    reporte.ID = reporte_ant.ID;
                    reporte.LONGITUD = reporte_ant.LONGITUD;
                    reporte.LATITUD = reporte_ant.LATITUD;
                    reporte.DATEREPORTED = reporte_ant.DATEREPORTED;
                    reporte.DATECREATED = reporte_ant.DATECREATED;
                    reporte.DATELASTMODIFICATION = reporte_ant.DATELASTMODIFICATION;
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = true;
                ViewBag.mensaje = "Error:" + ex.Message.ToString();
            }
            return View(reporte);
        }

        [HttpPost]
        public ActionResult Modificar(Models.Reporte reporte)
        {
            int id = reporte.ID;
            try
            {
                ViewBag.error = false;

                if (ModelState.IsValid)
                {
                    using (var context = new Datos.DatosEntities())
                    {
                        var u = context.REPORTES.SingleOrDefault(b => b.ID == id);
                        if (u != null)
                        {
                            u.ID = reporte.ID;
                            u.LATITUD = reporte.LATITUD;
                            u.LONGITUD = reporte.LONGITUD;
                            u.MEDICION = reporte.MEDICION;
                            u.SERIALSENSOR = reporte.SERIALSENSOR;
                            u.DATELASTMODIFICATION = DateTime.Now;
                        }
                        else
                        {
                            throw new Exception("No se pudo almacenar en la base de datos, no se encontró ID (Probablemente error del programador)");
                        }
                        context.SaveChanges();
                    }

                    ViewBag.mensaje = "Registro editado exitosamente";
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

        [HttpPost]
        [Seguridad.FiltroDisp]
        public string PostData(Models.Reporte reporte)
        {
            try
            {
                DateTime fechayhora = DateTime.Now;
                DateTime dateTime = DateTime.Now;

                using (var context = new DatosEntities())
                {
                    context.REPORTES.Add(new Datos.REPORTES
                    {
                        SERIALSENSOR = reporte.SERIALSENSOR,
                        DATEREPORTED = reporte.DATEREPORTED,
                        LATITUD = reporte.LATITUD,
                        LONGITUD = reporte.LONGITUD,
                        MEDICION = reporte.MEDICION,
                        DATECREATED = dateTime,
                        DATELASTMODIFICATION = dateTime
                    });
                    context.SaveChanges();
                }
                return "OK" + JsonSerializer.Serialize(reporte); ;
            }
            catch (Exception ex)
            {
                return "Transacción Rechazada- " + ex.Message;
            }
        }

        [HttpGet]
        public string GetDateTime(string SERIALSENSOR, int PASSWORD)
        {
            try
            {
                using (var context = new DatosEntities())
                {
                    var us = context.SENSORES.ToArray();
                    foreach (var u in us)
                    {
                        if (u.SERIAL == SERIALSENSOR)
                        {

                            //Cookie
                            string infoCookie = "DispositivoValido";
                            var Expire = DateTime.Now.AddMinutes(10);
                            HttpCookie cookie = new HttpCookie(infoCookie);
                            cookie["ID"] = u.ID.ToString();
                            cookie["NSerial"] = u.SERIAL.ToString();
                            cookie.Expires = Expire;
                            HttpContext.Response.Cookies.Add(cookie);

                            return DateTime.Now.ToString();
                        }
                    }
                    throw new Exception("Serial de Dispositivo reportado desconocido");
                }
            }
            catch (Exception ex)
            {
                return "Transacción Rechazada-" + ex.Message;
            }
        }



        [HttpGet]
        public JsonResult GetRegister(string id_search, string id_sensor_search,
                                      string medicion_search, string latitud_search,
                                      string longitud_search,
                                      string Date_init_search, string Date_final_search,
                                      int Page, int N_items)
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
                    var us = context.REPORTES.ToArray();
                    foreach (var u in us)
                    {
                        var dateToCheck = u.DATEREPORTED;
                        var modificacion = u.DATELASTMODIFICATION;
                        Contain = u.ID.ToString().Contains(id_search) &&
                                  u.SERIALSENSOR.ToString().Contains(id_sensor_search) &&
                                  u.MEDICION.ToString().Contains(medicion_search) &&
                                  u.LONGITUD.ToString().Contains(longitud_search) &&
                                  u.LATITUD.ToString().Contains(latitud_search);
                        if (Contain)
                        {
                            modelo.Registros.Add(new Models.Reporte
                            {
                                ID = u.ID,
                                SERIALSENSOR = u.SERIALSENSOR,
                                LATITUD = u.LATITUD,
                                LONGITUD = u.LONGITUD,
                                MEDICION = u.MEDICION,
                                DATEREPORTED = u.DATEREPORTED,
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

        public static DbGeography CreatePoint(double lat, double lon, int srid = 4326)
        {
            string wkt = String.Format("POINT({0} {1})", lon, lat);

            return DbGeography.PointFromText(wkt, srid);
        }
    }
}