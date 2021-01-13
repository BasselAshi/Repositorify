using Repositorify.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            const int MAX_DIMENSION = 128;

            var image = System.Drawing.Image.FromStream(stream);
            int thumbnailWidth = MAX_DIMENSION;
            int thumbnailHeight = MAX_DIMENSION;
            if (image.Height > image.Width)
            {
                thumbnailWidth = thumbnailHeight * image.Width / image.Height; // Maintain aspect ratio
            } else
            {
                thumbnailHeight = thumbnailWidth * image.Height / image.Width; // Maintain aspect ratio
            }
            var thumbnail = image.GetThumbnailImage(thumbnailWidth, thumbnailHeight, () => false, IntPtr.Zero);
            return thumbnail;
        }

        public string SaveImage(string serverPath, HttpPostedFileBase file, List<string> tags)
        {
            string imageId = "";
            try
            {
                // Create new tags
                CreateNewTags(tags);

                // Create an entry and save image
                var imageExtension = Path.GetExtension(file.FileName);
                imageId = CreateImage(tags, file.ContentLength, imageExtension);
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
                return ex.Message;
            }
            return "";
        }
        public List<ImageViewModel> GetImageViewModels(string tag)
        {
            using (RepositorifyEntities context = new RepositorifyEntities())
            {
                var contextTag = context.Tags.Find(tag);
                
                if (contextTag == null)
                {
                    return null;
                }

                var imageModels = (from i in context.vw_Images
                                   where i.TagId.Equals(tag)
                                   group i.Tag by new { i.ImageLink, i.ThumbnailLink } into g
                                   select new ImageViewModel {
                                       Image = g.Key.ImageLink,
                                       Thumbnail = g.Key.ThumbnailLink,
                                       Tags = g.ToList()
                                   }).ToList();

                return imageModels;
            }
        }

        public List<string> ConvertStringTags(string tags)
        {
            var tagsStripped = Regex.Replace(tags, @"\s+", "");
            var tagsList = tagsStripped.Split(',').ToList();
            return tagsList;
        }
    }
}