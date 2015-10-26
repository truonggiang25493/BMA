/*!
 * Copyright(c) 2014 Jan Blaha 
 */

module.exports = function(reporter, definition) {

    reporter.options.pingTimeout = reporter.options.pingTimeout;

    var initialized = false;

	reporter.on("express-configure", function(app) {

        if (initialized)
            return;

        initialized = true;

		var lastTimePing = new Date();
		
		app.get("/api/alive", function(req, res, next) {
            lastTimePing = new Date();
            res.send("ok");
		});

		app.post("/api/kill", function(req, res, next) {
	        process.nextTick(function() {
			    process.exit(1);
            });
			res.send("ok");
		});
		
		if (reporter.options.pingTimeout) {
			setInterval(function(){
					if (lastTimePing.getTime() < ((new Date().getTime()) - (reporter.options.pingTimeout * 1000)))
						process.exit(1);
			}, 500);
		}
    }); 	
};