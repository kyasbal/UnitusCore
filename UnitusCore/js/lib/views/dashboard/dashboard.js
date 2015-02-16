var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone', 'templates/dashboard/dashboard', 'views/dashboard/header', 'views/dashboard/panel', 'models/user', 'models/admin_panel'], function($, Backbone, template, HeaderView, PanelView, User, AdminPanel) {
  var DashboadView;
  return DashboadView = (function(_super) {
    __extends(DashboadView, _super);

    function DashboadView() {
      return DashboadView.__super__.constructor.apply(this, arguments);
    }

    DashboadView.prototype.initialize = function(option) {
      $("[data-js=loading]").fadeOut();
      this.user = new User();
      return $.ajax({
        url: 'https://core.unitus-ac.com/Dashboard',
        data: {
          validationToken: 'abc'
        },
        type: 'GET',
        success: (function(_this) {
          return function(msg) {
            var data;
            data = msg.Content;
            _this.user.set({
              name: data.Name
            });
            _this.user.set({
              mail: data.UserName
            });
            _this.user.set({
              avatar: data.AvatarUri
            });
            _this.user.set({
              isAdmin: data.IsAdministrator
            });
            _this.user.set({
              circles: data.CircleBelonging
            });
            if (_this.user.get("isAdmin")) {
              _this.admin_panel = new AdminPanel();
            }
            _this.renderDashboard();
            new HeaderView({
              el: $("[data-js=header]"),
              user: _this.user,
              admin_panel: _this.admin_panel
            });
            new PanelView({
              el: $("[data-js=panel]"),
              user: _this.user,
              admin_panel: _this.admin_panel
            });
            return _this.$el.fadeIn();
          };
        })(this),
        error: function(msg) {
          return console.log(msg);
        }
      });
    };

    DashboadView.prototype.renderDashboard = function() {
      return this.$el.html(template({
        user: this.user
      }));
    };

    return DashboadView;

  })(Backbone.View);
});
