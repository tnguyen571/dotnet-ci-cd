﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiJWT.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string FunctionDomain { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}
