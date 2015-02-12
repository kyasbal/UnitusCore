using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity.Owin;
using UnitusCore.Attributes;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Results;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class PeopleController : UnitusApiController
    {
        [EnableCors(GlobalConstants.CorsOrigins, "*", "*")]
        [Route("Person")]
        [Authorize]
        [RoleRestrict("Administrator")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPersonList(string validationKey,int Count=20,int Offset=0)
        {
            return await this.OnValidToken(validationKey, () =>
            {
                var persons =
                    DbSession.People.OrderBy(o => o.Id)
                        .Skip(Offset)
                        .Take(Count)
                        .Select(a =>a).ToArray();
                return Json(ResultContainer<GetPersonListResponse>.GenerateSuccessResult(new GetPersonListResponse(persons.Select(a=>GetPersonListPersonEntity.FromPerson(a)).ToArray())));
            });
        }

        [EnableCors(GlobalConstants.CorsOrigins,"*","*")]
        [Route("Person/Dummy")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPersonListDummy(int Count=20,int Offset=10)
        {
            return await this.OnValidToken("",() =>
            {
                var randomValues = new GetPersonListPersonEntity[178];
                for (int index = 0; index < randomValues.Length; index++)
                {
                    randomValues[index] = GetPersonListPersonEntity.GenerateDummy(index);
                }
                var persons =
                    randomValues.AsQueryable().OrderBy(o => o.Name)
                        .Skip(Offset)
                        .Take(Count)
                        .Select(a => a).ToArray();
                
                return Json(ResultContainer<GetPersonListResponse>.GenerateSuccessResult(new GetPersonListResponse(persons.ToArray())));
            });
        }
    }

    public class GetPersonListRequest :AjaxRequestModelBase
    {
        public GetPersonListRequest()
        {
            Count = 20;
            Offset = 0;
        }

        public int Count { get; set; }

        public int Offset { get; set; }
    }

    public class GetPersonListResponse
    {
        public GetPersonListResponse()
        {
            
        }

        public GetPersonListResponse(GetPersonListPersonEntity[] persons)
        {
            Persons = persons;
        }

        public GetPersonListPersonEntity[] Persons { get; set; }
    }

    public class GetPersonListPersonEntity
    {

        public static GetPersonListPersonEntity FromPerson(Person p)
        {
            return new GetPersonListPersonEntity(p.Email,p.BelongedColledge,p.Name,p.CurrentCource);
        }

        public static GetPersonListPersonEntity GenerateDummy(int index)
        {
            return new GetPersonListPersonEntity(index+IdGenerator.GetId(4)+"@gmail.com",IdGenerator.GetId(4)+"大学",IdGenerator.GetId(4)+" "+IdGenerator.GetId(4),IdGenerator.GetRandomEnum<Person.Cource>());
        }

        public string UserName { get; set; }

        public string BelongedTo { get; set; }

        public string Name { get; set; }

        public Person.Cource Grade { get; set; }


        public GetPersonListPersonEntity()
        {
        }

        public GetPersonListPersonEntity(string userName, string belongedTo, string name, Person.Cource grade)
        {
            UserName = userName;
            BelongedTo = belongedTo;
            Name = name;
            Grade = grade;
        }
    }

    
}
