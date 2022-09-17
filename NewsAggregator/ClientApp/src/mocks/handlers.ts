import { RequestHandler, rest } from "msw";
import { GetNewsResponse } from "../openapi/backendComponents";
import { NewsMock } from "./mocks";

export const handlerPath = {
  getWeatherForecast: "*/api/News" as const,
};

export const createGetNewsHandler = (
  response: GetNewsResponse
): RequestHandler => {
  return rest.get(handlerPath.getWeatherForecast, (req, res, ctx) => {
    return res(ctx.status(200), ctx.json<GetNewsResponse>(response));
  });
};

export const handlers = [
  createGetNewsHandler(NewsMock.buildList(3)),
];
