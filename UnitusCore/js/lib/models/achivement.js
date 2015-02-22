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
      Description: '詳細説明',
      AwardedPerson: '受賞者数',
      AwardedRate: '受賞者率',
      AcuireRateGraphPoints: '達成率の推移グラフ',
      AwardedDate: '取得日',
      BadgeImageUrl: 'バッジ画像',
      CircleStatistics: 'サークルメンバーの進捗状況',
      CurrentProgress: '進捗率',
      IsAwarded: false,
      ProgressDiff: '前日差分',
      ProgressGraphPoints: '達成率の変化グラフ',
      SumPerson: 'Unitus合計人数',
      isDetailGetting: false
    };

    return Achivement;

  })(Backbone.Model);
});
