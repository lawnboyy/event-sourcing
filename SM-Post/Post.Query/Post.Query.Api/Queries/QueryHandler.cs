using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries;

public class QueryHandler : IQueryHandler
{
  private readonly IPostRepository _postRepository;

  public QueryHandler(IPostRepository postRepository)
  {
    _postRepository = postRepository;
  }

  public async Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query)
  {
    return await _postRepository.GetAllAsync();
  }

  public async Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query)
  {
    var post = await _postRepository.GetByIdAsync(query.Id);
    return new List<PostEntity> { post };
  }

  public async Task<List<PostEntity>> HandleAsync(FindPostsByAuthorQuery query)
  {
    return await _postRepository.GetByAuthorAsync(query.Author);
  }

  public async Task<List<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query)
  {
    return await _postRepository.GetWithCommentsAsync();
  }

  public async Task<List<PostEntity>> HandleAsync(FindPostsWithLikesQuery query)
  {
    return await _postRepository.GetWithLikesAsync(query.NumberOfLikes);
  }
}