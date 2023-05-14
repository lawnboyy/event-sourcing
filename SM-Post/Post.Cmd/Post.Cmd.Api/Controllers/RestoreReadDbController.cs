using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RestoreReadDbController : ControllerBase
{ 
  private readonly ILogger<RestoreReadDbController> _logger;
  private readonly ICommandDispatcher _commandDispatcher;

  public RestoreReadDbController(ILogger<RestoreReadDbController> logger, ICommandDispatcher commandDispatcher)
  {
    _logger = logger;
    _commandDispatcher = commandDispatcher;
  }

  [HttpPost]
  public async Task<ActionResult> RestoreReadDbAsync()
  {
    try
    {      
      await _commandDispatcher.SendAsync(new RestoreReadDbCommand());

      return StatusCode(StatusCodes.Status201Created, new BaseResponse { Message = "Read database restore request completed successfully!" });
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogWarning(ex, "Client made a bad request");
      return BadRequest(new BaseResponse { Message = ex.Message });
    }
    catch (Exception ex)
    {
      const string SAFE_ERROR_MESSAGE = "An error occurred while processing your request to restore read database!";
      _logger.LogError(ex, SAFE_ERROR_MESSAGE);
      return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
    }
  }
}