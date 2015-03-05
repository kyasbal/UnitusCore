var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone', 'models/circle'], function($, Backbone, Circle) {
  var Circles;
  return Circles = (function(_super) {
    __extends(Circles, _super);

    function Circles() {
      return Circles.__super__.constructor.apply(this, arguments);
    }

    Circles.prototype.model = Circle;

    return Circles;

  })(Backbone.Collection);
});
