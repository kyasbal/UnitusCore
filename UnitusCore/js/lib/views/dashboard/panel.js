var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone', 'views/dashboard/user_panel', 'views/dashboard/admin_panel'], function($, Backbone, UserPanelView, AdminPanelView) {
  var PanelView;
  return PanelView = (function(_super) {
    __extends(PanelView, _super);

    function PanelView() {
      return PanelView.__super__.constructor.apply(this, arguments);
    }

    PanelView.prototype.initialize = function(option) {
      this.circles = option.circles;
      this.dashboard = option.dashboard;
      this.admin_panel = option.admin_panel;
      return this.renderDashboard();
    };

    PanelView.prototype.renderDashboard = function() {
      new UserPanelView({
        el: $("[data-js=basic]"),
        dashboard: this.dashboard,
        circles: this.circles
      });
      if (this.dashboard.get("IsAdministrator")) {
        return new AdminPanelView({
          el: $("[data-js=admin]"),
          admin_panel: this.admin_panel,
          circles: this.circles,
          dashboard: this.dashboard
        });
      }
    };

    return PanelView;

  })(Backbone.View);
});
