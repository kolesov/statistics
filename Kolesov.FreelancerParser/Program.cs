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
using Kolesov.Domain.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

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

        static bool SuitableKeyWords(Project project, List<string> keyWords, List<string> minusKeyWords)
        {
            bool result = false;

            if (keyWords != null)
            {
                foreach (var keyword in keyWords)
                {
                    if (project.Title.ToLower().Contains(keyword.ToLower()) || project.Description.ToLower().Contains(keyword.ToLower()))
                    {
                        result = true;
                        break;
                    }
                }
            }
            if (minusKeyWords != null)
            {
                foreach (var keyword in minusKeyWords)
                {
                    if (project.Title.ToLower().Contains(keyword.ToLower()) || project.Description.ToLower().Contains(keyword.ToLower()))
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        static void Main(string[] args)
        {
            ISkillsRepository skillsRepository = new FileSkillsRepository();
            IProjectRepository projectRepository = new FileProjectRepository();
            INotificationService notificationService = new SendEmailService();

            var users = new List<User>();
            users.Add(new User()
            {
                Id = 1,
                Emails = new List<string>() { "sergey.kolesov.gs@gmail.com" },
                Name = "Sergey",
                InterestedSkills = new List<string>() { ".NET", "ASP.NET", "HTML5", "MVC", "C# Programming"/*, "CSS", "HTML", "Javascript", "Software Architecture", "Bootstrap", "AJAX", "jQuery / Prototype", "Web Scraping"*/ },
                ExcludeSkills = new List<string>() { "PHP", "Wordpress" },
                KeyWords = new List<string>() { "Australia", "New Zealand" }
            });
            users.Add(new User()
            {
                Id = 2,
                Emails = new List<string>() { "sergey.kolesov.gs@gmail.com", "kamile-mamedova@mail.ru" },
                Name = "Kamile",
                InterestedSkills = new List<string>() { "Russian" },
                ExcludeSkills = new List<string>() {  },
                KeyWords = new List<string>() { "Russia", "Russian" }
            });

            string domain = "http://www.freelancer.com";
            while (true)
            {
                var html = GetPage(domain + "/jobs/1/");
                var getJsonaaData = new Regex(@"var\s*aaData\s*=\s*(?<json>.*);");
                var json = getJsonaaData.Match(html).Groups["json"].Value;
                
                var projects = JsonConvert.DeserializeObject<dynamic>(json);
                Console.WriteLine("Projects List loaded");

                if (projects == null)
                    continue;

                int i = 0;
                foreach (var item in projects)
                {
                    i++;
                    Console.WriteLine(i.ToString() + "/" + projects.Count);
                    var href = domain + item[21]; // link from json array
                    var project = new Project()
                    {
                        Link = href
                    };

                    if (!projectRepository.Exists(project))
                    {
                        string page = GetPage(href);
                        File.WriteAllText("current.html", page);
                        CQ projectPage = page;
                        project.Title = projectPage[".project-view-project-title"].Text();
                        project.Budget = projectPage[".project-statistic-value"].Text();
                        project.Description = projectPage[".project-description p"].Text();
                        var guid = Guid.NewGuid().ToString();
                        if (string.IsNullOrEmpty(project.Title))
                        {
                            File.WriteAllText("page_" + guid + "_notitle.html", page);
                        }
                        if (string.IsNullOrEmpty(project.Description))
                        {
                            File.WriteAllText("page_" + guid + "_nodescr.html", page);
                        }
                        if (string.IsNullOrEmpty(project.Budget))
                        {
                            File.WriteAllText("page_" + guid + "_nobudget.html", page);
                        }
                        foreach (var skill in projectPage["ul.project-view-landing-required-skill a.simple-tag"])
                        {
                            project.Skills.Add(skill.InnerText);
                            skillsRepository.Add(skill.InnerText);
                        }
                        if (project.Skills.Count == 0)
                        {
                            File.WriteAllText("page_" + guid + "_noskills.html", page);
                        }
                        Console.WriteLine(href);

                        foreach (var user in users)
                        {
                            if ((project.Skills.Intersect(user.InterestedSkills).Any() 
                                && !project.Skills.Intersect(user.ExcludeSkills).Any())
                                || SuitableKeyWords(project, user.KeyWords, user.MinusKeyWords))
                            {
                                notificationService.SendNotification(user, project.ToEmailMessage());
                            }
                        }

                        projectRepository.Add(project);
                    }
                }
            }
        }
    }
}
