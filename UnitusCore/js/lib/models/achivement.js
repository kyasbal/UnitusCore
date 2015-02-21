var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone'], function($, Backbone) {
  var Achivement;
  return Achivement = (function(_super) {
    __extends(Achivement, _super);

    function Achivement() {
      return Achivement.__super__.constructor.apply(this, arguments);
    }

    Achivement.prototype.defaults = {
      Name: '実績名',
      AwardedDate: '取得日',
      BadgeImageUrl: 'バッジ画像',
      CurrentProgress: '進捗率',
      IsAwarded: false,
      ProgressDiff: '前日差分'
    };

    return Achivement;

  })(Backbone.Model);
});
