var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone', 'templates/dashboard/user_panel', 'templates/dashboard/user_profile'], function($, Backbone, UserTemplate, UserProfile) {
  var UserPanelView;
  return UserPanelView = (function(_super) {
    __extends(UserPanelView, _super);

    function UserPanelView() {
      return UserPanelView.__super__.constructor.apply(this, arguments);
    }

    UserPanelView.prototype.initialize = function(option) {
      this.user = option.user;
      this.belongingCircles = this.user.attributes.circles;
      this.renderUserPanel();
      this.renderUserProfile();
      this.renderCircleList();
      if (this.belongingCircles.length > 0) {
        return this.renderBelongingCircles();
      }
    };

    UserPanelView.prototype.renderUserPanel = function() {
      return this.$el.html(UserTemplate());
    };

    UserPanelView.prototype.renderCircleList = function() {
      var sendData;
      sendData = {
        count: 40,
        offset: 0,
        validationToken: 'abc'
      };
      return $.ajax({
        type: "GET",
        url: "https://core.unitus-ac.com/Circle",
        data: sendData,
        success: function(msg) {
          return $.each(msg.Content.Circle, function() {
            var text;
            text = '';
            text += '<tr>';
            text += '<td class="name name_w">' + this.CircleName + '<i class="glyphicon glyphicon-eye-open"></i></td>';
            text += '<td class="author author_w">' + "閲覧者" + '</td>';
            text += '<td class="number number_w">' + this.MemberCount + '</td>';
            text += '<td class="university university_w">' + this.BelongedUniversity + '</td>';
            text += '<td class="update update_w">' + this.LastUpdateDate + '<i class="fa fa-clipboard" data-js="copyMail" data-clipboard-text="' + this.UserName + '"></i></td>';
            text += '</tr>';
            return $("[data-js=circleList]").append(text);
          });
        },
        error: function(msg) {
          return console.log(msg);
        }
      });
    };

    UserPanelView.prototype.renderUserProfile = function() {
      return this.$('[data-js="myProfile"]').html(UserProfile({
        user: this.user
      }));
    };

    UserPanelView.prototype.renderBelongingCircles = function() {
      var textPanel, textSidebar;
      textSidebar = '';
      textPanel = '';
      textSidebar += '<li class="divider"><h1>所属サークル</h1></li>';
      $.each(this.belongingCircles, function() {
        textSidebar += '<li role="presentation">';
        textSidebar += '<a href="#' + this.CircleId + '" aria-controls="#' + this.CircleId + '" role="tab" data-toggle="tab">';
        textSidebar += '<i class="circleIcon">' + this.CircleName.slice(0, 1) + '</i>';
        textSidebar += '<span class="title">' + this.CircleName + '</span>';
        textSidebar += '</a>';
        textSidebar += '</li>';
        textPanel += '<div id="' + this.CircleId + '" class="tab-pane fade in" role="tabpanel">';
        textPanel += '<h1>' + this.CircleName + '</h1>';
        return textPanel += '</div>';
      });
      $("[data-js=userSideList]").append(textSidebar);
      return $("[data-js=userPanelList]").append(textPanel);
    };

    return UserPanelView;

  })(Backbone.View);
});
