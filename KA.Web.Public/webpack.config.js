const path = require('path');
const root_js = "./wwwroot/js/";
const root_css = "./wwwroot/css/";

module.exports = {
    mode: 'production', // or "development" or "none" or "production"
    optimization: {
        minimize: true
    },
    entry: {
        polyfill: 'core-js',
        runtime: 'regenerator-runtime',
        "live.app": [            
            './wwwroot/js/ka.live.js'
        ],
        auction: './wwwroot/js/ka.auction.js',
        sample: ['./wwwroot/js/sample.js']
    },
    output: {        
        filename: 'ka.[name].js',
        path: path.resolve(__dirname, './wwwroot/bundle/js/'),
        //clean:true,
      },
    target: ['web', 'es5'],
    module: {
        rules: [
          {
            test: /\.?js$/,            
            include: [
              path.resolve(__dirname, root_js)
            ],
            exclude : /node_modules/,
            use: {
              loader: "babel-loader",
              options: {                
                plugins: ['@babel/plugin-proposal-class-properties','@babel/plugin-proposal-object-rest-spread']
              }
            }
          }  
        ]
      },
      devtool: 'source-map'
}
