using CsQuery;
using Kolesov.Domain.Interfaces;
using Kolesov.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Linq;

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

                Thread.Sleep(2500);
                return content;
            }
            catch
            {
                return "";
            }
        }

        static void Main(string[] args)
        {
            ISkillsRepository skillsRepository = new SkillsRepository();
            IProjectRepository projectRepository = new ProjectRepository();
            INotificationService notificationService = new SendEmailService();

            var interestedSkills = new List<string>() { ".NET", "ASP.NET", "HTML5", "MVC", "C# Programming", "CSS", "HTML", "Javascript", "Software Architecture", "Bootstrap", "AJAX", "jQuery / Prototype", "Web Scraping" };
            var excludeSkills = new List<string>() { "PHP", "Wordpress" };

            while (true)
            {
                CQ mainPage = GetPage(@"http://www.freelancer.com.au/jobs/1/");
                Console.WriteLine("Projects List loaded");

                foreach (var item in mainPage["tr.project-details a[data-id]"])
                {
                    var href = item.Attributes["href"];
                    href = href.Replace("https", "http").Replace("/projects/", "/jobs/").Replace(".html", "/");

                    if (!projectRepository.Exists(href))
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
                        foreach (var skill in skills)
                        {
                            skillsRepository.Add(skill);
                        }
                        Console.WriteLine(href);
                        if (skills.Intersect(interestedSkills).Any() && !skills.Intersect(excludeSkills).Any() && (budget.Contains("AUD") || budget.Contains("NZD")))
                        {
                            string message = title+description+budget+string.Join(", ", skills)+"\n\n"+href;
                            notificationService.SendNotification(message);
                        }

                        projectRepository.Add(href, skills.ToArray());
                    }
                }
            }
        }
    }
}
