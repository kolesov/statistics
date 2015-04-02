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
            /*string url = @"http://www.seek.com.au/job/28220225?pos=7&type=standard";
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
            Console.WriteLine(salary);*/

            string listUrl = @"http://www.seek.com.au/jobs-in-information-communication-technology/developers-programmers/#dateRange=1&workType=0&industry=6281&occupation=6287&graduateSearch=false&salaryFrom=0&salaryTo=999999&salaryType=annual&page=1&isAreaUnspecified=falseListedDate";
            WebRequest request = WebRequest.Create(listUrl);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();
            CQ document = content;
            reader.Close();
            response.Close();
            File.WriteAllText("html.html", content);
            Console.WriteLine(ScriptEngine.Eval("jscript", "1+2/3"));
        }
    }
}
