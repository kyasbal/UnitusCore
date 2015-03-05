define(["jade"],function(jade){

return function template(locals) {
var buf = [];
var jade_mixins = {};
var jade_interp;

buf.push("<div id=\"adminSidebar\" class=\"sidebar\"><ul role=\"tablist\" class=\"nav nav-tabs\"><li role=\"presentation\" class=\"active\"><a href=\"#adminNewCircle\" aria-controls=\"adminNewCircle\" role=\"tab\" data-toggle=\"tab\"><i class=\"glyphicon glyphicon-plus\"></i><span class=\"title\">新規団体追加</span></a></li><li role=\"presentation\"><a href=\"#adminUserList\" aria-controls=\"adminUserList\" role=\"tab\" data-toggle=\"tab\"><i class=\"glyphicon glyphicon-th-list\"></i><span class=\"title\">ユーザー一覧</span></a></li></ul></div><div id=\"adminContent\" class=\"content\"><div class=\"tab-content\"><div id=\"adminNewCircle\" role=\"tabpanel\" data-js=\"adminNewCircle\" class=\"tab-pane fade in active\"></div><div id=\"adminUserList\" role=\"tabpanel\" class=\"tab-pane fade\"><h1>ユーザー一覧</h1><table><thead><tr><th class=\"name_w\">名前</th><th class=\"author_w\">権限</th><th class=\"number_w\">学年</th><th class=\"university_w\">所属大学</th><th class=\"mail_w\">メールアドレス</th></tr></thead><tbody data-js=\"userList\"></tbody></table></div></div></div><div id=\"adminOptionbar\"><div data-js=\"close_admin\" class=\"close_btn\"><div class=\"glyphicon glyphicon-remove\"></div></div></div>");;return buf.join("");
};

});
