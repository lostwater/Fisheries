using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fisheries.Models
{
    public class Information
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }

        public int InformationTypeId { get; set; }
        public InformationType InformationType { get; set; }
    }

    public class InformationCreateModel
    {
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public HttpPostedFileBase Image { get; set; }
        public string Content { get; set; }
        public int InformationTypeId { get; set; }
    }

}