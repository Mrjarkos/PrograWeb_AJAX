#include <ESP8266WiFi.h>d

const char* ssid = "GABY_CATA";
const char* password = "ZAQzaqZAQ1234";
String host = "mrjarkos-001-site1.itempurl.com";
const int httpPort = 80;

const int id=random(0, 100);
float temp;

String GETMessage();

// the setup function runs once when you press reset or power the board
void setup() {
  Serial.begin(115200);
  WiFi.begin(ssid, password);
  Serial.printf("\n\n Conectando a la red: %s\n", WiFi.SSID().c_str());
  while(WiFi.status() != WL_CONNECTED){
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nConexión WiFi hecha");

  // Escribir la direccion IP
  Serial.print("Local IP: ");
  Serial.print(WiFi.localIP());
  Serial.println("");
  // initialize digital pin LED_BUILTIN as an output.
  pinMode(LED_BUILTIN, OUTPUT);
}

// the loop function runs over and over again forever
void loop() {
  digitalWrite(LED_BUILTIN, HIGH);   // turn the LED on (HIGH is the voltage level)
  delay(1000);                       // wait for a second
  digitalWrite(LED_BUILTIN, LOW);    // turn the LED off by making the voltage LOW
  delay(1000);                       // wait for a second
  temp = 15+(float)random(0, 10000)/(float)1000.0;
  Serial.print("Temperatura Medida ");
  Serial.println(temp);

  Serial.print("connecting to ");
  Serial.println(host);
  WiFiClient client;
  if (!client.connect(host, httpPort)) {
  Serial.println("connection failed");
  return;
  }
  String respuesta = PostData();
  Serial.println(respuesta);
  delay(15000);
}

String PostData(){
  String respuesta = "NOK";
  WiFiClient client;
  if (!client.connect(host, httpPort)) {
    Serial.println("connection failed");
  }
  else{
    Serial.println("Requesting URL: ");
    String Mensaje = "MEDICION=" + String(temp);
    String httprequest = "POST /Sensor/PostData/"+String(id)+" HTTP/1.1\r\n"+
                          "Host:"+host+":"+httpPort+"\r\n"+
                          "Content-Type: application/x-www-form-urlencoded\r\n" +
                          "Content-Length:"  + Mensaje.length() + "\r\n" +
                          "\r\n" +Mensaje;
    Serial.println(httprequest);
    client.print(httprequest);
    
     unsigned long timeout = millis();
     while (client.available() == 0) {
        if (millis() - timeout > 5000) {
        Serial.println(">>> Client Timeout !");
        client.stop();
        return respuesta;  
        }
    }
     // Read all the lines of the reply from server and print them to Serial
     while(client.available()){
        String line = client.readStringUntil('\r');
        Serial.print(line);
     }
  
     Serial.println();
     Serial.println("Closing connection");
     respuesta = "OK";
  }
  return respuesta;  
 }
