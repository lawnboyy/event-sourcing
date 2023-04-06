namespace Post.Cmd.Infrastructure.Config;

public class MongoDbConfig
{
  public string ConnectionString { get; set; } = string.Empty;
  public string Database { get; set; } = string.Empty;
  public string Collection { get; set; } = string.Empty;
}