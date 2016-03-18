
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace MailManager.Models
{
    public class CategoryModel
    {
        public long CategoryID { get; set; }

        [Required(ErrorMessage="Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Code is required")]
        [Remote("IsCodeAvailble", "Category", AdditionalFields = "CategoryID", ErrorMessage = "Code already exist.")]
        public string Code { get; set; }
        public string Description { get; set; }
        public string InsertedBy { get; set; }
        public DateTime InsertedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; } 

        }
}