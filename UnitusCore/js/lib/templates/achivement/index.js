define(["jade"],function(jade){

return function template(locals) {
var buf = [];
var jade_mixins = {};
var jade_interp;
;var locals_for_with = (locals || {});(function (achivement) {
buf.push("<div data-js=\"badge\" class=\"achive_badge\"><div class=\"image-wrap\"><img" + (jade.attr("src", "" + (achivement.get('BadgeImageUrl')) + "", true, false)) + "/></div><div class=\"achive_name\">" + (jade.escape(null == (jade_interp = achivement.get('Name')) ? "" : jade_interp)) + "</div><div class=\"row info\"><div class=\"achive_date col-xs-6\"><div class=\"tag\">取得日</div>");
if ( achivement.get('AwardedDate') == "")
{
buf.push("-");
}
else
{
buf.push(jade.escape(null == (jade_interp = achivement.get('AwardedDate')) ? "" : jade_interp));
}
buf.push("</div><div class=\"achive_progressDiff col-xs-6\"><div class=\"tag\">前日差</div>" + (jade.escape(null == (jade_interp = achivement.get('ProgressDiff')) ? "" : jade_interp)) + "</div></div><div class=\"achive_progress-wrap\"></div><div" + (jade.attr("style", "width: " + (achivement.get('CurrentProgress') * 100) + "%; background-color: rgb(220," + (achivement.get('CurrentProgress')*100+120) + ",0)", true, false)) + " class=\"achive_progress\"></div>");
if (!( achivement.get('IsAwarded')))
{
buf.push("<div class=\"smoke\"></div>");
}
buf.push("</div>");}.call(this,"achivement" in locals_for_with?locals_for_with.achivement:typeof achivement!=="undefined"?achivement:undefined));;return buf.join("");
};

});
