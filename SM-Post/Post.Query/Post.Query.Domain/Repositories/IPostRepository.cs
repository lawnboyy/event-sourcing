using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories;
public interface IPostRepository
{
  Task CreateAsync(PostEntity post);
  Task UpdateAsync(PostEntity post);
  Task DeleteAsync(Guid postId);
  Task<PostEntity> GetByIdAsync(Guid postId);
  Task<List<PostEntity>> GetAllAsync();
  Task<List<PostEntity>> GetByAuthorAsync(string author);
  Task<List<PostEntity>> GetWithLikesAsync(DateTime date);
  Task<List<PostEntity>> GetWithCommentsAsync(DateTime date);
}