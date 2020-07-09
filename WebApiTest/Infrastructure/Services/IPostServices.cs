using System.Collections.Generic;
using WebApiTest.DataAccess.Entities;
using WebApiTest.Infrastructure.Models;

namespace WebApiTest.Infrastructure.Services
{
    public interface IPostServices
    {
        Post GetPostById(long id, bool isIncludeTrans = false);
        PostModel GetPostModelById(long id, string languageCode);
        IEnumerable<PostModel> GetPosts(string languageCode);
        PostModel CreatePost(PostModel postModel);
        PostModel CreateTranslation(Post post, PostModel postModel);
        bool UpdatePost(long id, PostModel postModel);
        bool DeletePost(long id);
        bool DeleteTranslate(long id, string languageCode);
    }
}
