using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EditMessageController : ControllerBase
{
  private readonly ILogger<EditMessageController> _logger;
  private readonly ICommandDispatcher _commandDispatcher;

  public EditMessageController(ILogger<EditMessageController> logger, ICommandDispatcher commandDispatcher)
  {
    _logger = logger;
    _commandDispatcher = commandDispatcher;
  }

  [HttpPut("{id}")]
  public async Task<ActionResult> EditMessageAsync(Guid id, [FromBody] EditMessageCommand command)
  {
    try
    {
      command.Id = id;
      await _commandDispatcher.SendAsync(command);

      return Ok(new BaseResponse()
      {
        Message = "Message edited request completed successfully!",
      });
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogWarning(ex, "Client made a bad request");
      return BadRequest(new BaseResponse { Message = ex.Message });
    }
    catch (AggregateNotFoundException ex)
    {
      _logger.LogWarning(ex, "Could not retrieve aggregate; client passed an incorrect post ID targetting the aggregate.");
      return BadRequest(new BaseResponse { Message = ex.Message });
    }
    catch (Exception ex)
    {
      const string SAFE_ERROR_MESSAGE = "An error occurred while processing your request to edit post message!";
      _logger.LogError(ex, SAFE_ERROR_MESSAGE);
      return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse { Id = id, Message = SAFE_ERROR_MESSAGE });
    }
  }

}