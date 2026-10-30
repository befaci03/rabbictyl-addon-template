// Types for the browser-side add-on API exposed by the panel.
// Copy this file into your TypeScript project (or reference it) to get
// autocompletion for window.RabbiCtyl.

export interface AddonInstance {
  readonly id: string;

  /** First element with `data-addon-slot="{name}"`, if the page exposes one. */
  slot(name: string): HTMLElement | null;

  /** Fetch an endpoint owned by this add-on: `/addons/{id}/api/{path}`. */
  api(path: string, init?: RequestInit): Promise<Response>;

  /** Fetch JSON from one of the add-on's endpoints. */
  json<T>(path: string, init?: RequestInit): Promise<T>;

  /** Show a short message in the panel's toast stack. */
  toast(message: string): void;

  /** Run once the document is parsed. */
  ready(callback: () => void): void;
}

export interface AddonApi {
  forAddon(id: string): AddonInstance;
  slot(name: string): HTMLElement | null;
  toast(message: string): void;
}

declare global {
  interface Window {
    RabbiCtyl: AddonApi;
  }
}

export {};
