# rabbictyl-addon-template

Starting point for a RabbiCtyl add-on.

An add-on is a **self-contained folder**: the panel never contains code for it.
Everything the feature needs — settings page, storage, endpoints, sign-in —
lives here. The panel only exposes generic extension points.

## Layout

```
manifest.json            describes the add-on; "id" must match the folder name
scripts/*.ts             TypeScript loaded on panel pages (JavaScript is rejected)
assets/                  files exposed through manifest "assets" ids
TemplateAddon/           optional C# project, built into lib/
lib/                     compiled add-on assembly (git-ignored)
```

Rename the folder so it matches `id` (for example `my_addon/`) before copying it
into the panel's `addons/` directory.

## Manifest

```json
{
  "id": "my_addon",
  "name": "My Add-on",
  "version": "1.0.0",
  "description": "What it does.",
  "author": "You",
  "enabled": true,
  "assembly": "lib/MyAddon.dll",
  "entry": "MyAddon.MyAddon",
  "scripts": ["scripts/main.ts"],
  "assets": { "icon": "assets/icon.svg" }
}
```

`enabled` is the default; an administrator can toggle it from
Administration → Addons. C# add-ons start and stop with the panel, while browser
scripts load on the next page refresh.

## C# extension points

All types live in `RabbiCtyl.Addons.Abstractions`.

- `IAddon` — entry point. `InitializeAsync` receives an `AddonContext` with
  `Services` (the panel container), `Directory` (your folder) and
  `StorageDirectory` (a writable directory just for you).
- `IAddonAdminPage` — adds a tab in Administration at `/admin/addons/{Slug}`
  and returns the page HTML. Forms must include
  `<input type="hidden" name="__RequestVerificationToken" value="…">` using
  `AddonRequest.CsrfToken`.
- `IAddonEndpoint` — signed-in (and anonymous) endpoints at
  `/addons/{id}/api/{path}`. You enforce your own access rules; `AddonRequest`
  carries `UserId`, `IsAdmin` and `Username`.
- `IAddonUserGuard` — protect accounts your add-on manages. The panel then
  refuses to edit or delete them and shows your reason.
- `IAddonAuthentication` — resolve it from `Services` to sign a user in or out
  from your own authentication flow.

Store files under `AddonContext.StorageDirectory`; never write inside the add-on
folder, which may be read-only.

## Browser scripts

Add-on scripts run on every signed-in page and on the sign-in screens. Fill the
generic slots the panel exposes:

```html
<div data-addon-slot="auth.providers"></div>
<div data-addon-slot="server.banner" data-server-uuid="…"></div>
```

## Build

```bash
cd TemplateAddon
dotnet build -p:RabbiCtylPanelDir=/path/to/rabbictyl-panel
```

The project writes `lib/RabbiCtyl.Addon.Template.dll` next to the manifest.
`RabbiCtylPanelDir` defaults to `../../rabbictyl-panel-`.

## Install

1. Copy this folder into the panel's `addons/` directory and rename it to the
   manifest `id`.
2. Restart the panel (C# add-ons load at startup).
3. Enable it in Administration → Addons.
