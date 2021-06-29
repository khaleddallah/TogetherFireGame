a0='{"pindex":"0","data":[{"type":"move","target":"-3.990066/1.572848/-1"},{"type":"fire","gunType":"Grenade","target":"0.1158934/-0.1821193/-1"},{"type":"fire","gunType":"Sword"},{"type":"fire","gunType":"Pistol","target":"1.970199/-2.963576/-1"}]}'
a1='{"pindex":"1","data":[{"type":"move","target":"-2.799011/-1.41/0"},{"type":"fire","gunType":"Grenade","target":"-2.799011/1.389999/0"},{"type":"move","target":"-1.398987/-1.41/0"},{"type":"0"}]}'
a2='{"pindex":"2","data":[{"type":"move","target":"2.801025/-1.41/0"},{"type":"move","target":"0.7010498/-1.41/0"},{"type":"fire","gunType":"Grenade","target":"0.7010498/3.490002/0"}]}'
a3='{"pindex":"3","data":[{"type":"move","target":"-4.198975/-0.009998322/0"},{"type":"fire","gunType":"Grenade","target":"-2.098999/-0.009998322/0"},{"type":"move","target":"-4.198975/-1.41/0"}]}'

# curl --header "Content-Type: application/json"   --request POST   --data $a0 http://0.0.0.0:5000/submit&

curl --header "Content-Type: application/json"   --request POST   --data $a1 http://0.0.0.0:5000/submit&

curl --header "Content-Type: application/json"   --request POST   --data $a2 http://0.0.0.0:5000/submit&

curl --header "Content-Type: application/json"   --request POST   --data $a3 http://0.0.0.0:5000/submit&
