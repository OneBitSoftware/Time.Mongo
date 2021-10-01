using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Time.Mongo.Web.Models;

namespace Time.Mongo.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            SetDateTimeInfoInViewBag();
            await GetDateTimeData();
            return View();
        }

        private void SetDateTimeInfoInViewBag()
        {
            this.ViewBag.dateTimeUtcNow = DateTime.UtcNow;
            this.ViewBag.dateTimeNow = DateTime.Now;
            this.ViewBag.dateTimeOffsetUtcNow = DateTimeOffset.UtcNow;
            this.ViewBag.dateTimeOffsetNow = DateTimeOffset.Now;
            this.ViewBag.timeZonesLocal = TimeZoneInfo.Local;
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime razorHiddenDateTimeNow, DateTime razorHiddenDateTimeUtcNow)
        {
            var clientDate = this.Request.Form["dateFromClientHidden"];
            this.ViewBag.dateFromClient = clientDate.ToString();

            this.ViewBag.razorHiddenDateTimeNow = razorHiddenDateTimeNow;
            this.ViewBag.razorHiddenDateTimeUtcNow = razorHiddenDateTimeUtcNow;

            SetDateTimeInfoInViewBag();

            var collection = GetCollection();

            await collection.InsertOneAsync(new DateTimeEntry()
            {
                DateFromClientJavaScript = clientDate.ToString(),
                DateTimeNowRazor = razorHiddenDateTimeNow,
                DateTimeUtcNowRazor = razorHiddenDateTimeUtcNow,
                ServerDateTimeNow = this.ViewBag.dateTimeNow,
                ServerDateTimeUtcNow = this.ViewBag.dateTimeUtcNow
            });

            await GetDateTimeData(collection);

            return View();
        }

        private static IMongoCollection<DateTimeEntry> GetCollection()
        {
            var client = new MongoClient();
            var database = client.GetDatabase("DateTimeHacks");
            var collection = database.GetCollection<DateTimeEntry>("DateTimeEntries");
            return collection;
        }

        private async Task GetDateTimeData(IMongoCollection<DateTimeEntry> collection = null)
        {
            if (collection is null)
            {
                collection = GetCollection();
            }
            
            var results = await collection.Find(FilterDefinition<DateTimeEntry>.Empty).ToListAsync();
            this.ViewBag.Results = results;

            var command = new BsonDocument { { "hostInfo", 1 } };
            var commandResult = collection.Database.RunCommand<BsonDocument>(command);
            this.ViewBag.ServerTime = commandResult["system"].AsBsonDocument["currentTime"].ToString();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
