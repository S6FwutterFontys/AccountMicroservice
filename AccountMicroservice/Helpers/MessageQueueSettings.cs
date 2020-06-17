﻿using System;

namespace AccountService.Helpers
{
    public class MessageQueueSettings
    {
        public string Uri { get; set; }
        public string Exchange { get; set; }
    }
}