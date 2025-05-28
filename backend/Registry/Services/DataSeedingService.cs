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
                ImageUrl = "https://chatgpt.com/backend-api/public_content/enc/eyJpZCI6Im1fNjgzMzQ5YmZmY2UwODE5MWJjZjViYjMxYTkxNzBmNTM6ZmlsZV8wMDAwMDAwMDkzNTA2MWY4YWNkOTJhMjViZTYxNjE0OCIsInRzIjoiNDg1NjA4IiwicCI6InB5aSIsInNpZyI6IjY5ZjFiMWVhMzdmNmFkNmIwYWQxMWYwMzI2MmZjMmM1OGFlMThkYzc0Y2QxMWNlOGQ1ZjMwNDBhMjA3ZTdiZDEiLCJ2IjoiMCIsImdpem1vX2lkIjpudWxsfQ=="
            },
            new()
            {
                Id = Guid.Parse("db6602a4-56b1-4694-93ea-68d9e5463dad"),
                Type = "Instalatii sanitare",
                ImageUrl = "https://chatgpt.com/backend-api/public_content/enc/eyJpZCI6Im1fNjgzMzRhM2NmMTAwODE5MWJiODA1YmVhMGI0MDA4ZDY6ZmlsZV8wMDAwMDAwMDUxZTA2MWY4OGQzYmVlZjlkYTg2MzUyYSIsInRzIjoiNDg1NjA4IiwicCI6InB5aSIsInNpZyI6IjIyZTQ1MjcwM2Q2MTIxZGE2MmM5ZTc0NmY0MGFhYzhjYjdjNGIyMTNlMTA0YmU4ZDkxYmE2ZmUwYTI2MTNlZDUiLCJ2IjoiMCIsImdpem1vX2lkIjpudWxsfQ=="
            },
            new()
            {
                Id = Guid.Parse("b437715a-4143-460c-8125-69ba26e7f6f3"),
                Type = "Zugravit",
                ImageUrl = "https://chatgpt.com/backend-api/public_content/enc/eyJpZCI6Im1fNjgzMzRhMDM2M2RjODE5MThiMzYxMGFhMmM5NGUyZjc6ZmlsZV8wMDAwMDAwMDNjNmM2MWY4OTlmNGE2YzRmMzUyMDkxZCIsInRzIjoiNDg1NjA4IiwicCI6InB5aSIsInNpZyI6ImRjY2Q1NjBlMTE4YTI3MjE0MzZjYzgwOGNmNmRlYWZjZmI2YzJmMDgxOTM0NDc5MTY1ZjhhYzJiNzZjZDdlMWEiLCJ2IjoiMCIsImdpem1vX2lkIjpudWxsfQ=="
            },
            new()
            {
                Id = Guid.Parse("bb0ddd50-e749-4cea-aa57-22e6acb413eb"),
                Type = "Parchet",
                ImageUrl = "https://chatgpt.com/backend-api/public_content/enc/eyJpZCI6Im1fNjgzMzRhNjRmNzcwODE5MWI0NjdhNDE3YmRlMjI1YTE6ZmlsZV8wMDAwMDAwMGYwMjA2MWY4OTEwMjM5NDVmNDNmM2ViOSIsInRzIjoiNDg1NjA4IiwicCI6InB5aSIsInNpZyI6ImVmNmUyMmU4ZWM4MTM2MjU1NGJlNmQyMjQ3MTU1ZTg2NGRhOTU1NmQ4MjcwYjdjY2Y0ZjUzMGU1ZTZmNzcxOWEiLCJ2IjoiMCIsImdpem1vX2lkIjpudWxsfQ=="
            },
            new()
            {
                Id = Guid.Parse("32fff8b1-6f6a-4954-84b2-e44c4c5c83c4"),
                Type = "Curent electric",
                ImageUrl = "https://chatgpt.com/backend-api/public_content/enc/eyJpZCI6Im1fNjgzMzRhNjRmNzcwODE5MWI0NjdhNDE3YmRlMjI1YTE6ZmlsZV8wMDAwMDAwMGYwMjA2MWY4OTEwMjM5NDVmNDNmM2ViOSIsInRzIjoiNDg1NjA4IiwicCI6InB5aSIsInNpZyI6ImVmNmUyMmU4ZWM4MTM2MjU1NGJlNmQyMjQ3MTU1ZTg2NGRhOTU1NmQ4MjcwYjdjY2Y0ZjUzMGU1ZTZmNzcxOWEiLCJ2IjoiMCIsImdpem1vX2lkIjpudWxsfQ=="
            }
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
        }
    }
}