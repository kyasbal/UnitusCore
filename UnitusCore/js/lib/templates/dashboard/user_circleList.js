define(["jade"],function(jade){

return function template(locals) {
var buf = [];
var jade_mixins = {};
var jade_interp;
;var locals_for_with = (locals || {});(function (user) {
buf.push("<tr><h1>a</h1>" + (jade.escape(null == (jade_interp = user) ? "" : jade_interp)) + "</tr>");}.call(this,"user" in locals_for_with?locals_for_with.user:typeof user!=="undefined"?user:undefined));;return buf.join("");
};

});
