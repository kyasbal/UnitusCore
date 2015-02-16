var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone'], function($, Backbone) {
  var User;
  return User = (function(_super) {
    __extends(User, _super);

    function User() {
      return User.__super__.constructor.apply(this, arguments);
    }

    User.prototype.defaults = {
      name: 'サンプル太郎',
      mail: 'example.com',
      avatar: 'noImage',
      circles: '',
      isAdmin: true,
      valdationToken: 'abc'
    };

    return User;

  })(Backbone.Model);
});
