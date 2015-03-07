curl https://core.unitus-ac.com/Account/LoginWithAdmin -X POST -d "UserName=LimeStreem@gmail.com&Password=Kyasbal08!" -c COOKIE

curl https://core.unitus-ac.com/Dashboard?validationToken=a -X GET -b COOKIE -c COOKIE
