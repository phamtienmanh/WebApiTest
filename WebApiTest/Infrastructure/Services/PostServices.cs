using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApiTest.DataAccess.Contexts;
using WebApiTest.DataAccess.Entities;
using WebApiTest.Infrastructure.Models;

namespace WebApiTest.Infrastructure.Services
{
    public class PostServices: IPostServices
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public PostServices(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        private bool Save()
        {
            return _dbContext.SaveChanges() >= 0;
        }

        private PostTranslation GetTranslationById(long id, string languageCode)
        {
            return _dbContext.PostTranslations.Include(x => x.Post)
                .FirstOrDefault(x => x.PostId == id && x.LanguageCode == languageCode);
        }

        private PostTranslation GetPublishedTranslationById(long id, string languageCode)
        {
            return _dbContext.PostTranslations.Include(x => x.Post)
                .FirstOrDefault(x => x.Post.Published && x.PostId == id && x.LanguageCode == languageCode);
        }

        public Post GetPostById(long id, bool isIncludeTrans = false)
        {
            if (isIncludeTrans) return _dbContext.Posts.Include(x => x.PostTranslations)
                .FirstOrDefault(x => x.Id == id);
            return _dbContext.Posts.FirstOrDefault(x => x.Id == id);
        }

        public PostModel GetPostModelById(long id, string languageCode)
        {
            var postModel = _mapper.Map<PostModel>(GetPublishedTranslationById(id, languageCode));
            return postModel;
        }

        public IEnumerable<PostModel> GetPosts(string languageCode)
        {
            var posts = _dbContext.PostTranslations.Include(x => x.Post)
                .Where(x => x.Post.Published && x.LanguageCode == languageCode);
            return _mapper.Map<IEnumerable<PostModel>>(posts);
        }

        public PostModel CreatePost(PostModel postModel)
        {
            var newPost = _mapper.Map<Post>(postModel);
            newPost.CreatedDate = DateTime.Now;
            _dbContext.Posts.Add(newPost);
            Save();
            return _mapper.Map<PostModel>(newPost.PostTranslations.FirstOrDefault());
        }

        public PostModel CreateTranslation(Post post, PostModel postModel)
        {
            var createdTrans = _mapper.Map<PostTranslation>(postModel);
            post.PostTranslations.Add(createdTrans);
            Save();
            return _mapper.Map<PostModel>(createdTrans);
        }

        public bool UpdatePost(long id, PostModel postModel)
        {
            var updatePost = GetTranslationById(id, postModel.LanguageCode);
            if (updatePost is null)
            {
                return false;
            }
            _mapper.Map(postModel, updatePost);
            updatePost.Post.UpdatedDate = DateTime.Now;
            return Save();
        }

        public bool DeletePost(long id)
        {
            var post = GetPostById(id);
            if (post == null)
            {
                return false;
            }
            _dbContext.Posts.Remove(post);
            return Save();
        }

        public bool DeleteTranslate(long id, string languageCode)
        {
            var post = GetPostById(id, true);
            if (post == null || post.PostTranslations.All(x => x.LanguageCode != languageCode))
            {
                return false;
            }
            if (post.PostTranslations.Count <= 1)
            {
                _dbContext.Posts.Remove(post);
            }
            else
            {
                var trans = post.PostTranslations.FirstOrDefault(x => x.LanguageCode == languageCode);
                post.PostTranslations.Remove(trans);
            }
            return Save();
        }
    }
}
