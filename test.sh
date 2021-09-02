# F19E9E
#2E4E80
#1250B0
# reg="http://fa3b8a8136cd.ngrok.io/reg"
# is="http://fa3b8a8136cd.ngrok.io/isstarted"

reg="http://0.0.0.0:5000/reg"
is="http://0.0.0.0:5000/isstarted"


# curl   --header 'Content-Type: application/json'  --request POST   --data '{"name":"dddd"}'  http://0.0.0.0:5000/reg
curl   --header 'Content-Type: application/json'  --request POST   --data '{"name":"omar"}'  $reg
# curl   --header 'Content-Type: application/json'  --request POST   --data '{"name":"mhmd"}'  $reg
# curl   --header 'Content-Type: application/json'  --request POST   --data '{"name":"abd"}'  $reg

# curl --header "Content-Type: application/json"   --request POST   --data '{"index":"0"}' http://0.0.0.0:5000/isstarted&
curl --header "Content-Type: application/json"   --request POST   --data '{"index":"0","howMuchPlayersStarted":"0"}' $is &
# curl --header "Content-Type: application/json"   --request POST   --data '{"index":"2","howMuchPlayersStarted":"0"}' $is &
# curl --header "Content-Type: application/json"   --request POST   --data '{"index":"3","howMuchPlayersStarted":"0"}' $is &

