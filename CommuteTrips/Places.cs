public sealed record Place(string Street, string City);

public static class AddressCatalog
{
    public static IReadOnlyList<Place> KoldingArea { get; } =
    [
        new("Banegårdspladsen 4", "Kolding"),
        new("Buen 1", "Kolding"),
        new("Jernbanegade 8", "Kolding"),
        new("Akseltorv 5", "Kolding"),
        new("Rendebanen 7", "Kolding"),
        new("Danmarksgade 16", "Fredericia"),
        new("Gothersgade 11", "Fredericia"),
        new("Jernbanegade 5", "Fredericia"),
        new("Banegårdspladsen 2", "Vejle"),
        new("Dæmningen 14", "Vejle"),
        new("Nørretorv 1", "Vejle"),
        new("Østergade 10", "Middelfart"),
        new("Algade 16", "Middelfart"),
        new("Brogade 3", "Middelfart"),
    ];

    public static IReadOnlyList<Place> CopenhagenArea { get; } =
    [
        new("Bernstorffsgade 16", "København"),
        new("Rådhuspladsen 1", "København"),
        new("Købmagergade 52", "København"),
        new("Istedgade 44", "København"),
        new("Vesterbrogade 6A", "København"),
        new("Sønder Boulevard 48", "København"),
        new("Gammel Kongevej 80", "København"),
        new("Falkoner Allé 20", "København"),
        new("Allégade 10", "København"),
        new("Valby Langgade 34", "København"),
        new("Toftegårds Allé 8", "København"),
        new("Østerbrogade 70", "København"),
        new("Nordre Frihavnsgade 22", "København"),
        new("Strandvejen 203", "København"),
        new("Hellerupvej 12", "København"),
    ];
}

public static class Drivers
{
    public static IReadOnlyList<string> Names { get; } =
    [
        "Lucas",
        "Espen",
        "Maja",
        "Sofie",
        "Noah",
        "Emil",
        "Ida",
        "Oscar",
        "Freja",
        "William",
        "Clara",
        "Magnus",
        "Anna",
        "Oliver",
        "Laura",
        "Jonas",
        "Emma",
        "Mikkel",
        "Sara",
        "Andreas",
    ];
}
