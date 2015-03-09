using CsQuery;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace Kolesov.FreelancerParser
{
    class Program
    {
        static string GetPage(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();
                reader.Close();
                response.Close();

                Thread.Sleep(3000);
                return content;
            }
            catch
            {
                return "";
            }
        }

        static void Main(string[] args)
        {
            var projects = new List<string>();

            while (true)
            {
                CQ mainPage = GetPage(@"http://www.freelancer.com.au/jobs/1/");

                foreach (var item in mainPage["tr.project-details a[data-id]"])
                {
                    var href = item.Attributes["href"];
                    href = href.Replace("https", "http").Replace("/projects/", "/jobs/").Replace(".html", "/");

                    if (!projects.Contains(href))
                    {
                        string page = GetPage(href);
                        File.WriteAllText("current.html", page);
                        CQ projectPage = page;
                        var title = projectPage[".project-view-project-title"].Text();
                        var budget = projectPage[".project-statistic-value"].Text();
                        var description = projectPage[".project-description p"].Text();
                        var skills = new List<string>();
                        foreach (var skill in projectPage["ul.project-view-landing-required-skill a.simple-tag"])
                        {
                            skills.Add(skill.InnerText);
                        }
                        Console.WriteLine(href);
                        if (budget.Contains("AUD") || budget.Contains("NZD"))
                        {
                            var fromAddress = new MailAddress("kolesov.statistics@gmail.com", "Statistics");
                            var toAddress = new MailAddress("sergey.kolesov.gs@gmail.com", "Sergey Kolesov");
                            var fromPassword = "kolesov.password";
                            var subject = "New project";
                            var body = title+description+budget+string.Join(", ", skills)+"\n\n"+href;

                            var smtp = new SmtpClient
                            {
                                Host = "smtp.gmail.com",
                                Port = 587,
                                EnableSsl = true,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = false,
                                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                                Timeout = 20000
                            };
                            using (var message = new MailMessage(fromAddress, toAddress)
                            {
                                Subject = subject,
                                Body = body
                            })
                            {
                                smtp.Send(message);
                            }
                        }

                        projects.Add(href);
                    }
                }
            }
        }
    }
}
