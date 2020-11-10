#include <ESP8266WiFi.h>d

const char* ssid = "GABY_CATA";
const char* password = "ZAQzaqZAQ1234";
String host = "mrjarkos-001-site1.itempurl.com";
const int httpPort = 80;

const int id=random(0, 999);
String serial = "NS085";
String CookieDisp;
String Message;
String GH;
const char* PASSWORD = "1234";
bool unaut=true;

String GETMessage();
String id2serial(int id);
void GETLogin();
String getValue(String data, char separator, int index);
String PostData(String Date);

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
  
  //serial = id2serial(id);
  CookieDisp = " ";
  Message = "OK";
}

// the loop function runs over and over again forever
void loop() {
  digitalWrite(LED_BUILTIN, HIGH);   // turn the LED on (HIGH is the voltage level)
  delay(1000);                       // wait for a second
  digitalWrite(LED_BUILTIN, LOW);    // turn the LED off by making the voltage LOW
  delay(1000);                       // wait for a second

  Serial.print("connecting to ");
  Serial.println(host);
  WiFiClient client;
  if (!client.connect(host, httpPort)) {
  Serial.println("connection failed");
  return;
  }
  Serial.println("Autenticando");
  while(CookieDisp==" "){
      Serial.print(".");
      GETLogin();
  }
  Serial.println("Autenticado en Servidor");
  
  if (unaut)
  {
      CookieDisp = " ";
      GH = " ";
  }else{
    GH = GETMessage();
    delay(1000); 
    if (GH!=" ")
    {
        PostData(GH);
    }
    delay(1000);
  }
  delay(5000);
}

String PostData(String Date){
  String respuesta = "NOK";
  WiFiClient client;
  if (!client.connect(host, httpPort)) {
    Serial.println("connection failed");
  }
  else{
    Serial.println("Requesting URL: ");
    float ValueSensor = random(1000, 8000)/100;
    float Latitud = ((random(1000, 8000)/10000)*0.28009) + 4.48676;
    float Longitud = ((random(1000, 8000)/10000)* 0.19535) - 74.22157;
    String Variable = "SERIALSENSOR=" + serial + "&" + "MEDICION=" + String(ValueSensor) +
                                  "&LATITUD=" + String(Latitud) + "&LONGITUD=" + String(Longitud) +
                                  "&DATEREPORTED=" + Date;
    String httprequest = "POST /Reporte/PostData HTTP/1.1\r\nHost:"+host+":"+httpPort+
                          "\r\nCookie:" + CookieDisp +
                          "\r\nContent-Type: application/x-www-form-urlencoded\r\n" +
                          "Content-Length:"  + Variable.length() + "\r\n\r\n" +
                          Variable + "\r\n";
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

 String id2serial(int id){
    if(id<10){
      return "NS00"+id;
     }else if(id<100){
      return "NS0"+id;
     }else{
      return "NS"+id;
     }
  }

String GETMessage(){
  String respuesta = "NOK";

  WiFiClient client;
  if (!client.connect(host, httpPort)) {
    Serial.println("connection failed");
  }
  else{
    Serial.println("Requesting URL: ");
    String httprequest = "GET /Reporte/GetDateTime/ HTTP/1.1\r\nHost:"+host+":"+httpPort+
                          "\r\nCookie:" + CookieDisp + "\r\n\r\n";
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
    int content = 0;
     // Read all the lines of the reply from server and print them to Serial
     while(client.available()){
        String line = client.readStringUntil('\r');
        if(content==2){
          return line;
          }
         if(content==1){
          content = 2;
          }
        if(line.indexOf("Content-Length:") >= 0){
          content = 1;          
          }
        if(Message=="unauthorized"){
          unaut = true;
        }
        Serial.print(line);
     }
  
     Serial.println();
     Serial.println("Closing connection");
     
  }
  return respuesta;  
}


void GETLogin(){
  String parameters = "?SERIALSENSOR=" + serial + "&" + "PASSWORD=" + PASSWORD;

  WiFiClient client;
  if (!client.connect(host, httpPort)) {
    Serial.println("connection failed");
  }
  else{
    Serial.println("Requesting URL: ");
    String httprequest = "GET /Reporte/GetLogin/" + parameters + " HTTP/1.1\r\nHost:"+host+":"+httpPort+"\r\n\r\n";
    Serial.println(httprequest);
    client.print(httprequest);
    
     unsigned long timeout = millis();
     while (client.available() == 0) {
        if (millis() - timeout > 5000) {
        Serial.println(">>> Client Timeout !");
        client.stop();
        }
    }
     // Read all the lines of the reply from server and print them to Serial
     while(client.available()){
        String line = client.readStringUntil('\r');
        Serial.print(line);
        if(line.indexOf("Set-Cookie") > 0){
          line.remove(0, 12);
          CookieDisp = line;
          Serial.println(" ");
          Serial.println(CookieDisp);
          unaut = false;
          }
          if(Message=="unauthorized"){
          unaut = true;
        }
     }
     Serial.println();
     Serial.println("Closing connection");
  } 
}

String getValue(String data, char separator, int index)
{
  int found = 0;
  int strIndex[] = {0, -1};
  int maxIndex = data.length()-1;

  for(int i=0; i<=maxIndex && found<=index; i++){
    if(data.charAt(i)==separator || i==maxIndex){
        found++;
        strIndex[0] = strIndex[1]+1;
        strIndex[1] = (i == maxIndex) ? i+1 : i;
    }
  }

  return found>index ? data.substring(strIndex[0], strIndex[1]) : "";
}
