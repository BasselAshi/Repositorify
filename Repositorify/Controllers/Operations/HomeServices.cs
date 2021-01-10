using Repositorify.Models;
using System;
using System.Collections.Generic;
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
    }
}