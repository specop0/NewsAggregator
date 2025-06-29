import * as Factory from "factory.ts";
import { faker } from "@faker-js/faker";
import { News } from "../openapi/backendSchemas";

export const NewsMock = Factory.Sync.makeFactory<News>({
  title: Factory.each(() => faker.company.name()),
  summary: Factory.each(() => faker.lorem.sentences()),
  url: Factory.each(() => faker.internet.url()),
  imageUrl: Factory.each(() => faker.image.avatar()),
  imageData: Factory.each(() => faker.image.dataUri()),
});
