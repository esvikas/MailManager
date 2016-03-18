using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MailManager.Models
{
    public class VoucherConfigurationModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "BodyLength is required")]
        [RegularExpression(@"^[1-9]$", ErrorMessage = "BodyLength should contain single-digit number between 1-9 only")]

        public int BodyLength { get; set; }
        [RegularExpression(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$", ErrorMessage = "TotalLength number should contain only numbers")]
        public int TotalLength { get; set; }

        [Required(ErrorMessage = "StartNo is required")]
        [RegularExpression(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$", ErrorMessage = "Start number should contain only numbers")]
        public int StartNo { get; set; }

        [Required(ErrorMessage = "EndNo is required")]
        [RegularExpression(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$", ErrorMessage = "End number should contain only numbers")]
        public int EndNo { get; set; }
        public int CurrentNo { get; set; }
    }
}