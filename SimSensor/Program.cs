using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimSensor
{
 
    class Program{
        private static string CookieDisp;

        static void Main(string[] args)
            {
                while (true)
                {
                    CookieDisp = " ";
                    System.Threading.Thread.Sleep(1000);
                    string GH = " ";
                    Console.WriteLine("Listo....");
                    Console.ReadKey();
                    GH = GETMessage();
                    Console.WriteLine(CookieDisp);
                    System.Threading.Thread.Sleep(1000);
                    if (!GH.Equals(" "))
                    {
                        PUTMessage(GH);
                    }
                }

            }

            static void Analisis(string Respuesta)
            {
                var Mensaje = Respuesta.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
                var header = Mensaje[0];
                var body = Mensaje[1];

                var FieldHeader = header.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                var InformeR = FieldHeader[0].Split(new char[] { ' ' });

                if (InformeR[1] == "200") { Console.WriteLine("OK"); }
                else { Console.WriteLine("NOK"); }
            }


            static string GETMessage()
            {
            string FYH = " ";
            string HOST = "localhost";
            int PORT = 3197;
            string Serial = "NS002";
            string PASSWORD = "1234";

            try
                {
                    var parameters = "?SERIALSENSOR=" + Serial+"&"+ "PASSWORD=" + PASSWORD;
                    //connect
                    var client = new System.Net.Sockets.TcpClient();
                    //Send
                    client.Connect(HOST, PORT);
                    string MessageS = "GET /Reporte/GetDateTime/"+parameters+" HTTP/1.1\r\n" +
                    "Host:"+ HOST+":"+PORT.ToString()+"\r\n" +
                    "\r\n";


                    byte[] datOut = Encoding.UTF8.GetBytes(MessageS);
                    client.GetStream().Write(datOut, 0, datOut.Length);
                    //Espera
                    System.Threading.Thread.Sleep(1000);
                    //rec
                    if (client.Available > 0)
                    {
                    Console.WriteLine("GET:");
                        byte[] datIn = new byte[client.Available];
                        client.GetStream().Read(datIn, 0, datIn.Length);
                        string responce = Encoding.UTF8.GetString(datIn);
                        Console.WriteLine("Respuesta: \r\n" + responce);
                        var Mensaje = responce.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
                        var body = Mensaje[1];
                        var head = Mensaje[0];

                        //Cookie
                        int index = head.IndexOf("Set-Cookie:");
                        int indexedn = head.IndexOf("\r\n", index) - index-12;
                        CookieDisp = head.Substring(index + 12, indexedn);

                        DateTime dt = DateTime.Parse(body);
                        DateTime dt2 = dt.AddHours(0);
                        FYH = dt2.ToString();
                    }
                    else
                    {
                        Console.WriteLine("Fallo Comunicación-Intente nuevamente:GET\r\n");
                    }


                    //Disconect
                    client.Close();
                    return FYH;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                return FYH;
            }

            static void PUTMessage(string Date)
            {
            string HOST = "localhost";
            int PORT = 3197;
            string Serial = "NS002";
            try
                {

                var ran = new Random();

                var ValueSensor = (float)(Math.Round(ran.NextDouble() * 70 + 10, 6));
                var Latitud = (float)(Math.Round(ran.NextDouble() * 0.28009 + 4.48676, 6));
                var Longitud = (float)(Math.Round(ran.NextDouble() * 0.19535 - 74.22157, 6));



                string Variable = "SERIALSENSOR=" + Serial + "&" + "MEDICION=" + ValueSensor.ToString() + 
                                  "&" + "LATITUD=" + Latitud.ToString()+ "&" + "LONGITUD=" + Longitud.ToString()+
                                  "&" + "DATEREPORTED=" + Date;

                    //connect
                    var client = new System.Net.Sockets.TcpClient();
                    //Send
                    client.Connect(HOST, PORT);
                    string MessageS = "POST /Reporte/PostData HTTP/1.1\r\n" +
                    "Host:"+HOST+":"+PORT.ToString()+"\r\n" +
                    "Cookie:"+ CookieDisp + "\r\n" +
                    "Content-Type: application/x-www-form-urlencoded\r\n" +
                    "Content-Length:" + Variable.Length.ToString() + "\r\n" +
                    "\r\n" + Variable+ "\r\n";


                    byte[] datOut = Encoding.UTF8.GetBytes(MessageS);
                    client.GetStream().Write(datOut, 0, datOut.Length);
                    //Espera
                    System.Threading.Thread.Sleep(2000);
                    //rec
                    if (client.Available > 0)
                    {
                        byte[] datIn = new byte[client.Available];
                        client.GetStream().Read(datIn, 0, datIn.Length);
                        string responce = Encoding.UTF8.GetString(datIn);
                        Console.WriteLine("Respuesta: \r\n" + responce);
                    }
                    else
                    {
                        Console.WriteLine("Fallo Comunicación-Intente nuevamente: PUT\r\n");
                    }


                    //Disconect
                    client.Close();
                    Console.WriteLine("Fin de la conexion... \r\n");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            static string ReadCookie(string Head)
            {

            int index = Head.IndexOf("Set-Cookie:");
            int indexedn = Head.IndexOf("\n", index)-index;
            string CookieSend = Head.Substring(index + 12, indexedn);
            var Cookie = CookieSend.Split(new string[] {";"}, StringSplitOptions.None);
            var Parameters=Cookie[0].Split(new string[] { "=" }, StringSplitOptions.None);
            var Exp = Cookie[1].Split(new string[] {"="}, StringSplitOptions.None);
            var F_EXP= DateTime.ParseExact(Exp[1], "yyyy-mm-dd hh:mm:ss", null);
           
            return CookieSend;
            }

    }
}

