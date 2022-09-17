import React from "react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { render as reactRender } from "@testing-library/react";
import { RequestHandler } from "msw";
import { server } from "../mocks/server";

export const render = (
  ui: React.ReactElement,
  options?: {
    mocks?: RequestHandler[];
  }
) => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry(failureCount, error) {
          return false;
        },
      },
    },
    logger: {
      log(...args: any[]) {},
      warn(...args: any[]) {},
      error(...args: any[]) {},
    },
  });

  server.resetHandlers(...(options?.mocks ?? []));

  reactRender(
    <QueryClientProvider client={queryClient}>{ui}</QueryClientProvider>
  );
};
