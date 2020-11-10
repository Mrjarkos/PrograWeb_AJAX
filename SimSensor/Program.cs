using System;
using System.Linq;
using System.Text;

namespace SimSensor
{

    class Program
    {

        public static string CookieDisp { get; set; }
        public static bool Access { get; set; }
        public static string Message { get; set; }
        public static DateTime Comp { get; set; }
        public static DateTime Comp1 { get; set; }
        public static string HOST { get; set; }
        public static int PORT { get; set; }
        public static string Serial { get; set; }
        public static string PASSWORD { get; set; }

        static void Main(string[] args)
        {

            HOST = "mrjarkos-001-site1.itempurl.com";
            PORT = 80;
            Serial = "NS185";
            PASSWORD = "1234";

            CookieDisp = " ";
            Message = "OK";



            Console.ReadKey();
            while (true)
            {

                while (CookieDisp.Equals(" "))
                {
                    GETLogin();
                }

                //ReadCookie(CookieDisp);
                System.Threading.Thread.Sleep(1000);
                string GH = " ";
                Console.WriteLine("Listo....");
                GH = GETMessage();
                while (Message.Equals("unauthorized"))
                {
                    GH = GETMessage();
                }

                System.Threading.Thread.Sleep(1000);
                if (!GH.Equals(" "))
                {
                    PUTMessage(GH);
                }
                while (Message.Equals("unauthorized"))
                {
                    PUTMessage(GH);
                }

                System.Threading.Thread.Sleep(1000);

                if (Comp1 > Comp)
                {
                    CookieDisp = " ";
                }
            }

        }

        static void Analisis(string Respuesta)
        {
            var Mensaje = Respuesta.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
            var header = Mensaje[0];

            var FieldHeader = header.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var InformeR = FieldHeader[0].Split(new char[] { ' ' });

            if (InformeR[1] == "200") { Message = "OK"; }
            else if (InformeR[1] == "401") { Message = "unauthorized"; }
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
                client.Connect(HOST, PORT);
                string MessageS = "GET /Reporte/GetDateTime/" + " HTTP/1.1\r\n" +
                "Host:" + HOST + ":" + PORT.ToString() + "\r\n" +
                "Cookie:" + CookieDisp + "\r\n" + "\r\n";

                byte[] datOut = Encoding.UTF8.GetBytes(MessageS);
                client.GetStream().Write(datOut, 0, datOut.Length);
                //Espera
                System.Threading.Thread.Sleep(1000);
                //rec
                if (client.Available > 0)
                {
                    Console.WriteLine("GET:===============================");
                    byte[] datIn = new byte[client.Available];
                    client.GetStream().Read(datIn, 0, datIn.Length);
                    string responce = Encoding.UTF8.GetString(datIn);
                    Console.WriteLine("Respuesta: \r\n" + responce);
                    var Mensaje = responce.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
                    var body = Mensaje[1];
                    var head = Mensaje[0];

                    Analisis(responce);
                    if (!Message.Equals("unauthorized"))
                    {

                        DateTime dt = DateTime.Parse(body);
                        DateTime dt2 = dt.AddHours(0);
                        Comp1 = dt;
                        FYH = dt2.ToString();
                    }
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

        static bool PUTMessage(string Date)
        {

            try
            {

                var ran = new Random();

                var ValueSensor = (float)(Math.Round(ran.NextDouble() * 70 + 10, 6));
                var Latitud = (float)(Math.Round(ran.NextDouble() * 0.28009 + 4.48676, 6));
                var Longitud = (float)(Math.Round(ran.NextDouble() * 0.19535 - 74.22157, 6));



                string Variable = "SERIALSENSOR=" + Serial + "&" + "MEDICION=" + ValueSensor.ToString() +
                                  "&" + "LATITUD=" + Latitud.ToString() + "&" + "LONGITUD=" + Longitud.ToString() +
                                  "&" + "DATEREPORTED=" + Date;

                //connect
                var client = new System.Net.Sockets.TcpClient();
                //Send
                client.Connect(HOST, PORT);
                string MessageS = "POST /Reporte/PostData HTTP/1.1\r\n" +
                "Host:" + HOST + ":" + PORT.ToString() + "\r\n" +
                "Cookie:" + CookieDisp + "\r\n" +
                "Content-Type: application/x-www-form-urlencoded\r\n" +
                "Content-Length:" + Variable.Length.ToString() + "\r\n" +
                "\r\n" + Variable + "\r\n";


                byte[] datOut = Encoding.UTF8.GetBytes(MessageS);
                client.GetStream().Write(datOut, 0, datOut.Length);
                //Espera
                System.Threading.Thread.Sleep(2000);
                //rec
                if (client.Available > 0)
                {
                    Console.WriteLine("PUT:===============================");
                    byte[] datIn = new byte[client.Available];
                    client.GetStream().Read(datIn, 0, datIn.Length);
                    string responce = Encoding.UTF8.GetString(datIn);
                    Analisis(responce);
                    var Mensaje = responce.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
                    var body = Mensaje[1];
                    var head = Mensaje[0];


                    Console.WriteLine("Respuesta: \r\n" + responce);
                }
                else
                {
                    Console.WriteLine("Fallo Comunicación-Intente nuevamente: PUT\r\n");
                }


                //Disconect
                client.Close();
                Console.WriteLine("Fin de la conexion... \r\n");
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        static void ReadEXPCookie(string Head)
        {

            int index = Head.IndexOf("Set-Cookie:");
            int indexedn = Head.IndexOf("\n", index) - index;
            string CookieSend = Head.Substring(index + 12, indexedn);
            var Cookie = CookieSend.Split(new string[] { ";" }, StringSplitOptions.None);
            var Exp = Cookie[1].Split(new string[] { "=" }, StringSplitOptions.None);
            var Fecha = Exp[1].Split(new string[] { "," }, StringSplitOptions.None);
            var FechaF = Fecha[1].Split(new string[] { " " }, StringSplitOptions.None);
            Comp = DateTime.ParseExact(FechaF[2], "hh:mm:ss", null).AddHours(-3).AddDays(1);
        }

        static void GETLogin()
        {
            try
            {
                var parameters = "?SERIALSENSOR=" + Serial + "&" + "PASSWORD=" + PASSWORD;
                //connect
                var client = new System.Net.Sockets.TcpClient();
                //Send
                client.Connect(HOST, PORT);
                string MessageS = "GET /Reporte/GetLogin/" + parameters + " HTTP/1.1\r\n" +
                "Host:" + HOST + ":" + PORT.ToString() + "\r\n" + "\r\n";

                byte[] datOut = Encoding.UTF8.GetBytes(MessageS);
                client.GetStream().Write(datOut, 0, datOut.Length);
                //Espera
                System.Threading.Thread.Sleep(2000);
                //rec
                if (client.Available > 0)
                {
                    Console.WriteLine("GET-Login:===============================");
                    byte[] datIn = new byte[client.Available];
                    client.GetStream().Read(datIn, 0, datIn.Length);
                    string responce = Encoding.UTF8.GetString(datIn);
                    Console.WriteLine("Respuesta: \r\n" + responce);
                    var Mensaje = responce.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
                    var body = Mensaje[1];
                    var head = Mensaje[0];

                    Analisis(responce);

                    //Cookie

                    int index = head.IndexOf("Set-Cookie:");
                    int indexedn = head.IndexOf("\r\n", index) - index - 12;
                    CookieDisp = head.Substring(index + 12, indexedn);
                    ReadEXPCookie(head);

                }
                else
                {
                    Console.WriteLine("Fallo Comunicación-Intente nuevamente:GET-Login\r\n");
                }


                //Disconect
                client.Close();


            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }

    }
}

