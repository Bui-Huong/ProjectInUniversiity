using AssetManagementWebApi.Repositories.Entities;
using AssetManagementWebApi.Repositories.Entities.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementWebApi.Repositories.EFContext;
public static class AssetManagementSeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {   
        modelBuilder.Entity<AppRole>().HasData(
            new AppRole
            {
                Id = new Guid("bffb1013-98ef-4d79-a040-7b0443a32dd2"),
                Name = "Staff",
                NormalizedName = "STAFF",
                ConcurrencyStamp = "31BF5413-8303-4E21-8D3A-10099FCA95FE",
                Description = "Staff",
            }, new AppRole
            {
                Id = new Guid("f6029ede-ecbe-47e0-84ed-9e94cdcbc2b5"),
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "94BD65EE-DE64-4476-91AA-6258155DE018",
                Description = "Admin of the web",
            }
        );
        modelBuilder.Entity<AppUser>().HasData(
            new AppUser()
            {
                Id = new Guid("9bdb327a-c2af-482d-9d94-08da0c99a65e"),
                Code = "SD0001",
                Location = "HaNoi",
                FirstName = "Nghia",
                LastName = "Le Trung",
                DoB = new DateTime(2000, 02, 02),
                JoinDate = DateTime.Now,
                Gender = (Gender)0,
                Type = (Role)0,
                UserName = "nghialt",
                NormalizedUserName = "NGHIALT",
                IsDisabled = false,
                PasswordHash = "AQAAAAEAACcQAAAAELFeoYLfNwEY4tPzOMiT3AtO6oJoesCKz6SF+IUmcx6kLJyxbfp/jtsc5O0JXzorwA==",
                SecurityStamp = "2TPO3UTGO7MJ2NK2MQ25XMNIINYWFCZB",
                ConcurrencyStamp = "323ea961-d2b2-46de-8b3b-4b5fcd434545",
            },
            new AppUser()
            {
                Id = new Guid("b8ea2b0c-1e59-4687-9d95-08da0c99a65e"),
                Code = "SD0002",
                Location = "DaNang",
                FirstName = "Kien",
                LastName = "Vu Trung",
                DoB = new DateTime(2000, 03, 01),
                JoinDate = DateTime.Now,
                Gender = (Gender)0,
                Type = (Role)0,
                UserName = "kienvt",
                NormalizedUserName = "KIENVT",
                IsDisabled = false,
                PasswordHash = "AQAAAAEAACcQAAAAENzG98qyEaS0MwtEvSC6b3rpi50CZplcyuUFxvFhLyT5cqigExI4ARj5SXTF7X5wdg==",
                SecurityStamp = "O474CJFI2WM6GSY7VACWJTLAUNZUX333",
                ConcurrencyStamp = "3ed18893-c009-4622-995a-e219a1de8b48",
            },
            new AppUser()
            {
                Id = new Guid("ddab4ac5-8b10-4dc0-9d96-08da0c99a65e"),
                Code = "SD0003",
                Location = "HoChiMinh",
                FirstName = "Thu",
                LastName = "Vu Minh",
                DoB = new DateTime(1998, 03, 01),
                JoinDate = DateTime.Now,
                Gender = (Gender)0,
                Type = (Role)0,
                UserName = "thuvm",
                NormalizedUserName = "THUVM",
                IsDisabled = false,
                PasswordHash = "AQAAAAEAACcQAAAAEKMYuOVsItDYqtfcqpYDphyfCFGA/1x9+8noH+9o8IP11dluJONMlRsQJdpI+WBfzw==",
                SecurityStamp = "QMOWHSHOTF7JNNOGPUX6GFLLRNMTIFNB",
                ConcurrencyStamp = "4227dd14-be46-43f7-91f1-e38a7f0dc692",
            },
            new AppUser()
            {
                Id = new Guid("7e82249a-b7f9-4655-ae59-59676593c5ed"),
                Code = "SD0004",
                Location = "HaNoi",
                FirstName = "Nghia",
                LastName = "Le Trung",
                DoB = new DateTime(2000, 02, 02),
                JoinDate = DateTime.Now,
                Gender = (Gender)0,
                Type = (Role)1,
                UserName = "nghialt1",
                NormalizedUserName = "NGHIALT1",
                IsDisabled = false,
                PasswordHash = "AQAAAAEAACcQAAAAEH4NaHPsxk9BJIflGLDv5RT+1nNux29q3fJiEBCgwr+VjR+fXRdR/jAdsxFqYF/VAg==",
                SecurityStamp = "R3KWRDQK2FNCTQHCFDA6RH2SBCD2Y6RM",
                ConcurrencyStamp = "0b44ad84-971d-440a-862d-63379919a6c3",
            }
        );
        modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
            new IdentityUserRole<Guid>{
                UserId = new Guid("9bdb327a-c2af-482d-9d94-08da0c99a65e"),
                RoleId = new Guid("f6029ede-ecbe-47e0-84ed-9e94cdcbc2b5")
            }, 
            new IdentityUserRole<Guid>{
                UserId = new Guid("b8ea2b0c-1e59-4687-9d95-08da0c99a65e"),
                RoleId = new Guid("f6029ede-ecbe-47e0-84ed-9e94cdcbc2b5")
            },
            new IdentityUserRole<Guid>{
                UserId = new Guid("ddab4ac5-8b10-4dc0-9d96-08da0c99a65e"),
                RoleId = new Guid("f6029ede-ecbe-47e0-84ed-9e94cdcbc2b5")
            },
            new IdentityUserRole<Guid>{
                UserId = new Guid("7e82249a-b7f9-4655-ae59-59676593c5ed"),
                RoleId = new Guid("bffb1013-98ef-4d79-a040-7b0443a32dd2")
            }
        );
        modelBuilder.Entity<CategoryEntity>().HasData(
            new CategoryEntity{
                Id = new Guid("6b310f84-d463-492b-afea-894d9d77d79f"),
                CategoryName = "Laptop",
                CategoryPrefix = "LA"
            },
            new CategoryEntity{
                Id = new Guid("f23d6d81-201a-4fb2-aa3d-15b4b71a8603"),
                CategoryName = "Car",
                CategoryPrefix = "CA"
            }
        );
        modelBuilder.Entity<AssetEntity>().HasData(
            new AssetEntity{
                Id = new Guid("5070844a-2416-4101-bbef-c82f18923b37"),
                AssetCode = "LA00001",
                AssetName = "Alienware X17",
                CategoryId = new Guid("6b310f84-d463-492b-afea-894d9d77d79f"),
                Specification = "Just a laptop",
                InstalledDate = DateTime.Now,
                Location = "HaNoi",
                State = (AssetState)0
            },
            new AssetEntity{
                Id = new Guid("4a3eda74-216c-4a99-9f97-8866250f15e6"),
                AssetCode = "CA00001",
                AssetName = "Chevrolet Corvette C8",
                CategoryId = new Guid("f23d6d81-201a-4fb2-aa3d-15b4b71a8603"),
                Specification = "Super car for life",
                Location = "HoChiMinh",
                InstalledDate = DateTime.Now,
                State = (AssetState)0
            },
            new AssetEntity{
                Id = new Guid("9da5baa5-def0-4baa-ba5b-93482758eab9"),
                AssetCode = "CA00002",
                AssetName = "Ford mustang GT Premium Fastback",
                CategoryId = new Guid("f23d6d81-201a-4fb2-aa3d-15b4b71a8603"),
                Specification = "Muscle car is so cool!",
                Location = "DaNang",
                InstalledDate = DateTime.Now,
                State = (AssetState)0
            },
            new AssetEntity{
                Id = Guid.NewGuid(),
                AssetCode = "LA00002",
                AssetName = "HP Elite Book 8570w",
                Location = "HaNoi",
                CategoryId = new Guid("6b310f84-d463-492b-afea-894d9d77d79f"),
                Specification = "Workstation laptop",
                InstalledDate = DateTime.Now,
                State = (AssetState)0
            },
            new AssetEntity{
                Id = Guid.NewGuid(),
                AssetCode = "LA00003",
                AssetName = "Lenovo Thinking Pad",
                Location = "HaNoi",
                CategoryId = new Guid("6b310f84-d463-492b-afea-894d9d77d79f"),
                Specification = "Test laptop",
                InstalledDate = DateTime.Now,
                State = (AssetState)2
            },
            new AssetEntity{
                Id = Guid.NewGuid(),
                AssetCode = "LA00004",
                AssetName = "Mac",
                Location = "HaNoi",
                CategoryId = new Guid("6b310f84-d463-492b-afea-894d9d77d79f"),
                Specification = "Test laptop",
                InstalledDate = DateTime.Now,
                State = (AssetState)3
            },
            new AssetEntity{
                Id = Guid.NewGuid(),
                AssetCode = "LA00005",
                AssetName = "Dell Latittude",
                Location = "HaNoi",
                CategoryId = new Guid("6b310f84-d463-492b-afea-894d9d77d79f"),
                Specification = "Test laptop",
                InstalledDate = DateTime.Now,
                State = (AssetState)4
            }
        );
    }
}