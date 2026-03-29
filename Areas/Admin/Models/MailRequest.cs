using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GabriniCosmetics.Areas.Admin.Models
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}