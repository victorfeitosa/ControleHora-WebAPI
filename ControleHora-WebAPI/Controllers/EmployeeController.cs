using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using ControleHora_WebAPI.Models;
using Newtonsoft.Json;
using System;
using MongoDB.Bson.Serialization;

namespace ControleHora_WebAPI.Controllers
{
    [Route("[controller]")]
    public class EmployeeController : Controller
    {
        public static string ConnectionString = "mongodb://localhost:27017";
        public static MongoClient Client = new MongoClient(ConnectionString);
        public static IMongoDatabase DB = Client.GetDatabase("nettest");


        [HttpGet]
        public IEnumerable<Employee> Get()
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
                throw new Exception();
            }
            return employees;
        }

        [HttpPost]
        public IActionResult Post([FromBody] string employeeJson)
        {
            var collection = DB.GetCollection<Employee>("employees");
            var employee = BsonSerializer.Deserialize<Employee>(employeeJson);
            System.Console.WriteLine($"Employee JSON\n\t{employeeJson}");
            System.Console.WriteLine($"Employee Class\n\t{employee.ToString()}");

            if (employee.DateJoined == null)
            {
                employee.DateJoined = DateTime.Now;
            }

            try
            {
                collection.InsertOne(employee);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error, could not insert Employee into the database.");
                System.Console.WriteLine(e);
                throw new Exception();
            }
            return RedirectToAction("Get");
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
    }
}