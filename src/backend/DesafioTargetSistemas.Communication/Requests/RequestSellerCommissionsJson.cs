using System;
using System.Collections.Generic;
using System.Text;

namespace DesafioTargetSistemas.Communication.Requests
{
    public class RequestSellerCommissionsJson
    {
        public DateOnly? From { get; set; }
        public DateOnly? To { get; set; }
    }
}
