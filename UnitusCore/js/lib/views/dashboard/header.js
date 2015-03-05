var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; },
  __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone', 'templates/dashboard/header', 'templates/header/config'], function($, Backbone, HeaderTemplate, ConfigTemplate) {
  var HeaderView;
  return HeaderView = (function(_super) {
    __extends(HeaderView, _super);

    function HeaderView() {
      this.renderConfig = __bind(this.renderConfig, this);
      return HeaderView.__super__.constructor.apply(this, arguments);
    }

    HeaderView.prototype.initialize = function(option) {
      this.dashboard = option.dashboard;
      this.admin_panel = option.admin_panel;
      this.notyHelper = new NotyHelper();
      this.renderTemplate();
      this.renderConfig();
      if (!this.dashboard.get("GithubAssociation")) {
        return this.settingModalOpen();
      }
    };

    HeaderView.prototype.events = {
      "click [data-js=adminToggle]": "adminOpen",
      "click [data-js=setting]": "settingModalOpen",
      "click [data-js=authorizeGithub]": "authorizeGithub"
    };

    HeaderView.prototype.renderTemplate = function() {
      return this.$el.html(HeaderTemplate({
        dashboard: this.dashboard
      }));
    };

    HeaderView.prototype.settingModalOpen = function(e) {
      return $("[data-js=settingModal]").modal("show");
    };

    HeaderView.prototype.adminOpen = function(e) {
      e.preventDefault();
      e.stopPropagation();
      return this.admin_panel.set({
        isOpen: true
      });
    };

    HeaderView.prototype.authorizeGithub = function() {
      return location.assign('https://core.unitus-ac.com/Github/Authorize');
    };

    HeaderView.prototype.renderConfig = function() {
      return $.ajax({
        type: "GET",
        url: "https://core.unitus-ac.com/Config",
        success: (function(_this) {
          return function(msg) {
            return $.each(msg.Content.DisclosureConfigs, function(index, obj) {
              _this.renderConfigTemplate(obj);
              return $("[data-property=" + obj.Property + "] input").on("change", function(e) {
                var sendData;
                sendData = {
                  ConfigString: $(e.target).attr("id"),
                  Property: obj.Property
                };
                return $.ajax({
                  type: "PUT",
                  url: "https://core.unitus-ac.com/Config/Disclosure",
                  data: sendData,
                  success: function(msg) {
                    return _this.notyHelper.generate('success', '範囲変更', '公開範囲を変更しました。');
                  },
                  error: function(msg) {
                    return _this.notyHelper.generate('error', '変更失敗', '公開範囲を変更できませんでした。');
                  }
                });
              });
            });
          };
        })(this),
        error: (function(_this) {
          return function(msg) {
            return _this.notyHelper.generate('error', '取得失敗', '範囲設定項目を取得できませんでした。');
          };
        })(this)
      });
    };

    HeaderView.prototype.renderConfigTemplate = function(config) {
      return this.$("[data-js=DisclosureList]").append(ConfigTemplate({
        config: config
      }));
    };

    return HeaderView;

  })(Backbone.View);
});
