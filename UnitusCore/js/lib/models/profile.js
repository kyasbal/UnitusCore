var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone'], function($, Backbone) {
  var Profile;
  return Profile = (function(_super) {
    __extends(Profile, _super);

    function Profile() {
      return Profile.__super__.constructor.apply(this, arguments);
    }

    Profile.prototype.defaults = {
      BelongedSchool: '',
      CreatedDateInfo: '',
      CreatedDateInfoByDateOffset: '',
      CurrentGrade: '',
      Email: '',
      Faculty: '',
      GithubProfile: '',
      Major: '',
      Notes: '',
      Url: '',
      UserName: '',
      IsSelf: true
    };

    return Profile;

  })(Backbone.Model);
});
