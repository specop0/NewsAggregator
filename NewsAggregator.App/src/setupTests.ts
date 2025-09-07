import "@testing-library/jest-dom";
import { server } from "./mocks/server";

window.configuration = {
    backendUrl: "http://localhost/backendUrl",
    openIdConnect: {
        authority: "https://idp.local",
        clientId: "test",
        redirectUri: "http://localhost"
    }
};

beforeAll(() => server.listen());

afterEach(() => server.resetHandlers());

afterAll(() => server.close());
