
from flask import Flask, jsonify, render_template, request, redirect, url_for, make_response
import socket, random

app = Flask(__name__)
# app.config["CACHE_TYPE"] = "null"
# app.config["DEBUG"] = True


# data = {
#     "roleplays":[
#         {"actions":[
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Sword/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"MachineGun/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Pistol/9.0/30.0/30.4"}
#         ]},
#         {"actions":[
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Pistol/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"}
#         ]},
#         {"actions":[
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"MachineGun/9.0/30.0/30.4"}
#         ]},
#         {"actions":[
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"},
#             {"type":"fire", "ser":"Grenade/9.0/30.0/30.4"}
#         ]}]}

@app.route('/pdata', methods=['GET'])
def home():
    global data
    return jsonify(data)


@app.route('/submit', methods=['POST'])
def reroute():
    # x = request
    print(request.json['pindex'])
    print(request.json['data'])
    # print(request.stream.read())

    # return('r'+msg[0]+':'+msg[1])
    return("Good")
    # return jsonify(data)


app.run(debug=False, host='127.0.0.1', port=5000) 


