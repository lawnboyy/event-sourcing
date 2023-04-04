using CQRS.Core.Messages;

namespace CQRS.Core.Commands;

public class RemoveCommentCommand : BaseCommand
{
  public Guid CommentId { get; set; }
  public string Username { get; set; } = "";
}