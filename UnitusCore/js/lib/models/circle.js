var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone'], function($, Backbone) {
  var Circle;
  return Circle = (function(_super) {
    __extends(Circle, _super);

    function Circle() {
      return Circle.__super__.constructor.apply(this, arguments);
    }

    Circle.prototype.defaults = {
      CircleName: '',
      CircleDescription: '',
      Membercount: '',
      WebAddress: '',
      BelongedSchool: '',
      Notes: '',
      Contact: '',
      CanInterColledge: false,
      ActivityDate: '',
      LeaderUserName: ''
    };

    return Circle;

  })(Backbone.Model);
});
