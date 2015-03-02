﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitusCore.Controllers;
using UnitusCore.Controllers.Misc;
using UnitusCore.Models.BaseClasses;
using UnitusCore.Util;

namespace UnitusCore.Models.DataModel
{
    public class Person : ModelBaseWithTimeLogging,IMajorInfoContainer
    {
        public Person()
        {
            AttendedEvents=new HashSet<Event>();
            BelongedCircles = new HashSet<MemberStatus>();
            CommittedProjects = new HashSet<Project>();
            Skills = new HashSet<Skill>();
            InvitedPeople=new HashSet<CircleMemberInvitation>();
            UserStatistics=new HashSet<UserStatistics>();
        }

        public ApplicationUser ApplicationUser { get; set; }//binded

        public string Name { get; set; }

        public string Email { get; set; }

        public ICollection<MemberStatus> BelongedCircles { get; set; }//binded

        public ICollection<Project> CommittedProjects { get; set; }//binded

        public Cource CurrentCource { get; set; }

        public string BelongedSchool { get; set; }

        public string Faculty { get; set; }

        public string Major { get; set; }

        public string Notes { get; set; }

        public ICollection<Event> AttendedEvents { get; set; } //binded

        public ICollection<Skill> Skills { get; set; }//binded

        public ICollection<UserStatistics> UserStatistics { get; set; } 

        public ICollection<CircleMemberInvitation> InvitedPeople { get; set; } //binded

        public enum Cource
        {
            UG1, UG2, UG3, UG4, UG5, UG6, MC1, MC2, MC3, MC4, DC1, DC2, DC3, DC4
        }

        public async static Task<Person> FromIdAsync(ApplicationDbContext dbSession, string personID)
        {
            Guid guid = personID.ToValidGuid();
            return await dbSession.People.FindAsync(guid);
        }


        public async Task LoadStatisticsData(ApplicationDbContext dbSession)
        {
            var statisticsStatus = dbSession.Entry(this).Collection(a => a.UserStatistics);
            if (!statisticsStatus.IsLoaded) await statisticsStatus.LoadAsync();
        }

        public async Task LoadApplicationUser(ApplicationDbContext dbSession)
        {
            var applicationUserLoadingStatus = dbSession.Entry(this).Reference(a => a.ApplicationUser);
            if (!applicationUserLoadingStatus.IsLoaded) await applicationUserLoadingStatus.LoadAsync();
        }

        public async Task LoadBelongingCircles(ApplicationDbContext dbSession)
        {
            var circlesLoadingStatus = dbSession.Entry(this).Collection(a => a.BelongedCircles);
            if (!circlesLoadingStatus.IsLoaded) await circlesLoadingStatus.LoadAsync();
        }
    }
}