using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SugSession.Model
{
    public class APIResponses
    {
        public bool Success { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorMessageField { get; set; }
        public string InnerException { get; set; }
        public APIResponses()
        {
            this.Success = false;
            this.SuccessMessage = "";
            this.ErrorMessage = "";
            this.ErrorMessageField = "";
            this.InnerException = "";
        }
    }

    public class APILoginResponses : APIResponses
    {
        public string UserToken { get; set; }
    }
}