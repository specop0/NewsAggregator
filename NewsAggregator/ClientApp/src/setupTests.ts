import "@testing-library/jest-dom";
import { server } from "./mocks/server";

window.configuration = { backendUrl: "http://localhost/backendUrl" };

beforeAll(() => server.listen());

afterEach(() => server.resetHandlers());

afterAll(() => server.close());
