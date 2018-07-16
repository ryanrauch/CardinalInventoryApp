using System;

namespace CardinalInventoryApp.Contracts
{
    public class ApplicationUserContract
    {
        public Guid Id { get; set; }
        public bool Active { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
    }
}
