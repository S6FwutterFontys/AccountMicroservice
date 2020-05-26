﻿using System;

namespace AccountMicroservice.Exceptions
{
    [Serializable]
    public class EmailNotFoundException : Exception
    {
        public EmailNotFoundException()
            : base("A user with this email was not found.")
        {
        }
    }
}
