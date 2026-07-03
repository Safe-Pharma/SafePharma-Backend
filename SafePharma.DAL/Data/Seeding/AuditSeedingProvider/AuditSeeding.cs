using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.DAL
{
    public class AuditSeeding
    {
        public static List<Audit> GetAudits()
        {
            var createdDate = new DateTime(2026, 3, 3, 9, 0, 0);

            var a1 = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111111");
            var a2 = Guid.Parse("bbbbbbbb-2222-2222-2222-222222222222");
            var a3 = Guid.Parse("cccccccc-3333-3333-3333-333333333333");
            var a4 = Guid.Parse("dddddddd-4444-4444-4444-444444444444");
            var a5 = Guid.Parse("eeeeeeee-5555-5555-5555-555555555555");
            var a6 = Guid.Parse("ffffffff-6666-6666-6666-666666666666");
            var a7 = Guid.Parse("77777777-7777-7777-7777-777777777777");

            var u1 = "99999999-9999-9999-9999-999999999999";
            var u2 = "88888888-8888-8888-8888-888888888888";

            return new List<Audit>
    {
        new Audit
        {
            Id = a1,
            Date = createdDate,
            Action = "Create",
            Entity = "Product",
            Device = "Chrome - Windows",
            UserId = u1
        },
        new Audit
        {
            Id = a2,
            Date = createdDate.AddMinutes(10),
            Action = "Update",
            Entity = "Category",
            Device = "Edge - Windows",
            UserId = u2
        },
        new Audit
        {
            Id = a3,
            Date = createdDate.AddMinutes(20),
            Action = "Delete",
            Entity = "Order",
            Device = "Firefox - Windows",
            UserId = u1
        },
        new Audit
        {
            Id = a4,
            Date = createdDate.AddMinutes(30),
            Action = "Login",
            Entity = "Account",
            Device = "Chrome - Android",
            UserId = u2
        },
        new Audit
        {
            Id = a5,
            Date = createdDate.AddMinutes(40),
            Action = "Logout",
            Entity = "Account",
            Device = "Safari - iPhone",
            UserId = u1
        },
        new Audit
        {
            Id = a6,
            Date = createdDate.AddMinutes(50),
            Action = "Update",
            Entity = "Pharmacy",
            Device = "Edge - Windows",
            UserId = u2
        },
        new Audit
        {
            Id = a7,
            Date = createdDate.AddMinutes(60),
            Action = "Create",
            Entity = "User",
            Device = "Chrome - macOS",
            UserId = u1
        }
    };
        }
    }
}
