define('scripts.template.standard.model',["app", "core/basicModel", "underscore"], function (app, ModelBase, _) {
   
    return ModelBase.extend({
        
        fetch: function (options) {
            var self = this;

            function processItems(items) {
                self.items = items;

                var script = self.templateModel.get("script");

                if (!script) {
                    script = {};

                    //back compatibility
                    if (self.templateModel.get("scriptId")) {
                        script.shortid = self.templateModel.get("scriptId");
                    }

                    self.templateModel.set("script", script, { silent: true});
                }

                var custom;
                if (app.options.scripts.allowCustom) {
                    custom = {name: "- custom -", shortid: "custom", content: script.content};
                    self.items.unshift(custom);
                }

                var empty = { name: "- not selected -", shortid: null };
                self.items.unshift(empty);

                if (!script.content && !script.shortid)
                    self.set(custom || empty, { silent: true });

                if (script.shortid)
                    self.set(_.findWhere(items, { shortid: script.shortid }), { silent: true });

                if (script.content)
                    self.set(custom || empty, { silent: true });

                return options.success();
            }

            if (app.options.scripts.allowSelection) {
                return app.dataProvider.get("odata/scripts").then(processItems);
            } else {
                processItems([]);
            }
        },

        setTemplate: function (templateModel) {
            this.templateModel = templateModel;
            this.listenTo(templateModel, "api-overrides", this.apiOverride);
        },
        
        apiOverride: function(req) {
             req.template.script = { shortid: this.get("shortid") || "...", content: '....' };
        },

        newCustomScript: function() {

        },
 
        initialize: function () {
            var self = this;
            this.listenTo(this, "change:shortid", function() {
                self.templateModel.get("script").shortid = self.get("shortid") !== "custom" ? self.get("shortid") : undefined;
                self.templateModel.get("script").content = self.get("shortid") === "custom" ? self.get("content") : undefined;
                self.set(_.findWhere(self.items, { shortid: self.get("shortid")}));
            });

            this.listenTo(this, "change:content", function() {
                if (self.get("shortid") === "custom") {
                    self.templateModel.get("script").content = self.get("content");
                    _.findWhere(self.items, { shortid: "custom" }).content = self.get("content");
                }
            });
        }
    });
});
define('scripts.template.standard.view',["app", "underscore", "marionette", "core/view.base", "core/utils"], function(app, _, Marionette, ViewBase, Utils) {
    return ViewBase.extend({
        tagName: "li",
        template: "scripts-template-standard",
         
        initialize: function() {
            _.bindAll(this, "isFilled", "getItems", "getItemsLength");
        },

        isFilled: function() {
            return this.model.get("shortid") || this.model.get("content");
        },
        
        getItems: function () {
            return this.model.items;
        },
        
        getItemsLength: function () {
            return this.model.items.length;
        },
        
        onClose: function() {
            this.model.templateModel.unbind("api-overrides", this.model.apiOverride, this.model);
        }
    });
});
/*! 
 * Copyright(c) 2014 Jan Blaha 
 */

define(["jquery", "underscore", "app", "marionette", "backbone", "core/view.base", "core/listenerCollection", "scripts.template.standard.model", "scripts.template.standard.view",
        "core/aceBinder"],
    function ($, _, app, Marionette, Backbone, ViewBase, ListenerCollection, TemplateStandardModel, TemplateStandardView,
              aceBinder) {

        app.options.scripts = $.extend(app.options.scripts, { allowSelection: false, allowCustom: true}, app.options.scripts);

        return app.module("scripts", function (module) {

            var TemplateView = ViewBase.extend({
                template: "embed-scripts-template-extension",

                initialize: function() {
                    _.bindAll(this, "getItems");
                    var self = this;
                    this.listenTo(this.model, "change:shortid", function() {
                        self.contentEditor.setOptions({
                            readOnly: self.model.get("shortid") !== "custom" && app.options.scripts.allowSelection
                        });
                    });

                    this.listenTo(this, "animation-done", function() {
                        self.fixAcePosition();
                    });
                },

                getItems: function () {
                    return this.model.items;
                },

                onDomRefresh: function() {
                    this.contentEditor = ace.edit("contentArea");
                    this.contentEditor.setTheme("ace/theme/chrome");
                    this.contentEditor.getSession().setMode("ace/mode/javascript");
                    this.contentEditor.setOptions({
                        enableBasicAutocompletion: true,
                        enableSnippets: true,
                        readOnly: this.model.get("shortid") !== "custom" && app.options.scripts.allowChoosing
                    });

                    aceBinder(this.model, "content", this.contentEditor);

                    this.fixAcePosition();
                },

                fixAcePosition: function() {
                    var top = $("#contentWrap").position().top;
                    $("#contentArea").css("margin-top", top);
                }
            });


            app.on("extensions-menu-render", function(context) {
                context.result += "<li><a id='scriptsMenuCommand' title='define custom script (mostly loading data)'><i data-position='right' data-intro='Use custom script to load data or modify inputs' class='fa fa-cloud-download'></i></a></li>";

                context.on("after-render", function($el) {
                    $($el).find("#scriptsMenuCommand").click(function() {
                        var model = new TemplateStandardModel();
                        model.setTemplate(context.template);

                        model.fetch({ success: function () {
                            var view = new TemplateView({ model: model});
                            context.region.show(view, "scripts");
                        }});
                    });
                });
            });
        });
    });