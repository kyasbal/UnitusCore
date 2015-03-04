define(["jade"],function(jade){

return function template(locals) {
var buf = [];
var jade_mixins = {};
var jade_interp;
;var locals_for_with = (locals || {});(function (dashboard, undefined) {
buf.push("<div class=\"profilebar\"><h1 class=\"profile_name\"><i class=\"glyphicon glyphicon-chevron-right\"></i>" + (jade.escape(null == (jade_interp = dashboard.get("Name")) ? "" : jade_interp)) + "</h1><img" + (jade.attr("src", "" + (dashboard.get('AvatarUri')) + "", true, false)) + "/><div id=\"accordion\" role=\"tablist\" aria-multiselectable=\"true\" class=\"panel-group\"><div class=\"panel panel-default\"><div id=\"profileAccordion\" role=\"tab\" class=\"panel-heading\"><h4 class=\"panel-title\"><a data-toggle=\"collapse\" data-parent=\"#accordion\" href=\"#profileCollapse\" aria-expanded=\"false\" aria-controls=\"profileCollapse\"><i class=\"glyphicon glyphicon-chevron-right\"></i>基本情報</a></h4></div><div id=\"profileCollapse\" role=\"tabpanel\" aria-labelledby=\"profileAccordion\" class=\"panel-collapse collapse\"><ul role=\"tablist\" class=\"list-group\"><li data-js=\"sendMail\" class=\"list-group-item\"><i class=\"fa fa-envelope-o\"></i>" + (jade.escape(null == (jade_interp = dashboard.get("UserName")) ? "" : jade_interp)) + "</li></ul></div></div><div class=\"panel panel-default\"><div id=\"achievementAccordion\" role=\"tab\" class=\"panel-heading\"><h4 class=\"panel-title\"><a data-toggle=\"collapse\" data-parent=\"#accordion\" href=\"#achivementCollapse\" aria-expanded=\"false\" aria-controls=\"achivementCollapse\" class=\"collapsed\"><i class=\"glyphicon glyphicon-chevron-right\"></i>実績</a></h4></div><div id=\"achivementCollapse\" role=\"tabpanel\" aria-labelledby=\"achievementAccordion\" class=\"panel-collapse collapse\"><ul role=\"tablist\" class=\"list-group\">");
// iterate dashboard.get("AchivementCategories")
;(function(){
  var $$obj = dashboard.get("AchivementCategories");
  if ('number' == typeof $$obj.length) {

    for (var index = 0, $$l = $$obj.length; index < $$l; index++) {
      var categoryName = $$obj[index];

buf.push("<a data-js=\"achivementCategory\" href=\"#userAchivement\" area-controls=\"userAchivement\"><li class=\"list-group-item\">" + (jade.escape(null == (jade_interp = categoryName) ? "" : jade_interp)) + "</li></a>");
    }

  } else {
    var $$l = 0;
    for (var index in $$obj) {
      $$l++;      var categoryName = $$obj[index];

buf.push("<a data-js=\"achivementCategory\" href=\"#userAchivement\" area-controls=\"userAchivement\"><li class=\"list-group-item\">" + (jade.escape(null == (jade_interp = categoryName) ? "" : jade_interp)) + "</li></a>");
    }

  }
}).call(this);

buf.push("</ul></div></div><div class=\"panel panel-default\"><div id=\"accountAccordion\" role=\"tab\" class=\"panel-heading\"><h4 class=\"panel-title\"><a data-toggle=\"collapse\" data-parent=\"#accordion\" href=\"#accountCollapse\" aria-expanded=\"false\" aria-controls=\"accountCollapse\" class=\"collapsed\"><i class=\"glyphicon glyphicon-chevron-right\"></i>各種アカウント</a></h4></div><div id=\"accountCollapse\" role=\"tabpanel\" aria-labelledby=\"accountAccordion\" class=\"panel-collapse collapse\"><ul role=\"\" class=\"list-group\"><li class=\"list-group-item\">hoge</li><li class=\"list-group-item\">hoge</li><li class=\"list-group-item\">hoge</li></ul></div></div></div></div><div class=\"panels\"><div id=\"userAchivement\" role=\"tabpanel\" data-js=\"achivementList\" class=\"panelcontent tab-pane fade\"><div class=\"row\"><div class=\"col-xs-12\"><div class=\"title text-center\">実績一覧<span data-js=\"categoryName\"></span></div></div><div data-js=\"badges\" class=\"col-xs-12\"></div></div><div id=\"achivementPanel\" data-js=\"achivementPanel\" class=\"hidden_panel_r\"></div></div></div>");}.call(this,"dashboard" in locals_for_with?locals_for_with.dashboard:typeof dashboard!=="undefined"?dashboard:undefined,"undefined" in locals_for_with?locals_for_with.undefined:typeof undefined!=="undefined"?undefined:undefined));;return buf.join("");
};

});
