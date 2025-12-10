using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace StarWarsApp.E2E;

[TestFixture]
public class ExplorePageTests : PageTest
{
  private const string BaseUrl = "http://localhost:5276";

  [Test]
  public async Task Explore_ShowsHeading_AndAtLeastOneEntity()
  {
    await Page.GotoAsync(BaseUrl + "/explore");

    await Page.WaitForSelectorAsync(".entities-heading");

    var heading = await Page.TextContentAsync(".entities-heading");
    Assert.That(heading, Does.Contain("Explore"));

    await Page.WaitForSelectorAsync(".entities-list .entity-link");

    var links = Page.Locator(".entity-link");
    var count = await links.CountAsync();
    Assert.That(count, Is.GreaterThan(0));
  }

  [Test]
  public async Task Explore_Filter_Droids_ShowsOnlyDroidsLinks()
  {
    await Page.GotoAsync(BaseUrl + "/explore");
    await Page.WaitForSelectorAsync(".entities-filter-select");

    await Page.SelectOptionAsync(".entities-filter-select", "droids");

    await Page.WaitForSelectorAsync(".entities-list .entity-link");

    var links = Page.Locator(".entity-link");
    var count = await links.CountAsync();
    Assert.That(count, Is.GreaterThan(0), "Expected at least one droid entity.");

    for (var i = 0; i < count; i++)
    {
      var href = await links.Nth(i).GetAttributeAsync("href");
      Assert.That(href, Is.Not.Null.And.StartsWith("/droids/"), $"Expected href to start with /droids/, but got {href}");
    }
  }

  [Test]
  public async Task Explore_Filter_Characters_ShowsOnlyCharacterLinks()
  {
    await Page.GotoAsync(BaseUrl + "/explore");
    await Page.WaitForSelectorAsync(".entities-filter-select");

    await Page.SelectOptionAsync(".entities-filter-select", "characters");

    await Page.WaitForSelectorAsync(".entities-list .entity-link");

    var links = Page.Locator(".entity-link");
    var count = await links.CountAsync();
    Assert.That(count, Is.GreaterThan(0), "Expected at least one character entity.");

    for (var i = 0; i < count; i++)
    {
      var href = await links.Nth(i).GetAttributeAsync("href");
      Assert.That(href, Is.Not.Null.And.StartsWith("/characters/"), $"Expected href to start with /characters/, but got {href}");
    }
  }

  [Test]
  public async Task Explore_ClickingEntity_NavigatesToDetailPage()
  {
    await Page.GotoAsync(BaseUrl + "/explore");

    await Page.WaitForSelectorAsync(".entities-list .entity-link");

    var links = Page.Locator(".entity-link");
    var count = await links.CountAsync();
    Assert.That(count, Is.GreaterThan(0));

    await links.Nth(0).ClickAsync();

    await Page.WaitForSelectorAsync(".entity-hero-heading");

    var url = System.Uri.UnescapeDataString(Page.Url);

    Assert.That(url, Does.Contain("/droids/").Or.Contain("/characters/").Or.Contain("/creatures/"), $"Unexpected detail URL: {url}");
  }
}

