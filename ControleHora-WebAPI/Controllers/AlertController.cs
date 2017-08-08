
using System;
using ControleHora_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace ControleHora_WebAPI.Controllers
{
    [Route("[controller]")]
    public class AlertController : Controller
    {

        public static string ConnectionString = "mongodb://localhost:27017";
        public static MongoClient Client = new MongoClient(ConnectionString);
        public static IMongoDatabase DB = Client.GetDatabase("controle_horas");
        
#region CRUD for alerts

        /// List all the entries
        [HttpGet("{adminId}")]
        public JArray Get(string adminId)
        {
            System.Console.WriteLine("GETTING SHIT");
            //Gets all alerts from all employees
            var collection = DB.GetCollection<Employee>("employees");
            try
            {
                var admin = collection.Find<Employee>(e => e.Name == "admin" && e.Email == "admin@hourcheck.com")
                                            .First();
                if(admin.ID == ObjectId.Parse(adminId))
                {
                    var alerts = collection.Aggregate()
                                            .Unwind("alerts")
                                            .ToList();
                    System.Console.WriteLine("ADMIN FOUND " + admin.ID);
                    return JArray.Parse(alerts.ToJson());
                }
            }
            catch(Exception e)
            {
                System.Console.WriteLine(e);
            }

            System.Console.WriteLine("ADMIN NOT FOUND");
            return null;
        }

        /// List the entries from a employee
        [HttpGet("{adminId}/{employeeId}")]
        public JArray Get(string adminId, string employeeId)
        {
            var collection = DB.GetCollection<Employee>("employees");
            try
            {
                var employee = collection.Find<Employee>(e => e.ID == ObjectId.Parse(employeeId)).First();
                var alerts = employee.Alerts;
                return JArray.Parse(alerts.ToString());
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error, could not find employee's alerts");
                System.Console.WriteLine(e);
            }
            return null;
        }

        [HttpPost("add")]
        public IActionResult Post(string senderId, string receiverId)
        {
            return RedirectToAction("Get");
        }

        [HttpDelete("remove")]
        public IActionResult Delete(string adminId, string alertId)
        {
            return RedirectToAction("Get");
        }
#endregion
    }
}