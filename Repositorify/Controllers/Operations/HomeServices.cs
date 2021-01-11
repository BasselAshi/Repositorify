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

        //private string ConvertStreamToBase64(Stream stream)
        //{
        //    byte[] data;
        //    using (Stream inputStream = stream)
        //    {
        //        MemoryStream memoryStream = inputStream as MemoryStream;
        //        if (memoryStream == null)
        //        {
        //            memoryStream = new MemoryStream();
        //            inputStream.CopyTo(memoryStream);
        //        }
        //        data = memoryStream.ToArray();
        //        var b64 = Convert.ToBase64String(data);

        //        return b64;
        //    }
        //}

        public string CreateImage(List<string> tags, int sizeBytes, string extension)
        {
            using (RepositorifyEntities context = new RepositorifyEntities())
            {
                var image = new Image()
                {
                    Size = sizeBytes,
                    Extension = extension,
                    DateUploaded = DateTime.Now,
                    Uploader = HttpContext.Current.Request.UserHostAddress,
                    Enabled = true
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

                return image.Id;
            }
        }

        private void DisableImage(string imageId)
        {
            using (RepositorifyEntities context = new RepositorifyEntities())
            {
                var image = context.Images.Find(imageId);
                image.Enabled = false;
                context.SaveChanges();
            }
        }

        private System.Drawing.Image GetThumbnail(Stream stream)
        {
            const int THUMBNAIL_WIDTH = 120;

            var image = System.Drawing.Image.FromStream(stream);
            var height = image.Height;
            var width = image.Width;
            var thumbnailHeight = THUMBNAIL_WIDTH * height / width; // Get correct aspect ratio for thumbnail height

            var thumbnail = image.GetThumbnailImage(THUMBNAIL_WIDTH, thumbnailHeight, () => false, IntPtr.Zero);
            return thumbnail;
        }

        public bool SaveImage(string serverPath, HttpPostedFileBase file, string tags)
        {
            string imageId = "";
            try
            {
                // Create new tags
                var tagsList = ConvertTagStringToList(tags);
                CreateNewTags(tagsList);

                // Create an entry and save image
                var imageExtension = Path.GetExtension(file.FileName);
                imageId = CreateImage(tagsList, file.ContentLength, imageExtension);
                var imagePath = Path.Combine(serverPath, "Images", imageId + imageExtension);
                file.SaveAs(imagePath);

                // Create and save thumbnail
                var thumbnail = GetThumbnail(file.InputStream);
                var thumbnailPath = Path.Combine(serverPath, "Thumbnails", imageId + imageExtension);
                thumbnail.Save(thumbnailPath);
            }
            catch (Exception ex)
            {
                DisableImage(imageId);
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