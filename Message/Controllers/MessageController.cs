using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Message.Business.Concrete;
using Message.Business.Abstract;
using Message.Dal.Model;

namespace Message.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IElasticService _elasticService;
        public MessageController(IElasticService elasticService)
        {
            _elasticService = elasticService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model state is not valid");
            }
            var result = await _elasticService.Create(model);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _elasticService.Get(id);
            if (!result.Success || result.Data == null)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _elasticService.GetAll();
            if (!result.Success || !result.Data.Any())
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] MessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model state is not valid");
            }
            var result = await _elasticService.Update(model);
            if (!result.Success || result.Data == null)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _elasticService.Delete(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return NoContent();
        }


    }
}


