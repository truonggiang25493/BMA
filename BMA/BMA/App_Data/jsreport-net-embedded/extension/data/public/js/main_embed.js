define('data.template.model',["app", "core/basicModel", "underscore", "jquery"], function (app, ModelBase, _, $) {
   
    return ModelBase.extend({
        
        fetch: function (options) {
            var self = this;

            function processItems(items) {
                self.items = items;

                var data = self.templateModel.get("data");

                if (!data) {
                    data = {};

                    //back compatibility
                    if (self.templateModel.get("dataItemId")) {
                        data.shortid = self.templateModel.get("dataItemId");
                    }

                    self.templateModel.set("data", data);
                }
                var custom;
                if (app.options.data.allowCustom) {
                    custom = {name: "- custom -", shortid: "custom", dataJson: data.dataJson};
                    self.items.unshift(custom);
                }

                var empty = { name: "- not selected -", shortid: null };
                self.items.unshift(empty);

                if (!data.dataJson && !data.shortid)
                    self.set(custom || empty, { silent: true });

                if (data.shortid) {
                    self.set(_.findWhere(self.items, {shortid: data.shortid}), {silent: true});
                }

                if (data.dataJson)
                    self.set(custom || empty, { silent: true });

                return $.Deferred().resolve();
            }

            if (app.options.data.allowSelection) {
                return app.dataProvider.get("odata/data?$select=name,shortid").then(processItems);
            } else {
                return processItems([]);
            }
        },

        setTemplate: function (templateModel) {
            this.templateModel = templateModel;
            this.listenTo(templateModel, "api-overrides", this.apiOverride);
        },
        
        apiOverride: function(req) {
            req.template.data = { "shortid": this.get("shortid") || "...", "dataJson": "{\'foo\' : \'...\' }" };
        },

        initialize: function () {
            var self = this;

            this.listenTo(this, "change:shortid", function() {
                self.templateModel.get("data").shortid = self.get("shortid") !== "custom" ? self.get("shortid") : undefined;
                self.templateModel.get("data").dataJson = self.get("shortid") === "custom" ? self.get("dataJson") : undefined;
                self.set(_.findWhere(self.items, { shortid: self.get("shortid")}));
            });

            this.listenTo(this, "change:dataJson", function() {
                if (self.get("shortid") === "custom") {
                    self.templateModel.get("data").dataJson = self.get("dataJson");
                    _.findWhere(self.items, { shortid: "custom" }).dataJson = self.get("dataJson");
                }
            });
        }
    });
});
/*! 
 * Copyright(c) 2014 Jan Blaha 
 */

define(["jquery", "underscore", "app", "marionette", "backbone", "core/view.base", "core/listenerCollection", "data.template.model",
        "core/aceBinder"],
    function ($, _, app, Marionette, Backbone, ViewBase, ListenerCollection, TemplateStandardModel, aceBinder) {

        app.options.data = $.extend(app.options.data, { allowSelection: false, allowCustom: true}, app.options.data);

        return app.module("data", function (module) {

            app.options.data = app.options.data || { allowChoosing: true};

            var TemplateView = ViewBase.extend({
                template: "embed-data-template-extension",

                initialize: function () {
                    _.bindAll(this, "getItems");
                    var self = this;

                    this.listenTo(this.model, "change:shortid", function() {
                        self.contentEditor.setOptions({
                            readOnly: self.model.get("shortid") !== "custom" && app.options.data.allowSelection
                        });
                    });

                    this.listenTo(this, "animation-done", function() {
                        self.fixAcePosition();
                    });
                },

                getItems: function () {
                    return this.model.items;
                },

                onDomRefresh: function () {

                    this.contentEditor = ace.edit("contentArea");
                    this.contentEditor.setTheme("ace/theme/chrome");
                    this.contentEditor.getSession().setMode("ace/mode/json");
                    this.contentEditor.setOptions({
                        enableBasicAutocompletion: true,
                        enableSnippets: true,
                        readOnly: this.model.get("shortid") !== "custom" && app.options.data.allowChoosing
                    });

                    aceBinder(this.model, "dataJson", this.contentEditor);

                    this.fixAcePosition();
                },

                fixAcePosition: function() {
                    var top = $("#contentWrap").position().top;
                    $("#contentArea").css("margin-top", top);
                }


            });

            app.on("extensions-menu-render", function (context) {
                context.result += "<li><a id='dataMenuCommand' title='sample data definition'><i data-position='right' data-intro='Define sample input data' class='fa fa-file'></i></a></li>";

                context.on("after-render", function ($el) {
                    $($el).find("#dataMenuCommand").click(function () {
                        var model = new TemplateStandardModel();
                        model.setTemplate(context.template);

                        model.fetch().then(function () {
                                var view = new TemplateView({model: model});
                                context.region.show(view, "data");
                        });
                    });
                });
            });
        });
    });