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

        #region Employee CRUD
        //RETRIEVE
        [HttpGet]
        public JArray Get()
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

            return JArray.Parse(employees.ToJson());
        }

        [HttpGet("id/{id}")]
        public JArray Get(string id)
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
            return JArray.Parse(employees.ToJson());
        }

        [HttpGet("{name}")]
        public JArray GetByName(string name)
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
            return JArray.Parse(employees.ToJson());
        }

        //CREATE
        [HttpPost]
        public IActionResult Post([FromBody] JObject employeeJson)
        {
            var collection = DB.GetCollection<Employee>("employees");

            //deserializes JSON received from the DB naming convention
            var employee = BsonSerializer.Deserialize<Employee>(employeeJson.ToString());

            //checks wether data is ok
            if (employee.Name == null || employee.WeekHours == 0)
            {
                System.Console.WriteLine("Error, cannot insert employee without required parameters");
                return RedirectToAction("Get");
            }
            //Correct null properties
            if (employee.DateJoined == null)
            {
                employee.DateJoined = DateTime.Now;
            }
            if (employee.Entries == null)
            {
                employee.Entries = new List<HourEntry>();
            }

            //inserts document
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

        //UPDATE
        [HttpPut]
        public IActionResult Update([FromBody] JObject employeeJson)
        {
            var collection = DB.GetCollection<Employee>("employees");
            try
            {
                var employee = BsonSerializer.Deserialize<Employee>(employeeJson.ToString());
                collection.ReplaceOne(x => x.ID == employee.ID, employee);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error, could not update Employee in the database.");
                System.Console.WriteLine(e);
                throw new Exception();
            }
            return RedirectToAction("Get", new { id = employeeJson.Property("_id").Value.ToString() });
        }

        //DELETE
        [HttpDelete("id/{id}")]
        public IActionResult Delete(string id)
        {
            IMongoCollection<Employee> collection = DB.GetCollection<Employee>("employees");
            try
            {
                collection.DeleteOne(x => x.ID == ObjectId.Parse(id));
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error on deleting employee");
                System.Console.WriteLine(e);
            }
            return RedirectToAction("Get");
        }

        #endregion

        #region Hour Entry CRUD
        //RETRIEVE
        [HttpGet("entries/{id?}")]
        public JArray GetEntries(string id)
        {
            // var entries = new List<Employee>();
            var collection = DB.GetCollection<Employee>("employees");
            var entries = new BsonDocument();

            try
            {
                if (id == null)
                {
                    entries = collection.Aggregate()
                                .Unwind("hours")
                                .Group("{_id: '', hours: {$addToSet: {_id: '$hours._id'," +
                                                "employee_id: '$hours.employee_id'," +
                                                "employee: '$name'," +
                                                "date: '$hours.date'," +
                                                "reason: '$hours.reason'," +
                                                "amount: '$hours.amount' }}}")
                                .Project("{_id: 0, hours: 1}")
                                .Sort(Builders<BsonDocument>.Sort.Ascending("hours.date"))
                                .First();
                }
                else
                {
                    entries = collection.Aggregate()
                                .Match(x => x.ID == ObjectId.Parse(id))
                                .Unwind("hours")
                                .Group("{_id: '', hours: {$addToSet: {_id: '$hours._id'," +
                                                "employee_id: '$hours.employee_id'," +
                                                "employee: '$name'," +
                                                "date: '$hours.date'," +
                                                "reason: '$hours.reason'," +
                                                "amount: '$hours.amount' }}}")
                                .Project("{_id: 0, hours: 1}")
                                .Sort(Builders<BsonDocument>.Sort.Ascending("hours.date"))
                                .First();
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error on listing hour entries");
                System.Console.WriteLine(e);
            }

            return JArray.Parse(entries.GetValue("hours").ToJson());
        }

        [HttpGet("entries/from/{employeeName}")]
        public IActionResult GetEntriesByEmployeeName(string employeeName)
        {
            var collection = DB.GetCollection<Employee>("employees");

            try
            {
                var employee = collection.Find<Employee>(e => e.Name == employeeName).First();
                return RedirectToAction("GetEntries", new { id = employee.ID.ToString() });
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error on listing hour entries");
                System.Console.WriteLine(e);
            }

            return RedirectToAction("Get");
        }

        //CREATE
        [HttpPost("entries/add")]
        public IActionResult PostEntry([FromBody] JObject hourJson)
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
                System.Console.WriteLine("Hour Entry: " + hourEntry.ToString());
                if (hourEntry.EmployeeName == null)
                {
                    hourEntry.EmployeeName = collection
                                                .Find(e => e.ID == employeeObjId)
                                                .First().Name;
                }
                if (hourEntry.ID == null || hourEntry.ID == ObjectId.Empty)
                {
                    hourEntry.ID = ObjectId.GenerateNewId(DateTime.UtcNow);
                }
                if (hourEntry.DateRegistered == null)
                {
                    hourEntry.DateRegistered = DateTime.Now;
                }
                var employee = collection.FindOneAndUpdate<Employee>(e => e.ID == employeeObjId,
                                Builders<Employee>.Update
                                .AddToSet(e => e.Entries, hourEntry)
                                .Inc(e => e.HourBank, hourEntry.Amount));
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

            return RedirectToAction("Get", new { id = hourJson.Property("employee_id").Value.ToString() });
        }

        //UPDATE
        [HttpPut("entries/update")]
        public IActionResult UpdateEntry([FromBody]JObject entryJson)
        {
            var collection = DB.GetCollection<Employee>("employees");
            var entry = BsonSerializer.Deserialize<HourEntry>(entryJson.ToString());
            try
            {
                var hour = collection.Find<Employee>(Builders<Employee>.Filter.ElemMatch(e => e.Entries,
                                                        h => h.ID == entry.ID)).ToList().First()
                                                    .Entries.Find(h => h.ID == entry.ID);
                var diff = entry.Amount - hour.Amount;
                collection.FindOneAndUpdate<Employee>(Builders<Employee>.Filter.ElemMatch(e => e.Entries, h => h.ID == entry.ID),
                                                    Builders<Employee>.Update.Set(e => e.Entries[-1], entry)
                                                        .Inc(e => e.HourBank, diff));
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error, could not update the entry");
                System.Console.WriteLine(e);

                return RedirectToAction("Get");
            }
            return RedirectToAction("Get", new { id = entryJson.Property("employee_id").Value.ToString() });
        }

        //DELETE
        [HttpDelete("entries")]
        public IActionResult DeleteEntry([FromBody]JObject entryJson)
        {
            var entryId = entryJson.Property("_id").Value.ToString();
            System.Console.WriteLine("Deleting entry " + entryId);
            var collection = DB.GetCollection<Employee>("employees");
            try
            {
                //Get the hour to delete
                var hourId = ObjectId.Parse(entryId);
                var hour = collection.Find<Employee>(Builders<Employee>.Filter.ElemMatch(e => e.Entries,
                                                        h => h.ID == hourId)).ToList().First()
                                                    .Entries.Find(h => h.ID == hourId);
                //Pull the hour from the array
                collection.FindOneAndUpdate<Employee>(Builders<Employee>.Filter.ElemMatch(e => e.Entries, h => h.ID == hourId),
                                                    Builders<Employee>.Update.PullFilter(e => e.Entries, h => h.ID == hourId)
                                                        .Inc(e => e.HourBank, -hour.Amount));
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error, could not delete the entry");
                System.Console.WriteLine(e);
            }
            return RedirectToAction("Get");
        }

        #endregion
    }
}
