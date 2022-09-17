import {
  generateFetchers,
  generateReactQueryComponents,
  generateSchemaTypes,
} from "@openapi-codegen/typescript";
import { defineConfig } from "@openapi-codegen/cli";
import { ConfigBase } from "@openapi-codegen/typescript/lib/generators/types";
export default defineConfig({
  backend: {
    from: {
      relativePath: "./src/openapi/backend.json",
      source: "file",
    },
    outputDir: "/src/openapi",
    to: async (context) => {
      const backendConfig: ConfigBase = { filenamePrefix: "backend" };
      const { schemasFiles } = await generateSchemaTypes(
        context,
        backendConfig
      );
      await generateFetchers(context, {
        ...backendConfig,
        schemasFiles,
      });
      await generateReactQueryComponents(context, {
        ...backendConfig,
        schemasFiles,
      });
    },
  },
});
