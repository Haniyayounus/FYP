using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medius.Model.ViewModels
{
    class SmsViewModel
    {
    }
    public class SmsRequest
    {
        //{
        //    "messageBody": "asfasfasf",
        //    "recipientMsisdn": "03232601464"
        //}

        public string messageBody { get; set; }
        public string recipientMsisdn { get; set; }
    }
}
