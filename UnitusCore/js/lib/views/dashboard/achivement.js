var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; },
  __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone', 'models/achivement', 'templates/achivement/index', 'templates/achivement/show'], function($, Backbone, Achivement, AchivementListTemplate, AchivementShowTemplate) {
  var AchivementView;
  return AchivementView = (function(_super) {
    __extends(AchivementView, _super);

    function AchivementView() {
      this.achiveShow = __bind(this.achiveShow, this);
      return AchivementView.__super__.constructor.apply(this, arguments);
    }

    AchivementView.prototype.initialize = function(option) {
      this.dashboard = option.dashboard;
      this.achivements = option.achivements;
      return $.ajax({
        type: "GET",
        url: "https://core.unitus-ac.com/Achivements",
        success: (function(_this) {
          return function(data) {
            console.log(data);
            return $.each(data.Content.AchivementCategories, function(parentIndex, parentObj) {
              var categoryName;
              categoryName = "belonged" + parentObj.CategoryName;
              $.each(parentObj.Achivements, function(index, obj) {
                var achivement;
                if (parentIndex === 0) {
                  achivement = new Achivement({
                    Name: obj.AchivementName,
                    AwardedDate: obj.AwardedDate,
                    BadgeImageUrl: obj.BadgeImageUrl,
                    CurrentProgress: (obj.CurrentProgress === "NaN" ? null : obj.CurrentProgress.toFixed(2)),
                    IsAwarded: obj.IsAwarded,
                    ProgressDiff: (obj.ProgressDiff === "NaN" ? null : obj.ProgressDiff.toFixed(2))
                  });
                  achivement.set(_this.hash(categoryName, true));
                  return _this.achivements.add(achivement);
                } else {
                  achivement = _this.achivements.where({
                    Name: obj.AchivementName
                  })[0];
                  return achivement.set(_this.hash(categoryName, true));
                }
              });
              return _this.listenTo(_this.achivements, parentObj.CategoryName, function() {
                return _this.render(_this.achivements.where(_this.hash("belonged" + parentObj.CategoryName, true)), parentObj.CategoryName);
              });
            });
          };
        })(this),
        error: function(data) {
          return console.log(data);
        }
      });
    };

    AchivementView.prototype.events = {
      "click [data-js=badge]": "achiveShow",
      "click [data-js=closePanel]": "closePanel"
    };

    AchivementView.prototype.achiveShow = function(e) {
      var achive_name, sendData;
      achive_name = $($(e.currentTarget).children(".achive_name")[0]).text();
      this.achivement = this.achivements.where({
        Name: achive_name
      })[0];
      if (!this.achivement.get("isDetailGetting")) {
        sendData = {
          achivementName: achive_name
        };
        return $.ajax({
          type: "GET",
          url: "https://core.unitus-ac.com/Achivement",
          data: sendData,
          success: (function(_this) {
            return function(data) {
              var values;
              _this.achivement.set({
                isDetailGetting: true
              });
              values = data.Content;
              _this.achivement.set({
                Description: values.AchivementDescription,
                AwardedPerson: values.AwardedPerson,
                AwardedRate: (values.AwardedRate === "NaN" ? null : values.AwardedRate.toFixed(2)),
                AcuireRateGraphPoints: values.AcuireRateGraphPoints,
                AwardedPerson: values.AwardedPerson,
                CircleStatistics: values.CircleStatistics,
                ProgressGraphPoints: values.ProgressGraphPoints,
                SumPerson: values.SumPerson
              });
              return $(_this.$el.children("[data-js=achivementPanel]")[0]).html(AchivementShowTemplate({
                achivement: _this.achivement,
                data: JSON.stringify(_this.achivement),
                dashboard: _this.dashboard
              })).removeClass("hidden_panel_r");
            };
          })(this),
          error: function(data) {
            return console.log(data);
          }
        });
      } else {
        return $(this.$el.children("[data-js=achivementPanel]")[0]).html(AchivementShowTemplate({
          achivement: this.achivement
        })).removeClass("hidden_panel_r");
      }
    };

    AchivementView.prototype.closePanel = function(e) {
      return $(this.$el.children("[data-js=achivementPanel]")[0]).addClass("hidden_panel_r");
    };

    AchivementView.prototype.hash = function(key, value) {
      var h;
      h = {};
      h[key] = value;
      return h;
    };

    AchivementView.prototype.render = function(achivements, categoryName) {
      $(this.$el.find("[data-js=categoryName]")).html("（" + categoryName + "）");
      $(this.$el.find("[data-js=badges]")).html('');
      return $.each(achivements, (function(_this) {
        return function(index, a) {
          return $(_this.$el.find("[data-js=badges]")).append(AchivementListTemplate({
            achivement: a
          }));
        };
      })(this));
    };

    return AchivementView;

  })(Backbone.View);
});
