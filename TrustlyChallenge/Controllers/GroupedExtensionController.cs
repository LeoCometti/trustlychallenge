using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrustlyChallenge.Data;
using TrustlyChallenge.Model;

namespace TrustlyChallenge.Controllers
{
    [Route("api/GroupedExtension")]
    [ApiController]
    public class GroupedExtensionController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<GroupedExtension>>> Get([FromServices] DataContext context)
        {
            var groupedExtensions = await context.GroupedFiles.ToListAsync();
            return groupedExtensions;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<GroupedExtension>> Post(
            [FromServices] DataContext context,
            [FromBody] GroupedExtension model)
        {
            if (ModelState.IsValid)
            {
                context.GroupedFiles.Add(model);
                await context.SaveChangesAsync();
                return model;
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
