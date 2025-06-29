import { delay, RequestHandler, http, HttpResponse } from "msw";
import { GetNewsResponse } from "../openapi/backendSchemas";
import { NewsMock } from "./mocks";

export const handlerPath = {
  getNews: "*/api/News" as const,
};

export const createGetNewsHandler = (
  response: GetNewsResponse
): RequestHandler => {
  return http.post(handlerPath.getNews, async () => {
    await delay();
    return HttpResponse.json<GetNewsResponse>(response, { status: 200 });
  });
};

export const handlers = [
  createGetNewsHandler({ items: NewsMock.buildList(3) }),
];
