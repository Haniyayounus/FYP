using Medius.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medius.Model.ViewModels
{
    class CaseViewModel
    {
    }
    public class ChangeStatusViewModel
    {
        public string userId { get; set; }
        public string loggedInUserId { get; set; }
        public int caseId { get; set; }
        public Status Status { get; set; }
    }
}
