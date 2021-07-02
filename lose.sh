# reg="http://fa3b8a8136cd.ngrok.io/lose"

reg="http://0.0.0.0:5000/lose"


# curl   --header 'Content-Type: application/json'  --request POST   --data '{"name":"dddd"}'  http://0.0.0.0:5000/reg
curl   --header 'Content-Type: application/json'  --request POST   --data '{"pindex":"1"}'  $reg
# curl   --header 'Content-Type: application/json'  --request POST   --data '{"pindex":"2"}'  $reg
# curl   --header 'Content-Type: application/json'  --request POST   --data '{"pindex":"3"}'  $reg
