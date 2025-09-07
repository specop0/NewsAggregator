export { };

declare global {
  interface Window {
    configuration: {
      backendUrl: string;
      openIdConnect: {
        authority: string,
        clientId: string,
        redirectUri: string,
      }
    };
  }
}
