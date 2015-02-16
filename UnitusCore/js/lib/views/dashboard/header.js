var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone', 'templates/dashboard/header'], function($, Backbone, HeaderTemplate) {
  var HeaderView;
  return HeaderView = (function(_super) {
    __extends(HeaderView, _super);

    function HeaderView() {
      return HeaderView.__super__.constructor.apply(this, arguments);
    }

    HeaderView.prototype.initialize = function(option) {
      this.user = option.user;
      this.admin_panel = option.admin_panel;
      return this.renderTemplate();
    };

    HeaderView.prototype.events = {
      "click [data-js=dropdown]": "dropdownToggle",
      "click [data-js=adminToggle]": "adminOpen"
    };

    HeaderView.prototype.renderTemplate = function() {
      return this.$el.html(HeaderTemplate({
        user: this.user
      }));
    };

    HeaderView.prototype.dropdownToggle = function(event) {};

    HeaderView.prototype.adminOpen = function(e) {
      e.preventDefault();
      e.stopPropagation();
      return this.admin_panel.set({
        isOpen: true
      });
    };

    return HeaderView;

  })(Backbone.View);
});
