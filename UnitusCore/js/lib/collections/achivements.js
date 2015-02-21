var __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
  __hasProp = {}.hasOwnProperty;

define(['jquery', 'backbone', 'models/achivement'], function($, Backbone, Achivement) {
  var Achivements;
  return Achivements = (function(_super) {
    __extends(Achivements, _super);

    function Achivements() {
      return Achivements.__super__.constructor.apply(this, arguments);
    }

    Achivements.prototype.model = Achivement;

    return Achivements;

  })(Backbone.Collection);
});
