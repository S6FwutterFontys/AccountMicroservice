﻿using System;

namespace AccountMicroservice.Exceptions
{
    [Serializable]
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException(string message) : base(message)
        {
            
        }
    }
}