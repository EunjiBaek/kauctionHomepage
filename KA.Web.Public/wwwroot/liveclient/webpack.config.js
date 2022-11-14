const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const ReactRefreshWebpackPlugin = require("@pmmmwh/react-refresh-webpack-plugin");


const webpackDevClientEntry = require.resolve(
  "react-dev-utils/webpackHotDevClient"
);

const reactRefreshOverlayEntry = require.resolve(
  "react-dev-utils/refreshOverlayInterop"
);

module.exports = {
    entry: './src/index.js',
    output: {
        path: path.join(__dirname, '/dist'),
        filename: 'index_bundle.js'
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                exclude: /node_module/,
                use: {
                    loader: 'babel-loader'
                }
            },
            {
                test:/\.css$/,
                use:['style-loader','css-loader'],
            },
            {
                test: /\.(svg|png|jpe?g|gif)$/i,
                loader: 'file-loader',
                options: {
                    name: "[name].[ext]",
                    outputhPath: "imgs"
                }
            }
        ]
    },
    plugins: [
        new HtmlWebpackPlugin({
            template: 'public/index.html',
        }),
        new ReactRefreshWebpackPlugin({
            overlay: {
                entry: webpackDevClientEntry,
                module: reactRefreshOverlayEntry
            }
        })
    ],
}





