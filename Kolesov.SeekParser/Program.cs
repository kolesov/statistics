using CsQuery;
using System;
using System.IO;
using System.Net;

namespace Kolesov.SeekParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = @"http://www.seek.com.au/job/28220225?pos=7&type=standard";
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            CQ html = reader.ReadToEnd();
            reader.Close();
            response.Close();

            string content = html["#jobTemplate div"].Text();
            string salary = html["div[itemprop=baseSalary]"].Text();
            
            Console.WriteLine(content);
            Console.WriteLine(salary);
            Console.ReadKey();
        }
    }
}
