from flask import Flask, jsonify



app = Flask(__name__)
app.config["DEBUG"] = True


data = {
    "roleplays":[
        {"actions":[
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"}
        ]},
        {"actions":[
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"}
        ]},
        {"actions":[
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"}
        ]},
        {"actions":[
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
            {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"}
        ]}]}

@app.route('/', methods=['GET'])
def home():
    global data
    return jsonify(data)

app.run()