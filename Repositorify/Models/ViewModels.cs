using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repositorify.Models
{
    public class ViewModels
    {
        public class TagViewModel
        {
            public string Id { get; set; }
        }

        public class ImageViewModel
        {
            public string Image { get; set; }
            public string Thumbnail { get; set; }
            public List<string> Tags { get; set; }
        }
    }
}