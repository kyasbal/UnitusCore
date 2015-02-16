var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone', 'templates/dashboard/user_panel'], function($, Backbone, UserTemplate) {
  var UserPanelView;
  return UserPanelView = (function(_super) {
    __extends(UserPanelView, _super);

    function UserPanelView() {
      return UserPanelView.__super__.constructor.apply(this, arguments);
    }

    UserPanelView.prototype.initialize = function(option) {
      this.renderUserPanel();
      return console.log(this.$el.html());
    };

    UserPanelView.prototype.renderUserPanel = function() {
      return this.$el.html(UserTemplate());
    };

    return UserPanelView;

  })(Backbone.View);
});
