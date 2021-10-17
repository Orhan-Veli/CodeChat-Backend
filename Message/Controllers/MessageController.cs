using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Message.Business.Concrete;
using Message.Business.Abstract;
using Message.Dal.Model;
using Serilog;
using Microsoft.AspNetCore.Cors;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
namespace Message.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class MessageController : ControllerBase
    {
        private readonly IElasticService _elasticService;
        private readonly IRabbitMqService _rabbit;
        public MessageController(IElasticService elasticService, IRabbitMqService rabbit)
        {
            _elasticService = elasticService;
            _rabbit = rabbit;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] MessageModel model, [FromHeader] string authorization)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(authorization))
            {
                return BadRequest("Model state is not valid");
            }
            if(AuthenticationHeaderValue.TryParse(authorization,out var headerVal))
            {
                var jwtToken = headerVal.Parameter;
                var handler = new JwtSecurityTokenHandler();
                var val = handler.ReadJwtToken(jwtToken);
                model.UserName = val.Claims.FirstOrDefault(x=> x.Type=="Name").Value;
                model.UserId = Guid.Parse(val.Claims.FirstOrDefault(c=>c.Type== "id").Value);                
            }
            var result = await _elasticService.CreateAsync(model);
            await _rabbit.Consumer(model);
            await _rabbit.Reciever();
            return StatusCode((int)result.Response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var result = await _elasticService.GetAsync(id);
            return StatusCode((int)result.Response,new { result.Message,result.Data });
        }

        [HttpGet("getall/{id}")]
        public async Task<IActionResult> GetAllAsync(Guid id)
        {
            var result = await _elasticService.GetAllAsync(id);
            return StatusCode((int)result.Response,new {result.Message,result.Data });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] MessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model state is not valid");
            }
            var result = await _elasticService.UpdateAsync(model);
            return StatusCode((int)result.Response,result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _elasticService.DeleteAsync(id);
            return StatusCode((int)result.Response);
        }

    }
}


