/** @type {import('next').NextConfig} */

// const withTM = require('next-transpile-modules')(['adaptivecards', 'swiper']);

const nextConfig = {
    experimental: {
        // appDir: true,        
        runtime: 'experimental-edge',
        serverActions: true,
    },

    async headers() {
        return [{
                source: '/:path*',
                headers: [{
                    key: 'Access-Control-Allow-Origin',
                    value: '*',
                }, ],
            },
            {
                source: '/api/:path*',
                headers: [{
                    key: 'Access-Control-Allow-Origin',
                    value: '*',
                }, ],
            },
            {
                // matching all API routes
                source: "/api/:path*",
                headers: [
                    { key: "Access-Control-Allow-Credentials", value: "true" },
                    { key: "Access-Control-Allow-Origin", value: "*" }, // replace this your actual origin
                    { key: "Access-Control-Allow-Methods", value: "GET,DELETE,PATCH,POST,PUT" },
                    { key: "Access-Control-Allow-Headers", value: "X-CSRF-Token, X-Requested-With, Accept, Accept-Version, Content-Length, Content-MD5, Content-Type, Date, X-Api-Version" },
                ]
            },
            {
                // matching all API routes
                source: "/.well-known/:path*",
                headers: [
                    { key: "Access-Control-Allow-Credentials", value: "true" },
                    { key: "Access-Control-Allow-Origin", value: "*" }, // replace this your actual origin
                    { key: "Access-Control-Allow-Methods", value: "GET,DELETE,PATCH,POST,PUT" },
                    { key: "Access-Control-Allow-Headers", value: "X-CSRF-Token, X-Requested-With, Accept, Accept-Version, Content-Length, Content-MD5, Content-Type, Date, X-Api-Version" },
                ]
            }
        ]
    },

    // webpack:  (config, { isServer }) => {
    //     if (!isServer) {
    //       config.resolve.fallback.fs = false;
    //     }

    //     config.module.rules.push({
    //       test: /\.m?js/,
    //       resolve: {
    //         fullySpecified: false,
    //       },
    //     });

    //     return config;
    //   }      
}

module.exports = nextConfig;