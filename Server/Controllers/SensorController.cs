﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Datos;

namespace Server.Controllers
{
    public class SensorController : Controller
    {
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
                        FECHAYHORA = respuesta.FECHAYHORA.ToString(),
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
        public ActionResult ShowRegister()
        {
            var modelo = new Models.SensorView();
            modelo.sensores = new List<Models.Sensor>();
            ViewBag.error = false;
            try
            {
                using (var context = new Datos.DatosEntities())
                {
                    var us = context.SensorDevice.ToArray();
                    foreach (var u in us)
                    {
                        modelo.sensores.Add(new Models.Sensor
                        {
                            ID_REG = u.ID_REG,
                            ID_SENSOR = u.ID_SENSOR,
                            MEDICION = (float)u.MEDICION,
                            FECHAYHORA = DateTime.Parse(u.FECHAYHORA)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = true;
                ViewBag.mensaje = "Error:" + ex.Message.ToString();
            }
            var r = View("index", modelo);
            return r;
        }
    }
}