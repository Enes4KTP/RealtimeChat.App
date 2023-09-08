using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RtChat.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtChat.BusinessLayer.Services
{
	public class MessageServices
	{
		private readonly IMongoCollection<Messages> _messageCollection;

		public static MessageServices? Instance { get; private set; }

		public MessageServices(IOptions<MongoDBSettings> options)
		{
			var mongoClient = new MongoClient(options.Value.ConnectionURI);
			var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
			_messageCollection = mongoDatabase.GetCollection<Messages>(options.Value.CollectionName);

			Instance = this;
		}

		public async Task<List<Messages>> GetAsync() =>
			await _messageCollection.Find(x => true).ToListAsync();

		public async Task<Messages?> GetAsync(string id) =>
			await _messageCollection.Find(x => x.MessagesID == id).FirstOrDefaultAsync();

		public async Task CreateAsync(Messages newMessage) =>
			await _messageCollection.InsertOneAsync(newMessage);

		public async Task UpdateAsync(string id, Messages updateMessage) =>
			 await _messageCollection.ReplaceOneAsync(x => x.MessagesID == id, updateMessage);

		public async Task RemoveAsync(string id) =>
			 await _messageCollection.DeleteOneAsync(x => x.MessagesID == id);


		public async Task<List<Messages>> GetMessagesForUserAsync(string userId, string to)
		{
			var filter = Builders<Messages>.Filter.Or(
				Builders<Messages>.Filter.And(
					Builders<Messages>.Filter.Eq(x => x.To, userId),
					Builders<Messages>.Filter.Eq(x => x.From, to)),
				Builders<Messages>.Filter.And(
					Builders<Messages>.Filter.Eq(x => x.To, to),
					Builders<Messages>.Filter.Eq(x => x.From, userId))
			);

			var messages = await _messageCollection.Find(filter).ToListAsync();

			return messages.OrderBy(m => m.Time).ToList();
		}

		public async Task<List<Messages>> GetMessagesByRecipientAsync(string recipientUserId)
		{
			var filter = Builders<Messages>.Filter.Or(
				  Builders<Messages>.Filter.Eq(x => x.To, recipientUserId),
				  Builders<Messages>.Filter.Eq(x => x.From, recipientUserId)
				);

			var messages = await _messageCollection.Find(filter).ToListAsync();
			return messages;
		}

	}
}
