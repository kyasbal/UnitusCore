var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone', 'templates/dashboard/dashboard', 'views/dashboard/header', 'views/dashboard/panel', 'models/admin_panel'], function($, Backbone, template, HeaderView, PanelView, AdminPanel) {
  var DashboadView;
  return DashboadView = (function(_super) {
    __extends(DashboadView, _super);

    function DashboadView() {
      return DashboadView.__super__.constructor.apply(this, arguments);
    }

    DashboadView.prototype.initialize = function(option) {
      this.dashboard = option.dashboard;
      this.circles = option.circles;
      $.ajaxSetup({
        xhrFields: {
          withCredentials: true
        },
        dataType: 'json',
        data: {
          ValidationToken: 'abc'
        }
      });
      return $.ajax({
        url: 'https://core.unitus-ac.com/Dashboard',
        type: 'GET',
        success: (function(_this) {
          return function(msg) {
            var data;
            console.log(msg);
            $("[data-js=loading]").fadeOut();
            data = msg.Content;
            _this.dashboard.set({
              Name: data.Name
            });
            _this.dashboard.set({
              UserName: data.UserName
            });
            _this.dashboard.set({
              AvatarUri: data.AvatarUri
            });
            _this.dashboard.set({
              IsAdministrator: data.IsAdministrator
            });
            _this.dashboard.set({
              CircleBelonging: data.CircleBelonging
            });
            _this.dashboard.set({
              Profile: data.Profile
            });
            _this.dashboard.set({
              GithubAssociation: data.Profile.GithubProfie.AssociationEnabled
            });
            if (_this.dashboard.get("IsAdministrator")) {
              _this.admin_panel = new AdminPanel();
            }
            _this.renderDashboard();
            new HeaderView({
              el: $("[data-js=header]"),
              dashboard: _this.dashboard,
              admin_panel: _this.admin_panel
            });
            new PanelView({
              el: $("[data-js=panel]"),
              dashboard: _this.dashboard,
              admin_panel: _this.admin_panel,
              circles: _this.circles
            });
            return _this.$el.fadeIn();
          };
        })(this),
        error: function(XMLHttpRequest, textStatus) {
          console.log(XMLHttpRequest);
          console.log(textStatus);
          if (textStatus === "error" || XMLHttpRequest.ErrorMessage === "Unauthorized API Access") {
            return location.assign("https://core.unitus-ac.com/Account/Login");
          }
        }
      });
    };

    DashboadView.prototype.renderDashboard = function() {
      return this.$el.html(template({
        dashboard: this.Dashboard
      }));
    };

    return DashboadView;

  })(Backbone.View);
});
