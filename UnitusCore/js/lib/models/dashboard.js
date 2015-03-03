var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone'], function($, Backbone) {
  var Dashboard;
  return Dashboard = (function(_super) {
    __extends(Dashboard, _super);

    function Dashboard() {
      return Dashboard.__super__.constructor.apply(this, arguments);
    }

    Dashboard.prototype.defaults = {
      AchivementCategories: '',
      AvatarUri: '',
      CircleBelonging: '',
      IsAdministrator: '',
      Name: '',
      UserName: '',
      Profile: '',
      GithubAssociation: false
    };

    return Dashboard;

  })(Backbone.Model);
});
