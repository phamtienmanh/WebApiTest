using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApiTest.Infrastructure.Enums;
using WebApiTest.Infrastructure.Models;
using WebApiTest.Infrastructure.Services;

namespace WebApiTest.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostServices _postServices;
        private readonly UserManager<IdentityUser> _userManager;

        public PostController(IPostServices postServices, UserManager<IdentityUser> userManager)
        {
            _postServices = postServices;
            _userManager = userManager;
        }

        [HttpGet("{id}", Name = "GetById")]
        public IActionResult Get(long id, string languageCode = LanguageCode.English)
        {
            var postModel = _postServices.GetPostModelById(id, languageCode);
            if (postModel == null)
            {
                return NotFound();
            }

            return Ok(postModel);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string languageCode = LanguageCode.English)
        {
            var user = await _userManager.GetUserAsync(User);
            var posts = _postServices.GetPosts(languageCode);
            return Ok(posts);
        }

        [HttpPost]
        public IActionResult Create([FromBody] PostModel postModel)
        {
            var createdPostModel = _postServices.CreatePost(postModel);

            return CreatedAtRoute("GetById",
                new {id = createdPostModel.Id, languageCode = createdPostModel.LanguageCode},
                createdPostModel);
        }

        [HttpPost("{id}/translation")]
        public IActionResult CreateTranslation(long id, [FromBody] PostModel postModel)
        {
            var languageCode = postModel.LanguageCode;
            var post = _postServices.GetPostById(id, true);
            if (post == null)
            {
                ModelState.AddModelError("error", "Post is not exist!");
                return BadRequest(ModelState);
            }
            if (post.PostTranslations.Any(x => x.LanguageCode == languageCode))
            {
                ModelState.AddModelError("error", $"Translation for {languageCode} is already exist!");
                return BadRequest(ModelState);
            }
            var createdPostModel = _postServices.CreateTranslation(post, postModel);
            return CreatedAtRoute("GetById",
                new { id, languageCode },
                createdPostModel);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] PostModel postModel)
        {
            if (!_postServices.UpdatePost(id, postModel))
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            if (!_postServices.DeletePost(id))
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}/translation")]
        public IActionResult DeleteTranslation(long id, string languageCode = LanguageCode.English)
        {
            if (!_postServices.DeleteTranslate(id, languageCode))
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
