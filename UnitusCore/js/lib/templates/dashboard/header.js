define(["jade"],function(jade){

return function template(locals) {
var buf = [];
var jade_mixins = {};
var jade_interp;
;var locals_for_with = (locals || {});(function (dashboard) {
buf.push("<h1 data-js=\"logoTitle\" class=\"text-center\">UNITUS</h1><div class=\"dropdown\"><div id=\"account\" data-js=\"dropdown\" data-toggle=\"dropdown\" aria-expanded=\"false\">" + (jade.escape(null == (jade_interp = dashboard.get('Name')) ? "" : jade_interp)) + "<span class=\"caret\"></span></div><ul role=\"menu\" aria-labelledby=\"account\" class=\"dropdown-menu\">");
if ( dashboard.get('IsAdministrator'))
{
buf.push("<li class=\"author menu\">管理者メニュー</li><li data-js=\"adminToggle\" class=\"item\">管理画面を開く</li><li class=\"divider\"></li>");
}
buf.push("<!-- (管理者用ここまで)--><li class=\"author menu\">アカウントメニュー</li><li data-js=\"setting\" class=\"item\">設定</li><li class=\"divider\"></li><li data-js=\"logout\" class=\"item\">ログアウト</li><li class=\"divider\"></li></ul></div><div id=\"settingModal\" data-js=\"settingModal\" class=\"modal fade\"><div class=\"modal-dialog\"><div class=\"modal-content\"><div class=\"modal-header\"><button type=\"button\" data-dismiss=\"modal\" aria-label=\"Close\" class=\"close\"><span aria-hidden=\"true\">&times;</span></button><h4 id=\"settingModalLabel\" class=\"modal-title\">設定メニュー</h4></div><div class=\"modal-body\"><div role=\"tabpanel\"><ul role=\"tablist\" class=\"nav nav-tabs\"><li role=\"presentation\" class=\"active\"><a href=\"#settingGithub\" aria-controls=\"settingGithub\" role=\"tab\" data-toggle=\"tab\">Github連携</a></li><li role=\"presentation\"><a href=\"#settingDisclosure\" aria-controls=\"settingDisclosure\" role=\"tab\" data-toggle=\"tab\">公開設定</a></li></ul><div class=\"tab-content\"><div id=\"settingGithub\" role=\"tabpanel\" class=\"tab-pane active\">");
if ( dashboard.get("GithubAssociation"))
{
buf.push("<h1>Github認証済み</h1><p>あなたは既にGithub認証を完了しています。\n以下のようなことが可能になりました。</p>");
}
else
{
buf.push("<h1>Github未認証</h1><p>あなたはまだGithub認証をしていません。\nGithub認証をすると、以下のようなことが出来るようになります。</p>");
}
buf.push("<ul><li>プロフィール画像の自動設定</li><li>統計情報の表示</li><li>プロジェクトへの自分の関与率の表示</li><li>ランキングとやる気の出現</li></ul><div class=\"pull-right\">");
if ( dashboard.get("GithubAssociation"))
{
buf.push("<div data-js=\"authorizeGithub\" disabled=\"disabled\" class=\"btn btn-success\">Github認証済み</div>");
}
else
{
buf.push("<div data-js=\"authorizeGithub\" class=\"btn btn-success\">Github認証する</div>");
}
buf.push("</div><div class=\"clear\"></div></div><div id=\"settingDisclosure\" role=\"tabpanel\" class=\"tab-pane\"><div data-js=\"DisclosureList\" class=\"row\"></div></div></div></div></div></div></div></div>");}.call(this,"dashboard" in locals_for_with?locals_for_with.dashboard:typeof dashboard!=="undefined"?dashboard:undefined));;return buf.join("");
};

});
