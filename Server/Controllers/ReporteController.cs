using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Datos;
using Server.Models;
using System.Data.Entity.Spatial;

namespace Server.Controllers
{
    public class ReporteController : Controller
    {
        // GET: Registro
        public ActionResult Load()
        {
            return View();
        }
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
                        bool serial_exist = false;
                        var us = context.SENSORES.ToArray();
                        foreach (var u in us)
                        {
                            if (u.SERIAL == reporte.SERIALSENSOR)
                            {
                                serial_exist = true;
                                break;
                            }
                        }
                        if (serial_exist)
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

        //[httppost]
        //public string postdata(string serial, float medicion)
        //{
        //    try
        //    {
        //        datetime fechayhora = datetime.now;
        //        var respuesta = new { resultado = "ok", medicion = medicion, fechayhora = fechayhora, id = id };
        //        var numero = respuesta.medicion;


        //        using (var context = new datosentities())
        //        {
        //            context.sensores.add(new datos.sensores
        //            {
        //                id = respuesta.id,
        //                serial = 
        //                medicion = numero,
        //                fechayhora = respuesta.fechayhora,
        //            });
        //            context.savechanges();
        //        }
        //        return respuesta.id + " " + respuesta.medicion + " " + respuesta.fechayhora + " " + respuesta.resultado;
        //    }
        //    catch (exception ex)
        //    {
        //        return "error: " + ex.message;
        //    }
        //}

        //[httpget]
        //public string getdatetime(int id)
        //{
        //    try
        //    {

        //        var autorizatedid = new list<int> { 33, 22, 11, 99 };
        //        if (!autorizatedid.exists(x => x == id)) throw new exception("device: id desconocido");
        //        return datetime.now.tostring("yyyy-mm-dd hh:mm:ss");
        //    }
        //    catch (exception ex)
        //    {
        //        return "error:" + ex.message;
        //    }
        //}

        [HttpGet]
        public JsonResult GetRegister(string id_search, string id_sensor_search,
                                      string medicion_search, string latitud_search,
                                      string longitud_search,
                                      string Reported_search, string Modificacion_search,
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
                        var creacion = u.DATECREATED;
                        var reported = u.DATEREPORTED;
                        var modificacion = u.DATELASTMODIFICATION;
                        Contain = u.ID.ToString().Contains(id_search) &&
                                  u.SERIALSENSOR.ToString().Contains(id_sensor_search) &&
                                  u.MEDICION.ToString().Contains(medicion_search) &&
                                  u.LONGITUD.ToString().Contains(longitud_search) &&
                                  u.LATITUD.ToString().Contains(latitud_search) &&
                                  reported.ToString("dd-MM-yy HH:mm:ss").Contains(Reported_search) &&
                                  modificacion.ToString("dd-MM-yy HH:mm:ss").Contains(Modificacion_search);
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