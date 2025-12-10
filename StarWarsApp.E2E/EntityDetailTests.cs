using NUnit.Framework;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace StarWarsApp.E2E;

[TestFixture]
public class EntityDetailTests : PageTest
{
  private const string BaseUrl = "http://localhost:5276";

  [Test]
  public async Task Detail_FromExplore_RendersHeroAndBio()
  {
    await Page.GotoAsync(BaseUrl + "/explore");

    await Page.WaitForSelectorAsync(".entities-list .entity-link");
    var links = Page.Locator(".entity-link");
    var count = await links.CountAsync();
    Assert.That(count, Is.GreaterThan(0), "Expected at least one entity on Explore page.");

    await links.Nth(0).ClickAsync();

    await Page.WaitForSelectorAsync(".entity-hero-heading");

    var prefix = await Page.TextContentAsync(".hero-prefix");
    Assert.That(prefix, Is.Not.Null.And.Contains("I AM").IgnoreCase);

    var heroName = await Page.TextContentAsync(".hero-name");
    Assert.That(heroName, Is.Not.Null.And.Not.Empty);

    var bioLabel = await Page.TextContentAsync(".entity-bio-label");
    Assert.That(bioLabel, Is.EqualTo("Bio"));

    var bioText = await Page.TextContentAsync(".entity-bio");
    Assert.That(bioText, Is.Not.Null.And.Not.Empty);
  }

  [Test]
  public async Task Detail_BackLink_NavigatesBackToExplore_WithCategory()
  {
    await Page.GotoAsync(BaseUrl + "/explore");
    await Page.WaitForSelectorAsync(".entities-filter-select");

    await Page.SelectOptionAsync(".entities-filter-select", "characters");

    await Page.WaitForSelectorAsync(".entities-list .entity-link");
    var links = Page.Locator(".entity-link");
    var count = await links.CountAsync();
    Assert.That(count, Is.GreaterThan(0), "Expected at least one character entity.");

    await links.Nth(0).ClickAsync();

    await Page.WaitForSelectorAsync(".entity-hero-heading");

    await Page.ClickAsync(".back-link");

    await Page.WaitForURLAsync("**/explore**");

    var url = Page.Url;
    Assert.That(url, Does.Contain("/explore"));
    Assert.That(url, Does.Contain("category=characters"));
  }

  [Test]
  public async Task Detail_InvalidName_ShowsErrorState()
  {
    const string bogusName = "WRONG_NAME";

    await Page.GotoAsync($"{BaseUrl}/droids/{Uri.EscapeDataString(bogusName)}");

    await Page.WaitForSelectorAsync(".entity-detail-root");

    await Page.WaitForSelectorAsync(".error-state");

    var errorText = await Page.TextContentAsync(".error-state");
    Assert.That(errorText, Does.Contain("No entity found").IgnoreCase);
  }
}

