const path = require("path");
const webpack = require('webpack');

module.exports = {
    module: {
        rules: [
            {
                test: /.(js|jsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: "babel-loader"
                }
            }
        ]
    },
    node: {
        global: true
    },
    output: {
        path: path.resolve(__dirname, '../wwwroot/js'),
        filename: "recover_lib.js",
        library: "RecoverLib"
    },
    resolve: {
        extensions: ['.js', '.jsx', '.ts', '.tsx'],
    },
    plugins: [
        new webpack.ProvidePlugin({
            Buffer: ['buffer', 'Buffer'],
        }),
        new webpack.ProvidePlugin({
            process: 'process/browser',
        }),
    ],
};