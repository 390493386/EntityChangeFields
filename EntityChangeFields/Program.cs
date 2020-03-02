using EntityChangeFields.ChangeTracking;

namespace EntityChangeFields
{
    class Program
    {
        static void Main(string[] args)
        {
            ChangeTracker<User>.CreateConfig()
                .SetTrackingField(x => x.Name, fieldName: "姓名")
                .SetTrackingField(x => x.Account)
                .SetTrackingField(x => x.Age);

            var user1 = new User
            {
                Name = "詹晖",
                Account = "Skyder",
                Age = 30,
            };
            var user2 = new User
            {
                Name = "杨珍珍",
                Account = "YZZ",
                Age = 18,
            };

            var adding = ChangeTracker<User>.GetChangeRecord(null, user1);
            var midifying = ChangeTracker<User>.GetChangeRecord(user1, user2);
            var deleting = ChangeTracker<User>.GetChangeRecord(user2, null);
        }
    }
}
