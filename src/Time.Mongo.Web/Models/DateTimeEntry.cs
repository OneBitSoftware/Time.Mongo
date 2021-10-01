using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Time.Mongo.Web.Models
{
    public class DateTimeEntry
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string DateFromClientJavaScript { get; set; }
        public DateTime DateTimeNowRazor { get; set; }
        public DateTime DateTimeUtcNowRazor { get; set; }
        public DateTime ServerDateTimeNow { get; set; }
        public DateTime ServerDateTimeUtcNow { get; set; }

        public DateTimeOffset ServerDateTimeOffsetNow { get; set; }
        public DateTimeOffset ServerDateTimeOffsetUtcNow { get; set; }

    }
}
