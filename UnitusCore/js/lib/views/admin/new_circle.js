var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; },
  __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone', 'templates/admin/new_circle', 'models/circle'], function($, Backbone, NewCircleTemplate, Circle) {
  var AdminNewCircleView;
  return AdminNewCircleView = (function(_super) {
    __extends(AdminNewCircleView, _super);

    function AdminNewCircleView() {
      this.isCircleExist = __bind(this.isCircleExist, this);
      this.validationCreateButton = __bind(this.validationCreateButton, this);
      this.watchNewCircleEvents = __bind(this.watchNewCircleEvents, this);
      this.watchChangeValue = __bind(this.watchChangeValue, this);
      return AdminNewCircleView.__super__.constructor.apply(this, arguments);
    }

    AdminNewCircleView.prototype.initialize = function(option) {
      this.notyHelper = new NotyHelper();
      this.circle = new Circle();
      this.render();
      this.watchNewCircleEvents();
      return $.ajax({
        type: "GET",
        url: "https://core.unitus-ac.com/Candidate/University",
        success: (function(_this) {
          return function(msg) {
            console.log("栄光");
            console.log(msg);
            $.each(msg.Content, function(index, obj) {
              return _this.$("[data-js=circleSelect]").append("<option>" + obj + "</option>");
            });
            return _this.$("[data-js=circleSelect]").append("<option>その他</option>");
          };
        })(this),
        error: (function(_this) {
          return function(msg) {
            console.log("栄光ジャナイ");
            return console.log(msg);
          };
        })(this)
      });
    };

    AdminNewCircleView.prototype.events = {
      "change input": "watchChangeValue",
      "change textarea": "watchChangeValue",
      "change select": "watchChangeValue",
      "click [data-js=createCircle]": "createCircle"
    };

    AdminNewCircleView.prototype.render = function() {
      return this.$el.html(NewCircleTemplate());
    };

    AdminNewCircleView.prototype.createCircle = function(e) {
      var sendData;
      e.preventDefault();
      e.stopPropagation();
      $(e.target).html("<img src='./img/send.gif'>");
      sendData = {
        Name: this.circle.get("CircleName"),
        Description: this.circle.get("CircleDescription"),
        MemberCount: this.circle.get("MemberCount"),
        BelongedSchool: this.circle.get("BelongedSchool"),
        Notes: this.circle.get("Notes"),
        Contact: this.circle.get("Contact"),
        CanInterColledge: true,
        ActivityDate: this.circle.get("ActivityDate"),
        LeaderUserName: this.circle.get("LeaderUserName")
      };
      return $.ajax({
        type: "POST",
        url: "https://core.unitus-ac.com/Circle",
        data: sendData,
        success: (function(_this) {
          return function(msg) {
            _this.notyHelper.generate("success", "作成完了", (_this.circle.get("CircleName")) + "を追加しました。");
            console.log("成功したよ");
            console.log(msg);
            return $(e.target).html("作成する");
          };
        })(this),
        error: (function(_this) {
          return function(msg) {
            console.log("失敗したよ");
            console.log(msg);
            if (msg.statusText === "Conflict") {
              _this.notyHelper.generate("error", "作成失敗", (_this.circle.get("CircleName")) + "はすでに存在します。");
            } else if (msg.statusText === "Not Found") {
              _this.notyHelper.generate("error", "作成失敗", "代表者のメールアドレス(" + (_this.circle.get("LeaderUserName")) + ")はデータベースに存在しません。");
              _this.$("[data-js=LeaderUserName]").addClass("form-danger");
            } else {
              switch (msg.responseText) {
                case "Name is empty.":
                  _this.notyHelper.generate("error", "作成失敗", "団体名が記入されていません。");
                  _this.$("[data-js=CircleName]").addClass("form-danger");
                  break;
                case "LeaderUserName is empty.":
                  _this.notyHelper.generate("error", "作成失敗", "代表者名が記入されていません。");
                  _this.$("[data-js=LeaderUserName]").addClass("form-danger");
                  break;
                case "BelongedSchool is empty.":
                  _this.notyHelper.generate("error", "作成失敗", "所属大学が記入されていません。");
                  _this.$("[data-js=BelongedSchool]").addClass("form-danger");
                  break;
                default:
                  _this.notyHelper.generate("error", "作成失敗", (_this.circle.get("CircleName")) + "は何らかの理由で作成できませんでした。");
              }
            }
            return $(e.target).html("作成する");
          };
        })(this)
      });
    };

    AdminNewCircleView.prototype.watchChangeValue = function(e) {
      var $target;
      $target = $(e.target);
      console.log($target.attr("data-js"));
      $target.removeClass("form-danger");
      return this.circle.trigger($target.attr("data-js"));
    };

    AdminNewCircleView.prototype.watchNewCircleEvents = function(e) {
      this.circle.on("CircleName", (function(_this) {
        return function() {
          _this.circle.set({
            CircleName: _this.$("[data-js=CircleName]").val()
          });
          if (_this.isCircleExist()) {
            _this.validationCreateButton();
          }
          return console.log(_this.circle.get("CircleName"));
        };
      })(this));
      this.circle.on("CircleDescription", (function(_this) {
        return function() {
          _this.circle.set({
            CircleDescription: _this.$("[data-js=CircleDescription]").val()
          });
          return console.log(_this.circle.get("CircleDescription"));
        };
      })(this));
      this.circle.on("MemberCount", (function(_this) {
        return function() {
          _this.circle.set({
            MemberCount: _this.$("[data-js=MemberCount]").val()
          });
          return console.log(_this.circle.get("MemberCount"));
        };
      })(this));
      this.circle.on("WebAddress", (function(_this) {
        return function() {
          return _this.circle.set({
            WebAddress: _this.$("[data-js=WebAddress]").val()
          });
        };
      })(this));
      this.circle.on("circleSelect", (function(_this) {
        return function() {
          var value;
          value = _this.$("[data-js=circleSelect]").val();
          if (value === "その他") {
            _this.$("[data-js=formWrap]").addClass("open");
            _this.circle.set({
              BelongedSchool: ""
            });
          } else {
            _this.$("[data-js=formWrap]").removeClass("open");
            $("[data-js=BelongedSchool]").val("");
            if (value === "-") {
              _this.circle.set({
                BelongedSchool: ""
              });
            } else {
              _this.circle.set({
                BelongedSchool: value
              });
            }
          }
          if (_this.isCircleExist()) {
            _this.validationCreateButton();
          }
          return console.log(_this.circle.get("BelongedSchool"));
        };
      })(this));
      this.circle.on("BelongedSchool", (function(_this) {
        return function() {
          _this.circle.set({
            BelongedSchool: _this.$("[data-js=BelongedSchool]").val()
          });
          if (_this.isCircleExist()) {
            _this.validationCreateButton();
          }
          return console.log(_this.circle.get("BelongedSchool"));
        };
      })(this));
      this.circle.on("Notes", (function(_this) {
        return function() {
          _this.circle.set({
            Notes: _this.$("[data-js=Notes]").val()
          });
          return console.log(_this.circle.get("Notes"));
        };
      })(this));
      this.circle.on("Contact", (function(_this) {
        return function() {
          _this.circle.set({
            Contact: _this.$("[data-js=Contact]").val()
          });
          return console.log(_this.circle.get("Contact"));
        };
      })(this));
      this.circle.on("CanInterColledge", (function(_this) {
        return function() {
          if (_this.$("[data-js=CanInterColledge]").is(':checked')) {
            _this.circle.set({
              CanInterColledge: true
            });
          } else {
            _this.circle.set({
              CanInterColledge: false
            });
          }
          return console.log(_this.circle.get("CanInterColledge"));
        };
      })(this));
      this.circle.on("ActivityDate", (function(_this) {
        return function() {
          _this.circle.set({
            ActivityDate: _this.$("[data-js=ActivityDate]").val()
          });
          return console.log(_this.circle.get("ActivityDate"));
        };
      })(this));
      return this.circle.on("LeaderUserName", (function(_this) {
        return function() {
          _this.circle.set({
            LeaderUserName: _this.$("[data-js=LeaderUserName]").val()
          });
          _this.validationCreateButton();
          return console.log(_this.circle.get("LeaderUserName"));
        };
      })(this));
    };

    AdminNewCircleView.prototype.validationCreateButton = function() {
      if (this.circle.get("CircleName") !== "" && this.circle.get("LeaderUserName") !== "" && this.circle.get("BelongedSchool") !== "") {
        return this.$("[data-js=createCircle]").prop("disabled", false);
      } else {
        return this.$("[data-js=createCircle]").prop("disabled", true);
      }
    };

    AdminNewCircleView.prototype.isCircleExist = function() {
      var sendData;
      console.log("isCircles");
      sendData = {
        circleName: "" + (this.circle.get("CircleName")),
        universityName: "" + (this.circle.get("BelongedSchool"))
      };
      return $.ajax({
        type: "GET",
        url: "https://core.unitus-ac.com/Circle/CheckExist",
        dataType: "text",
        data: sendData,
        success: (function(_this) {
          return function(msg) {
            return false;
          };
        })(this),
        error: (function(_this) {
          return function(msg) {
            _this.notyHelper.generate("error", "作成失敗", "既にそのサークルはデータベースに存在しています");
            return true;
          };
        })(this)
      });
    };

    return AdminNewCircleView;

  })(Backbone.View);
});
