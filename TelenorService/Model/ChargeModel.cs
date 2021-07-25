using System;
using System.Collections.Generic;
using System.Text;

namespace TelenorService.Model
{
    public class ChargeRequest
    {
        //{
        //    "msisdn": "03007043064",
        //    "chargableAmount": "0.01",
        //    "correlationId": "1234",
        //    "PartnerID": "TP-Muse",
        //    "ProductID": "TP-MuseWeekly-Charge",
        //    "TransactionID": "1234"
        //}
        public string msisdn { get; set; }
        public decimal chargableAmount { get; set; }
        public string correlationId { get; set; }
        public string PartnerID { get; set; }
        public string ProductID { get; set; }
        public string TransactionID { get; set; }
        public int DurationInWeeks { get; set; }
    }
    public class ChargeResponse
    {
        //{
        //    "requestId": "52587-31572056-3",
        //    "errorCode": "500.007.05",
        //    "errorMessage": "The subscriber does not exist or the customer that the subscriber belongs to is being migrated. Please check."
        //}
        //{
        //    "RequestID": "117252-929049-1",
        //    "Timestamp": "20200715081943",
        //    "Message": "Success"
        //}
        public string requestId { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string Message { get; set; }
        public string Timestamp { get; set; }
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

    public class SmsResponse
    {
        //{
        //    "Message": "Message submitted successfully"
        //}

        public string Message { get; set; }
        public string requestId { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
    }
    public class AuthResponse
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
    }
}
