﻿using CsQuery;
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
                Email = "sergey.kolesov.gs@gmail.com",
                Name = "Sergey",
                InterestedSkills = new List<string>() { ".NET", "ASP.NET", "HTML5", "MVC", "C# Programming", "CSS", "HTML", "Javascript", "Software Architecture", "Bootstrap", "AJAX", "jQuery / Prototype", "Web Scraping" },
                ExcludeSkills = new List<string>() { "PHP", "Wordpress" },
                KeyWords = new List<string>() { "Australia", "New Zealand", "*" }
            });

            while (true)
            {
                CQ mainPage = GetPage(@"http://www.freelancer.com.au/jobs/1/");
                Console.WriteLine("Projects List loaded");

                foreach (var item in mainPage["tr.project-details a[data-id]"])
                {
                    var href = item.Attributes["href"];
                    href = href.Replace("https", "http").Replace("/projects/", "/jobs/").Replace(".html", "/");
                    var project = new Project()
                    {
                        Link = href
                    };

                    if (!projectRepository.Exists(project))
                    {
                        string page = GetPage(href);
                        File.WriteAllText("current.html", page);
                        CQ projectPage = page;
                        var title = projectPage[".project-view-project-title"].Text();
                        var budget = projectPage[".project-statistic-value"].Text();
                        var description = projectPage[".project-description p"].Text();
                        foreach (var skill in projectPage["ul.project-view-landing-required-skill a.simple-tag"])
                        {
                            project.Skills.Add(skill.InnerText);
                            skillsRepository.Add(skill.InnerText);
                        }
                        Console.WriteLine(href);

                        foreach (var user in users)
                        {
                            if ((project.Skills.Intersect(user.InterestedSkills).Any() 
                                && !project.Skills.Intersect(user.ExcludeSkills).Any())
                                || SuitableKeyWords(project, user.KeyWords, user.MinusKeyWords))
                            {
                                string message = title + description + budget + string.Join(", ", project.Skills) + "\n\n" + href;
                                notificationService.SendNotification(user, message);
                            }
                        }

                        projectRepository.Add(project);
                    }
                }
            }
        }
    }
}
