using Registry.DTO;
using Registry.Models;
using Registry.Services.Interfaces;

namespace Registry.Services;

public class DataSeedingService : IDataSeedingService
{
    private readonly IUserService _authenticationService;
    private readonly ITradesManService _tradesManService;

    public DataSeedingService(IUserService authenticationService, ITradesManService tradesManService)
    {
        _authenticationService = authenticationService;
        _tradesManService = tradesManService;
    }

    public async Task GenerateData()
    {
        var random = new Random();
        var specialities = new List<Speciality>
        {
            new()
            {
                Id = Guid.Parse("993adea3-d67a-47b8-a504-b892c73ba3b5"),
                Type = "Gresie si faianta",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/destinationbucketimages.appspot.com/o/ChatGPT%20Image%20May%2025%2C%202025%2C%2004_27_39%20PM.png?alt=media&token=eb348a62-4e7a-417c-a8e3-a80e43f826bf"
            },
            new()
            {
                Id = Guid.Parse("db6602a4-56b1-4694-93ea-68d9e5463dad"),
                Type = "Instalatii sanitare",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/destinationbucketimages.appspot.com/o/ChatGPT%20Image%20May%2025%2C%202025%2C%2004_57_53%20PM.png?alt=media&token=e16e0196-0d48-4031-8912-dbfffec95191"
            },
            new()
            {
                Id = Guid.Parse("b437715a-4143-460c-8125-69ba26e7f6f3"),
                Type = "Zugravit",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/destinationbucketimages.appspot.com/o/ChatGPT%20Image%20May%2025%2C%202025%2C%2004_56_00%20PM.png?alt=media&token=e956cb70-ec84-4629-8953-ab0708681dd0"
            },
            new()
            {
                Id = Guid.Parse("bb0ddd50-e749-4cea-aa57-22e6acb413eb"),
                Type = "Parchet",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/destinationbucketimages.appspot.com/o/ChatGPT%20Image%20May%2025%2C%202025%2C%2004_59_45%20PM.png?alt=media&token=823c9f76-3af9-4100-9e59-b76f578d7b39"
            },
            new()
            {
                Id = Guid.Parse("32fff8b1-6f6a-4954-84b2-e44c4c5c83c4"),
                Type = "Curent electric",
                ImageUrl = "https://firebasestorage.googleapis.com/v0/b/destinationbucketimages.appspot.com/o/ChatGPT%20Image%20Jun%202%2C%202025%2C%2007_55_22%20PM.png?alt=media&token=69228b8e-8cef-4201-a4d2-b99101df521f"
            }
        };

        var workorderImages = new List<string>
        {
            "https://firebasestorage.googleapis.com/v0/b/destinationbucketimages.appspot.com/o/image%20(1).webp?alt=media&token=5ad7db11-9052-4bbf-b9e7-9f4663f70766",
            "https://firebasestorage.googleapis.com/v0/b/destinationbucketimages.appspot.com/o/image%20(2).webp?alt=media&token=d1d87a5a-4db2-44aa-af85-32ca36221f55",
            "https://firebasestorage.googleapis.com/v0/b/destinationbucketimages.appspot.com/o/image.webp?alt=media&token=fdc870ee-26d1-428a-9cb2-7cfb6269f659",
        };

        var users = new List<User>
        {
            await _authenticationService.CreateUser("Vasile", "vasile", "0734980910", "vasile@gmail.com"),
            await _authenticationService.CreateUser("Ioan", "ioan", "0734934910", "ioan@gmail.com"),
            await _authenticationService.CreateUser("Gigi", "gigi", "0734980123", "gigi@gmail.com"),
            await _authenticationService.CreateUser("Ferencz", "ferencz", "0776909230", "ferencz@gmail.com")
        };

        await _tradesManService.AddSpecialitiesBulk(specialities);

        foreach (var user in users)
        {
            await _tradesManService.UpdateTradesManProfile(user, new TradesManDTO
            {
                Description = $"Mesterul {user.Name}",
                City = "Cluj-Napoca",
                County = "Cluj",
                Specialities = specialities.OrderBy(x => random.Next()).Take(random.Next(1, specialities.Count)).Select(s => new AssignSpecialityDTO
                {
                    Name = s.Type,
                    UnitOfMeasure = "metru patrat",
                    Price = (uint)random.Next(1, 500),
                }).ToList(),
            });

            var tradesmanId = (await _tradesManService.GetTradesManInfo(user.Id)).Id;

            await _tradesManService.AddWorkorderImages(tradesmanId, workorderImages);
        }
    }
}