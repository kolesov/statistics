using CsQuery;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace Kolesov.FreelancerParser
{
    class Program
    {
        static string GetPage(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();
            reader.Close();
            response.Close();

            Thread.Sleep(2000);
            return content;
        }

        static void Main(string[] args)
        {
            var projects = new List<string>();

            CQ mainPage = GetPage(@"http://www.freelancer.com.au/jobs/1/");

            foreach (var item in mainPage["tr.project-details a[data-id]"])
            {
                var href = item.Attributes["href"];
                href = href.Replace("https", "http").Replace("/projects", "/jobs/").Replace(".html", "/");

                if (!projects.Contains(href))
                {
                    CQ projectPage = GetPage(href);
                    var budget = projectPage[".project-statistic-value"].Text();
                    if (budget.Contains("AUD"))
                        Console.WriteLine(href);

                    projects.Add(href);
                }
            }

            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
