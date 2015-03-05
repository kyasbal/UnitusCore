define(["jade"],function(jade){

return function template(locals) {
var buf = [];
var jade_mixins = {};
var jade_interp;
;var locals_for_with = (locals || {});(function (config) {
buf.push("<div class=\"col-xs-6\"><p>" + (jade.escape(null == (jade_interp = config.DisplayConfigureName) ? "" : jade_interp)) + "</p></div><div class=\"col-xs-6\"><div data-toggle=\"buttons\"" + (jade.attr("data-property", config.Property, true, false)) + " class=\"btn-group\">");
if ( config.ConfigString == "Public")
{
buf.push("<label class=\"btn active\"><input id=\"Public\" type=\"radio\" name=\"disclosure\" autocomplete=\"off\" checked=\"checked\"/> 公開</label>");
}
else
{
buf.push("<label class=\"btn\"><input id=\"Public\" type=\"radio\" name=\"disclosure\" autocomplete=\"off\"/> 公開</label>");
}
if ( config.ConfigString == "CircleOnly")
{
buf.push("<label class=\"btn active\"><input id=\"CircleOnly\" type=\"radio\" name=\"disclosure\" autocomplete=\"off\" checked=\"checked\"/> サークルのみ</label>");
}
else
{
buf.push("<label class=\"btn\"><input id=\"CircleOnly\" type=\"radio\" name=\"disclosure\" autocomplete=\"off\"/> サークルのみ</label>");
}
if ( config.ConfigString == "Private")
{
buf.push("<label class=\"btn active\"><input id=\"Private\" type=\"radio\" name=\"disclosure\" autocomplete=\"off\" checked=\"checked\"/> 非公開</label>");
}
else
{
buf.push("<label class=\"btn\"><input id=\"Private\" type=\"radio\" name=\"disclosure\" autocomplete=\"off\"/> 非公開</label>");
}
buf.push("</div></div>");}.call(this,"config" in locals_for_with?locals_for_with.config:typeof config!=="undefined"?config:undefined));;return buf.join("");
};

});
