using System;

namespace AccountMicroservice.Exceptions
{
    [Serializable]
    public class EmailAlreadyExistsException : Exception
    {
        
        public EmailAlreadyExistsException() 
                : base("This email is already in use.")
            {
            }
        }

}