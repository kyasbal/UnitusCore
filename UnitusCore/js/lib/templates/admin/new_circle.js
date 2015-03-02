define(["jade"],function(jade){

return function template(locals) {
var buf = [];
var jade_mixins = {};
var jade_interp;

buf.push("<h1>新規団体追加<div data-js=\"markdown\" class=\"label label-default\">プレビュー</div></h1><form><div class=\"form-group\"><label for=\"circle_name\">団体名</label><input id=\"circle_name\" type=\"text\" placeholder=\"応用数学研究部\" data-js=\"CircleName\" class=\"form-control\"/><label for=\"circle_description\">団体説明を記入</label><textarea id=\"circle_description\" placeholder=\"団体説明を記入\" rows=\"10\" data-js=\"CircleDescription\" class=\"form-control\"></textarea><label for=\"circle_num\">人数</label><input id=\"circle_num\" type=\"text\" placeholder=\"18\" data-js=\"MemberCount\" class=\"form-control\"/><label for=\"site_name\">ウェブサイト</label><input id=\"site_name\" type=\"text\" placeholder=\"http://unitus-ac.com\" data-js=\"WebAddress\" class=\"form-control\"/><label for=\"university\">所属大学</label><select data-js=\"circleSelect\" class=\"form-control\"><option>-</option></select><div data-js=\"formWrap\" class=\"form-wrap\"><input id=\"university\" type=\"text\" placeholder=\"新しい大学名を入力\" data-js=\"BelongedSchool\" class=\"form-control\"/></div><label for=\"remarks\">備考</label><textarea id=\"remarks\" placeholder=\"男ばっかりのサークルです。\" data-js=\"Notes\" class=\"form-control\"></textarea><label for=\"contact\">連絡先</label><input id=\"contact\" type=\"text\" placeholder=\"Tel: 090123456\" data-js=\"Contact\" class=\"form-control\"/><label for=\"leader\">代表者</label><input id=\"leader\" type=\"text\" placeholder=\"yamada@gmail.com\" data-js=\"LeaderUserName\" class=\"form-control\"/><label for=\"activityDate\">活動日</label><textarea id=\"activityDate\" type=\"text\" placeholder=\"#土曜日\n#月曜日\" rows=\"4\" data-js=\"ActivityDate\" class=\"form-control\"></textarea><div class=\"checkbox\"><label><input id=\"isAcceptOutside\" type=\"checkbox\" data-js=\"CanInterColledge\"/> 外部生のサークル加入可否</label></div><div class=\"pull-right\"><button type=\"submit\" data-js=\"createCircle\" disabled=\"disabled\" class=\"btn btn-primary\">作成する</button></div><div class=\"clear\"></div></div></form>");;return buf.join("");
};

});
