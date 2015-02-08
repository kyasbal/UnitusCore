using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace UnitusCore.Models
{
    public class BasicDbContext : DbContext
    {

        public static BasicDbContext Create()
        {
            return new BasicDbContext();
        }
        public BasicDbContext()
        {
            
        }
        public DbSet<Circle> Circles { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Achivement> Achivements { get; set; }

        public DbSet<Person> People { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<Statistics> Statisticses { get; set; }

        public DbSet<CircleStatistics> CircleStatisticses { get; set; }

        public DbSet<MemberStatus> MemberStatuses { get; set; }
    }
    public abstract class ModelBase
    {
        public void GenerateId()
        {
            Id=Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
    }

    public class Circle :ModelBase
    {
        public Circle()
        {
            Events = new HashSet<Event>();
            Projects = new HashSet<Project>();
            Achivements=new HashSet<Achivement>();
            Members=new HashSet<MemberStatus>();
            CircleStatistises=new HashSet<CircleStatistics>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public int MemberCount { get; set; }

        public string WebAddress { get; set; }

        public string BelongedSchool { get; set; }

        public ICollection<Event> Events { get; set; }
        
        public ICollection<Project> Projects { get; set; }
        
        public ICollection<Achivement> Achivements { get; set; }

        public ICollection<MemberStatus> Members { get; set; } 
        
        public string  Notes { get; set; }

        public string Contact { get; set; }

        public bool CanInterCollege { get; set; }

        public ICollection<CircleStatistics> CircleStatistises { get; set; }
        
        public CircleStatistics LastCircleStatistics { get; set; } 
    }

    public class MemberStatus : ModelBase
    {
        public string Occupation { get; set; }

        public bool IsActiveMember { get; set; }

        public Person TargetPerson { get; set; }
    }

    public class Project :ModelBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ProjectAddress { get; set; }

        public ICollection<Circle> Circles { get; set; }

        public string Notes { get; set; }

        public ICollection<Person> Members { get; set; }

        public int Progress { get; set; }

        public DateTime BeginTime { get; set; }

        public enum  ProjectType
        {
            EventProject,StandaloneProject
        }
    }

    public class Event : ModelBase
    {
        public Event()
        {
            Circles=new HashSet<Circle>();
            Achivements = new HashSet<Achivement>();
            Projects = new HashSet<Project>();
            Participants=new List<Person>();
        }
        public bool AlreadyHosted { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public ICollection<Circle> Circles { get; set; }

        public ICollection<Achivement> Achivements { get; set; }

        public ICollection<Project> Projects { get; set; }

        public ICollection<Person> Participants { get; set; }


    }

    public class Achivement :ModelBase
    {
        public Achivement()
        {
            Events=new HashSet<Event>();
            Projects=new HashSet<Project>();
        }
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Event> Events { get; set; }

        public ICollection<Project> Projects { get; set; } 
    }

    public class Person :ModelBase
    {
        public Person()
        {
            BelongedCircles = new HashSet<Circle>();
            CommittedProjects=new HashSet<Project>();
            Skills=new HashSet<Skill>();
        }
        public string Name { get; set; }

        public string Email { get; set; }

        public ICollection<Circle> BelongedCircles { get; set; }

        public ICollection<Project> CommittedProjects { get; set; }

        public Cource CurrentCource { get; set; }

        public string BelongedColledge { get; set; }

        public string Faculty { get; set; }

        public string Major { get; set; }

        public string Notes { get; set; }

        public ICollection<Skill> Skills { get; set; }



        public enum Cource
        {
            UG1, UG2, UG3, UG4, UG5, UG6, MC1, MC2, MC3, MC4, DC1, DC2, DC3, DC4
        }
    }

    public class Skill :ModelBase
    {
        public string Name { get; set; }

        public ICollection<Person> People { get; set; }
    }

    public class Statistics : ModelBase
    {
        public DateTime StatDate { get; set; }

        public int SumCircles { get; set; }

        public int SumPeoples { get; set; }
    }

    public class CircleStatistics : ModelBase
    {
        public int GithubUserCount { get; set; }

        public int RepositoryCount { get; set; }

        public int CommitCount { get; set; }

        public double CommitPerUser { get; set; }

        public Circle RelatedCircle { get; set; }

        public DateTime StatDate { get; set; }
    }
}