public class AddressCatalogTests
{
    [Fact]
    public void West_jutland_addresses_cover_the_towns_around_kolding()
    {
        var cities = AddressCatalog.KoldingArea.Select(place => place.City).ToHashSet();
        Assert.Equal(["Fredericia", "Kolding", "Middelfart", "Vejle"], cities.OrderBy(city => city));
        Assert.Contains(AddressCatalog.KoldingArea, place => place.Street == "Banegårdspladsen 4" && place.City == "Kolding");
        Assert.All(AddressCatalog.KoldingArea, place => Assert.False(string.IsNullOrWhiteSpace(place.Street)));
    }

    [Fact]
    public void Copenhagen_addresses_are_labeled_kobenhavn()
    {
        Assert.Contains(AddressCatalog.CopenhagenArea, place => place.Street == "Bernstorffsgade 16");
        Assert.All(AddressCatalog.CopenhagenArea, place => Assert.Equal("København", place.City));
    }

    [Fact]
    public void Driver_names_include_the_sample_drivers()
    {
        Assert.Contains("Lucas", Drivers.Names);
        Assert.Contains("Espen", Drivers.Names);
        Assert.Equal(Drivers.Names.Count, Drivers.Names.Distinct().Count());
    }
}
