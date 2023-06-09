using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LikePostController : ControllerBase
{
  private readonly ILogger<LikePostController> _logger;
  private readonly ICommandDispatcher _commandDispatcher;

  public LikePostController(ILogger<LikePostController> logger, ICommandDispatcher commandDispatcher)
  {
    _logger = logger;
    _commandDispatcher = commandDispatcher;
  }

  [HttpPut("{id}")]
  public async Task<ActionResult> LikePostAsync(Guid id)
  {
    try
    {
      await _commandDispatcher.SendAsync(new LikePostCommand { Id = id });

      return Ok(new BaseResponse()
      {
        Message = "Like post request completed successfully!",
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
      const string SAFE_ERROR_MESSAGE = "An error occurred while processing your request to like post!";
      _logger.LogError(ex, SAFE_ERROR_MESSAGE);
      return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse { Id = id, Message = SAFE_ERROR_MESSAGE });
    }
  }
}