using Registry.Models;
using Registry.Services.Interfaces;

namespace Registry.Services;

public class DataSeedingService : IDataSeeding
{
    private IAuthenticationService _authenticationService;

    public DataSeedingService(AuthenticationService authenticationService)
    {
        this._authenticationService = authenticationService;
    }
    
    public async Task GenerateData()
    {
        var specialties = new List<Specialty>
        {
            new()
            {
                Id = Guid.Parse("993adea3-d67a-47b8-a504-b892c73ba3b5"),
                Type = "Gresie si faianta"
            },
            new()
            {
                Id = Guid.Parse("db6602a4-56b1-4694-93ea-68d9e5463dad"),
                Type = "Instalatii sanitare"
            },
            new()
            {
                Id = Guid.Parse("b437715a-4143-460c-8125-69ba26e7f6f3"),
                Type = "Zugravit"
            },
            new()
            {
                Id = Guid.Parse("bb0ddd50-e749-4cea-aa57-22e6acb413eb"),
                Type = "Parchet"
            },
            new()
            {
                Id = Guid.Parse("32fff8b1-6f6a-4954-84b2-e44c4c5c83c4"),
                Type = "Curent electric"
            }
        };

        var users = new List<User>
        {
            await _authenticationService.CreateUser("Vasile", "vasile", "0734980910"),
            await _authenticationService.CreateUser("Ioan", "ioan", "0734934910"),
            await _authenticationService.CreateUser("Gigi", "gigi", "0734980123"),
            await _authenticationService.CreateUser("Ferencz", "ferencz", "0776909230")
        };
        
        
    }
}