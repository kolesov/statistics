using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kolesov.SeekParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = @"http://www.seek.com.au/job/28220831?pos=4&type=standard";
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();
            reader.Close();
            response.Close();

            Console.WriteLine(content);
            Console.ReadKey();
        }
    }
}
