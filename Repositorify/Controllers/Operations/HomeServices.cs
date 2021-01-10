using Repositorify.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using static Repositorify.Models.ViewModels;

namespace Repositorify.Controllers.Operations
{
    public class HomeServices
    {
        public List<TagViewModel> GetTagViewModels()
        {
            using (RepositorifyEntities context = new RepositorifyEntities())
            {
                var tags = (from t in context.Tags
                            select new TagViewModel
                            {
                                Id = t.Id
                            }).ToList();
                return tags;
            }
        }

        private List<string> ConvertTagStringToList(string tags)
        {
            if (string.IsNullOrEmpty(tags))
            {
                return null;
            }

            var tagsList = tags.ToLower().Split(new string[] { ", " }, StringSplitOptions.None).Distinct().ToList();
            return tagsList;
        }

        public void CreateNewTags(List<string> tags)
        {
            using (RepositorifyEntities context = new RepositorifyEntities())
            {
                var existingTags = context.Tags.Select(t => t.Id).ToList();
                var newTags = tags.Where(t => !existingTags.Contains(t)).ToList();
                var listNewTags = (from t in newTags
                                   select new Tag
                                   {
                                       Id = t,
                                       DateCreated = DateTime.Now
                                   }).ToList();
                context.Tags.AddRange(listNewTags);
                context.SaveChanges();
            }
        }

        private string ConvertStreamToBase64(Stream stream)
        {
            byte[] data;
            using (Stream inputStream = stream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
                var b64 = Convert.ToBase64String(data);

                return b64;
            }
        }

        public void CreateImage(string data, List<string> tags)
        {
            using (RepositorifyEntities context = new RepositorifyEntities())
            {
                var image = new Image()
                {
                    Id = 0,
                    ImageData = data,
                    DateUploaded = DateTime.Now,
                    Uploader = HttpContext.Current.Request.UserHostAddress
                };
                context.Images.Add(image);
                context.SaveChanges();

                var tagItems = (from t in tags
                                select new ImageTag
                                {
                                    Id = 0,
                                    ImageId = image.Id,
                                    TagId = t
                                }).ToList();
                context.ImageTags.AddRange(tagItems);
                context.SaveChanges();
            }
        }

        public bool UploadImage(HttpPostedFileBase file, string tags)
        {
            try
            {
                var tagsList = ConvertTagStringToList(tags);

                // Save new tags
                CreateNewTags(tagsList);

                // Save image in database
                var base64 = ConvertStreamToBase64(file.InputStream);
                CreateImage(base64, tagsList);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public List<Image> GetImages(string tag)
        {
            using (RepositorifyEntities context = new RepositorifyEntities())
            {
                var contextTag = context.Tags.Find(tag);
                var images = context.ImageTags.Where(i => i.TagId.Equals(tag)).Select(i => i.Image).ToList();
                return images;
            }
        }
    }
}