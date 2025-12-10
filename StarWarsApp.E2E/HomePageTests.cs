using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace StarWarsApp.E2E;

[TestFixture]
public class HomePageTests : PageTest
{
  private const string BaseUrl = "http://localhost:5276";

  [Test]
  public async Task Home_ShowsHeroContent_WhenLoaded()
  {
    await Page.GotoAsync(BaseUrl + "/");

    await Page.WaitForSelectorAsync(".hero-title-main");

    var heading = await Page.TextContentAsync(".hero-title-main");
    Assert.That(heading, Does.Contain("Explore the Galaxy"));

    var locationName = await Page.TextContentAsync(".hero-location-name");
    Assert.That(locationName, Is.Not.Null.And.Not.Empty);
  }

  [Test]
  public async Task Search_ShowsError_WhenEmpty()
  {
    await Page.GotoAsync(BaseUrl + "/");
    await Page.WaitForSelectorAsync(".hero-search-input");

    await Page.ClickAsync(".hero-search-button");

    var error = await Page.TextContentAsync(".hero-search-error");
    Assert.That(error, Is.EqualTo("Please enter a character name."));
  }

  [Test]
  public async Task Search_ShowsError_WhenTooShort()
  {
    await Page.GotoAsync(BaseUrl + "/");
    await Page.WaitForSelectorAsync(".hero-search-input");

    await Page.FillAsync(".hero-search-input", "L");
    await Page.ClickAsync(".hero-search-button");

    var error = await Page.TextContentAsync(".hero-search-error");
    Assert.That(error, Is.EqualTo("Type at least 2 characters."));
  }

  [Test]
  public async Task Search_NavigatesToCharacterDetail_WhenValid()
  {
    await Page.GotoAsync(BaseUrl + "/");
    await Page.WaitForSelectorAsync(".hero-search-input");

    const string query = "Test";

    await Page.FillAsync(".hero-search-input", query);
    await Page.ClickAsync(".hero-search-button");

    await Page.WaitForURLAsync("**/characters/**");

    var decodedUrl = System.Uri.UnescapeDataString(Page.Url);

    Assert.That(decodedUrl, Does.Contain("/characters/"));
    Assert.That(decodedUrl, Does.Contain(query));
  }
}

