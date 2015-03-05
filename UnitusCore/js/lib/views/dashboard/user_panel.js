var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; },
  __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone', 'templates/dashboard/user_panel', 'views/dashboard/achivement', 'models/circle', 'views/dashboard/profilebar', 'collections/achivements'], function($, Backbone, UserTemplate, AchivementView, Circle, ProfilebarView, Achivements) {
  var UserPanelView;
  return UserPanelView = (function(_super) {
    __extends(UserPanelView, _super);

    function UserPanelView() {
      this.deleteCircle = __bind(this.deleteCircle, this);
      this.renderCircleList = __bind(this.renderCircleList, this);
      return UserPanelView.__super__.constructor.apply(this, arguments);
    }

    UserPanelView.prototype.initialize = function(option) {
      this.dashboard = option.dashboard;
      this.circles = option.circles;
      this.belongingCircles = this.dashboard.get("CircleBelonging");
      this.achivements = new Achivements();
      this.notyHelper = new NotyHelper();
      this.renderUserPanel();
      new ProfilebarView({
        el: '[data-js="myProfile"]',
        dashboard: this.dashboard,
        achivements: this.achivements
      });
      this.renderCircleList();
      return new AchivementView({
        el: '[data-js=achivementList]',
        dashboard: this.dashboard,
        achivements: this.achivements
      });
    };

    UserPanelView.prototype.events = {
      "click [data-js=deleteCircle]": "deleteCircle"
    };

    UserPanelView.prototype.renderUserPanel = function() {
      return this.$el.html(UserTemplate());
    };

    UserPanelView.prototype.renderCircleList = function() {
      var circleList, dashboard, sendData;
      circleList = [];
      dashboard = this.dashboard;
      sendData = {
        count: 40,
        offset: 0
      };
      return $.ajax({
        type: "GET",
        url: "https://core.unitus-ac.com/Circle",
        data: sendData,
        success: (function(_this) {
          return function(msg) {
            return $.each(msg.Content.Circle, function(index, obj) {
              var circle, existCircle;
              existCircle = _this.circles.where({
                CircleID: obj.CircleId
              });
              if (existCircle.length <= 0) {
                circle = new Circle({
                  CircleID: obj.CircleId,
                  CircleName: obj.CircleName,
                  MemberCount: obj.MemberCount,
                  BelongedSchool: obj.BelongedSchool,
                  LastUpdateDate: obj.LastUpdateDate,
                  IsBelonging: obj.IsBelonging
                });
                return _this.circles.add(circle);
              } else {
                console.log("これです。");
                return existCircle[0].set({
                  CircleID: obj.CircleId,
                  CircleName: obj.CircleName,
                  MemberCount: obj.MemberCount,
                  BelongedSchool: obj.BelongedSchool,
                  LastUpdateDate: obj.LastUpdateDate,
                  IsBelonging: obj.IsBelonging
                });
              }
            });
          };
        })(this),
        error: function(msg) {
          return console.log(msg);
        }
      });
    };

    UserPanelView.prototype.deleteCircle = function(e) {
      var $circleRow, id, sendData;
      e.preventDefault();
      e.stopPropagation();
      $circleRow = $($($(e.target).get(0)).closest("tr").get(0));
      if (confirm($($circleRow.children("td.name").get(0)).text() + "を削除しますか？")) {
        id = $circleRow.attr("data-circleListID");
        sendData = {
          circleID: id
        };
        return $.ajax({
          type: "DELETE",
          url: "https://core.unitus-ac.com/Circle",
          data: sendData,
          success: (function(_this) {
            return function(msg) {
              var target;
              _this.notyHelper.generate('info', '削除成功', "サークルを削除しました。");
              target = "[data-commonId=" + id + "]";
              console.log(target);
              return $(target).remove();
            };
          })(this),
          error: (function(_this) {
            return function(msg) {
              return _this.notyHelper.generate('error', '削除失敗', "何らかの理由でサークルを削除できませんでした。");
            };
          })(this)
        });
      }
    };

    return UserPanelView;

  })(Backbone.View);
});
