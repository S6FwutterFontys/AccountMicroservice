﻿using System;

namespace AccountMicroservice.Exceptions
{
    [Serializable]
    public class IncorrectPasswordException : Exception
    {
        public IncorrectPasswordException()
            : base("This password is incorrect.")
        {
        }
    }
}