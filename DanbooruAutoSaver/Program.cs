using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DanbooruAutoSaver.Models;
using DanbooruAutoSaver.Services;
using Microsoft.Extensions.Configuration;

namespace DanbooruAutoSaver
{
    internal static class Program
    {
        private static IConfiguration _configuration;
        private static SaverConfiguration _saverConfiguration;

        private static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);
            builder.AddJsonFile("appsettings.json", false);
            builder.AddJsonFile("appsettings.Production.json", false);
            _configuration = builder.Build();
            _saverConfiguration = _configuration.GetSection("Saver").Get<SaverConfiguration>();

            var posts = await LoadPosts();
            var onlyNew = await FilterExistingPosts(posts);
            await SaveFilteredPosts(onlyNew);
            Console.WriteLine("Completed!");
            //Console.ReadKey();
        }

        private static async Task SaveFilteredPosts(IReadOnlyCollection<Post> onlyNew)
        {
            Console.WriteLine($"Saving {onlyNew.Count} posts...");

            var saver = new PostSaver(GetHttpClient());

            await saver.SavePosts(onlyNew, _saverConfiguration.SaveToPath);
        }

        private static async Task<IReadOnlyCollection<Post>> FilterExistingPosts(List<Post> posts)
        {
            if (!_saverConfiguration.CheckInternal)
                return posts;

            Console.WriteLine("Filtering already saved posts...");
            var checker = new RoomSavedChecker(GetHttpClient());
            var onlyNew = await checker.GetOnlyNewPosts(posts);
            var counter = 1;
            foreach (var post in onlyNew)
            {
                Console.WriteLine(counter++ + " " + post.FileUrl);
            }

            return onlyNew;
        }

        private static async Task<List<Post>> LoadPosts()
        {
            Console.WriteLine("Loading fav posts...");
            var counter = 1;
            var posts = new List<Post>();

            var loader = new DanbooruFavoritesLoader(
                GetHttpClient(),
                _configuration.GetSection("Danbooru").Get<BooruConfiguration>());

            await foreach (var favoritePost in loader.GetFavoritesUrls())
            {
                Console.WriteLine(counter++ + " D " + favoritePost.FileUrl);
                posts.Add(favoritePost);
            }

            var yandereLoader = new YandereFavoritesLoader(
                GetHttpClient(),
                _configuration.GetSection("Yandere").Get<BooruConfiguration>());

            await foreach (var favoritePost in yandereLoader.GetFavoritesUrls())
            {
                Console.WriteLine(counter++ + " Y " + favoritePost.FileUrl);
                posts.Add(favoritePost);
            }

            posts.Reverse();
            return posts;
        }

        private static HttpClient GetHttpClient()
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(5)
            };

            return client;
        }
    }
}
