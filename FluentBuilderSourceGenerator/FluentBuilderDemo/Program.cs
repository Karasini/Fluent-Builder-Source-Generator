// See https://aka.ms/new-console-template for more information

var user = new UserBuilder()
    .WithFirstName("Gildia")
    .WithLastName(".NET")
    .Build();

Console.WriteLine($"Hello, {user.FirstName} {user.LastName}");