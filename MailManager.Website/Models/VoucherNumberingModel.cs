using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MailManager.Models
{
    public class VoucherNumberingModel
    {
      public long ID { get; set; }
      public long CategoryId { get; set; }
      [ForeignKey("CategoryId")]
      public virtual CategoryModel categoryModel { get; set; }
      public string Description { get; set; }
      public int Style { get; set; }
      public string Prefix { get; set; }
      public string Suffix { get; set; }

      [RegularExpression(@"^[1-9]$", ErrorMessage = "BodyLength should contain single-digit number between 1-9 only")]
      
      public int BodyLength { get; set; }
        [RegularExpression(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$", ErrorMessage = "TotalLength number should contain only numbers")]
      public int TotalLength { get; set; }

        [RegularExpression(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$", ErrorMessage = "Start number should contain only numbers")]
      public int StartNo { get; set; }

        
        [RegularExpression(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$", ErrorMessage = "End number should contain only numbers")]
      public int EndNo { get; set; }
      public int CurrentNo { get; set; }
      public string CreatedBy { get; set; }
      public DateTime CreateDate { get; set; }
      public string LastUpdatedBy { get; set; }
      public DateTime LastUpdateDate { get; set; }
      public Boolean IsPrefillzero { get; set; }
     
    }
}