using Registry.DTO;
using Registry.Models;
using Registry.Services.Interfaces;

namespace Registry.Services;

public class DataSeedingService : IDataSeedingService
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ITradesManService _tradesManService;

    public DataSeedingService(IAuthenticationService authenticationService, ITradesManService tradesManService)
    {
        _authenticationService = authenticationService;
        _tradesManService = tradesManService;
    }

    public async Task GenerateData()
    {
        var specialties = new List<Speciality>
        {
            new()
            {
                Id = Guid.Parse("993adea3-d67a-47b8-a504-b892c73ba3b5"),
                Type = "Gresie si faianta",
                ImageUrl = "https://sdmntprwestus2.oaiusercontent.com/files/00000000-9350-61f8-acd9-2a25be616148/raw?se=2025-05-25T14%3A26%3A15Z&sp=r&sv=2024-08-04&sr=b&scid=06a4ee18-3de7-5050-a291-5c78a4b0eb15&skoid=30ec2761-8f41-44db-b282-7a0f8809659b&sktid=a48cca56-e6da-484e-a814-9c849652bcb3&skt=2025-05-25T07%3A25%3A09Z&ske=2025-05-26T07%3A25%3A09Z&sks=b&skv=2024-08-04&sig=1x9Qwh//TCcxdVFWisOszaa9nBQvqcqkCgyU4QJ4v48%3D"
            },
            new()
            {
                Id = Guid.Parse("db6602a4-56b1-4694-93ea-68d9e5463dad"),
                Type = "Instalatii sanitare",
                ImageUrl = "https://sdmntprwestus2.oaiusercontent.com/files/00000000-51e0-61f8-8d3b-eef9da86352a/raw?se=2025-05-25T14%3A58%3A25Z&sp=r&sv=2024-08-04&sr=b&scid=6673c41e-3069-5dc2-83a3-1e244391f76e&skoid=30ec2761-8f41-44db-b282-7a0f8809659b&sktid=a48cca56-e6da-484e-a814-9c849652bcb3&skt=2025-05-25T01%3A32%3A11Z&ske=2025-05-26T01%3A32%3A11Z&sks=b&skv=2024-08-04&sig=EChezJ4ESL6d4UfK8F3rWEgTCNQBDo/z4fVjxxlPrCc%3D"
            },
            new()
            {
                Id = Guid.Parse("b437715a-4143-460c-8125-69ba26e7f6f3"),
                Type = "Zugravit",
                ImageUrl = "https://sdmntprwestus2.oaiusercontent.com/files/00000000-3c6c-61f8-99f4-a6c4f352091d/raw?se=2025-05-25T14%3A58%3A25Z&sp=r&sv=2024-08-04&sr=b&scid=58f6058d-7452-511e-a551-24f56dc756ae&skoid=30ec2761-8f41-44db-b282-7a0f8809659b&sktid=a48cca56-e6da-484e-a814-9c849652bcb3&skt=2025-05-25T02%3A20%3A10Z&ske=2025-05-26T02%3A20%3A10Z&sks=b&skv=2024-08-04&sig=XinUbp4FKcrjAarAo2eMZljrqTffWSNCg%2BESQ9r0geA%3D"
            },
            new()
            {
                Id = Guid.Parse("bb0ddd50-e749-4cea-aa57-22e6acb413eb"),
                Type = "Parchet",
                ImageUrl = "https://sdmntprwestus2.oaiusercontent.com/files/00000000-f020-61f8-9102-3945f43f3eb9/raw?se=2025-05-25T14%3A58%3A25Z&sp=r&sv=2024-08-04&sr=b&scid=80d2ec75-e413-573f-bb78-a73e8d0d74a3&skoid=30ec2761-8f41-44db-b282-7a0f8809659b&sktid=a48cca56-e6da-484e-a814-9c849652bcb3&skt=2025-05-25T10%3A23%3A03Z&ske=2025-05-26T10%3A23%3A03Z&sks=b&skv=2024-08-04&sig=FdMddcLMMUEefriiXpS/fiSZlDQtFtYvm%2BneQ%2BoJtUY%3D"
            },
            new()
            {
                Id = Guid.Parse("32fff8b1-6f6a-4954-84b2-e44c4c5c83c4"),
                Type = "Curent electric",
                ImageUrl = "https://sdmntprwestus2.oaiusercontent.com/files/00000000-f020-61f8-9102-3945f43f3eb9/raw?se=2025-05-25T14%3A58%3A25Z&sp=r&sv=2024-08-04&sr=b&scid=80d2ec75-e413-573f-bb78-a73e8d0d74a3&skoid=30ec2761-8f41-44db-b282-7a0f8809659b&sktid=a48cca56-e6da-484e-a814-9c849652bcb3&skt=2025-05-25T10%3A23%3A03Z&ske=2025-05-26T10%3A23%3A03Z&sks=b&skv=2024-08-04&sig=FdMddcLMMUEefriiXpS/fiSZlDQtFtYvm%2BneQ%2BoJtUY%3D"
            }
        };

        var users = new List<User>
        {
            await _authenticationService.CreateUser("Vasile", "vasile", "0734980910"),
            await _authenticationService.CreateUser("Ioan", "ioan", "0734934910"),
            await _authenticationService.CreateUser("Gigi", "gigi", "0734980123"),
            await _authenticationService.CreateUser("Ferencz", "ferencz", "0776909230")
        };

        await _tradesManService.AddSpecialitiesBulk(specialties);

        foreach (var user in users)
        {
            await _tradesManService.UpdateTradesManProfile(user, new TradesManDTO
            {
                Description = $"Mesterul {user.Name}",
                Specialties = specialties.Select(s => s.Type).ToList(),
            });
        }
    }
}