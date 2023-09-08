using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RtChat.EntityLayer.Concrete;

namespace RtChat.EntityLayer.Concrete
{
	[BsonIgnoreExtraElements]
	public class Messages
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string MessagesID { get; set; } = String.Empty;

		[BsonElement("from")]
		public string From { get; set; } = String.Empty;

		[BsonElement("to")]
		public string To { get; set; } = String.Empty;

		[BsonElement("message")]
		public string Message { get; set; } = String.Empty;

		[BsonElement("time")]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime Time { get; set; } = DateTime.UtcNow;

		[BsonElement("read")]
		public bool IsRead { get; set; }

		[BsonElement("user")]
		public User? User { get; set; } 
	}

}
