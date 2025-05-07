using System.Net;
using System.IO;
using System.Text;
using System.Xml;

namespace ServerRpc
{
    class Program
    {
        static void Main()
        {
            Server service = new Server();
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();
            System.Console.WriteLine("Сервер запущен...");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();

                //лог запроса
                Stream inputStream = context.Request.InputStream;
                StreamReader reader = new StreamReader(inputStream);
                string requestXml = reader.ReadToEnd();
                LogXml("Request", requestXml);
                reader.Close();
                inputStream.Close();

                //обработка ответа
                byte[] requestBytes = Encoding.UTF8.GetBytes(requestXml);
                MemoryStream requestStream = new MemoryStream(requestBytes);
                Stream responseStream = service.Invoke(requestStream);
                requestStream.Close();

                //лог ответа
                StreamReader responseReader = new StreamReader(responseStream);
                string responseXml = responseReader.ReadToEnd();
                LogXml("Response", responseXml);

                //обработка ответа
                byte[] responseBytes = Encoding.UTF8.GetBytes(responseXml);
                context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
                responseReader.Close();
                responseStream.Close();
                context.Response.Close();
            }
        }

        static void LogXml(string type, string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings{ Indent = true });
            doc.Save(writer);
            writer.Close();

            System.Console.WriteLine($"---------- {type} ----------\n{sb.ToString()}\n");
        }
    }
}