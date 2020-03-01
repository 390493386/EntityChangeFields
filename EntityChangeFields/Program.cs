using EntityChangeFields.FieldChanges;
using System;

namespace EntityChangeFields
{
    class Program
    {
        static void Main(string[] args)
        {
            AutoChangeLog<User>.CreateConfig()
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

            var adding = AutoChangeLog<User>.GetChangeRecord(null, user1);
            var midifying = AutoChangeLog<User>.GetChangeRecord(user1, user2);
            var deleting = AutoChangeLog<User>.GetChangeRecord(user2, null);
        }
    }
}
