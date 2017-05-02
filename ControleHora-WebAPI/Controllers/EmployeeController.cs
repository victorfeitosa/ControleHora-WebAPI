using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using ControleHora_WebAPI.Models;
using System;

namespace ControleHora_WebAPI.Controllers
{
    [Route("[controller]")]
    public class EmployeeController : Controller
    {
        public static string ConnectionString = "mongodb://localhost:27017";
        public static MongoClient Client = new MongoClient(ConnectionString);
        public static IMongoDatabase DB = Client.GetDatabase("controle_horas");


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
            }
            foreach (Employee employee in employees)
            {
                System.Console.WriteLine("List of employees:\n" + employee.ToString());
            }

            return employees;
        }

        [HttpGet("id/{id}")]
        public IEnumerable<Employee> Get(string id)
        {
            if(id == null)
            {
                throw new Exception($"Error, couldn't find object id parameter.");
            }
            System.Console.WriteLine("ObjectId: " + id);
            var objId = ObjectId.Parse(id);
            List<Employee> employees = new List<Employee>();
            IMongoCollection<Employee> collection = DB.GetCollection<Employee>("employees");
            try
            {
                var filter = Builders<Employee>.Filter;
                employees = collection.Find(x => x.ID == objId).ToList();
                if (employees.Count <= 0)
                {
                    throw new Exception($"Error, couldn't find any documents with guid ${objId}.");
                }

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error on listing employees");
                System.Console.WriteLine(e);
            }
            return employees;
        }

        [HttpGet("{name}")]
        public IEnumerable<Employee> GetName(string name)
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

        [HttpDelete("id")]
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
