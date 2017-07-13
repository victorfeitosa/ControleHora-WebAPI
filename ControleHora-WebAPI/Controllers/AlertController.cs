
using ControleHora_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace ControleHora_WebAPI.Controllers
{
    [Route("[controller]")]
    class AlertController : Controller
    {

        public static string ConnectionString = "mongodb://localhost:27017";
        public static MongoClient Client = new MongoClient(ConnectionString);
        public static IMongoDatabase DB = Client.GetDatabase("controle_horas");
        
#region CRUD for alerts

        /// List all the entries
        [HttpGet("{adminId}")]
        public JArray Get(string adminId)
        {
            var alerts = DB.GetCollection<Alert>("employee_alerts");
            return JArray.Parse(alerts.ToString());
        }

        /// List the entries from a employee
        [HttpGet("{adminId}/{employeeId}")]
        public JArray Get(string adminId, string employeeId)
        {
            var collection = DB.GetCollection<Alert>("employee_alerts");
            try
            {
                var alerts = collection.Find<Alert>(a => a.EmployeeId.ToString() == employeeId);
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