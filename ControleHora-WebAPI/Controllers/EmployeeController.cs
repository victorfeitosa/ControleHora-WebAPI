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

namespace ControleHora_WebAPI.Controllers
{
    [Route("[controller]")]
    public class EmployeeController : Controller
    {
        public static string ConnectionString = "mongodb://localhost:27017";
        public static MongoClient Client = new MongoClient(ConnectionString);
        public static IMongoDatabase DB = Client.GetDatabase("controle_horas");


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
                var filter = Builders<Employee>.Filter;
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
                var filter = Builders<Employee>.Filter;
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

        [HttpPost]
        public IActionResult Post([FromBody] JObject employeeJson)
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
            return RedirectToAction("Get", new {id=employee.ID.ToString()});
        }

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
