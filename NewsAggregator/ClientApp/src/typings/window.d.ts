export {};

declare global {
  interface Window {
    configuration: {
      backendUrl: string;
    };
  }
}
