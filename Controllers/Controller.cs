using Microsoft.AspNetCore.Mvc;
using Mosaic2.Models;
using Mosaic2.Services;

namespace Mosaic2.Controllers
{
    [ApiController]
    [Route("orders")]
    public class Controller : ControllerBase
    {
        IRepository _repository;

        public Controller(IRepository repository)
        {
            //if the repository doesn't exist in the dependancy injection
            //container throw exception
            _repository = repository 
                ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("all")]
        public IActionResult GetAllOrders()
        {
            var orders = _repository.GetAllOrders();
            if (orders.Count < 1)
            {
                return NotFound();
            }

            return Ok(orders);
        }

        [HttpGet("schedule")]
        public IActionResult GetSchedule()
        {
            var schedule = _repository.GetSchedule();
            if (schedule.Count < 1)
            {
                return NotFound();
            }

            //sort the list based on baker name that is assigned the order
            schedule.Sort((s1, s2) => string.Compare(s1.bakerName,s2.bakerName));

            return Ok(schedule);
        }

        [HttpGet("bakers")]
        public IActionResult GetBakers()
        {
            return Ok(_repository.GetBakers());
        }

        [HttpPost]
        public IActionResult AddOrder(OrderModel order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            _repository.CreateOrder(order);
            return Ok();
        }

        [HttpDelete]
        public IActionResult RemoveOrder(Guid orderId)
        {
            _repository.RemoveOrder(orderId);
            return Ok();
        }
    }
}
