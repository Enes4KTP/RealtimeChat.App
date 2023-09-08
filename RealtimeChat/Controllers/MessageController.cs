using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RtChat.BusinessLayer.Services;
using RtChat.EntityLayer.Concrete;

namespace RealtimeChat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MessageServices _messageServices;

        public MessageController(MessageServices messageServices)
        {
            _messageServices = messageServices;
        }

        [HttpGet]
        public async Task<List<Messages>> Get() =>
            await _messageServices.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Messages>> Get(string id)
        {
            var message = await _messageServices.GetAsync(id);

            if (message is null)
            {
                return NotFound();
            }

            return message;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Messages newMessage)
        {
            await _messageServices.CreateAsync(newMessage);

            return CreatedAtAction(nameof(Get), new { id = newMessage.MessagesID }, newMessage);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Messages updatedMessage)
        {
            var message = await _messageServices.GetAsync(id);

            if (message is null)
            {
                return NotFound();
            }

            updatedMessage.MessagesID = message.MessagesID;

            await _messageServices.UpdateAsync(id, updatedMessage);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var message = await _messageServices.GetAsync(id);

            if (message is null)
            {
                return NotFound();
            }

            await _messageServices.RemoveAsync(id);

            return NoContent();
        }
    }
}
