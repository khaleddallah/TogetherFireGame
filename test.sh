curl   --header 'Content-Type: application/json'  --request POST   --data '{"name":"omar"}'  http://0.0.0.0:5000/reg
curl   --header 'Content-Type: application/json'  --request POST   --data '{"name":"mhmd"}'  http://0.0.0.0:5000/reg
curl   --header 'Content-Type: application/json'  --request POST   --data '{"name":"abd"}'  http://0.0.0.0:5000/reg

curl --header "Content-Type: application/json"   --request POST   --data '{"index":"1"}' http://0.0.0.0:5000/isstarted&
curl --header "Content-Type: application/json"   --request POST   --data '{"index":"2"}' http://0.0.0.0:5000/isstarted&
curl --header "Content-Type: application/json"   --request POST   --data '{"index":"3"}' http://0.0.0.0:5000/isstarted&

