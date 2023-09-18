import { withSwagger } from 'next-swagger-doc';

const swaggerHandler = withSwagger({
  definition: {
    openapi: '2.0',
    info: {
      title: 'NextJS Swagger',
      description: "This is the OpenAPI Document on Azure Functions",
      version: '0.1.0',
    },
    host: 'localhost:9080',
    schemes: ['http'],
  },
  apiFolder: 'src/app/api',
});

export default swaggerHandler();
 