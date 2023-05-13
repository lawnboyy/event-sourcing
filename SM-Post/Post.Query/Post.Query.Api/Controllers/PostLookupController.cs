using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.Dtos;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PostLookupController : ControllerBase
{
  private readonly ILogger<PostLookupController> _logger;
  private readonly IQueryDispatcher<PostEntity> _queryDispatcher;

  public PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
  {
    _logger = logger;
    _queryDispatcher = queryDispatcher;
  }

  [HttpGet]
  public async Task<ActionResult> GetAllPostsAsync()
  {
    try
    {
      var posts = await _queryDispatcher.SendAsync(new FindAllPostsQuery());
      return GetNormalResponse(posts);
    }
    catch (Exception ex)
    {
      return GetExceptionResponse(ex, "An error occurred while attempting to retrieve all posts.");
    }
  }

  [HttpGet("byId/{postId}")]
  public async Task<ActionResult> GetPostByIdAsync(Guid postId)
  {
    try
    {
      var posts = await _queryDispatcher.SendAsync(new FindPostByIdQuery() { Id = postId });
      return GetNormalResponse(posts);
    }
    catch (Exception ex)
    {
      return GetExceptionResponse(ex, "An error occurred while attempting to retrieve a post by ID.");
    }
  }

  [HttpGet("byAuthor/{authorId}")]
  public async Task<ActionResult> GetPostsByAuthorAsync(string author)
  {
    try
    {
      var posts = await _queryDispatcher.SendAsync(new FindPostsByAuthorQuery() { Author = author });
      return GetNormalResponse(posts);
    }
    catch (Exception ex)
    {
      return GetExceptionResponse(ex, "An error occurred while attempting to retrieve posts by author.");
    }
  }

  [HttpGet("withComments")]
  public async Task<ActionResult> GetPostsWithCommentsAsync()
  {
    try
    {
      var posts = await _queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());
      return GetNormalResponse(posts);
    }
    catch (Exception ex)
    {
      return GetExceptionResponse(ex, "An error occurred while attempting to retrieve posts with comments.");
    }
  }

  [HttpGet("withLikes/{numberOfLikes}")]
  public async Task<ActionResult> GetPostsWithLikesAsync(int numberOfLikes)
  {
    try
    {
      var posts = await _queryDispatcher.SendAsync(new FindPostsWithLikesQuery() { NumberOfLikes = numberOfLikes });
      return GetNormalResponse(posts);
    }
    catch (Exception ex)
    {
      return GetExceptionResponse(ex, "An error occurred while attempting to retrieve posts with likes.");
    }
  }

  private ActionResult GetExceptionResponse(Exception ex, string safeErrorMessage)
  {
    _logger.LogError(ex, safeErrorMessage);
    return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = safeErrorMessage });
  }

  private ActionResult GetNormalResponse(List<PostEntity> posts)
  {    
    if (posts == null || !posts.Any())
      return NotFound();

    var count = posts.Count;

    return Ok(new PostLookupResponse
    {
      Message = $"Successfully found {count} post{(count > 1 ? "s" : string.Empty)}.",
      Posts = posts
    });
  }
}