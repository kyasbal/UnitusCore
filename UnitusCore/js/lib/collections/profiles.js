var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone', 'models/profile'], function($, Backbone, Profile) {
  var Profiles;
  return Profiles = (function(_super) {
    __extends(Profiles, _super);

    function Profiles() {
      return Profiles.__super__.constructor.apply(this, arguments);
    }

    Profiles.prototype.model = Profile;

    return Profiles;

  })(Backbone.Collection);
});
