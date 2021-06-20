a0='{"pindex":"0","data":[{"type":"move","target":"-3.990066/1.572848/-1"},{"type":"fire","gunType":"Grenade","target":"0.1158934/-0.1821193/-1"},{"type":"fire","gunType":"Sword"},{"type":"fire","gunType":"Pistol","target":"1.970199/-2.963576/-1"}]}'
a1='{"pindex":"1","data":[{"type":"move","target":"-3.930463/0.9768212/-1"},{"type":"fire","gunType":"Pistol","target":"0.6456957/2.665563/-1"},{"type":"fire","gunType":"Sword"},{"type":"fire","gunType":"Grenade","target":"2.168875/-0.6463576/-1"}]}'
a2='{"pindex":"2","data":[{"type":"move","target":"-4.930463/0.9768212/-1"},{"type":"fire","gunType":"Pistol","target":"0.8456957/2.665563/-1"},{"type":"fire","gunType":"Sword"},{"type":"fire","gunType":"Grenade","target":"2.168875/-0.5463576/-1"}]}'
a3='{"pindex":"3","data":[{"type":"move","target":"-5.930463/0.9768212/-1"},{"type":"fire","gunType":"Pistol","target":"0.9456957/2.665563/-1"},{"type":"fire","gunType":"Sword"},{"type":"fire","gunType":"Grenade","target":"2.168875/-0.1463576/-1"}]}'

# curl --header "Content-Type: application/json"   --request POST   --data $a0 http://0.0.0.0:5000/submit&

curl --header "Content-Type: application/json"   --request POST   --data $a1 http://0.0.0.0:5000/submit&

curl --header "Content-Type: application/json"   --request POST   --data $a2 http://0.0.0.0:5000/submit&

curl --header "Content-Type: application/json"   --request POST   --data $a3 http://0.0.0.0:5000/submit&
