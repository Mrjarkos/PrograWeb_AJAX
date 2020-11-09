using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimSensor
{
 
    class Program{
            static void Main(string[] args)
            {
                while (true)
                {
                    string GH = " ";
                    Console.WriteLine("Listo....");
                    Console.ReadKey();
                    GH = GETMessage();
                    //if (GH != " ") { PUTMessage(GH); }
                    //else { Console.WriteLine("Error de conexion intente de nuevo"); }

                    //Console.ReadKey();
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
                try
                {

                    //connect
                    var client = new System.Net.Sockets.TcpClient();
                    //Send
                    client.Connect("localhost", 3197);
                    string MessageS = "GET /Sensor/GetDateTime/99 HTTP/1.1\r\n" +
                    "Host: localhost:3197\r\n" +
                    "\r\n";


                    byte[] datOut = Encoding.UTF8.GetBytes(MessageS);
                    client.GetStream().Write(datOut, 0, datOut.Length);
                    //Espera
                    System.Threading.Thread.Sleep(1000);
                    //rec
                    if (client.Available > 0)
                    {
                        byte[] datIn = new byte[client.Available];
                        client.GetStream().Read(datIn, 0, datIn.Length);
                        string responce = Encoding.UTF8.GetString(datIn);
                        Console.WriteLine("Respuesta: \r\n" + responce);
                        var Mensaje = responce.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
                        var body = Mensaje[1];
                        var head = Mensaje[0];

                        //Analisis(responce);
                        ReadCookie(head);

                        DateTime dt = DateTime.ParseExact(body, "yyyy-MM-dd HH:mm:ss", null);
                        DateTime dt2 = dt.AddHours(2);
                        FYH = dt2.ToString("yyyy-MM-dd HH:mm:ss");

                        Console.WriteLine("FYH:" + FYH + "\r\n");
                    }
                    else
                    {
                        Console.WriteLine("No hay Respuesta GET: \r\n");
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
                try
                {

                    var ran = new Random();

                    float ValueSensor = (float)ran.NextDouble();


                    string Variable = "MEDICION=" + ValueSensor.ToString() + "&" + "FECHAYHORA=" + Date.ToString();


                    //connect
                    var client = new System.Net.Sockets.TcpClient();
                    //Send
                    client.Connect("williamap-001-site1.itempurl.com", 3197);
                    string MessageS = "PUT /Device/PutData/99 HTTP/1.1\r\n" +
                    "Host: williamap-001-site1.itempurl.com:80\r\n" +
                    "Content-Type: application/x-www-form-urlencoded\r\n" +
                    "Content-Length:" + Variable.Length.ToString() + "\r\n" +
                    "\r\n" + Variable;


                    byte[] datOut = Encoding.UTF8.GetBytes(MessageS);
                    client.GetStream().Write(datOut, 0, datOut.Length);
                    //Espera
                    System.Threading.Thread.Sleep(1000);
                    //rec
                    if (client.Available > 0)
                    {
                        byte[] datIn = new byte[client.Available];
                        client.GetStream().Read(datIn, 0, datIn.Length);
                        string responce = Encoding.UTF8.GetString(datIn);
                        Console.WriteLine("Respuesta: \r\n" + responce);
                        Analisis(responce);
                    }
                    else
                    {
                        Console.WriteLine("No hay Respuesta PUT: \r\n");
                    }


                    //Disconect
                    client.Close();
                    Console.WriteLine("Fin de la conexion \r\n");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            static string ReadCookie(string Head)
            {

                int index = Head.IndexOf("Set-Cookie:");
                int indexedn = Head.IndexOf("\n", index);
                string Dispositivo;
                return "NEL";
            }

    }
}

