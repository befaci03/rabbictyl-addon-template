using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using RabbiCtyl.Addons.Abstractions;

namespace RabbiCtyl.Addon.Template;

/// <summary>
/// Example add-on. Everything it needs lives in this folder: the settings page,
/// an endpoint and its storage directory. The panel only provides the host.
/// </summary>
public sealed class TemplateAddon : IAddon, IAddonAdminPage, IAddonEndpoint
{
    public string Id => "template_addon";

    public string Slug => "template";

    public string Label => "Template";

    public Task InitializeAsync(AddonContext context, CancellationToken cancellationToken = default)
    {
        context.Logger.LogInformation(
            "Template add-on started; storage lives in {Storage}.",
            context.StorageDirectory);

        return Task.CompletedTask;
    }

    public Task<AddonResponse> HandleAsync(
        AddonRequest request,
        CancellationToken cancellationToken = default)
    {
        // Public endpoint: /addons/template_addon/api/ping
        if (string.Equals(request.Path, "ping", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(
                AddonResponse.File(Encoding.UTF8.GetBytes("pong"), "text/plain"));
        }

        var csrf = WebUtility.HtmlEncode(request.CsrfToken ?? string.Empty);
        var name = request.Form.TryGetValue("name", out var submitted) && submitted.Trim().Length > 0
            ? submitted.Trim()
            : "world";

        var html =
            $"""
            <section class="card">
                <div class="card__header">
                    <div>
                        <h2 class="card__title">Template add-on</h2>
                        <p class="card__subtitle">This page is rendered by the add-on itself.</p>
                    </div>
                </div>

                <form method="post" action="/admin/addons/template" class="form">
                    <input type="hidden" name="__RequestVerificationToken" value="{csrf}" />
                    <div class="form__group">
                        <label class="form__label" for="template-name">Name</label>
                        <input class="form__input" id="template-name" name="name" value="{WebUtility.HtmlEncode(name)}" />
                    </div>
                    <div class="admin-create__actions">
                        <button type="submit" class="button button--primary">Send</button>
                    </div>
                </form>

                <p>Hello, {WebUtility.HtmlEncode(name)}.</p>
            </section>
            """;

        return Task.FromResult(AddonResponse.Page(html));
    }
}
