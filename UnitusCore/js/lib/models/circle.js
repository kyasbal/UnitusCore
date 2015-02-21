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
      CircleName: '応用数学研究部',
      CircleDescription: '',
      Membercount: 5,
      WebAddress: 'hoge.com',
      BelongedSchool: '東京理科大学',
      Notes: 'サークルじゃなくて部活です。',
      Contact: 'twitter: @_HTTP418',
      CanInterColledge: true,
      ActivityDate: 'FRYDAY'
    };

    return Circle;

  })(Backbone.Model);
});
