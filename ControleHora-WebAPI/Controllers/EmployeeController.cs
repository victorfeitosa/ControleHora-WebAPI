using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using ControleHora_WebAPI.Models;
using System;
using System.Linq;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace ControleHora_WebAPI.Controllers
{
    [Route("[controller]")]
    public class EmployeeController : Controller
    {
        public static string ConnectionString = "mongodb://localhost:27017";
        public static MongoClient Client = new MongoClient(ConnectionString);
        public static IMongoDatabase DB = Client.GetDatabase("controle_horas");

        #region Gets
        //Gets
        [HttpGet]
        public string Get()
        {
            List<Employee> employees = new List<Employee>();
            IMongoCollection<Employee> collection = DB.GetCollection<Employee>("employees");
            try
            {
                employees = collection.Find(new BsonDocument()).ToList();
                if (employees.Count <= 0)
                {
                    throw new Exception("Error, couldn't find any documents.");
                }

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error on listing employees");
                System.Console.WriteLine(e);
            }

            return employees.ToJson();
        }

        [HttpGet("id/{id}")]
        public string Get(string id)
        {
            if (id == null)
            {
                throw new Exception($"Error, couldn't find object id parameter.");
            }
            List<Employee> employees = new List<Employee>();
            IMongoCollection<Employee> collection = DB.GetCollection<Employee>("employees");
            try
            {
                var objId = ObjectId.Parse(id);
                employees = collection.Find(x => x.ID == objId).ToList();
                if (employees.Count <= 0)
                {
                    throw new Exception($"Error, couldn't find any documents with guid {objId}.");
                }

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error on listing employees");
                System.Console.WriteLine(e);
            }
            return employees.ToJson();
        }

        [HttpGet("{name}")]
        public string GetName(string name)
        {
            List<Employee> employees = new List<Employee>();
            IMongoCollection<Employee> collection = DB.GetCollection<Employee>("employees");
            try
            {
                employees = collection.Find(x => x.Name == name).ToList();
                if (employees.Count <= 0)
                {
                    throw new Exception("Error, couldn't find any documents.");
                }

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error on listing employees");
                System.Console.WriteLine(e);
            }
            return employees.ToJson();
        }

        [HttpGet("entries/{id?}")]
        public string GetEntries(string id)
        {
            // var entries = new List<Employee>();
            var collection = DB.GetCollection<Employee>("employees");
            var entries = new List<BsonDocument>();

            try
            {
                if (id == null)
                {
                    entries = collection.Aggregate()
                                .Unwind("hours")
                                .Group("{_id: '', hours: {$addToSet: {employee_id: '$hours.employee_id'" +
                                                "employee: '$name'" +
                                                "date: '$hours.date'," +
                                                "reason: '$hours.reason'," +
                                                "amount: '$hours.amount' }}}")
                                .Project("{_id: 0, hours: 1}")
                                .ToList();
                }
                else
                {
                    entries = collection.Aggregate()
                                .Match(x => x.ID == ObjectId.Parse(id))
                                .Unwind("hours")
                                .Group("{_id: null, hours: {$addToSet: {employee_id: '$hours.employee_id'" +
                                                "employee: '$name'" +
                                                "date: '$hours.date'," +
                                                "reason: '$hours.reason'," +
                                                "amount: '$hours.amount' }}}")
                                .Project("{_id: 0, hours: 1}")
                                .ToList();
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error on listing hour entries");
                System.Console.WriteLine(e);
            }

            return entries.ToJson();
        }

        #endregion
        //Posts
        [HttpPost]
        public IActionResult PostEmployee([FromBody] JObject employeeJson)
        {
            var collection = DB.GetCollection<Employee>("employees");

            //deserializes JSON received from the DB naming convention
            var employee = BsonSerializer.Deserialize<Employee>(employeeJson.ToString());

            if (employee.DateJoined == null)
            {
                employee.DateJoined = DateTime.Now;
            }

            try
            {
                collection.InsertOne(employee);
                System.Console.WriteLine($"Inserted employee:\n{employee.ToString()}");
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error, could not insert Employee into the database.");
                System.Console.WriteLine(e);
                throw new Exception();
            }
            return RedirectToAction("Get", new { id = employee.ID.ToString() });
        }

        [HttpPost("addhour")]
        public IActionResult PostHourEntry([FromBody] JObject hourJson)
        {
            var collection = DB.GetCollection<Employee>("employees");
            if (hourJson.Property("employee_id") == null)
            {
                throw new Exception("Error, employee id is not valid");
            }

            try
            {
                var hourEntry = BsonSerializer.Deserialize<HourEntry>(hourJson.ToString());
                var employeeObjId = hourEntry.EmployeeId;
                if (hourEntry.EmployeeName == null)
                {
                    hourEntry.EmployeeName = collection.Find(x => x.ID == employeeObjId)
                                                .First().Name;
                }
                var employee = collection.FindOneAndUpdate<Employee>(x => x.ID == employeeObjId,
                                Builders<Employee>.Update
                                .AddToSet(x => x.Entries, hourEntry)
                                .Inc(x => x.HourBank, hourEntry.Amount));
            }
            catch (System.FormatException e)
            {
                System.Console.WriteLine("Error, JSON Format not recognized!");
                System.Console.WriteLine(e);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error, could not insert Hour Entry into the database.");
                System.Console.WriteLine(e);

            }

            return RedirectToAction("Get", new {id = hourJson.Property("employee_id").Value});
        }
        //TODO: post to insert hours into a addHour/{id}

        [HttpPut]
        public IActionResult Put([FromBody] string employeeJson)
        {
            var collection = DB.GetCollection<Employee>("employees");
            var employee = BsonSerializer.Deserialize<Employee>(employeeJson);
            System.Console.WriteLine($"Employee Class\n\t{employee.ToString()}");

            try
            {
                collection.ReplaceOne(Builders<Employee>.Filter.Eq("_id", employee.ID), employee);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error, could not update Employee in the database.");
                System.Console.WriteLine(e);
                throw new Exception();
            }

            return RedirectToAction("Get");
        }

        [HttpDelete("user/id")]
        public IActionResult Delete(string id)
        {
            List<Employee> employees = new List<Employee>();
            var objId = ObjectId.Parse(id);
            IMongoCollection<Employee> collection = DB.GetCollection<Employee>("employees");
            try
            {
                var filter = Builders<Employee>.Filter;
                collection.DeleteOne(x => x.ID == objId);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error on deleting employee");
                System.Console.WriteLine(e);
            }
            return RedirectToAction("Get");
        }
    }
}
