using Recombee.ApiClient;
using Recombee.ApiClient.ApiRequests;
using Recombee.ApiClient.Util;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.Net;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main()
    {
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

        var dbId = config["Recombee:DatabaseId"];
        var token = config["Recombee:PrivateToken"];

        RecombeeClient client = new RecombeeClient(dbId, token, region: Region.EuWest);

        await AddProperty(client, "Song", "string");
        await AddProperty(client, "Artist", "string");
        await AddProperty(client, "Album", "string");
        await AddProperty(client, "Popularity", "int");
        await AddProperty(client, "Danceability", "double");

        var csvPath = Path.Combine(AppContext.BaseDirectory, "spotify_songs_cleaned.csv");
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header?.Trim().ToLower().Replace(" ", "_"),
            MissingFieldFound = null,
            HeaderValidated = null,
            BadDataFound = null,
        };

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, csvConfig);
        csv.Context.RegisterClassMap<SpotifyMap>();

        var rows = csv.GetRecords<SpotifyRow>().Take(1000);
        int cnt = 0;

        foreach (var r in rows)
        {
            if (string.IsNullOrWhiteSpace(r.Id))
                continue;

            var values = new Dictionary<string, object>
            {
                ["Song"] = r.Song,
                ["Artist"] = r.Artist,
                ["Album"] = r.Album,
                ["Popularity"] = r.Popularity,
                ["Danceability"] = r.Danceability,
            };

            await client.SendAsync(new SetItemValues(r.Id, values, cascadeCreate: true));
            cnt++;
        }

        Console.WriteLine($"Gata! Am adaugat {cnt} items. Vezi Catalog - Items.");

        await AddUserProperty(client, "FirstName", "string");
        await AddUserProperty(client, "LastName", "string");
        await AddUserProperty(client, "Email", "string");
        await AddUserProperty(client, "Age", "int");
        await AddUserProperty(client, "Country", "string");

        var random = new Random();
        var firstNames = new[] { "Ana", "Mihai", "Ioana", "Andrei", "Elena", "Radu", "Maria", "Vlad", "Carmen", "Alex", "Stefania", "Bogdan", "Dan", "Cristina", "Toma", "Andrada", "Anca", "Edi", "Matei", "Marius" };
        var lastNames = new[] { "Popescu", "Ionescu", "Georgescu", "Dumitrescu", "Stan", "Tudor", "Rusu", "Marin", "Enache", "Iliescu", "Apostol", "Tanasescu", "Pana", "Gheorghe", "Stanila", "Catana", "Mihai", "Marinescu", "Enachescu", "Ilie" };
        var countries = new[] { "Romania", "Italy", "France", "Germany", "Spain" };

        for (int i = 1; i <= 20; i++)
        {
            string userId = $"user-{i}";
            string firstName = firstNames[random.Next(firstNames.Length)];
            string lastName = lastNames[random.Next(lastNames.Length)];
            string email = $"{firstName.ToLower()}.{lastName.ToLower()}@example.com";
            int age = random.Next(18, 60);
            string country = countries[random.Next(countries.Length)];

            var userValues = new Dictionary<string, object>
            {
                ["FirstName"] = firstName,
                ["LastName"] = lastName,
                ["Email"] = email,
                ["Age"] = age,
                ["Country"] = country
            };

            await client.SendAsync(new SetUserValues(userId, userValues, cascadeCreate: true));
            Console.WriteLine($"Utilizator adaugat: {firstName} {lastName} ({email})");
        }

        Console.WriteLine("Toti utilizatorii au fost adaugti!");
    }

    static async Task AddProperty(RecombeeClient client, string name, string type)
    {
        try
        {
            await client.SendAsync(new AddItemProperty(name, type));
            Console.WriteLine($"AddItemProperty OK: {name} ({type})");
        }
        catch (ResponseException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            Console.WriteLine($"AddItemProperty exista: {name} (ignora)");
        }
    }

    static async Task AddUserProperty(RecombeeClient client, string name, string type)
    {
        try
        {
            await client.SendAsync(new AddUserProperty(name, type));
            Console.WriteLine($"AddUserProperty OK: {name} ({type})");
        }
        catch (ResponseException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            Console.WriteLine($"AddUserProperty exista: {name} (ignora)");
        }
    }
}

class SpotifyRow
{
    public string Id { get; set; }
    public string? Song { get; set; }
    public string? Artist { get; set; }
    public string? Album { get; set; }
    public int Popularity { get; set; }
    public double Danceability { get; set; }
}

sealed class SpotifyMap : ClassMap<SpotifyRow>
{
    public SpotifyMap()
    {
        ;
        Map(m => m.Id).Name("track_id");
        Map(m => m.Song).Name("track_name");
        Map(m => m.Artist).Name("track_artist");
        Map(m => m.Album).Name("track_album_name");
        Map(m => m.Popularity).Name("track_popularity", "track_pop");
        Map(m => m.Danceability).Name("danceability");
    }
}