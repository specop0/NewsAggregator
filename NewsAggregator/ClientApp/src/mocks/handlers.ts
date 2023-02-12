import { RequestHandler, rest } from "msw";
import { GetNewsResponse } from "../openapi/backendComponents";
import { NewsMock } from "./mocks";

export const handlerPath = {
  getNews: "*/api/News" as const,
};

export const createGetNewsHandler = (
  response: GetNewsResponse
): RequestHandler => {
  return rest.get(handlerPath.getNews, (req, res, ctx) => {
    return res(ctx.status(200), ctx.json<GetNewsResponse>(response));
  });
};

export const handlers = [
  createGetNewsHandler(NewsMock.buildList(3)),
];
