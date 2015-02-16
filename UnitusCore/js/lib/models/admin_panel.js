var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone'], function($, Backbone) {
  var AdminPanel;
  return AdminPanel = (function(_super) {
    __extends(AdminPanel, _super);

    function AdminPanel() {
      return AdminPanel.__super__.constructor.apply(this, arguments);
    }

    AdminPanel.prototype.defaults = {
      isOpen: false
    };

    return AdminPanel;

  })(Backbone.Model);
});
