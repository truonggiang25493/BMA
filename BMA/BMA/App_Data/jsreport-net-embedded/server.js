var options = {};
options.rootDirectory = require("path").join(__dirname);

require("./lib/bootstrapper.js")(options).start();

