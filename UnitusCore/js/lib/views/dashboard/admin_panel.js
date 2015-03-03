var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone', 'templates/dashboard/admin_panel', 'views/admin/new_circle'], function($, Backbone, AdminTemplate, AdminNewCircle) {
  var AdminPanelView;
  return AdminPanelView = (function(_super) {
    __extends(AdminPanelView, _super);

    function AdminPanelView() {
      return AdminPanelView.__super__.constructor.apply(this, arguments);
    }

    AdminPanelView.prototype.initialize = function(option) {
      var sendData;
      this.circles = option.circles;
      this.dashboard = option.dashboard;
      sendData = {
        count: 40
      };
      $.ajax({
        type: "GET",
        url: "https://core.unitus-ac.com/Person",
        data: sendData,
        success: function(msg) {
          return $.each(msg.Content.Persons, function() {
            var text;
            text = '';
            text += '<tr>';
            text += '<td class="name name_w">' + this.Name + '<i data-js="deleteAccount" class="fa fa-times"></i></td>';
            text += '<td class="author author_w">' + "閲覧者" + '</td>';
            text += '<td class="number number_w">' + this.Grade + '</td>';
            text += '<td class="university university_w">' + this.BelongedSchool + '</td>';
            text += '<td class="mail mail_w">' + this.UserName + '<i class="fa fa-clipboard" data-js="copyMail" data-clipboard-text="' + this.UserName + '"></i></td>';
            text += '</tr>';
            return $("[data-js=userList]").append(text);
          });
        },
        error: function(msg) {
          return console.log(msg);
        }
      });
      this.admin_panel = option.admin_panel;
      this.admin_panel.on("change:isOpen", (function(_this) {
        return function() {
          console.log("open");
          return _this.$el.toggleClass("hidden_panel_l");
        };
      })(this));
      this.renderAdminPanel();
      return new AdminNewCircle({
        el: $("[data-js=adminNewCircle]"),
        circles: this.circles,
        dashboard: this.dashboard
      });
    };

    AdminPanelView.prototype.events = {
      "click [data-js=close_admin]": "closePanel",
      "focus #adminNewCircle input": "watchChangeValue",
      "focus #adminNewCircle textarea": "watchChangeValue",
      "click [data-js=createCircle]": "createCircle"
    };

    AdminPanelView.prototype.renderAdminPanel = function() {
      return this.$el.html(AdminTemplate());
    };

    AdminPanelView.prototype.closePanel = function() {
      return this.admin_panel.set({
        isOpen: false
      });
    };

    return AdminPanelView;

  })(Backbone.View);
});
