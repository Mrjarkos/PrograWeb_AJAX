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
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string PostData(int id, float MEDICION)
        {
            try
            {
                DateTime FECHAYHORA = DateTime.Now;
                var respuesta = new { Resultado = "OK", MEDICION = MEDICION, FECHAYHORA = FECHAYHORA, Id = id };
                var Numero = respuesta.MEDICION;


                using (var context = new DatosEntities())
                {
                    context.SensorDevice.Add(new Datos.SensorDevice
                    {
                        ID_SENSOR = respuesta.Id,
                        MEDICION = Numero,
                        FECHAYHORA = respuesta.FECHAYHORA,
                    });
                    context.SaveChanges();
                }
                return respuesta.Id + " " + respuesta.MEDICION + " " + respuesta.FECHAYHORA + " " + respuesta.Resultado;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        [HttpGet]
        public string GetDateTime(int id)
        {
            try
            {

                var autorizatedID = new List<int> { 33, 22, 11, 99 };
                if (!autorizatedID.Exists(X => X == id)) throw new Exception("Device: ID Desconocido");
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                return "ERROR:" + ex.Message;
            }
        }

        public JsonResult GetRegister()
        {
            string id_search = Request.QueryString["id_search"];
            string id_sensor_search = Request.QueryString["id_sensor_search"];
            string medicion_search = Request.QueryString["medicion_search"];
            string Fecha_search = Request.QueryString["Fecha_search"];
            int Page = Convert.ToInt32( Request.QueryString["Page"]);
            bool Contain=false;

            var modelo = new Models.SensorView();
            modelo.sensores = new List<Models.Sensor>();
            var Responce = new Models.JsonTable();


            ViewBag.error = false;
            try
            {
                using (var context = new Datos.DatosEntities())
                {
                    var us = context.SensorDevice.ToArray();
                    foreach (var u in us)
                    {
                        Contain = u.ID_REG.ToString().Contains(id_search) &&
                                  u.ID_SENSOR.ToString().Contains(id_sensor_search) &&
                                  u.MEDICION.ToString().Contains(medicion_search) &&
                                  u.FECHAYHORA.ToString().Contains(Fecha_search);
                        if (Contain)
                        {
                            modelo.sensores.Add(new Models.Sensor
                            {
                                ID_REG = u.ID_REG,
                                ID_SENSOR = u.ID_SENSOR,
                                MEDICION = (float)u.MEDICION,
                                FECHAYHORA = u.FECHAYHORA
                            });
                        }

                       
                    }


                    //division

                    Responce = new Models.JsonTable() {
                        TotalPaginas = (int)Math.Ceiling((double)modelo.sensores.Count() / 10),
                        PaginaActual =Page,
                        Registros= modelo.sensores.Skip((Page - 1) * 10).Take(10).ToList()
                    };
                    

                    //Numero a enviar

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