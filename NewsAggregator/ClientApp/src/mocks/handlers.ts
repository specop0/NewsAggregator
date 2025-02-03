import { RequestHandler, http, HttpResponse } from "msw";
import { GetNewsResponse } from "../openapi/backendComponents";
import { NewsMock } from "./mocks";

export const handlerPath = {
  getNews: "*/api/News" as const,
};

export const createGetNewsHandler = (
  response: GetNewsResponse
): RequestHandler => {
  return http.post(handlerPath.getNews, (s) => {
    return HttpResponse.json<GetNewsResponse>(response, {status: 200});
  });
};

export const handlers = [
  createGetNewsHandler(NewsMock.buildList(3)),
];
