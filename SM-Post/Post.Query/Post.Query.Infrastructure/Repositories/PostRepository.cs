using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
  private readonly DatabaseContextFactory _contextFactory;

  public PostRepository(DatabaseContextFactory contextFactory)
  {
    _contextFactory = contextFactory;
  }

  public async Task CreateAsync(PostEntity post)
  {
    using DatabaseContext context = _contextFactory.CreateDbContext();
    context.Posts.Add(post);
    
    _ = await context.SaveChangesAsync();
  }

  public async Task DeleteAsync(Guid postId)
  {
    using DatabaseContext context = _contextFactory.CreateDbContext();
    var post = await GetByIdAsync(postId);

    if (post == null)
    {
      return;
    }

    context.Posts.Remove(post);
    _ = await context.SaveChangesAsync();
  }

  public async Task<List<PostEntity>> GetAllAsync()
  {
    using DatabaseContext context = _contextFactory.CreateDbContext();
    // Use AsNoTracking() for performance in read-only scenarios.
    return await context.Posts.AsNoTracking().Include(p => p.Comments).ToListAsync();
  }

  public async Task<List<PostEntity>> GetByAuthorAsync(string author)
  {
    using DatabaseContext context = _contextFactory.CreateDbContext();
    // Use AsNoTracking() for performance in read-only scenarios.
    return await context.Posts.AsNoTracking().Include(p => p.Comments).Where(p => p.Author.Contains(author)).ToListAsync();
  }

  public async Task<PostEntity> GetByIdAsync(Guid postId)
  {
    using DatabaseContext context = _contextFactory.CreateDbContext();
    var post = await context.Posts
      .Include(p => p.Comments)
      .FirstOrDefaultAsync(p => p.PostId == postId);

    if (post == null)
    {
      // TODO: Use custom exception
      throw new Exception ("Post not found");
    }

    return post;
  }

  public async Task<List<PostEntity>> GetWithCommentsAsync()
  {
    using DatabaseContext context = _contextFactory.CreateDbContext();
    // Use AsNoTracking() for performance in read-only scenarios.
    return await context.Posts.AsNoTracking().Include(p => p.Comments).Where(p => p.Comments.Any()).ToListAsync();
  }

  public async Task<List<PostEntity>> GetWithLikesAsync(int numberOfLikes)
  {
    using DatabaseContext context = _contextFactory.CreateDbContext();
    // Use AsNoTracking() for performance in read-only scenarios.
    return await context.Posts.AsNoTracking().Include(p => p.Comments).Where(p => p.Likes >= numberOfLikes).ToListAsync();
  }

  public async Task UpdateAsync(PostEntity post)
  {
    using DatabaseContext context = _contextFactory.CreateDbContext();
    context.Posts.Update(post);

    _ = await context.SaveChangesAsync();
  }
}