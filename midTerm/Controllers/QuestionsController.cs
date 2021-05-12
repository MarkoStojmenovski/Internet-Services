using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using midTerm.Models.Models.Question;
using midTerm.Services.Abstractions;

namespace midTerm.Controllers
{
    /// <summary>
    /// Question API Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _service;

        /// <summary>
        /// Quesiton Constructor API Controller
        /// </summary>
        /// <param name="service">Question service</param>
        public QuestionsController(IQuestionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get Items
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Questions
        ///
        /// </remarks>
        /// <returns>A list of Base Question items</returns>
        /// <response code="200">All went well</response>
        /// <response code="500">Server side error</response>
        [HttpGet (Name = nameof(Get))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<QuestionModelBase>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var result = await _service.Get();
            return Ok(result);
        }

        /// <summary>
        /// Get question by id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Question/{id}
        ///
        /// </remarks>
        /// <param name="id">identifier of the item</param>
        /// <returns>An Extended Question model item</returns>
        /// <response code="200">All went well</response>
        /// <response code="500">Server side error</response> 
        [HttpGet("{id}", Name = nameof(GetById))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QuestionModelExtended))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);
            return Ok(result);
        }

        /// <summary>
        /// Create Item
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Question
        ///     {
        ///         "Text":"A question",     
        ///         "Description": "An interesting question"
        ///     }
        /// 
        /// </remarks>
        /// <param name="model">model to create</param>
        /// <returns>created item</returns>
        /// <response code="201">Returns the the created item</response>
        /// <response code="400">If the item is null</response>
        /// <response code="405">Method not allowed</response>    
        /// <response code="409">If the item is not created</response>   
        /// <response code="500">server side error</response> 
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] QuestionCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var question = await _service.Insert(model);
                return question != null
                    ? (IActionResult)CreatedAtRoute(nameof(GetById), question, question.Id)
                    : Conflict();
            }
            return BadRequest();
        }

        /// <summary>
        /// Update Item
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Question/{id}
        ///     {
        ///         "id": 0,
        ///         "Text":"A question",     
        ///         "Description": "An interesting question"
        ///     }
        /// 
        /// </remarks>
        /// <param name="id">identifier of the item</param>
        /// <param name="model">model to update</param>
        /// <returns>updated item</returns>
        /// <response code="201">Returns the the updated item</response>
        /// <response code="400">If the item is null</response>
        /// <response code="405">Method not allowed</response>    
        /// <response code="409">If the item is not created</response>   
        /// <response code="500">server side error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QuestionModelBase))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromBody] QuestionUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = id;
                var result = await _service.Update(model);

                return result != null
                    ? (IActionResult)Ok(result)
                    : NoContent();
            }
            return BadRequest();
        }

        /// <summary>
        /// Deletes an Item
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/Question/{id}
        /// 
        /// </remarks>
        /// <param name="id">identifier of the item</param>
        /// <returns>updated item</returns>
        /// <response code="200">true if deleted</response>
        /// <response code="400">If the item is not deleted</response>   
        /// <response code="405">Method not allowed</response>    
        /// <response code="500">server side error</response> 
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _service.Delete(id));
            }
            return BadRequest();
        }
    }
}
